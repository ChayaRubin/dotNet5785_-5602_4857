using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Call
{
    public class CallTimeLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime endTime)
            {
                TimeSpan remaining = endTime - DateTime.Now;
                return remaining.TotalSeconds < 0 ? "Expired" : $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
