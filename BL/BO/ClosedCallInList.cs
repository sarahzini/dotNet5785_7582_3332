namespace BO;

public class ClosedCallInList
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public required string CallAddress { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndTime { get; set; } = null;
    public EndStatus? TypeOfEnd { get; set; } = null;
    public override string ToString() => this.ToStringProperty();

}
