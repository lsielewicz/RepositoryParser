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
