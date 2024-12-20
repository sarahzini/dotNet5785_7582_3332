namespace BO;

/// <summary>
/// Represents a call with various properties
/// </summary>
/// <param name="CallId">Represents a unique identifier for the call, obtained from the Data layer.</param>
/// <param name="TypeOfCall">the type of the call it can be either, ICUAmbulance, or RegularAmbulance,</param>
/// <param name="Description"> The description of the call.</param>
/// <param name="CallAddress">The the address of the call.</param>
/// <param name="CallLatitude">the latitude of the call.</param>
/// <param name="CallLongitude">The longitude of the call.</param>
/// <param name="BeginTime">The time the call was created.</param>
/// <param name="MaxEndTime"></param> 
/// <param name="Status">The status of the call it can be either  Open, InAction,Closed,Expired,OpenToRisk,InActionToRisk</param>
/// <param name="CallAssigns">The list of assignments that have been assigned to this call /param>
/// <param name="ToString() function">It returns a string representation of the object/param>
public class Call
{
    
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public string? Description { get; init; } = null;
    public required string CallAddress { get; set; }
    public double CallLatitude { get; set; }
    public double CallLongitude { get; set; }
    public DateTime BeginTime { get; init; }
    public DateTime? MaxEndTime { get; set; } = null;
    public Statuses Status { get; set; }
    public List<BO.CallAssignInList>? CallAssigns { get; set; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
