using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;
using BO;

namespace PL.Call
{
    public class AssignmentCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<CallAssignInList> list)
                return list.Count.ToString();
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
