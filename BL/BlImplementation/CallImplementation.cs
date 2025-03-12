using BlApi;
using BO;
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

            var existingAssignment = _dal.Assignment.Read(a => a.CallId == callId &&
                (a.CallResolutionStatus == CallResolutionStatus.open || a.CallResolutionStatus == CallResolutionStatus.Treated));

            if (existingAssignment != null)
                throw new DalAlreadyExistsException($"The call {callId} is already assigned or completed.");

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
            //var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);
            /*if (latitude == 0 || longitude == 0)
                throw new BO.BlInvalidTimeUnitException("Invalid address: Unable to retrieve coordinates.");

            call.Latitude = latitude;
            call.Longitude = longitude;*/

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
                throw new BlDoesNotExistException("Call not found.");

            var openAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId && a.CallResolutionStatus == DO.CallResolutionStatus.open);

            if (openAssignments.Any())
                throw new BO.BlInvalidTimeUnitException("Cannot delete a call that is still open.");

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

    public void CancelCall(int requestorId, int assignmentId)
    {
        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(a => a.Id == assignmentId)
                ?? throw new DalDoesNotExistException("The requested assignment does not exist");

            DO.Volunteer volunteer = _dal.Volunteer.Read(v => v.Id == assignment.VolunteerId)
                ?? throw new DalDoesNotExistException("The volunteer was not found in the system");

            bool isAdmin = volunteer.Position == DO.PositionEnum.Manager;
            bool isVolunteer = assignment.VolunteerId == requestorId;

            if (!isAdmin && !isVolunteer)
                throw new DalNoPermitionException("You do not have permission to cancel this call");

            if (assignment.FinishCompletionTime != null || assignment.EntryTime < DateTime.Now)
                throw new DalGeneralDatabaseException("Cannot cancel a call that has already been closed");

            assignment.EntryTime = ClockManager.Now;
            _dal.Assignment.Update(assignment);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlUnauthorizedAccessException($"Login failed: {ex.Message}");
        }
        catch (DalNoPermitionException ex)
        {
            throw new BlNoPermitionException($"Login failed: {ex.Message}");
        }
        catch (DalGeneralDatabaseException ex)
        {
            throw new BlGeneralDatabaseException($"Login failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred during login: {ex.Message}");
        }
    }

    public void CloseCall(int volunteerId, int assignmentId)
    {
        try
        {
            var existingAssignment = _dal.Assignment.Read(a => a.Id == assignmentId)
                ?? throw new DalDoesNotExistException($"Assignment with ID {assignmentId} not found");

            if (existingAssignment.VolunteerId != volunteerId)
                throw new DalNoPermitionException("Volunteer is not authorized to close this call");

            if (existingAssignment.FinishCompletionTime != null)
                throw new DalGeneralDatabaseException("Call has already been closed or expired");

            existingAssignment.FinishCompletionTime = ClockManager.Now;
            existingAssignment.CallResolutionStatus = DO.CallResolutionStatus.Closed;
            _dal.Assignment.Update(existingAssignment);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlUnauthorizedAccessException($"Login failed: {ex.Message}");
        }
        catch (DalNoPermitionException ex)
        {
            throw new BlNoPermitionException($"Login failed: {ex.Message}");
        }
        catch (DalGeneralDatabaseException ex)
        {
            throw new BlGeneralDatabaseException($"Login failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred during login: {ex.Message}");
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
