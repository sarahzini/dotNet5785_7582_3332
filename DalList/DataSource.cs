using static System.Net.Mime.MediaTypeNames;
using System;
namespace Dal;

/// <summary>
/// These lists serve as in-memory data storage for managing assignments, calls, and volunteers within the application.
/// </summary>
internal static class DataSource
{
    internal static List<DO.Assignment> Assignments { get; } = new();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Volunteer> Volunteers { get; } = new();
}
