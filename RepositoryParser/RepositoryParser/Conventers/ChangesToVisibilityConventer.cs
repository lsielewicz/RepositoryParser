﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Conventers
{
    public class ChangesToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || String.IsNullOrEmpty(((Changes)value).Path))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
