using DO;

namespace BO;
/// <summary>
// Represents a CallAssignInList with various properties
/// </summary>
/// <param name="VolunteerId">An unique identifier for each volunteer.</param>
/// <param name="VolunteerName">The name of the volunteer.</param>
/// <param name="BeginActionTime">The time the call was assigned to the volunteer.</param>
/// <param name="EndActionTime">The time the call was completed by the volunteer.</param>
/// <param name="ClosureType">T the type of closure for the call it can beCompleted, SelfCancelled,ManagerCancelled, Expired</param>
/// <param name="ToString() ">Return a string representation of the object.</param>
public class CallAssignInList
{
    public int? VolunteerId { get; init; }=null;
    public string? VolunteerName { get; init; } = null;
    public DateTime BeginActionTime { get; init; }
    public DateTime? EndActionTime { get; init; } = null;
    public EndStatus? ClosureType { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
