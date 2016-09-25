using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NHibernate.Util;
using RepositoryParser.Controls.Extensions;
using RepositoryParser.ViewModel;

namespace RepositoryParser.View
{
    /// <summary>
    /// Interaction logic for FilteringView.xaml
    /// </summary>
    public partial class FilteringView : UserControl
    {
        public FilteringView()
        {
            var selectedRepositories = new List<string>(ViewModelLocator.Instance.Filtering.SelectedRepositories);
            var selectedAuthors = new List<string>(ViewModelLocator.Instance.Filtering.SelectedAuthors);
            InitializeComponent();
            this.RepositoryList.SelectedItems.Clear();
            this.AuthorComboBox.SelectedItems.Clear();
            selectedRepositories.ForEach(r=>this.RepositoryList.SelectedItems.Add(r));
            var viewModel = this.DataContext as FilteringViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedRepositoriesItemChanged.Execute(viewModel);
                selectedAuthors.ForEach(a => this.AuthorComboBox.SelectedItems.Add(a));
                viewModel.SelectedAuthorsItemChanged.Execute(viewModel);
            }
        }
    }
}
