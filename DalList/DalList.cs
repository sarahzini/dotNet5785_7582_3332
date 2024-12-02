namespace Dal;
using DalApi;


/// <summary>
/// Represents the data access layer (DAL) implementation using lists.
/// </summary>
sealed internal class DalList : IDal
{
    // Singleton instance
    public static IDal Instance { get; } = new DalList();
    //Empty constructor
    private DalList() { }
    // Properties to access different data entities
    public IVolunteer Volunteer { get; } = new VolunteerImplementations();
    public ICall Call { get; } = new CallImplementations();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig config { get; } = new ConfigImplementation();

    // Resets the database by deleting all records from each entity and also resetting the configuration.
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
    
}

