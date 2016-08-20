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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace RepositoryParser.View
{
    /// <summary>
    /// Interaction logic for UndockedPresentationWindowView.xaml
    /// </summary>
    public partial class UndockedPresentationWindowView : MetroWindow
    {
        public PresentationView ParentView { get; set; }
        public UndockedPresentationWindowView(PresentationView parentView)
        {
            this.ParentView = parentView;
            InitializeComponent();
        }
    }
}
