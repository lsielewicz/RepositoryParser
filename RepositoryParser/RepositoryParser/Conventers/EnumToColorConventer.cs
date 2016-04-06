using System;
using System.Globalization;
using System.Windows;
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
                    return (Brush)Application.Current.FindResource("ChangesGreen");
                else if (s == ChangesColorModel.ChangeType.Modified)
                    return (Brush)Application.Current.FindResource("ChangesBlue");
                else if (s == ChangesColorModel.ChangeType.Deleted)
                    return (Brush)Application.Current.FindResource("ChangesRed");
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
