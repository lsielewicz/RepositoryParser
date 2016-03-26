using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using RepositoryParser.Properties;

namespace RepositoryParser.Conventers
{
    public class BoolToBackgroundColorConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool) value == true)
                {
                    return (Brush)Application.Current.FindResource("GitOrange");
                }
                else
                {
                    return (Brush) Application.Current.FindResource("SvnBlue");
                } 
            }
            return Brushes.Transparent;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
