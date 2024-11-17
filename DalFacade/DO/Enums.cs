
namespace DO;

//The enum that are used in different methods 
public enum Job { Volunteer, Director }
public enum WhichDistance { AirDistance, WalkingDistance, DrivingDistance }
public enum SystemType { ICUAmbulance, RegularAmbulance }
public enum EndStatus { Completed, SelfCancelled, DirectorCancelled, Expired }
//Enum of the different options for the user to choose from
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
//Enum representing  the crud options ( a submenu of the mainMenuOptions) if the user chose AssignmentMenu,
// CallMenu or the VolunteerMenu he will have different options to choose from.
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
internal class Enums
{
}
