using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RepositoryParser.Controls.Extensions
{
    public class ListViewExtensions : DependencyObject
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(ListViewExtensions),
                new FrameworkPropertyMetadata((IList)null,
                    OnSelectedItemsChanged));

        public static IList GetSelectedItems(UIElement element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(UIElement element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Handles changes to the SelectedItems property.
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = (ListView)d;
            var viewModelList = (IList)e.NewValue;
            if (e.OldValue != null && viewModelList != null && viewModelList.Count == 0)
            {
                listView.SelectedItems.Clear();
            }
            ReSetSelectedItems(listView);
            listView.SelectionChanged += delegate
            {
                ReSetSelectedItems(listView);
            };
        }

        private static void ReSetSelectedItems(ListView listView)
        {
            IList selectedItems = GetSelectedItems(listView);
            if(selectedItems.Count > 0)
                selectedItems.Clear();
            if (listView.SelectedItems != null)
            {
                foreach (var item in listView.SelectedItems)
                    selectedItems.Add(item);
            }
        }
    }
}
