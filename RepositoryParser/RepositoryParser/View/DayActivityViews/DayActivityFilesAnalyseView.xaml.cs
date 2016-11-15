using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.DayActivityViewModels;

namespace RepositoryParser.View.DayActivityViews
{
    /// <summary>
    /// Interaction logic for DayActivityFilesAnalyseView.xaml
    /// </summary>
    public partial class DayActivityFilesAnalyseView : UserControl
    {
        public DayActivityFilesAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<DayActivityFilesAnalyseViewModel>(this, this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
