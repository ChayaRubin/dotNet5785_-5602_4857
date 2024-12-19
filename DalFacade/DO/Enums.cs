namespace DO;

/// <summary>
/// Enum for Distance TypeEnum
/// </summary>
public enum DistanceTypeEnum
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

/// <summary>
/// Enum for Call Resolution Status
/// </summary>
public enum CallResolutionStatus
{
    Treated = 1,      
    SelfCanceled, 
    AdminCanceled, 
    Expired        
}

/// <summary>
/// Enum for Call Type
/// </summary>
public enum CallType
{
    Urgent = 1,
    Medium_Urgency,
    General_Assistance,
    Non_Urgent,
}

/// <summary>
/// Enum for Main Menu Option
/// </summary>
public enum MainMenuOption
{
    Exit,
    VolunteerMenu,
    CallMenu,
    AssignmentMenu,
    InitializeData,
    DisplayAllData,
    ConfigMenu,
    ResetDatabase
}

/// <summary>
/// Enum for Action Menu
/// </summary>
public enum ActionMenu
{
    Exit,
    Create,
    Read,
    ReadAll,
    Update,
    Delete,
    DeleteAll
}

/// <summary>
/// Enum for Config Menu
/// </summary>
public enum ConfigMenu
{
    Exit,
    AdvanceClockByMinute,
    AdvanceClockByHour,
    AdvanceClockByDay,
    AdvanceClockByMonth,
    AdvanceClockByYear,
    DisplayClock,
    ChangeRiskRange,
    DisplayRiskRange,
    Reset
}