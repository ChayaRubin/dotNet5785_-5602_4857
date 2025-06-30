using BlApi;
using BO;
using Dal;
using DO;
using Helpers;
using System.Net.Mail;
using System.Net;
/*using DalApi;*/

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// assign's a call to the volunteerId sent
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="callId"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    /// <exception cref="DalAlreadyExistsException"></exception>
    /// <exception cref="DalArgumentException"></exception>
    public void AssignCall(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(c => c.RadioCallId == callId);
            if (call == null)
                throw new DalDoesNotExistException($"Call with ID {callId} does not exist.");

            var existingAssignment = _dal.Assignment.Read(a => a.CallId == callId && (a.CallResolutionStatus == CallResolutionStatus.Treated));

            if (existingAssignment != null)
                throw new DalAlreadyExistsException($"The call {callId} is already assigned or completed.");

            if (call.ExpiredTime < _dal.Config.Clock)
                throw new DalArgumentException($"Call {callId} has expired.");

            var newAssignment = new Assignment
            {
                Id = Config.NextAssignmentId,
                CallId = callId,
                VolunteerId = volunteerId,
                EntryTime = _dal.Config.Clock,
                FinishCompletionTime = null,
                CallResolutionStatus = null
            };

            _dal.Assignment.Create(newAssignment);
            CallManager.Observers.NotifyListUpdated();  // כמו בשאר המקומות
        }
        catch (Exception ex)
        {
            CallManager.HandleDalException(ex);
        }
    }

    /// <summary>
    /// returns calls by status
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralDatabaseException"></exception>
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

    /// <summary>
    /// returns a certain call's details
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
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

            var volunteerIds = assignmentData
                .Where(a => a.VolunteerId != 0)
                .Select(a => a.VolunteerId)
                .Distinct()
                .ToList();

            var volunteers = _dal.Volunteer.ReadAll(v => volunteerIds.Contains(v.Id))
                .ToDictionary(v => v.Id, v => v.Name);

            List<BO.CallAssignInList> assignmentList = assignmentData.Select(a => new BO.CallAssignInList
            {
                Id = a.Id,
                VolunteerId = a.VolunteerId != 0 ? a.VolunteerId : null,
                VolunteerName = volunteers.GetValueOrDefault(a.VolunteerId),
                AssignTime = a.EntryTime,
                CompletionTime = a.FinishCompletionTime,
                EndType = a.CallResolutionStatus switch
                {
                    DO.CallResolutionStatus.Treated => BO.CallStatus.Treated,
                    DO.CallResolutionStatus.SelfCanceled => BO.CallStatus.Canceled,
                    DO.CallResolutionStatus.Expired => BO.CallStatus.Expired,
                    _ => null
                }
            }).ToList();

            BO.Call callBO = new BO.Call
            {
                Id = callData.RadioCallId,
                Type = (BO.CallTypeEnum)callData.CallType,
                Description = callData.Description,
                Address = callData.Address,
                Latitude = callData.Latitude,
                Longitude = callData.Longitude,
                OpenTime = callData.StartTime,
                MaxEndTime = callData.ExpiredTime,
                Assignments = assignmentList,
                Status = CallManager.GetCallStatus(callData, assignmentList)
            };

            return callBO;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL Exception: {ex.Message}");
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"BL Exception: {ex.Message}");
        }
        catch (DalArgumentException ex)
        {
            throw new BO.BlArgumentException($"BL Exception: {ex.Message}");
        }
        catch (DalFormatException ex)
        {
            throw new BO.BlFormatException($"BL Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// update a certain call's details
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="BO.BlInvalidTimeUnitException"></exception>
    public void UpdateCallDetails(BO.Call call)
    {
        if (call == null)
            throw new DO.DalInvalidTimeUnitException("Call object cannot be null.");

        try
        {
            CallManager.ValidateCall(call);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);

            CallManager.ValidateLogicalFields(call);

            call.Latitude = latitude;
            call.Longitude = longitude;

            DO.Call doCall = CallManager.ConvertToDO(call);
            _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.RadioCallId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DalInvalidTimeUnitException ex)
        {
            throw new BO.BlInvalidTimeUnitException($"BL Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// get the call list
    /// </summary>
    /// <param name="filterByField"></param>
    /// <param name="filterValue"></param>
    /// <param name="sortByField"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlGeneralDatabaseException"></exception>
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
        catch (BlGeneralDatabaseException ex)
        {
            throw new BO.BlGeneralDatabaseException($"An unexpected error occurred while retrieving the call list: {ex.Message}");
        }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            var call = _dal.Call.Read(c => c.RadioCallId == callId)
                ?? throw new DalDoesNotExistException("Call not found.");

            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();

            var boAssignments = assignments.Select(a => new BO.CallAssignInList
            {
                Id = a.Id,
                VolunteerId = a.VolunteerId,
                AssignTime = a.EntryTime,
                CompletionTime = a.FinishCompletionTime,
                EndType = a.CallResolutionStatus.HasValue
                    ? (BO.CallStatus?)Enum.Parse(typeof(BO.CallStatus), a.CallResolutionStatus.ToString())
                    : null
            }).ToList();

            var status = CallManager.GetCallStatus(call, boAssignments);

            if (status != BO.CallStatus.None && assignments.Any())
                throw new BO.BlUnauthorizedAccessException("Can only delete calls that were never assigned and are still in 'None' status.");

            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} not found. {ex.Message}");
        }
        catch (BO.BlUnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while trying to delete the call.");
        }
    }


    /// <summary>
    /// add a call
    /// </summary>
    /// <param name="newCall">the call to add</param>
    public async Task AddCall(BO.Call newCall)
    {
        try
        {
            var coordinates = await CallManager.ValidateCall(newCall);

            newCall.Latitude = coordinates.latitude;
            newCall.Longitude = coordinates.longitude;

            CallManager.ValidateLogicalFields(newCall);

            DO.Call newCallDO = CallManager.ConvertToDO(newCall);
            _dal.Call.Create(newCallDO);
            SendEmailToVolunteers(newCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case DalDoesNotExistException _:
                    throw new BlDoesNotExistException($"BL Exception: {ex.Message}");
                case DalAlreadyExistsException _:
                    throw new BlAlreadyExistsException($"BL Exception: {ex.Message}");
                case DalArgumentException _:
                    throw new BlArgumentException($"BL Exception: {ex.Message}");
                case DalFormatException _:
                    throw new BO.BlFormatException($"BL Exception: {ex.Message}");
                default:
                    throw new BlGeneralDatabaseException($"An unexpected error occurred: {ex.Message}");
            }
        }
    }


    /// <summary>
    /// cancel a call
    /// </summary>
    /// <param name="requestorId">the assignments voluteerId to cancel</param>
    /// <param name="assignmentId">the assignments to cancel</param>
    /// <exception cref="BlUnauthorizedAccessException"></exception>
    /// <exception cref="BlNoPermitionException"></exception>
    /// <exception cref="BlGeneralDatabaseException"></exception>
    public void CancelCall(int requestorId, int assignmentId)
    {
        try
        {
            Console.WriteLine($"Checking Assignment ID: {assignmentId}");
            DO.Assignment assignment = _dal.Assignment.Read(a => a.Id == assignmentId)
                ?? throw new DalDoesNotExistException("The requested assignment does not exist");

            Console.WriteLine($"Assignment found: {assignment.Id}, Volunteer: {assignment.VolunteerId}");

            Console.WriteLine($"Checking Volunteer ID: {assignment.VolunteerId}");
            DO.Volunteer volunteer = _dal.Volunteer.Read(v => v.Id == assignment.VolunteerId)
                ?? throw new DalDoesNotExistException("The volunteer was not found in the system");

            Console.WriteLine($"Volunteer found: {volunteer.Id}, Position: {volunteer.Position}");

            DO.Volunteer M = _dal.Volunteer.Read(v => v.Id == requestorId)
                ?? throw new DalDoesNotExistException("The requestor was not found in the system");
            bool isAdmin = M.Position == DO.PositionEnum.Manager;
            bool isVolunteer = (assignment.VolunteerId == requestorId);

            Console.WriteLine($"Requestor ID: {requestorId}, IsAdmin: {isAdmin}, IsVolunteer: {isVolunteer}");

            if (!isAdmin && !isVolunteer)
                throw new DalNoPermitionException("You do not have permission to cancel this call");

            if (assignment.FinishCompletionTime < DateTime.Now)
                throw new DalGeneralDatabaseException("Cannot cancel a call that has already been closed");

            assignment.EntryTime = AdminManager.Now;

            assignment.CallResolutionStatus = assignment.VolunteerId == requestorId ? DO.CallResolutionStatus.SelfCanceled : DO.CallResolutionStatus.Canceled;

            _dal.Assignment.Update(assignment);
            AssignmentManager.Observers.NotifyItemUpdated(assignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated();  //stage 5
            CallManager.Observers.NotifyListUpdated(); 


            if (isAdmin)
            {
                BO.Call call = GetCallDetails(assignment.CallId);
                DO.Volunteer volunteer1 = _dal.Volunteer.Read(v => v.Id == assignment.VolunteerId) ?? throw new DalDoesNotExistException("The requested volunteer does not exist");
                BO.Volunteer volunteer2 = VolunteerManager.ConvertToBO(volunteer1);
                SendCancellationEmailAsync(call, volunteer2);
            }
            Console.WriteLine("Call cancelled successfully.");
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlUnauthorizedAccessException($"canceling failed: {ex.Message}");
        }
        catch (DalNoPermitionException ex)
        {
            throw new BlNoPermitionException($"canceling failed: {ex.Message}");
        }
        catch (DalGeneralDatabaseException ex)
        {
            throw new BlGeneralDatabaseException($"canceling failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred during canceling the call: {ex.Message}");
        }
    }
   
    public void CloseCall(int volunteerId, int assignmentId)
    {
        try
        {
            string logPath = "log.txt";
            File.AppendAllText(logPath,
                $"[CloseCall CALLED at {DateTime.Now:HH:mm:ss}] VolunteerId={volunteerId}, AssignmentId={assignmentId}\n");
            File.AppendAllText("log.txt", Environment.StackTrace + "\n\n");

            var assignment = _dal.Assignment.Read(a => a.Id == assignmentId)
                ?? throw new DalDoesNotExistException($"Assignment with ID {assignmentId} not found.");

            File.AppendAllText("log.txt", $"FinishCompletionTime: {(assignment.FinishCompletionTime?.ToString() ?? "NULL")}{Environment.NewLine}");


            if (assignment.VolunteerId != volunteerId)
                throw new DalNoPermitionException("Volunteer is not authorized to close this call.");

            if (assignment.FinishCompletionTime.HasValue && assignment.FinishCompletionTime.Value > DateTime.MinValue)
                throw new DalGeneralDatabaseException("This assignment has already been marked as completed.");

            var call = _dal.Call.Read(c => c.RadioCallId == assignment.CallId)
                ?? throw new DalDoesNotExistException($"Call with ID {assignment.CallId} not found.");

            if (call.ExpiredTime < AdminManager.Now)
                throw new DalGeneralDatabaseException("Cannot close a call that has expired.");

            assignment.FinishCompletionTime = AdminManager.Now;
            assignment.CallResolutionStatus = DO.CallResolutionStatus.Treated;

            _dal.Assignment.Update(assignment);
            AssignmentManager.Observers.NotifyItemUpdated(assignment.Id);
            AssignmentManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyListUpdated(); // ← חשוב!

        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Failed to close call: {ex.Message}");
        }
        catch (DalNoPermitionException ex)
        {
            throw new BlNoPermitionException($"Authorization error: {ex.Message}");
        }
        catch (DalGeneralDatabaseException ex)
        {
            throw new BlGeneralDatabaseException($"Business rule error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// returns open calls for specific volunteer
    /// </summary>
    /// <param name="volunteerId">specific volunteer Id</param>
    /// <param name="callType">bool param close or open</param>
    /// <param name="sortByField">enum value to sort by</param>
    /// <returns></returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.OpenCallInList>(volunteerId, callType, sortByField, isOpen: true);
    }

    /// <summary>
    /// returns close calls for specific volunteer
    /// </summary>
    /// <param name="volunteerId">specific volunteer Id</param>
    /// <param name="callType">bool param close or open</param>
    /// <param name="sortByField">enum value to sort by</param>
    /// <returns></returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Enum? callType = null, Enum? sortByField = null)
    {
        return CallManager.GetCallsForVolunteer<BO.ClosedCallInList>(volunteerId, callType, sortByField, isOpen: false);
    }

    /// <summary>
    /// function that Send's Email To Volunteers
    /// </summary>
    /// <param name="newCall">the call that was opened to send</param>
    /// <returns></returns>
    public async Task SendEmailToVolunteers(BO.Call newCall)
    {
        try
        {
            var volunteers = _dal.Volunteer.ReadAll();
            List<Task> emailTasks = new List<Task>();
            int maxConcurrentEmails = 5;

            foreach (var volunteer in volunteers)
            {
                BO.Volunteer boVolunteer = VolunteerManager.ConvertToBO(volunteer);

                if (CallManager.IsWithinMaxDistance(boVolunteer, newCall))
                {
                    string subject = $"New call available: {newCall.Description}";
                    string body = $"A new call has been added.\n" +
                                  $"Description: {newCall.Description}\n" +
                                  $"📍 Location: {newCall.Address}\n" +
                                  $"⚠️ Call Type: {newCall.Type}\n" +
                                  $"⏳ Start Time: {newCall.OpenTime}\n" +
                                  $"🚨 Expires At: {newCall.MaxEndTime}\n" +
                                  $"Please review and take action.";

                    emailTasks.Add(CallManager.SendEmailAsync(boVolunteer.Email, subject, body));

                    if (emailTasks.Count >= maxConcurrentEmails)
                    {
                        await Task.WhenAll(emailTasks);
                        emailTasks.Clear();
                    }
                }
            }

            if (emailTasks.Count > 0)
            {
                await Task.WhenAll(emailTasks);
            }
        }
        catch (BlSendingEmailException ex)
        {
            Console.WriteLine($"Error sending emails: {ex.Message}");
        }
    }

    /// <summary>
    /// func that Send's Cancellation Email
    /// </summary>
    /// <param name="call">tha call that was canceled</param>
    /// <param name="volunteer">the volunteer that is getting the Cancellation Email</param>
    /// <returns></returns>
    public async Task SendCancellationEmailAsync(BO.Call call, BO.Volunteer volunteer)
    {
        try
        {
            var emailTasks = new List<Task>();

            string subject = $"Assignment Cancelled: {call.Description}";
            string body = $"Hello {volunteer.FullName},\n\n" +
                          $"Unfortunately, you are no longer handling this call.\n\n" +
                          $"Call details:\n" +
                          $"Description: {call.Description}\n" +
                          $"📍 Location: {call.Address}\n" +
                          $"⚠️ Call Type: {call.Type}\n" +
                          $"⏳ Start Time: {call.OpenTime}\n" +
                          $"🚨 Expiration Time: {call.MaxEndTime}\n\n" +
                          $"Thank you for your cooperation,\nThe System Team";


            emailTasks.Add(CallManager.SendEmailAsync(volunteer.Email, subject, body));  

            if (emailTasks.Count > 0)
            {
                await Task.WhenAll(emailTasks);
            }
        }
        catch (BlSendingEmailException ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    public int? GetAssignmentId(int callId, int volunteerId)
    {
        var assignment = _dal.Assignment.Read(a => a.CallId == callId && a.VolunteerId == volunteerId);
        return assignment?.Id;
    }

    public double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        return CallManager.CalculateDistance(lat1, lon1, lat2, lon2);
    }
    public IEnumerable<BO.OpenCallInList> GetAvailableOpenCalls(int volunteerId)
    {
        var volunteerDO = _dal.Volunteer.Read(v => v.Id == volunteerId)
            ?? throw new BO.BlNullPropertyException($"Volunteer with ID {volunteerId} not found.");

        var volunteer = new BO.Volunteer
        {
            Id = volunteerDO.Id,
            Latitude = volunteerDO.Latitude,
            Longitude = volunteerDO.Longitude,
            MaxDistance = volunteerDO.MaxResponseDistance,
            CurrentAddress = volunteerDO.Address
        };

        var allAssignments = _dal.Assignment.ReadAll();

        var openCalls = _dal.Call.ReadAll()
            .Where(call =>
                call.ExpiredTime > AdminManager.Now && 
                !allAssignments.Any(a =>
                    a.CallId == call.RadioCallId &&
                    a.CallResolutionStatus == DO.CallResolutionStatus.Treated)) 
            .Select(call => new BO.Call
            {
                Id = call.RadioCallId,
                Description = call.Description,
                Type = (BO.CallTypeEnum)call.CallType,
                Address = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.StartTime,
                MaxEndTime = call.ExpiredTime
            })
            .Where(call => CallManager.IsWithinMaxDistance(volunteer, call))
            .Select(call => new BO.OpenCallInList
            {
                Id = call.Id,
                CallType = call.Type,
                Description = call.Description,
                FullAddress = call.Address,
                OpenTime = call.OpenTime,
                MaxCloseTime = call.MaxEndTime,
                DistanceFromVolunteer = CallManager.CalculateDistance(
                    volunteer.Latitude, volunteer.Longitude,
                    call.Latitude, call.Longitude)
            });

        return openCalls;
    }
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
}

