using System.Windows;
using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.UsersActivityViews.UsersActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for UsersCodeFrequencyAddedChartView.xaml
    /// </summary>
    public partial class UsersCodeFrequencyAddedChartView : UserControl
    {
        public UsersCodeFrequencyAddedChartView()
        {
            InitializeComponent();
        }

        private void UsersCodeFrequencyAddedChartView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChartingHelper.Instance.DrawChart<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance);
        }
    }
}
