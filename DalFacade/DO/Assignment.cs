namespace DO;

/// <summary>
/// Represents an assignment of a volunteer to a call with various properties such as ID, call ID, volunteer ID, and timestamps.
/// </summary>
/// <param name="Id">Represents a unique identifier for the assignment, obtained from the configuration entity.</param>
/// <param name="CallId">Represents the unique identifier of the call that the volunteer chose to handle.</param>
/// <param name="VolunteerId">Represents the ID of the volunteer who chose to handle the call.</param>
/// <param name="Begin">The date and time when the call was first taken by the volunteer.</param>
/// <param name="End">The actual date and time when the volunteer finished handling the call, can be null if the call is still in progress.</param>
/// <param name="MyEndStatus">The manner in which the handling of the call was concluded (e.g., Completed, SelfCancelled, DirectorCancelled, Expired).</param>
public record Assignment
(
    //The assignment properties
    int Id,
    int CallId,
    int VolunteerId,
    DateTime Begin,
    DateTime? End=null,
    EndStatus MyEndStatus= EndStatus.Completed

)
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { } //empty ctor for stage 3
}
