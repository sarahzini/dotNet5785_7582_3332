namespace Helpers;
using System;
using System.Net.Http;
using BO;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// The GeocodingHelper class provides a method to retrieve geographical coordinates (latitude and longitude)
/// for a given address using the LocationIQ API.
/// </summary>
public class GeocodingService
{
    // The URL of the LocationIQ API
    private const string BaseUrl = "https://eu1.locationiq.com/v1/search.php";

    // our API key
    private const string ApiKey = "675b1b0034c57582409131rpj9f815d";
    public static (double latitude, double longitude) GetCoordinates(string address)
    {
        using (var client = new HttpClient())
        {
            // Build the URL for the API request
            var url = $"{BaseUrl}?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";

            try
            {
                // Send the request
                var response = client.GetStringAsync(url).Result;

                // If the response is empty, throw an exception
                if (string.IsNullOrEmpty(response))
                {
                    throw new BLFormatException("The Adress is empty.");
                }

                // Parse the response into JSON format
                var jsonResponse = JArray.Parse(response);

                // If no results are returned, throw an exception
                if (jsonResponse.Count == 0)
                {
                    throw new BLFormatException("The address is invalid or not found.");
                }

                // Extract the coordinates (latitude and longitude) from the response
                var latitude = double.Parse(jsonResponse[0]["lat"].ToString());
                var longitude = double.Parse(jsonResponse[0]["lon"].ToString());

                return (latitude, longitude);
            }
            catch (Exception ex)
            {
                // If an error occurs, throw an exception with the error details
                throw new BLFormatException("An error occurred while retrieving coordinates for the address: " + ex.Message);
            }
        }
    }
}
