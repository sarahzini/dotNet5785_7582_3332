namespace BO;

public class Volunteer
{ 
    //The Volunteeer properties
    public int VolunteerId{ get; init; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; } = null; //bonus 
    public string? VolunteerAddress { get; set; } = null;
    public double? VolunteerLatitude { get; set; } = null;
    public double? VolunteerLongitude { get; set; } = null;
    public Job VolunteerJob { get; set; }
    public bool IsActive { get; set; } = false;
    public double? MaxVolunteerDistance { get; set; } = null;
    public DistanceType VolunteerDT { get; set; } //bonus : we can set it
    public int CompletedCalls { get; init; }
    public int CancelledCalls { get; init; }
    public int ExpiredCalls { get; init; }
    public BO.CallInProgress? CurrentCall { get; init; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);

}


