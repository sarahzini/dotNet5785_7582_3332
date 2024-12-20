namespace BO;

// <summary>
/// Represents a ClosedCallInList with various properties.
/// </summary>
/// <param name="CallId">The unique identifier for the call.</param>
/// <param name="TypeOfCall">The type of the call, such as ICU Ambulance or Regular Ambulance.</param>
/// <param name="CallAddress">The address where the call is located.</param>
/// <param name="BeginTime">The time the call was initiated.</param>
/// <param name="BeginActionTime">The time the action on the call began.</param>
/// <param name="EndActionTime">The time the action on the call ended.</param>
/// <param name="TypeOfEnd">The type of end status for the call, such as Completed, SelfCancelled, ManagerCancelled, or Expired.</param>
public class ClosedCallInList
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; init; }
    public required string CallAddress { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndActionTime { get; init; } = null;
    public EndStatus? TypeOfEnd { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);

}
