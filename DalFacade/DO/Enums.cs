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
    Treated,      
    SelfCanceled, 
    AdminCanceled, 
    Expired        
}

/// <summary>
/// Enum for Call Type
/// </summary>
public enum CallType
{
    Urgent,
    Medium_Urgency,
    General_Assistance,
    Non_Urgent,
}