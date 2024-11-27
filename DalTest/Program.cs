using DO;
using Dal;
using DalApi;

namespace DalTest
{
    internal class Program
    {

        static Call CreateCallFromUserInput(int Id)
        {

            int RadioCallId = Id;

            console.WriteLine("Enter the call description (optinal)");
            string? Description = console.ReadLine();

            Console.WriteLine("Enter Call type, 1 for Urgent, 2 for Medium_Urgency, 3 for General_Assistance, 4 for Non_Urgent");
            int callType = int.TryParse(Console.ReadLine(), out int temp) ? temp : throw new FormatException("Invalid call type.");

            console.WriteLine("Enter the event address");
            string Address = console.ReadLine();

            console.WriteLine("Enter the event latitude");
            double Latitude = double.TryParse(Console.ReadLine(), out double temp) ? temp : throw new FormatException("Invalid latitude");

            console.WriteLine("Enter the event longitude");
            double Longitude = double.TryParse(Console.ReadLine(), out double temp) ? temp : throw new FormatException("Invalid longitude.");

            Console.WriteLine("enter the Start Time of the event (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime st)) throw new FormatException("event date is invalid!");
            DateTime StartTime = Console.WriteLine(st);

            Console.WriteLine("enter the Expired Time of the call (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime et)) throw new FormatException("event date is invalid!");
            DateTime ExpiredTime = Console.WriteLine(et);

            return new Call(RadioCallId, Description, CallType, Address, Latitude, Longitude, StartTime, ExpiredTime);
        }

        static Call CreateVolunteerFromUserInput(int Id)
        {
            int Id = Id;

            console.WriteLine("Enter your name");
            string Name = console.ReadLine();

            console.WriteLine("Enter your phone number");
            string Phone = console.ReadLine();

            console.WriteLine("Enter your email address");
            string Email = console.ReadLine();

            console.WriteLine("Enter your password (optinal)");
            string? Password = console.ReadLine();

            console.WriteLine("Enter your address (optinal)");
            string? Address = console.ReadLine();

            console.WriteLine("Enter your Latitude (optinal)");
            double? Latitude = double.TryParse(Console.ReadLine(), out double temp) ? temp : throw new FormatException("Invalid latitude");


            console.WriteLine("Enter your Longitude (optinal)");
            double? Longitude = double.TryParse(Console.ReadLine(), out double temp) ? temp : throw new FormatException("Invalid latitude");


            Console.WriteLine("Enter position type, 1 for Manager, 2 for Volunteer");
            PositionEnum Position = console.ReadLine();
            int.TryParse(Console.ReadLine(), out int temp) ? temp : throw new FormatException("Invalid Position.");

            Console.WriteLine("Enter true or false for Active status:");
            bool Active = bool.TryParse(Console.ReadLine(), out Active) && Active;

            Console.WriteLine("Enter max response distance (optinal)");
            double? MaxResponseDistance = console.ReadLine();

            return new Volunteer(Id, Name, Phone, Email, Password, Address, Latitude, Longitude, Position, Active, MaxResponseDistance);

        }

        static Call CreateAssignmentFromUserInput(int Id)
        {
            int Id = config.getNextAssignmentId;

            Console.Write("Enter Call Id: ");
            int callId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Volunteer Id: ");
            int volunteerId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Start Time of complain (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime et)) throw new FormatException("event date is invalid!");
            DateTime EntryTime = Console.WriteLine(et);

            Console.Write("Enter finish Time of complain (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime fct)) throw new FormatException("event date is invalid!");
            DateTime FinishCompletionTime = Console.WriteLine(fct);

            Console.WriteLine("Enter CallResolutionStatus type, 1 for Treated, 2 for SelfCanceled, 3 for AdminCanceled, 4 for Expired");
            CallResolutionStatus CallResolutionStatus = Console.WriteLine();

            return new Assignment(Id, CallId, EntryTime, FinishCompletionTime, CallResolutionStatus);
        }

        private static void ConfigMenu()
        {
            try
            {
                Console.WriteLine("Config Menu:");
                foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
                Console.Write("Select an option: ");
                if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput)) throw new FormatException("Invalid choice");

                while (Input is not ConfigMenu.Exit)
                {
                    switch (Input)
                    {
                        case ConfigMenu.AdvanceClockByMinute:
                            s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                            break;

                        case ConfigMenu.AdvanceClockByHour:
                            s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                            break;

                        case ConfigMenu.AdvanceClockByHour:
                            s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                            break;

                        case ConfigMenu.AdvanceClockByMonth:
                            s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                            break;

                        case ConfigMenu.AdvanceClockByHour:
                            s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                            break;

                        case ConfigMenu.DisplayClock:
                            Console.WriteLine($"Clock: {s_dal.Config.Clock()}");
                            break;

                        case ConfigMenu.ChangeSpan:
                            Console.WriteLine("Enter a time span in the format [hh:mm:ss]:");
                            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
                                throw new FormatException("Invalid time format.");
                            Config.RiskRange = newRiskRange;
                            break;

                        case ConfigMenu.DisplaySpan:
                            Console.WriteLine($"RiskRange updated to: {s_dalConfig.GetRiskRange()}");
                            break;

                        case ConfigMenu.Reset:
                            s_dal.Config.Reset();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfigSubmenu function: {ex.Message}");
            }
        }

    }
}
