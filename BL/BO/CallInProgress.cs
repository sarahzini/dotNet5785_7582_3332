
namespace BO;

public  class CallInProgress
{
    //The CallInProgress properties
    public int AssignId { get; set; }
    public int CallId { get; set; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public required string CallAddress { get; set; }
    public DateTime BeginTime { get; set; }
    public DateTime? MaxEndTime { get; set; } = null;
    public DateTime BeginActionTime { get; set; }
    public double VolunteerDistanceToCall { get; set; }
    public EndStatus ClosureType { get; set; }
    public override string ToString() => this.ToStringProperty();


}
