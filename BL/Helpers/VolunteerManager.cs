/*using DalApi;
using BO;
using System.Data;
using DO;
using BlApi;
using System.Text;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// 
    /// </summary>
    /// <param name="volunteers"></param>
    /// <returns></returns>
    public static List<BO.Volunteer> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        return volunteers.Select(v => new BO.Volunteer
        {
            Id = v.Id,
            FullName = v.Name,
            PhoneNumber = v.Phone,
            Email = v.Email,
            Password = v.Password,
            CurrentAddress = v.Address,
            Latitude = v.Latitude,
            Longitude = v.Longitude,
            Role = (BO.PositionEnum)v.Position,  // המרה מ-DO.PositionEnum ל-BO.PositionEnum
            IsActive = v.Active,
            MaxDistance = v.MaxResponseDistance,
            TypeOfDistance = (BO.DistanceType)v.TypeOfDistance,  // המרה מ-DO.DistanceTypeEnum ל-BO.DistanceType
            TotalCallsHandled = 0, // אם יש נתונים נוספים יש להוסיף כאן
            TotalCallsCanceled = 0, // אם יש נתונים נוספים יש להוסיף כאן
            TotalCallsExpired = 0, // אם יש נתונים נוספים יש להוסיף כאן
            CurrentCall = null // אם יש קריאה נוכחית יש להוסיף כאן
        }).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
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
            Role = (BO.PositionEnum)volunteer.Position, // המרה מ-DO.PositionEnum ל-BO.PositionEnum
            IsActive = volunteer.Active,
            MaxDistance = volunteer.MaxResponseDistance,
            TypeOfDistance = (BO.DistanceType)volunteer.TypeOfDistance, // המרה מ-DO.DistanceTypeEnum ל-BO.DistanceType
            TotalCallsHandled = 0, // אם יש נתונים נוספים יש להוסיף כאן
            TotalCallsCanceled = 0, // אם יש נתונים נוספים יש להוסיף כאן
            TotalCallsExpired = 0, // אם יש נתונים נוספים יש להוסיף כאן
            CurrentCall = null // אם יש קריאה נוכחית יש להוסיף כאן
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    public static DO.Volunteer ConvertToDO(BO.Volunteer volunteer)
    {
        return new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.FullName,
            Phone = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.CurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Position = (DO.PositionEnum)volunteer.Role, // המרה מ-BO.PositionEnum ל-DO.PositionEnum
            Active = volunteer.IsActive,
            MaxResponseDistance = volunteer.MaxDistance,
            TypeOfDistance = (DO.DistanceTypeEnum)volunteer.TypeOfDistance // המרה מ-BO.DistanceType ל-DO.DistanceTypeEnum
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    public static bool IsRequesterAuthorized(string requesterId, BO.Volunteer volunteer)
    {
        return requesterId == volunteer.Id.ToString() || IsAdmin(requesterId, volunteer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="boVolunteer"></param>
    /// <returns></returns>
    /// <exception cref="DalUnauthorizedAccessException"></exception>
    private static bool IsAdmin(string requesterId, BO.Volunteer boVolunteer)
    {
        // Check if the requester is the volunteer or an admin
        if (!(requesterId == boVolunteer.Id.ToString()) && boVolunteer.Role != BO.PositionEnum.Manager)
        {
            throw new DalUnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");
        }

        // Check if only an admin can update the volunteer's role
        if (boVolunteer.Role != BO.PositionEnum.Manager && boVolunteer.Role != BO.PositionEnum.Volunteer)
        {
            throw new DalUnauthorizedAccessException("Only an admin can update the volunteer's role.");
        }

        return true; // Return true if authorized
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requesterId"></param>
    /// <param name="existingVolunteer"></param>
    /// <param name="volunteerToUpdate"></param>
    /// <returns></returns>
    /// <exception cref="DalUnauthorizedAccessException"></exception>
    public static bool CanUpdateFields(string requesterId, DO.Volunteer existingVolunteer, BO.Volunteer volunteerToUpdate)
    {
        // Check if the requester is either the volunteer themselves or an admin
        if (requesterId != existingVolunteer.Id.ToString() && !IsAdmin(requesterId, volunteerToUpdate))
        {
            return false;
        }

        // Additional logic to check which fields can be updated, e.g., ensure the role is not being changed by non-admins
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
    /// 
    /// </summary>
    /// <param name="volunteerBO"></param>
    /// <exception cref="DalFormatException"></exception>
    public static void ValidateLogicalFields(BO.Volunteer volunteerBO)
    {
        // מוודא שניתן להמיר את הכתובת לקואורדינטות
        var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);

        // אם הקואורדינטות לא תקינות (לדוגמה, 0, 0)
        if (latitude == 0 && longitude == 0)
        {
            throw new DalFormatException("Invalid address format or unable to fetch coordinates.");
        }

        // אם הקואורדינטות לא נמצאות בטווח הגיאוגרפי התקני
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            throw new DalFormatException("Coordinates are out of valid geographic range.");
        }
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="volunteer"></param>
    /// <exception cref="DalFormatException"></exception>
    public static void ValidateInputFormat(BO.Volunteer volunteer)
    {
        // המרת ת.ז (מספר) למיתר לפני שליחה לפונקציה
        if (!IsValidIdNumber(volunteer.Id.ToString()))
        {
            throw new DalFormatException("Invalid ID number.");
        }

        // בדיקה אם כתובת מגוריו לא ריקה
        if (string.IsNullOrWhiteSpace(volunteer.CurrentAddress))
        {
            throw new DalFormatException("Address cannot be empty.");
        }

        // בדיקה אם הטלפון תקין
        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
        {
            throw new DalFormatException("Invalid phone number.");
        }

        // בדיקה אם המייל תקין
        if (!IsValidEmail(volunteer.Email))
        {
            throw new DalFormatException("Invalid email.");
        }

        // בדיקה אם הסיסמא חזקה
        if (!IsValidPassword(volunteer.Password))
        {
            throw new DalFormatException("Password is not strong enough.");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="idNumber"></param>
    /// <returns></returns>
    public static bool IsValidIdNumber(string idNumber)
    {
        // נוודא שהתעודה מכילה רק ספרות ושהיא באורך הנכון (9 ספרות)
        if (idNumber.Length != 9 || !idNumber.All(char.IsDigit))
        {
            return false;
        }

        // המרת המיתר למספר באמצעות TryParse
        if (int.TryParse(idNumber, out int id))
        {
            // חישוב ספרת הביקורת
            int[] digits = new int[9];
            for (int i = 0; i < 9; i++)
            {
                digits[i] = idNumber[i] - '0'; // המרת תו לספרה
            }

            // חישוב סכום לפי שיטת ספרת ביקורת
            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int multiplier = (i % 2 == 0) ? 1 : 2;
                int product = digits[i] * multiplier;
                sum += product > 9 ? product - 9 : product;  // אם התוצאה גדולה מ-9, מורידים 9
            }

            int checkDigit = (10 - sum % 10) % 10;  // ספרת הביקורת

            // השוואה אם ספרת הביקורת תואמת
            return checkDigit == digits[8];
        }

        // אם לא ניתן להמיר למספר, התעודה לא תקינה
        return false;
    }

/// <summary>
/// 
/// </summary>
/// <param name="phoneNumber"></param>
/// <returns></returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // ניתן להוסיף כאן לוגיקה לבדוק אם מספר הטלפון תקין (למשל, לפי פורמט מספר טלפון תקני)
        return phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit); // דוגמה לבדיקה
    }

    // פונקציה שמבצעת בדיקה אם המייל תקין
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(string email)
    {
        // בדיקה אם המייל מכיל את הסימן @ ואת החלק אחרי ה-@ כמו domain.com
        return email.Contains("@") && email.Contains(".") && email.IndexOf("@") < email.LastIndexOf(".");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool IsValidPassword(string password)
    {
        // בדיקה אם הסיסמא מכילה לפחות 8 תווים, אותיות גדולות, קטנות, מספרים ותו מיוחד
        return password.Length >= 8 &&
               password.Any(char.IsLower) &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsDigit) &&
               password.Any(c => "!@#$%^&*()_+[]{}|;:',.<>?".Contains(c));
    }
}
*/

using DalApi;
using BO;
using System.Data;
using DO;
using BlApi;
using System.Text;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

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
    public static void ValidateLogicalFields(BO.Volunteer volunteerBO)
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
    }

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
        if (idNumber.Length != 9 || !idNumber.All(char.IsDigit))
        {
            return false;
        }

        if (int.TryParse(idNumber, out int id))
        {
            int[] digits = new int[9];
            for (int i = 0; i < 9; i++)
            {
                digits[i] = idNumber[i] - '0';
            }

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int multiplier = (i % 2 == 0) ? 1 : 2;
                int product = digits[i] * multiplier;
                sum += product > 9 ? product - 9 : product;
            }

            int checkDigit = (10 - sum % 10) % 10;
            return checkDigit == digits[8];
        }

        return false;
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
        return password.Length >= 8 &&
               password.Any(char.IsLower) &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsDigit) &&
               password.Any(c => "!@#$%^&*()_+[]{}|;:',.<>?".Contains(c));
    }
}