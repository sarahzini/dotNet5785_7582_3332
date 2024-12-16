namespace BO;
public class VolunteerInList
{
    public int VolunteerId { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
    public int CompletedCalls { get; init; }
    public int CanceledCalls { get; init; }
    public int ExpiredCalls { get; init; }
    public int? ActualCallId { get; init; } = null;
    public SystemType TypeOfCall { get; init; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);

}
