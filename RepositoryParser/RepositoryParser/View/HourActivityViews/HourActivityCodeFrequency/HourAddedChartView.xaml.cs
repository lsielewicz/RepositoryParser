using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.HourActivityViews.HourActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for HourAddedChartView.xaml
    /// </summary>
    public partial class HourAddedChartView : UserControl
    {
        public HourAddedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawChart<CodeFrequencySubChartViewModel>(this, this.ChartViewInstance);
            };
        }
    }
}
