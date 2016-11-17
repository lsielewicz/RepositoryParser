using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.MonthActivityViewModels;

namespace RepositoryParser.View.MonthActivityViews
{
    /// <summary>
    /// Interaction logic for MonthActivityChartView.xaml
    /// </summary>
    public partial class MonthActivityChartView : UserControl
    {
        public MonthActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<MonthActivityChartViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
