namespace Helpers;
using System.Text.Json;


public static class GeocodingService
{
    private static readonly HttpClient HttpClient = new HttpClient();

    private const string GoogleApiKey = "AIzaSyCuGWKseIQvrkb9Yk3U14e_9K9pltkSwug"; // API key

    /// <summary>
    /// Gets the coordinates (latitude, longitude) for a given address.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <returns>A tuple containing latitude and longitude.</returns>
    /// <exception cref="ArgumentException">Thrown when the address is null or empty.</exception>
    /// <exception cref="Exception">Thrown when the API response is invalid or unsuccessful.</exception>
    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The address cannot be null or empty.", nameof(address));
        }

        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={GoogleApiKey}";

        var response = await HttpClient.GetStringAsync(url);
        var geocodeResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(response);

        if (geocodeResponse == null || geocodeResponse.Results.Length == 0)
        {
            throw new Exception($"Address not found or invalid response from Google Maps API: {geocodeResponse?.Status}");
        }

        var location = geocodeResponse.Results[0].Geometry.Location;
        return (location.Lat, location.Lng);
    }
    /// <summary>
    /// Represents a response from the Google Geocoding API.
    /// </summary>
    public class GoogleGeocodeResponse
    {
        public string Status { get; set; }
        public GoogleGeocodeResult[] Results { get; set; }
    }

    /// <summary>
    /// Represents a single result from the Google Geocoding API.
    /// </summary>
    public class GoogleGeocodeResult
    {
        public GoogleGeometry Geometry { get; set; }
    }

    /// <summary>
    /// Represents the geometry object in the Google Geocoding API response.
    /// </summary>
    public class GoogleGeometry
    {
        public GoogleLocation Location { get; set; }
    }

    /// <summary>
    /// Represents the location object in the Google Geocoding API response.
    /// </summary>
    public class GoogleLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

}