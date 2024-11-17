using Dal;
using DalApi;
using DalList;
using DalTest;
using DO;
namespace DaleTest;

internal class Program
{
    private static IAssignment? s_dalAssignement = new AssignmentImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementations(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementations(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

    private enum MainMenuOptions
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

    private enum CrudMenuOptions
    {
        Exit,
        Create,
        Read,
        ReadAll,
        Update,
        Delete,
        DeleteAll
    }

    private static void Main(string[] args)
    {
        try
        {
            while (true)
            {
                DisplayMainMenu();
                var choice = (MainMenuOptions)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case MainMenuOptions.Exit:
                        return;
                    case MainMenuOptions.AssignmentMenu:
                        DisplayCrudMenu(s_dalAssignement);
                        break;
                    case MainMenuOptions.CallMenu:
                        DisplayCrudMenu(s_dalCall);
                        break;
                    case MainMenuOptions.VolunteerMenu:
                        DisplayCrudMenu(s_dalVolunteer);
                        break;
                    case MainMenuOptions.InitializeData:
                        InitializeData();
                        break;
                    case MainMenuOptions.DisplayAllData:
                        DisplayAllData();
                        break;
                    case MainMenuOptions.ConfigMenu:
                        DisplayConfigMenu();
                        break;
                    case MainMenuOptions.DeleteAndReset:
                        DeleteReset();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void DisplayMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Assignment Menu");
        Console.WriteLine("2. Call Menu");
        Console.WriteLine("3. Volunteer Menu");
        Console.WriteLine("4. Config Menu");
        Console.WriteLine("5. Initialize Data");
        Console.WriteLine("6. Display All Data");
    }

    private static void DisplayCrudMenu<T>(T? dal) where T : class
    {
        if (dal == null) return;

        while (true)
        {
            Console.WriteLine("CRUD Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Read All");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Delete All");

            var choice = (CrudMenuOptions)int.Parse(Console.ReadLine() ?? "0");

            try
            {
                switch (choice)
                {
                    case CrudMenuOptions.Exit:
                        return;
                    case CrudMenuOptions.Create:
                        CreateEntity(dal);
                        break;
                    case CrudMenuOptions.Read:
                        ReadEntity(dal);
                        break;
                    case CrudMenuOptions.ReadAll:
                        ReadAllEntities(dal);
                        break;
                    case CrudMenuOptions.Update:
                        UpdateEntity(dal);
                        break;
                    case CrudMenuOptions.Delete:
                        DeleteEntity(dal);
                        break;
                    case CrudMenuOptions.DeleteAll:
                        DeleteAllEntities(dal);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    private static void DisplayConfigMenu()
    {
        while (true)
        {
            Console.WriteLine("Config Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Advance Clock by Minute");
            Console.WriteLine("2. Advance Clock by Hour");
            Console.WriteLine("3. Display Current Clock");
            Console.WriteLine("4. Set New Config Value");
            Console.WriteLine("5. Display Current Config Value");
            Console.WriteLine("6. Reset Config");

            var choice = int.Parse(Console.ReadLine() ?? "0");

            try
            {
                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                        break;
                    case 2:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                        break;
                    case 3:
                        Console.WriteLine(s_dalConfig.Clock);
                        break;
                    case 4:
                        SetNewConfigValue();
                        break;
                    case 5:
                        DisplayCurrentConfigValue();
                        break;
                    case 6:
                        s_dalConfig.Reset();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    private static void InitializeData()
    {
        Initialization.Do(s_dalVolunteer, s_dalAssignement, s_dalCall, s_dalConfig);
    }

    private static void DisplayAllData()
    {
        Console.Write("Volunteers: ");
        ReadAllEntities(s_dalVolunteer);

        Console.Write("Calls: ");
        ReadAllEntities(s_dalCall);

        Console.Write("Assignements: ");
        ReadAllEntities(s_dalAssignement); 
    }
    private static void CreateEntity<T>(T dal) where T : class
    {
        if (dal is IVolunteer volunteerDal)
        {
            // Prompt user for Volunteer details
            Console.WriteLine("Enter Volunteer details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            //helped by IA
            Console.Write("Latitude: ");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : (double?)null;

            //helped by IA
            Console.Write("Longitude: ");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : (double?)null;

            Console.Write("Job (Director or Volunteer): ");
            Job job = Enum.TryParse(Console.ReadLine(), out Job parsedJob) ? parsedJob : Job.Volunteer;

            Console.Write("Active (true/false): ");
            bool active = bool.TryParse(Console.ReadLine(), out bool isActive) && isActive;

            Console.Write("Distance: ");
            double? distance = double.TryParse(Console.ReadLine(), out double dist) ? dist : (double?)null;

            Console.Write("Which Distance: ");
            WhichDistance whichDistance = Enum.TryParse(Console.ReadLine(), out WhichDistance parsedDistance) ? parsedDistance : WhichDistance.AirDistance;

            // Create new Volunteer object
            Volunteer newVolunteer = new Volunteer(id, name, phoneNumber, email, address, latitude, longitude, job, active, distance, whichDistance);

            // Add Volunteer to DAL
            volunteerDal.Create(newVolunteer);

        }
        else if (dal is ICall callDal)
        {
            // Prompt user for Call details
            Console.WriteLine("Enter Call details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Address: ");
            string address = Console.ReadLine() ?? "";

            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("DateTime (yyyy-MM-dd HH:mm:ss): ");
            DateTime dateTime = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("Choice (SystemType): ");
            SystemType choice = Enum.TryParse(Console.ReadLine(), out SystemType parsedChoice) ? parsedChoice : SystemType.ICUAmbulance;

            Console.Write("Description (optional): ");
            string? description = Console.ReadLine();

            Console.Write("EndDateTime (optional, yyyy-MM-dd HH:mm:ss): ");
            DateTime? endDateTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndDateTime) ? parsedEndDateTime : (DateTime?)null;

            // Create new Call object
            Call newCall = new Call(id, address, latitude, longitude, dateTime, choice, description, endDateTime);

            // Add Call to DAL
            callDal.Create(newCall);
        }
        else if (dal is IAssignment assignmentDal)
        {
            // Prompt user for Assignment details
            Console.WriteLine("Enter Assignment details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Call ID: ");
            int callId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Begin DateTime (yyyy-MM-dd HH:mm:ss): ");
            DateTime begin = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("End DateTime (optional, yyyy-MM-dd HH:mm:ss): ");
            DateTime? end = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEnd) ? parsedEnd : (DateTime?)null;

            Console.Write("End Status (Completed, Canceled, InProgress): ");
            EndStatus endStatus = Enum.TryParse(Console.ReadLine(), out EndStatus parsedEndStatus) ? parsedEndStatus : EndStatus.Completed;

            // Create new Assignment object
            Assignment newAssignment = new Assignment(id, callId, volunteerId, begin, end, endStatus);

            // Add Assignment to DAL
            assignmentDal.Create(newAssignment);

        }
    }

    private static void ReadEntity<T>(T dal) 
    {
        Console.Write("Please enter ID: ");
        int id = int.Parse(Console.ReadLine() ?? "0");

        if (dal is IVolunteer volunteerDal)
        {
            DO.Volunteer? v = volunteerDal.Read(id);
            if (v != null)
                Console.WriteLine(v);

        }
        else if (dal is ICall callDal)
        {
            DO.Call? c = callDal.Read(id);
            if (c != null)
                Console.WriteLine(c);

        }
        else if (dal is IAssignment assignmentDal)
        {
            DO.Assignment? a = assignmentDal.Read(id);
            if (a != null)
                Console.WriteLine(a);
        }
           

    }

    private static void ReadAllEntities<T>(T dal) 
    {
        if (dal is IVolunteer volunteerDal)
        {
            List<Volunteer> volunteers = volunteerDal.ReadAll();
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine(volunteer);
            }



        }
        else if (dal is ICall callDal)
        {
            List<Call> calls = callDal.ReadAll();
            foreach (var call in calls)
            {
                Console.WriteLine(call);
            }

        }
        else if (dal is IAssignment assignmentDal)
        {
            List<Assignment> assignments = assignmentDal.ReadAll();
            foreach (var assignment in assignments)
            {
                Console.WriteLine(assignment);
            }
        }
    }

    private static void UpdateEntity<T>(T dal)
    {
        if (dal is IVolunteer volunteerDal)
        {
            // Prompt user for Volunteer details
            Console.WriteLine("Enter Volunteer details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            //helped by IA
            Console.Write("Latitude: ");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : (double?)null;

            //helped by IA
            Console.Write("Longitude: ");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : (double?)null;

            Console.Write("Job (Director or Volunteer): ");
            Job job = Enum.TryParse(Console.ReadLine(), out Job parsedJob) ? parsedJob : Job.Volunteer;

            Console.Write("Active (true/false): ");
            bool active = bool.TryParse(Console.ReadLine(), out bool isActive) && isActive;

            Console.Write("Distance: ");
            double? distance = double.TryParse(Console.ReadLine(), out double dist) ? dist : (double?)null;

            Console.Write("Which Distance: ");
            WhichDistance whichDistance = Enum.TryParse(Console.ReadLine(), out WhichDistance parsedDistance) ? parsedDistance : WhichDistance.AirDistance;

            // Create new Volunteer object
            Volunteer newVolunteer = new Volunteer(id, name, phoneNumber, email, address, latitude, longitude, job, active, distance, whichDistance);

            // Add Volunteer to DAL
            volunteerDal.Update(newVolunteer);

        }
        else if (dal is ICall callDal)
        {
            // Prompt user for Call details
            Console.WriteLine("Enter Call details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Address: ");
            string address = Console.ReadLine() ?? "";

            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("DateTime (yyyy-MM-dd HH:mm:ss): ");
            DateTime dateTime = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("Choice (SystemType): ");
            SystemType choice = Enum.TryParse(Console.ReadLine(), out SystemType parsedChoice) ? parsedChoice : SystemType.ICUAmbulance;

            Console.Write("Description (optional): ");
            string? description = Console.ReadLine();

            Console.Write("EndDateTime (optional, yyyy-MM-dd HH:mm:ss): ");
            DateTime? endDateTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndDateTime) ? parsedEndDateTime : (DateTime?)null;

            // Create new Call object
            Call newCall = new Call(id, address, latitude, longitude, dateTime, choice, description, endDateTime);

            // Add Call to DAL
            callDal.Update(newCall);
        }
        else if (dal is IAssignment assignmentDal)
        {
            // Prompt user for Assignment details
            Console.WriteLine("Enter Assignment details:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Call ID: ");
            int callId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Begin DateTime (yyyy-MM-dd HH:mm:ss): ");
            DateTime begin = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("End DateTime (optional, yyyy-MM-dd HH:mm:ss): ");
            DateTime? end = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEnd) ? parsedEnd : (DateTime?)null;

            Console.Write("End Status (Completed, Canceled, InProgress): ");
            EndStatus endStatus = Enum.TryParse(Console.ReadLine(), out EndStatus parsedEndStatus) ? parsedEndStatus : EndStatus.Completed;

            // Create new Assignment object
            Assignment newAssignment = new Assignment(id, callId, volunteerId, begin, end, endStatus);

            // Add Assignment to DAL
            assignmentDal.Update(newAssignment);

        }
    }

    private static void DeleteEntity<T>(T dal) 
    {
        Console.Write("Please enter ID: ");
        int id = int.Parse(Console.ReadLine() ?? "0");

        if (dal is IVolunteer volunteerDal)
            volunteerDal.Delete(id);
        else if (dal is ICall callDal)
            callDal.Delete(id);
        else if (dal is IAssignment assignmentDal)
            assignmentDal.Delete(id);
    }

    private static void DeleteAllEntities<T>(T dal) 
    {
        if (dal is IVolunteer volunteerDal)
            volunteerDal.DeleteAll();
        else if (dal is ICall callDal)
            callDal.DeleteAll();
        else if (dal is IAssignment assignmentDal)
            assignmentDal.DeleteAll();
    }

    private static void DeleteReset()
    {
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignement.DeleteAll();
        s_dalConfig.Reset();

    }

    private static void SetNewConfigValue()
    {
        Console.Write("Please enter new value for a config variable:risk range ");
        int value = int.Parse(Console.ReadLine() ?? "0");
        s_dalConfig.RiskRange = TimeSpan.FromMinutes(value);
    }

    private static void DisplayCurrentConfigValue()
    {
        Console.WriteLine(s_dalConfig.RiskRange);
    }
}
