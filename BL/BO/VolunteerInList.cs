namespace BO;
public class VolunteerInList
{
    public int VolunteerId { get; init; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public int CompletedCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int ExpiredCalls { get; set; }
    public int? ActualCallId { get; set; } = null;
    public SystemType TypeOfCall { get; set; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);

}
