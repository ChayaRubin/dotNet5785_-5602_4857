
namespace DalApi;

public interface IDal
{
    IAssignment Assignment { get; }
    ICall Volunteer { get; }
    ICall Call { get; }
    IConfig Config { get; }
    void ResetDB();
}

