using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BO;

namespace PL.Call
{
    public class CallTreatmentDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInList call && call.Assignments != null)
            {
                var treated = call.Assignments.LastOrDefault(a => a.EndType == CallStatus.Treated);
                if (treated?.CompletionTime != null)
                {
                    TimeSpan duration = treated.CompletionTime.Value - call.OpenTime;
                    return $"{(int)duration.TotalMinutes} min";
                }
            }

            return "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
