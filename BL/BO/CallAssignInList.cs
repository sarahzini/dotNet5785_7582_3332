namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }=null;
    public string? VolunteerName { get; init; } = null;
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndActionTime { get; init; } = null;
    public EndStatus? ClosureType { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
