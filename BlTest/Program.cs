using BlApi;
using BO;
using System;
using System.Collections.Generic;

class Program
{
    private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Emergency Response System Test Program");

        while (true)
        {
            try
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Volunteer Management");
                Console.WriteLine("2. Call Management");
                Console.WriteLine("3. Admin Functions");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        VolunteerMenu();
                        break;
                    case 2:
                        CallMenu();
                        break;
                    case 3:
                        //AdminMenu();
                        break;
                    case 0:
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }

    private static void VolunteerMenu()
    {
        while (true)
        {
            Console.WriteLine("\nVolunteer Management Menu:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Get Volunteers List");
            Console.WriteLine("3. Get Volunteer Details");
            Console.WriteLine("4. Update Volunteer Details");
            Console.WriteLine("5. Delete Volunteer");
            Console.WriteLine("6. Add Volunteer");
            Console.WriteLine("0. Return to Main Menu");
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
                        Console.Write("Enter username: ");
                        string username = Console.ReadLine() ?? "";
                        Console.Write("Enter password: ");
                        string password = Console.ReadLine() ?? "";
                        string result = s_bl.Volunteer.Login(username, password);
                        Console.WriteLine($"Login result: {result}");
                        break;

                    case 2:
                        Console.WriteLine("Filter by active status? (y/n/any)");
                        string activeInput = Console.ReadLine()?.ToLower() ?? "";
                        bool? isActive = activeInput == "y" ? true :
                                       activeInput == "n" ? false : null;

                        Console.WriteLine("Sort by field? (1: FullName, 2: TotalHandledCalls, 3: TotalCanceledCalls, 4: TotalExpiredCalls, 0: None)");
                        if (int.TryParse(Console.ReadLine(), out int sortChoice))
                        {
                            VolunteerSortBy? sortBy = sortChoice switch
                            {
                                1 => VolunteerSortBy.FullName,
                                2 => VolunteerSortBy.TotalHandledCalls,
                                3 => VolunteerSortBy.TotalCanceledCalls,
                                4 => VolunteerSortBy.TotalExpiredCalls,
                                _ => null
                            };
                            var volunteers = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                            foreach (var volunteer in volunteers)
                                Console.WriteLine(volunteer);
                        }
                        break;

                    case 3:
                        Console.Write("Enter volunteer ID number: ");
                        string idNumber = Console.ReadLine() ?? "";
                        var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(idNumber);
                        Console.WriteLine(volunteerDetails);
                        break;

                    case 4:
                        Console.Write("Enter volunteer ID to update: ");
                        string updateId = Console.ReadLine() ?? "";
                        var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updateId);

                        Console.Write($"Enter new FullName (current: {existingVolunteer.FullName}, press Enter to keep current): ");
                        string updatedName = Console.ReadLine() ?? "";
                            existingVolunteer.FullName = updatedName;

                        Console.Write($"Enter new PhoneNumber (current: {existingVolunteer.PhoneNumber}, press Enter to keep current): ");
                        string updatedPhoneNumber = Console.ReadLine() ?? "";
                        existingVolunteer.PhoneNumber = updatedPhoneNumber;

                        Console.Write($"Enter new Email (current: {existingVolunteer.Email}, press Enter to keep current): ");
                        string updatedEmail = Console.ReadLine() ?? "";
                        existingVolunteer.Email = updatedEmail;

                        Console.Write($"Enter new Email (current: {existingVolunteer.CurrentAddress}, press Enter to keep current): ");
                        string updatedAddress = Console.ReadLine() ?? "";
                        existingVolunteer.CurrentAddress = updatedAddress;

                        Console.Write($"Enter new latitude (current: {existingVolunteer.Latitude}, press Enter to keep current): ");
                        string latInput = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(latInput) && double.TryParse(latInput, out double newLat))
                            existingVolunteer.Latitude = newLat;

                        Console.Write($"Enter new longitude (current: {existingVolunteer.Longitude}, press Enter to keep current): ");
                        string lonInput = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(lonInput) && double.TryParse(lonInput, out double newLon))
                            existingVolunteer.Longitude = newLon;

                        s_bl.Volunteer.UpdateVolunteerDetails(updateId, existingVolunteer);
                        Console.WriteLine("Volunteer updated successfully");
                        break;

                    case 5:
                        Console.Write("Enter volunteer ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                            s_bl.Volunteer.DeleteVolunteer(deleteId);
                        Console.WriteLine("Volunteer deleted successfully");
                        break;

                    case 6:
                        var newVolunteer = new Volunteer
                        {
                            Id=GetId(),
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
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }

    private static void CallMenu()
    {
        while (true)
        {
            Console.WriteLine("\nCall Management Menu:");
            Console.WriteLine("1. Get Call Counts by Status");
            Console.WriteLine("2. Get Call List");
            Console.WriteLine("3. Get Call Details");
            Console.WriteLine("4. Update Call Details");
            Console.WriteLine("5. Delete Call");
            Console.WriteLine("6. Add Call");
            Console.WriteLine("7. Get Closed Calls by Volunteer");
            Console.WriteLine("8. Get Open Calls for Volunteer");
            Console.WriteLine("9. Assign Call");
            Console.WriteLine("0. Return to Main Menu");
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
                        var counts = s_bl.Call.GetCallCountsByStatus();
                        foreach (var count in counts)
                            Console.WriteLine(count);
                        break;

                    case 2:
                        Console.WriteLine("Filter by field? (1: Id, 2: FullName, 3: TotalHandledCalls, 4: TotalCanceledCalls, 5: TotalExpiredCalls, 6: CurrentCallId, 7: CurrentCallType, 0: None)");
                        CallField? filterField = null;
                        object? filterValue = null;
                        if (int.TryParse(Console.ReadLine(), out int filterChoice) && filterChoice > 0 && filterChoice <= 7)
                        {
                            filterField = (CallField)(filterChoice - 1);
                            Console.Write("Enter filter value: ");
                            filterValue = Console.ReadLine();
                        }

                        Console.WriteLine("Sort by same fields? (1-7 as above, 0: None)");
                        CallField? sortField = null;
                        if (int.TryParse(Console.ReadLine(), out int sortChoice) && sortChoice > 0 && sortChoice <= 7)
                            sortField = (CallField)(sortChoice - 1);

                        var calls = s_bl.Call.GetCallList(filterField, filterValue, sortField);
                        foreach (var call in calls)
                            Console.WriteLine(call);
                        break;

                    case 3:
                        Console.Write("Enter call ID: ");
                        if (int.TryParse(Console.ReadLine(), out int callId))
                        {
                            var callDetails = s_bl.Call.GetCallDetails(callId);
                            Console.WriteLine(callDetails);
                        }
                        break;

                    case 4:
                        Console.Write("Enter call ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateCallId))
                        {
                            var existingCall = s_bl.Call.GetCallDetails(updateCallId);

                            Console.WriteLine("Enter new call type (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent, 0: Keep current):");
                            if (int.TryParse(Console.ReadLine(), out int typeChoice) && typeChoice > 0 && typeChoice <= 4)
                                existingCall.Type = (CallTypeEnum)typeChoice;

                            existingCall.Description = GetInput("Enter new description (or press Enter to keep current): ");
                            existingCall.Address = GetInput("Enter new address (or press Enter to keep current): ");

                            if (GetCoordinate("latitude", out double newLat))
                                existingCall.Latitude = newLat;

                            if (GetCoordinate("longitude", out double newLon))
                                existingCall.Longitude = newLon;

                            Console.WriteLine("Enter new status (1: Treated, 2: SelfCanceled, 3: AdminCanceled, 4: Expired, 0: Keep current):");
                            if (int.TryParse(Console.ReadLine(), out int statusChoice) && statusChoice > 0 && statusChoice <= 4)
                                existingCall.Status = (CallStatus)statusChoice;

                            s_bl.Call.UpdateCallDetails(existingCall);
                            Console.WriteLine("Call updated successfully");
                        }
                        break;

                    case 5:
                        Console.Write("Enter call ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteCallId))
                        {
                            s_bl.Call.DeleteCall(deleteCallId);
                            Console.WriteLine("Call deleted successfully");
                        }
                        break;

                    case 6:
                        Console.WriteLine("Enter call type (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent):");
                        if (!int.TryParse(Console.ReadLine(), out int newTypeChoice) || newTypeChoice < 1 || newTypeChoice > 4)
                        {
                            Console.WriteLine("Invalid call type");
                            break;
                        }

                        var newCall = new Call
                        {
                            Type = (CallTypeEnum)newTypeChoice,
                            Description = GetInput("Enter description: "),
                            Address = GetInput("Enter address: "),
                            OpenTime = DateTime.Now,
                            Status = CallStatus.Treated
                        };

                        if (!GetCoordinate("latitude", out double newCallLat) ||
                            !GetCoordinate("longitude", out double newCallLon))
                        {
                            Console.WriteLine("Invalid coordinates");
                            break;
                        }

                        newCall.Latitude = newCallLat;
                        newCall.Longitude = newCallLon;

                        s_bl.Call.AddCall(newCall);
                        Console.WriteLine("Call added successfully");
                        break;

                    case 7:
                        Console.Write("Enter volunteer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int volId))
                        {
                            Console.WriteLine("Filter by call type? (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent, 0: None):");
                            CallTypeEnum? filterType = null;
                            if (int.TryParse(Console.ReadLine(), out int typeFilter) && typeFilter > 0 && typeFilter <= 4)
                                filterType = (CallTypeEnum)typeFilter;

                            var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volId, filterType);
                            foreach (var closedCall in closedCalls)
                                Console.WriteLine(closedCall);
                        }
                        break;

                    case 8:
                        Console.Write("Enter volunteer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int volunteerId))
                        {
                            Console.WriteLine("Filter by call type? (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent, 0: None):");
                            CallTypeEnum? openFilterType = null;
                            if (int.TryParse(Console.ReadLine(), out int openTypeFilter) && openTypeFilter > 0 && openTypeFilter <= 4)
                                openFilterType = (CallTypeEnum)openTypeFilter;

                            var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, openFilterType);
                            foreach (var openCall in openCalls)
                                Console.WriteLine(openCall);
                        }
                        break;

                    case 9:
                        Console.Write("Enter call ID to assign: ");
                        if (!int.TryParse(Console.ReadLine(), out int assignCallId))
                        {
                            Console.WriteLine("Invalid call ID");
                            break;
                        }

                        Console.Write("Enter volunteer ID to assign to: ");
                        if (!int.TryParse(Console.ReadLine(), out int assignVolId))
                        {
                            Console.WriteLine("Invalid volunteer ID");
                            break;
                        }

                        s_bl.Call.AssignCall(assignCallId, assignVolId);
                        Console.WriteLine("Call assigned successfully");
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }
    // Define this method somewhere in your Program class or another accessible class
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


}
