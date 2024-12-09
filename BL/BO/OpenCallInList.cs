namespace BO;

public class OpenCallInList
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public string? Description { get; init; } = null;
    public required string CallAddress { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public double VolunteerDistanceToCall { get; init; }
    public override string ToString() => this.ToStringProperty();
}
