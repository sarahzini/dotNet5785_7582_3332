namespace BO;
/// <summary>
/// The enum that are used in different methods
/// </summary> 
public enum Job
{
    Volunteer,
    Manager  
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
public enum Statuses
{
    Open,
    InAction,
    Closed,
    Expired,
    OpenToRisk,
    InActionToRisk
}
public enum EndStatus
{
    Completed,
    SelfCancelled,
    ManagerCancelled,
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

public enum VolunteerInListFieldSort
{
    VolunteerId,
    Name,
    CompletedCalls,
    CancelledCalls,
    ExpiredCalls,
    ActualCallId
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

