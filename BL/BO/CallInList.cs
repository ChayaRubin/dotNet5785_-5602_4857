using System;
using System.Collections.Generic;
//using static BO.Enums;

namespace BO;

public class CallInList
{
    public int Id { get; init; } // מספר מזהה רץ של ישות הקריאה

    public CallTypeEnum Type { get; set; } // סוג הקריאה (ENUM)

    public string? Description { get; set; } // תיאור מילולי

    public string Address { get; set; } // כתובת מלאה של הקריאה

    public double Latitude { get; set; } // קו רוחב

    public double Longitude { get; set; } // קו אורך

    public DateTime? OpenTime { get; set; } // זמן פתיחה

    public TimeSpan? MaxEndTime { get; set; } // זמן מקסימלי לסיום הקריאה

    public TimeSpan? completeTime { get; set; }
    public CallStatus Status { get; set; } // סטטוס הקריאה (ENUM)
    public List<CallAssignInList>? Assignments { get; set; } // רשימת הקצאות עבור הקריאה

    public int AssignmentId { get; set; }

    public String LastVolunteerName { get; set; }

    public int TotalAssignments { get; set; }
}


