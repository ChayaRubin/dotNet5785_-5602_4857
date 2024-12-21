namespace DalApi
{
    /// <summary>
    /// Defines the data access layer (DAL) interface that provides access to various components of the system.
    /// This interface exposes properties to manage assignments, volunteers, calls, and configurations, 
    /// as well as a method to reset the database.
    /// </summary>
    public interface IDal
    {
        /// <summary>
        /// Gets the <see cref="IAssignment"/> component responsible for managing assignments in the system.
        /// </summary>
        IAssignment Assignment { get; }

        /// <summary>
        /// Gets the <see cref="IVolunteer"/> component responsible for managing volunteers in the system.
        /// </summary>
        IVolunteer Volunteer { get; }

        /// <summary>
        /// Gets the <see cref="ICall"/> component responsible for managing calls in the system.
        /// </summary>
        ICall Call { get; }

        /// <summary>
        /// Gets the <see cref="IConfig"/> component responsible for managing configuration settings in the system.
        /// </summary>
        IConfig Config { get; }

        /// <summary>
        /// Resets the database by deleting all assignments, volunteers, calls, and resetting configurations.
        /// </summary>
        void ResetDB();
    }
}
