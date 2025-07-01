namespace Dal;
using DalApi;

/// <summary>
/// This class implements the IDal interface and provides access to various data access objects (DAOs)
/// for managing <see cref="Assignment"/>, <see cref="Volunteer"/>, <see cref="Call"/>, and <see cref="Config"/> entities.
/// It encapsulates the logic for interacting with these entities and their respective implementations.
/// </summary>
sealed internal class DalList : IDal
{
    /// <summary>
    /// Static readonly property initialized to a new instance of the class
    /// </summary>
    public static IDal Instance { get; } = new DalList();
    /// <summary>
    /// private ctor
    /// </summary>
    private DalList() { }

    /// <summary>
    /// Gets the <see cref="IAssignment"/> implementation for interacting with assignment-related data.
    /// Provides methods for performing CRUD operations on assignments.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Gets the <see cref="IVolunteer"/> implementation for interacting with volunteer-related data.
    /// Provides methods for performing CRUD operations on volunteers.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Gets the <see cref="ICall"/> implementation for interacting with call-related data.
    /// Provides methods for performing CRUD operations on calls.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Gets the <see cref="IConfig"/> implementation for interacting with configuration settings.
    /// Provides methods to manage and reset application configuration.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the entire database by deleting all assignments, volunteers, and calls, and resetting the configuration settings.
    /// This method is useful for re-initializing the database to its default state.
    /// </summary>
    public void ResetDB()
    {
        Assignment.DeleteAll(); 
        Volunteer.DeleteAll();   
        Call.DeleteAll();        
        Config.Reset();          
    }
}


