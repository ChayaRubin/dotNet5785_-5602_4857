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

namespace Helpers;

internal static class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get; // stage 4

    public static List<CallInList> GetCallList(IEnumerable<DO.Call> calls)
    {
        var callList = calls.Select(ConvertToBO).ToList();

        var uniqueCalls = callList
            .GroupBy(call => call.Id) 
            .Select(group => group.OrderByDescending(call => call.OpenTime).First()) 
            .ToList();

        return uniqueCalls;
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

            // שליפת שמות כל המתנדבים בבת אחת
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
            MaxEndTime = call.ExpiredTime,
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
            if (callData.ExpiredTime < s_dal.Config.Clock)
                return CallStatus.Expired;

            if (assignmentData.Any(a => a.EndType.ToString() == DO.CallResolutionStatus.Treated.ToString()))
                return CallStatus.Treated;

            if (assignmentData.Any(a => a.EndType == null) &&
                (callData.ExpiredTime - s_dal.Config.Clock <= s_dal.Config.RiskRange &&
                callData.ExpiredTime > s_dal.Config.Clock))
            {
                return CallStatus.InProgressAtRisk;
            }

            if (assignmentData.Any(a => a.EndType == null))
            {
                return CallStatus.InProgress;
            }
            if ((callData.ExpiredTime - s_dal.Config.Clock <= s_dal.Config.RiskRange &&
                callData.ExpiredTime > s_dal.Config.Clock))
            {
                return CallStatus.OpenAtRisk;
            }
            else
            {
                return CallStatus.Open;
            }
        }
    }

    /*public static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        try
        {
            List<int> updatedCallIds = new();

            List<DO.Call> callsToUpdate;
            lock (AdminManager.BlMutex)
            {
                // שליפת קריאות לרשימה קונקרטית
                callsToUpdate = s_dal.Call.ReadAll(c => c.ExpiredTime > AdminManager.Now).ToList();
            }

            foreach (var call in callsToUpdate)
            {
                List<DO.Assignment> allAssignmentsCall;
                lock (AdminManager.BlMutex)
                {
                    allAssignmentsCall = s_dal.Assignment
                        .ReadAll(a => a.CallId == call.RadioCallId && a.FinishCompletionTime == null)
                        .ToList();
                }

                if (!allAssignmentsCall.Any())
                {
                    var newAssignment = new DO.Assignment(
                        0, call.RadioCallId, 0, AdminManager.Now, null, DO.CallResolutionStatus.Expired);

                    lock (AdminManager.BlMutex)
                    {
                        s_dal.Assignment.Create(newAssignment);
                    }
                }
                else
                {
                    var assignmentToUpdate = allAssignmentsCall.First();
                    lock (AdminManager.BlMutex)
                    {
                        s_dal.Assignment.Update(assignmentToUpdate with
                        {
                            FinishCompletionTime = AdminManager.Now,
                            CallResolutionStatus = DO.CallResolutionStatus.Expired
                        });
                    }
                }

                updatedCallIds.Add(call.RadioCallId);
            }

            // מחוץ ל-lock – לשלוח Notify
            foreach (int callId in updatedCallIds.Distinct())
            {
                CallManager.Observers.NotifyItemUpdated(callId);
            }
        }
        catch (BO.BlInvalidInputException e)
        {
            Console.WriteLine($"Error updating periodic calls: {e.Message}"); // Logging error
        }
    }
*/

    /*internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        bool callUpdated = false; //stage 5
        List<DO.Call> expiredCalls;
        lock (AdminManager.BlMutex)
            expiredCalls = s_dal.Call.ReadAll(c => c.ExpiredTime < newClock).ToList();
        if (expiredCalls.Count > 0)
            callUpdated = true;
        expiredCalls.ForEach(call =>
        {
            List<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.RadioCallId).ToList();

            if (!assignments.Any())
            {
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Create(new DO.Assignment(
                        id: 0,
                        callId: call.RadioCallId,
                        volunteerId: 0,
                        entryTime: AdminManager.Now,
                        finishCompletionTime: AdminManager.Now,
                        callResolutionStatus: DO.CallResolutionStatus.Expired
                    ));
            }

            List<DO.Assignment> assignmentsWithNull;
            lock (AdminManager.BlMutex)
                assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.RadioCallId && a.CallResolutionStatus is null).ToList();
            if (assignmentsWithNull.Any())
            {
                assignments.ForEach(assignment =>
                {
                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(assignment with
                        {
                            FinishCompletionTime = AdminManager.Now,
                            CallResolutionStatus = (DO.CallResolutionStatus)BO.CallStatus.Expired
                        });
                    Observers.NotifyItemUpdated(assignment.Id);
                }
                    );
            }
        });
        bool yearChanged = oldClock.Year != newClock.Year; //stage 5
        if (yearChanged || callUpdated) //stage 5
            Observers.NotifyListUpdated(); //stage 5

    }*/

    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        //Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}"; //stage 7 (optional)
        List<DO.Call> expiredCalls;
        List<DO.Assignment> assignments;
        List<DO.Assignment> assignmentsWithNull;
        lock (AdminManager.BlMutex) //stage 7
            expiredCalls = s_dal.Call.ReadAll(c => c.ExpiredTime < newClock).ToList();
        expiredCalls.ForEach(call =>
        {
            lock (AdminManager.BlMutex)
            {//stage 7
                assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.RadioCallId).ToList();
                if (!assignments.Any())
                {
                    s_dal.Assignment.Create(new DO.Assignment(
                        id: 0,
                        callId: call.RadioCallId,
                        volunteerId: 0,
                        entryTime: AdminManager.Now,
                        finishCompletionTime: AdminManager.Now,
                        callResolutionStatus: DO.CallResolutionStatus.SelfCanceled
                    ));
                }
            }
            Observers.NotifyItemUpdated(call.RadioCallId);


            lock (AdminManager.BlMutex) //stage 7
                assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.RadioCallId && a.CallResolutionStatus is null).ToList();
            if (assignmentsWithNull.Any())
            {
                lock (AdminManager.BlMutex) //stage 7
                    foreach (var assignment in assignmentsWithNull)
                    {
                        s_dal.Assignment.Update(assignment with
                        {
                            FinishCompletionTime = AdminManager.Now,
                            CallResolutionStatus = (DO.CallResolutionStatus)BO.CallStatus.SelfCanceled
                        });
                    }

                Observers.NotifyItemUpdated(call.RadioCallId);
            }

        });

    }
}



