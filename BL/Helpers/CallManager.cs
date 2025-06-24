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
            .GroupBy(call => call.Id)  // קבוצת קריאות לפי ID
            .Select(group => group.OrderByDescending(call => call.OpenTime).First()) // מקבל את הקריאה האחרונה על פי זמן פתיחה
            .ToList();

        return uniqueCalls;
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

        // Call the asynchronous function and get the coordinates
        var coordinates = Tools.GetCoordinatesFromAddress(newCall.Address);
        return coordinates;
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
            var volunteer = s_dal.Volunteer.Read(v => v.Id == volunteerId);
            if (volunteer == null)
                throw new BO.BlNullPropertyException($"Volunteer with ID {volunteerId} not found.");

            var now = DateTime.Now;

            // כל ההקצאות של המתנדב
            var assignments = s_dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId)
                .ToList();

            if (assignments.Count == 0)
                throw new BO.BlDoesNotExistException("No assignments found for the volunteer.");

            // חיבור בין הקריאות להקצאות
            var calls = from assign in assignments
                        join call in s_dal.Call.ReadAll() on assign.CallId equals call.RadioCallId
                        where isOpen
                            ? assign.CallResolutionStatus == null && call.ExpiredTime > now
                            : assign.CallResolutionStatus != null || call.ExpiredTime <= now
                        select new { call, assign };

            // סינון לפי סוג קריאה (אם נבחר)
            if (callType != null)
                calls = calls.Where(x => x.call.CallType.Equals(callType));

            // המרה ל-BL לפי האם פתוחה או סגורה
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
                    EndTreatmentTime = x.assign.FinishCompletionTime.HasValue
                        ? x.assign.FinishCompletionTime.Value
                        : (DateTime?)null,
                    CompletionType = Enum.TryParse<CallStatus>(x.assign.CallResolutionStatus.ToString(), out CallStatus status)
                        ? (CallStatus?)status
                        : null
                } as T);

            // מיון לפי שדה נבחר
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

    public static bool IsWithinMaxDistance(BO.Volunteer volunteer, BO.Call call)
    {
        const double R = 6371; // Earth's radius in km

        // Extract coordinates, ensuring they are not null
        double lat1 = volunteer.Latitude ?? 0;
        double lon1 = volunteer.Longitude ?? 0;
        double lat2 = call.Latitude;
        double lon2 = call.Longitude;

        // Ensure MaxDistance exists and is not null
        double maxDistance = volunteer.MaxDistance ?? 0;

        // Convert degrees to radians
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c; // Distance in km

        // Return whether the call is within the volunteer's max response distance
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
            // הגדרת פרטי השרת
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587, // Port for Gmail
                Credentials = new NetworkCredential("yedidim26@gmail.com", "xtnd teca qkxt hjpl"),
                EnableSsl = true,
            };

            // הגדרת הודעת המייל
            var mailMessage = new MailMessage
            {
                From = new MailAddress("yedidim26@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, // אם אתה רוצה להוסיף HTML במייל, שים true
            };

            // הוספת נמען
            mailMessage.To.Add(toEmail);

            // שליחת המייל
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
        if (callData.ExpiredTime < s_dal.Config.Clock)
            return CallStatus.Expired;

        if (assignmentData.Any(a => a.EndType.ToString() == DO.CallResolutionStatus.Treated.ToString()))
            return CallStatus.Treated;

        /*if (assignmentData.Any() &&
            callData.ExpiredTime - s_dal.Config.Clock <= s_dal.Config.RiskRange &&
            callData.ExpiredTime > s_dal.Config.Clock)
            {
                return CallStatus.InProgressAtRisk;
            }*/
        if (
            callData.ExpiredTime - s_dal.Config.Clock <= s_dal.Config.RiskRange &&
            callData.ExpiredTime > s_dal.Config.Clock)
        {
            return CallStatus.OpenAtRisk;
        }
        else
        {
            return CallStatus.InProgressAtRisk;
        }

    }

}



