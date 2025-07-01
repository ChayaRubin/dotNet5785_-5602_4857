using DO;
namespace DO;
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
    public DateTime? StartTime { get; set; }
    public DateTime? ExpiredTime { get; set; }

    /// <summary>
    /// Default constructor that initializes the object with default values.
    /// </summary>
    public Call() : this(1, "Default Description", CallType.General_Assistance, "Default Address", 0.0, 0.0, DateTime.Now, DateTime.Now.AddHours(1)) { }

    /// <summary>
    /// Custom constructor with specific values for all properties.
    /// </summary>
    public Call(int radioCallId, string description, CallType callType, string address, double latitude, double longitude, DateTime startTime, DateTime expiredTime)
    {
        RadioCallId = radioCallId;
        CallType = callType;
        Description = description;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
        StartTime = startTime;
        ExpiredTime = expiredTime;
    }
}




