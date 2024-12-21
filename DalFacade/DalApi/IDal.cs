
namespace DalApi;

public interface IDal
{
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IConfig Config { get; }
    void ResetDB();
}

