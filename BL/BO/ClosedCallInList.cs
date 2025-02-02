using System;
using BO;
//using static BO.Enums;

namespace BO
{
    /// <summary>
    /// Represents a closed call in a list, used for display purposes only.
    /// This entity is part of the "Volunteer Call History" screen, specifically for a volunteer.
    /// </summary>
    public class ClosedCallInList
    {
        /// <summary>
        /// A unique identifier for the call entity.
        /// Retrieved from DO.Call.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The type of the call.
        /// Retrieved from DO.Call.
        /// </summary>
        public CallTypeEnum Type { get; set; }

        /// <summary>
        /// The full address of the call.
        /// Retrieved from DO.Call.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The time the call was opened.
        /// Retrieved from DO.Call.
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The time the call entered treatment.
        /// Retrieved from DO.Assignment.
        /// </summary>
        public DateTime StartTreatmentTime { get; set; }

        /// <summary>
        /// The actual time the treatment ended.
        /// Retrieved from DO.Assignment. Nullable because it may not have ended yet.
        /// </summary>
        public DateTime? EndTreatmentTime { get; set; }

        /// <summary>
        /// The type of treatment completion.
        /// Retrieved from DO.Assignment. Nullable because the status might not have been updated.
        /// </summary>
        public CallStatus? CompletionType { get; set; }


        /// <summary>
        /// The type of treatment completion.
        /// Retrieved from DO.Assignment. Nullable because the status might not have been updated.
        /// </summary>

        public override string ToString()
        {
            // את יכולה להמיר את הערכים למחרוזת בצורה קריאה
            return $"Call ID: {Id}, " +
                   $"Type: {Type}, " +
                   $"Address: {Address}, " +
                   $"Open Time: {OpenTime}, " +
                   $"Start Treatment: {StartTreatmentTime}, " +
                   $"End Treatment: {EndTreatmentTime?.ToString() ?? "Not Ended"}, " +
                   $"Completion Type: {CompletionType?.ToString() ?? "Not Updated"}";
        }
    }
}
