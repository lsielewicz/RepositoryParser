using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using RepositoryParser.Controls.Common;

namespace RepositoryParser.Controls.MetroLoadingProgressRing.Conventers
{
    public class BooleanToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                if (parameter != null && parameter is ConventerDirection && (ConventerDirection)parameter == ConventerDirection.Inverse)
                {
                        return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }
                return (bool) value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
