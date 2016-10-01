using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using RepositoryParser.Configuration;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel
{
    public class SettingsViewModel : RepositoryAnalyserViewModelBase
    {

        private bool _isInitialized;
        private string _selectedLangauge;
        private string _currentRepositorySavingPath;
        private RelayCommand _restartApplicationCommand;
        private RelayCommand _openRepositoriesDirectory;
        private RelayCommand _openDirectoryFilePicker;

        public SettingsViewModel()
        {
            if (ConfigurationService.Instance.Configuration.CurrentLanguage == ApplicationLanguage.Polish)
                SelectedLanguage = this.GetLocalizedString("Polish");
            if (ConfigurationService.Instance.Configuration.CurrentLanguage == ApplicationLanguage.English)
                SelectedLanguage = this.GetLocalizedString("English");

            CurrentRepositorySavingPath = ConfigurationService.Instance.Configuration.SavingRepositoryPath;

            _isInitialized = true;
        }

        public string CurrentRepositorySavingPath
        {
            get { return _currentRepositorySavingPath; }
            set
            {
                if (_currentRepositorySavingPath == value)
                    return;
                _currentRepositorySavingPath = value;
                if (_isInitialized && ZetaLongPaths.ZlpIOHelper.DirectoryExists(_currentRepositorySavingPath))
                {
                    ConfigurationService.Instance.Configuration.SavingRepositoryPath = _currentRepositorySavingPath;
                    ConfigurationService.Instance.SaveChanges();
                }

                RaisePropertyChanged();
            }
        }
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

        public RelayCommand OpenRepositoriesDirectoryCommand
        {
            get
            {
                return _openRepositoriesDirectory ?? (_openRepositoriesDirectory = new RelayCommand(() =>
                {
                    Process.Start(ConfigurationService.Instance.Configuration.SavingRepositoryPath);
                }));
            }
        }

        public RelayCommand OpenDirectoryFilePicker
        {
            get
            {
                return _openDirectoryFilePicker ?? (_openDirectoryFilePicker = new RelayCommand(() =>
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                    fbd.Description = this.GetLocalizedString("PickFolderWithRepo");

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        this.CurrentRepositorySavingPath = fbd.SelectedPath;
                    }
                }));
            }
        }

    }
}
