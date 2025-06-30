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

    public string Login(int id, string password)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                var volunteer = _dal.Volunteer.Read(v => v.Id == id);

                if (volunteer == null)
                    throw new DalUnauthorizedAccessException("Invalid username or password");

                Console.WriteLine($"Input username: '{id}'");
                Console.WriteLine($"Input password: '{password}'");
                Console.WriteLine($"Stored password: '{volunteer.Password}'");

                if (!VolunteerManager.VerifyPassword(password, volunteer.Password))
                    throw new DalUnauthorizedAccessException("Invalid username or password");

                return volunteer.Position.ToString();
            }
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
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerSortBy? sortBy, CallTypeEnum? callTypeFilter = null)
    {
        try
        {
            IEnumerable<DO.Volunteer> volunteers;
            lock (AdminManager.BlMutex)
                volunteers = _dal.Volunteer.ReadAll(v => !isActive.HasValue || v.Active == isActive.Value);

            var boVolunteers = volunteers.Select(VolunteerManager.ConvertToBO);
            var volunteerList = VolunteerManager.GetVolunteerList(boVolunteers);

            foreach (var volunteer in volunteerList)
            {
                IEnumerable<DO.Assignment> assignments;
                lock (AdminManager.BlMutex)
                    assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.Id);

                volunteer.TotalHandledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Treated);
                volunteer.TotalCanceledCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Canceled
                                                                   || a.CallResolutionStatus == CallResolutionStatus.SelfCanceled);
                volunteer.TotalExpiredCalls = assignments.Count(a => a.CallResolutionStatus == CallResolutionStatus.Expired);

                var currentAssignment = assignments.FirstOrDefault(a => a.CallResolutionStatus == null);
                if (currentAssignment != null)
                {
                    volunteer.CurrentCallId = currentAssignment.CallId;

                    DO.Call? call;
                    lock (AdminManager.BlMutex)
                        call = _dal.Call.Read(c => c.RadioCallId == currentAssignment.CallId);

                    if (call != null && call.ExpiredTime > DateTime.Now)
                    {
                        volunteer.CurrentCallId = call.RadioCallId;
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

            if (callTypeFilter.HasValue)
            {
                volunteerList = volunteerList
                    .Where(v =>
                    {
                        IEnumerable<DO.Assignment> assignments;
                        lock (AdminManager.BlMutex)
                            assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);

                        foreach (var assignment in assignments)
                        {
                            DO.Call? call;
                            lock (AdminManager.BlMutex)
                                call = _dal.Call.Read(c => c.RadioCallId == assignment.CallId);

                            if (call != null && (BO.CallTypeEnum)(int)call.CallType == callTypeFilter.Value)
                                return true;
                        }
                        return false;
                    })
                    .ToList();
            }

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
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(predicate)
                    ?? throw new BlDoesNotExistException("Volunteer not found");

            var boVolunteer = VolunteerManager.ConvertToBO(volunteer);

            DO.Assignment? ongoingAssignment;
            lock (AdminManager.BlMutex)
                ongoingAssignment = _dal.Assignment.Read(a =>
                    a.VolunteerId == volunteer.Id &&
                    a.CallResolutionStatus.HasValue &&
                    (BO.CallStatus)a.CallResolutionStatus.Value == BO.CallStatus.Open);

            if (ongoingAssignment != null)
            {
                DO.Call relatedCall;
                lock (AdminManager.BlMutex)
                    relatedCall = _dal.Call.Read(c => c.RadioCallId == ongoingAssignment.CallId);

                double distance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, relatedCall.Latitude, relatedCall.Longitude);
                if (boVolunteer.Latitude.HasValue && boVolunteer.Longitude.HasValue)
                {
                    distance = Tools.CalculateDistance(
                        relatedCall.Latitude,
                        relatedCall.Longitude,
                        boVolunteer.Latitude.Value,
                        boVolunteer.Longitude.Value);
                }

                boVolunteer.CurrentCall = new BO.CallInProgress
                {
                    Id = ongoingAssignment.Id,
                    CallType = (BO.CallTypeEnum)relatedCall.CallType,
                    Description = relatedCall.Description,
                    Address = relatedCall.Address,
                    CallId = ongoingAssignment.CallId,
                    OpeningTime = ongoingAssignment.EntryTime,
                    MaxCompletionTime = ongoingAssignment.FinishCompletionTime,
                    AssignmentStartTime = ongoingAssignment.EntryTime,
                    Status = (BO.CallStatus?)ongoingAssignment.CallResolutionStatus,
                    DistanceFromVolunteer = distance,
                };
            }

            return boVolunteer;
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
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            if (!int.TryParse(idNumber, out int id))
                throw new DalFormatException("Invalid ID format");

            DO.Volunteer? existingVolunteer;
            lock (AdminManager.BlMutex) // stage 7
                existingVolunteer = _dal.Volunteer.Read(v => v.Id == id);

            if (existingVolunteer == null)
                throw new BlDoesNotExistException($"The volunteer with ID={idNumber} was not found.");

            if (!VolunteerManager.IsRequesterAuthorized(idNumber, volunteerBO))
                throw new BlUnauthorizedAccessException("Unauthorized to update this volunteer.");

            bool isPasswordUpdateRequested = !string.IsNullOrWhiteSpace(volunteerBO.Password);
            if (!isPasswordUpdateRequested)
            {
                volunteerBO.Password = existingVolunteer.Password;
            }

            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);
            volunteerBO.Latitude = latitude;
            volunteerBO.Longitude = longitude;

            VolunteerManager.ValidateInputFormat(volunteerBO);
            VolunteerManager.ValidateLogicalFields(volunteerBO);

            if (isPasswordUpdateRequested && volunteerBO.Password != existingVolunteer.Password)
            {
                if (idNumber != volunteerBO.Id.ToString() && volunteerBO.Role != BO.PositionEnum.Manager)
                    throw new BlUnauthorizedAccessException("Only the volunteer or a manager can update the password.");

                volunteerBO.Password = VolunteerManager.HashPassword(volunteerBO.Password); // הנחה: זו לא נוגעת ל-DAL
            }

            if (!VolunteerManager.CanUpdateFields(idNumber, existingVolunteer, volunteerBO))
                throw new BlFormatException("You do not have permission to update certain fields.");

            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteerBO);

            lock (AdminManager.BlMutex) // stage 7
            {
                _dal.Volunteer.Update(volunteerDO);
            }

            VolunteerManager.Observers.NotifyItemUpdated(volunteerDO.Id);
            VolunteerManager.Observers.NotifyListUpdated();
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
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex) // stage 7
                volunteer = _dal.Volunteer.Read(v => v.Id == id)
                    ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

            bool anyAssignment;
            lock (AdminManager.BlMutex) // stage 7
                anyAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id).Any();

            if (anyAssignment)
            {
                throw new BO.BlUnauthorizedAccessException("The volunteer cannot be deleted because they are or were assigned to a call.");
            }

            lock (AdminManager.BlMutex) // stage 7
                _dal.Volunteer.Delete(id);

            VolunteerManager.Observers.NotifyListUpdated(); // stage 5
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} was not found in the database.");
        }
        catch (BO.BlUnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception)
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
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            DO.Volunteer? existingVolunteer;
            lock (AdminManager.BlMutex) // stage 7
                existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id);

            if (existingVolunteer != null)
            {
                throw new DalArgumentException("Please put in a different ID number.");
            }

            var coordinates = VolunteerManager.ValidateInputFormat(volunteer);

            volunteer.Latitude = coordinates.latitude;
            volunteer.Longitude = coordinates.longitude;

            VolunteerManager.ValidateLogicalFields(volunteer);

            string HashedPassword = VolunteerManager.HashPassword(volunteer.Password);
            volunteer.Password = HashedPassword;

            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteer);

            lock (AdminManager.BlMutex) // stage 7
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
