using DalApi;
using System.Diagnostics;
namespace Dal;

/// <summary>
/// DalXml class provides implementations for managing Volunteers, Calls, Assignments, and Configurations using XML storage.
/// </summary>
sealed internal class DalXml : IDal
{
    // Singleton instance
    public static IDal Instance { get; } = new DalXml();
    //Empty constructor
    private DalXml() { }
    // Gets the implementation for managing Volunteer records.
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    // Gets the implementation for managing Call records.
    public ICall Call { get; } = new CallImplementation();

    // Gets the implementation for managing Assignment records
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    //Gets the implementation for managing configuration settings.
    public IConfig config { get; } = new ConfigImplementation();

    // Resets the database by deleting all Volunteer, Call, and Assignment records, and resetting the configuration.
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
