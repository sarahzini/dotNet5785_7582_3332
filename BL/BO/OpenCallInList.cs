namespace BO;

public class OpenCallInList
{
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public required string CallAddress { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public double VolunteerDistanceToCall { get; set; }
    public override string ToString() => this.ToStringProperty();
}
