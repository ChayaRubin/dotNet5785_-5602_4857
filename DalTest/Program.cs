namespace DalTest
{
    internal class Program
    {
        private static IStudent? s_dalStudent = new StudentImplementation(); //stage 1
        private static ICourse? s_dalCourse = new CourseImplementation(); //stage 1
        private static ILink? s_dalLink = new LinkImplementation(); //stage 1
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
            ChangeClockOrRiskRange,
            DisplayConfigVar,
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
                        Volunteer currentVol = CreateVolunteer(myId);
                        s_dalVolunteer.Create(currentVol);
                        break;
                    case "CallMenu":
                        Call currentCall = CreateCall(myId);
                        s_dalCall.Create(currentCall);
                        break;
                    case "AssignmentMenu":
                        Assignment currentAss = CreateAssignment(myId);
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
                    Volunteer currentVol = CreateVolunteer(myId);
                    s_dalVolunteer.Update(currentVol);
                    break;
                case "CallMenu":
                    Call currentCall = CreateCall(myId);
                    s_dalCall.Update(currentCall);
                    break;
                case "AssignmentMenu":
                    Assignment currentAss = CreateAssignment(myId);
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
                foreach (Submenu option in Enum.GetValues(typeof(Submenu)))
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
                            int myId = int.Parse(Console.ReadLine()!);
                            Delete(choice, myId);
                            break;

                        case ActionMenu.DeleteAll:
                            DeleteAll(choice);
                            break;

                        default:
                            Console.WriteLine("");
                            break;
                    }
                    Console.WriteLine("Enter a number:");
                    Enum.TryParse(Console.ReadLine(), out subChoice);
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
                                ConfigMenu();
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
    }
}
