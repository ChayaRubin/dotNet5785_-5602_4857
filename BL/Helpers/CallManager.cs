using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; // stage 4

    public static List<CallInList> GetCallList(IEnumerable<DO.Call> calls)
    {
        return calls.Select(ConvertToBO).ToList();
    }

    public static CallInList ConvertToBO(DO.Call call)
    {
        return new CallInList
        {
            Id = call.RadioCallId,
            Description = call.Description,
            Type = (CallTypeEnum)call.CallType,
            Address = call.Address,
            OpenTime = call.StartTime,
            MaxEndTime = call.ExpiredTime
        };
    }

    public static bool FilterCall(DO.Call call, CallField filterByField, object? filterValue)
    {
        return filterByField switch
        {
            CallField.Id => call.RadioCallId.Equals(filterValue),
            CallField.FullName => call.Description?.Equals(filterValue as string) ?? false,
            CallField.TotalHandledCalls => call.StartTime.Equals(filterValue),
            CallField.TotalCanceledCalls => call.ExpiredTime.Equals(filterValue),
            CallField.TotalExpiredCalls => ((CallTypeEnum)call.CallType).Equals(filterValue),
            CallField.CurrentCallId => call.RadioCallId.Equals(filterValue),
            CallField.CurrentCallType => ((CallTypeEnum)call.CallType).Equals(filterValue),
            _ => true
        };
    }

    // בדיקות תקינות על ערכי הקריאה
    public static void ValidateCall(BO.Call newCall)
    {
        if (newCall == null)
            throw new Exception("Call object cannot be null");

        if (string.IsNullOrWhiteSpace(newCall.Description))
            throw new Exception("Call description cannot be empty");
        if (string.IsNullOrWhiteSpace(newCall.Address))
            throw new Exception("Call address cannot be empty");
        if (newCall.MaxEndTime <= newCall.OpenTime)
            throw new Exception("Expiration time must be later than start time");
    }

    // המרה מ-BO.Call ל-DO.Call בצורה מבוקשת
    public static DO.Call ConvertToDO(BO.Call boCall)
    {
        return new DO.Call
        {
            RadioCallId = boCall.Id,
            Description = boCall.Description,
            CallType = (DO.CallType)boCall.Type,  // המרה בין הטיפוסים
            Address = boCall.Address,
            Latitude = boCall.Latitude,
            Longitude = boCall.Longitude,
            StartTime = boCall.OpenTime,
            ExpiredTime = boCall.MaxEndTime ?? DateTime.Now
        };

    }

    public static BO.Call ConvertToBO(DO.Call call, List<CallAssignInList> assignments)
    {
        return new BO.Call
        {
            Id = call.RadioCallId,
            Description = call.Description,
            Type = (CallTypeEnum)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.StartTime,
            MaxEndTime = call.ExpiredTime,
            Assignments = assignments
        };
    }

    /// <summary>
    /// מחזירה רשימה ממיונת של קריאות עבור מתנדב מסוים.
    /// </summary>
    public static IEnumerable<T> GetCallsForVolunteer<T>(int volunteerId, Enum? callType, Enum? sortByField, bool isOpen) where T : class
    {
        try
        {
            var volunteer = s_dal.Volunteer.Read(v => v.Id == volunteerId);
            if (volunteer == null)
                throw new BO.BlNullPropertyException($"Volunteer with ID {volunteerId} not found.");

            var assignments = s_dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId &&
                            (isOpen ? a.CallResolutionStatus == DO.CallResolutionStatus.open || a.CallResolutionStatus == DO.CallResolutionStatus.OpenRisk
                                    : a.CallResolutionStatus == DO.CallResolutionStatus.Closed));


            var calls = from assign in assignments
                        join call in s_dal.Call.ReadAll()
                            on assign.CallId equals call.RadioCallId
                        select new { call, assign };

            if (callType != null)
                calls = calls.Where(x => x.call.CallType.Equals(callType));

            var result = calls.Select(x => isOpen
                ? new BO.OpenCallInList
                {
                    Id = x.call.RadioCallId,
                    CallType = (CallTypeEnum)x.call.CallType,
                    Description = x.call.Description,
                    FullAddress = x.call.Address,
                    OpenTime = x.call.StartTime,
                    MaxCloseTime = x.call.ExpiredTime,
                    DistanceFromVolunteer = CalculateDistance(volunteer.Latitude, volunteer.Longitude, x.call.Latitude, x.call.Longitude)
                } as T
                : new BO.ClosedCallInList
                {
                    Id = x.call.RadioCallId,
                    Type = (CallTypeEnum)x.call.CallType,
                    Address = x.call.Address,
                    OpenTime = x.call.StartTime,
                    StartTreatmentTime = x.assign.EntryTime,
                    EndTreatmentTime = x.assign.FinishCompletionTime,
                    CompletionType = Enum.TryParse<CallStatus>(x.assign.CallResolutionStatus.ToString(), out CallStatus completionStatus)
                                       ? (CallStatus?)completionStatus
                                       : null
                } as T);

            if (sortByField != null)
            {
                var property = typeof(T).GetProperty(sortByField.ToString());
                if (property != null)
                    result = result.OrderBy(call => property.GetValue(call));
            }
            else
            {
                result = isOpen
                    ? result.Cast<BO.OpenCallInList>().OrderBy(call => call.Id).Cast<T>()
                    : result.Cast<BO.ClosedCallInList>().OrderBy(call => call.OpenTime).Cast<T>();
            }

            return result;
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException("Error retrieving calls.");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while fetching calls.");
        }
    }

    /// <summary>
    /// מחשבת את המרחק בין שתי נקודות גיאוגרפיות.
    /// </summary>
    public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        if (lat1 == null || lon1 == null)
            throw new ArgumentException("Latitude or Longitude values are null.");

        const double R = 6371; // רדיוס כדור הארץ בקילומטרים
        double lat1Value = lat1.Value;
        double lon1Value = lon1.Value;
        double dLat = (lat2 - lat1Value) * Math.PI / 180;
        double dLon = (lon2 - lon1Value) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    public static void HandleDalException(Exception ex)
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


