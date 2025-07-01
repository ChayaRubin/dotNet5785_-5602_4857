using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Call
{
    public class CallTimeLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan.TotalSeconds < 0)
                    return "Expired";

                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                    timeSpan.Days,
                    timeSpan.Hours,
                    timeSpan.Minutes,
                    timeSpan.Seconds);
            }

            return "N/A";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
