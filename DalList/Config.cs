
using System.Security.Cryptography;

namespace Dal;

internal static class Config
{
    //The minimum ID that can be assigned to a new object.
    internal const int MinCallId = 100000000;
    //The _CallId will be assigned with the value of MinCallId.
    private static int _CallId = MinCallId;
    //Each time that we will get the _CallId value, it will return it and then increment it.
    internal static int _nextCallId { get => _CallId++; }

    //The minimum assignment ID that can be assigned to a new object.
    internal const int MinAssignmentId = 1000;
    //The _AssignmentId will be assigned with the value of MinAssignmentId.
    private static int _AssignmentId = MinAssignmentId;
    //Each time that we will get the _AssignmentId value, it will return it and then increment it.
    internal static int _nextAssignmentId { get => _AssignmentId++; }


    /// <summary>
    /// Represents the global system clock for the application.
    /// Initialized to the current date and time at the moment the class is loaded.
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;


    // Define a risk range of 90 minutes
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Resets the configuration parameters to their initial values.
    /// This includes resetting the next course ID to the starting ID
    /// and synchronizing the global system clock to the current date and time.
    /// </summary>
    internal static void Reset()
    {
        _AssignmentId = MinAssignmentId;
        _CallId = MinCallId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromMinutes(30);
    }

    

}
