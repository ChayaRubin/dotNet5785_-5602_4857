using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;
using System.Net.Mail;
using BlImplementation;
using System.Globalization;
using Avalonia;

namespace Helpers;

internal static class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get; // stage 4

    internal static BO.CallInList CreateCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments, Dictionary<int, string> volunteers)
    {
        var callAssignments = assignments
            .Where(a => a.CallId == call.RadioCallId)
            .OrderByDescending(a => a.EntryTime)
            .ToList();

        var latestAssignment = callAssignments.FirstOrDefault();

        BO.CallStatus status;
        lock (AdminManager.BlMutex)
        {
            status = CallManager.CalculateCallStatus(call.RadioCallId);
        }

        return new BO.CallInList
        {
            Description = call.Description,
            Id = call.RadioCallId,
            Type = (BO.CallTypeEnum)call.CallType,
            OpenTime = call.StartTime,
            Status = status,
            AssignmentId = latestAssignment?.Id ?? 0,
            LastVolunteerName = latestAssignment?.VolunteerId != null &&
                                volunteers.TryGetValue(latestAssignment!.VolunteerId, out var name)
                                ? name : null,
            TotalAssignments = callAssignments.Count,
            MaxEndTime = call.ExpiredTime.HasValue
                ? call.ExpiredTime - AdminManager.Now
                : null,
            completeTime = latestAssignment?.FinishCompletionTime.HasValue == true
                ? latestAssignment.FinishCompletionTime.Value - latestAssignment.EntryTime
                : null,
        };
    }

    /*    internal static BO.CallInList CreateCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments, Dictionary<int, string> volunteers)
        {
            var callAssignments = assignments.Where(a => a.CallId == call.RadioCallId).OrderByDescending(a => a.EntryTime).ToList();
            var latestAssignment = callAssignments.FirstOrDefault();

            return new BO.CallInList
            {
                Description=call.Description,
                Id = call.RadioCallId,
                Type = (BO.CallTypeEnum)call.CallType,
                OpenTime = call.StartTime,
                Status = CallManager.CalculateCallStatus(call.RadioCallId),
                AssignmentId = latestAssignment?.Id ?? 0,
                LastVolunteerName = latestAssignment?.VolunteerId != null && volunteers.TryGetValue(latestAssignment!.VolunteerId, out var name) ? name : null,
                TotalAssignments = callAssignments.Count,
                MaxEndTime = call.ExpiredTime.HasValue
                ? call.ExpiredTime - AdminManager.Now
                : null,
                completeTime = latestAssignment?.FinishCompletionTime.HasValue == true
                ? latestAssignment.FinishCompletionTime.Value - latestAssignment.EntryTime
                : null,

            };
        }*/
    internal static BO.CallStatus CalculateCallStatus(int callId)

    {
        try
        {

            // Get the call from database
            DO.Call call;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
            {
                call = s_dal.Call.Read(c => c.RadioCallId == callId)!;
                if (call == null)
                    throw new ArgumentException($"Call with ID={callId} does not exist.");
                if (AdminManager.Now > call.ExpiredTime)
                    return BO.CallStatus.Expired;
                assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId);
            }
            if (assignments == null)
            {
                TimeSpan timeToExpiration = (DateTime)call.ExpiredTime! - AdminManager.Now;
                if (timeToExpiration <= AdminManager.RiskRange)
                    return BO.CallStatus.OpenAtRisk;

                return BO.CallStatus.Open;
            }

            var activeAssignment = assignments.FirstOrDefault(a => a.FinishCompletionTime == null && a.CallResolutionStatus == null);
            if (activeAssignment == null)
            {
                var successfulAssignment = assignments.Any(a => a.CallResolutionStatus == DO.CallResolutionStatus.Treated);
                return successfulAssignment ? BO.CallStatus.Closed : BO.CallStatus.Open;
            }


            var remainingTime = (DateTime)call.ExpiredTime - AdminManager.Now;
            if (remainingTime <= AdminManager.RiskRange)
                return BO.CallStatus.InProgressAtRisk;

            return BO.CallStatus.InProgress;
        }
        catch (Exception ex)
        {
            throw new BlArgumentException($"Error calculating call status: {ex.Message}");
        }
    }
    public static CallInList ConvertToBO(DO.Call call)
    {
        List<DO.Assignment> allAssignments;
        Dictionary<int, string> volunteerNames;

        lock (AdminManager.BlMutex)
        {
            allAssignments = s_dal.Assignment
                .ReadAll(a => a.CallId == call.RadioCallId)
                .ToList();

            var volunteerIds = allAssignments.Select(a => a.VolunteerId).Distinct().ToList();
            volunteerNames = volunteerIds.ToDictionary(
                id => id,
                id =>
                {
                    try
                    {
                        var vol = s_dal.Volunteer.Read(v => v.Id == id);
                        return vol?.Name ?? "[Unknown]";

                    }
                    catch
                    {
                        return "[Unknown]";
                    }
                });
        }

        var assignmentData = allAssignments.Select(a =>
            new CallAssignInList
            {
                Id = a.Id,
                VolunteerId = a.VolunteerId,
                VolunteerName = volunteerNames.ContainsKey(a.VolunteerId)
                    ? volunteerNames[a.VolunteerId]
                    : "[Unknown]",
                AssignTime = a.EntryTime,
                CompletionTime = a.FinishCompletionTime,
                EndType = a.CallResolutionStatus.HasValue
                    ? Enum.TryParse<CallStatus>(a.CallResolutionStatus.ToString(), out var status) ? status : null
                    : null
            }).ToList();

        return new CallInList
        {
            Id = call.RadioCallId,
            Description = call.Description,
            Type = (CallTypeEnum)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.StartTime,
            //MaxEndTime = call.ExpiredTime.Value,
            Status = GetCallStatus(call, assignmentData),
            Assignments = assignmentData
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

    public static async Task<(double latitude, double longitude)> ValidateCall(BO.Call newCall)
    {
        if (newCall == null)
            throw new Exception("Call object cannot be null");

        if (string.IsNullOrWhiteSpace(newCall.Description))
            throw new Exception("Call description cannot be empty");

        if (string.IsNullOrWhiteSpace(newCall.Address))
            throw new Exception("Call address cannot be empty");

        if (newCall.MaxEndTime <= newCall.OpenTime)
            throw new Exception("Expiration time must be later than start time");

        var coordinates = Tools.GetCoordinatesFromAddress(newCall.Address);
        return coordinates;
    }


    public static DO.Call ConvertToDO(BO.Call boCall)
    {
        return new DO.Call
        {
            RadioCallId = boCall.Id,
            Description = boCall.Description,
            CallType = (DO.CallType)boCall.Type,  
            Address = boCall.Address,
            Latitude = boCall.Latitude,
            Longitude = boCall.Longitude,
            StartTime = boCall.OpenTime,
            ExpiredTime = boCall.MaxEndTime ?? DateTime.Now
        };

    }

    /// <summary>
    /// Validates the logical correctness of volunteer fields, specifically address and coordinates
    /// Ensures the address can be converted to valid coordinates within acceptable ranges
    /// </summary>
    /// <param name="volunteerBO">Volunteer object to validate</param>
    /// <exception cref="DalFormatException">Thrown when validation fails</exception>
    /// ---------------------------------------------------------------------------------------------------------------------------------
    public static void ValidateLogicalFields(BO.Call Call)
    {
        var (latitude, longitude) = Tools.GetCoordinatesFromAddress(Call.Address!);

        if (latitude == 0 && longitude == 0)
        {
            throw new DalCoordinationExceprion("Invalid address format or unable to fetch coordinates.");
        }

        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            throw new DalCoordinationExceprion("Coordinates are out of valid geographic range.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="call"></param>
    /// <param name="assignments"></param>
    /// <returns></returns>
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
            DO.Volunteer volunteer;
            List<DO.Assignment> assignments;
            List<DO.Call> allCalls;

            // נעילה רק סביב קריאות DAL
            lock (AdminManager.BlMutex)
            {
                volunteer = s_dal.Volunteer.Read(v => v.Id == volunteerId)
                    ?? throw new BO.BlNullPropertyException($"Volunteer with ID {volunteerId} not found.");

                assignments = s_dal.Assignment.ReadAll()
                    .Where(a => a.VolunteerId == volunteerId)
                    .ToList();

                allCalls = s_dal.Call.ReadAll().ToList();
            }

            if (!assignments.Any())
                throw new BO.BlDoesNotExistException("No assignments found for the volunteer.");

            var now = AdminManager.Now;

            var calls = from assign in assignments
                        join call in allCalls on assign.CallId equals call.RadioCallId
                        where isOpen
                            ? assign.CallResolutionStatus == null && call.ExpiredTime > now
                            : assign.CallResolutionStatus != null || call.ExpiredTime <= now
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
                    DistanceFromVolunteer = CalculateDistance(
                        volunteer.Latitude, volunteer.Longitude,
                        x.call.Latitude, x.call.Longitude)
                } as T
                : new BO.ClosedCallInList
                {
                    Id = x.call.RadioCallId,
                    Type = (CallTypeEnum)x.call.CallType,
                    Address = x.call.Address,
                    OpenTime = x.call.StartTime,
                    StartTreatmentTime = x.assign.EntryTime,
                    EndTreatmentTime = x.assign.FinishCompletionTime,
                    CompletionType = x.assign.CallResolutionStatus switch
                    {
                        DO.CallResolutionStatus.Treated => CallStatus.Treated,
                        DO.CallResolutionStatus.SelfCanceled => CallStatus.Canceled,
                        DO.CallResolutionStatus.Expired => CallStatus.Expired,
                        _ => null
                    }
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
            throw new BO.BlGeneralDatabaseException($"An unexpected error occurred while fetching calls. Details: {ex.Message}");
        }
    }


    /// <summary>
    /// מחשבת את המרחק בין שתי נקודות גיאוגרפיות.
    /// </summary>
    public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
            throw new ArgumentException("Latitude or Longitude values are null.");

        const double R = 6371; // רדיוס כדור הארץ בקילומטרים
        double lat1Value = lat1.Value;
        double lon1Value = lon1.Value;
        double lat2Value = lat2.Value;
        double lon2Value = lon2.Value;

        double dLat = (lat2Value - lat1Value) * Math.PI / 180;
        double dLon = (lon2Value - lon1Value) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2Value * Math.PI / 180) *
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
    public static bool IsWithinMaxDistance(BO.Volunteer volunteer, BO.Call call)
    {
        const double R = 6371; 

        double lat1 = volunteer.Latitude ?? 0;
        double lon1 = volunteer.Longitude ?? 0;
        double lat2 = call.Latitude;
        double lon2 = call.Longitude;
        double maxDistance = volunteer.MaxDistance ?? 0;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c;

        return distance <= maxDistance;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }


    public static void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587, 
                Credentials = new NetworkCredential("yedidim26@gmail.com", "xtnd teca qkxt hjpl"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("yedidim26@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, 
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
        catch (BlSendingEmailException ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("yedidim26@gmail.com", "xtnd teca qkxt hjpl");
                smtpClient.EnableSsl = true;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("yedidim26@gmail.com");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = false;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            Console.WriteLine($"Email sent successfully to {toEmail}.");
        }
        catch (BlSendingEmailException ex)
        {
            Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
        }
    }

    public static CallStatus GetCallStatus(DO.Call callData, List<BO.CallAssignInList> assignmentData)
    {
        lock (AdminManager.BlMutex)
        {
            if (callData.ExpiredTime < AdminManager.Now)
                return CallStatus.Expired;

            if (assignmentData.Any(a => a.EndType.ToString() == DO.CallResolutionStatus.Treated.ToString()))
                return CallStatus.Treated;

            if (assignmentData.Any(a => a.EndType == null) &&
                (callData.ExpiredTime - AdminManager.Now <= AdminManager.RiskRange &&
                callData.ExpiredTime > AdminManager.Now))
            {
                return CallStatus.InProgressAtRisk;
            }

            if (assignmentData.Any(a => a.EndType == null))
            {
                return CallStatus.InProgress;
            }
            if ((callData.ExpiredTime - AdminManager.Now <= AdminManager.RiskRange &&
                callData.ExpiredTime > AdminManager.Now))
            {
                return CallStatus.OpenAtRisk;
            }
            else
            {
                return CallStatus.Open;
            }
        }
    }
    private static int s_periodicCounter = 0;
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}";

        List<DO.Call> expiredCalls;

        // שלב 1: שלוף את כל הקריאות שפג תוקפן לפי AdminManager.Now
        lock (AdminManager.BlMutex)
        {
            expiredCalls = s_dal.Call
                .ReadAll(c => c.ExpiredTime < AdminManager.Now)
                .ToList();
        }

        foreach (var call in expiredCalls)
        {
            DO.Assignment? openAssignment;

            // שלב 2: בדוק אם יש שיבוץ פתוח לקריאה הזו
            lock (AdminManager.BlMutex)
            {
                openAssignment = s_dal.Assignment
                    .ReadAll(a => a.CallId == call.RadioCallId && a.CallResolutionStatus == null)
                    .FirstOrDefault();
            }

            if (openAssignment is not null)
            {
                // שלב 3: עדכן את השיבוץ כ־Expired
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Update(openAssignment with
                    {
                        FinishCompletionTime = AdminManager.Now,
                        CallResolutionStatus = DO.CallResolutionStatus.Expired
                    });
                }

                CallManager.Observers.NotifyItemUpdated(call.RadioCallId);
            }
        }
    }

}



