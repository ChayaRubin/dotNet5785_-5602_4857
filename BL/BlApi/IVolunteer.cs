using BO;
namespace BlApi;

public interface IVolunteer : IObservable //stage 5 
{

    // 1. מתודת כניסה למערכת
    string Login(string username, string password);

    //// 2. מתודת בקשת רשימת מתנדבים
    //IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, VolunteerSortBy? fieldToSortBy);
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerSortBy? fieldToSortBy = null, CallTypeEnum? callType = null);

    //// 3. מתודת בקשת פרטי מתנדב
    BO.Volunteer GetVolunteerDetails(string idNumber);

    //// 4. מתודת עדכון פרטי מתנדב
    void UpdateVolunteerDetails(string idNumber, BO.Volunteer volunteer);

    //// 5. מתודת בקשת מחיקת מתנדב
    void DeleteVolunteer(int idNumber);

    //// 6. מתודת הוספת מתנדב
    void AddVolunteer(BO.Volunteer volunteer);
}


