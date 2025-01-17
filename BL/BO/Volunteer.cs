namespace BO;

/// <summary>
/// Represents a Volunteer with various properties.
/// </summary>
/// <param name="VolunteerId">The unique identifier for the volunteer.</param>
/// <param name="Name">The name of the volunteer.</param>
/// <param name="PhoneNumber">The phone number of the volunteer.</param>
/// <param name="Email">The email address of the volunteer.</param>
/// <param name="Password">The password for the volunteer's account (optional).</param>
/// <param name="VolunteerAddress">The address of the volunteer (optional).</param>
/// <param name="VolunteerLatitude">The latitude coordinate of the volunteer's location (optional).</param>
/// <param name="VolunteerLongitude">The longitude coordinate of the volunteer's location (optional).</param>
/// <param name="VolunteerJob">The job role of the volunteer, such as Volunteer or Manager.</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="MaxVolunteerDistance">The maximum distance the volunteer is willing to travel (optional).</param>
/// <param name="VolunteerDT">The type of distance measurement for the volunteer, such as AirDistance, WalkingDistance, or DrivingDistance.</param>
/// <param name="CompletedCalls">The number of calls the volunteer has completed.</param>
/// <param name="CancelledCalls">The number of calls the volunteer has cancelled or the manager has cancelled.</param>
/// <param name="ExpiredCalls">The number of calls that have expired for the volunteer.</param>
/// <param name="CurrentCall">The current call the volunteer is handling (optional).</param>
public class Volunteer
{
    public int VolunteerId { get; init; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; } = null; // bonus 
    public string? VolunteerAddress { get; set; } = null;
    public double? VolunteerLatitude { get; set; } = null;
    public double? VolunteerLongitude { get; set; } = null;
    public Job VolunteerJob { get; set; }
    public bool IsActive { get; set; } = false;
    public double? MaxVolunteerDistance { get; set; } = null;
    public DistanceType VolunteerDT { get; set; } 
    public int CompletedCalls { get; init; }
    public int CancelledCalls { get; init; }
    public int ExpiredCalls { get; init; }
    public BO.CallInProgress? CurrentCall { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}

