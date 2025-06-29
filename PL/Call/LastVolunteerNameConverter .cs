using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BlApi;
using BO;

namespace PL.Call
{
    public class LastVolunteerNameConverter : IValueConverter
    {
        private readonly IBl bl = Factory.Get(); 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<CallAssignInList> list)
            {
                // למצוא הקצאה אחרונה שבה היה VolunteerId
                var last = list.LastOrDefault(a => a.VolunteerId != null);
                if (last?.VolunteerId is int id)
                {
                    try
                    {
                        return bl.Volunteer.GetVolunteerDetails(id.ToString()).FullName;
                    }
                    catch
                    {
                        return "[Unknown Volunteer]";
                    }
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
