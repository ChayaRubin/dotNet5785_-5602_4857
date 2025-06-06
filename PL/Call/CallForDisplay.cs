public class CallForDisplay
{
    public BO.CallInList OriginalCall { get; set; }

    // Expose the Id for convenience
    public int Id => OriginalCall.Id;

    public string StatusText => OriginalCall.Status.ToString();

    public string RemainingTime =>
        OriginalCall.MaxEndTime.HasValue
            ? (OriginalCall.MaxEndTime.Value - DateTime.Now).ToString(@"hh\:mm")
            : "-";

    public string TotalTreatmentTime =>
        OriginalCall.Assignments?
            .Where(a => a.CompletionTime != null)
            .Select(a => a.CompletionTime.Value - a.AssignTime)
            .Aggregate(TimeSpan.Zero, (acc, t) => acc + t)
            .ToString(@"hh\:mm") ?? "-";

    public string LastVolunteerName =>
        OriginalCall.Assignments?.LastOrDefault()?.VolunteerName ?? "-";

    public int TotalAssignments => OriginalCall.Assignments?.Count ?? 0;

    public bool CanDelete =>
        OriginalCall.Status == BO.CallStatus.Open &&
        (OriginalCall.Assignments?.Count ?? 0) == 0;

    public bool CanCancelAssignment =>
        OriginalCall.Status == BO.CallStatus.InProgress &&
        (OriginalCall.Assignments?.Any() ?? false);
}
