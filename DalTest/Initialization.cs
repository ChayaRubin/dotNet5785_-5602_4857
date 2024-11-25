
namespace DalTest;
using DalApi;
using DO;

//נעזרנו בגיפיטי בקשר למילוי המערכים וכוונו אותו לפונקציות מסוימות.
public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; 
    private static ICall? s_dalCall; 
    private static IAssignment? s_dalAssignment; 
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();
    public class createVolunteer
    {
        private static string[] names = {
        "Yaakov Cohen", "Miriam Levy", "Avraham Ben-David", "Sarah Shalom", "Chaim Adler", "Ruth Klein",
        "David Katz", "Esther Goldstein", "Moshe Fogel", "Tova Levi", "Yitzhak Mizrahi", "Naomi Rosen",
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

        private static void CreateVolunteerEntries()
        {

            int MIN_ID = 100000000;
            int MAX_ID = 999999999;
            int MAX_DISTANCE = 50;

            for (int i = 0; i < 16; i++)
            {
                string name = names[i];

                int id;
                do
                {
                    id = rand.Next(MIN_ID, MAX_ID);
                } while (s_dalVolunteer!.Read(id) != null); 

                string phoneNumber;
                do
                {
                    phoneNumber = "05" + rand.Next(1000000, 9999999).ToString();
                } while (s_dalVolunteer.GetAll().Any(v => v.Phone == phoneNumber)); 

                string email = $"{name.ToLower().Replace(" ", ".")}@email.com";
                string address = addresses[i];
                double latitude = latitudes[i];
                double longitude = longitudes[i];
                PositionEnum position = i == 0 ? PositionEnum.Manager : PositionEnum.Volunteer;
                string password = GenerateStrongPassword();
                string encryptedPassword = Encrypt(password);

                Volunteer volunteer = new Volunteer
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
                    MaxResponseDistance = rand.NextDouble() * MAX_DISTANCE,
                    TypeOfDistince = DistanceTypeEnum.AirDistance,
                    Password = encryptedPassword
                };
                s_dalVolunteer.Create(volunteer);
                Console.WriteLine($"Created {position} {name} with encrypted password: {encryptedPassword}");
            }
        }


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
        //הצפנה בAES
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

    }


    public class createCall
    {
        string[] callAddresses = new string[]
        {
            "Meah Shearim St 10, Jerusalem", "Chazon Ish St 6, Jerusalem", "Ramat Eshkol St 11, Jerusalem",
            "Har Safra St 1, Jerusalem", "Mount Scopus St 4, Jerusalem", "Keren Hayesod St 30, Jerusalem",
            "Neve Yaakov St 17, Jerusalem", "Shmuel HaNavi St 12, Jerusalem", "Yechiel St 3, Jerusalem",
            "Rav Kook St 4, Jerusalem", "Talmud Torah St 8, Jerusalem", "Sanhedria St 18, Jerusalem",
            "Kiryat Moshe St 6, Jerusalem", "Achad Ha'am St 2, Jerusalem", "Bar Ilan St 7, Jerusalem",
            "City Center St 14, Jerusalem", "Rechov Yechiel 3, Jerusalem", "Giv'at Shaul St 7, Jerusalem",
            "Nachlaot St 7, Jerusalem", "Rav Kook St 5, Jerusalem", "Har Nof St 18, Jerusalem",
            "Ramat Shlomo St 15, Jerusalem", "Sderot Yitzhak Rabin St 5, Jerusalem", "Har Hatzofim St 8, Jerusalem",
            "Giv'at HaMivtar St 6, Jerusalem", "Tefilat Yisrael St 14, Jerusalem", "Malkhei Yisrael St 10, Jerusalem",
            "Kiryat Tzahal St 6, Jerusalem", "Nachal Noach St 17, Jerusalem", "Maalot Dafna St 6, Jerusalem",
            "Har HaMor St 3, Jerusalem", "Ramat HaSharon St 2, Jerusalem", "Yakar St 3, Jerusalem",
            "Rav Haim Ozer St 9, Jerusalem", "Yehoshua Ben-Nun St 5, Jerusalem", "Meir Schauer St 12, Jerusalem",
            "Menachem Begin St 11, Jerusalem", "Yisrael Yaakov St 13, Jerusalem", "Ben Yehuda St 6, Jerusalem"
        };

        double[] callLongitudes = new double[]
        {
            35.225721, 35.217133, 35.229169, 35.230535, 35.225939,
            35.224211, 35.219538, 35.224968, 35.226063, 35.219375,
            35.213736, 35.217712, 35.229053, 35.217509, 35.220429,
            35.222809, 35.222797, 35.226436, 35.221255, 35.220655,
            35.229191, 35.222992, 35.227074, 35.221162, 35.227591,
            35.225712, 35.220829, 35.223016, 35.219865, 35.230012,
            35.220076, 35.221336, 35.228300, 35.221133, 35.224713,
            35.227271, 35.219754, 35.226358, 35.225099, 35.228086,
            35.228418, 35.222438, 35.221694, 35.223145, 35.221228,
            35.222590, 35.222579, 35.222869, 35.226072, 35.221711
        };

        double[] callLatitudes = new double[]
        {
            31.776545, 31.771675, 31.767727, 31.771267, 31.768520,
            31.785228, 31.786335, 31.769799, 31.773315, 31.786812,
            31.776216, 31.773144, 31.764577, 31.767558, 31.774280,
            31.782129, 31.784256, 31.779211, 31.783858, 31.783022,
            31.774607, 31.773122, 31.782645, 31.783712, 31.773770,
            31.779614, 31.767658, 31.785070, 31.778488, 31.766734,
            31.780314, 31.783537, 31.775809, 31.773657, 31.781039,
            31.779433, 31.771505, 31.770824, 31.774722, 31.776229,
            31.773940, 31.777524, 31.774912, 31.770963, 31.777611,
            31.776597, 31.785040, 31.772628, 31.776763, 31.780179
        };

        string[] callDescriptions = new string[]
        {
            // High Urgency (Immediate and Critical):
            "Medical Emergency", "Accident Assistance", "Vehicle Recovery", "Emergency Towing", "Stuck in Mud",
            "Engine Overheating", "Heavy Accident", "Emergency Tow", "Battery Issues", "Fire in Vehicle",

            // Medium Urgency (Urgent but less critical):
            "Fuel Assistance", "Flat Tire", "Lockout Assistance", "Vehicle Breakdown", "Flat Battery",
            "Vehicle Stuck in Traffic", "Accident Help", "Lost Keys", "Driving Assistance", "Tire Puncture",
    
            // General Assistance (Everyday issues or less urgent):
            "Driving Safety", "Parking Assistance", "Car Repair Advice", "Vehicle Inspection", "Insurance Assistance",
            "Vehicle Jumpstart", "Call for Help", "Car Service Appointment", "Windshield Repair", "Lost Wallet",
            "Fuel Running Low", "Battery Jumpstart", "Car Alarm Issues", "Motorcycle Breakdown", "Lost GPS Navigation",
            "Roadside Assistance", "Headlight Malfunction", "Engine Check", "Car Wash", "Flat Tire Change",

            // General or Non-Urgent Assistance:
            "Road Advice", "Car Repair Consultation", "Driving Safety Tips", "Insurance Policy Questions", "Basic Vehicle Maintenance",
            "Navigation Assistance", "Towing Insurance", "Tire Pressure Check", "Late Night Assistance", "General Car Troubleshooting"
        };


        private static void CreateCallEntries()
        {


            Random rand = new Random();
            DateTime now = DateTime.Now;
            DateTime twoHoursAgo = now.AddHours(-2);
            int totalMinutesRange = (int)(now - twoHoursAgo).TotalMinutes;
            int randomMinutes = rand.Next(totalMinutesRange);
            DateTime MyStartTime = twoHoursAgo.AddMinutes(randomMinutes);



            for (int i = 0; i < 50; i++)
            {
                int MyRadioCallId = Config.NextCallId;
                string MyDescription = callDescriptions[i];
                string MyAddress = callAddresses[i];
                double MyLatitude = callLatitudes[i];
                double MyLongitude = callLongitudes[i];
                DateTime MyExpiredTime;
                CallType MyCallType;

                switch (i)
                {
                    case int n when (n < 10): 
                        MyCallType = CallType.Urgent;
                        MyExpiredTime = MyStartTime.AddMinutes(15); 
                        break;

                    case int n when (n >= 10 && n < 20): 
                        MyCallType = CallType.Medium_Urgency;
                        MyExpiredTime = MyStartTime.AddMinutes(30);
                        break;

                    case int n when (n >= 20 && n < 40): 
                        MyCallType = CallType.General_Assistance;
                        MyExpiredTime = MyStartTime.AddHours(1);
                        break;

                    case int n when (n >= 40 && n < 50): 
                        MyCallType = CallType.Non_Urgent;
                        MyExpiredTime = MyStartTime.AddHours(2); 
                        break;

                    default:
                        return;
                }

                Call call = new Call
                {
                    RadioCallId = MyRadioCallId,
                    Description = MyDescription,
                    Address = MyAddress,
                    Latitude = MyLatitude,
                    Longitude = MyLongitude,
                    StartTime = MyStartTime,
                    ExpiredTime = MyExpiredTime,
                    callType = MyCallType,
                };
            }
        }
    }
}



















//פענוח
//public static string Decrypt(string cipherText)
//{
//    using (Aes aesAlg = Aes.Create())
//    {
//        aesAlg.Key = Encoding.UTF8.GetBytes(key);
//        aesAlg.IV = Encoding.UTF8.GetBytes(iv);

//        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

//        using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
//        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
//        {
//            return srDecrypt.ReadToEnd();
//        }
//    }
//}



