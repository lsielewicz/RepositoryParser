using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.DayActivityViews.DayActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for DayAddedChartView.xaml
    /// </summary>
    public partial class DayAddedChartView : UserControl
    {
        public DayAddedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawChart<CodeFrequencySubChartViewModel>(this,this.ChartViewInstance);
            };
        }
    }
}
