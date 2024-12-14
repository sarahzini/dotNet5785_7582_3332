using DalApi;
using DO;
using System.Net;
using System.Text.Json;
namespace Helpers;
/// <summary>
/// All the Helpers methods that are used for the implementations of calls.
/// </summary>
internal static partial class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
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


}

