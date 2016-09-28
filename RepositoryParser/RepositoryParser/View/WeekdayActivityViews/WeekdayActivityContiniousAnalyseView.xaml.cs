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
using RepositoryParser.ViewModel.WeekdayActivityViewModels;

namespace RepositoryParser.View.WeekdayActivityViews
{
    /// <summary>
    /// Interaction logic for WeekdayActivityContiniousAnalyseView.xaml
    /// </summary>
    public partial class WeekdayActivityContiniousAnalyseView : UserControl
    {
        public WeekdayActivityContiniousAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<WeekdayActivityContiniousAnalyseViewModel>(this,this.ChartViewInstance);
        }
    }
}
