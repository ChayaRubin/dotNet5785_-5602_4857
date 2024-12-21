namespace DalApi
{
    using DO;

    /// <summary>
    /// Defines the configuration settings for the system, including the system's clock, the risk range, and a method to reset configuration values.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets the current system clock time.
        /// </summary>
        DateTime Clock { get; set; }

        /// <summary>
        /// Gets or sets the range of risk used in the system, represented as a time span.
        /// </summary>
        TimeSpan RiskRange { get; set; }

        /// <summary>
        /// Resets the configuration settings to their default values.
        /// </summary>
        void Reset();
    }
}
