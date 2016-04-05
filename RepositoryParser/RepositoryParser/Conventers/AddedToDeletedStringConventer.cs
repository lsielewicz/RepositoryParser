using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RepositoryParser.Conventers
{
    public class AddedToDeletedStringConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue == "Deleted")
                    return "Deleted";
                else if (stringValue == "Added")
                    return "Added";
                else
                {
                    return value;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
