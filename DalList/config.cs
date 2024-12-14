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
    public const int firstCallId = 1000; 
    public static int NextCallId = firstCallId;
    public static int getNextCallId { get => NextCallId++; }
 
    public const int firstAssignmentId = 1000; 
    public static int NextAssignmentId = firstAssignmentId;
    public static int getNextAssignmentId { get => NextAssignmentId++; }
    public static DateTime Clock { get; set; } = DateTime.Now;

    public static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    public static void Reset()
    {
        NextCallId = firstCallId;
        NextAssignmentId= firstAssignmentId;
        Clock = DateTime.Now;     
        RiskRange = TimeSpan.Zero; 
    }


}






