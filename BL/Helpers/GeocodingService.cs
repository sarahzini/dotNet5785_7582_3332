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
    //private const string LocationIqApiKey = "675b1b0034c57582409131rpj9f815d";

    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BLFormatException("The provided address is empty or invalid.");
        }

        using (var client = new HttpClient())
        {
            try
            {
                string requestUrl = $"{LocationIqBaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";
                HttpResponseMessage response = client.GetAsync(requestUrl).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error while making the geocoding request: {response.StatusCode}");
                }

                string jsonResponse = response.Content.ReadAsStringAsync().Result;


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
                // If an error occurs, throw an exception with the error details
                throw new BLFormatException("An error occurred while retrieving coordinates for the address: " + ex.Message);
            }
        }
    }
}
