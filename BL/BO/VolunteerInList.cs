namespace BO;

/// <summary>
/// Represents a VolunteerInList with various properties.
/// </summary>
/// <param name="VolunteerId">The unique identifier for the volunteer.</param>
/// <param name="Name">The name of the volunteer.</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="CompletedCalls">The number of calls the volunteer has completed.</param>
/// <param name="CanceledCalls">The number of calls the volunteer has canceled.</param>
/// <param name="ExpiredCalls">The number of calls that have expired for the volunteer.</param>
/// <param name="ActualCallId">The identifier of the current call the volunteer is handling (optional).</param>
/// <param name="TypeOfCall">The type of the current call, such as ICU Ambulance or Regular Ambulance.</param>
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

