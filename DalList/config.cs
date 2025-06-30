/*namespace Dal;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Reset()
    {
        nextCallId = firstCallId;
        nextAssignmentId= firstAssignmentId;
        Clock = DateTime.Now;     
        RiskRange = TimeSpan.Zero; 
    }
}
*/
using System.Runtime.CompilerServices;

public static class Config
{
    public const int firstCallId = 1060;
    private static int nextCallId = firstCallId;

    public static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextCallId++;
    }

    public const int firstAssignmentId = 1030;
    private static int nextAssignmentId = firstAssignmentId;

    public static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextAssignmentId++;
    }

    private static DateTime clock = DateTime.Now;
    public static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => clock;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set => clock = value;
    }

    private static TimeSpan riskRange = TimeSpan.Zero;
    public static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => riskRange;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set => riskRange = value;
    }

    private static TimeSpan maxRange = TimeSpan.Zero;
    public static TimeSpan MaxRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => maxRange;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set => maxRange = value;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Reset()
    {
        nextCallId = firstCallId;
        nextAssignmentId = firstAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}
