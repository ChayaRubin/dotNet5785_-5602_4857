using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;

namespace PL.Call
{
    public class CanDeleteCallConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInList call)
            {
                return (call.Status == CallStatus.Open && (call.Assignments?.Count ?? 0) == 0)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

