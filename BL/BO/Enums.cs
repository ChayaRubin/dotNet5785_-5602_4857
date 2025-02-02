
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
        Urgent = 1,
        Medium_Urgency,
        General_Assistance,
        Non_Urgent,
    }

    // סטטוס הקריאה (ENUM)
    public enum CallStatus
    {
        Treated = 1,
        SelfCanceled,
        AdminCanceled,
        Expired
    }

    public enum TimeUnit
    {
        MINUTE,
        HOUR,
        DAY,
        MONTH,
        YEAR
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
