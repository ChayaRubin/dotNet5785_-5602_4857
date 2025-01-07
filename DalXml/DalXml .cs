//נעזרנו בגיפיטי בתיעוד  בכמה מקומות.
using DalApi;
using System.Diagnostics;
namespace Dal
{
    /// <summary>
    /// A new DalXml class that implements the IDal interface.
    /// Provides access to implementations for managing Assignments, Volunteers, Calls, and Configuration settings.
    /// Also includes functionality to reset the database by clearing all stored data.
    /// </summary>
    sealed internal class DalXml : IDal
    {
        /// <summary>
        /// Static readonly property initialized to a new instance of the class
        /// </summary>
        public static IDal Instance { get; } = new DalXml();
        /// <summary>
        /// private ctor
        /// </summary>
        private DalXml() { }
        /// <summary>
        /// Provides access to the Assignment implementation, allowing for management of Assignment data.
        /// </summary>
        public IAssignment Assignment => new AssignmentImplementation();

        /// <summary>
        /// Provides access to the Volunteer implementation, enabling management of Volunteer data.
        /// </summary>
        public IVolunteer Volunteer => new VolunteerImplementation();

        /// <summary>
        /// Provides access to the Call implementation, allowing for management of Call data.
        /// </summary>
        public ICall Call => new CallImplementation();

        /// <summary>
        /// Provides access to the Config implementation, enabling management of configuration settings.
        /// </summary>
        public IConfig Config => new ConfigImplementation();

        /// <summary>
        /// Resets the database by deleting all stored data for Volunteers, Calls, Assignments, and Configurations.
        /// This method ensures that all data is cleared and the system is returned to its default state.
        /// </summary>
        public void ResetDB()
        {
            Volunteer.DeleteAll();
            Call.DeleteAll();
            Assignment.DeleteAll();
            Config.Reset();
        }
    }
}

