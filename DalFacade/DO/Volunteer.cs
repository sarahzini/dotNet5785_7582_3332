using System.Net;

namespace DO;

/// <summary>
/// Represents a volunteer with various properties such as ID, name, contact information, and location details.
/// </summary>
/// <param name="VolunteerId">Represents a unique and valid ID for the volunteer.</param>
/// <param name="Name">The full name (first and last) of the volunteer.</param>
/// <param name="PhoneNumber">Represents a valid mobile phone number (10 digits).</param>
/// <param name="Email">Represents a valid email address format.</param>
/// <param name="Address">The current full and valid address of the volunteer, which can be null.</param>
/// <param name="Latitude">Latitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="Longitude">Longitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="MyJob">The role of the volunteer, either "Manager" or "Volunteer".</param>
/// <param name="IsActive">Indicates whether the volunteer is IsActive or inIsActive (retired from the organization).</param>
/// <param name="MaxDistance">The maximum MaxDistance for receiving calls, can be null indicating no MaxDistance limit.</param>
/// <param name="MyDistanceType">The type of distance (air, walking, driving) used for calculating different distances.</param>
public record Volunteer
(
    //The Volunteeer properties
    int VolunteerId,
    string Name,
    string PhoneNumber,
    string Email,
    string? Password=null, //bonus stage 1
    string? Address=null,
    double? Latitude=null,
    double? Longitude=null,
    Job MyJob=Job.Volunteer,
    bool IsActive=false,
    double? MaxDistance=null,
    DistanceType MyDistanceType= DistanceType.AirDistance
)
{
    public override string ToString()
    {
        string yesOrNo = IsActive ? "Yes" : "No";

        return $"Id: {VolunteerId}, Name: {Name}, Phone Number: {PhoneNumber}, Email: {Email}, Address: {Address}, Job: {MyJob}, IsActive: {yesOrNo}," +
            $" Max Distance to the Call: {MaxDistance}, Distance Type: {MyDistanceType}";
    }
    public Volunteer() : this(0, "", "", "") {} //empty ctor for stage 3
}
