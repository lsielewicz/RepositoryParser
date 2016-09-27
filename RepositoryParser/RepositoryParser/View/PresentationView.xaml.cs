using System.Windows.Controls;
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
                viewModel.ViewInstance = this;
                if (!viewModel.IsDocked)
                {
                    this.RootGrid.Children.Remove(this.PresentationGrid);
                }
            }
        }
    }
}
