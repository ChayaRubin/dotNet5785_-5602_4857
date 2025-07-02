using System.Collections;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;

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

        private const string LOCATIONIQ_API_KEY = "pk.47640146141430"; // Replace with your LocationIQ API key
        private const string GEOCODING_API_URL = "https://us1.locationiq.com/v1/search.php";

        /// <summary>
        /// Gets coordinates for a given address using LocationIQ geocoding service
        /// </summary>
        /// <param name="address">The address to geocode</param>
        /// <returns>Tuple containing latitude and longitude</returns>
        /// <exception cref="ArgumentException">Thrown when address is null or empty</exception>
        /// <exception cref="InvalidOperationException">Thrown when geocoding fails or returns invalid results</exception>
        public static (double latitude, double longitude) GetCoordinatesFromAddress(string address)
        {
            string apiKey = "pk.2c8ee1e2c0361af3c52d5073f44ab9ef";
            using var client = new HttpClient();
            string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

            var response = client.GetAsync(url).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new Exception("Invalid address or API error.");

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
                throw new Exception("Address not found.");

            var root = doc.RootElement[0];

            double latitude = double.Parse(root.GetProperty("lat").GetString());
            double longitude = double.Parse(root.GetProperty("lon").GetString());
            Console.WriteLine($"Latitude-1: {latitude}, Longitude-1: {longitude}");
            return (latitude, longitude);
        }

        /// <summary>
        /// מחשבת את המרחק בין שתי נקודות גיאוגרפיות.
        /// </summary>
        public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
        {
            if (lat1 == null || lon1 == null)
                throw new ArgumentException("Latitude or Longitude values are null.");

            const double R = 6371; // רדיוס כדור הארץ בקילומטרים
            double lat1Value = lat1.Value;
            double lon1Value = lon1.Value;
            double dLat = (lat2 - lat1Value) * Math.PI / 180;
            double dLon = (lon2 - lon1Value) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
