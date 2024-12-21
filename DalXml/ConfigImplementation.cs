//using DalApi;
//using DO;
namespace Dal;
using DalApi;
using DO;

/// <summary>
/// A new ConfigImplementation class that inherits from IConfig.
/// </summary>

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public void Reset() => Config.Reset();

}
