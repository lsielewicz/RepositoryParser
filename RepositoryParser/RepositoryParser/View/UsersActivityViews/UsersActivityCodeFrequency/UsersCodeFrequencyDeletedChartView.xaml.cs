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
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.UserActivityViewModels.UsersActivityCodeFrequency;

namespace RepositoryParser.View.UsersActivityViews.UsersActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for UsersCodeFrequencyDeletedChartView.xaml
    /// </summary>
    public partial class UsersCodeFrequencyDeletedChartView : UserControl
    {
        public UsersCodeFrequencyDeletedChartView()
        {
            InitializeComponent();
        }

        private void UsersCodeFrequencyDeletedChartView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChartingHelper.Instance.DrawChart<UsersDeletedChartViewModel>(this, this.ChartViewInstance);
        }
    }
}
