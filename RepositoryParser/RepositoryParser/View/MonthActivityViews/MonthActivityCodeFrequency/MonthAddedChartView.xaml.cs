using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.MonthActivityViews.MonthActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for MonthAddedChartView.xaml
    /// </summary>
    public partial class MonthAddedChartView : UserControl
    {
        public MonthAddedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawCharts<CodeFrequencySubChartViewModel>(this,this.ChartViewInstance,this.ChartViewInstance2);
            };
        }
    }
}
