using System;

namespace DO
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Call"/> class with specific values for all properties.
    /// </summary>
    /// <param name="myRadioCallId">The unique identifier for the radio call.</param>
    /// <param name="myDescription">The description of the call.</param>
    /// <param name="myAddress">The address associated with the call.</param>
    /// <param name="myLatitude">The latitude of the call's location.</param>
    /// <param name="myLongitude">The longitude of the call's location.</param>
    /// <param name="myStartTime">The start time of the call.</param>
    /// <param name="myExpiredTime">The expiration time of the call.</param>
    ///
    public record Call
    {
        public int RadioCallId { get; set; }
        public string? Description { get; set; }
        public CallType CallType { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpiredTime { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        //public Call() : this(Config.NextAssignmentId, "", "", 0, 0, DateTime.MinValue, DateTime.MinValue) { }

        /// <summary>
        /// Custom constructor with specific values for all fields.
        /// </summary>
        public Call(int MyRadioCallId, string MyDescription, string MyAddress, double MyLatitude, double MyLongitude, DateTime MyStartTime,
        DateTime MyExpiredTime)
        {
            RadioCallId = MyRadioCallId;
            Description = MyDescription;
            Address = MyAddress;
            Latitude = MyLatitude;
            Longitude = MyLongitude;
            StartTime = MyStartTime;
            ExpiredTime = MyExpiredTime;
        }
    }
}
