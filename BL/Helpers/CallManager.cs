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
    internal static ObserverManager Observers = new(); //stage 5 
    /// <summary>
    /// This method converts a DO.Call object to a BO.CallInList object.
    /// </summary>
    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == call.CallId);
        DO.Assignment? assign = assignments?.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        string? name = s_dal.Volunteer.Read(volunteer => volunteer.VolunteerId == assign?.VolunteerId)?.Name;

        TimeSpan? exTime = null;
        if (assign?.End != null) exTime = assign.End - call.OpenTime;

        BO.Statuses status = ToDeterminateStatus(assign, call);

        TimeSpan? rangeTimeToEnd = status == BO.Statuses.Expired ? TimeSpan.Zero : call.MaxEnd - s_dal.Config.Clock;

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
    internal static BO.OpenCallInList ConvertToOpenCallInList(DO.Call call, int volunteerId)
    {
        double? volunteerLong = s_dal.Volunteer.Read(volunteerId)?.Longitude;
        double? volunteerLat = s_dal.Volunteer.Read(volunteerId)?.Latitude;

        double distance = Math.Sqrt(
        Math.Pow((double)(volunteerLong - call.Longitude), 2) +
        Math.Pow((double)(volunteerLat - call.Latitude), 2));

        return new BO.OpenCallInList
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            Description = call.Description,
            CallAddress = call.Address,
            BeginTime = call.OpenTime,
            MaxEndTime = call.MaxEnd,
            VolunteerDistanceToCall = distance
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
        if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd) { return BO.Statuses.Expired; }
        else if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange)
        { return BO.Statuses.OpenToRisk; }
        else if (assign?.End == null)
        {
            return call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange ?
                BO.Statuses.InActionToRisk : BO.Statuses.InAction;
        }
        else if (assign.MyEndStatus == DO.EndStatus.Completed) { return BO.Statuses.Closed; }
        else if (assign.MyEndStatus == DO.EndStatus.Expired) { return BO.Statuses.Expired; }
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

        //the adress details will be check after the call of this function
    }
    public static class GetCoordinatesFromAddressSync
    {
        private const string GoogleApiKey = "AIzaSyCuGWKseIQvrkb9Yk3U14e_9K9pltkSwug";

        public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("The address cannot be null or empty.", nameof(address));
            }

            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={GoogleApiKey}";

            try
            {
                using (var webClient = new WebClient())
                {
                    string response = webClient.DownloadString(url);
                    var geocodeResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(response);

                    if (geocodeResponse == null || geocodeResponse.Status != "OK" || geocodeResponse.Results.Length == 0)
                    {
                        throw new Exception($"Address not found or invalid response from Google Maps API: {geocodeResponse?.Status}");
                    }

                    double latitude = geocodeResponse.Results[0].Geometry.Location.Lat;
                    double longitude = geocodeResponse.Results[0].Geometry.Location.Lng;

                    return (latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the coordinates: {ex.Message}");
                throw;
            }
        }
    }

    // Classes pour désérialiser la réponse de l'API Google Maps
    public class GoogleGeocodeResponse
    {
        public string Status { get; set; }
        public GoogleGeocodeResult[] Results { get; set; }
    }

    public class GoogleGeocodeResult
    {
        public GoogleGeometry Geometry { get; set; }
    }

    public class GoogleGeometry
    {
        public GoogleLocation Location { get; set; }
    }

    public class GoogleLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

/// <summary>
/// Checks if the coordinates of a call match the coordinates of the address.
/// </summary>
   internal static  bool AreCoordinatesMatching(BO.Call call)
    {
        var (latitude, longitude) = GetCoordinatesFromAddressSync.GetCoordinates(call.CallAddress);
        return (latitude, longitude) == (call.CallLatitude, call.CallLongitude);
    }
}
