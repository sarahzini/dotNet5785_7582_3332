using Dal;
using DalApi;
using DalTest;
using DO;
using System.Buffers.Text;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace DaleTest;

/// <summary>
/// The Main method  operates in a continuous loop, displaying a main menu to the user and executing actions based on the user's choice. Here's a step-by-step explanation:
//  Based on the user's choice, a corresponding action is executed:
//•	Exit: Terminates the application.
//•	AssignmentMenu, CallMenu, VolunteerMenu: Displays a CRUD menu for managing assignments, calls, or volunteers.
//•	InitializeData: Initializes data using the provided data access layer(DAL) objects.
//•	DisplayAllData: Displays all data for volunteers, calls, and assignments.
//•	ConfigMenu: Displays the Configuration menu for managing application settings.
//•	DeleteAndReset: Deletes all data and resets the Configuration.
//
//Exception Handling: Any exceptions that occur are caught and an error message is displayed.
///// </summary>
internal class Program
{
    //A static method that initializes the data using the provided DAL objects.
    //static readonly IDal s_dal = new DalList(); //stage 2

    // Initializes a static readonly instance of DalXml to manage data access operations.
    // static readonly IDal s_dal = new DalXml(); //stage 3

    private static void Main(string[] args)
    {
        Console.WriteLine("Welcome in the MDA volunteers system !");
        try
        {
            while (true)
            {
                // Display the main menu to the user
                DisplayMainMenu();
                // Executing the corresponding action based on the user's choice
                var choice = (MainMenuOptions)int.Parse(Console.ReadLine() ?? "0");
                
                //Depending on the user's choice it will handle it 
                switch (choice)
                {
                    //Exiting the programm
                    case MainMenuOptions.Exit:
                        return;
                    case MainMenuOptions.AssignmentMenu:
                        // Displaying the  CRUD menu for assignments
                        DisplayCrudMenu(s_dal!.Assignment);
                        break;
                    case MainMenuOptions.CallMenu:
                        // Displaying the CRUD menu for calls
                        DisplayCrudMenu(s_dal!.Call);
                        break;
                    case MainMenuOptions.VolunteerMenu:
                        // Displaying the CRUD menu for volunteers
                        DisplayCrudMenu(s_dal!.Volunteer);
                        break;
                    case MainMenuOptions.InitializeData:
                        // Initializing data
                        InitializeData();
                        break;
                    case MainMenuOptions.DisplayAllData:
                        // Displaying all the  data
                        DisplayAllData();
                        break;
                    case MainMenuOptions.ConfigMenu:
                        // Displaying the Configuration menu
                        DisplayConfigMenu();
                        break;
                    case MainMenuOptions.DeleteAndReset:
                        // Deleting  and reset data
                        DeleteReset();
                        break;
                }
            }
        }
        // Handle any exceptions that occur and display an error message
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
   
    //Displaying to the user the menu
    private static void DisplayMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Assignment Menu");
        Console.WriteLine("2. Call Menu");
        Console.WriteLine("3. Volunteer Menu");
        Console.WriteLine("4. Initialize Data");
        Console.WriteLine("5. Display All Data");
        Console.WriteLine("6. Config Menu");
        Console.WriteLine("7. Delete And Reset\n");
        Console.WriteLine("Please Choose an option:");
    }

    //Displaying to the user the CRUD menu (Create,Read,RealAll,Update,Delete and DeleteAll) 
    //All these functions are placed in the end of the file
    private static void DisplayCrudMenu<T>(T? dal) where T : class
    {
        // Checks if the data access layer object is null
        if (dal == null) return;

        while (true)
        {
            //Displaying the choice from the CRUD menu
            Console.WriteLine("CRUD Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Read All");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Delete All\n");
            Console.WriteLine("Please Choose an option:");
            //Handling the user input 
            var choice = (CrudMenuOptions)int.Parse(Console.ReadLine() ?? "0");

            try
            {
                // Executing the corresponding action based on the user's choice
                switch (choice)
                {
                    case CrudMenuOptions.Exit:
                        //Exiting the CRUD menu
                        return;
                    case CrudMenuOptions.Create:
                        // Creating a new entity
                        CreateEntity(dal);
                        break;
                    case CrudMenuOptions.Read:
                        //Reading an entity
                        ReadEntity(dal);
                        break;
                    case CrudMenuOptions.ReadAll:
                        // Reading all entities
                        ReadAllEntities(dal);
                        break;
                    case CrudMenuOptions.Update:
                        // Updating an existing entity
                        UpdateEntity(dal);
                        break;
                    case CrudMenuOptions.Delete:
                        // Deleting an entity 
                        DeleteEntity(dal);
                        break;
                    case CrudMenuOptions.DeleteAll:
                        // Deleting all entities
                        DeleteAllEntities(dal);
                        break;
                }
            }
            // Handling any exceptions that occur and display an error message
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // Initializing the data using the provided DAL object.
    private static void InitializeData() 
       // => Initialization.Do(s_dal); //stage 2
        =>Initialization.Do(); //stage 4
    //Displaying all the data (Volunteers, Calls and Assignments)
    private static void DisplayAllData()
    {
        // Displaying all the volunteers
        Console.Write("Volunteers: \n");
        ReadAllEntities(s_dal!.Volunteer);
        // Displaying all the calls
        Console.Write("Calls: \n");
        ReadAllEntities(s_dal!.Call);
        // Displaying all the assignments
        Console.Write("Assignements: \n");
        ReadAllEntities(s_dal!.Assignment);
    }

    // Displaying the Configuration menu for managing application settings
    private static void DisplayConfigMenu()
    {
        while (true)
        {
            // Displaying the Configuration menu options
            Console.WriteLine("Config Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Advance Clock by a Minute");
            Console.WriteLine("2. Advance Clock by a Hour");
            Console.WriteLine("3. Display Current Clock");
            Console.WriteLine("4. Set New Config Value");
            Console.WriteLine("5. Display Current Config Value");
            Console.WriteLine("6. Reset Config\n");
            Console.WriteLine("Please Choose an option:");
            // Executing the corresponding action based on the user's choice
            var choice = int.Parse(Console.ReadLine() ?? "0");

            try
            {
                switch (choice)
                {
                    case 0:
                        // Exiting the Configuration menu
                        return;
                    case 1:
                        // Advancing the clock one minute (only if s_dalConfig isn't null)
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddMinutes(1);
                        break;
                    case 2:
                        // Advancing the clock by one hour
                        s_dal!.Config!.Clock = s_dal!.Config.Clock.AddHours(1);
                        break;
                    case 3:
                        // Displaying the current clock value
                        Console.WriteLine(s_dal!.Config!.Clock);
                        break;
                    case 4:
                        // Seting a new Configuration value
                        SetNewConfigValue();
                        break;
                    case 5:
                        // Displaying a new Configuration value
                        DisplayCurrentConfigValue();
                        break;
                    case 6:
                        // Resetting the Configuration 
                        s_dal!.Config!.Reset();
                        break;
                }
            }
            // Handle any exceptions that occur and display an error message
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // This method deletes all the entities from the data access layers and resets the Configuration.
    private static void DeleteReset()
    {
        if (s_dal!.Volunteer != null)
            s_dal!.Volunteer.DeleteAll();
        if (s_dal!.Call != null)
            s_dal!.Call.DeleteAll();
        if (s_dal!.Assignment != null)
            s_dal!.Assignment.DeleteAll();
        if (s_dal!.Config != null)
            s_dal!.Config.Reset();

    }

    //Creating a new entity depending on the type of the DAL object (user's choice)
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

            //bonus
            string Password = GenerateRandomPassword();
            Console.Write($"The administration has created a Password: {Password}" +
                $" Do you want to change it? If yes, press 1, otherwise press any other key.\n");
            Console.Write("Password (Include at least one uppercase letter and one number): ");
            int choice = int.Parse(Console.ReadLine() ?? "2");
            if (choice == 1) { ChangePassword(ref Password); }

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            //helped by IA
            Console.Write("Latitude: ");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : (double?)null;

            //helped by IA
            Console.Write("Longitude: ");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : (double?)null;

            Console.Write("Job (Manager or Volunteer): ");
            Job job = Enum.TryParse(Console.ReadLine(), out Job parsedJob) ? parsedJob : Job.Volunteer;

            Console.Write("Active (true/false): ");
            bool active = bool.TryParse(Console.ReadLine(), out bool isActive) && isActive;

            Console.Write("Distance: ");
            double? distance = double.TryParse(Console.ReadLine(), out double dist) ? dist : (double?)null;

            Console.Write("Which Distance: ");
            DistanceType whichDistance = Enum.TryParse(Console.ReadLine(), out DistanceType parsedDistance) ? parsedDistance : DistanceType.AirDistance;

            // Create new Volunteer object
            Volunteer newVolunteer = new Volunteer(id, name, phoneNumber, email, Password, address, latitude, longitude, job, active, distance, whichDistance);

            // Add Volunteer to DAL
            volunteerDal.Create(newVolunteer);

        }
        else if (dal is ICall callDal)
        {
            // Prompt the user for Call details
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

    //Printing the entity depending on the user's choice (Volunteer, Call or Assignment)
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

    //Printing all the  entities (Volunteer, Call and Assignment)
    private static void ReadAllEntities<T>(T dal) 
    {
        if (dal is IVolunteer volunteerDal)
        {
            List<Volunteer> volunteers = volunteerDal.ReadAll().ToList(); //converting the IEnumerable to List
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine(volunteer);
                Console.WriteLine("\n");
            }

        }
        else if (dal is ICall callDal)
        {
            List<Call> calls = callDal.ReadAll().ToList(); //converting the IEnumerable to List
            foreach (var call in calls)
            {
                Console.WriteLine(call);
                Console.WriteLine("\n");
            }

        }
        else if (dal is IAssignment assignmentDal)
        {
            List<Assignment> assignments = assignmentDal.ReadAll().ToList(); //converting the IEnumerable to List
            foreach (var assignment in assignments)
            {
                Console.WriteLine(assignment);
                Console.WriteLine("\n");
            }
        }
    }

    //Uptating the entity depending on the user's choice
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

            Console.Write("Password: ");
            string Password = Console.ReadLine() ?? "";

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            //helped by IA
            Console.Write("Latitude: ");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : (double?)null;

            //helped by IA
            Console.Write("Longitude: ");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : (double?)null;

            Console.Write("Job (Manager or Volunteer): ");
            Job job = Enum.TryParse(Console.ReadLine(), out Job parsedJob) ? parsedJob : Job.Volunteer;

            Console.Write("Active (true/false): ");
            bool active = bool.TryParse(Console.ReadLine(), out bool isActive) && isActive;

            Console.Write("Distance: ");
            double? distance = double.TryParse(Console.ReadLine(), out double dist) ? dist : (double?)null;

            Console.Write("Which Distance: ");
            DistanceType whichDistance = Enum.TryParse(Console.ReadLine(), out DistanceType parsedDistance) ? parsedDistance : DistanceType.AirDistance;

            // Create new Volunteer object
            Volunteer newVolunteer = new Volunteer(id, name, phoneNumber, email, Password, address, latitude, longitude, job, active, distance, whichDistance);

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

    //Deleting the entity depending on the user's choice (Volunteer, Call or Assignment)
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

    // This method deletes all entities of a specific type from the data access layer
    private static void DeleteAllEntities<T>(T dal) 
    {
        if (dal is IVolunteer volunteerDal)
            volunteerDal.DeleteAll();
        else if (dal is ICall callDal)
            callDal.DeleteAll();
        else if (dal is IAssignment assignmentDal)
            assignmentDal.DeleteAll();
    }
    
    //Setting a new Configuration value depending on the user's value
    private static void SetNewConfigValue()
    {
        int choice = 2;
        do
        {
            Console.Write("Please enter 0 to Config a RiskRange and 1 to Config Clock (in minutes) ");
            choice = int.Parse(Console.ReadLine() ?? "2");

        }
        while (choice != 0 || choice != 1);

        Console.WriteLine("Please enter the new value to set");
        int value = int.Parse(Console.ReadLine() ?? "0");

        if (choice==0)
            s_dal!.Config!.Clock= DateTime.Now.AddMinutes(value);
        else
            s_dal!.Config!.RiskRange = TimeSpan.FromMinutes(value);
    }

    // This method displays the current Configuration values.
    private static void DisplayCurrentConfigValue()
    {
        Console.WriteLine("Here are all the Configuration values:");
        Console.WriteLine($"Risk Range: {s_dal!.Config!.RiskRange}");
        Console.WriteLine($"Clock: {s_dal!.Config!.Clock}/n");
    }

    //This method is used to change the Password of the volunteer by his choice
    private static void ChangePassword(ref string Password)
    {
        do
        {
            Password = Console.ReadLine() ?? "";
            // string class functions Any, IsUpper, IsDigit
            if (Password.Any(char.IsUpper) && Password.Any(char.IsDigit))
            {
                break;
            }
            Console.WriteLine("Password must contain at least one uppercase letter and one number. Please try again.");

        } while (true);
    }

    // this function was generated by AI 
    //The GenerateRandomPassword method generates a random Password containing at least one uppercase letter,
    /// one number, and a mix of other characters. The Password has a length of 6 characters. 
    private static string GenerateRandomPassword()
    {
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string allChars = lowerChars + upperChars + digits;

        Random random = new Random();
        StringBuilder Password = new StringBuilder();

        // Ensure at least one uppercase letter and one digit
        Password.Append(upperChars[random.Next(upperChars.Length)]);
        Password.Append(digits[random.Next(digits.Length)]);

        // Fill the rest of the Password length (6 characters) with random characters
        for (int i = 2; i < 6; i++)
        {
            Password.Append(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle the characters in the Password to ensure randomness
        return new string(Password.ToString().OrderBy(c => random.Next()).ToArray());
    }
}

