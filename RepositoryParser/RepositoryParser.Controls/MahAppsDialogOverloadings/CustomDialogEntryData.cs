using System.Windows.Input;
using MahApps.Metro.Controls;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;

namespace RepositoryParser.Controls.MahAppsDialogOverloadings
{
    public class CustomDialogEntryData
    {
        public string DialogTitle { get; set; }
        public string DialogMessage { get; set; }
        public string OkButtonMessage { get; set; }
        public string NoButtonMessage { get; set; }
        public MetroWindow MetroWindow { get; set; }
        public InformationType InformationType { get; set; }
        public ICommand OkCommand { get; set; }
    }
}
