
namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1

    private static readonly Random s_dalAssignment = new();
    private static readonly Random s_dalCall = new();
    private static readonly Random s_dalConfig = new();
    private static readonly Random s_dalVolunteer = new();

    public class createVolunteer
    {
        private static string[] addresses = {
        "Yaakov Cohen", "Miriam Levy", "Avraham Ben-David", "Sarah Shalom", "Chaim Adler", "Ruth Klein",
        "David Katz", "Esther Goldstein", "Moshe Yeshua", "Tova Levi", "Yitzhak Mizrahi", "Naomi Rosen",
        "Yehuda Friedman", "Shoshana Cohen", "Eliezer Glick", "Chava Shapiro", "Yonatan Weiss", "Leah Schwartz",
        "Ezra Cohen", "Rachel Abramovitch", "Shimon Ben-Tov", "Malkah Lieberman", "Tzvi Segal", "Chana Rubin",
        "Binyamin Stein", "Hannah Golan", "Simcha Kaplan", "Tamar Halperin", "Menachem Schneider", "Yaara Berman"
    };
        private static string[] addresses = {
        "Atsil 13, Jerusalem", "King David Street 7, Jerusalem", "Ben Yehuda Street 45, Jerusalem",
        "Jaffa Street 56, Jerusalem", "Agrippas Street 22, Jerusalem", "Shmuel Hanavi Street 5, Jerusalem",
        "Yehuda Halevi Street 3, Jerusalem", "Hillel Street 19, Jerusalem", "Ramban Street 9, Jerusalem",
        "Strauss Street 12, Jerusalem", "Yafo Road 34, Jerusalem", "Kehilat Yaakov Street 8, Jerusalem",
        "Mordechai Ben Hillel Street 11, Jerusalem", "Keren Hayesod Street 16, Jerusalem", "Shazar Boulevard 21, Jerusalem"
    };
        private static double[] longitudes = {
        35.2080, 35.2130, 35.2215, 35.2160, 35.2250, 35.2205,
        35.2225, 35.2270, 35.2290, 35.2135, 35.2100, 35.2295,
        35.2240, 35.2265, 35.2175
    };
        private static double[] latitudes = {
        31.7735, 31.7685, 31.7760, 31.7810, 31.7730, 31.7800,
        31.7775, 31.7745, 31.7790, 31.7715, 31.7755, 31.7730,
        31.7705, 31.7795, 31.7720
    };

        private static void createVolunteer()
        {
            Random s_rand = new Random(); // מניח שזה כבר מוגדר
            int MIN_ID = 100000000;  // טווח מינימלי לתעודת זהות
            int MAX_ID = 999999999;  // טווח מקסימלי לתעודת זהות
            int MAX_DISTANCE = 50;   // המרחק המרבי לקבלת קריאה (במרחק אווירי)
            for (int i = 0; i < 16; i++)  // 15 מתנדבים + 1 מנהל
            {
                string name = names[s_rand.Next(names.Count)];
                int id;
                do
                    id = s_rand.Next(MIN_ID, MAX_ID);
                while (false);  // לא ממש חשוב כאן - תעודת הזהות לא נבדקת בקוד הזה
                string email = name.ToLower().Replace(" ", ".") + "@email.com";
                string phoneNumber = "05" + s_rand.Next(1000000, 9999999).ToString();
                string address = addresses[s_rand.Next(addresses.Length)];
                double latitude = latitudes[s_rand.Next(latitudes.Length)];
                double longitude = longitudes[s_rand.Next(longitudes.Length)];
                PositionEnum position = i == 0 ? PositionEnum.Manager : PositionEnum.Volunteer;  // המנהל תמיד הראשון
                string password = GenerateStrongPassword();
                string encryptedPassword = EncryptPassword(password);
                double? maxResponseDistance = s_rand.NextDouble() * MAX_DISTANCE;  // בין 0 ל-50 ק"מ
                DistanceTypeEnum typeOfDistince = DistanceTypeEnum.AirDistance;
                var volunteer = new Volunteer
                {
                    Id = id,
                    Name = name,
                    Phone = phoneNumber,
                    Email = email,
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude,
                    Position = position,
                    Active = true,
                    MaxResponseDistance = maxResponseDistance,
                    TypeOfDistince = typeOfDistince,
                    Password = encryptedPassword  // הסיסמה מוצפנת נשמרת
                };
                Console.WriteLine($"Created {position} {name} with encrypted password: {encryptedPassword}");
            }
        }
        // פונקציה ליצירת סיסמה חזקה
        private static string GenerateStrongPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            Random rand = new Random();
            StringBuilder password = new StringBuilder(12);
            for (int i = 0; i < 12; i++)
            {
                password.Append(validChars[rand.Next(validChars.Length)]);
            }
            return password.ToString();
        }

        // פונקציה להצפנת סיסמה ב- SHA256
        private static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));
                }
                return hashStringBuilder.ToString();
            }
        }

        public static void Main(string[] args)
        {
            createVolunteer();
        }
    }



}
