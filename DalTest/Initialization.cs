namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Security.Cryptography;
using System.Text;


//נעזרנו בגיפיטי בקשר למילוי המערכים וכוונו אותו לפונקציות מסוימות.

public static class Initialization
{
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();

    public class CreateVolunteer
    {

        public static string[] names = new string[] {

        "Yaakov Cohen", "Miriam Levy", "Avraham Ben-David", "Sarah Shalom", "Chaim Adler", "Ruth Klein",
        "David Katz", "Esther Goldstein", "Moshe Fogel", "Tova Levi", "Yitzhak Mizrahi", "Naomi Rosen",
        "Yehuda Friedman", "Shoshana Cohen", "Eliezer Glick", "Chava Shapiro",
    };

        public static string[] addresses = new string[] {

        "Atsil 13, Jerusalem", "King David Street 7, Jerusalem", "Ben Yehuda Street 45, Jerusalem",
        "Jaffa Street 56, Jerusalem", "Agrippas Street 22, Jerusalem", "Shmuel Hanavi Street 5, Jerusalem",
        "Yehuda Halevi Street 3, Jerusalem", "Hillel Street 19, Jerusalem", "Ramban Street 9, Jerusalem",
        "Strauss Street 12, Jerusalem", "Yafo Road 34, Jerusalem", "Kehilat Yaakov Street 8, Jerusalem",
        "Mordechai Ben Hillel Street 11, Jerusalem", "Keren Hayesod Street 16, Jerusalem", "Shazar Boulevard 21, Jerusalem", "25 Shlomzion Hamalka Street, Jerusalem:"
    };
        public static double[] longitudes = new double[] {

        35.2080, 35.2130, 35.2215, 35.2160, 35.2250, 35.2205,
        35.2225, 35.2270, 35.2290, 35.2135, 35.2100, 35.2295,
        35.2240, 35.2265, 35.2175, 35.2045
    };
        public static double[] latitudes = new double[] {

        31.7735, 31.7685, 31.7760, 31.7810, 31.7730, 31.7800,
        31.7775, 31.7745, 31.7790, 31.7715, 31.7755, 31.7730,
        31.7705, 31.7795, 31.7720, 31.7765
    };


        public static void CreateVolunteerEntries()
        {
            int MIN_ID = 100000000;
            int MAX_ID = 999999999;
            int MAX_DISTANCE = 50;

            int minLength = Math.Min(Math.Min(names.Length, addresses.Length), Math.Min(latitudes.Length, longitudes.Length));
            for (int i = 0; i < minLength; i++)
            {
                string name = names[i];
                int id;

                do
                {
                    id = s_rand.Next(MIN_ID, MAX_ID);
                } while (s_dal!.Volunteer.Read(id) != null);  // Ensure unique ID


                Random rand = new Random();
                string phoneNumber;
                int phoneNumberInt;
                do
                {
                    phoneNumber = $"05{rand.Next(1000000, 10000000)}";
                    phoneNumberInt = int.Parse(phoneNumber);
                } while (s_dal.Volunteer.Read(phoneNumberInt) != null);

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
                    MaxResponseDistance = s_rand.NextDouble() * MAX_DISTANCE,
                    TypeOfDistance = DistanceTypeEnum.AirDistance,
                    Password = encryptedPassword
                };

                s_dal.Volunteer.Create(volunteer);
                //Console.WriteLine($"Created {position} {name} with encrypted password: {encryptedPassword}");
            }
        }


        private static string GenerateStrongPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder password = new StringBuilder(12);

            for (int i = 0; i < 12; i++)
            {
                password.Append(validChars[s_rand.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        // Encrypt the password using AES
        public static string Encrypt(string plainText)
        {
            string key = "0123456789abcdef";  // Example key, replace with a secure key
            string iv = "abcdef9876543210";   // Example IV, replace with a secure IV

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


    public class CreateCall
    {

        public static string[] CallAddresses = new string[]
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
    "Menachem Begin St 11, Jerusalem", "Yisrael Yaakov St 13, Jerusalem", "Ben Yehuda St 6, Jerusalem",
    "Hadar St 3, Jerusalem", "Maharal St 8, Jerusalem", "Yosef Schwartz St 4, Jerusalem",
    "Jabotinsky St 7, Jerusalem", "Shazar St 5, Jerusalem", "Gonenim St 12, Jerusalem",
    "Talpiot St 14, Jerusalem", "Bilu St 9, Jerusalem", "Yovel St 2, Jerusalem",
    "Herzl St 3, Jerusalem", "Hashmonai St 6, Jerusalem", "Ramot St 17, Jerusalem",
    "Shalom Aleichem St 10, Jerusalem", "Eli Cohen St 4, Jerusalem", "Shlomo HaMelech St 7, Jerusalem"
   };

        public static double[] CallLongitudes = new double[]
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

        public static double[] CallLatitudes = new double[]
        {
    31.776346, 31.777594, 31.777009, 31.776115, 31.776034,
    31.768798, 31.779074, 31.778777, 31.777356, 31.772628,
    31.777441, 31.768927, 31.775993, 31.781968, 31.778674,
    31.780973, 31.779477, 31.780509, 31.782150, 31.776243,
    31.782617, 31.775681, 31.775062, 31.776265, 31.774854,
    31.776105, 31.779306, 31.776027, 31.773822, 31.772712,
    31.777875, 31.777619, 31.779239, 31.779170, 31.779692,
    31.779158, 31.779050, 31.778736, 31.781393, 31.781827,
    31.773728, 31.776413, 31.773155, 31.773418, 31.774642,
    31.775249, 31.777264, 31.779530, 31.776292, 31.775522
        };

        public static string[] CallDescriptions = new string[]

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


        public static void CreateCallEntries()
        {


            Random rand = new Random();
            DateTime now = DateTime.Now;
            DateTime twoHoursAgo = now.AddHours(-2);
            int totalMinutesRange = (int)(now - twoHoursAgo).TotalMinutes;
            int randomMinutes = rand.Next(totalMinutesRange);
            DateTime MyStartTime = twoHoursAgo.AddMinutes(randomMinutes);



            for (int i = 0; i < 50; i++)
            {
                int MyRadioCallId = Config.getNextCallId;
                string MyDescription = CallDescriptions[i];
                string MyAddress = CallAddresses[i];
                double MyLatitude = CallLatitudes[i];
                double MyLongitude = CallLongitudes[i];
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
                    CallType = MyCallType,
                };

                s_dal.Call.Create(call);
            }
        }
    }


    private static void createAssignments()
    {
        List<Volunteer>? volunteersList = s_dal.Volunteer.ReadAll();
        List<Call>? callsList = s_dal.Call.ReadAll();

        for (int i = 0; i < 49; i++)
        {
            DateTime minTime = callsList[i].StartTime;
            DateTime maxTime = (DateTime)callsList[i].ExpiredTime;
            TimeSpan diff = maxTime - minTime - TimeSpan.FromHours(2);
            // DateTime randomTime = minTime.AddMinutes(s_rand.Next((int)diff.TotalMinutes));
            DateTime randomTime = minTime.AddMinutes(s_rand.Next(Math.Abs((int)diff.TotalMinutes)));


            CallResolutionStatus typeOfEndTime;
            if (i < 5)
            {
                typeOfEndTime = CallResolutionStatus.Expired;
            }
            else
            {
                typeOfEndTime = (CallResolutionStatus)s_rand.Next(Enum.GetValues(typeof(CallResolutionStatus)).Length);
            }

            s_dal!.Assignment.Create(new Assignment(
                Config.getNextAssignmentId,
                callsList[s_rand.Next(callsList.Count - 15)].RadioCallId, // CallId
                volunteersList[s_rand.Next(volunteersList.Count)].Id, // VolunteerId
                randomTime, // EntryTime
                randomTime.AddHours(2), // FinishCompletionTime
                typeOfEndTime // CallResolutionStatus
            ));
        }
    }


    public static void Do(IDal? dal) //stage 2
    {
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();//stage 2


        Console.WriteLine("Initializing Volunteers...");
        CreateVolunteer.CreateVolunteerEntries();

        Console.WriteLine("Initializing Calls...");
        CreateCall.CreateCallEntries();

        Console.WriteLine("Initializing Assignments...");
        createAssignments();


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



