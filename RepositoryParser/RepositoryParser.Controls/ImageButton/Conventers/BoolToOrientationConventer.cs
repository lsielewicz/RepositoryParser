﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RepositoryParser.Controls.ImageButton.Conventers
{
    public class BoolToOrientationConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                if ((bool) value == true)
                    return Orientation.Vertical;
            }
            return Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
