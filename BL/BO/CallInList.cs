namespace BO;

/// <summary>
/// Represents a CallInList with various properties.
/// </summary>
/// <param name="AssignId">The unique identifier for the assignment.</param>
/// <param name="CallId">The unique identifier for the call.</param>
/// <param name="TypeOfCall">The type of the call, such as ICU Ambulance or Regular Ambulance.</param>
/// <param name="BeginTime">The time the call was initiated.</param>
/// <param name="RangeTimeToEnd">The remaining time until the call is expected to end.</param>
/// <param name="NameLastVolunteer">The name of the last volunteer who handled the call.</param>
/// <param name="ExecutedTime">The time taken to execute the call.</param>
/// <param name="Status">The status of the call, such as Open, InAction, Closed, ect.</param>
/// <param name="TotalAssignment">The total number of assignments for the call.</param>
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
