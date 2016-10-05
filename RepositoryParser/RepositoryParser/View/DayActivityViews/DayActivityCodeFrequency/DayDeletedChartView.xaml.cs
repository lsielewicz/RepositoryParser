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
using RepositoryParser.ViewModel.DayActivityViewModels.DayActivityCodeFrequency;

namespace RepositoryParser.View.DayActivityViews.DayActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for DayDeletedChartView.xaml
    /// </summary>
    public partial class DayDeletedChartView : UserControl
    {
        public DayDeletedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawChart<DayDeletedChartViewModel>(this, this.ChartViewInstance);
            };
        }
    }
}
