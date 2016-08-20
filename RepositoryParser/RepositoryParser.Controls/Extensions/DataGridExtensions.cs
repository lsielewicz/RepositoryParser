using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RepositoryParser.Controls.Extensions
{
    public class DataGridExtensions
    {
        public static readonly DependencyProperty LastColumnFillProperty = DependencyProperty.RegisterAttached("LastColumnFill", typeof(bool), typeof(DataGridExtensions), new PropertyMetadata(default(bool), OnLastColumnFillChanged));

        public static void SetLastColumnFill(DataGrid element, bool value)
        {
            element.SetValue(LastColumnFillProperty, value);
        }

        public static bool GetLastColumnFill(DataGrid element)
        {
            return (bool)element.GetValue(LastColumnFillProperty);
        }

        private static void OnLastColumnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null) return;

            dataGrid.Loaded -= OnDataGridLoaded;
            dataGrid.Loaded += OnDataGridLoaded;
        }

        private static void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            var lastColumn = dataGrid.Columns.LastOrDefault();
            if (lastColumn != null)
                lastColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            // Autofit all other columns
            foreach (var column in dataGrid.Columns)
            {
                if (column == lastColumn) break;

                double beforeWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);
                double sizeCellsWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
                double sizeHeaderWidth = column.ActualWidth;
                column.MinWidth = Math.Max(beforeWidth, Math.Max(sizeCellsWidth, sizeHeaderWidth));
            }
        }
    }
}
