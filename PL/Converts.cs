//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Data;

//namespace PL;
///// <summary>
///// A class that converts the Id to a string
///// </summary>
//class ConvertIdToContent : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        return (int)value == 0 ? "Add" : "Update";
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using BO; // Make sure this is your enum namespace

namespace PL
{
    /// <summary>
    /// A class that converts the Id to a string
    /// </summary>
    class ConvertIdToContent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? "Add" : "Update";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts CallTypeEnum to a friendly string with spaces
    /// </summary>
    class CallTypeEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallTypeEnum callType)
            {
                switch (callType)
                {
                    case CallTypeEnum.Urgent:
                        return "Urgent";
                    case CallTypeEnum.Medium_Urgency:
                        return "Medium Urgency";
                    case CallTypeEnum.General_Assistance:
                        return "General Assistance";
                    case CallTypeEnum.Non_Urgent:
                        return "Non Urgent";
                    case CallTypeEnum.None:
                        return "None";
                    default:
                        return "Unknown";
                }
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts CallTypeEnum to background color brush
    /// </summary>
    class CallTypeEnumToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallTypeEnum callType)
            {
                switch (callType)
                {
                    case CallTypeEnum.Urgent:
                        return new SolidColorBrush(Colors.Red);
                    case CallTypeEnum.Medium_Urgency:
                        return new SolidColorBrush(Colors.Orange);
                    case CallTypeEnum.General_Assistance:
                        return new SolidColorBrush(Colors.LightBlue);
                    case CallTypeEnum.Non_Urgent:
                        return new SolidColorBrush(Colors.LightGreen);
                    case CallTypeEnum.None:
                        return new SolidColorBrush(Colors.Gray);
                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

