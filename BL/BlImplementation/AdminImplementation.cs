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
        return AdminManager.Now;
    }

    /// <summary>
    /// Advances the system clock by a given time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit to advance (e.g., minute, hour, day, etc.).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid time unit is provided.</exception>
    public void AdvanceSystemClock(TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DateTime newClock = timeUnit switch
        {
            TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1),
            TimeUnit.HOUR => AdminManager.Now.AddHours(1),
            TimeUnit.DAY => AdminManager.Now.AddDays(1),
            TimeUnit.MONTH => AdminManager.Now.AddMonths(1),
            TimeUnit.YEAR => AdminManager.Now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit")
        };

        AdminManager.UpdateClock(newClock);
    }

    /// <summary>
    /// Gets the current risk time range.
    /// </summary>
    /// <returns>The configured risk time range.</returns>
    public TimeSpan GetRiskTimeRange()
    {
        return AdminManager.RiskRange;
    }

    /// <summary>
    /// Sets the risk time range.
    /// </summary>
    /// <param name="riskTimeRange">The new risk time range value.</param>
    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.RiskRange = riskTimeRange;
    }

    /// <summary>
    /// Resets the database configuration.
    /// </summary>
    public void ResetDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    /// <summary>
    /// Initializes the database.
    /// </summary>
    public void InitializeDatabase()
    {
         
        //AdminManager.Reset();
        //DalTest.Initialization.Do();
        //ClockManager.UpdateClock(ClockManager.Now);
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7

    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }


    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;

}
