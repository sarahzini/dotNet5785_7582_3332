namespace BO;

//The enum that are used in different methods 
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
    RegularAmbulance
}
public enum EndStatus
{
    Completed,
    SelfCancelled,
    DirectorCancelled,
    Expired
}
public enum MainMenuOptions
{
    Exit,
    AssignmentMenu,
    CallMenu,
    VolunteerMenu,
    InitializeData,
    DisplayAllData,
    ConfigMenu,
    DeleteAndReset
}

public enum CrudMenuOptions
{
    Exit,
    Create,
    Read,
    ReadAll,
    Update,
    Delete,
    DeleteAll
}
