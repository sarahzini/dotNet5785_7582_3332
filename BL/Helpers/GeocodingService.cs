
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using BO;
using DalApi;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Helpers;

/// <summary>
/// The GeocodingHelper class provides a method to retrieve geographical coordinates (latitude and longitude)
/// for a given address using the LocationIQ API.
/// </summary>
/// 
public static class GeocodingService
{
    private static IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5 

    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1/search.php";
    private const string LocationIqApiKey = "pk.57b6dccaa943970004ae28c88b3506f5";
    //private const string LocationIqApiKey = "pk.675b1b0034c57582409131rpj9f815d";

    public static async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The provided address is empty or invalid.");
        }

        using (var client = new HttpClient())
        {
            try
            {
                string requestUrl = $"{LocationIqBaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error while making the geocoding request: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                using (JsonDocument document = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = document.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        JsonElement firstResult = root[0];

                        if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
                            firstResult.TryGetProperty("lon", out JsonElement lonElement))
                        {
                            double latitude = double.Parse(latElement.GetString() ?? throw new InvalidOperationException("Latitude is missing."), CultureInfo.InvariantCulture);
                            double longitude = double.Parse(lonElement.GetString() ?? throw new InvalidOperationException("Longitude is missing."), CultureInfo.InvariantCulture);

                            return (latitude, longitude);
                        }
                    }
                }

                throw new Exception("Latitude or Longitude is missing in the response.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving the coordinates of the address.", ex);
            }
        }
    }

    internal static async Task updateCoordinatesForCallAddressAsync(DO.Call call)
    {
        if (call.Address is not null)
        {
            (double latitude, double longitude) = await GetCoordinates(call.Address);
            call = call with { Latitude = latitude, Longitude = longitude };
            lock (AdminManager.BlMutex)
                s_dal.Call.Update(call);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(call.CallId);

        }
    }

    internal static async Task updateCoordinatesForVolunteerAddressAsync(DO.Volunteer volunteer)
    {
        if (volunteer.Address is not null)
        {
            (double latitude, double longitude) = await GetCoordinates(volunteer.Address);
            volunteer = volunteer with { Latitude = latitude, Longitude = longitude };
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(volunteer);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(volunteer.VolunteerId);
        }
    }



namespace Helpers;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using BO;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// The GeocodingHelper class provides a method to retrieve geographical coordinates (latitude and longitude)
/// for a given address using the LocationIQ API.
/// </summary>
/// 
public static class GeocodingService
{
    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1/search.php";
    private const string LocationIqApiKey = "pk.57b6dccaa943970004ae28c88b3506f5";
    //private const string LocationIqApiKey = "pk.675b1b0034c57582409131rpj9f815d";

    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("The provided address is empty or invalid.");
            }

            using (var client = new HttpClient())
            {
                try
                {
                    string requestUrl = $"{LocationIqBaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error while making the geocoding request: {response.StatusCode}");
                    }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                    using (JsonDocument document = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = document.RootElement;
                        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                        {
                            JsonElement firstResult = root[0];

                            if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
                                firstResult.TryGetProperty("lon", out JsonElement lonElement))
                            {
                                double latitude = double.Parse(latElement.GetString() ?? throw new InvalidOperationException("Latitude is missing."), CultureInfo.InvariantCulture);
                                double longitude = double.Parse(lonElement.GetString() ?? throw new InvalidOperationException("Longitude is missing."), CultureInfo.InvariantCulture);

                                return (latitude, longitude);
                            }
                        }
                    }

                    throw new Exception("Latitude or Longitude is missing in the response.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while retrieving the coordinates of the address.", ex);
                }
            }
        }
    }
}
