namespace BO;

public class Call
{
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public string? CallAddress { get; set; } = null;
    public double? CallLatitude { get; set; } = null;
    public double? CallLongitude { get; set; } = null;
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public EndStatus ClosureType { get; set; }
    public List<BO.CallAssignInList>? CallAssigns { get; set; } = null;
    public override string ToString() => this.ToStringProperty();
}
