using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.HourActivityViewModels;

namespace RepositoryParser.View.HourActivityViews
{
    /// <summary>
    /// Interaction logic for HourActivityFilesAnalyseView.xaml
    /// </summary>
    public partial class HourActivityFilesAnalyseView : UserControl
    {
        public HourActivityFilesAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<HourActivityFilesAnalyseViewModel>(this, this.ChartViewInstance,this.ChartViewInstance2);
        }
    }
}
