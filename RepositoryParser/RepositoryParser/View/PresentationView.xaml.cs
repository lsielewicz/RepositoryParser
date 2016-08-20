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
using RepositoryParser.ViewModel;

namespace RepositoryParser.View
{
    /// <summary>
    /// Interaction logic for PresentationView.xaml
    /// </summary>
    public partial class PresentationView : UserControl
    {

        public PresentationView()
        {
            InitializeComponent();
            PresentationViewModel viewModel = DataContext as PresentationViewModel;
            if (viewModel != null)
            {
                if (!viewModel.IsDocked)
                {
                    this.RootGrid.Children.Remove(this.PresentationGrid);
                }
            }
        }
    }
}
