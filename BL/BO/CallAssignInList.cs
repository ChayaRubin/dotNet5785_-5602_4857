
//using static BO.Enums

namespace BO
{
    /// <summary>
    /// Represents a logical data entity for a call assignment in the list. This entity is read-only
    /// and displays assignment details for use in management screens, such as the "Single Call Management" screen for managers.
    /// </summary>
    public class CallAssignInList
    {
        /// <summary>
        /// Gets or sets the unique ID of the assignment.
        /// </summary>
        public int Id { get; init; } // Sourced from DO.Assignment
   
        /// <summary>
        /// Gets or sets the ID of the volunteer.
        /// Nullable: This can be null in cases where the assignment was created artificially
        /// due to an unhandled call being updated to the "Expired Cancellation" end type.
        /// </summary>
        public int? VolunteerId { get; init; } // Sourced from DO.Assignment

        /// <summary>
        /// Gets or sets the name of the volunteer.
        /// Nullable: This can be null if no volunteer is associated with the assignment.
        /// </summary>
        public string? VolunteerName { get; set; } // Sourced from DO.Volunteer

        /// <summary>
        /// Gets or sets the time the assignment started.
        /// Non-nullable: This field cannot be null.
        /// </summary>
        public DateTime AssignTime { get; set; } // Sourced from DO.Assignment

        /// <summary>
        /// Gets or sets the actual time the assignment was completed.
        /// Nullable: This can be null if the assignment is still in progress or was canceled.
        /// </summary>
        public DateTime? CompletionTime { get; set; } // Sourced from DO.Assignment

        /// <summary>
        /// Gets or sets the type of treatment termination.
        /// Nullable: This can be null if the assignment is ongoing.
        /// </summary>
        public CallStatus? EndType { get; set; } // Sourced from DO.Assignment


     
    }
}
