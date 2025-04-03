
namespace BO;

/// <summary>
/// Enum for Distance TypeEnum
/// </summary>
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}

/// <summary>
/// Enum for PositionEnum Status
/// </summary>
public enum PositionEnum
{
    Manager,
    Volunteer
}

public enum CallTypeEnum
{
    Urgent,
    Medium_Urgency,
    General_Assistance,
    Non_Urgent,
}

// סטטוס הקריאה (ENUM)
public enum CallStatus
{
    Open = 1,            // פתוחה - לא בטיפול של אף מתנדב כרגע
    InProgress,          // בטיפול - בטיפול כרגע על ידי מתנדב
    Closed,              // סגורה - מתנדב סיים לטפל בה
    SelfCanceled,
    Canceled,
    Expired,             // פג תוקף - לא נבחרה לטיפול או לא הסתיימה בזמן
    OpenAtRisk,          // פתוחה בסיכון - קריאה פתוחה שמתקרבת לזמן הסיום
    InProgressAtRisk     // בטיפול בסיכון - קריאה בטיפול שמתקרבת לזמן הסיום
}


public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR,
    UNDEFINED
}

public enum CallField
{
    Id,
    FullName,
    TotalHandledCalls,
    TotalCanceledCalls,
    TotalExpiredCalls,
    CurrentCallId,
    CurrentCallType
}

public enum VolunteerSortBy
{
    FullName,
    TotalHandledCalls,
    TotalCanceledCalls,
    TotalExpiredCalls
}


public enum CallListFilter
{
    CallType,
    Address
}

public enum CallListSortBy
{
    CallId,
    StartTime
}
