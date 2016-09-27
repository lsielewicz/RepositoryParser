using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.UserActivityViewModels;

namespace RepositoryParser.View.UsersActivityViews
{
    /// <summary>
    /// Interaction logic for UserActivityChartView.xaml
    /// </summary>
    public partial class UserActivityChartView : UserControl
    {
        public UserActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<UsersActivityViewModel>(this, this.StackedColumnChartInstance);
        }
    }
}
