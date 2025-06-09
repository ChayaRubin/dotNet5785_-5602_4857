using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using BO;

namespace PL.Call
{
    public class CanCancelAssignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInList call)
            {
                // Can cancel if there's an active (not completed) assignment
                return (call.Assignments?.Any(a => a.CompletionTime == null) == true)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
