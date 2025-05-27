using BlApi;
using BO;
using System.Net.Mail;
using System.Net;
using System.Xml;


class Program
{
    private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    /// <summary>
    /// Main display inviting
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Emergency Response System");
        MainLoginMenu();
    }

    /// <summary>
    /// Login display
    /// </summary>
    private static void MainLoginMenu()
    {
        while (true)
        {
            try
            {
                //ManagerMainMenu();
                Console.WriteLine("\nLogin Menu:");
                Console.Write("Enter username: ");
                string username = Console.ReadLine() ?? "";
                Console.Write("Enter password: ");
                string password = Console.ReadLine() ?? "";

                string role = s_bl.Volunteer.Login(username, password);

                switch (role.ToLower())
                {
                    case "manager":
                        ManagerMainMenu(username);
                        break;
                    case "volunteer":
                        VolunteerMainMenu(username);
                        break;
                    default:
                        Console.WriteLine("Invalid login credentials. Please try again.");
                        break;
                }
            }
            catch (BlProgramException ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Manager disolay
    /// </summary>
    /// <param name="Manager">a parameter that holds the Manager's details</param>
    private static void ManagerMainMenu(string Manager)
    //private static void ManagerMainMenu()

    {
        while (true)
        {
            Console.WriteLine("\nManager Main Menu:");
            Console.WriteLine("1. Add Call Screen");
            Console.WriteLine("2. Add Volunteer Screen");
            Console.WriteLine("3. Delete Call");
            Console.WriteLine("4. Delete a volunteer");
            Console.WriteLine("5. get list of volunteers");
            Console.WriteLine("6. Logout");
            Console.WriteLine("7. Volunteer Screen");
            Console.WriteLine("\nEnter your choice: ");

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
                        AddCallScreen();
                        break;
                    case 2:
                        AddVolunteerScreen();
                        break;
                    case 3:
                        DeleteCallScreen();
                        break;
                    case 4:
                        DeleteVolunteerOption();
                        break;
                    case 5:
                        DisplayVolunteersList();
                        break;
                    case 6:
                        return; // Return to login menu
                    case 7:
                        VolunteerMainMenu(Manager);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BlProgramException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// The regular volunteer's display
    /// </summary>
    private static void VolunteerMainMenu(string username)
    {
        BO.Volunteer? volunteer = s_bl.Volunteer.GetVolunteerDetails(username);

        while (true)
        {
            Console.WriteLine("\nVolunteer Main Menu:");
            Console.WriteLine("2. Get Closed calls by valunteer");
            Console.WriteLine("3. Get open calls by valunteer");
            Console.WriteLine("4. Get Volunteer Details");
            Console.WriteLine("5. Update Volunteer Details");
            Console.WriteLine("6. Test Advance System Clock");
            Console.WriteLine("7. Test RiskTime Range");
            Console.WriteLine("8. Test Reset Database");
            Console.WriteLine("9. Test Get Call Details");
            Console.WriteLine("10. Test Update Call Details");
            Console.WriteLine("11. Test Close Call");
            Console.WriteLine("12. Test Assign Call");
            Console.WriteLine("13. Test Cancel Call");
            Console.WriteLine("14. Test Initialize Database");
            Console.WriteLine("15. Logout");
            Console.WriteLine("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 2:
                        VolunteerClosedCallsScreen(volunteer.Id);
                        break;
                    case 3:
                        VolunteerOpenCallsScreen(volunteer.Id);
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
                    case 6:
                        TestAdvanceSystemClock();
                        break;
                    case 7:
                        TestRiskTimeRange();
                        break;
                    case 8:
                        TestResetDatabase();
                        break;
                    case 9:
                        TestGetCallDetails();
                        break;
                    case 10:
                        TestUpdateCallDetails();
                        break;
                    case 11:
                        TestCloseCall();
                        break;
                    case 12:
                        TestAssignCall();
                        break;
                    case 13:
                        TestCancelCall();
                        break;
                    case 14:
                        TestInitializeDatabase();
                        break;
                    case 15:
                        return; // Return to login menu
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BlProgramException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Add Call display
    /// </summary>
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

            string input = Console.ReadLine();
            if (!(Enum.TryParse(input, true, out CallTypeEnum callType)))
            {
                Console.WriteLine("The type is not valid.");
            }

            Console.Write("Enter the call status (e.g., Open, Closed): ");
            string statusInput = Console.ReadLine();

            if (!Enum.TryParse(statusInput, true, out CallStatus status))
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
                OpenTime = DateTime.Now, // Current time as OpenTime
                MaxEndTime = maxEndTime,
                Status = status,

            };
            s_bl.Call.AddCall(newCall);

            Console.WriteLine("New call added successfully!");

            s_bl.Call.SendEmailToVolunteers(newCall);
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An error occurred while adding the call: {ex.Message}");
        }
    }

    /// <summary>
    /// Add volunteer display
    /// </summary>
    private static void AddVolunteerScreen()
    {
        try
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

            Console.Write("Enter maximum distance (in km, press Enter for default): ");
            if (double.TryParse(Console.ReadLine(), out double maxDist))
                newVolunteer.MaxDistance = maxDist;

            Console.WriteLine("Select distance type (1: AirDistance, 2: WalkingDistance, 3: DrivingDistance): ");
            if (int.TryParse(Console.ReadLine(), out int distType) && distType >= 1 && distType <= 3)
                newVolunteer.TypeOfDistance = (DistanceType)(distType - 1);

            s_bl.Volunteer.AddVolunteer(newVolunteer);
            Console.WriteLine("Volunteer added successfully");
        }
        catch (BlFormatException ex)
        {
            Console.WriteLine($"Invalid input format: {ex.Message}");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"Argument error: {ex.Message}");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// shows Volunteer's Closed Calls Screen
    /// </summary>
    /// <param name="volunteerId">the specific volunteerId</param>
    /// <returns></returns>
    public static List<BO.ClosedCallInList> VolunteerClosedCallsScreen(int volunteerId)
    {
        try
        {
            var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId);

            Console.WriteLine($"Found {closedCalls.Count()} closed calls for volunteer {volunteerId}");

            foreach (var call in closedCalls)
            {
                Console.WriteLine($"Call ID: {call.Id}, Type: {call.Type}, " +
                                 $"Address: {call.Address}, Open Time: {call.OpenTime}, " +
                                 $"End Time: {call.EndTreatmentTime}");
            }

            return closedCalls.ToList();
        }
        catch (BlGeneralDatabaseException ex)
        {
            Console.WriteLine($"Error fetching closed calls for volunteer {volunteerId}: {ex.Message}");
            return new List<BO.ClosedCallInList>();
        }
    }

    /// <summary>
    /// shows Volunteer's Open Calls Screen
    /// </summary>
    /// <param name="volunteerId">the specific volunteerId</param>
    /// <returns></returns>
    public static List<BO.OpenCallInList> VolunteerOpenCallsScreen(int volunteerId)
    {
        Console.WriteLine("Volunteer Open Calls Screen");
        try
        {
            var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId);

            Console.WriteLine($"Found {openCalls.Count()} Open calls for volunteer {volunteerId}");

            foreach (var call in openCalls)
            {
                Console.WriteLine($"Call ID: {call.Id}, Type: {call.CallType}, " +
                                 $"Address: {call.FullAddress}, Open Time: {call.OpenTime}");
            }
            return openCalls.ToList();
        }
        catch (BlGeneralDatabaseException ex)
        {
            Console.WriteLine($"Error fetching Open calls for volunteer {volunteerId}: {ex.Message}");

            return new List<BO.OpenCallInList>();
        }
    }

    /// <summary>
    /// returns specific fields
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    private static string GetInput(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    /// <summary>
    /// returns Id
    /// </summary>
    /// <returns></returns>
    private static int GetId()
    {
        try
        {
            Console.Write("Enter Id: ");
            string input = Console.ReadLine() ?? "";

            int id = int.Parse(input); 
            return id;
        }
        catch (BlFormatException)
        {
            Console.WriteLine("Invalid input. Please enter a valid integer.");
            return -1; 
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }


    /// <summary>
    /// function that updates volunteers details
    /// </summary>
    /// <param name="volunteer"></param>
    private static void UpdateVolunteerDetails(Volunteer volunteer)
    {
        try
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

            Console.Write($"Enter new isActive (current: {volunteer.IsActive} enter true or false, press Enter to keep current): ");
            string isActiveInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(isActiveInput) && bool.TryParse(isActiveInput, out bool newAct))
                volunteer.IsActive = newAct;

            Console.Write($"Enter new MaxDistance (current: {volunteer.MaxDistance}, press Enter to keep current): ");
            string maxDistanceInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(maxDistanceInput) && double.TryParse(maxDistanceInput, out double newMaxDis))
                volunteer.MaxDistance = newMaxDis;

            Console.Write($"Enter new Role (current: {volunteer.Role}, press Enter to keep current): ");
            string roleInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(roleInput) && PositionEnum.TryParse(roleInput, out PositionEnum newRole))
                volunteer.Role = newRole;

            Console.Write($"Enter new TypeOfDistance (current: {volunteer.TypeOfDistance}, press Enter to keep current): ");
            string typeDisInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(typeDisInput) && DistanceType.TryParse(typeDisInput, out DistanceType newTypeDis))
                volunteer.TypeOfDistance = newTypeDis;
        }
        catch (BlFormatException ex)
        {
            Console.WriteLine($"Invalid input format: {ex.Message}");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"Invalid argument: {ex.Message}");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// function that deletes Call
    /// </summary>
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

            s_bl.Call.DeleteCall(callId);

            Console.WriteLine("Call deleted successfully.");
        }
        catch (BlGeneralDatabaseException ex)
        {
            Console.WriteLine($"Error deleting call: {ex.Message}");
        }
    }

    /// <summary>
    /// function that deletes Volunteer
    /// </summary>
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
            catch (DO.DalDoesNotExistException ex)
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

    /// <summary>
    /// shows all volunteers details
    /// </summary>
    static void DisplayVolunteersList()
    {
        try
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

            var volunteers = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);

            Console.WriteLine("List of Volunteers:");
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.FullName}, Total Canceled Calls: {volunteer.TotalCanceledCalls}, Total Expired Calls: {volunteer.TotalExpiredCalls}");
            }
        }
        catch (BlFormatException ex)
        {
            Console.WriteLine($"Invalid input format. Please enter a valid number: {ex.Message}");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// Test Advance System Clock
    /// </summary>
    private static void TestAdvanceSystemClock()
    {
        Console.WriteLine("\nTesting Advance System Clock...");

        foreach (TimeUnit timeUnit in Enum.GetValues(typeof(TimeUnit)))
        {
            if (timeUnit != TimeUnit.UNDEFINED)
            {
                Console.WriteLine($"Advancing by: {timeUnit}");
                s_bl.Admin.AdvanceSystemClock(timeUnit);
                Console.WriteLine($"New System Clock: {s_bl.Admin.GetSystemClock()}\n");
            }
        }
    }

    /// <summary>
    /// Test Risk Time Range
    /// </summary>
    private static void TestRiskTimeRange()
    {
        Console.WriteLine("\nTesting Risk Time Range...");

        TimeSpan currentRiskTimeRange = s_bl.Admin.GetRiskTimeRange();
        Console.WriteLine($"Current Risk Time Range: {currentRiskTimeRange}");

        TimeSpan newRiskTimeRange = new TimeSpan(2, 30, 0);
        s_bl.Admin.SetRiskTimeRange(newRiskTimeRange);
        Console.WriteLine($"New Risk Time Range set to: {newRiskTimeRange}");
    }

    /// <summary>
    /// Test Reset Database
    /// </summary>
    private static void TestResetDatabase()
    {
        Console.WriteLine("\nTesting Reset Database...");
        s_bl.Admin.ResetDatabase();
        Console.WriteLine("Database has been reset.");
    }

    /// <summary>
    /// Test Initialize Database
    /// </summary>
    private static void TestInitializeDatabase()
    {
        Console.WriteLine("\nTesting Initialize Database...");
        s_bl.Admin.InitializeDatabase();
        Console.WriteLine("Database has been initialized.");
    }

    /// <summary>
    /// Test Get Call Details
    /// </summary>
    private static void TestGetCallDetails()
    {
        Console.WriteLine("Enter Call ID: ");
        int callId = int.Parse(Console.ReadLine() ?? "0");
        var callDetails = s_bl.Call.GetCallDetails(callId);
        Console.WriteLine(callDetails);
    }

    /// <summary>
    /// Test Update Call Details
    /// </summary>
    private static void TestUpdateCallDetails()
    {
        try
        {
            Console.WriteLine("Enter Call ID to update: ");
            int callId = int.Parse(Console.ReadLine() ?? "0");
            var call = s_bl.Call.GetCallDetails(callId);

            Console.Write($"Enter new Type (current: {call.Type}, press Enter to keep current): ");
            string typeInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(typeInput) && Enum.TryParse(typeInput, out CallTypeEnum newType))
                call.Type = newType;

            Console.Write($"Enter new Description (current: {(string.IsNullOrEmpty(call.Description) ? "N/A" : call.Description)}, press Enter to keep current): ");
            string updatedDescription = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(updatedDescription))
                call.Description = updatedDescription;

            Console.Write($"Enter new Address (current: {call.Address}, press Enter to keep current): ");
            string updatedAddress = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(updatedAddress))
                call.Address = updatedAddress;

            Console.Write($"Enter new Latitude (current: {call.Latitude}, press Enter to keep current): ");
            string latInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(latInput) && double.TryParse(latInput, out double newLat))
                call.Latitude = newLat;

            Console.Write($"Enter new Longitude (current: {call.Longitude}, press Enter to keep current): ");
            string lonInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(lonInput) && double.TryParse(lonInput, out double newLon))
                call.Longitude = newLon;

            Console.Write($"Enter new Max End Time (current: {(call.MaxEndTime.HasValue ? call.MaxEndTime.Value.ToString() : "N/A")}, press Enter to keep current): ");
            string endTimeInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(endTimeInput) && DateTime.TryParse(endTimeInput, out DateTime newEndTime))
                call.MaxEndTime = newEndTime;

            Console.Write($"Enter new Status (current: {call.Status}, press Enter to keep current): ");
            string statusInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(statusInput) && Enum.TryParse(statusInput, out CallStatus newStatus))
                call.Status = newStatus;

            s_bl.Call.UpdateCallDetails(call);
            Console.WriteLine("Call updated successfully.");
        }
        catch (BlFormatException ex)
        {
            Console.WriteLine($"Input format is incorrect. Please check your input: {ex.Message}");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"A required argument is null: {ex.Message}");
        }
        catch (BlDoesNotExistException ex)
        {
            Console.WriteLine($"Call not found: {ex.Message}");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// Test Close Call
    /// </summary>
    private static void TestCloseCall()
    {
        try
        {
            Console.WriteLine("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.WriteLine("Enter Assignment ID: ");
                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                {
                    s_bl.Call.CloseCall(volunteerId, assignmentId);
                    Console.WriteLine("Call closed successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid Assignment ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Volunteer ID.");
            }
        }
        catch (BlDoesNotExistException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Call or volunteer might not exist.");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"Argument Error: {ex.Message}. Please check your inputs.");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// Test Cance lCall
    /// </summary>
    private static void TestCancelCall()
    {
        try
        {
            Console.WriteLine("Enter Requestor ID: ");
            if (int.TryParse(Console.ReadLine(), out int requestorId))
            {
                Console.WriteLine("Enter Assignment ID: ");
                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                {
                    s_bl.Call.CancelCall(requestorId, assignmentId);
                    Console.WriteLine("Call cancelled successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid Assignment ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Requestor ID.");
            }
        }
        catch (BlDoesNotExistException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. The call, requestor, or assignment might not exist.");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"Argument Error: {ex.Message}. Please check your inputs.");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    /// <summary>
    /// Test Assign Call
    /// </summary>
    private static void TestAssignCall()
    {
        try
        {
            Console.WriteLine("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.WriteLine("Enter Call ID to assign: ");
                if (int.TryParse(Console.ReadLine(), out int callId))
                {
                    s_bl.Call.AssignCall(volunteerId, callId);
                    Console.WriteLine("Call assigned successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid Call ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Volunteer ID.");
            }
        }
        catch (BlDoesNotExistException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. The call or volunteer might not exist.");
        }
        catch (BlArgumentException ex)
        {
            Console.WriteLine($"Argument Error: {ex.Message}. Please check your inputs.");
        }
        catch (BlProgramException ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

}


