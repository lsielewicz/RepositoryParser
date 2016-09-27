using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RepositoryParser.Conventers
{
    public class MultiBooleanAndIntegerToVisibilityConvnenter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = true;
            foreach (var value in values)
            {
                if (value is bool)
                    visible = visible && (bool) value;
                else if (value is int)
                {
                    if ((int) value == 0)
                        visible = false;
                }
            }
            if (visible)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
