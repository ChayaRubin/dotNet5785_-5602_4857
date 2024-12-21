
using DalApi;
namespace Dal;
/// <summary>
/// A new DalXml class that inherits from IDal.
/// </summary>

public class DalXml : IDal
{
    public IAssignment Assignment => throw new NotImplementedException();

    public IVolunteer Volunteer => throw new NotImplementedException();

    public ICall Call => throw new NotImplementedException();

    public IConfig Config => throw new NotImplementedException();

    public void ResetDB()
    {
        throw new NotImplementedException();
    }
}
