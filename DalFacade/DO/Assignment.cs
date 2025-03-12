using System.Net;

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
public record Assignment
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? FinishCompletionTime { get; set; }
    public CallResolutionStatus CallResolutionStatus { get; set; }

    public Assignment()
    {
        Id = 0;  
        CallId = 0;  
        VolunteerId = 0;  
        EntryTime = DateTime.Now; 
        FinishCompletionTime = null; 
        CallResolutionStatus = CallResolutionStatus.Treated;  
    }
    public Assignment(int id, int callId, int volunteerId, DateTime entryTime, DateTime? finishCompletionTime, CallResolutionStatus 
        
        
        
        callResolutionStatus)
    {
        Id = id;
        CallId = callId;
        VolunteerId = volunteerId;
        EntryTime = entryTime;
        FinishCompletionTime = finishCompletionTime;
        this.CallResolutionStatus = callResolutionStatus;
    }


    //public override string ToString()
    //{
    //    return $"Id: {Id}, CallId: {CallId}, VolunteerId: {VolunteerId}, EntryTime: {EntryTime}, FinishCompletionTime: {FinishCompletionTime}, callResolutionStatus: {callResolutionStatus}";
    //}
}






