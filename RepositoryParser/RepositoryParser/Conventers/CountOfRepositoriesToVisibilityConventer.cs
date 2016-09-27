using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RepositoryParser.Helpers.Enums;

namespace RepositoryParser.Conventers
{
    public class CountOfRepositoriesToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                var intValue = (int)value;
                if (parameter != null && parameter is VisibilityConventerEnumDirection && (VisibilityConventerEnumDirection)parameter == VisibilityConventerEnumDirection.Inverse)
                {
                    if (intValue == 0)
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                }
                if (intValue == 0)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
