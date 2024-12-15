namespace DO;

//The enum that are used in different methods 
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
    RegularAmbulance
}
public enum EndStatus
{
    Completed,
    SelfCancelled, 
    ManagerCancelled, 
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
