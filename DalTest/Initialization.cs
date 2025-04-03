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
        //Chava Shapiro password - Da@I%*k*UTn6

        public static string[] addresses = new string[] {

        "Etsel 13, Jerusalem", "King David Street 7, Jerusalem", "Ben Yehuda Street 45, Jerusalem",
        "Jaffa Street 56, Jerusalem", "Agrippas Street 22, Jerusalem", "Shmuel Hanavi Street 5, Jerusalem",
        "Yehuda Halevi Street 3, Jerusalem", "Hillel Street 19, Jerusalem", "Ramban Street 9, Jerusalem",
        "Strauss Street 12, Jerusalem", "Yafo Road 34, Jerusalem", "Kehilat Yaakov Street 8, Jerusalem",
        "Mordechai Ben Hillel Street 11, Jerusalem", "Keren Hayesod Street 16, Jerusalem", "Shazar Boulevard 21, Jerusalem", "25 Shlomzion Hamalka Street, Jerusalem:"
    };
        public static double[] latitudes = new double[] {
        31.7511651, 31.70954, 31.759595, 31.78168, 31.69916,
        31.7959211, 31.7513601, 31.780094, 31.528593, 31.906037,
        31.759595, 31.759595, 31.759595, 31.759595, 31.759595, 31.759595
    };

        public static double[] longitudes = new double[] {
        34.9819467, 35.205725, 35.215315, 35.220332, 35.196286,
        35.2197757, 34.9811379, 35.218297, 35.103687, 35.203005,
        35.215315, 35.215315, 35.215315, 35.215315, 35.215315, 35.215315
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
                } while (s_dal.Volunteer.Read(v => v.Id == id) != null);


                Random rand = new Random();
                string phoneNumber;
                int phoneNumberInt;
                do
                {
                    phoneNumber = $"05{rand.Next(1000000, 10000000)}";
                    phoneNumberInt = int.Parse(phoneNumber);
                } while (s_dal.Volunteer.Read(v => v.Phone == phoneNumber) != null);

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
                    Password = encryptedPassword,
                };

                s_dal.Volunteer.Create(volunteer);
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
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // המרת הסיסמה ל-בייטים
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // המרת הבייטים למחרוזת Hex
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
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

        public static double[] CallLatitudes = new double[] {
    31.7865608, 31.759595, 31.8017893, 31.759595, 31.794767, 
    31.7723879, 31.842212, 31.7875022, 31.7472349, 31.7831088, 
    31.898714, 31.8051921, 31.7857651, 31.759595, 31.7957696, 
    31.7259643, 31.7476677, 31.759595, 31.703275, 31.7831088, 
    31.7858115, 31.8111253, 31.759595, 31.759595, 31.759595, 
    31.779042, 31.7909196, 32.0416824, 31.759595, 31.7933736, 
    31.7854673, 32.3260388, 31.8405465, 31.759595, 31.759595, 
    31.759595, 31.7686856, 31.8383352, 31.7815767, 32.8089768, 
    31.7215207, 31.759595, 31.7709719, 31.7135737, 31.906037, 
    31.7515394, 31.762803, 31.780514, 31.74948, 31.77676, 
    31.81561, 31.7723328, 31.7668532, 31.759595
    };

        public static double[] CallLongitudes = new double[] {
    35.2208052, 35.215315, 35.2228759, 35.215315, 35.2425346, 
    35.2215257, 35.24206, 35.2265973, 35.2326395, 35.2203032, 
    35.185758, 35.2157309, 35.1968887, 35.215315, 35.2198077, 
    34.7437502, 35.2323345, 35.215315, 35.194809, 35.2203032, 
    35.1741509, 35.2174861, 35.215315, 35.215315, 35.215315, 
    35.229702, 35.2089577, 34.7904787, 35.215315, 35.2246764, 
    35.1001866, 34.8511049, 35.2454408, 35.215315, 35.215315, 
    35.215315, 35.1950858, 35.2441809, 35.2180856, 34.997939, 
    35.2284413, 35.215315, 35.2210202, 34.9838857, 35.203005, 
    35.2160228, 35.2099602, 35.217981, 34.9880954, 35.230342, 
    35.1954938, 35.2215927, 35.213483, 35.215315
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
                int MyRadioCallId = Config.NextCallId;
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
                        MyExpiredTime = MyStartTime.AddDays(15);
                        break;

                    case int n when (n >= 10 && n < 20):
                        MyCallType = CallType.Medium_Urgency;
                        MyExpiredTime = MyStartTime.AddDays(30);
                        break;

                    case int n when (n >= 20 && n < 40):
                        MyCallType = CallType.General_Assistance;
                        MyExpiredTime = MyStartTime.AddYears(1);
                        break;

                    case int n when (n >= 40 && n < 50):
                        MyCallType = CallType.Non_Urgent;
                        MyExpiredTime = MyStartTime.AddMonths(2);
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
        List<Volunteer>? volunteersList = s_dal.Volunteer.ReadAll()?.ToList();
        List<Call>? callsList = s_dal.Call.ReadAll()?.ToList();

        for (int i = 0; i < 49; i++)
        {
            DateTime minTime = callsList[i].StartTime;
            DateTime maxTime = (DateTime)callsList[i].ExpiredTime;
            TimeSpan diff = maxTime - minTime - TimeSpan.FromHours(2);
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
                Config.NextAssignmentId,
                callsList[s_rand.Next(callsList.Count - 15)].RadioCallId, // CallId
                volunteersList[s_rand.Next(volunteersList.Count)].Id, // VolunteerId
                randomTime, // EntryTime
                randomTime.AddDays(2), // FinishCompletionTime
                typeOfEndTime // CallResolutionStatus
            ));
        }
    }


    //public static void Do(IDal? dal) //stage 2
    public static void Do() //stage 4
    {
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4
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



