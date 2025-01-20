namespace BO;
/// <summary>
/// The enum that are used in different methods
/// </summary> 
public enum Job
{
    Volunteer,
    Manager  
}
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}
public enum SystemType
{
    ICUAmbulance=0,
    RegularAmbulance=1, 
    None,
    All
}
public enum Statuses
{
    Open,
    InAction,
    Closed,
    Expired,
    OpenToRisk,
    InActionToRisk,
    All
}
public enum EndStatus
{
    Completed=1,
    SelfCancelled,
    ManagerCancelled,
    Expired
}
public enum TimeUnit 
{
    Minute=1,
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
    TotalAssignment,
    Status
}

public enum ClosedCallInListField
{
    CallId,
    BeginTime,
    BeginActionTime,
    EndTime,
    TypeOfEnd
}

public enum OpenCallInListField
{
    CallId,
    BeginTime,
    MaxEndTime,
    VolunteerDistanceToCall
}

public enum VolunteerMethods
{
    Login=1,
    GetVolunteersInList,
    GetVolunteerDetails,
    UpdateVolunteer,
    DeleteVolunteer,
    AddVolunteer,
    BackToMainMenu
}

public enum CallMethods
{
    TypeOfCallCounts=1,
    GetSortedCallsInList,
    GetCallDetails,
    UpdateCallDetails,
    DeleteCall,
    AddCall,
    SortClosedCalls,
    SortOpenCalls,
    CompleteCall,
    CancelAssignment,
    AssignCallToVolunteer,
    BackToMainMenu
}

public enum AdminMethods
{
    GetClock=1,
    ForwardClock,
    GetRiskRange,
    SetRiskRange,
    InitializeDB,
    ResetDB,
    BackToMainMenu
}

