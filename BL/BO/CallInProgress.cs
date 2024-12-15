
namespace BO;

public  class CallInProgress
{
    //The CallInProgress properties
    public int AssignId { get; init; }
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public required string CallAddress { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; init; } = null;
    public DateTime BeginActionTime { get; init; }
    public double VolunteerDistanceToCall { get; init; }
    public Statuses ClosureType { get; init; }
    public override string ToString() => Helpers.Tools.ToStringProperty(this);


}
