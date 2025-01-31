using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Text.Json;

namespace Helpers
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Tools
    {
        public static string ToStringProperty<T>(this T t)
        {
            if (t == null) return "null"; // אם האובייקט הוא null, מחזירים מחרוזת "null"

            var stringBuilder = new StringBuilder();
            var properties = t.GetType().GetProperties();

            foreach (var property in properties)
            {
                // אם התכונה היא אוסף/רשימה
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.GetValue(t) != null)
                {
                    var collection = (IEnumerable)property.GetValue(t);
                    stringBuilder.AppendLine($"{property.Name}: [ {string.Join(", ", collection)} ]");
                }
                else
                {
                    var value = property.GetValue(t);
                    stringBuilder.AppendLine($"{property.Name}: {value}");
                }
            }

            return stringBuilder.ToString(); // מחזירים את המחרוזת שהורכבה
        }

        private const string LOCATIONIQ_API_KEY = "your_api_key_here"; // Replace with your LocationIQ API key
        private const string GEOCODING_API_URL = "https://us1.locationiq.com/v1/search.php";

        /// <summary>
        /// Gets coordinates for a given address using LocationIQ geocoding service
        /// </summary>
        /// <param name="address">The address to geocode</param>
        /// <returns>Tuple containing latitude and longitude</returns>
        /// <exception cref="ArgumentException">Thrown when address is null or empty</exception>
        /// <exception cref="InvalidOperationException">Thrown when geocoding fails or returns invalid results</exception>
        internal static (double latitude, double longitude) GetCoordinatesFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be null or empty", nameof(address));
            }

            try
            {
                using (var client = new HttpClient())
                {
                    // Build the API request URL
                    var requestUrl = $"{GEOCODING_API_URL}?key={LOCATIONIQ_API_KEY}&q={Uri.EscapeDataString(address)}&format=json";

                    // Make synchronous request (no async/await)
                    var response = client.GetStringAsync(requestUrl).Result;  // Here we use .Result to make it synchronous

                    // Parse JSON response
                    using (JsonDocument document = JsonDocument.Parse(response))
                    {
                        // LocationIQ returns an array of results, we take the first one
                        var firstResult = document.RootElement.EnumerateArray().First();

                        var lat = firstResult.GetProperty("lat").GetString();
                        var lon = firstResult.GetProperty("lon").GetString();

                        if (double.TryParse(lat, out double latitude) &&
                            double.TryParse(lon, out double longitude))
                        {
                            return (latitude, longitude);
                        }

                        throw new InvalidOperationException("Failed to parse coordinates from API response");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to geocode address: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse geocoding response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error during geocoding: {ex.Message}", ex);
            }
        }


    }

}
