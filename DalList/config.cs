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
