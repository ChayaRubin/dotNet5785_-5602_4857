using BlApi;
using BO;
using DalApi;
using Helpers;

namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    //private static readonly IDal _dal = BlApi.Factory.Get();
    private static readonly IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// Returns the current system clock time.
    /// </summary>
    /// <returns>The current system time.</returns>
    public DateTime GetSystemClock()
    {
        return ClockManager.Now;
    }

    /// <summary>
    /// Advances the system clock by a given time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit to advance (e.g., minute, hour, day, etc.).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid time unit is provided.</exception>
    public void AdvanceSystemClock(TimeUnit timeUnit)
    {
        DateTime newClock = timeUnit switch
        {
            TimeUnit.MINUTE => ClockManager.Now.AddMinutes(1),
            TimeUnit.HOUR => ClockManager.Now.AddHours(1),
            TimeUnit.DAY => ClockManager.Now.AddDays(1),
            TimeUnit.MONTH => ClockManager.Now.AddMonths(1),
            TimeUnit.YEAR => ClockManager.Now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit")
        };

        ClockManager.UpdateClock(newClock);
    }

    /// <summary>
    /// Gets the current risk time range.
    /// </summary>
    /// <returns>The configured risk time range.</returns>
    public TimeSpan GetRiskTimeRange()
    {
        return _dal.Config.RiskRange;
    }

    /// <summary>
    /// Sets the risk time range.
    /// </summary>
    /// <param name="riskTimeRange">The new risk time range value.</param>
    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        _dal.Config.RiskRange = riskTimeRange;
    }

    /// <summary>
    /// Resets the database configuration.
    /// </summary>
    public void ResetDatabase()
    {
        _dal.Config.Reset();
    }

    /// <summary>
    /// Initializes the database.
    /// </summary>
    public void InitializeDatabase()
    {
        _dal.Config.Reset();
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}
