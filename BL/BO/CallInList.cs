namespace BO;

public class CallInList
{
    public int? AssignId { get; init; } = null;
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public DateTime BeginTime { get; init; }
    public TimeSpan? RangeTimeToEnd { get; init; } = null;
    public string? NameLastVolunteer { get; init; } = null;
    public TimeSpan? ExecutedTime { get; init; } = null;
    public Statuses Status { get; init; }
    public int TotalAssignment { get; init; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
