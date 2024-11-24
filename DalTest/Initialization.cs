
namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
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
            Random rand = new Random();
            int MIN_ID = 100000000;
            int MAX_ID = 999999999;
            int MAX_DISTANCE = 50;
            HashSet<string> usedPhoneNumbers = new HashSet<string>(); // ודא שהטלפונים יהיו ייחודיים

            for (int i = 0; i < 16; i++)
            {
                string name = names[rand.Next(names.Length)];
                int id = rand.Next(MIN_ID, MAX_ID);
                string email = $"{name.ToLower().Replace(" ", ".")}@email.com";
                string phoneNumber = GenerateUniquePhoneNumber(rand, usedPhoneNumbers);
                string address = addresses[rand.Next(addresses.Length)];
                double latitude = latitudes[rand.Next(latitudes.Length)];
                double longitude = longitudes[rand.Next(longitudes.Length)];
                PositionEnum position = i == 0 ? PositionEnum.Manager : PositionEnum.Volunteer;
                string password = GenerateStrongPassword();
                string encryptedPassword = EncryptPassword(password);

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

                Console.WriteLine($"Created {position} {name} with encrypted password: {encryptedPassword}");
            }
        }

        // פונקציה להבטיח מספר טלפון ייחודי-gpt
        private static string GenerateUniquePhoneNumber(Random rand, HashSet<string> usedPhoneNumbers)
        {
            string phoneNumber;
            do
            {
                phoneNumber = "05" + rand.Next(1000000, 9999999).ToString();
            } while (usedPhoneNumbers.Contains(phoneNumber));

            usedPhoneNumbers.Add(phoneNumber);
            return phoneNumber;
        }

        // פונקציה ליצירת סיסמה חזקה--gpt
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

        // פונקציה להצפנת סיסמה ב-SHA256-gpt
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
            "Car Breakdown", "Accident Assistance", "Flat Tire", "Fuel Assistance", "Stuck in Traffic",
            "Engine Overheating", "Battery Issues", "Lockout Assistance", "Lost Keys", "Emergency Towing",
            "Medical Emergency", "Mechanical Repair", "Driving Assistance", "Flat Battery", "Stuck Vehicle",
            "Traffic Incident", "Vehicle Recovery", "Vehicle Jumpstart", "Fuel Delivery", "Accident Help",
            "Emergency Response", "Breakdown Help", "Tire Change", "Vehicle Inspection", "Parking Assistance",
            "Stalled Vehicle", "Safety Check", "Battery Jumpstart", "Accident Coordination", "Emergency Cleanup",
            "Tow Truck Dispatch", "Tire Puncture", "Car Repair Advice", "Fuel Running Low", "Engine Check",
            "Motorcycle Breakdown", "Vehicle Stuck in Mud", "Vehicle Lockout", "Emergency Tow", "Speeding Violation",
            "Driving Safety", "Insurance Assistance", "Emergency Roadside Service", "Vehicle Inspection Service", "Call for Help",
            "Late Night Assistance", "Urgent Breakdown", "Nighttime Recovery", "Heavy Traffic", "Accident Avoidance"
        };

        private static void CreateCallEntries()
        {
            int currentIndex = 0;
            for (int i = 0; i < 50; i++)
            {
                int MyRadioCallId = Config.NextCallId;//????????
                string MyDescription = callDescriptions[currentIndex];
                string MyAddress = callAddresses[currentIndex];
                double MyLatitude = callLatitudes[currentIndex];
                double MyLongitude = callLongitudes[currentIndex];

                currentIndex++;

                //Calls.Add(new Call(Config.NextAssignmentId, addresses[i], coordinates[i].Latitude, coordinates[i].Longitude, Config.Clock, callTypes[rand.Next(callTypes.Length)]));
                call call = new call
                {
                    RadioCallId = MyRadioCallId,
                    Description = MyDescription,
                    Address = MyAddress,
                    Latitude = MyLatitude,
                    Longitude = MyLongitude,
                    StartTime = MyStartTime,
                    ExpiredTime = MyExpiredTime,

            };
            }
        }
    }   
}
