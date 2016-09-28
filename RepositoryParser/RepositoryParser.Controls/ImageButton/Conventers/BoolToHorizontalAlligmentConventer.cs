using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RepositoryParser.Controls.ImageButton.Conventers
{
    public class BoolToHorizontalAlligmentConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                if ((bool)value == true)
                    return HorizontalAlignment.Center;
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
