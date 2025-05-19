namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Provides an implementation of the IConfig interface for managing configuration settings.
/// This class interacts with a shared Config object to manage settings like the system clock and risk range.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the system clock, representing the current time in the system's configuration.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range, represented as a TimeSpan, which defines a configurable range of risk-related time.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// Resets all configuration settings to their default values.
    /// This method ensures that the system configuration is cleared and restored to its initial state.
    /// </summary>
    /// 

    public TimeSpan MaxRange
    {
        get => Config.MaxRange;
        set => Config.MaxRange = value;
    }


    public void Reset() => Config.Reset();
}

