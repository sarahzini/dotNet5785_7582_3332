namespace BO;

public class ClosedCallInList
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public required string CallAddress { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndEndActionTime { get; init; } = null;
    public EndStatus? TypeOfEnd { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);

}
