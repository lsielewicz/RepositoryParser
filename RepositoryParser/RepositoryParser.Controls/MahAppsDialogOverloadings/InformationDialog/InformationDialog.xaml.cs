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
using MahApps.Metro.Controls.Dialogs;

namespace RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog
{
    /// <summary>
    /// Interaction logic for InformationDialog.xaml
    /// </summary>
    public partial class InformationDialog : CustomDialog
    {
        public InformationDialog(CustomDialogEntryData data)
        {
            InitializeComponent();
            this.DataContext = new InformationDialogViewModel(data);
        }
    }
}
