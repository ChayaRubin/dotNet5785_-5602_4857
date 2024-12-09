using DO;
using Dal;
using DalApi;

namespace DalTest
{
    internal class Program
    {
        static readonly IDal s_dal = new DalList(); //stage 2


        //Create- Sends to different function to create different types based on users choice.
        private static void Create(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details");
                switch (choice)
                {
                    case "VolunteerMenu":
                        Console.Write("Enter ID: ");
                        int myId = int.Parse(Console.ReadLine()!);
                        Volunteer currentVol = CreateVolunteerFromUserInput(myId);
                        s_dal.Volunteer.Create(currentVol);
                        break;
                    case "CallMenu":
                        /* Console.Write("Enter ID: ");*/
                        int myId2 = int.Parse(Console.ReadLine()!);
                        Call currentCall = CreateCallFromUserInput();
                        s_dal.Call.Create(currentCall);
                        break;
                    case "AssignmentMenu":
                        Assignment currentAss = CreateAssignmentFromUserInput();
                        s_dal.Assignment.Create(currentAss);
                        break;
                }

            }
            catch (DalAlreadyExistsException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Item already exists.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
        }

        //Read Reads and prints a specific item based on users choice.
        private static void Read(string choice, int id)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerMenu":
                        Console.WriteLine(s_dal.Volunteer.Read(v => v.Id == id));
                        break;

                    case "CallMenu":
                        Console.WriteLine(s_dal.Call.Read(p => p.RadioCallId == id));
                        break;

                    case "AssignmentMenu":
                        Console.WriteLine(s_dal.Assignment.Read(a => a.Id == id));
                        break;
                }
            }
            catch (DalDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Item does not exist.");
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
        }

        //ReadAll- Reads and prints a specific list of items based on users choice.
        private static void ReadAll(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerMenu":
                        foreach (var item in s_dal!.Volunteer.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "CallMenu":
                        foreach (var item in s_dal!.Call.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "AssignmentMenu":
                        foreach (var item in s_dal!.Assignment.ReadAll())
                            Console.WriteLine(item);
                        break;
                }
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
        }


        // Update- Updates a specific item based on users choice.
        private static void Update(string choice)
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");
            int myId = int.Parse(Console.ReadLine()!);
            try
            {
                switch (choice)
                {
                    case "VolunteerMenu":
                        Volunteer currentVol = CreateVolunteerFromUserInput(myId);
                        s_dal.Volunteer.Update(currentVol);
                        break;
                    case "CallMenu":
                        Call currentCall = CreateCallFromUserInput();
                        s_dal.Call.Update(currentCall);
                        break;
                    case "AssignmentMenu":
                        Assignment currentAss = CreateAssignmentFromUserInput();
                        s_dal.Assignment.Update(currentAss);
                        break;
                }
            }
            catch (DalDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Item does not exist.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
        }

        //Delete- Deletes a specific item based on users choice.
        private static void Delete(string choice, int id)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerMenu":
                        s_dal.Volunteer.Delete(id);
                        break;
                    case "CallMenu":
                        s_dal.Call.Delete(id);
                        break;
                    case "AssignmentMenu":
                        s_dal.Assignment.Delete(id);
                        break;
                }
            }
            catch (DalDeletionImpossible ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Deletion impossible.");
            }
            catch (DalDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Item does not exist.");
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
        }

        //DeleteAll- Deletes a specific list of items based on users choice.
        private static void DeleteAll(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerMenu":
                        s_dal.Volunteer.DeleteAll();
                        break;
                    case "CallMenu":
                        s_dal.Call.DeleteAll();
                        break;
                    case "AssignmentMenu":
                        s_dal.Assignment.DeleteAll();
                        break;
                }
            }
            catch (DalDeletionImpossible ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Deletion impossible.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
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
            catch (DalDeletionImpossible ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Deletion impossible.");
            }
            catch (DalDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Item does not exist.");
            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
        }

        private static void DisplayAllData()
        {
            ReadAll("VolunteerMenu");
            Console.WriteLine();
            ReadAll("CallMenu");
            Console.WriteLine();
            ReadAll("AssignmentMenu");
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
                                Initialization.Do(s_dal); //stage 2
                                break;

                            case MainMenuOption.DisplayAllData:
                                DisplayAllData();
                                break;

                            case MainMenuOption.ConfigMenu:
                                ConfigMenuFunction();
                                break;

                            case MainMenuOption.ResetDatabase:
                                s_dal.ResetDB();//stage 2
                                break;

                            default:
                                Console.WriteLine("Try again");
                                break;
                        }
                        Console.WriteLine("Enter a number:");
                        Enum.TryParse(Console.ReadLine(), out choice);
                    }
                }

            }
            catch (DalInvalidOptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid option selected.");
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
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
            double Latitude = double.TryParse(Console.ReadLine(), out Latitude) ? Latitude : throw new DalFormatException("Invalid latitude");


            Console.WriteLine("Enter your Longitude (optinal)");
            double Longitude = double.TryParse(Console.ReadLine(), out Longitude) ? Longitude : throw new DalFormatException("Invalid latitude");


            Console.WriteLine("Enter position type, 1 for Manager, 2 for Volunteer");
            PositionEnum Position;
            Position = PositionEnum.TryParse(Console.ReadLine(), out Position) ? Position : throw new DalFormatException("Invalid Position.");

            Console.WriteLine("Enter true or false for Active status:");
            bool Active = bool.TryParse(Console.ReadLine(), out Active) && Active;

            Console.WriteLine("Enter max response distance (optinal)");
            double? MaxResponseDistance = double.TryParse(Console.ReadLine(), out var distance) ? (double?)distance : null;

            Console.WriteLine("Enter type of distance, 1 for air distance, 2 for walking distance,3 for driving distance");
            DistanceTypeEnum typeOfDistance;
            typeOfDistance = DistanceTypeEnum.TryParse(Console.ReadLine(), out typeOfDistance) ? typeOfDistance : throw new DalInvalidOptionException("Invalid Position.");

            return new Volunteer(VolunteerId, Name, Phone, Email, Password, Address, Latitude, Longitude, Position, Active, MaxResponseDistance, typeOfDistance);

        }

        static Call CreateCallFromUserInput()
        {

            int RadioCallId = 0;

            Console.WriteLine("Enter the call description (optinal)");
            string? Description = Console.ReadLine();

            Console.WriteLine("Enter Call type, 1 for Urgent, 2 for Medium_Urgency, 3 for General_Assistance, 4 for Non_Urgent");
            CallType callType = CallType.TryParse(Console.ReadLine(), out callType) ? callType : throw new DalInvalidOptionException("Invalid call type.");

            Console.WriteLine("Enter the event address");
            string Address = Console.ReadLine();

            Console.WriteLine("Enter the event latitude");
            double Latitude = double.TryParse(Console.ReadLine(), out Latitude) ? Latitude : throw new DalFormatException("Invalid latitude");

            Console.WriteLine("Enter the event longitude");
            double Longitude = double.TryParse(Console.ReadLine(), out Longitude) ? Longitude : throw new DalFormatException("Invalid longitude.");

            Console.WriteLine("Enter the Start Time of the event (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime StartTime))
                throw new DalFormatException("Event date is invalid!");
            Console.WriteLine(StartTime);


            Console.WriteLine("Enter the Expired Time of the call (in format dd/mm/yy hh:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime ExpiredTime))
                throw new DalFormatException("Event date is invalid!");
            Console.WriteLine(ExpiredTime);


            return new Call(RadioCallId, Description, callType, Address, Latitude, Longitude, StartTime, ExpiredTime);
        }


        static Assignment CreateAssignmentFromUserInput()
        {
            int AssignmentId = 0;

            Console.Write("Enter Call Id: ");
            int CallId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Volunteer Id: ");
            int volunteerId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Start Time of complain (YYYY-MM-DD HH:MM): ");
            DateTime EntryTime = DateTime.TryParse(Console.ReadLine(), out DateTime et) ? et : throw new DalFormatException("Event date is invalid!");
            Console.WriteLine($"Entry Time: {EntryTime}");

            Console.Write("Enter finish Time of complain (YYYY-MM-DD HH:MM): ");
            DateTime FinishCompletionTime = DateTime.TryParse(Console.ReadLine(), out DateTime fct) ? fct : throw new DalFormatException("Event date is invalid!");
            Console.WriteLine($"Finish Completion Time: {FinishCompletionTime}");

            Console.Write("Enter CallResolutionStatus type, 1 for Treated, 2 for SelfCanceled, 3 for AdminCanceled, 4 for Expired: ");
            if (!Enum.TryParse<CallResolutionStatus>(Console.ReadLine(), out CallResolutionStatus status) ||
                !Enum.IsDefined(typeof(CallResolutionStatus), status))
            {
                throw new DalInvalidOptionException("Invalid CallResolutionStatus value!");
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
            if (!Enum.TryParse(Console.ReadLine(), out ConfigMenu Input)) throw new DalInvalidOptionException("Invalid choice");

            try
            {
                while (Input is not ConfigMenu.Exit)
                {
                    switch (Input)
                    {
                        case ConfigMenu.AdvanceClockByMinute:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                            break;

                        case ConfigMenu.AdvanceClockByHour:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                            break;

                        case ConfigMenu.AdvanceClockByDay:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                            break;

                        case ConfigMenu.AdvanceClockByMonth:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                            break;

                        case ConfigMenu.AdvanceClockByYear:
                            s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                            break;

                        case ConfigMenu.DisplayClock:
                            Console.WriteLine($"Clock: {s_dal.Config.Clock}");
                            break;

                        case ConfigMenu.ChangeRiskRange:
                            Console.WriteLine("Enter a time span in the format [hh:mm:ss]:");
                            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
                                throw new DalFormatException("Invalid time format.");
                            s_dal.Config.RiskRange = newRiskRange;
                            break;

                        case ConfigMenu.DisplayRiskRange:
                            Console.WriteLine($"Risk Range: {s_dal.Config.RiskRange}");
                            break;

                        case ConfigMenu.Reset:
                            s_dal.Config.Reset();
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }

                    // הצגת התפריט מחדש
                    Console.WriteLine("\nConfig Menu:");
                    foreach (ConfigMenu option in Enum.GetValues(typeof(ConfigMenu)))
                    {
                        Console.WriteLine($"{(int)option}. {option}");
                    }
                    Console.Write("Select an option: ");
                    while (!Enum.TryParse(Console.ReadLine(), out Input) || !Enum.IsDefined(typeof(ConfigMenu), Input))
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.Write("Select an option: ");
                    }
                }
            }
            catch (DalFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Invalid data format.");
            }
        }
    }
}
