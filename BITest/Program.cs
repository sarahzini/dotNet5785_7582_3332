using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using BlApi;
using BO;
using DalApi;
using DO;

namespace BlTest;

internal class Program
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the MDA Volunteers System !:");
        try
        {
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Manage Volunteers");
                Console.WriteLine("2. Manage Calls");
                Console.WriteLine("3. Manage Admin");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ManageVolunteers();
                        break;
                    case "2":
                        ManageCalls();
                        break;
                    case "3":
                        ManageAdmin();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        catch (BO.BLDoesNotExistException ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (BO.BLAlreadyExistException ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (BO.BLIncorrectPassword ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (BO.BLFormatException ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (BO.BLInvalidOperationException ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (BO.BLXMLFileLoadCreateException ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" {ex.Message}{ex.InnerException?.Message}");
        }
    }
    private static void ManageCalls()
    {
        while (true)
        {
            Console.WriteLine("Calls Menu:");
            Console.WriteLine("1. Type Of Call Counts");
            Console.WriteLine("2. Get Sorted Calls In List");
            Console.WriteLine("3. Get Call Details");
            Console.WriteLine("4. Update Call Details");
            Console.WriteLine("5. Delete Call");
            Console.WriteLine("6. Add Call");
            Console.WriteLine("7. Sort Closed Calls");
            Console.WriteLine("8. Sort Open Calls");
            Console.WriteLine("9. Complete Call");
            Console.WriteLine("10. Cancel Assignment");
            Console.WriteLine("11. Assign Call To Volunteer");
            Console.WriteLine("12. Back to Main Menu");
            Console.Write("Select an option: ");

            CallMethods choice = (CallMethods)int.Parse(Console.ReadLine() ?? "0");
            switch (choice)
            {
                case CallMethods.TypeOfCallCounts:
                    int[] result = s_bl.Call.TypeOfCallCounts();
                    Console.WriteLine($"Number of ICU calls in the system :{result[0]} ");
                    Console.WriteLine($"Number of Regular calls in the system :{result[1]} ");
                    break;
                case CallMethods.GetSortedCallsInList:
                    GetSortedCallsInList();
                    break;
                case CallMethods.GetCallDetails:
                    GetDetails("call");
                    break;
                case CallMethods.UpdateCallDetails:
                    Update("call");
                    break;
                case CallMethods.DeleteCall:
                    Delete("call");
                    break;
                case CallMethods.AddCall:
                    Add("call");
                    break;
                case CallMethods.SortClosedCalls:
                    SortClosedCalls();
                    break;
                case CallMethods.SortOpenCalls:
                    SortOpenCalls();
                    break;
                case CallMethods.CompleteCall:
                    CompleteCall();
                    break;
                case CallMethods.CancelAssignment:
                    CancelAssignment();
                    break;
                case CallMethods.AssignCallToVolunteer:
                    AssignCallToVolunteer();
                    break;
                case CallMethods.BackToMainMenu:
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    static void ManageVolunteers()
    {
        while (true)
        {
            Console.WriteLine("Volunteers Menu:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Get Volunteers In List");
            Console.WriteLine("3. Get Volunteer Details");
            Console.WriteLine("4. Update Volunteer");
            Console.WriteLine("5. Delete Volunteer");
            Console.WriteLine("6. Add Volunteer");
            Console.WriteLine("7. Back to Main Menu");
            Console.Write("Select an option: ");

            VolunteerMethods choice = (VolunteerMethods)int.Parse(Console.ReadLine() ?? "0");
            switch (choice)
            {
                case VolunteerMethods.Login:
                    Login();
                    break;
                case VolunteerMethods.GetVolunteersInList:
                    GetVolunteersInList();
                    break;
                case VolunteerMethods.GetVolunteerDetails:
                    GetDetails("volunteer");
                    break;
                case VolunteerMethods.UpdateVolunteer:
                    Update("volunteer");
                    break;
                case VolunteerMethods.DeleteVolunteer:
                    Delete("volunteer");
                    break;
                case VolunteerMethods.AddVolunteer:
                    Add("volunteer");
                    break;
                case VolunteerMethods.BackToMainMenu:
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        }
    static void ManageAdmin()
    {
        while (true)
        {
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. Get Clock");
            Console.WriteLine("2. Forward Clock");
            Console.WriteLine("3. Get Risk Range");
            Console.WriteLine("4. Set Risk Range");
            Console.WriteLine("5. Initialize DB");
            Console.WriteLine("6. Reset DB");
            Console.WriteLine("7. Back to Main Menu");
            Console.Write("Select an option: ");

            AdminMethods choice = (AdminMethods)int.Parse(Console.ReadLine() ?? "0");
            switch (choice)
            {
                case AdminMethods.GetClock:
                    Console.WriteLine(s_bl.Admin.GetClock());
                    break;
                case AdminMethods.ForwardClock:
                    Console.WriteLine("Select the time unit to forward the clock:");
                    Console.WriteLine("1. Minute");
                    Console.WriteLine("2. Hour");
                    Console.WriteLine("3. Day");
                    Console.WriteLine("4. Month");
                    Console.WriteLine("5. Year");
                    Console.Write("Enter your choice (if you don't enter one of this number, the clock will forward a minute): ");
                    TimeUnit timeUnit = (TimeUnit)int.Parse(Console.ReadLine() ?? "1");
                    s_bl.Admin.ForwardClock(timeUnit);
                    break;
                case AdminMethods.GetRiskRange:
                    Console.WriteLine(s_bl.Admin.GetRiskRange());
                    break;
                case AdminMethods.SetRiskRange:
                    Console.WriteLine("Please enter a new value of risk range");
                    s_bl.Admin.SetRiskRange(TimeSpan.Parse(Console.ReadLine() ?? "0"));
                    break;
                case AdminMethods.InitializeDB:
                    s_bl.Admin.InitializeDB();
                    break;
                case AdminMethods.ResetDB:
                    s_bl.Admin.ResetDB();
                    break;
                case AdminMethods.BackToMainMenu:
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    private static void Login()
    {
        Console.Write("Enter your name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter your password: ");
        string? password = Console.ReadLine();

        DO.Job job = s_bl.Volunteer.Login(name, password);
        Console.WriteLine($"Welcome to the {job} {name} !");
    }
    private static void GetVolunteersInList()
    {
        Console.Write("Enter 'true' to get active volunteers, 'false' to get inactive volunteers, or leave empty to get all volunteers: ");
        bool? isActive = bool.TryParse(Console.ReadLine(), out bool result) ? result : (bool?)null;

        Console.Write("Enter the field to sort by (0 for VolunteerId, 1 for Name, 2 for CompletedCalls, 3 for CancelledCalls, 4 for ExpiredCalls, 5 for ActualCallId, or leave empty and we will sort by Id): ");
        VolunteerInListFieldSort? sortField = int.TryParse(Console.ReadLine(), out int sortFieldResult) ? (VolunteerInListFieldSort)sortFieldResult : (VolunteerInListFieldSort?)null;

        IEnumerable<VolunteerInList>? v = s_bl.Volunteer.GetVolunteersInList(isActive, sortField);
        foreach(VolunteerInList volunteer in v)
        {
            Console.WriteLine(volunteer);
        }

    }
    private static void GetDetails(string type)
    {
        if (type == "volunteer")
        {
            Console.Write("Enter the Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine() ?? "0");

            BO.Volunteer v = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
            Console.WriteLine(v);
        }
        else
        {
            Console.Write("Enter the Call ID: ");
            int callId = int.Parse(Console.ReadLine() ?? "0");

            BO.Call c = s_bl.Call.GetCallDetails(callId);
            Console.WriteLine(c);
        }
    }
    private static void Update(string type)
    {
        if (type == "volunteer")
        {
            Console.WriteLine("Please enter your ID: ");
            int requesterId = int.Parse(Console.ReadLine() ?? "0");

            s_bl.Volunteer.UpdateVolunteer(requesterId, InputBOVolunteer());
        }
        else //type is call
        {
            s_bl.Call.UpdateCallDetails(InputBOCall());
        }

    }
    private static void Delete(string type)
    {
        if (type == "volunteer")
        {
            Console.WriteLine("Please enter the ID of the volunteer you want to delete: ");
            int volunteerId = int.Parse(Console.ReadLine() ?? "0");

            s_bl.Volunteer.DeleteVolunteer(volunteerId);
        }
        else //type is call
        {
            Console.WriteLine("Please enter the ID of the call you want to delete: ");
            int callId = int.Parse(Console.ReadLine() ?? "0");

            s_bl.Call.DeleteCall(callId);
        }
    }
    private static void Add(string type)
    {
        if (type=="volunteer")
            s_bl.Volunteer.AddVolunteer(InputBOVolunteer());
        else //type is call
            s_bl.Call.AddCall(InputBOCall());
    }
    private static void GetSortedCallsInList()
    {
        Console.Write("Enter the field to filter by (0 for CallId, 1 for BeginTime, 2 for NameLastVolunteer, 3 for ExecutedTime, 4 for TotalAssignment, or leave empty and we will sort by Id: ");
        CallInListField? filterField = int.TryParse(Console.ReadLine(), out int filterFieldResult) ? (CallInListField)filterFieldResult : (CallInListField?)null;

        Console.Write("Enter the value to filter, or empty if you decided to cancel the filter: ");
        object? filterValue = string.IsNullOrEmpty(Console.ReadLine()) ? null : Console.ReadLine();

        Console.Write("Enter the field to sort by (0 for CallId, 1 for BeginTime, 2 for NameLastVolunteer, 3 for ExecutedTime, 4 for TotalAssignment, or leave empty and we will sort by Id: ");
        CallInListField? sortField = int.TryParse(Console.ReadLine(), out int sortFieldResult) ? (CallInListField)sortFieldResult : (CallInListField?)null;

        IEnumerable<CallInList>? c = s_bl.Call.GetSortedCallsInList(filterField, filterValue, sortField);
        foreach (CallInList call in c)
        {
            Console.WriteLine(call);
        }
    } 
    private static void SortClosedCalls()
    {
        Console.Write("Enter your Volunteer ID: ");
        int volunteerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter '1' to get ICU Call, '2' to get Regular Call, or leave empty to get all the closed calls: ");
        BO.SystemType? callType = int.TryParse(Console.ReadLine(), out int result) && (result == 1 || result == 2) ? (BO.SystemType?)result : null;

        Console.Write("Enter the field to sort by (0 for CallId, 1 for BeginTime, 2 for BeginActionTime, 3 for EndTime, or leave empty and we will sort by Id: ");
        ClosedCallInListField? sortField = int.TryParse(Console.ReadLine(), out int sortFieldResult) ? (ClosedCallInListField)sortFieldResult : (ClosedCallInListField?)null;

        IEnumerable<ClosedCallInList>? c = s_bl.Call.SortClosedCalls(volunteerId,callType, sortField);
        foreach (ClosedCallInList call in c)
        {
            Console.WriteLine(call);
        }
    } 
    private static void SortOpenCalls()
    {
        Console.Write("Enter your Volunteer ID: ");
        int volunteerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter '1' to get ICU Call, '2' to get Regular Call, or leave empty to get all the open calls: ");
        BO.SystemType? callType = int.TryParse(Console.ReadLine(), out int result) && (result == 1 || result == 2) ? (BO.SystemType?)result : null;

        Console.Write("Enter the field to sort by (0 for CallId, 1 for BeginTime, 2 for RangeTimeToEnd, or leave empty and we will sort by Id: ");
        OpenCallInListField? sortField = int.TryParse(Console.ReadLine(), out int sortFieldResult) ? (OpenCallInListField)sortFieldResult : (OpenCallInListField?)null;

        IEnumerable<OpenCallInList>? c = s_bl.Call.SortOpenCalls(volunteerId, callType, sortField);
        foreach (OpenCallInList call in c)
        {
            Console.WriteLine(call);
        }
    } 
    private static void CompleteCall()
    {
        Console.Write("Enter your ID: ");
        int volunteerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter the Id of the assignment that you want to complete: ");
        int assignmentId = int.Parse(Console.ReadLine() ?? "0");

        s_bl.Call.CompleteCall(volunteerId, assignmentId);

    }
    private static void CancelAssignment()
    {
        Console.Write("Enter your ID: ");
        int requesterId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter the Id of the assignment that you want to cancel: ");
        int assignmentId = int.Parse(Console.ReadLine() ?? "0");

        s_bl.Call.CancelAssignment(requesterId, assignmentId);
    }
    private static void AssignCallToVolunteer()
    {
        Console.Write("Enter your ID: ");
        int volunteerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter the Id of the call that you want to do: ");
        int callId = int.Parse(Console.ReadLine() ?? "0");

        s_bl.Call.AssignCallToVolunteer(volunteerId, callId);
    }
    private static BO.Volunteer InputBOVolunteer()
    {
        Console.WriteLine("Please enter the following volunteer details:");

        Console.Write("ID: ");
        int id = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Phone Number: ");
        string phoneNumber = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ??"";

        //bonus
        string password = GenerateRandomPassword();
        Console.Write($"The administration has created a Password: {password}" +
            $" Do you want to change it? If yes, press 1, otherwise press any other key.\n");
        Console.Write("Password (Include at least one uppercase letter and one number): ");
        int choice = int.Parse(Console.ReadLine() ?? "2");
        if (choice == 1) { ChangePassword(ref password); }

        Console.Write("Address: ");
        string? address = Console.ReadLine();

        Console.Write("Volunteer Job (0 for Volunteer, 1 for Manager): ");
        BO.Job volunteerJob = Enum.TryParse(Console.ReadLine(), out BO.Job parsedJob) ? parsedJob : BO.Job.Volunteer;

        Console.Write("Is Active (true/false): ");
        bool isActive = bool.TryParse(Console.ReadLine(), out bool parsedIsActive) ? parsedIsActive : true;

        Console.Write("Max Distance for a call: ");
        double? distance = double.TryParse(Console.ReadLine(), out double dist) ? dist : (double?)null;

        Console.Write("Which Distance (Air,Walking,Driving): ");
        BO.DistanceType whichDistance = Enum.TryParse(Console.ReadLine(), out BO.DistanceType parsedDistance) ? parsedDistance : BO.DistanceType.AirDistance;

        //rest of the members we dont need because we are going to convert to DO.Volunteer
        return new BO.Volunteer
            {
                VolunteerId = id,
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                VolunteerAddress = address,
                VolunteerLatitude = null,
                VolunteerLongitude = null,
                VolunteerJob = volunteerJob,
                IsActive = isActive,
                MaxVolunteerDistance = distance,
                VolunteerDT = whichDistance,
                CompletedCalls = 0,
                CancelledCalls = 0,
                ExpiredCalls = 0,
                CurrentCall = null
            };
        
    }
    private static BO.Call InputBOCall()
    {
        Console.WriteLine("Please enter the following call details:");

        Console.Write("CallID: ");
        int CallId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Address: ");
        string address = Console.ReadLine() ?? "";

        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";

        Console.Write("Open Time: ");
        DateTime openTime = DateTime.Parse(Console.ReadLine() ?? "");

        Console.Write("Max End Time: ");
        DateTime maxEnd = DateTime.Parse(Console.ReadLine() ?? "");

        Console.Write("Ambulance Type (ICUAmbulance or RegularAmbulance): ");
        BO.SystemType ambulanceType = Enum.TryParse(Console.ReadLine(), out BO.SystemType parsedType) ? parsedType : BO.SystemType.RegularAmbulance;


        //rest of the members we dont need because we are going to convert to DO.Volunteer

        return new BO.Call
        {
            CallId = CallId,
            TypeOfCall = ambulanceType,
            Description = description,
            CallAddress = address,
            CallLatitude = 0,
            CallLongitude = 0,
            BeginTime = openTime,
            MaxEndTime = maxEnd,
            Status = Statuses.Open,
            CallAssigns=null
        };
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

