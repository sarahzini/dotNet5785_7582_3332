using DalApi;
using System.Net;
using System.Text.Json;
namespace Helpers;
/// <summary>
/// All the Helpers methods that are used for the implementations of calls.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// This method converts a DO.Call object to a BO.CallInList object.
    /// </summary>
    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == call.CallId);
        DO.Assignment? assign = assignments?.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        string? name = s_dal.Volunteer.Read(volunteer => volunteer.VolunteerId == assign?.VolunteerId)?.Name;

        TimeSpan? exTime=null;
        if (assign?.End != null) exTime = assign.End - call.OpenTime;

        BO.Statuses status= ToDeterminateStatus(assign,call);

        return new BO.CallInList
        {
            AssignId = assign?.AssignmentId,
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            BeginTime = call.OpenTime,
            RangeTimeToEnd = call.MaxEnd - s_dal.Config.Clock,
            NameLastVolunteer = name,
            ExecutedTime = exTime,
            Status = status,
            TotalAssignment = assignments is null ? 0 : assignments.Count()
        };
    }
    /// <summary>
    /// This method converts a DO.Call object to a BO.ClosedCallInList.
    /// </summary>
    internal static BO.ClosedCallInList ConvertToClosedCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll().Where(assignment => assignment.CallId == call.CallId);
        DO.Assignment? assign = assignments.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        return new BO.ClosedCallInList
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            CallAddress = call.Address,
            BeginTime = call.OpenTime,
            BeginActionTime = assign!.Begin,
            EndActionTime = assign!.End,
            TypeOfEnd = (BO.EndStatus)assign?.MyEndStatus
        };
    }

    /// <summary>
    /// This method converts a DO.Call object to a BO.OpenCallInList.
    /// </summary>
    internal static BO.OpenCallInList ConvertToOpenCallInList(DO.Call call,int volunteerId)
    {
        double? volunteerLong = s_dal.Volunteer.Read(volunteerId)?.Longitude;
        double? volunteerLat = s_dal.Volunteer.Read(volunteerId)?.Latitude;

        double distance = Math.Sqrt(
        Math.Pow((double)(volunteerLong - call.Longitude), 2) +
        Math.Pow((double)(volunteerLat - call.Latitude), 2));

        return new BO.OpenCallInList
        {
            CallId = call.CallId,
            TypeOfCall=(BO.SystemType)call.AmbulanceType,
            Description=call.Description,
            CallAddress=call.Address,
            BeginTime= call.OpenTime,
            MaxEndTime=call.MaxEnd,
            VolunteerDistanceToCall=distance
        };
    } //changer distance
   
    /// <summary>
    /// This method converts a BO.Call object to a DO.Call object.
    /// </summary>
    internal static DO.Call ConvertToDataCall(BO.Call call)
    {
        return new DO.Call
        {
            CallId = call.CallId,
            Address = call.CallAddress,
            Latitude = call.CallLatitude,
            Longitude = call.CallLongitude,
            OpenTime = call.BeginTime,
            AmbulanceType = (DO.SystemType)call.TypeOfCall,
            Description = call.Description,
            MaxEnd = call.MaxEndTime
        };
    }

    /// <summary>
    /// This method converts a DO.Call object to a BO.Call object.
    /// </summary>
    internal static BO.Call ConvertToLogicCall(DO.Call call)
    {

        // Get the call assignments from the data layer
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == call.CallId);

        DO.Assignment? assign = assignments?.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();
        BO.Statuses status = ToDeterminateStatus(assign, call);

        List<BO.CallAssignInList>? CallAssignInList = assignments?
            .Select(assignment => new BO.CallAssignInList
            {
                VolunteerId = assignment.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(assignment.VolunteerId)?.Name,
                BeginActionTime = assignment.Begin,
                EndActionTime = assignment.End,
                ClosureType = (BO.EndStatus)assignment.MyEndStatus
            })
            .ToList();

        return new BO.Call
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            Description = call.Description,
            CallAddress = call.Address,
            CallLatitude = call.Latitude,
            CallLongitude = call.Longitude,
            BeginTime = call.OpenTime,
            MaxEndTime = call.MaxEnd,
            Status = status,
            CallAssigns = CallAssignInList
        };
    }

    /// <summary>
    ///This method determine the status of a call, depending on the case it can be Open, Closed, InAction, InActionToRisk, OpenToRisk, Expired.
    /// </summary>
    private static BO.Statuses ToDeterminateStatus(DO.Assignment? assign, DO.Call call)
    {
        if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd ) { return BO.Statuses.Expired; }
        else if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange) 
        { return BO.Statuses.OpenToRisk; }
        else if (assign?.End == null) { return call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange ?
                BO.Statuses.InActionToRisk :BO.Statuses.InAction; }
        else if (assign.MyEndStatus == DO.EndStatus.Completed) { return BO.Statuses.Closed; }
        else if (assign.MyEndStatus == DO.EndStatus.Expired ) { return BO.Statuses.Expired; }
        else { return BO.Statuses.Open; }

    }

    /// <summary>
    /// This method determine if the requester is a manager.
    /// </summary>
    internal static bool IsRequesterManager(int requesterId)
    {
        DO.Volunteer? user = s_dal.Volunteer.Read(requesterId);
        return user is null ? false : user?.MyJob == DO.Job.Manager;
    }

    /// <summary>
    /// This method checks the details of a call.
    /// </summary>
     internal static void ValidateCallDetails(BO.Call call)
    {
        //checks wether the Maxendtime is greater than the call start time and the current time
        if (call.MaxEndTime <= call.BeginTime || call.MaxEndTime <= DateTime.Now)
        {
            throw new BO.BLFormatException("The maximum end time must be greater than the call start time and the current time.");
        }

        if (call.TypeOfCall != BO.SystemType.RegularAmbulance && call.TypeOfCall != BO.SystemType.RegularAmbulance)
        {
            throw new BO.BLFormatException("Type of the call must be either 'Regular Ambulance' or 'ICU Ambulance'.");
        }

        //the adress details will be check after the call of this function
    }

    /// <summary>
    /// The HttpClient instance is used to make HTTP requests to external services.
    /// In the context of the CallManager class, it is used to interact with the LocationIQ API 
    /// to get the coordinates of an address.
    /// </summary>
    private static readonly HttpClient httpClient = new HttpClient();

    // LocationIQ API key
    private const string LocationIQApiKey = "pk.3abfd5d3f5b2b87a10e7cb0d73c2c30e";

    /// <summary>
    /// This method returns the latitude and longitude coordinates of a given address.
    /// </summary>
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
            throw new Exception("An error occurred while retrieving the coordinates.", ex);
        }
    }

    /// <summary>
    /// This class is used to deserialize the response from the LocationIQ API.
    /// </summary>
    private class LocationIQResponse
    {
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }

    /// <summary>
    /// Checks if the coordinates of a call match the coordinates of the address.
    /// </summary>
    internal static bool AreCoordinatesMatching(BO.Call call)
    {
        return GetCoordinatesFromAddress(call.CallAddress) == (call.CallLatitude, call.CallLongitude);
    }
}
