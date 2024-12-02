namespace DalApi;
using DO;

public interface IConfig 
{
    int FirstCallId { get; }
    int NextCallId { get; set; }
    int NextAssignmentId { get; }
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();
}
