using System.Net;

namespace DO;

/// <summary>
/// Represents a volunteer with various properties such as ID, name, contact information, and location details.
/// </summary>
/// <param name="Id">Represents a unique and valid ID for the volunteer.</param>
/// <param name="Name">The full name (first and last) of the volunteer.</param>
/// <param name="PhoneNumber">Represents a valid mobile phone number (10 digits, starting with 0).</param>
/// <param name="Email">Represents a valid email address format.</param>
/// <param name="Adress">The current full and valid address of the volunteer, which can be null.</param>
/// <param name="Latitude">Latitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="Longitude">Longitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="MyJob">The role of the volunteer, either "Manager" or "Volunteer".</param>
/// <param name="active">Indicates whether the volunteer is active or inactive (retired from the organization).</param>
/// <param name="distance">The maximum distance for receiving calls, can be null indicating no distance limit.</param>
/// <param name="MyWhichDistance">The type of distance (air, walking, driving) used for calculating distances.</param>
public record Volunteer
(
    //The Volunteeer properties
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string? password=null, //addition of stage 1
    string? Adress=null,
    double? Latitude=null,
    double? Longitude=null,
    Job MyJob=Job.Volunteer,
    bool active=false,
    double? distance=null,
    WhichDistance MyWhichDistance= WhichDistance.AirDistance
)
{
    public override string ToString()
    {
        string yesOrNo = active ? "Yes" : "No";

        return $"Id: {Id}, Name: {Name}, PhoneNumber: {PhoneNumber}, Email: {Email}, Address: {Adress}, Job: {MyJob}, Active: {yesOrNo}," +
            $" Distance: {distance}, WhichDistance: {MyWhichDistance}";
    }
    public Volunteer() : this(0, "", "", "") {} //empty ctor for stage 3
}
