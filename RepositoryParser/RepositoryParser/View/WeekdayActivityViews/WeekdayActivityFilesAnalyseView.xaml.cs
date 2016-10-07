using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.WeekdayActivityViewModels;

namespace RepositoryParser.View.WeekdayActivityViews
{
    /// <summary>
    /// Interaction logic for WeekdayActivityFilesAnalyseView.xaml
    /// </summary>
    public partial class WeekdayActivityFilesAnalyseView : UserControl
    {
        public WeekdayActivityFilesAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<WeekdayActivityFilesAnalyseViewModel>(this, this.ChartViewInstance);
        }
    }
}
