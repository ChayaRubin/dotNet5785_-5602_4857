using BlApi;
using BO;
//using DalApi;
using DO;
using Helpers;
using System.Globalization;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public string Login(string username, string password)

    {
        var volunteer = _dal.Volunteer.Read(v => v.Name == username);

        if (volunteer == null || volunteer.Password != password)
            throw new BlUnauthorizedAccessException("Invalid username or password");

        return volunteer.Position.ToString();
    }

    public IEnumerable<BO.Volunteer> GetVolunteersList(bool? isActive, VolunteerSortBy? sortBy)
    {
        // שליפת המתנדבים עם סינון של פעילות
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
            !isActive.HasValue || v.Active == isActive.Value);

        // המרת המתנדבים ל-BO
        var volunteerList = VolunteerManager.GetVolunteerList(volunteers);

        // מיון הרשימה לפי הקריטריון שנבחר
        volunteerList = sortBy.HasValue ? sortBy.Value switch
        {
            VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
            VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalCallsHandled).ToList(),
            VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCallsCanceled).ToList(),
            VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalCallsExpired).ToList(),
            _ => volunteerList.OrderBy(v => v.Id).ToList()
        } : volunteerList.OrderBy(v => v.Id).ToList();

        // שליחת הרשימה הממוינת לפונקציה החיצונית
        return volunteerList;
    }

    public BO.Volunteer GetVolunteerDetails(string idNumber)
    {
        DO.Volunteer? volunteer = null;

        if (int.TryParse(idNumber, out int id))
        {
            volunteer = _dal.Volunteer.Read(v => v.Id == id);
        }
        else
        {
            throw new BlFormatException("Invalid ID format");
        }

        if (volunteer == null)
            throw new BlDoesNotExistException("Volunteer not found");

        BO.Volunteer volunteerBO = VolunteerManager.ConvertToBO(volunteer);

        return volunteerBO;
    }

    public void UpdateVolunteerDetails(string idNumber, BO.Volunteer volunteerBO)
    {
        try
        {
            // בדיקת תקינות ת.ז של המבקש
            if (!VolunteerManager.IsRequesterAuthorized(idNumber, volunteerBO))
            {
                throw new DalUnauthorizedAccessException("You do not have permission to update this volunteer.");
            }

            // ווידוא שהקלט תקין מבחינת פורמט
            VolunteerManager.ValidateInputFormat(volunteerBO);

            // ווידוא תקינות ערכים לוגיים
            VolunteerManager.ValidateLogicalFields(volunteerBO);

            // חישוב קווי אורך ורוחב מהכתובת
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerBO.CurrentAddress!);

            volunteerBO.Latitude = latitude;
            volunteerBO.Longitude = longitude;

            // קבלת המתנדב מה- DAL
            DO.Volunteer? existingVolunteer = null;

            if (int.TryParse(idNumber, out int id))
            {
                existingVolunteer = _dal.Volunteer.Read(v => v.Id == id);
            }
            else
            {
                throw new DalFormatException("Invalid ID format");
            }

            // ווידוא אילו שדות השתנו והאם מותר לשנותם
            if (!Helpers.VolunteerManager.CanUpdateFields(idNumber, existingVolunteer, volunteerBO))
            {
                throw new DalFormatException("You do not have permission to update the Role field.");
            }

            // המרת ה- BO ל- DO
            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteerBO);

            // עדכון המתנדב ב- DAL
            _dal.Volunteer.Update(volunteerDO);
        }
        catch (DO.DalFormatException ex)
        {
            throw new BlFormatException($"Invalid data for volunteer update: {ex.Message}");
        }
        catch (DO.DalUnauthorizedAccessException ex)
        {
            throw new BlUnauthorizedAccessException($"Invalid data for volunteer update: {ex.Message}");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"The volunteer with ID={volunteerBO.Id} was not found.");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.");
        }
    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(v => v.Id == id) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.FinishCompletionTime == null).FirstOrDefault();
            if (currentAssignment != null)
            {
                throw new InvalidOperationException("Cannot delete volunteer while they are handling a call.");
            }
            _dal.Volunteer.Delete(id);
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

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            // ווידוא שהקלט תקין מבחינת פורמט
            VolunteerManager.ValidateInputFormat(volunteer);

            // ווידוא שהקלט תקין מבחינת לוגית
            VolunteerManager.ValidateLogicalFields(volunteer);

            // חישוב קווי אורך ורוחב מהכתובת
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress!);

            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;

            // המרת ה- BO ל- DO
            DO.Volunteer volunteerDO = VolunteerManager.ConvertToDO(volunteer);
            try
            {
                _dal.Volunteer.Create(volunteerDO);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                // מתנדב כבר קיים עם ת.ז - זרוק חריגה מותאמת
                throw new DalDoesNotExistException($"Volunteer with ID={volunteer.Id} already exists. {ex.Message}");
            }
            catch (Exception ex)
            {
                // חריגות כלליות במהלך הוספת המתנדב
                throw new BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.");
            }

        }
        catch (DO.DalFormatException ex)
        {
            throw new BlFormatException($"Invalid data for volunteer update: {ex.Message}");
        }
        catch (DO.DalUnauthorizedAccessException ex)
        {
            throw new BlUnauthorizedAccessException($"Invalid data for volunteer update: {ex.Message}");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"The volunteer with ID={volunteer.Id} was not found.");
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.");
        }
    }
}
