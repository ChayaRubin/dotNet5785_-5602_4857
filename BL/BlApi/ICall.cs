using BO;
namespace BlApi;
public interface ICall : IObservable //stage 5 
{
    Dictionary<CallStatus, int> GetCallCountsByStatus();
    IEnumerable<BO.CallInList> GetCallList(
        CallField? filterByField = null,
        object? filterValue = null,
        CallField? sortByField = null);

    //מתודת בקשת פרטי קריאה
    Call GetCallDetails(int callId);

    //מתודת עדכון פרטי קריאה
    void UpdateCallDetails(Call call);

    //מתודת מחיקת קריאה
    void DeleteCall(int callId);

    //מתודת הוספת קריאה
    Task AddCall(Call newCall);

    //מתודת בקשת רשימת קריאות סגורות שטופלו על ידי מתנדב
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
        int volunteerId,
        Enum? callType = null,
        Enum? sortByField = null);

    //מתודת בקשת רשימת קריאות פתוחות לבחירה על ידי מתנדב
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(
        int volunteerId,
        Enum? callType = null,
        Enum? sortByField = null);

    //מתודת עדכון "סיום טיפול" בקריאה
    void CloseCall(int volunteerId, int assignmentId);

    //מתודת עדכון "ביטול טיפול" בקריאה
    void CancelCall(int requestorId, int assignmentId);

    void AssignCall(int volunteerId, int callId);
    Task SendEmailToVolunteers(Call call);
    int? GetAssignmentId(int callId, int volunteerId);
    IEnumerable<OpenCallInList> GetAvailableOpenCalls(int volunteerId);
    double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2);
}