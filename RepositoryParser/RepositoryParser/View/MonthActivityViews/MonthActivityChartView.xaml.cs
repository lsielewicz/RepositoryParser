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
using RepositoryParser.ViewModel.HourActivityViewModels;
using RepositoryParser.ViewModel.MonthActivityViewModels;

namespace RepositoryParser.View.MonthActivityViews
{
    /// <summary>
    /// Interaction logic for MonthActivityChartView.xaml
    /// </summary>
    public partial class MonthActivityChartView : UserControl
    {
        public MonthActivityChartView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<MonthActivityChartViewModel>(this, this.StackedColumnChartInstance);
        }
    }
}
