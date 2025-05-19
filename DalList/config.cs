namespace Dal;
using DO;
using System.Collections.Generic;
/// <summary>
/// configity Entity 
/// </summary>
/// <param name="firstCallId"></param>
/// <param name="NextCallId">The identifier for the next incoming call. Automatically increments by 1 with each new call</param>
/// <param name="NextAssignmentId">The identifier for the next assignment instance between a volunteer and a call. Increments by 1 with each new assignment</param>
/// <param name="Clock"> A time range after which a call is considered at risk, nearing its required end time.</param>
/// <param name="RiskRange"> a time range after which a call is considered at risk, nearing its required end time.</param>

public static class Config
{
    public const int firstCallId = 1060; 
    private static int nextCallId = firstCallId;
    public static int NextCallId { get => nextCallId++; }
 
    public const int firstAssignmentId = 1030; 
    public static int nextAssignmentId = firstAssignmentId;
    public static int NextAssignmentId { get => nextAssignmentId++; }
    public static DateTime Clock { get; set; } = DateTime.Now;

    public static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    public static TimeSpan MaxRange { get; set; } = TimeSpan.Zero;

    public static void Reset()
    {
        nextCallId = firstCallId;
        nextAssignmentId= firstAssignmentId;
        Clock = DateTime.Now;     
        RiskRange = TimeSpan.Zero; 
    }
}






