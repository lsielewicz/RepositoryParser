using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Conventers
{
    class EnumToColorConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (ChangesColorModel.ChangeType)value ;
              
                    if (s == ChangesColorModel.ChangeType.Added)
                        return Brushes.Green;
                    else if (s == ChangesColorModel.ChangeType.Modified)
                        return Brushes.DeepSkyBlue;
                    else if (s == ChangesColorModel.ChangeType.Deleted)
                        return Brushes.Red;
                    else 
                        return Brushes.Transparent;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
