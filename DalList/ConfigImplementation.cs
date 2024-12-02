using DalApi;
using DO;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    public int FirstCallId => Config.firstCallId;
    public int NextCallId
    {
        get => Config.NextCallId;
        set => Config.NextCallId = value;
    }
    public int NextAssignmentId => Config.NextAssignmentId;
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
