using System;
using System.Globalization;
using System.Windows.Data;

namespace RepositoryParser.Conventers
{
    public class IntToLenghtConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                double desiredValue = (double) ((int) value* 40);
                return desiredValue >= 500 ? desiredValue : 500;
            }
            return double.NaN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
