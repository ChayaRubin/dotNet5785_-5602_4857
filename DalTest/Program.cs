using DO;
using Dal;
using DalApi;

namespace DalTest
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

        private enum MainMenuOption
        {
            Exit,
            VolunteerMenu,
            CallMenu,
            AssignmentMenu,
            InitializeData,
            DisplayAllData,
            ConfigMenu,
            ResetDatabase
        }
        private enum ActionMenu
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }
        private enum ConfigMenu
        {
            Exit,
            AdvanceClockByMinute,
            AdvanceClockByHour,
            AdvanceClockByDay,
            AdvanceClockByMonth,
            AdvanceClockByYear,
            DisplayClock,
            ChangeRiskRange,
            DisplayRiskRange,
            Reset
        }

        private static void Create(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details");
                Console.Write("Enter ID: ");
                int myId = int.Parse(Console.ReadLine()!);
                switch (choice)
                {
                    case "VolunteerMenu":
                        Volunteer currentVol = CreateVolunteerFromUserInput(myId);
                        s_dalVolunteer.Create(currentVol);
                        break;
                    case "CallMenu":
                        Call currentCall = CreateCallFromUserInput(myId);
                        s_dalCall.Create(currentCall);
                        break;
                    case "AssignmentMenu":
                        Assignment currentAss = CreateAssignmentFromUserInput(myId);
                        s_dalAssignment.Create(currentAss);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create function: {ex.Message}");
            }
        }

        private static void Read(string choice, int id)
        {
            switch (choice)
            {
                case "VolunteerMenu":
                    s_dalVolunteer.Read(id);
                    break;
                case "CallMenu":
                    s_dalCall.Read(id);
                    break;
                case "AssignmentMenu":
                    s_dalAssignment.Read(id);
                    break;
            }
        }
        private static void ReadAll(string choice)
        {
            switch (choice)
            {
                case "VolunteerMenu":
                    foreach (var item in s_dalVolunteer!.ReadAll())
                        Console.WriteLine(item);
                    break;
                case "CallMenu":
                    foreach (var item in s_dalCall!.ReadAll())
                        Console.WriteLine(item);
                    break;
                case "AssignmentMenu":
                    foreach (var item in s_dalAssignment!.ReadAll())
                        Console.WriteLine(item);
                    break;
            }
        }

        private static void Update(string choice)
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");
            int myId = int.Parse(Console.ReadLine()!);
            switch (choice)
            {
                case "VolunteerMenu":
                    Volunteer currentVol = CreateVolunteerFromUserInput(myId);
                    s_dalVolunteer.Update(currentVol);
                    break;
                case "CallMenu":
                    Call currentCall = CreateCallFromUserInput(myId);
                    s_dalCall.Update(currentCall);
                    break;
                case "AssignmentMenu":
                    Assignment currentAss = CreateAssignmentFromUserInput(myId);
                    s_dalAssignment.Update(currentAss);
                    break;
            }
        }

        private static void Delete(string choice,int id)
        {
            switch (choice)
            {
                case "VolunteerMenu":
                    s_dalVolunteer.Delete(id);
                    break;
                case "CallMenu":
                    s_dalCall.Delete(id);
                    break;
                case "AssignmentMenu":
                    s_dalAssignment.Delete(id);
                    break;
            }
        }

        private static void DeleteAll(string choice)
        {
            switch (choice)
            {
                case "VolunteerMenu":
                    s_dalVolunteer.DeleteAll();
                    break;
                case "CallMenu":
                    s_dalCall.DeleteAll();
                    break;
                case "AssignmentMenu":
                    s_dalAssignment.DeleteAll();
                    break;
            }
        }

        private static void EntityMenu(string choice)
        {
            try
            {
                Console.WriteLine("Enter a number:");
                foreach (ActionMenu option in Enum.GetValues(typeof(ActionMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
                ActionMenu actionChoice;
                Enum.TryParse(Console.ReadLine(), out actionChoice);
                while (actionChoice != ActionMenu.Exit)
                {
                    switch (actionChoice)
                    {
                        case ActionMenu.Create:
                            Create(choice);
                            break;

                        case ActionMenu.Read:
                            Console.WriteLine("Enter your ID");
                            int myId = int.Parse(Console.ReadLine()!);
                            Read(choice, myId);
                            break;

                        case ActionMenu.ReadAll:
                            ReadAll(choice);
                            break;

                        case ActionMenu.Update:
                            Update(choice);
                            break;

                        case ActionMenu.Delete:
                            Console.WriteLine("Enter ID");
                            int myIdToDelete = int.Parse(Console.ReadLine()!);
                            Delete(choice, myIdToDelete);
                            break;

                        case ActionMenu.DeleteAll:
                            DeleteAll(choice);
                            break;

                        default:
                            Console.WriteLine("");
                            break;
                    }
                    Console.WriteLine("Enter a number:");
                    Enum.TryParse(Console.ReadLine(), out actionChoice);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"");
            }
        }

        private static void DisplayAllData()
        {
            ReadAll("VolunteerSubmenu");
            ReadAll("CallSubmenu");
            ReadAll("AssignmentSubmenu");
        }

        private static void ResetDatabase()
        {
            s_dalConfig.Reset(); 
            s_dalVolunteer.DeleteAll();
            s_dalCall.DeleteAll();
            s_dalAssignment.DeleteAll();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter a number:");
                foreach (MainMenuOption option in Enum.GetValues(typeof(MainMenuOption)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                MainMenuOption choice;
                Enum.TryParse(Console.ReadLine(), out choice);

                {
                    while (choice is not MainMenuOption.Exit)
                    {
                        switch (choice)
                        {
                            case MainMenuOption.VolunteerMenu:
                            case MainMenuOption.CallMenu:
                            case MainMenuOption.AssignmentMenu:
                                string myChoice = choice.ToString();
                                EntityMenu(myChoice);
                                break;

                            case MainMenuOption.InitializeData:
                                Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                                break;

                            case MainMenuOption.DisplayAllData:
                                DisplayAllData();
                                break;

                            case MainMenuOption.ConfigMenu:
                                ConfigMenuFunction();
                                break;

                            case MainMenuOption.ResetDatabase:
                                ResetDatabase();
                                break;

                            default:
                                Console.WriteLine("בחירה לא חוקית, נסה שנית.");
                                break;
                        }
                        Console.WriteLine("Enter a number:");
                        Enum.TryParse(Console.ReadLine(), out choice);
                    }



                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

        }

        static Volunteer CreateVolunteerFromUserInput(int Id)
        {
            int VolunteerId = Id;

            Console.WriteLine("Enter your name");
            string Name = Console.ReadLine();

            Console.WriteLine("Enter your phone number");
            string Phone = Console.ReadLine();

            Console.WriteLine("Enter your email address");
            string Email = Console.ReadLine();

            Console.WriteLine("Enter your password (optinal)");
            string? Password = Console.ReadLine();

            Console.WriteLine("Enter your address (optinal)");
            string? Address = Console.ReadLine();

            Console.WriteLine("Enter your Latitude (optinal)");
            double Latitude = double.TryParse(Console.ReadLine(), out Latitude) ? Latitude : throw new FormatException("Invalid latitude");


            Console.WriteLine("Enter your Longitude (optinal)");
            double Longitude = double.TryParse(Console.ReadLine(), out  Longitude) ? Longitude : throw new FormatException("Invalid latitude");


            Console.WriteLine("Enter position type, 1 for Manager, 2 for Volunteer");
            PositionEnum Position;
            Position = PositionEnum.TryParse(Console.ReadLine(), out Position) ? Position : throw new FormatException("Invalid Position.");

            Console.WriteLine("Enter true or false for Active status:");
            bool Active = bool.TryParse(Console.ReadLine(), out Active) && Active;

            Console.WriteLine("Enter max response distance (optinal)");
            double? MaxResponseDistance = double.TryParse(Console.ReadLine(), out var distance) ? (double?)distance : null;

            Console.WriteLine("Enter type of distance, 1 for air distance, 2 for walking distance,3 for driving distance");
            DistanceTypeEnum typeOfDistance;
            typeOfDistance = DistanceTypeEnum.TryParse(Console.ReadLine(), out typeOfDistance) ? typeOfDistance : throw new FormatException("Invalid Position.");
            
            return new Volunteer(VolunteerId, Name, Phone, Email, Password, Address, Latitude, Longitude, Position, Active, MaxResponseDistance , typeOfDistance);

        }

        static Call CreateCallFromUserInput(int Id)
        {

            int RadioCallId = Id;

            Console.WriteLine("Enter the call description (optinal)");
            string? Description = Console.ReadLine();

            Console.WriteLine("Enter Call type, 1 for Urgent, 2 for Medium_Urgency, 3 for General_Assistance, 4 for Non_Urgent");
            CallType callType = CallType.TryParse(Console.ReadLine(), out callType) ? callType : throw new FormatException("Invalid call type.");

            Console.WriteLine("Enter the event address");
            string Address = Console.ReadLine();

            Console.WriteLine("Enter the event latitude");
            double Latitude = double.TryParse(Console.ReadLine(), out Latitude) ? Latitude : throw new FormatException("Invalid latitude");

            Console.WriteLine("Enter the event longitude");
            double Longitude = double.TryParse(Console.ReadLine(), out  Longitude) ? Longitude : throw new FormatException("Invalid longitude.");

            Console.WriteLine("Enter the Start Time of the event (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime StartTime))
                throw new FormatException("Event date is invalid!");
            Console.WriteLine(StartTime);


            Console.WriteLine("Enter the Expired Time of the call (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime ExpiredTime))
                throw new FormatException("Event date is invalid!");
            Console.WriteLine(ExpiredTime);


            return new Call(RadioCallId, Description, callType, Address, Latitude, Longitude, StartTime, ExpiredTime);
        }


        static Assignment CreateAssignmentFromUserInput(int Id)
        {
            int AssignmentId = Config.getNextAssignmentId;

            Console.Write("Enter Call Id: ");
            int CallId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Volunteer Id: ");
            int volunteerId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Start Time of complain (YYYY-MM-DD HH:MM): ");
            DateTime EntryTime = DateTime.TryParse(Console.ReadLine(), out DateTime et) ? et : throw new FormatException("Event date is invalid!");
            Console.WriteLine($"Entry Time: {EntryTime}");

            Console.Write("Enter finish Time of complain (YYYY-MM-DD HH:MM): ");
            DateTime FinishCompletionTime = DateTime.TryParse(Console.ReadLine(), out DateTime fct) ? fct : throw new FormatException("Event date is invalid!");
            Console.WriteLine($"Finish Completion Time: {FinishCompletionTime}");

            Console.Write("Enter CallResolutionStatus type, 1 for Treated, 2 for SelfCanceled, 3 for AdminCanceled, 4 for Expired: ");
            if (!Enum.TryParse<CallResolutionStatus>(Console.ReadLine(), out CallResolutionStatus status) ||
                !Enum.IsDefined(typeof(CallResolutionStatus), status))
            {
                throw new FormatException("Invalid CallResolutionStatus value!");
            }
            Console.WriteLine($"Call Resolution Status: {status}");

            return new Assignment(AssignmentId, CallId, volunteerId, EntryTime, FinishCompletionTime, status);
        }

        private static void ConfigMenuFunction()
        {

            Console.WriteLine("Config Menu:");
            foreach (ConfigMenu option in Enum.GetValues(typeof(ConfigMenu)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            Console.Write("Select an option: ");
            if (!Enum.TryParse(Console.ReadLine(), out ConfigMenu Input)) throw new FormatException("Invalid choice");

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

                    case ConfigMenu.AdvanceClockByDay:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                        break;

                    case ConfigMenu.AdvanceClockByMonth:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;

                    case ConfigMenu.AdvanceClockByYear:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                        break;

                    case ConfigMenu.DisplayClock:
                        Console.WriteLine($"Clock: {s_dalConfig.Clock}");
                        break;

                    case ConfigMenu.ChangeRiskRange:
                        Console.WriteLine("Enter a time span in the format [hh:mm:ss]:");
                        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
                            throw new FormatException("Invalid time format.");
                        Config.RiskRange = newRiskRange;
                        break;

                    case ConfigMenu.DisplayRiskRange:
                        break;

                    case ConfigMenu.Reset:
                        s_dalConfig.Reset();
                        break;
                }
            }


        }

    }
}
