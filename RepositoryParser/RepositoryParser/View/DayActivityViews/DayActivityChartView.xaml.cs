using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.DayActivityViewModels;

namespace RepositoryParser.View.DayActivityViews
{
    /// <summary>
    /// Interaction logic for DayActivityChartView.xaml
    /// </summary>
    public partial class DayActivityChartView : UserControl
    {
        public DayActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<DayActivityViewModel>(this, this.StackedColumnChartInstance);
        }
    }
}
