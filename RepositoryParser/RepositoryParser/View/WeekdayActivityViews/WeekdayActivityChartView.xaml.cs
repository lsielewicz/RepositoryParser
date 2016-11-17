using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.WeekdayActivityViewModels;

namespace RepositoryParser.View.WeekdayActivityViews
{
    /// <summary>
    /// Interaction logic for WeekdayActivityChartView.xaml
    /// </summary>
    public partial class WeekdayActivityChartView : UserControl
    {
        public WeekdayActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<WeekDayActivityViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
