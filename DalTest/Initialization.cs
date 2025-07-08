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
            // Create volunteers from XML data
            var volunteersFromXml = new[]
            {
                new { Id = 621662838, Name = "Yaakov Cohen", Phone = "056036018", Email = "yaakov.cohen@email.com", Password = "d7c52e3caf30a785b4c59b89ea5a4bfe2bfbaeecb732dab2278562bfb8b11c6c", Address = "Etsel 13, Jerusalem", Latitude = 31.7511651, Longitude = 34.9819467, Position = PositionEnum.Manager, MaxResponseDistance = 31.67534935371749, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 817875406, Name = "Miriam Levy", Phone = "052084623", Email = "miriam.levy@email.com", Password = "12ce56b36b78c99c87aaf096420fad885dda898fa5d5b2330c03d0e19289f685", Address = "King David Street 7, Jerusalem", Latitude = 31.70954, Longitude = 35.205725, Position = PositionEnum.Volunteer, MaxResponseDistance = 6.914661921350146, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 592103913, Name = "Avraham Ben-David", Phone = "052143059", Email = "avraham.ben-david@email.com", Password = "d07ee81f3fc78347420bfdfc57481583d315e6c25ae8eb6f3db1cafd9b9ae6d3", Address = "Ben Yehuda Street 45, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 13.626726168685693, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 706485399, Name = "Sarah Shalom", Phone = "051034944", Email = "sarah.shalom@email.com", Password = "48ec0135c8fbb667cdfd7ce858da86e2db7dbb5550c22e97f29dd53bc63b9c05", Address = "Jaffa Street 56, Jerusalem", Latitude = 31.78168, Longitude = 35.220332, Position = PositionEnum.Volunteer, MaxResponseDistance = 21.154270913962563, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 116366798, Name = "Chaim Adler", Phone = "054060951", Email = "chaim.adler@email.com", Password = "31306ee7c582931c75a78d381824248ddd9e018bf0c8878fe90af5b5e95f3e20", Address = "Agrippas Street 22, Jerusalem", Latitude = 31.69916, Longitude = 35.196286, Position = PositionEnum.Volunteer, MaxResponseDistance = 1.9111417006922593, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 979539050, Name = "Ruth Klein", Phone = "052493767", Email = "ruth.klein@email.com", Password = "5bb2345a31d8f6b9784729f7ac0d776870c6e7700bef324f760c64604204b335", Address = "Shmuel Hanavi Street 5, Jerusalem", Latitude = 31.7959211, Longitude = 35.2197757, Position = PositionEnum.Volunteer, MaxResponseDistance = 18.882628874722577, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 506371448, Name = "David Katz", Phone = "056416322", Email = "david.katz@email.com", Password = "ae824c3ce490d07f5c6db2cf1da8816f23b38b93619f72993f278520343ec32c", Address = "Yehuda Halevi Street 3, Jerusalem", Latitude = 31.7513601, Longitude = 34.9811379, Position = PositionEnum.Volunteer, MaxResponseDistance = 22.44583719455538, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 197318211, Name = "Esther Goldstein", Phone = "053587658", Email = "esther.goldstein@email.com", Password = "96c685e1bd3c7cd24b5e8cd6a921722e2f26d5b015298dd2d1732def431107f7", Address = "Hillel Street 19, Jerusalem", Latitude = 31.780094, Longitude = 35.218297, Position = PositionEnum.Volunteer, MaxResponseDistance = 28.876821568556228, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 340194993, Name = "Moshe Fogel", Phone = "058672420", Email = "moshe.fogel@email.com", Password = "3f072a0ac2b99dff31361d886e0c2e17bae59fa272a8ca6449d3e1a56e1a0849", Address = "Ramban Street 9, Jerusalem", Latitude = 31.528593, Longitude = 35.103687, Position = PositionEnum.Volunteer, MaxResponseDistance = 32.501537531213586, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 335198765, Name = "Tova Levi", Phone = "054534200", Email = "tova.levi@email.com", Password = "0b58cb58a6d28d3e070aaba81d621fbdd0860615cc73b4f5038ce100c26bceb5", Address = "Strauss Street 12, Jerusalem", Latitude = 31.906037, Longitude = 35.203005, Position = PositionEnum.Volunteer, MaxResponseDistance = 44.32661590097071, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 119785600, Name = "Yitzhak Mizrahi", Phone = "052712994", Email = "yitzhak.mizrahi@email.com", Password = "5d9e7bb65a5e2216023e649ddd06d7ac5c28d8d91ebb90cc000feabd9cf478a6", Address = "Yafo Road 34, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 2.1154350392300514, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 824706269, Name = "Naomi Rosen", Phone = "056970859", Email = "naomi.rosen@email.com", Password = "82d33f1ac5f403fbe5ca8e0924fa30618f4580d1494157ce3a793b6d8635659c", Address = "Kehilat Yaakov Street 8, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 31.178321844100182, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 690931779, Name = "Yehuda Friedman", Phone = "058999962", Email = "yehuda.friedman@email.com", Password = "b5056d7939a78155c604feff9be3635cb286ffd2a58f3342029db5d5c23ab966", Address = "Mordechai Ben Hillel Street 11, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 25.81193005468694, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 914689136, Name = "Shoshana Cohen", Phone = "051635145", Email = "shoshana.cohen@email.com", Password = "359ce90df34dbbb56326ffcd4ddb755fc651883651e1196cafe0be4d5c571765", Address = "Keren Hayesod Street 16, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 48.087094533852, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 245962634, Name = "Eliezer Glick", Phone = "058093862", Email = "eliezer.glick@email.com", Password = "ba0535b80407b53c6347ff29a5797681164cf4cf03690019579508bd57febebf", Address = "Shazar Boulevard 21, Jerusalem", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 31.169077415008424, TypeOfDistance = DistanceTypeEnum.AirDistance },
                new { Id = 648377993, Name = "Chava Shapiro", Phone = "054124530", Email = "chava.shapiro@email.com", Password = "c78468d681d7e6983700f50705d1c044f87fb1a414e884c5a95511fd9d0c206a", Address = "25 Shlomzion Hamalka Street, Jerusalem:", Latitude = 31.759595, Longitude = 35.215315, Position = PositionEnum.Volunteer, MaxResponseDistance = 7.707261513926877, TypeOfDistance = DistanceTypeEnum.AirDistance }
            };

            // Add the two new volunteers specified by the user
            var additionalVolunteers = new[]
            {
                new { Id = 218431161, Name = "Zevi Rubin", Phone = "0548428809", Email = "shanirubin6@gmail.com", Password = "8b69b3186291b9955fb5d53a498c5e8c50b966142d72e6de9fc027c1c30dba8d", Address = "Etsel 13, jerusalim", Latitude = 31.7511651, Longitude = 34.9819467, Position = PositionEnum.Volunteer, MaxResponseDistance = 30.0, TypeOfDistance = DistanceTypeEnum.WalkingDistance },
                new { Id = 327006516, Name = "Ma", Phone = "0548428809", Email = "shanirubin2@gmail.com", Password = "e8fce29f0c9d3d8faaf1e8539b5612b671213f129457add0e7a928e5dc9e1301", Address = "Etsel 13, jerusalem", Latitude = 31.7511651, Longitude = 34.9819467, Position = PositionEnum.Manager, MaxResponseDistance = 1000.0, TypeOfDistance = DistanceTypeEnum.AirDistance }
            };

            // Combine all volunteers
            var allVolunteers = volunteersFromXml.Concat(additionalVolunteers);

            // Create all volunteers
            foreach (var vol in allVolunteers)
            {
                Volunteer volunteer = new Volunteer
                {
                    Id = vol.Id,
                    Name = vol.Name,
                    Phone = vol.Phone,
                    Email = vol.Email,
                    Address = vol.Address,
                    Latitude = vol.Latitude,
                    Longitude = vol.Longitude,
                    Position = vol.Position,
                    Active = true,
                    MaxResponseDistance = vol.MaxResponseDistance,
                    TypeOfDistance = vol.TypeOfDistance,
                    Password = vol.Password,
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

        public static string Encrypt(string plainText)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

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
            "Medical Emergency", "Accident Assistance", "Vehicle Recovery", "Emergency Towing", "Stuck in Mud",
            "Engine Overheating", "Heavy Accident", "Emergency Tow", "Battery Issues", "Fire in Vehicle",
            "Fuel Assistance", "Flat Tire", "Lockout Assistance", "Vehicle Breakdown", "Flat Battery",
            "Vehicle Stuck in Traffic", "Accident Help", "Lost Keys", "Driving Assistance", "Tire Puncture",
            "Driving Safety", "Parking Assistance", "Car Repair Advice", "Vehicle Inspection", "Insurance Assistance",
            "Vehicle Jumpstart", "Call for Help", "Car Service Appointment", "Windshield Repair", "Lost Wallet",
            "Fuel Running Low", "Battery Jumpstart", "Car Alarm Issues", "Motorcycle Breakdown", "Lost GPS Navigation",
            "Roadside Assistance", "Headlight Malfunction", "Engine Check", "Car Wash", "Flat Tire Change",
            "Road Advice", "Car Repair Consultation", "Driving Safety Tips", "Insurance Policy Questions", "Basic Vehicle Maintenance",
            "Navigation Assistance", "Towing Insurance", "Tire Pressure Check", "Late Night Assistance", "General Car Troubleshooting"
        };


        public static void CreateCallEntries()
        {


            Random rand = new Random();
            DateTime now = Config.Clock;
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
                        MyExpiredTime = MyStartTime.AddMinutes(75);
                        break;

                    case int n when (n >= 10 && n < 20):
                        MyCallType = CallType.Medium_Urgency;
                        MyExpiredTime = MyStartTime.AddMinutes(120);
                        break;

                    case int n when (n >= 20 && n < 40):
                        MyCallType = CallType.General_Assistance;
                        MyExpiredTime = MyStartTime.AddHours(10);
                        break;

                    case int n when (n >= 40 && n < 50):
                        MyCallType = CallType.Non_Urgent;
                        MyStartTime = twoHoursAgo;
                        MyExpiredTime = Config.Clock.AddHours(-rand.Next(1, 2));
                        
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

        if (volunteersList == null || callsList == null)
            return;

        for (int i = 0; i < 49; i++)
        {
            var call = callsList[i % callsList.Count];
            var volunteer = volunteersList[i % volunteersList.Count];

            DateTime minTime = call.StartTime ?? Config.Clock;
            DateTime maxTime = call.ExpiredTime ?? Config.Clock;

            TimeSpan diff = maxTime - minTime - TimeSpan.FromHours(2);

            if (diff.TotalMinutes <= 0)
                continue;

            int totalMinutes = (int)diff.TotalMinutes;
            DateTime randomTime = minTime.AddMinutes(s_rand.Next(totalMinutes));

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
                call.RadioCallId,         
                volunteer.Id,             
                randomTime,               
                randomTime.AddDays(2),    
                typeOfEndTime             
            ));
        }
    }


    //public static void Do(IDal? dal) //stage 2
    public static void Do(bool createAssignments = true) //stage 4
    {
        s_dal = DalApi.Factory.Get; //stage 4
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();//stage 2

        Console.WriteLine("Initializing Volunteers...");
        CreateVolunteer.CreateVolunteerEntries();

        Console.WriteLine("Initializing Calls...");
        CreateCall.CreateCallEntries();

        /*if (createAssignments)
        {
            Console.WriteLine("Initializing Assignments...");
            createAssignments();
        }*/
    }

}



