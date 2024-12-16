namespace BO;

public class Call
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public string? Description { get; init; } = null;
    public required string CallAddress { get; set; }
    public double CallLatitude { get; set; }
    public double CallLongitude { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; set; } = null;
    public Statuses Status { get; set; }
    public List<BO.CallAssignInList>? CallAssigns { get; set; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
