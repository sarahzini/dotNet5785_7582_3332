namespace BO;

public class CallInList
{
    public int? AssignId { get; init; } = null;
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public DateTime BeginTime { get; init; }
    public TimeSpan? RangeTimeToEnd { get; init; } = null;
    public string? NameLastVolunteer { get; set; } = null;
    public TimeSpan? ExecutedTime { get; init; } = null;
    public EndStatus ClosureType { get; set; }
    public int TotalAssignment { get; set; }
    public override string ToString() => this.ToStringProperty();
}
