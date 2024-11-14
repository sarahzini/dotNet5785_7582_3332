namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="PhoneNumber"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="Adress"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="MyJob"></param>
/// <param name="active"></param>
/// <param name="distance"></param>
/// <param name="MyWhichDistance"></param>
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
