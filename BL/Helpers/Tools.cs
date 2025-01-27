using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal class Tools
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


    }
}
