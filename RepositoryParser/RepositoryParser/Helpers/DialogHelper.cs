using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.ViewModel;

namespace RepositoryParser.Helpers
{
    public class DialogHelper
    {
        private static DialogHelper _instance;

        public static DialogHelper Instance
        {
            get { return _instance ?? (_instance = new DialogHelper()); }
        }

        private DialogHelper()
        {
            _dialogCoordinator = DialogCoordinator.Instance;
        }

        private readonly IDialogCoordinator _dialogCoordinator;

        public async Task ShowDialog(CustomDialogEntryData data)
        {
            await _dialogCoordinator.ShowMetroDialogAsync(ViewModelLocator.Instance.Main, new InformationDialog(data));
        }
    }
}
