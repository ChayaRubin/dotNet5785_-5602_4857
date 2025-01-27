using System;
using static BO.Enums;

namespace BO
{
    public class VolunteerInList
    {
        public int Id { get; init; } // ת.ז מתנדב
        public string FullName { get; set; } // שם מלא (פרטי ומשפחה)
        public bool IsActive { get; set; } // האם פעיל
        public int TotalHandledCalls { get; set; } // סך הקריאות שטופלו
        public int TotalCanceledCalls { get; set; } // סך הקריאות שבוטלו
        public int TotalExpiredCalls { get; set; } // סך הקריאות שפג תוקפן
        public int? CurrentCallId { get; set; } // מספר מזהה של הקריאה בטיפולו (אם קיימת)
        public CallTypeEnum CurrentCallType { get; set; } // סוג הקריאה שבטיפולו

        /// <summary>
        /// Provides a string representation of the object for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the VolunteerInList object.</returns>
        public override string ToString()
        {
            return $"VolunteerInList:\n" +
                   $"  Id: {Id}\n" +
                   $"  FullName: {FullName}\n" +
                   $"  IsActive: {IsActive}\n" +
                   $"  TotalHandledCalls: {TotalHandledCalls}\n" +
                   $"  TotalCanceledCalls: {TotalCanceledCalls}\n" +
                   $"  TotalExpiredCalls: {TotalExpiredCalls}\n" +
                   $"  CurrentCallId: {(CurrentCallId.HasValue ? CurrentCallId.ToString() : "None")}\n" +
                   $"  CurrentCallType: {CurrentCallType}";
        }
    }
}
