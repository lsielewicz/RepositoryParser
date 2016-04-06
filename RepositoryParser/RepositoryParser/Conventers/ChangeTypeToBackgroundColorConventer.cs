using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RepositoryParser.Conventers
{
    public class ChangeTypeToBackgroundColorConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null)
            {
                if (s == "Added")
                    return (Brush)Application.Current.FindResource("ChangesGreen");
                else if (s == "Modified")
                    return (Brush)Application.Current.FindResource("ChangesBlue");
                else if (s == "Deleted")
                    return (Brush)Application.Current.FindResource("ChangesRed");
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
