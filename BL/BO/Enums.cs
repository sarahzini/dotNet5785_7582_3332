namespace BO;
/// <summary>
/// The enum that are used in different methods
/// </summary> 
public enum Job
{
    Volunteer,
    Director
}
public enum WhichDistance
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}
public enum SystemType
{
    ICUAmbulance,
    RegularAmbulance,
    None
}
public enum EndStatus
{
    Completed,
    SelfCancelled,
    DirectorCancelled,
    Expired
}
public enum TimeUnit 
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}

public enum VolunteerFieldSort
{
    VolunteerId,
    Name,
    CompletedCalls,
    CancelledCalls,
    ExpiredCalls
}

public enum CallInListField
{
    CallId,
    BeginTime,
    NameLastVolunteer,
    ExecutedTime,
    TotalAssignment
}

public enum ClosedCallInListField
{
    CallId,
    CallAddress,
    BeginTime,
    RangeTimeToEnd,
    NameLastVolunteer,
    ExecutedTime,
    ClosureType,
    TotalAssignment
}

public enum OpenCallInListField
{
    CallId,
    TypeOfCall,
    CallAddress,
    BeginTime,
    RangeTimeToEnd,
    NameLastVolunteer,
    ExecutedTime,
    ClosureType,
    TotalAssignment
}

