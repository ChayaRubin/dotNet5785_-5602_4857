using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Call
{
    public class AddressToGoogleMapsLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string address && !string.IsNullOrWhiteSpace(address))
            {
                return $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(address)}";
            }
            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
