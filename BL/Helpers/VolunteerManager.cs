using DalApi;
using BO;
using System.Data;
using DO;
using BlApi;
using System.Text;
namespace Helpers;
using System.Security.Cryptography;

internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4

    /// <summary>
    /// Converts a collection of DO.Volunteer objects to a list of BO.Volunteer objects
    /// by utilizing the ConvertToBO function for each volunteer in the collection
    /// </summary>
    /// <param name="volunteers">Collection of DO.Volunteer objects to convert</param>
    /// <returns>List of converted BO.Volunteer objects</returns>
    public static List<BO.Volunteer> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        return volunteers.Select(ConvertToBO).ToList();
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
            TotalCallsHandled = 0,
            TotalCallsCanceled = 0,
            TotalCallsExpired = 0,
            CurrentCall = null
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
            Password = HashPassword(GenerateStrongPassword()),
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

        if (boVolunteer.Role != BO.PositionEnum.Manager && boVolunteer.Role != BO.PositionEnum.Volunteer)
        {
            throw new DalUnauthorizedAccessException("Only an admin can update the volunteer's role.");
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
        if (requesterId != existingVolunteer.Id.ToString() && !IsAdmin(requesterId, volunteerToUpdate))
        {
            return false;
        }

        if (volunteerToUpdate.Role != (BO.PositionEnum)existingVolunteer.Position)
        {
            if (volunteerToUpdate.Role != BO.PositionEnum.Manager && volunteerToUpdate.Role != BO.PositionEnum.Volunteer)
            {
                throw new DalUnauthorizedAccessException("Only an admin can update the volunteer's role.");
            }
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
 /*   public static void ValidateLogicalFields(BO.Volunteer volunteerBO)
    {
        var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);

        if (latitude == 0 && longitude == 0)
        {
            throw new DalFormatException("Invalid address format or unable to fetch coordinates.");
        }

        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            throw new DalFormatException("Coordinates are out of valid geographic range.");
        }
    }*/

    /// <summary>
    /// Validates the format of all input fields in a volunteer object
    /// Checks ID number, address, phone number, email, and password format
    /// </summary>
    /// <param name="volunteer">Volunteer object to validate</param>
    /// <exception cref="DalFormatException">Thrown when any validation fails</exception>
    public static void ValidateInputFormat(BO.Volunteer volunteer)
    {
        if (!IsValidIdNumber(volunteer.Id.ToString()))
        {
            throw new DalFormatException("Invalid ID number.");
        }

        if (string.IsNullOrWhiteSpace(volunteer.CurrentAddress))
        {
            throw new DalFormatException("Address cannot be empty.");
        }

        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
        {
            throw new DalFormatException("Invalid phone number.");
        }

        if (!IsValidEmail(volunteer.Email))
        {
            throw new DalFormatException("Invalid email.");
        }

        if (!IsValidPassword(volunteer.Password))
        {
            throw new DalFormatException("Password is not strong enough.");
        }
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
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
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
        return hashedInput == storedHash;
    }

    /// <summary>
    /// returns a randoml strong password
    /// </summary>
    /// <returns></returns>
    public static string GenerateStrongPassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        const string allChars = uppercase + lowercase + digits + special;

        char[] password = new char[8];

        // Ensure at least one of each required character type
        password[0] = uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)];
        password[1] = lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        // Fill the remaining characters randomly from all types
        for (int i = 4; i < 8; i++)
        {
            password[i] = allChars[RandomNumberGenerator.GetInt32(allChars.Length)];
        }

        // Shuffle the password to avoid any predictable ordering
        return new string(password.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }
}