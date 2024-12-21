namespace Dal;
using DalApi;

sealed public class DalList : IDal
{
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}

