/*

using DalApi;

namespace BlApi;

public interface IVolunteer
{
    // 1. מתודת כניסה למערכת
    string Login(string username, string password);

    // 2. מתודת בקשת רשימת מתנדבים
    // IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerField? fieldToSortBy);

    // 3. מתודת בקשת פרטי מתנדב
    BO.Volunteer GetVolunteerDetails(string idNumber);

    // 4. מתודת עדכון פרטי מתנדב
    void UpdateVolunteerDetails(string idNumber, BO.Volunteer volunteer);

    // 5. מתודת בקשת מחיקת מתנדב
    void DeleteVolunteer(string idNumber);

    // 6. מתודת הוספת מתנדב
    void AddVolunteer(BO.Volunteer volunteer);

}*/

using BlApi;
using DalApi;
using BO;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly IDal _dal = Factory.Get;

    // 1. מתודת כניסה למערכת
    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

     //2. מתודת בקשת רשימת מתנדבים(מושבתת כרגע)
     public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerField? fieldToSortBy)
    {
        throw new NotImplementedException();
    }

    // 3. מתודת בקשת פרטי מתנדב
    public BO.Volunteer GetVolunteerDetails(string idNumber)
    {
        throw new NotImplementedException();
    }

    // 4. מתודת עדכון פרטי מתנדב
    public void UpdateVolunteerDetails(string idNumber, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    // 5. מתודת בקשת מחיקת מתנדב
    public void DeleteVolunteer(string idNumber)
    {
        throw new NotImplementedException();
    }

    // 6. מתודת הוספת מתנדב
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}



