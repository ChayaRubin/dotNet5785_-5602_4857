namespace DO;

public enum DistanceTypeEnum
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}

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

public enum CallType
{
    Urgent,
    Medium_Urgency,
    General_Assistance,
    Non_Urgent,
}