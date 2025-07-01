using DalApi;
using BO;
using System.Data;
using DO;
using BlApi;
using System.Text;
namespace Helpers;
using System.Security.Cryptography;
using System;
using BlImplementation;

internal static class VolunteerManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = DalApi.Factory.Get; //stage 4

    /// <summary>
    /// Converts a collection of DO.Volunteer objects to a list of BO.Volunteer objects
    /// by utilizing the ConvertToBO function for each volunteer in the collection
    /// </summary>
    /// <param name="volunteers">Collection of DO.Volunteer objects to convert</param>
    /// <returns>List of converted BO.Volunteer objects</returns>
    public static List<BO.VolunteerInList> GetVolunteerList(IEnumerable<BO.Volunteer> volunteers)
    {
        return volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalHandledCalls = v.TotalCallsHandled,
            TotalCanceledCalls = v.TotalCallsCanceled,
            TotalExpiredCalls = v.TotalExpiredCalls,
            CurrentCallId = v.CurrentCall?.CallId,
            CurrentCallType = v.CurrentCall?.CallType ?? BO.CallTypeEnum.None
        }).ToList();
    }


    /// <summary>
    /// Converts a single DO.Volunteer object to a BO.Volunteer object
    /// with initialized statistics (calls handled, canceled, expired) set to 0
    /// </summary>
    /// <param name="volunteer">DO.Volunteer object to convert</param>
    /// <returns>Converted BO.Volunteer object</returns>
    public static BO.Volunteer ConvertToBO(DO.Volunteer volunteer)
    {
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            FullName = volunteer.Name,
            PhoneNumber = volunteer.Phone,
            Email = volunteer.Email,
            Password = volunteer.Password,
            CurrentAddress = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.PositionEnum)volunteer.Position,
            IsActive = volunteer.Active,
            MaxDistance = volunteer.MaxResponseDistance,
            TypeOfDistance = (BO.DistanceType)volunteer.TypeOfDistance,
/*            TotalCallsHandled = 0,
            TotalCallsCanceled = 0,
            TotalCallsExpired = 0,*/
        };
    }

    /// <summary>
    /// Converts a BO.Volunteer object to a DO.Volunteer object
    /// Note: This conversion may lose some information as DO objects have fewer fields
    /// </summary>
    /// <param name="volunteer">BO.Volunteer object to convert</param>
    /// <returns>Converted DO.Volunteer object</returns>
    public static DO.Volunteer ConvertToDO(BO.Volunteer volunteer)
    {
        return new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.FullName,
            Phone = volunteer.PhoneNumber,
            Email = volunteer.Email,
            //Password = HashPassword(GenerateStrongPassword()),
            Password = volunteer.Password,
            Address = volunteer.CurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Position = (DO.PositionEnum)volunteer.Role,
            Active = volunteer.IsActive,
            MaxResponseDistance = volunteer.MaxDistance,
            TypeOfDistance = (DO.DistanceTypeEnum)volunteer.TypeOfDistance
        };
    }

    /// <summary>
    /// Checks if the requester is authorized to access/modify the volunteer's information
    /// Authorization is granted if the requester is either the volunteer themselves or an admin
    /// </summary>
    /// <param name="requesterId">ID of the person making the request</param>
    /// <param name="volunteer">Volunteer object being accessed</param>
    /// <returns>True if requester is authorized, false otherwise</returns>
    public static bool IsRequesterAuthorized(string requesterId, BO.Volunteer volunteer)
    {
        return requesterId == volunteer.Id.ToString() || IsAdmin(requesterId, volunteer);
    }

    /// <summary>
    /// Verifies if the requester has admin privileges for modifying volunteer information
    /// Throws exception if requester is not authorized or attempting unauthorized role changes
    /// </summary>
    /// <param name="requesterId">ID of the person making the request</param>
    /// <param name="boVolunteer">Volunteer object being modified</param>
    /// <returns>True if admin access is granted</returns>
    /// <exception cref="DalUnauthorizedAccessException">Thrown when access is not authorized</exception>
    private static bool IsAdmin(string requesterId, BO.Volunteer boVolunteer)
    {
        if (!(requesterId == boVolunteer.Id.ToString()) && boVolunteer.Role != BO.PositionEnum.Manager)
        {
            throw new DalUnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");
        }
        return true;
    }

    /// <summary>
    /// Validates if the requester has permission to update specific volunteer fields
    /// Checks both authorization and field-specific update permissions
    /// </summary>
    /// <param name="requesterId">ID of the person making the request</param>
    /// <param name="existingVolunteer">Current volunteer data in the system</param>
    /// <param name="volunteerToUpdate">New volunteer data to be updated</param>
    /// <returns>True if updates are allowed, false otherwise</returns>
    /// <exception cref="DalUnauthorizedAccessException">Thrown when update permission is denied</exception>
    public static bool CanUpdateFields(string requesterId, DO.Volunteer existingVolunteer, BO.Volunteer volunteerToUpdate)
    {
        if (!int.TryParse(requesterId, out int requesterIdInt))
            throw new ArgumentException($"Invalid requester ID format: {requesterId}");

        DO.Volunteer requester;
        int currentManagers;

        lock (AdminManager.BlMutex)
        {
            requester = s_dal.Volunteer.Read(v => v.Id == requesterIdInt)
                ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={requesterId} does not exist.");

            currentManagers = s_dal.Volunteer
                .ReadAll(v => v.Position.ToString() == "Manager" && v.Active)
                .Count();
        }

        // רק המתנדב עצמו או מנהל יכולים לעדכן פרטים
        if (requesterId != existingVolunteer.Id.ToString() &&
            requester.Position.ToString() != BO.PositionEnum.Manager.ToString())
        {
            return false;
        }

        // מניעת הסרת המנהל האחרון
        if (existingVolunteer.Position == DO.PositionEnum.Manager &&
            volunteerToUpdate.Role != BO.PositionEnum.Manager &&
            currentManagers <= 1)
        {
            throw new BO.BlNoPermitionException("Cannot remove the only active manager");
        }

        return true;
    }



    /// <summary>
    /// Validates the logical correctness of volunteer fields, specifically address and coordinates
    /// Ensures the address can be converted to valid coordinates within acceptable ranges
    /// </summary>
    /// <param name="volunteerBO">Volunteer object to validate</param>
    /// <exception cref="DalFormatException">Thrown when validation fails</exception>
    /// ---------------------------------------------------------------------------------------------------------------------------------
    public static void ValidateLogicalFields(BO.Volunteer volunteerBO)
    {
        var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);

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
    /// Validates the format of all input fields in a volunteer object
    /// Checks ID number, address, phone number, email, and password format
    /// </summary>
    /// <param name="volunteer">Volunteer object to validate</param>
    /// <exception cref="DalFormatException">Thrown when any validation fails</exception>
    public static (double latitude, double longitude) ValidateInputFormat(BO.Volunteer volunteer)
    {
        if (!IsValidIdNumber(volunteer.Id.ToString()))
            throw new DalFormatException("Invalid ID number.");

        if (string.IsNullOrWhiteSpace(volunteer.CurrentAddress))
            throw new DalFormatException("Address cannot be empty.");

        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
            throw new DalFormatException("Invalid phone number.");

        if (!IsValidEmail(volunteer.Email))
            throw new DalFormatException("Invalid email.");

        if (!string.IsNullOrWhiteSpace(volunteer.Password))
        {
            if (volunteer.Password.Length < 30)
            {
                if (!IsValidPassword(volunteer.Password))
                    throw new DalFormatException("Password is not strong enough.");
            }
        }

        int currentManagers;
        lock (AdminManager.BlMutex)
        {
            currentManagers = s_dal.Volunteer
                .ReadAll(v => v.Position.ToString() == "Manager" && v.Active)
                .Count();
        }

        if (volunteer.Role.ToString() == DO.PositionEnum.Manager.ToString() &&
            volunteer.Role != BO.PositionEnum.Manager &&
            currentManagers <= 1)
        {
            throw new BO.BlNoPermitionException("Cannot remove the only active manager");
        }

        var coordinates = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress);
        volunteer.Latitude = coordinates.latitude;
        volunteer.Longitude = coordinates.longitude;

        return coordinates;
    }




    /// <summary>
    /// Validates an Israeli ID number using the check digit algorithm
    /// Ensures the ID is 9 digits and matches the checksum calculation
    /// </summary>
    /// <param name="idNumber">ID number to validate</param>
    /// <returns>True if ID is valid, false otherwise</returns>
    public static bool IsValidIdNumber(string idNumber)
    {
        // בדיקת אורך וסוג התו (האם כל התו הוא ספרה)
        if (idNumber.Length != 9 || !idNumber.All(char.IsDigit))
        {
            return false;
        }

        // המרת המספר למערך של ספרות
        int[] digits = new int[9];
        for (int i = 0; i < 9; i++)
        {
            digits[i] = idNumber[i] - '0'; // המרת תו לספרה
        }

        // חישוב סכום לפי אלגוריתם מספר הזהות
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int multiplier = (i % 2 == 0) ? 1 : 2;
            int product = digits[i] * multiplier;
            sum += product > 9 ? product - 9 : product;
        }

        // חישוב ספרת ביקורת
        int checkDigit = (10 - sum % 10) % 10;

        // בדיקה אם ספרת הביקורת נכונה
        return checkDigit == digits[8];
    }


    /// <summary>
    /// Validates a phone number format
    /// Currently checks if the number is exactly 10 digits long and contains only digits
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if phone number is valid, false otherwise</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        phoneNumber = phoneNumber.Replace(" ", "").Trim();
        return phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit);
    }

    /// <summary>
    /// Validates an email address format
    /// Checks for the presence of @ and . symbols in correct order
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if email format is valid, false otherwise</returns>
    public static bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".") && email.IndexOf("@") < email.LastIndexOf(".");
    }

    /// <summary>
    /// Validates password strength
    /// Ensures password meets minimum requirements: 8+ chars, lowercase, uppercase,
    /// numbers, and special characters
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <returns>True if password meets strength requirements, false otherwise</returns>
    public static bool IsValidPassword(string password)
    {
        // More detailed password requirements
        var hasMinLength = password.Length >= 8;
        var hasLower = password.Any(char.IsLower);
        var hasUpper = password.Any(char.IsUpper);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecialChar = password.Any(c => "!@#$%^&*()_+[]{}|;:',.<>?".Contains(c));

        return hasMinLength && hasLower && hasUpper && hasDigit && hasSpecialChar;
    }

    /// <summary>
    /// Hashes the given password using the SHA256 algorithm and returns the hashed value as a Base64 string.
    /// </summary>
    /// <param name="password">The plain text password to be hashed.</param>
    /// <returns>A Base64-encoded string representation of the hashed password.</returns>
    public static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // המרת הסיסמה ל-בייטים
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // המרת הבייטים למחרוזת Hex
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Verifies if the provided plain text password matches the stored hashed password.
    /// </summary>
    /// <param name="inputPassword">The plain text password to verify.</param>
    /// <param name="storedHash">The previously stored hashed password for comparison.</param>
    /// <returns>True if the input password matches the stored hash, otherwise false.</returns>
    public static bool VerifyPassword(string inputPassword, string storedHash)
    {
        var hashedInput = HashPassword(inputPassword);
        //var hashedInput = inputPassword;
        return hashedInput == storedHash;
    }

    public static BO.CallStatus CalculateCallStatus(int callId)
    {
        try
        {
            DO.Call? call;
            List<DO.Assignment> assignments;

            lock (AdminManager.BlMutex)
            {
                call = s_dal.Call.Read(c => c.RadioCallId == callId);
                if (call == null)
                    throw new ArgumentException($"Call with ID={callId} does not exist.");

                assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
            }

            if (!assignments.Any())
            {
                if (AdminManager.Now > call.ExpiredTime)
                    return BO.CallStatus.Expired;

                TimeSpan timeToExpiration = (DateTime)call.ExpiredTime - AdminManager.Now;
                if (timeToExpiration <= AdminManager.RiskRange)
                    return BO.CallStatus.OpenAtRisk;

                return BO.CallStatus.Open;
            }

            var activeAssignment = assignments.FirstOrDefault(a => a.FinishCompletionTime == null);
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
            throw new BlSendingEmailException($"Error calculating call status: {ex.Message}");
        }
    }
    /*internal static void SimulateVolunteerAssignmentsAndCallHandling()
    {
        Thread.CurrentThread.Name = $"Simulator{Thread.CurrentThread.ManagedThreadId}";

        List<int> updatedVolunteerIds = new();
        List<int> updatedCallIds = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = DalApi.Factory.Get.Volunteer.ReadAll(v => v.Active).ToList(); // ToList מיד
        }

        foreach (var volunteer in activeVolunteers)
        {
            DO.Assignment? currentAssignment;
            lock (AdminManager.BlMutex)
            {
                currentAssignment = DalApi.Factory.Get.Assignment
                    .ReadAll(a => a.VolunteerId == volunteer.Id && a.FinishCompletionTime == null)
                    .FirstOrDefault();
            }

            if (currentAssignment == null)
            {
                List<BO.OpenCallInList> openCalls = new CallImplementation().GetAvailableOpenCalls(volunteer.Id).ToList();



                if (!openCalls.Any() || Random.Shared.NextDouble() > 0.2) continue;

                var selectedCall = openCalls[Random.Shared.Next(openCalls.Count)];
                try
                {
                    new CallImplementation().AssignCall(volunteer.Id, selectedCall.Id);
                    updatedVolunteerIds.Add(volunteer.Id);
                    updatedCallIds.Add(selectedCall.Id);
                }
                catch { continue; }
            }
            else
            {
                DO.Call? call;
                lock (AdminManager.BlMutex)
                {
                    call = DalApi.Factory.Get.Call.Read(c => c.RadioCallId == currentAssignment.CallId);
                }

                if (call is null) continue;

                double distance = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, call.Latitude, call.Longitude);
                TimeSpan baseTime = TimeSpan.FromMinutes(distance * 2);
                TimeSpan extra = TimeSpan.FromMinutes(Random.Shared.Next(1, 5));
                TimeSpan totalNeeded = baseTime + extra;
                TimeSpan actual = AdminManager.Now - currentAssignment.EntryTime;

                if (actual >= totalNeeded)
                {
                    try
                    {
                        new CallImplementation().CloseCall(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.RadioCallId);
                    }
                    catch { continue; }
                }
                else if (Random.Shared.NextDouble() < 0.1)
                {
                    try
                    {
                        new CallImplementation().CancelCall(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.RadioCallId);
                    }
                    catch { continue; }
                }
            }
        }

        foreach (var id in updatedVolunteerIds.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);

        foreach (var id in updatedCallIds.Distinct())
            CallManager.Observers.NotifyItemUpdated(id);
    }*/
    internal static void SimulateVolunteerAssignmentsAndCallHandling()
    {
        Thread.CurrentThread.Name = $"Simulator{Thread.CurrentThread.ManagedThreadId}";

        List<int> updatedVolunteerIds = new();
        List<int> updatedCallIds = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = DalApi.Factory.Get.Volunteer.ReadAll(v => v.Active).ToList();
        }

        foreach (var volunteer in activeVolunteers)
        {
            DO.Assignment? currentAssignment;
            lock (AdminManager.BlMutex)
            {
                currentAssignment = DalApi.Factory.Get.Assignment
                    .ReadAll(a => a.VolunteerId == volunteer.Id && a.FinishCompletionTime == null)
                    .FirstOrDefault();
            }

            if (currentAssignment == null)
            {
                List<BO.OpenCallInList> openCalls = new CallImplementation().GetAvailableOpenCalls(volunteer.Id).ToList();

                if (!openCalls.Any() || Random.Shared.NextDouble() > 0.5) continue;

                var selectedCall = openCalls[Random.Shared.Next(openCalls.Count)];
                try
                {
                    new CallImplementation().AssignCall(volunteer.Id, selectedCall.Id);
                    updatedVolunteerIds.Add(volunteer.Id);
                    updatedCallIds.Add(selectedCall.Id);
                }
                catch { continue; }
            }
            else
            {
                DO.Call? call;
                lock (AdminManager.BlMutex)
                {
                    call = DalApi.Factory.Get.Call.Read(c => c.RadioCallId == currentAssignment.CallId);
                }

                if (call is null) continue;

                double distance = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, call.Latitude, call.Longitude);
                TimeSpan baseTime = TimeSpan.FromMinutes(distance * 2);
                TimeSpan extra = TimeSpan.FromMinutes(Random.Shared.Next(1, 5));
                TimeSpan totalNeeded = baseTime + extra;
                TimeSpan actual = AdminManager.Now - currentAssignment.EntryTime;

                double r = Random.Shared.NextDouble();
                if (r < 0.4)
                {
                    try
                    {
                        new CallImplementation().CancelCall(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.RadioCallId);
                    }
                    catch { continue; }
                }
                else if (r < 0.8)
                {
                    try
                    {
                        new CallImplementation().CloseCall(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.RadioCallId);
                    }
                    catch { continue; }
                }
                // else: 20% לא קורה כלום
            }
        }

        foreach (var id in updatedVolunteerIds.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);

        foreach (var id in updatedCallIds.Distinct())
            CallManager.Observers.NotifyItemUpdated(id);
    }

    // הבעיות שזיהיתי ותיקונן:

    /*internal static void SimulateVolunteerAssignmentsAndCallHandling()
    {
        Thread.CurrentThread.Name = $"Simulator{Thread.CurrentThread.ManagedThreadId}";

        List<int> updatedVolunteerIds = new();
        List<int> updatedCallIds = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = DalApi.Factory.Get.Volunteer.ReadAll(v => v.Active).ToList();
        }

        // הוספת לוג לבדיקה
        Console.WriteLine($"Found {activeVolunteers.Count} active volunteers");

        foreach (var volunteer in activeVolunteers)
        {
            try
            {
                DO.Assignment? currentAssignment;
                lock (AdminManager.BlMutex)
                {
                    currentAssignment = DalApi.Factory.Get.Assignment
                        .ReadAll(a => a.VolunteerId == volunteer.Id && a.FinishCompletionTime == null)
                        .FirstOrDefault();
                }

                if (currentAssignment == null)
                {
                    // בעיה 1: השימוש ב-CallImplementation ללא ממשק עלול לגרום לבעיות
                    // בעיה 2: אין הדפסת שגיאות להבנת מה קורה

                    List<BO.OpenCallInList> openCalls;
                    try
                    {
                        // במקום CallImplementation ישירות, עדיף להשתמש בממשק
                        var callImplementation = new CallImplementation();
                        openCalls = callImplementation.GetAvailableOpenCalls(volunteer.Id).ToList();

                        Console.WriteLine($"Volunteer {volunteer.Id} has {openCalls.Count} available calls");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting open calls for volunteer {volunteer.Id}: {ex.Message}");
                        continue;
                    }

                    // בעיה 3: הסיכוי של 0.2 (20%) עלול להיות נמוך מדי לבדיקה
                    // העלה ל-50% או יותר לבדיקה
                    if (!openCalls.Any() || Random.Shared.NextDouble() > 0.5) // שונה מ-0.2 ל-0.5
                    {
                        if (!openCalls.Any())
                            Console.WriteLine($"No open calls for volunteer {volunteer.Id}");
                        else
                            Console.WriteLine($"Random skip assignment for volunteer {volunteer.Id}");
                        continue;
                    }

                    var selectedCall = openCalls[Random.Shared.Next(openCalls.Count)];
                    try
                    {
                        Console.WriteLine($"Attempting to assign call {selectedCall.Id} to volunteer {volunteer.Id}");

                        var callImplementation = new CallImplementation();
                        callImplementation.AssignCall(volunteer.Id, selectedCall.Id);

                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(selectedCall.Id);

                        Console.WriteLine($"Successfully assigned call {selectedCall.Id} to volunteer {volunteer.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to assign call {selectedCall.Id} to volunteer {volunteer.Id}: {ex.Message}");
                        continue;
                    }
                }
                else
                {
                    // בעיה 4: אין הדפסות לוג כדי לראות מה קורה בחלק הזה
                    Console.WriteLine($"Volunteer {volunteer.Id} has active assignment {currentAssignment.Id}");

                    DO.Call? call;
                    lock (AdminManager.BlMutex)
                    {
                        call = DalApi.Factory.Get.Call.Read(c => c.RadioCallId == currentAssignment.CallId);
                    }

                    if (call is null)
                    {
                        Console.WriteLine($"Call {currentAssignment.CallId} not found for assignment {currentAssignment.Id}");
                        continue;
                    }

                    // בדיקת מרחק וזמן
                    double distance = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, call.Latitude, call.Longitude);
                    TimeSpan baseTime = TimeSpan.FromMinutes(distance * 2);
                    TimeSpan extra = TimeSpan.FromMinutes(Random.Shared.Next(1, 5));
                    TimeSpan totalNeeded = baseTime + extra;
                    TimeSpan actual = AdminManager.Now - currentAssignment.EntryTime;

                    Console.WriteLine($"Assignment {currentAssignment.Id}: Distance={distance:F2}km, " +
                                    $"TotalNeeded={totalNeeded.TotalMinutes:F1}min, Actual={actual.TotalMinutes:F1}min");

                    if (actual >= totalNeeded)
                    {
                        try
                        {
                            Console.WriteLine($"Attempting to close assignment {currentAssignment.Id}");

                            var callImplementation = new CallImplementation();
                            callImplementation.CloseCall(volunteer.Id, currentAssignment.Id);

                            updatedVolunteerIds.Add(volunteer.Id);
                            updatedCallIds.Add(call.RadioCallId);

                            Console.WriteLine($"Successfully closed assignment {currentAssignment.Id}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to close assignment {currentAssignment.Id}: {ex.Message}");
                            continue;
                        }
                    }
                    else if (Random.Shared.NextDouble() < 0.2) // 10% chance to cancel
                    {
                        try
                        {
                            Console.WriteLine($"Attempting to cancel assignment {currentAssignment.Id}");

                            var callImplementation = new CallImplementation();
                            callImplementation.CancelCall(volunteer.Id, currentAssignment.Id);

                            updatedVolunteerIds.Add(volunteer.Id);
                            updatedCallIds.Add(call.RadioCallId);

                            Console.WriteLine($"Successfully canceled assignment {currentAssignment.Id}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to cancel assignment {currentAssignment.Id}: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing volunteer {volunteer.Id}: {ex.Message}");
                continue;
            }
        }

        // Notify observers
        Console.WriteLine($"Notifying updates for {updatedVolunteerIds.Distinct().Count()} volunteers and {updatedCallIds.Distinct().Count()} calls");

        foreach (var id in updatedVolunteerIds.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);

        foreach (var id in updatedCallIds.Distinct())
            CallManager.Observers.NotifyItemUpdated(id);
    }

*/

}