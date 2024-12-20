namespace BO;

/// <summary>
/// Represents an OpenCallInList with various properties.
/// </summary>
/// <param name="CallId">The unique identifier for the call.</param>
/// <param name="TypeOfCall">The type of the call, such as ICU Ambulance or Regular Ambulance.</param>
/// <param name="Description">A description of the call.</param>
/// <param name="CallAddress">The address where the call is located.</param>
/// <param name="BeginTime">The time the call was initiated.</param>
/// <param name="MaxEndTime">The maximum time by which the call should end.</param>
/// <param name="VolunteerDistanceToCall">The distance of the volunteer to the call location.</param>
public class OpenCallInList
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public string? Description { get; init; } = null;
    public required string CallAddress { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public double VolunteerDistanceToCall { get; init; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
