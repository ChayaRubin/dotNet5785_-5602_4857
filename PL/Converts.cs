using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;

namespace PL
{
    /// <summary>
    /// Converts CallTypeEnum to a friendly string with spaces
    /// </summary>
    public class CallTypeEnumToStringConverter : IValueConverter
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
}

