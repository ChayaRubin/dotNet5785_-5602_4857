
using DalApi;
namespace Dal;
/// <summary>
/// A new DalXml class that inherits from IDal.
/// </summary>

public class DalXml : IDal
{
    public IAssignment Assignment => new AssignmentImplementation();

    public IVolunteer Volunteer => new VolunteerImplementation();

    public ICall Call => new CallImplementation();

    public IConfig Config => new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();

    }
}
