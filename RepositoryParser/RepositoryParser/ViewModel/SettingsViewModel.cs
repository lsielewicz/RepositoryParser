using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using RepositoryParser.Configuration;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel
{
    public class SettingsViewModel : RepositoryAnalyserViewModelBase
    {

        public SettingsViewModel()
        {
            if (ConfigurationService.Instance.Configuration.CurrentLanguage == ApplicationLanguage.Polish)
                SelectedLanguage = this.GetLocalizedString("Polish");
            if (ConfigurationService.Instance.Configuration.CurrentLanguage == ApplicationLanguage.English)
                SelectedLanguage = this.GetLocalizedString("English");
            _isInitialized = true;
        }

        private bool _isInitialized;
        private string _selectedLangauge;
        private RelayCommand _restartApplicationCommand;

        public string SelectedLanguage
        {
            get { return _selectedLangauge; }
            set
            {
                if (_selectedLangauge == value)
                    return;
                _selectedLangauge = value;
                if(_isInitialized)
                    SelectedLanguageItemChanged(_selectedLangauge);
                this.RaisePropertyChanged();
            }
        }

        private async void SelectedLanguageItemChanged(string item)
        {
            if (item == this.GetLocalizedString("Polish"))
                ConfigurationService.Instance.Configuration.CurrentLanguage = ApplicationLanguage.Polish;
            if (item == this.GetLocalizedString("English"))
                ConfigurationService.Instance.Configuration.CurrentLanguage = ApplicationLanguage.English;

            ConfigurationService.Instance.SaveChanges();

            await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
            {
                MetroWindow = StaticServiceProvider.MetroWindowInstance,
                InformationType = InformationType.Information,
                DialogMessage = this.GetLocalizedString("RestartRequired"),
                DialogTitle = this.GetLocalizedString("Information"),
                OkButtonMessage = "Restart",
                OkCommand = this.RestartApplicationCommand
            });

            //this.RestartApplicationCommand.Execute(this);
        }

        public RelayCommand RestartApplicationCommand
        {
            get
            {
                return _restartApplicationCommand ?? (_restartApplicationCommand = new RelayCommand(() =>
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }));
            }
        }
    }
}
