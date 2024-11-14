namespace DO;
public record Volunteer
(
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string? Password=null, //faire quelque chose
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
