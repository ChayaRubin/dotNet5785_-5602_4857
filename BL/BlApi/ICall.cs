using BO;
namespace BlApi;

public interface ICall
{
    //IEnumerable<int> GetCallCountsByStatus();

    // מתודת בקשת רשימת קריאות
    IEnumerable<BO.CallInList> GetCallList(
        Enum? filterByField = null,
        object? filterValue = null,
        Enum? sortByField = null);

    // מתודת בקשת פרטי קריאה
    Call GetCallDetails(int callId);

    // מתודת עדכון פרטי קריאה
    //void UpdateCallDetails(Call call);

    //// מתודת מחיקת קריאה
    //void DeleteCall(int callId);

    //// מתודת הוספת קריאה
    //void AddCall(Call newCall);

    //// מתודת בקשת רשימת קריאות סגורות שטופלו על ידי מתנדב
    //IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
    //    int volunteerId,
    //    Enum? callType = null,
    //    Enum? sortByField = null);

    //// מתודת בקשת רשימת קריאות פתוחות לבחירה על ידי מתנדב
    //IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(
    //    int volunteerId,
    //    Enum? callType = null,
    //    Enum? sortByField = null);

    //// מתודת עדכון "סיום טיפול" בקריאה
    //void CloseCall(int volunteerId, int assignmentId);

    //// מתודת עדכון "ביטול טיפול" בקריאה
    //void CancelCall(int requestorId, int assignmentId);

    //// מתודת בחירת קריאה לטיפול
    //void AssignCall(int volunteerId, int callId);
}


