namespace BO;

public class ClosedCallInList
{
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public required string CallAddress { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndTime { get; init; } = null;
    public EndStatus TypeOfEnd { get; set; }
    public override string ToString() => this.ToStringProperty();

}
