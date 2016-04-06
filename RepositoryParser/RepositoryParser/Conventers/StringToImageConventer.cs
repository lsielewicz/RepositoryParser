using System;
using System.Globalization;
using System.Windows.Data;

namespace RepositoryParser.Conventers
{
    public class StringToImageConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if((string)value == "GIT")
                    return new Uri("../Assets/Images/git_logo_grey.png", UriKind.Relative);
                else if ((string) value == "SVN")
                    return new Uri("../Assets/Images/svn_logo_grey.png",UriKind.Relative);
            }
            return new Uri("../Assets/Images/transparent", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
