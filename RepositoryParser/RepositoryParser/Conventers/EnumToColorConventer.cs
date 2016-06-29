using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using RepositoryParser.Core.Enums;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Conventers
{
    class EnumToColorConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (ChangeType)value ;
              
                if (s == ChangeType.Added)
                    return (Brush)Application.Current.FindResource("ChangesGreen");
                else if (s == ChangeType.Modified)
                    return (Brush)Application.Current.FindResource("ChangesBlue");
                else if (s == ChangeType.Deleted)
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
