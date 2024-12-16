using BIApi;
using DalApi;
using System.Text.Json;
namespace Helpers;
/// <summary>
/// All the Helpers methods that are used for the implementations of calls.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; 
    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll().Where(assignment => assignment.CallId == call.Id);
        DO.Assignment? assign = assignments.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        string? name = s_dal.Volunteer.Read(volunteer => volunteer.Id == assign?.VolunteerId)?.Name;

        TimeSpan? exTime=null;
        if (assign?.End != null) exTime = assign.End - call.DateTime;

        BO.Statuses status= ToDeterminateStatus(assign,call);

        return new BO.CallInList
        {
            AssignId = assign?.Id, 
            CallId = call.Id,
            TypeOfCall = (BO.SystemType)call.Choice,
            BeginTime = call.DateTime,
            RangeTimeToEnd = call.EndDateTime - call.DateTime,
            NameLastVolunteer = name,
            ExecutedTime = exTime,
            Status = status,
            TotalAssignment = assignments.Count()
        };


    }
    internal static BO.ClosedCallInList ConvertToClosedCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll().Where(assignment => assignment.CallId == call.Id);
        DO.Assignment? assign = assignments.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        return new BO.ClosedCallInList
        {
            CallId = call.Id,
            TypeOfCall = (BO.SystemType)call.Choice,
            CallAddress = call.Address,
            BeginTime = call.DateTime,
            BeginActionTime = assign!.Begin,
            EndTime = assign!.End,
            TypeOfEnd = (BO.EndStatus)assign.MyEndStatus
        };
    }
    internal static BO.OpenCallInList ConvertToOpenCallInList(DO.Call call,int volunteerId)
    {
        double? volunteerLong = s_dal.Volunteer.Read(volunteerId).Longitude;
        double? volunteerLat = s_dal.Volunteer.Read(volunteerId).Latitude;
        double distance = Math.Sqrt(
        Math.Pow((double)(volunteerLong - call.Longitude), 2) +
        Math.Pow((double)(volunteerLat - call.Latitude), 2));
        return new BO.OpenCallInList
        {
            CallId = call.Id,
            TypeOfCall=(BO.SystemType)call.Choice,
            Description=call.Description,
            CallAddress=call.Address,
            BeginTime= call.DateTime,
            MaxEndTime=call.EndDateTime,
            VolunteerDistanceToCall=distance
        };
    }
    internal static DO.Call ConvertToDataCall(BO.Call call)
    {
        DO.Assignment? assign = AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.CallId);

        return new DO.Call
        {
            Id = call.CallId,
            Address = call.CallAddress,
            Latitude = call.CallLatitude,
            Longitude = call.CallLongitude,
            DateTime = call.BeginTime,
            Choice = (DO.SystemType)call.TypeOfCall,
            Description = call.Description,
            EndDateTime = assign?.End
        };
    }
    internal static BO.Call ConvertToLogicCall(DO.Call call)
    {

        // Get the call assignments from the data layer
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll().Where(assignment => assignment.CallId == call.Id);
        DO.Assignment? assign = assignments.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        List<BO.CallAssignInList> CallAssignInList = assignments
            .Select(assignment => new BO.CallAssignInList
            {
                VolunteerId = assignment.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(assignment!.VolunteerId).Name,
                BeginActionTime = assignment.Begin,
                EndTime = assignment.End,
                ClosureType = (BO.EndStatus)assignment!.MyEndStatus
            })
            .ToList();

        BO.Statuses status=ToDeterminateStatus(assign,call);

        return new BO.Call
        {
            CallId = call.Id,
            TypeOfCall = (BO.SystemType)call.Choice,
            Description = call.Description,
            CallAddress = call.Address,
            CallLatitude = call.Latitude,
            CallLongitude = call.Longitude,
            BeginTime = call.DateTime,
            MaxEndTime = call.EndDateTime,
            ClosureType = status,
            CallAssigns = CallAssignInList
        };
    }
    private static BO.Statuses ToDeterminateStatus(DO.Assignment assign, DO.Call call)
    {
        if (assign == null) { return BO.Statuses.Open; }
        else if (assign.End == null) { return BO.Statuses.InAction; }
        else if (assign.MyEndStatus == DO.EndStatus.Completed) { return BO.Statuses.Closed; }
        else if (assign.MyEndStatus == DO.EndStatus.Expired || (call.EndDateTime.HasValue && DateTime.Now > call.EndDateTime.Value)) { return BO.Statuses.Expired; }
        else if (call.EndDateTime.HasValue && DateTime.Now > call.EndDateTime.Value - s_dal.Config.RiskRange) { return assign.End == null ? BO.Statuses.InActionToRisk : BO.Statuses.OpenToRisk; }
        else { return BO.Statuses.Open; }

    }
    internal static bool IsRequesterManager(int requesterId)
    {
        DO.Volunteer? user = s_dal.Volunteer.Read(requesterId);
        return user?.MyJob == DO.Job.Manager;
    }
    internal static void ValidateCallDetails(BO.Call call)
    {
        //checks wether the Maxendtime is greater than the call start time and the current time
        if (call.MaxEndTime <= call.BeginTime || call.MaxEndTime <= DateTime.Now)
        {
            throw new BO.BLException("The maximum end time must be greater than the call start time and the current time.");
        }

        if (!AreCoordinatesMatching(call))
        {
            throw new BO.BLException("The coordinates of the call do not match the coordinates of the address.");
        }
    } //??????????

    /// The HttpClient instance is used to make HTTP requests to external services.
    /// In the context of the CallManager class, it is used to interact with the LocationIQ API 
    /// to get the coordinates of an address.
    private static readonly HttpClient httpClient = new HttpClient();

    // LocationIQ API key
    private const string LocationIQApiKey = "675b1b0034c57582409131rpj9f815d";

    //use of AI to get the coordinates of an address
    /// <summary>
    /// This method returns the latitude and longitude coordinates of a given address.
    /// </summary>
    /// <param name="address">The address of a volunteer/call/assignment </param>
    /// <returns></returns>
    public static (double Latitude, double Longitude) GetCoordinatesFromAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The address cannot be null or empty.", nameof(address));
        }

        string url = $"https://us1.locationiq.com/v1/search.php?key={LocationIQApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        try
        {
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            string responseBody = response.Content.ReadAsStringAsync().Result;
            var locationData = JsonSerializer.Deserialize<LocationIQResponse[]>(responseBody);

            if (locationData == null || locationData.Length == 0)
            {
                throw new Exception("Invalid address.");
            }

            double latitude = double.Parse(locationData[0].Latitude);
            double longitude = double.Parse(locationData[0].Longitude);

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while validating the address.", ex);
        }
    }
    /// <summary>
    /// This class is used to deserialize the response from the LocationIQ API
    /// </summary>
    private class LocationIQResponse
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    /// <summary>
    /// Checks if the coordinates of a call match the coordinates of the address
    /// </summary>
    /// <param name="call">A Call variable </param>
    internal static bool AreCoordinatesMatching(BO.Call call)
    {
        return GetCoordinatesFromAddress(call.CallAddress) == (call.CallLatitude, call.CallLongitude);

    }

}

