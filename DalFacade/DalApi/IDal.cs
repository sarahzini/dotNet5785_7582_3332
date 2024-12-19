namespace DalApi;

/// <summary>
/// Provides access to various data access layers (DAL) for managing volunteers, calls, assignments, and configuration settings.
/// </summary>
public interface IDal
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IConfig Config { get; }
    void ResetDB();

}

