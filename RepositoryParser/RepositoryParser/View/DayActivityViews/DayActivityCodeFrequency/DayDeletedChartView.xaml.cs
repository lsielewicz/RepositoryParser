using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.DayActivityViews.DayActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for DayDeletedChartView.xaml
    /// </summary>
    public partial class DayDeletedChartView : UserControl
    {
        public DayDeletedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawChart<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance);
            };
        }
    }
}
