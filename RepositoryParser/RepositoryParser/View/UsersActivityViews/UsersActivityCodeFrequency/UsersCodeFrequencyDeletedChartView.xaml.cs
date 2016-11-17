using System.Windows;
using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

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
            ChartingHelper.Instance.DrawCharts<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
