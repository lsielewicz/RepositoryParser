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
using RepositoryParser.ViewModel.WeekdayActivityViewModels.WeekdayCodeFrequency;

namespace RepositoryParser.View.WeekdayActivityViews.WeekdayActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for WeekdayDeletedChartView.xaml
    /// </summary>
    public partial class WeekdayDeletedChartView : UserControl
    {
        public WeekdayDeletedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawChart<WeekdayDeletedChartViewModel>(this, this.ChartViewInstance);
            };
        }
    }
}
