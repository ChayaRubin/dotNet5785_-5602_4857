using System.Windows.Media.Animation;

namespace Dal;
/// <summary>
/// configity Entity 
/// </summary>
/// <param name="firstCallId"></param>
/// <param name="NextCallId">The identifier for the next incoming call. Automatically increments by 1 with each new call</param>
/// <param name="NextAssignmentId">The identifier for the next assignment instance between a volunteer and a call. Increments by 1 with each new assignment</param>
/// <param name="Clock"> A time range after which a call is considered at risk, nearing its required end time.</param>
/// <param name="RiskRange"> A time range after which a call is considered at risk, nearing its required end time.</param>
internal static class Config
{
    internal const int firstCallId = 1000;
    private static int NextCallId = firstCallId;
    internal static int NextAssignmentId { get => NextCallId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    internal static void Reset()
    {
        NextCallId = firstCallId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
        Clock = DateTime.Now;
    }

}






