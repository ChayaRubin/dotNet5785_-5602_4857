using Helpers;
using System.Security.Authentication;
using BlApi;
using Helpers;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;
using System.Security.Authentication;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    public BO.Position Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll()
            .Select(v => VolunteerManager.MapVolunteer(v))
            .FirstOrDefault(v => v.FullName == username);

        if (volunteer == null || !VolunteerManager.VerifyPassword(password, volunteer.Password!))
            throw new AuthenticationException("שם משתמש או סיסמה שגויים.");
        return volunteer.MyPosition;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.SortByField? sortBy = null)
    {

        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
         !isActive.HasValue || v.IsActive == isActive.Value);

        var volunteerList = VolunteerManager.GetVolunteerList(volunteers);
        volunteerList = sortBy.HasValue ? sortBy.Value switch
        {
            BO.SortByField.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
            BO.SortByField.IsActive => volunteerList.OrderBy(v => v.IsActive).ToList(),
            BO.SortByField.TotalExpiredReadings => volunteerList.OrderBy(v => v.TotalExpiredReadings).ToList(),
            BO.SortByField.TotalCallsCanceled => volunteerList.OrderBy(v => v.TotalCallsCanceled).ToList(),
            BO.SortByField.TotalCallsHandled => volunteerList.OrderBy(v => v.TotalCallsHandled).ToList(),
            _ => volunteerList.OrderBy(v => v.Id).ToList()
        } : volunteerList.OrderBy(v => v.Id).ToList();

        return volunteerList;
    }


    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {

        var dalVolunteer = _dal.Volunteer.Read(volunteerId);

        if (dalVolunteer == null)
            throw new KeyNotFoundException("מתנדב עם תעודת הזהות המבוקשת לא נמצא.");

        var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndingTimeOfTreatment == null).FirstOrDefault();
        BO.CallInProgress? callInProgress = null;

        if (currentAssignment != null)
        {
            var callDetails = _dal.Call.Read(currentAssignment.CallId);
            if (callDetails != null)
                callInProgress = new BO.CallInProgress
                {
                    Id = currentAssignment.Id,
                    CallId = currentAssignment.CallId,
                    MyCall = (BO.CallType)callDetails.MyCall,
                    VerbalDescription = callDetails.VerbalDescription,
                    FullAddressCall = callDetails.FullAddressCall,
                    OpeningTime = callDetails.OpeningTime,
                    MaxTimeFinishCalling = callDetails.MaxTimeFinishCalling,
                    EntryTimeTreatment = currentAssignment.EntryTimeOfTreatment,
                    ReadingDistanceFromCaringVolunteer = Tools.CalculateDistance(dalVolunteer.Latitude ?? 0, dalVolunteer.Longitude ?? 0, callDetails.Latitude, callDetails.Longitude),
                    myStatus = Tools.CalculateStatus(currentAssignment, callDetails, 30)//לבדוק למה זה נכון מה שאני שו
                };
        }

        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = dalVolunteer.FullName,
            Email = dalVolunteer.Email,
            Phone = dalVolunteer.Phone,
            MyPosition = (BO.Position)dalVolunteer.MyPosition,
            IsActive = dalVolunteer.IsActive,
            MaxDistance = dalVolunteer.MaxDistance,
            Password = dalVolunteer.Password,
            CurrentAddress = dalVolunteer.CurrentAddress,
            Longitude = dalVolunteer.Longitude,
            Latitude = dalVolunteer.Latitude,
            MyDistance = (BO.DistanceType)dalVolunteer.MyDistance,
            totalChosenAndExpiredCalls = callInProgress
        };
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {

        VolunteerManager.ValidateInputFormat(volunteer);
        var coordinates = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress);


        volunteer.Latitude = coordinates.Latitude;
        volunteer.Longitude = coordinates.Longitude;
        VolunteerManager.ValidatePermissions(id, volunteer);

        var originalVolunteer = _dal.Volunteer.Read(volunteer.Id)!;
        var changedFields = VolunteerManager.GetChangedFields(originalVolunteer, volunteer);
        if (!VolunteerManager.CanUpdateFields(id, changedFields, volunteer))
            throw new UnauthorizedAccessException("You do not have permission to update the Role field.");

        DO.Volunteer doVolunteer = Helpers.VolunteerManager.CreateDoVolunteer(volunteer);

        _dal.Volunteer.Update(doVolunteer);
    }

    public void DeleteVolunteer(int id)
    {
        var volunteerToDelete = _dal.Volunteer.Read(id);
        IEnumerable<DO.Assignment> assignmentsOfVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == id);
        if (assignmentsOfVolunteer.Any())
        {

        }
        _dal.Volunteer.Delete(id);

    }
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        var existingVolunteer = _dal.Volunteer.Read(volunteer.Id);

        VolunteerManager.ValidateInputFormat(volunteer);
        var (latitude, longitude) = VolunteerManager.logicalChecking(volunteer);
        if (latitude != null && longitude != null)
        {
            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;
        }
        DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteer);

        _dal.Volunteer.Create(doVolunteer);
    }






}
VolunteerImplementation.cs
Displaying image.png.Next