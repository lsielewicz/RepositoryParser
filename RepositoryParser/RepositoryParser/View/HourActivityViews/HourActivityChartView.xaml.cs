using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.HourActivityViewModels;

namespace RepositoryParser.View.HourActivityViews
{
    /// <summary>
    /// Interaction logic for HourActivityChartView.xaml
    /// </summary>
    public partial class HourActivityChartView : UserControl
    {
        public HourActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<HourActivityViewModel>(this,this.ChartViewInstance,this.ChartViewInstance2);
        }
    }
}
