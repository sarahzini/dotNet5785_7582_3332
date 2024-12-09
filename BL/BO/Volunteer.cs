namespace BO;

public record Volunteer
(
    //The Volunteeer properties
    int VolunteerId,
    string Name,
    string PhoneNumber,
    string Email,
    int CompletedCalls,
    int CancelledCalls,
    int ExpiredCalls,
    string? Password = null, //bonus stage 4
    string? Adress = null,
    double? Latitude = null,
    double? Longitude = null,
    Job MyJob = Job.Volunteer,
    bool IsActive = false,
    double? MaxDistance = null,
    WhichDistance MyWhichDistance = WhichDistance.AirDistance,
    BO.CallInProgress? CurrentCall = null

)
{
    public override string ToString()
    {
        string yesOrNo = IsActive ? "Yes" : "No";

        return $"Id: {VolunteerId}, Name: {Name}, PhoneNumber: {PhoneNumber}, Email: {Email}, Address: {Adress}, Job: {MyJob}, Active: {yesOrNo}," +
            $" Distance: {MaxDistance}, WhichDistance: {MyWhichDistance}";
    }
}

