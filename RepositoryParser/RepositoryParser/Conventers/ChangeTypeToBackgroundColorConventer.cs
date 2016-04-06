using System;
using System.Globalization;
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
                    return Brushes.Green;
                else if (s == "Modified")
                    return Brushes.DeepSkyBlue;
                else if (s == "Deleted")
                    return Brushes.Red;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
