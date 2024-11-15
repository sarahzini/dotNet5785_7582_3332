namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    // Fields for managing various entities and configurations in the Data Access Layer (DAL).
    private static IVolunteer? s_dalVolunteer;
    private static IAssignment? s_dalAssignement;
    private static ICall? s_dalCall;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();

    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    private static void createVolunteers()
    {
        string[] names = { "John Doe", "Jane Smith", "Alice Johnson", "Bob Brown" };
        string[] phoneNumbers = { "123-456-7890", "234-567-8901", "345-678-9012", "456-789-0123" };
        string[] emails = { "john@example.com", "jane@example.com", "alice@example.com", "bob@example.com" };

        for (int i = 0; i < names.Length; i++)
        {
            Volunteer volunteer = new(i + 1, names[i], phoneNumbers[i], emails[i]);
            if (s_dalVolunteer?.Read(volunteer.Id) == null)
            {
                s_dalVolunteer?.Create(volunteer);
            }
        }
    }

    private static void createAssignments()
    {
        for (int i = 0; i < 10; i++)
        {
            Assignment assignment = new(i + 1, s_rand.Next(1, 5), s_rand.Next(1, 5), DateTime.Now);
            if (s_dalAssignement?.Read(assignment.Id) == null)
            {
                s_dalAssignement?.Create(assignment);
            }
        }
    }

    private static void createCalls()
    {
        string[] addresses = { "123 Main St", "456 Elm St", "789 Oak St", "101 Pine St" };
        SystemType[] choices = { SystemType.Food, SystemType.Medical, SystemType.Transport, SystemType.Other };

        for (int i = 0; i < addresses.Length; i++)
        {
            Call call = new(i + 1, addresses[i], s_rand.NextDouble() * 90, s_rand.NextDouble() * 180, DateTime.Now, choices[i]);
            if (s_dalCall?.Read(call.Id) == null)
            {
                s_dalCall?.Create(call);
            }
        }
    }
}


