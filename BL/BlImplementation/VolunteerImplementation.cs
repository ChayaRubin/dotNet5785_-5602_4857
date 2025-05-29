using BlApi;
using BO;
using DO;
using Helpers;
using System.Net.Mail;
using System.Net;
using System.Xml;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Authenticates a volunteer by checking their username and password.
    /// </summary>
    /// <param name="username">The volunteer's username.</param>
    /// <param name="password">The volunteer's password.</param>
    /// <returns>The position of the volunteer if authentication is successful.</returns>
    /// <exception cref="BlUnauthorizedAccessException">Thrown if the username or password is incorrect.</exception>

    public string Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(v => v.Name == username);

            if (volunteer == null)
                throw new DalUnauthorizedAccessException("Invalid username or password");

            // Debug logging
            Console.WriteLine($"Input username: '{username}'");
            Console.WriteLine($"Input password: '{password}'");
            Console.WriteLine($"Stored password: '{volunteer.Password}'");

            // Compare passwords
            if (!VolunteerManager.VerifyPassword(password, volunteer.Password))
                throw new DalUnauthorizedAccessException("Invalid username or password");

            return volunteer.Position.ToString();
        }
        catch (DalUnauthorizedAccessException ex)
        {
            throw new BlUnauthorizedAccessException($"Login failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred during login: {ex.Message}");
        }
    }
    /// <summary>
    /// Retrieves a list of volunteers, optionally filtered by their activity status and sorted by the specified criteria.
    /// </summary>
    /// <param name="isActive">Optional parameter to filter volunteers by activity status.</param>
    /// <param name="sortBy">Optional parameter to specify how to sort the volunteers.</param>
    /// <returns>A list of volunteers that match the criteria.</returns>
    //public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerSortBy? sortBy)
    //{
    //    try
    //    {
    //        // Retrieve volunteers with activity filtering
    //        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
    //            !isActive.HasValue || v.Active == isActive.Value);

    //        // Convert DO volunteers to BO volunteers

    //        var BoVolunteers = volunteers.Select(VolunteerManager.ConvertToBO);


    //        var volunteerList = VolunteerManager.GetVolunteerList(BoVolunteers);

    //        // Loop through each volunteer to count the calls they have handled, canceled, and expired
    //        foreach (var volunteer in volunteerList)
    //        {
    //            // Retrieve all assignments for the volunteer
    //            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.Id);

    //            // Calculate the total number of calls handled, canceled, and expired
    //            volunteer.TotalHandledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Treated);

    //            volunteer.TotalCanceledCalls = assignments.Count(a =>
    //                a.CallResolutionStatus == CallResolutionStatus.Canceled
    //                || a.CallResolutionStatus == CallResolutionStatus.SelfCanceled);

    //            volunteer.TotalExpiredCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Expired);

    //        }

    //        // Sort the list based on selected criteria
    //        volunteerList = sortBy.HasValue ? sortBy.Value switch
    //        {
    //            VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
    //            VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls).ToList(),
    //            VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCanceledCalls).ToList(),
    //            VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls).ToList(),
    //            _ => volunteerList.OrderBy(v => v.Id).ToList()
    //        } : volunteerList.OrderBy(v => v.Id).ToList();

    //        // Return the sorted list of volunteers
    //        return volunteerList;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BlGeneralDatabaseException($"An unexpected error occurred while retrieving the volunteer list: {ex.Message}");
    //    }
    //}

    /* public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerSortBy? sortBy, CallTypeEnum? callTypeFilter = null)
     {
         try
         {
             // Step 1: Filter volunteers by isActive if specified
             IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                 !isActive.HasValue || v.Active == isActive.Value);

             // Convert DO.Volunteer to BO.VolunteerInList (assuming you have such a method)
             var boVolunteers = volunteers.Select(VolunteerManager.ConvertToBO);

             var volunteerList = VolunteerManager.GetVolunteerList(boVolunteers);

             foreach (var volunteer in volunteerList)
             {
                 // Get all assignments for this volunteer
                 var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.Id);

                 // Calculate handled, canceled, expired calls counts
                 volunteer.TotalHandledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Treated);
                 volunteer.TotalCanceledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Canceled
                                                                    || a.CallResolutionStatus == CallResolutionStatus.SelfCanceled);
                 volunteer.TotalExpiredCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Expired);

                 // Optional: find current active assignment (e.g. InProgress)
                 var currentAssignment = assignments.FirstOrDefault(a => a.CallResolutionStatus == CallResolutionStatus.InProgress);
                 if (currentAssignment != null)
                 {
                     volunteer.CurrentCallId = currentAssignment.CallId;

                     // Get the call to check its type
                     var call = _dal.Call.Read(c => c.RadioCallId == currentAssignment.CallId);
                     if (call != null)
                     {
                         // Convert DO.CallType to BO.CallTypeEnum safely
                         volunteer.CurrentCallType = (BO.CallTypeEnum)(int)call.CallType;
                     }
                     else
                     {
                         volunteer.CurrentCallType = BO.CallTypeEnum.None;
                     }
                 }
                 else
                 {
                     volunteer.CurrentCallId = null;
                     volunteer.CurrentCallType = BO.CallTypeEnum.None;
                 }
             }

             // Step 2: Filter by CallTypeEnum if specified
             if (callTypeFilter.HasValue)
             {
                 volunteerList = volunteerList
                     .Where(v => v.CurrentCallType == callTypeFilter.Value)
                     .ToList();
             }

             // Step 3: Sort the list based on sortBy parameter or default by Id
             volunteerList = sortBy.HasValue ? sortBy.Value switch
             {
                 VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
                 VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls).ToList(),
                 VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCanceledCalls).ToList(),
                 VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls).ToList(),
                 _ => volunteerList.OrderBy(v => v.Id).ToList()
             } : volunteerList.OrderBy(v => v.Id).ToList();

             return volunteerList;
         }
         catch (Exception ex)
         {
             throw new BlGeneralDatabaseException($"Unexpected error while retrieving volunteers: {ex.Message}");
         }
     }*/
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerSortBy? sortBy, CallTypeEnum? callTypeFilter = null)
    {
        try
        {
            // Step 1: Filter volunteers by isActive if specified
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                !isActive.HasValue || v.Active == isActive.Value);

            // Convert DO.Volunteer to BO.VolunteerInList (assuming you have such a method)
            var boVolunteers = volunteers.Select(VolunteerManager.ConvertToBO);

            var volunteerList = VolunteerManager.GetVolunteerList(boVolunteers);

            foreach (var volunteer in volunteerList)
            {
                // Get all assignments for this volunteer
                var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.Id);

                // Calculate handled, canceled, expired calls counts
                volunteer.TotalHandledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Treated);
                volunteer.TotalCanceledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Canceled
                                                                   || a.CallResolutionStatus == CallResolutionStatus.SelfCanceled);
                volunteer.TotalExpiredCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Expired);

                // Optional: find current active assignment (e.g. InProgress)
                var currentAssignment = assignments.FirstOrDefault(a => a.CallResolutionStatus == CallResolutionStatus.InProgress);
                if (currentAssignment != null)
                {
                    volunteer.CurrentCallId = currentAssignment.CallId;

                    // Get the call to check its type
                    var call = _dal.Call.Read(c => c.RadioCallId == currentAssignment.CallId);
                    if (call != null)
                    {
                        volunteer.CurrentCallType = (BO.CallTypeEnum)(int)call.CallType;
                    }
                    else
                    {
                        volunteer.CurrentCallType = BO.CallTypeEnum.None;
                    }
                }
                else
                {
                    volunteer.CurrentCallId = null;
                    volunteer.CurrentCallType = BO.CallTypeEnum.None;
                }
            }

            // Step 2: Filter by CallTypeEnum if specified (check all calls for each volunteer)
            if (callTypeFilter.HasValue)
            {
                volunteerList = volunteerList
                    .Where(v =>
                    {
                        // Get all assignments for this volunteer
                        var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);

                        // Check if any of the calls in assignments match the filter type
                        foreach (var assignment in assignments)
                        {
                            var call = _dal.Call.Read(c => c.RadioCallId == assignment.CallId);
                            if (call != null && (BO.CallTypeEnum)(int)call.CallType == callTypeFilter.Value)
                                return true;
                        }
                        return false;
                    })
                    .ToList();
            }

            // Step 3: Sort the list based on sortBy parameter or default by Id
            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
                VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls).ToList(),
                VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCanceledCalls).ToList(),
                VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls).ToList(),
                _ => volunteerList.OrderBy(v => v.Id).ToList()
            } : volunteerList.OrderBy(v => v.Id).ToList();

            return volunteerList;
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"Unexpected error while retrieving volunteers: {ex.Message}");
        }
    }


    /// <summary>
    /// Retrieves detailed information about a volunteer based on their ID number.
    /// </summary>
    /// <param name="idNumber">The ID number of the volunteer.</param>
    /// <returns>The details of the volunteer.</returns>
    /// <exception cref="BlFormatException">Thrown if the ID format is invalid.</exception>
    /// <exception cref="BlDoesNotExistException">Thrown if no volunteer is found with the given ID.</exception>
    public BO.Volunteer GetVolunteerDetails(int idNumber) => GetVolunteer(v => v.Id == idNumber, idNumber.ToString());

    public BO.Volunteer GetVolunteerDetails(string idNumber) =>
        GetVolunteer(v => v.Id == int.Parse(idNumber), idNumber);

    private BO.Volunteer GetVolunteer(Func<DO.Volunteer, bool> predicate, string identifier)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(predicate)
                            ?? throw new BlDoesNotExistException("Volunteer not found");

            var boVolunteer = VolunteerManager.ConvertToBO(volunteer);

            var ongoingAssignment = _dal.Assignment.Read(a =>
                a.VolunteerId == volunteer.Id &&
                (CallStatus)a.CallResolutionStatus == CallStatus.Open); // או Open - לפי הלוגיקה שלך

            if (ongoingAssignment != null)
            {
                var relatedCall = _dal.Call.Read(c => c.RadioCallId == ongoingAssignment.CallId);

            double distance = 0;
            if (boVolunteer.Latitude.HasValue && boVolunteer.Longitude.HasValue)
            {
                distance = Tools.CalculateDistance(
                    relatedCall.Latitude,
                    relatedCall.Longitude,
                    boVolunteer.Latitude.Value,
                    boVolunteer.Longitude.Value);
            }

            if (ongoingAssignment != null)
            {
                boVolunteer.CurrentCall = new BO.CallInProgress
                {
                    Id = ongoingAssignment.Id,
                    Address = relatedCall.Address,
                    CallId = ongoingAssignment.CallId,
                    OpeningTime = ongoingAssignment.EntryTime,
                    MaxCompletionTime = ongoingAssignment.FinishCompletionTime,
                    Status = (BO.CallStatus)ongoingAssignment.CallResolutionStatus,
                    DistanceFromVolunteer = distance,
                };
            }
         }      
            return boVolunteer; // זו השורה שתוקנה
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Volunteer with ID={identifier} was not found. {ex.Message}");
        }
        catch (DalFormatException ex)
        {
            throw new BlFormatException($"Invalid format: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred while retrieving the volunteer details: {ex.Message}");
        }
    }



    /// <summary>
    /// Updates the details of an existing volunteer based on their ID number.
    /// </summary>
    /// <param name="idNumber">The ID number of the volunteer to be updated.</param>
    /// <param name="volunteerBO">The updated volunteer details.</param>
    /// <exception cref="DalFormatException">Thrown if the ID format is invalid.</exception>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="DalUnauthorizedAccessException">Thrown if the user does not have permission to update the volunteer.</exception>
    /// <exception cref="BlUnauthorizedAccessException">Thrown if the volunteer does not have permission to update certain fields.</exception>
    /// <exception cref="BlFormatException">Thrown if the updated volunteer data is in an invalid format.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if an unexpected error occurs during the update operation.</exception>
    public void UpdateVolunteerDetails(string idNumber, BO.Volunteer volunteerBO)
    {
        try
        {
            // Retrieve existing volunteer from DAL
            //DO.Volunteer? existingVolunteer = _dal.Volunteer.Read(v => v.Id == idNumber);
            DO.Volunteer? existingVolunteer;

            System.Diagnostics.Debug.WriteLine($"Received idNumber: '{idNumber}'");

            if (int.TryParse(idNumber, out int id))
            {
                Console.WriteLine($"Parsed ID: {id}, Success: {int.TryParse(idNumber, out id)}");
                existingVolunteer = _dal.Volunteer.Read(v => v.Id == id);
            }
            else
            {
                throw new DalFormatException("Invalid ID format");
            }
            if (existingVolunteer == null)
            {
                throw new BlDoesNotExistException($"The volunteer with ID={idNumber} was not found.");
            }

            // Check for authorization
            if (!VolunteerManager.IsRequesterAuthorized(idNumber, volunteerBO))
            {
                throw new BlUnauthorizedAccessException("Unauthorized to update this volunteer.");
            }

            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);
            volunteerBO.Latitude = latitude;
            volunteerBO.Longitude = longitude;
            // Validate input format
            VolunteerManager.ValidateInputFormat(volunteerBO);

            // Validate logical fields
            VolunteerManager.ValidateLogicalFields(volunteerBO);

            // Handle password update
            if (volunteerBO.Password != existingVolunteer.Password)
            {
                if (idNumber != volunteerBO.Id.ToString() && volunteerBO.Role != BO.PositionEnum.Manager)
                {
                    throw new BlUnauthorizedAccessException("Only the volunteer or a manager can update the password.");
                }
                String HashedPassword = VolunteerManager.HashPassword(volunteerBO.Password);
                volunteerBO.Password = HashedPassword;
            }

            // Ensure field updates are allowed
            if (!VolunteerManager.CanUpdateFields(idNumber, existingVolunteer, volunteerBO))
            {
                throw new BlFormatException("You do not have permission to update certain fields.");
            }

            // Convert BO to DO and update in DAL
            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteerBO);
            _dal.Volunteer.Update(volunteerDO);
            VolunteerManager.Observers.NotifyItemUpdated(volunteerDO.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalFormatException ex)
        {
            throw new BlFormatException($"Invalid data for volunteer update: {ex.Message}");
        }
        catch (DO.DalUnauthorizedAccessException ex)
        {
            throw new BlUnauthorizedAccessException($"Unauthorized access: {ex.Message}");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"The volunteer with ID={volunteerBO.Id} was not found.");
        }
        catch (DO.DalCoordinationExceprion ex)
        {
            throw new BlCoordinationExceprion($"{ex.Message}");
        }
        catch (Exception ex)
        {
            //throw new BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.");
            throw new BlGeneralDatabaseException($"An unexpected error occurred while updating the volunteer: {ex.Message}");
        }
    }


    /// <summary>
    /// Deletes a volunteer based on their ID number.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer cannot be deleted because they are currently handling a call.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if an unexpected error occurs during the deletion process.</exception>
    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(v => v.Id == id) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.CallResolutionStatus == CallResolutionStatus.Open).FirstOrDefault();

            if (currentAssignment != null)
            {
                throw new InvalidOperationException("Cannot delete volunteer while they are handling a call.");
            }
            _dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} was not found in the database.");
        }
        catch (InvalidOperationException ex)
        {
            throw new BO.BlUnauthorizedAccessException("The volunteer cannot be deleted as they are handling a call.");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while trying to delete the volunteer.");
        }
    }

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer to be added.</param>
    /// <exception cref="BlAlreadyExistsException">Thrown if a volunteer with the same ID already exists.</exception>
    /// <exception cref="BlFormatException">Thrown if the input data for the volunteer is invalid.</exception>
    /// <exception cref="BlUnauthorizedAccessException">Thrown if the operation is not authorized.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if an unexpected error occurs while adding the volunteer.</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id);
            if (existingVolunteer != null)
            {
                throw new DalArgumentException("Please put in a different ID number.");
            }

            // Validate input format
            var coordinates =  VolunteerManager.ValidateInputFormat(volunteer);
            //var coordinates = await CallManager.ValidateCall(newCall);

            // Set the coordinates to the newCall object
            volunteer.Latitude = coordinates.latitude;
            volunteer.Longitude = coordinates.longitude;

            // Validate logical fields
            VolunteerManager.ValidateLogicalFields(volunteer);

            //Hash the initial password before storing it
            String HashedPassword = VolunteerManager.HashPassword(volunteer.Password);
            volunteer.Password = HashedPassword;

            // Convert BO to DO
            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteer);

            // Attempt to add volunteer to database
            _dal.Volunteer.Create(volunteerDO);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists. {ex.Message}");
        }
        catch (DO.DalCoordinationExceprion ex)
        {
            throw new BlCoordinationExceprion($"{ex.Message}");
        }
        catch (DO.DalFormatException ex)
        {
            throw new BlFormatException($"Invalid volunteer data: {ex.Message}");
        }
        catch (DO.DalUnauthorizedAccessException ex)
        {
            throw new BlUnauthorizedAccessException($"Unauthorized to add volunteer: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException($"An unexpected error occurred while adding the volunteer: {ex.Message}");
        }
    }

    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
}
