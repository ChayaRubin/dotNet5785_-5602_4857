using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static BO.Enums;

namespace BO
{
    public class CallInProgress
    {
        public int Id { get; init; } // מספר מזהה של ישות ההקצאה (לא יופיע בתצוגה)
        public int CallId { get; set; } // מספר מזהה רץ של ישות הקריאה
        public CallTypeEnum CallType { get; set; } // סוג הקריאה
        public string? Description { get; set; } // תיאור מילולי
        public string Address { get; set; } // כתובת מלאה של הקריאה
        public DateTime OpeningTime { get; set; } // זמן פתיחה
        public DateTime? MaxCompletionTime { get; set; } // זמן מקסימלי לסיום הקריאה
        public DateTime AssignmentStartTime { get; set; } // זמן כניסה לטיפול
        public double DistanceFromVolunteer { get; set; } // מרחק הקריאה מהמתנדב המטפל
        public CallStatus Status { get; set; } // סטטוס הקריאה

        public override string ToString()
        {
            return $"--- Call In Progress ---\n" +
                   $"Assignment ID: {Id}\n" +
                   $"Call ID: {CallId}\n" +
                   $"Call Type: {CallType}\n" +
                   $"Description: {Description ?? "No description"}\n" +
                   $"Address: {Address}\n" +
                   $"Opening Time: {OpeningTime}\n" +
                   $"Max Completion Time: {(MaxCompletionTime?.ToString() ?? "N/A")}\n" +
                   $"Assignment Start Time: {AssignmentStartTime}\n" +
                   $"Distance From Volunteer: {DistanceFromVolunteer:F2} km\n" +
                   $"Status: {Status}\n";
        }

    }

}
