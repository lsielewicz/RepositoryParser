using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.MonthActivityViewModels;

namespace RepositoryParser.View.MonthActivityViews
{
    /// <summary>
    /// Interaction logic for MonthActivityFilesAnalyseView.xaml
    /// </summary>
    public partial class MonthActivityFilesAnalyseView : UserControl
    {
        public MonthActivityFilesAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<MonthActivityFilesAnalyseViewModel>(this,this.ChartViewInstance);
        }
    }
}
