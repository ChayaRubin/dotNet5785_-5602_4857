using DalApi;
using DO;

namespace Dal
{
    /// <summary>
    /// Implementation of the <see cref="IConfig"/> interface that manages configuration settings for the application.
    /// Provides properties to access and modify various configuration values such as call IDs, assignment IDs,
    /// system clock, and risk range, along with a method to reset the configuration.
    /// </summary>
    internal class ConfigImplementation : IConfig
    {
        /// <summary>
        /// Gets the first call ID from the configuration.
        /// This value represents the initial call ID used in the system.
        /// </summary>
        public int FirstCallId => Config.firstCallId;

        /// <summary>
        /// Gets or sets the next call ID from the configuration.
        /// This value is incremented after each new call is added to the system.
        /// </summary>
        public int NextCallId
        {
            get => Config.NextCallId;    // Gets the next call ID
            //set => Config.NextCallId = value; // Sets the next call ID
        }

        /// <summary>
        /// Gets the next assignment ID from the configuration.
        /// This value is used for assigning IDs to new assignments in the system.
        /// </summary>
        public int NextAssignmentId => Config.NextAssignmentId;

        /// <summary>
        /// Gets or sets the system clock from the configuration.
        /// This value represents the current date and time in the system.
        /// </summary>
        public DateTime Clock
        {
            get => Config.Clock;   // Gets the current system time
            set => Config.Clock = value; // Sets the current system time
        }

        /// <summary>
        /// Gets or sets the risk range from the configuration.
        /// This value defines the acceptable range for risk calculations within the system.
        /// </summary>
        public TimeSpan RiskRange
        {
            get => Config.RiskRange;  // Gets the current risk range
            set => Config.RiskRange = value;  // Sets the new risk range
        }

        /// <summary>
        /// Resets the configuration to its default state.
        /// This method is used to restore the initial settings for all configuration values.
        /// </summary>
        public void Reset() => Config.Reset();
    }
}

