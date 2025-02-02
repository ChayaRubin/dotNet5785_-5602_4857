/*using BlApi;
using BO;
using Dal;
//using DalApi;\

//using DalApi;
using DO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="callId"></param>
    /// <exception cref="BlDoesNotExistException"></exception>
    /// <exception cref="BlAlreadyExistsException"></exception>
    /// <exception cref="BlArgumentException"></exception>
    /// <exception cref="BlGeneralDatabaseException"></exception>
    public void AssignCall(int volunteerId, int callId)
    {
        try
        {
            // שליפת פרטי הקריאה מה-DAL
            var call = _dal.Call.Read(c => c.RadioCallId == callId);

            // אם הקריאה לא קיימת, זריקת חריגה
            if (call == null)
                throw new DalDoesNotExistException($"Call with ID {callId} does not exist.");

            // בדיקה אם הקריאה כבר טופלה או אם יש הקצאה פתוחה
            var existingAssignment = _dal.Assignment.Read(a => a.Id == callId);

            if (existingAssignment != null)
            {
                // אם יש הקצאה פתוחה או אם הקריאה כבר טופלה, זריקת חריגה
                if (existingAssignment.CallResolutionStatus == CallResolutionStatus.open ||
                    existingAssignment.CallResolutionStatus == CallResolutionStatus.Treated)
                {
                    throw new DalAlreadyExistsException($"The call {callId} is already assigned or completed.");
                }

            }

            // בדיקה אם הקריאה לא פג תוקף
            if (call.ExpiredTime < _dal.Config.Clock)  // השוואה עם השעון המובנה בקונפיג
                throw new DalArgumentException($"Call {callId} has expired.");

            // יצירת הקצאה חדשה
            var newAssignment = new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                EntryTime = _dal.Config.Clock, // שימוש בשעון המובנה בקונפיג
                FinishCompletionTime = null, // לא ידוע עדיין
                CallResolutionStatus = CallResolutionStatus.open // הקריאה בסטטוס פתוח
            };

            // הוספת ההקצאה ל-DAL
            _dal.Assignment.Create(newAssignment);

            Console.WriteLine($"Assignment created for volunteer {volunteerId} and call {callId}");
        }
        catch (DalDoesNotExistException ex)
        {
            // במקרה שהקריאה לא קיימת
            throw new BlDoesNotExistException($"BL Exception: {ex.Message}");
        }
        catch (DalAlreadyExistsException ex)
        {
            // במקרה שכבר קיימת הקצאה
            throw new BlAlreadyExistsException($"BL Exception: {ex.Message}");
        }
        catch (DalArgumentException ex)
        {
            // במקרה שקריאה פגה תוקף
            throw new BlArgumentException($"BL Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            // טיפול כללי בשגיאות
            throw new BlGeneralDatabaseException($"An unexpected error occurred: {ex.Message}");
        }
    }

    public IEnumerable<int> GetCallCountsByStatus()
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            return calls.GroupBy(call => (int)call.CallType)
                        .OrderBy(group => group.Key)
                        .Select(group => group.Count())
                        .ToArray();
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Error retrieving call counts by status");
        }
    }

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            // ניסיון לקבל את נתוני הקריאה מהשכבה הנתונים
            DO.Call? callData = _dal.Call.Read(c => c.RadioCallId == callId);
            if (callData == null)
                throw new BO.BlDoesNotExistException("Call not found");

            // קבלת רשימת ההקצאות עבור הקריאה
            var assignmentData = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId)
                .ToList();

            List<BO.CallAssignInList> assignmentList = assignmentData.Select(a => new BO.CallAssignInList
            {
                AssignTime = a.EntryTime,
                VolunteerId = a.VolunteerId
                // ניתן להוסיף שדות נוספים אם יש צורך
            }).ToList();

            // המרת נתוני הקריאה לישות הלוגית BO.Call, כולל רשימת ההקצאות
            BO.Call callBO = CallManager.ConvertToBO(callData, assignmentList);

            return callBO;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"An unexpected error occurred while retrieving the call details: {ex.Message}");
        }
        catch (DalFormatException ex)
        {
            throw new BO.BlFormatException($"An unexpected error occurred while retrieving the call details: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException($"An unexpected error occurred while retrieving the call details: {ex.Message}");
        }
    }

    public void UpdateCallDetails(BO.Call call)
    {
        if (call == null)
            throw new BO.BlInvalidTimeUnitException("Call object cannot be null.");

        try
        {
            // בדיקות תקינות של הקריאה
            CallManager.ValidateCall(call);

            // קבלת קואורדינטות לפי כתובת
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);
            if (latitude == 0 || longitude == 0)
                throw new BO.BlInvalidTimeUnitException("Invalid address: Unable to retrieve coordinates.");

            // עדכון הקואורדינטות לקריאה
            call.Latitude = latitude;
            call.Longitude = longitude;

            // המרת הקריאה ל-DO.Call
            DO.Call doCall = CallManager.ConvertToDO(call);

            // עדכון הקריאה ב-DAL
            _dal.Call.Update(doCall);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Failed to update call details.");
        }
    }



    public IEnumerable<BO.CallInList> GetCallList(
    CallField? filterByField = null,
    object? filterValue = null,
    CallField? sortByField = null)
    {
        try
        {
            // שליפת הקריאות עם סינון
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll(call =>
                !filterByField.HasValue || CallManager.FilterCall(call, filterByField.Value, filterValue));

            // המרת הקריאות ל-BO
            var callList = CallManager.GetCallList(calls);

            // מיון הרשימה לפי הקריטריון שנבחר
            callList = sortByField.HasValue ? sortByField.Value switch
            {
                CallField.Id => callList.OrderBy(c => c.Id).ToList(),
                CallField.FullName => callList.OrderBy(c => c.Description).ToList(),
                CallField.TotalHandledCalls => callList.OrderByDescending(c => c.OpenTime).ToList(),
                CallField.TotalCanceledCalls => callList.OrderByDescending(c => c.MaxEndTime).ToList(),
                CallField.TotalExpiredCalls => callList.OrderByDescending(c => c.Type).ToList(),
                CallField.CurrentCallId => callList.OrderBy(c => c.Id).ToList(),
                CallField.CurrentCallType => callList.OrderBy(c => c.Type).ToList(),
                _ => callList.OrderBy(c => c.Id).ToList()
            } : callList.OrderBy(c => c.Id).ToList();

            // החזרת הרשימה הממוינת
            return callList;
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred while retrieving the call list: {ex.Message}");
        }
    }


    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call exists in the data layer
            var call = _dal.Call.Read(c => c.RadioCallId == callId);

            if (call == null)
            {
                throw new DalDoesNotExistException("Call not found.");
            }

            // Check if the call is assigned to any volunteer
            DO.Assignment? assignment = _dal.Assignment.Read(a => a.CallId == callId);

            if (assignment != null)
            {
                // If the call has been assigned, it cannot be deleted
                throw new BO.BlNullPropertyException("Cannot delete a call that has been assigned.");
            }

            // Check if there are any open status assignments
            var openAssignments = _dal.Assignment.Read(a => a.CallId == callId && a.CallResolutionStatus == DO.CallResolutionStatus.open);

            if (openAssignments != null)
            {
                throw new BO.BlInvalidTimeUnitException("Only open calls can be deleted.");
            }

            // Perform the deletion of the call
            _dal.Call.Delete(callId);
        }
        catch (DO.DalFormatException ex)
        {
            throw new BO.BlNullPropertyException("An error occurred while deleting the call.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Call not found in the database.");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Error deleting the call.");
        }
    }

    public void AddCall(BO.Call newCall)
    {
        try
        {
            // בודקים אם ערכי הקריאה תקינים
            CallManager.ValidateCall(newCall);

            // ממירים את BO.Call ל-DO.Call
            DO.Call newCallDO = CallManager.ConvertToDO(newCall);

            // מנסים להוסיף את הקריאה החדשה לשכבת הנתונים
            _dal.Call.Create(newCallDO);
        }
        catch (Exception ex)
        {
            // במקרה של בעיה, נזרוק חריגה מתאימה
            throw new BlGeneralDatabaseException("Error adding call to database");
        }
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.OpenCallInList>(volunteerId, callType, sortByField, isOpen: true);
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.ClosedCallInList>(volunteerId, callType, sortByField, isOpen: false);
    }

    /// <summary>
    /// Calculates the distance between two geographic coordinates.
    /// </summary>
    private double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        // אם אחד מהערכים lat1 או lon1 הוא null, תחזיר 0 או ערך ברירת מחדל אחר
        if (lat1 == null || lon1 == null)
        {
            throw new ArgumentException("Latitude or Longitude values are null.");
        }

        const double R = 6371; // Earth's radius in km

        // המר את הערכים של lat1 ו-lon1 אם הם לא null
        double lat1Value = lat1.Value;
        double lon1Value = lon1.Value;

        // חישוב המרחק
        double dLat = (lat2 - lat1Value) * Math.PI / 180;
        double dLon = (lon2 - lon1Value) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // המרחק בקילומטרים
    }


}
*/

using BlApi;
using BO;
using Dal;
using DO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AssignCall(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(c => c.RadioCallId == callId);

            if (call == null)
                throw new DalDoesNotExistException($"Call with ID {callId} does not exist.");

            var existingAssignment = _dal.Assignment.Read(a => a.Id == callId);
            if (existingAssignment != null &&
                (existingAssignment.CallResolutionStatus == CallResolutionStatus.open ||
                existingAssignment.CallResolutionStatus == CallResolutionStatus.Treated))
            {
                throw new DalAlreadyExistsException($"The call {callId} is already assigned or completed.");
            }

            if (call.ExpiredTime < _dal.Config.Clock)
                throw new DalArgumentException($"Call {callId} has expired.");

            var newAssignment = new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                EntryTime = _dal.Config.Clock,
                FinishCompletionTime = null,
                CallResolutionStatus = CallResolutionStatus.open
            };

            _dal.Assignment.Create(newAssignment);
            Console.WriteLine($"Assignment created for volunteer {volunteerId} and call {callId}");
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
        }
    }

    public IEnumerable<int> GetCallCountsByStatus()
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            return calls.GroupBy(call => (int)call.CallType)
                        .OrderBy(group => group.Key)
                        .Select(group => group.Count())
                        .ToArray();
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Error retrieving call counts by status");
        }
    }

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call? callData = _dal.Call.Read(c => c.RadioCallId == callId);
            if (callData == null)
                throw new BO.BlDoesNotExistException("Call not found");

            var assignmentData = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId)
                .ToList();

            List<BO.CallAssignInList> assignmentList = assignmentData.Select(a => new BO.CallAssignInList
            {
                AssignTime = a.EntryTime,
                VolunteerId = a.VolunteerId
            }).ToList();

            BO.Call callBO = CallManager.ConvertToBO(callData, assignmentList);
            return callBO;
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
            throw;
        }
    }

    public void UpdateCallDetails(BO.Call call)
    {
        if (call == null)
            throw new BO.BlInvalidTimeUnitException("Call object cannot be null.");

        try
        {
            CallManager.ValidateCall(call);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);
            if (latitude == 0 || longitude == 0)
                throw new BO.BlInvalidTimeUnitException("Invalid address: Unable to retrieve coordinates.");

            call.Latitude = latitude;
            call.Longitude = longitude;

            DO.Call doCall = CallManager.ConvertToDO(call);
            _dal.Call.Update(doCall);
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
        }
    }

    public IEnumerable<BO.CallInList> GetCallList(
        CallField? filterByField = null,
        object? filterValue = null,
        CallField? sortByField = null)
    {
        try
        {
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll(call =>
                !filterByField.HasValue || CallManager.FilterCall(call, filterByField.Value, filterValue));

            var callList = CallManager.GetCallList(calls);

            callList = sortByField.HasValue ? sortByField.Value switch
            {
                CallField.Id => callList.OrderBy(c => c.Id).ToList(),
                CallField.FullName => callList.OrderBy(c => c.Description).ToList(),
                CallField.TotalHandledCalls => callList.OrderByDescending(c => c.OpenTime).ToList(),
                CallField.TotalCanceledCalls => callList.OrderByDescending(c => c.MaxEndTime).ToList(),
                CallField.TotalExpiredCalls => callList.OrderByDescending(c => c.Type).ToList(),
                CallField.CurrentCallId => callList.OrderBy(c => c.Id).ToList(),
                CallField.CurrentCallType => callList.OrderBy(c => c.Type).ToList(),
                _ => callList.OrderBy(c => c.Id).ToList()
            } : callList.OrderBy(c => c.Id).ToList();

            return callList;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException($"An unexpected error occurred while retrieving the call list: {ex.Message}");
        }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            var call = _dal.Call.Read(c => c.RadioCallId == callId);

            if (call == null)
            {
                throw new DalDoesNotExistException("Call not found.");
            }

            DO.Assignment? assignment = _dal.Assignment.Read(a => a.CallId == callId);

            if (assignment != null)
            {
                throw new BO.BlNullPropertyException("Cannot delete a call that has been assigned.");
            }

            var openAssignments = _dal.Assignment.Read(a => a.CallId == callId && a.CallResolutionStatus == DO.CallResolutionStatus.open);

            if (openAssignments != null)
            {
                throw new BO.BlInvalidTimeUnitException("Only open calls can be deleted.");
            }

            _dal.Call.Delete(callId);
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
        }
    }

    public void AddCall(BO.Call newCall)
    {
        try
        {
            CallManager.ValidateCall(newCall);

            DO.Call newCallDO = CallManager.ConvertToDO(newCall);
            _dal.Call.Create(newCallDO);
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
        }
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.OpenCallInList>(volunteerId, callType, sortByField, isOpen: true);
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.ClosedCallInList>(volunteerId, callType, sortByField, isOpen: false);
    }


}
