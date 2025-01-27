using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi
{
    public interface IVolunteer
    {
        // 1. מתודת כניסה למערכת
        string Login(string username, string password);

        // 2. מתודת בקשת רשימת מתנדבים
        IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, VolunteerField? fieldToSortBy);

        // 3. מתודת בקשת פרטי מתנדב
        Volunteer GetVolunteerDetails(string idNumber);

        // 4. מתודת עדכון פרטי מתנדב
        void UpdateVolunteerDetails(string idNumber, Volunteer volunteer);

        // 5. מתודת בקשת מחיקת מתנדב
        void DeleteVolunteer(string idNumber);

        // 6. מתודת הוספת מתנדב
        void AddVolunteer(Volunteer volunteer);
    }

}
