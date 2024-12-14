using BIApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.Json;
namespace Helpers;
/// <summary>
/// All the Helpers methods that are used for the implementations of calls.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll().Where(assignment => assignment.CallId == call.Id);
        DO.Assignment? assign = assignments.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        string? name = s_dal.Volunteer.Read(volunteer => volunteer.Id == assign?.VolunteerId)?.Name;

        TimeSpan? exTime=null;
        if (assign?.End != null) exTime = assign.End - call.DateTime;

        BO.Statuses status;
        if (assign == null) { status = BO.Statuses.Open; }
        else if (assign.End == null) { status = BO.Statuses.InAction; }
        else if (assign.MyEndStatus == DO.EndStatus.Completed) { status = BO.Statuses.Closed; }
        else if (assign.MyEndStatus == DO.EndStatus.Expired || (call.EndDateTime.HasValue && DateTime.Now > call.EndDateTime.Value)) { status = BO.Statuses.Expired; }
        else if (call.EndDateTime.HasValue && DateTime.Now > call.EndDateTime.Value - s_dal.config.RiskRange) { status = assign.End == null ? BO.Statuses.InActionToRisk : BO.Statuses.OpenToRisk; }
        else { status = BO.Statuses.Open; }


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
        double? distance = Math.Sqrt(
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="BO.BLException"></exception>
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
    }

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


    /// <summary>
    /// Helper method to check if the requester is a director
    /// </summary>
    /// <param name="requesterId"></param>
    /// <returns></returns>
    internal static bool IsRequesterDirector(int requesterId)
    {
        var user = s_dal.Volunteer.Read(requesterId);
        return user != null && user.MyJob== Job.Director;
    }
    internal static DO.Call ConvertToLogicCall(BO.Call call)
    {
        return new DO.Call
        {
            Id = call.CallId,
            Address = null,
            Latitude = call.CallLatitude,
            Longitude = call.CallLongitude,
            DateTime = call.BeginTime,
            Choice = (DO.SystemType)call.TypeOfCall,
            Description = null,  //pas bon a changer
            EndDateTime = null  //pas bon a changer
        };
    }

}

