using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using RepositoryParser.Controls.Common;

namespace RepositoryParser.Conventers
{
    public class IntegerToBooleanConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                if (parameter != null && parameter is ConventerDirection)
                {
                    var eParameter = (ConventerDirection) parameter;
                    if (eParameter == ConventerDirection.Inverse)
                    {
                        return (int) value == 0;
                    }
                }
                return (int)value != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
