using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Configuration;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.Core.Enum;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel
{
    public class DataBaseManagementViewModel : RepositoryAnalyserViewModelBase
    {
        #region private variables

        private IVersionControlFilePersister _repositoryFilePersister;
        private string _urlTextBox;
        private bool _isCloneButtonEnabled = true;
        private bool _isLocal;
        private bool _isOpening;
        private bool _isGitRepositoryPicked;
        private readonly BackgroundWorker _clearDbWorker;
        private RelayCommand _startWorkCommand;
        private RelayCommand _asyncClearDbCommand;
        private RelayCommand _pickFileCommand;
        private RelayCommand _pickGitRepositoryCommand;
        private RelayCommand _pickSvnRepositoryCommand;
        #endregion

        private readonly IDialogCoordinator _dialogCoordinator;
/*        private readonly DialogView _dialogView = new DialogView();*/
        public DataBaseManagementViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            this._clearDbWorker = new BackgroundWorker();
            this._clearDbWorker.DoWork += this.DoClearWork;
            this._clearDbWorker.RunWorkerCompleted += this.DoClearWorkCompleted;
        }

        #region Getters/Setters
        public bool IsGitRepositoryPicked
        {
            get { return _isGitRepositoryPicked; }
            set
            {
                if (_isGitRepositoryPicked != value)
                {
                    _isGitRepositoryPicked = value;
                    RaisePropertyChanged("IsGitRepositoryPicked");
                }
            }
        }

        public bool IsOpening
        {
            get
            {
                return _isOpening;
            }
            set
            {
                if (_isOpening != value)
                {
                    _isOpening = value;
                    RaisePropertyChanged("IsOpening");
                }
            }
        }

        public string UrlTextBox
        {
            get
            {
                return _urlTextBox;
            }
            set
            {
                if (_urlTextBox != value)
                {
                    _urlTextBox = value;
                    RaisePropertyChanged("UrlTextBox");
                    IsCloneButtonEnabled = true;
                }
            }
        }
        public bool IsCloneButtonEnabled
        {
            get
            {
                return _isCloneButtonEnabled;
            }
            set
            {
                _isCloneButtonEnabled = value;
                RaisePropertyChanged("IsCloneButtonEnabled");
            }
        }
        #endregion

        #region Buttons getters
        public RelayCommand PickGitRepositoryCommand
        {
            get
            {
                return _pickGitRepositoryCommand ?? (_pickGitRepositoryCommand = new RelayCommand(PickGitRepository));
            }
        }
        public RelayCommand PickSvnRepositoryCommand
        {
            get
            {
                return _pickSvnRepositoryCommand ?? (_pickSvnRepositoryCommand = new RelayCommand(PickSvnRepository));
            }
        }

        public RelayCommand PickFileCommand
        {
            get
            {
                return _pickFileCommand ?? (_pickFileCommand = new RelayCommand(PickFile));
            }
        }

        public RelayCommand StartWorkCommand
        {
            get
            {
                return _startWorkCommand ??
                       (_startWorkCommand = new RelayCommand(this.OpenRepository));
            }
        }


        public RelayCommand AsyncClearDbCommand
        {
            get
            {
                return _asyncClearDbCommand ??
                       (_asyncClearDbCommand =
                           new RelayCommand(_clearDbWorker.RunWorkerAsync, () => !_clearDbWorker.IsBusy));
            }
        }
        #endregion

        #region Buttons actions
        private bool SetIsLocal()
        {
            string pattern = @"https?.*";
            Regex rgx = new Regex(pattern);
            Match m = rgx.Match(this.UrlTextBox);
            if (m.Success)
                return false;

            return true;
        }


        private void PickGitRepository()
        {
            IsGitRepositoryPicked = true;
        }

        private void PickSvnRepository()
        {
            IsGitRepositoryPicked = false;
        }


        public void PickFile()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            fbd.Description = this.GetLocalizedString("PickFolderWithRepo");

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                UrlTextBox = fbd.SelectedPath;
            }
        }

        public async void OpenRepository()
        {
            if (!string.IsNullOrEmpty(UrlTextBox))
            {
                _isLocal = SetIsLocal();
                try
                {
                    IsLoading = true;
                    if (IsGitRepositoryPicked)
                    {
                        RepositoryCloneType currentRepositoryType = RepositoryCloneType.Public;
                        string username = string.Empty, password = string.Empty;
                        if (!_isLocal && GitCloneService.CheckRepositoryCloneType(UrlTextBox) == RepositoryCloneType.Private)
                        {
                            currentRepositoryType = RepositoryCloneType.Private;
                             var loginResult =
                                 await
                                     _dialogCoordinator.ShowLoginAsync(ViewModelLocator.Instance.Main, 
                                     this.GetLocalizedString("LoginInformation"),
                                     this.GetLocalizedString("EnterCredentials"));

                            if (loginResult != null)
                            {
                                username = loginResult.Username;
                                password = loginResult.Password;
                            }
                        }

                        await Task.Run(() =>
                        {
                            this.IsOpening = true;
                            switch (currentRepositoryType)
                            {
                                case RepositoryCloneType.Private:
                                    _repositoryFilePersister = new GitFilePersister(UrlTextBox,
                                        RepositoryCloneType.Private, username, password,
                                        ConfigurationService.Instance.Configuration.SavingRepositoryPath,
                                        ConfigurationService.Instance.Configuration.CloneAllBranches);
                                    break;
                                case RepositoryCloneType.Public:
                                    _repositoryFilePersister = new GitFilePersister(this.UrlTextBox, !_isLocal,
                                        ConfigurationService.Instance.Configuration.SavingRepositoryPath,
                                        ConfigurationService.Instance.Configuration.CloneAllBranches);
                                    break;
                            }
                            _repositoryFilePersister.AddRepositoryToDataBase(DbService.Instance.SessionFactory);
                        });
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            _repositoryFilePersister = new SvnFilePersister(UrlTextBox);
                            _repositoryFilePersister.AddRepositoryToDataBase(DbService.Instance.SessionFactory);
                        });
                    }

                    this.UrlTextBox = string.Empty;
                    Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
                    ViewModelLocator.Instance.Filtering.ResetInitialization();
                    Messenger.Default.Send<RefreshMessageToFiltering>(new RefreshMessageToFiltering(true));
                    ViewModelLocator.Instance.Main.OnLoad();
                }
                catch (Exception ex)
                {
                    await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                    {
                        MetroWindow = StaticServiceProvider.MetroWindowInstance,
                        DialogTitle = this.GetLocalizedString("Error"),
                        DialogMessage = ex.Message,
                        OkButtonMessage = "Ok",
                        InformationType = InformationType.Error
                    });
                }
                finally
                {
                    this.IsLoading = false;
                }
            }
        }

        public override async void OnLoad()
        {
            try
            {
        
            }
            catch (Exception ex)
            {
                await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                {
                    MetroWindow = StaticServiceProvider.MetroWindowInstance,
                    DialogTitle = this.GetLocalizedString("Error"),
                    DialogMessage = ex.Message,
                    OkButtonMessage = "Ok",
                    InformationType = InformationType.Error
                });
            }
        }

        private void ClearDataBase()
        {
            DbService.Instance.CreateDataBase();
            Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
        }
        #endregion

        #region BackgroundWorker



        private void DoClearWork(object sender, DoWorkEventArgs e)
        {
            IsOpening = false;
            IsLoading = true;
            ClearDataBase();
        }

        private async void DoClearWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                {
                    MetroWindow = StaticServiceProvider.MetroWindowInstance,
                    DialogTitle = this.GetLocalizedString("Error"),
                    DialogMessage = e.Error.Message,
                    OkButtonMessage = "Ok",
                    InformationType = InformationType.Error
                });
            }
            else
            {
                Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
                Messenger.Default.Send<RefreshMessageToFiltering>(new RefreshMessageToFiltering(true));
                IsLoading = false;
                ViewModelLocator.Instance.Main.OnLoad();
            }
        }
        #endregion
    }
}
