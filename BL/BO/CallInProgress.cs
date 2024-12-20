namespace BO;

/// <summary>
/// Represents a CallInProgress with various properties.
/// </summary>
/// <param name="AssignId">The unique identifier for the assignment.</param>
/// <param name="CallId">The unique identifier for the call.</param>
/// <param name="TypeOfCall">The type of the call, such as ICU Ambulance or Regular Ambulance.</param>
/// <param name="Description">A description of the call.</param>
/// <param name="CallAddress">The address where the call is located.</param>
/// <param name="BeginTime">The time the call was initiated.</param>
/// <param name="MaxEndTime">The maximum time by which the call should end.</param>
/// <param name="BeginActionTime">The time the action on the call began.</param>
/// <param name="VolunteerDistanceToCall">The distance of the volunteer to the call location.</param>
/// <param name="Status">The status of the call, such as Open, InAction, Closed, etc.</param>
public class CallInProgress
{
    //The CallInProgress properties
    public int AssignId { get; init; }
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public required string CallAddress { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public DateTime BeginActionTime { get; init; }
    public double VolunteerDistanceToCall { get; init; }
    public Statuses Status { get; init; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);


}
