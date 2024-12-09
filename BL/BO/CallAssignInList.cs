namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }=null;
    public string? VolunteerName { get; set; } = null;
    public DateTime BeginActionTime { get; set; }
    public DateTime? EndTime { get; set; } = null;
    public EndStatus? ClosureType { get; set; } = null;
    public override string ToString() => this.ToStringProperty();
}
