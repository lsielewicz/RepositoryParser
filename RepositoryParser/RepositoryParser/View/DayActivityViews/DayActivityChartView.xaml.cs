using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.DayActivityViewModels;

namespace RepositoryParser.View.DayActivityViews
{
    /// <summary>
    /// Interaction logic for DayActivityChartView.xaml
    /// </summary>
    public partial class DayActivityChartView : UserControl
    {
        public DayActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<DayActivityViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
