

namespace BO
{
    public class Call
    {
        public int Id { get; init; }

        public CallTypeEnum Type { get; set; }

        public string? Description { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime OpenTime { get; init; }

        public DateTime? MaxEndTime { get; set; }

        public CallStatus Status { get; set; }


        public List<CallAssignInList>? Assignments { get; set; }

        // Overriding ToString for debugging purposes
        public override string ToString()
        {
            string assignmentsSummary = Assignments != null
                ? $"Assignments Count: {Assignments.Count}"
                : "No Assignments";

            return $@"
Call Details:
  Id: {Id}
  Type: {Type}
  Description: {(string.IsNullOrEmpty(Description) ? "N/A" : Description)}
  Address: {Address}
  Latitude: {Latitude}, Longitude: {Longitude}
  OpenTime: {OpenTime}
  MaxEndTime: {(MaxEndTime.HasValue ? MaxEndTime.Value.ToString() : "N/A")}
  Status: {Status}
  {assignmentsSummary}
";
        }
    }
}

