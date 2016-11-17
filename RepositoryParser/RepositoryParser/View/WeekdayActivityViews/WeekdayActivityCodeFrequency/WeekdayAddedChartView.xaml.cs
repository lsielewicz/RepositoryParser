using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.WeekdayActivityViews.WeekdayActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for WeekdayAddedChartView.xaml
    /// </summary>
    public partial class WeekdayAddedChartView : UserControl
    {
        public WeekdayAddedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawCharts<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
            };
        }
    }
}
