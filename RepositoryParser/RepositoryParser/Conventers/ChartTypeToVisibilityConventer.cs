using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using RepositoryParser.Core.Enum;

namespace RepositoryParser.Conventers
{
    public class ChartTypeToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter is ChartType && value is ChartType)
            {
                var currentType = (ChartType) value;
                var eParameter = (ChartType) parameter;
                return currentType == eParameter ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
