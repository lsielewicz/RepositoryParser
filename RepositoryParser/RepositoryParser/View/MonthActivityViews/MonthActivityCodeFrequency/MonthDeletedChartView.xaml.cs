using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.MonthActivityViews.MonthActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for MonthDeletedChartView.xaml
    /// </summary>
    public partial class MonthDeletedChartView : UserControl
    {
        public MonthDeletedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawCharts<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
            };
        }
    }
}
