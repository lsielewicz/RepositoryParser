using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.MonthActivityViewModels;

namespace RepositoryParser.View.MonthActivityViews
{
    /// <summary>
    /// Interaction logic for MonthActivityContiniousAnalyseView.xaml
    /// </summary>
    public partial class MonthActivityContiniousAnalyseView : UserControl
    {
        public MonthActivityContiniousAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<MonthActivityContiniousAnalyseViewModel>(this,this.ChartViewInstance,this.ChartViewInstance2);
        }
    }
}
