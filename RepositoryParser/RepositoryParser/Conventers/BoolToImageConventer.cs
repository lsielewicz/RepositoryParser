using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RepositoryParser.Conventers
{
    public class BoolToImageConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                if ((bool)value)
                    return new Uri("../Assets/Images/git_logo_grey.png", UriKind.Relative);
                else
                    return new Uri("../Assets/Images/svn_logo_grey.png", UriKind.Relative);
            }
            return new Uri("../Assets/Images/transparent", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
