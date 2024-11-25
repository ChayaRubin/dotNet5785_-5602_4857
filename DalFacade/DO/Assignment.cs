namespace DO;

/// <summary>
/// Assignment Entity represents a link between a call and a volunteer
/// </summary>
/// <param name="Id">Unique identifier for the assignment entity</param>
/// <param name="CallId">The ID of the call being handled</param>
/// <param name="VolunteerId">The ID of the volunteer handling the call</param>
/// <param name="StartDateTime">The time the call was assigned to the volunteer</param>
/// <param name="EndDateTime">The time the volunteer finished handling the call (nullable)</param>
/// <param name="CompletionType">Enum indicating how the call was completed (nullable)</param>
public class Assignment
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ActualCompletionTime { get; set; }

    public CallResolutionStatus callResolutionStatus { get; set; }

    //public Assignment() : this(Config.,Call.MyRadioCallId, , "", null, null, null, null, PositionEnum.Volunteer, false, null, DistanceTypeEnum.AirDistance) { }

}





