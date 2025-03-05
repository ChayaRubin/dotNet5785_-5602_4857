using BlApi;
using BO;

class Program
{
    private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;


    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Emergency Response System");
        MainLoginMenu();
    }

    private static void MainLoginMenu()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("\nLogin Menu:");
                Console.Write("Enter username: ");
                string username = Console.ReadLine() ?? "";
                Console.Write("Enter password: ");
                string password = Console.ReadLine() ?? "";

                // Assuming Login method returns role as string
                string role = s_bl.Volunteer.Login(username, password);

                switch (role.ToLower())
                {
                    case "manager":
                        ManagerMainMenu();
                        break;
                    case "volunteer":
                        VolunteerMainMenu(username);
                        break;
                    default:
                        Console.WriteLine("Invalid login credentials. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
        }
    }

    private static void ManagerMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\nManager Main Menu:");
            //Console.WriteLine("1. Call Management Screen");
            //Console.WriteLine("2. Single Call Management Screen");
            Console.WriteLine("3. Add Call Screen");
            Console.WriteLine("4. Volunteer Management Screen");
            Console.WriteLine("5. Single Volunteer Management Screen");
            Console.WriteLine("6. Add Volunteer Screen");
            Console.WriteLine("7. Delete Call");
            Console.WriteLine("8. Logout");
            Console.WriteLine("9. Delete a volunteer");
            Console.WriteLine("10. get list of volunteers");
            Console.WriteLine("Testing Admin Interface");

            // Test GetSystemClock Method
            Console.WriteLine("System Clock: ");

            
            Console.Write("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        CallManagementScreen();
                        break;
                    case 2:
                        SingleCallManagementScreen();
                        break;
                    case 3:
                        AddCallScreen();
                        break;
                    case 4:
                        VolunteerManagementScreen();
                        break;
                    case 5:
                        SingleVolunteerManagementScreen();
                        break;
                    case 6:
                        AddVolunteerScreen();
                        break;
                    case 7:
                        DeleteCallScreen();
                        break;
                    case 9:
                        DeleteVolunteerOption();
                        break;
                    case 10:
                        // Option 1: Get list of all volunteers
                        DisplayVolunteersList();
                        break;
                    case 8:
                        return; // Return to login menu
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void VolunteerMainMenu(string username)
    {
        // Get volunteer details to have context
        //var volunteer? = s_bl.Volunteer.GetVolunteerDetails(username);
        BO.Volunteer? volunteer = s_bl.Volunteer.GetVolunteerDetails(username);


        while (true)
        {
            Console.WriteLine("\nVolunteer Main Menu:");
            Console.WriteLine("1. Select Call for Handling");
            //Console.WriteLine("2. Volunteer Call History");
            Console.WriteLine("2. Get Closed calls by valunteer");
            Console.WriteLine("3. Get open calls by valunteer");
            Console.WriteLine("4. Get valunteer Deatails");
            Console.WriteLine("5. update volunteer details");
            Console.WriteLine("6. Logout");
            Console.WriteLine("7. TestRiskTimeRange");
            Console.WriteLine("8. TestAdvanceSystemClock");
            Console.Write("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        //SelectCallForHandlingScreen(volunteer);
                        break;

                    case 2:
                        VolunteerClosedCallsScreen(volunteer.Id);
                        break;

                    case 3:
                        //VolunteerOpenCallsScreen(volunteer);
                        break;

                    case 4:
                        Console.WriteLine(volunteer);
                        break;

                    case 5:
                        UpdateVolunteerDetails(volunteer);
                        string id = volunteer.Id.ToString();
                        s_bl.Volunteer.UpdateVolunteerDetails(id, volunteer);
                        Console.WriteLine("Volunteer updated successfully");
                        break;
                    case 7:
                        TestAdvanceSystemClock();
                        break;
                    case 8:
                        TestRiskTimeRange();
                        break;
                    /*case 9:
                        TestResetDatabase();
                        return;
                    case 10:
                        TestInitializeDatabase();*/
                    //return
                    case 6:
                        return; // Return to login menu
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Placeholder methods for specific screens
    private static void CallManagementScreen()
    {
        Console.WriteLine("Call Management Screen");
        // Implement detailed call management logic
        // List all calls, filter, sort, etc.
    }

    private static void SingleCallManagementScreen()
    {
        Console.WriteLine("Single Call Management Screen");
        // Implement logic for managing a specific call
        // Details, update, assign, etc.
    }

    private static void AddCallScreen()
    {
        try
        {
            Console.WriteLine("Add Call Screen");

            // Collecting call details from the user
            Console.Write("Enter the call description: ");
            string description = Console.ReadLine();

            Console.Write("Enter the address for the call: ");
            string address = Console.ReadLine();

            Console.Write("Enter the call type: ");
            CallTypeEnum callType = (CallTypeEnum)Enum.Parse(typeof( CallTypeEnum), Console.ReadLine());


            // Collecting Latitude and Longitude from the user
            Console.Write("Enter the Latitude: ");
            double latitude = double.Parse(Console.ReadLine());

            Console.Write("Enter the Longitude: ");
            double longitude = double.Parse(Console.ReadLine());

            // Collecting call type
           /* Console.Write("Enter the call type (e.g., Emergency, Routine): ");
            string callTypeInput = Console.ReadLine();
            CallTypeEnum callType;
            if (!Enum.TryParse(callTypeInput, true, out callType))
            {
                Console.WriteLine("Invalid call type. Setting to 'Routine' by default.");
                callType = CallTypeEnum.Routine;  // Set a default value if the input is invalid
            }*/

            // Collecting the call status
            Console.Write("Enter the call status (e.g., Open, Closed): ");
            string statusInput = Console.ReadLine();
            CallStatus status;
            if (!Enum.TryParse(statusInput, true, out status))
            {
                Console.WriteLine("Invalid status. Setting to 'Open' by default.");
                status = CallStatus.Open;  // Set a default value if the input is invalid
            }

            // Set MaxEndTime (Optional)
            DateTime? maxEndTime = null;
            Console.Write("Enter the MaxEndTime (leave blank if none): ");
            string maxEndTimeInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(maxEndTimeInput))
            {
                if (DateTime.TryParse(maxEndTimeInput, out DateTime parsedMaxEndTime))
                {
                    maxEndTime = parsedMaxEndTime;
                }
                else
                {
                    Console.WriteLine("Invalid MaxEndTime format.");
                }
            }

            // Create the new call object
            Call newCall = new Call
            {
                Description = description,
                Type = callType,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                OpenTime = DateTime.Now, // Current time as OpenTime
                MaxEndTime = maxEndTime,
                Status = status,
                // s_bl.Volunteer.AddVolunteer(newVolunteer);

            };
            s_bl.Call.AddCall(newCall);
            // Validate the call
            /*CallManager.ValidateCall(newCall);*/

            // Add the call to the system
            /* CallImplementation callImplementation = new CallImplementation();*/
            /*callImplementation.AddCall(newCall);*/

            Console.WriteLine("New call added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding the call: {ex.Message}");
        }
    }



    private static void VolunteerManagementScreen()
    {
        Console.WriteLine("Volunteer Management Screen");
        // Implement logic for managing volunteers
        // List, filter, sort volunteers
    }

    private static void SingleVolunteerManagementScreen()
    {
        Console.WriteLine("Single Volunteer Management Screen");
        // Implement logic for managing a specific volunteer
        // Update details, view history, etc.
    }

    private static void AddVolunteerScreen()
    {
        Console.WriteLine("Add Volunteer Screen");
        var newVolunteer = new Volunteer
        {
            Id = GetId(),
            FullName = GetInput("Enter full name "),
            PhoneNumber = GetInput("Enter phone number "),
            Email = GetInput("Enter email "),
            Password = GetInput("Enter password "),
            CurrentAddress = GetInput("Enter current address "),
            Role = PositionEnum.Volunteer,
            IsActive = true,
            TypeOfDistance = DistanceType.DrivingDistance // Default value
        };

        if (GetCoordinate("latitude", out double lat))
            newVolunteer.Latitude = lat;

        if (GetCoordinate("longitude", out double lon))
            newVolunteer.Longitude = lon;

        Console.Write("Enter maximum distance (in km, press Enter for default): ");
        if (double.TryParse(Console.ReadLine(), out double maxDist))
            newVolunteer.MaxDistance = maxDist;

        Console.WriteLine("Select distance type (1: AirDistance, 2: WalkingDistance, 3: DrivingDistance): ");
        if (int.TryParse(Console.ReadLine(), out int distType) && distType >= 1 && distType <= 3)
            newVolunteer.TypeOfDistance = (DistanceType)(distType - 1);

        s_bl.Volunteer.AddVolunteer(newVolunteer);
        Console.WriteLine("Volunteer added successfully");
    }

    public static List<BO.ClosedCallInList> VolunteerClosedCallsScreen(int volunteerId)
    {
        try
        {
            // קריאה לפונקציה שתביא את הקריאות הסגורות שטופלו על ידי המתנדב
            //var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId);
            //var closedCalls = s_bl..CallManager.GetCallsForVolunteer(volunteerId);//????????????????//
            //var closedCalls = s_bl.CallManager.GetCallsForVolunteer<BO.ClosedCallInList>(volunteerId, null, null, false);
            var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId);


            // החזרת רשימת הקריאות הסגורות
            Console.WriteLine(closedCalls);
            return closedCalls.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching closed calls: {ex.Message}");
            return new List<BO.ClosedCallInList>();
        }
    }




    /*private static void VolunteerOpenCallsScreen(Volunteer volunteer)
    {
        Console.WriteLine("Volunteer Open Calls Screen");
    }*/



    private static bool GetCoordinate(string coordinateType, out double coordinate)
    {
        coordinate = 0; // Default value
        Console.Write($"Enter {coordinateType}: ");
        string input = Console.ReadLine();

        // Check if the input can be parsed into a valid double
        if (double.TryParse(input, out double parsedCoordinate))
        {
            coordinate = parsedCoordinate;
            return true;
        }
        else
        {
            Console.WriteLine($"Invalid {coordinateType}. Please enter a valid number.");
            return false;
        }
    }

    private static string GetInput(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }
    private static int GetId()
    {
        Console.Write("Enter Id: ");
        string input = Console.ReadLine() ?? ""; // מקבל קלט מהמקלדת

        // מנסה להמיר את הקלט לאינט
        if (int.TryParse(input, out int id))
        {
            return id; // מחזיר את ה-ID אם ההמרה הצליחה
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid integer.");
            return -1; // מחזיר ערך ברירת מחדל במקרה של קלט לא תקין
        }
    }

    private static void UpdateVolunteerDetails(Volunteer volunteer)
    {
        Console.Write($"Enter new FullName (current: {volunteer.FullName}, press Enter to keep current): ");
        string updatedName = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(updatedName))
            volunteer.FullName = updatedName;

        Console.Write($"Enter new PhoneNumber (current: {volunteer.PhoneNumber}, press Enter to keep current): ");
        string updatedPhoneNumber = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(updatedPhoneNumber))
            volunteer.PhoneNumber = updatedPhoneNumber;

        Console.Write($"Enter new Email (current: {volunteer.Email}, press Enter to keep current): ");
        string updatedEmail = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(updatedEmail))
            volunteer.Email = updatedEmail;

        Console.Write($"Enter new Password (current: {volunteer.Password}, press Enter to keep current): ");
        string updatedPassword = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(updatedPassword))
            volunteer.Password = updatedPassword;

        Console.Write($"Enter new Address (current: {volunteer.CurrentAddress}, press Enter to keep current): ");
        string updatedAddress = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(updatedAddress))
            volunteer.CurrentAddress = updatedAddress;

        Console.Write($"Enter new latitude (current: {volunteer.Latitude}, press Enter to keep current): ");
        string latInput = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(latInput) && double.TryParse(latInput, out double newLat))
            volunteer.Latitude = newLat;

        Console.Write($"Enter new longitude (current: {volunteer.Longitude}, press Enter to keep current): ");
        string lonInput = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(lonInput) && double.TryParse(lonInput, out double newLon))
            volunteer.Longitude = newLon;
    }

    private static void DeleteCallScreen()
    {
        try
        {
            Console.Write("Enter the Call ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int callId))
            {
                Console.WriteLine("Invalid Call ID.");
                return;
            }

            // Call the DeleteCall method from CallImplementation
            //var callImplementation = new CallImplementation();
            s_bl.Call.DeleteCall(callId);

            Console.WriteLine("Call deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting call: {ex.Message}");
        }
    }

    private static void DeleteVolunteerOption()
    {
        Console.Write("Enter the ID of the volunteer to delete: ");
        if (int.TryParse(Console.ReadLine(), out int volunteerId))
        {
            try
            {
                s_bl.Volunteer.DeleteVolunteer(volunteerId);
                Console.WriteLine($"Volunteer with ID {volunteerId} has been successfully deleted.");
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID. Please enter a valid number.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void DisplayVolunteersList()
    {
        Console.WriteLine("Do you want to filter by activity status? (y/n): ");
        bool? isActive = null;

        string filterInput = Console.ReadLine();
        if (filterInput.ToLower() == "y")
        {
            Console.WriteLine("Enter status (active/inactive): ");
            string status = Console.ReadLine();

            if (status.ToLower() == "active")
            {
                isActive = true;
            }
            else if (status.ToLower() == "inactive")
            {
                isActive = false;
            }
            else
            {
                Console.WriteLine("Invalid status. Showing all.");
            }
        }

        Console.WriteLine("Sort by: (1) Full Name (2) Total Handled Calls (3) Total Canceled Calls (4) Total Expired Calls");
        int sortOption = int.Parse(Console.ReadLine());

        VolunteerSortBy? sortBy = sortOption switch
        {
            1 => VolunteerSortBy.FullName,
            2 => VolunteerSortBy.TotalHandledCalls,
            3 => VolunteerSortBy.TotalCanceledCalls,
            4 => VolunteerSortBy.TotalExpiredCalls,
            _ => null
        };

        try
        {
            var volunteers = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);

            Console.WriteLine("List of Volunteers:");
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.FullName}, Total Handled Calls: {volunteer.TotalCallsHandled}, Total Canceled Calls: {volunteer.TotalCallsCanceled}, Total Expired Calls: {volunteer.TotalCallsExpired}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    //private static AdminClass admin = new ();  // Replace with your actual class name
    private static void TestAdvanceSystemClock()
    {
        Console.WriteLine("\nTesting Advance System Clock...");

        // Test advancing the system clock by different units
        foreach (TimeUnit timeUnit in Enum.GetValues(typeof(TimeUnit)))
        {
            if (timeUnit != TimeUnit.UNDEFINED) // Skip 'UNDEFINED'
            {
                Console.WriteLine($"Advancing by: {timeUnit}");
                s_bl.Admin.AdvanceSystemClock(timeUnit);
                Console.WriteLine($"New System Clock: {s_bl.Admin.GetSystemClock()}\n");
            }
        }
    }

    private static void TestRiskTimeRange()
    {
        Console.WriteLine("\nTesting Risk Time Range...");

        // Get current risk time range
        TimeSpan currentRiskTimeRange = s_bl.Admin.GetRiskTimeRange();
        Console.WriteLine($"Current Risk Time Range: {currentRiskTimeRange}");

        // Set a new risk time range
        TimeSpan newRiskTimeRange = new TimeSpan(2, 30, 0); // 2 hours 30 minutes
        s_bl.Admin.SetRiskTimeRange(newRiskTimeRange);
        Console.WriteLine($"New Risk Time Range set to: {newRiskTimeRange}");
    }
/*
    private static void TestResetDatabase()
    {
        Console.WriteLine("\nTesting Reset Database...");
        admin.ResetDatabase();
        Console.WriteLine("Database has been reset.");
    }

    private static void TestInitializeDatabase()
    {
        Console.WriteLine("\nTesting Initialize Database...");
        admin.InitializeDatabase();
        Console.WriteLine("Database has been initialized.");
    }*/
}

