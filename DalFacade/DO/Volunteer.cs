namespace DO;
public record Volunteer
(
    //The Volunteeer properties
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string? Adress=null,
    double? Latitude=null,
    double? Longitude=null,
    Job MyJob=Job.Volunteer,
    bool active=false,
    double? distance=null,
    WhichDistance MyWhichDistance= WhichDistance.AirDistance
)
{
    public Volunteer() : this(0, "", "", "") {} //empty ctor for stage 3
}
