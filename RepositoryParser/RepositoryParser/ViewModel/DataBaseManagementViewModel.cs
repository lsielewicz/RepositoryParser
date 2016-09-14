using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using MessageBox = System.Windows.MessageBox;

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
        private readonly BackgroundWorker _worker;
        private readonly BackgroundWorker _clearDbWorker;
        private RelayCommand _startWorkCommand;
        private RelayCommand _asyncClearDbCommand;
        private RelayCommand _pickFileCommand;
        private RelayCommand _pickGitRepositoryCommand;
        private RelayCommand _pickSvnRepositoryCommand;
        #endregion

        public DataBaseManagementViewModel()
        {
            this._worker = new BackgroundWorker();
            this._worker.DoWork += this.DoWork;
            this._worker.RunWorkerCompleted += this.RunWorkerCompleted;

            this._clearDbWorker = new BackgroundWorker();
            this._clearDbWorker.DoWork += this.DoClearWork;
            this._clearDbWorker.RunWorkerCompleted += this.DoClearWorkCompleted;

            OnLoad();
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
                       (_startWorkCommand = new RelayCommand(_worker.RunWorkerAsync, () => !_worker.IsBusy));
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
            fbd.Description = ResourceManager.GetString("PickFolderWithRepo");

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                UrlTextBox = fbd.SelectedPath;
            }
        }

        public void OpenRepository()
        {
            if (!string.IsNullOrEmpty(UrlTextBox))
            {
                _isLocal = SetIsLocal();
                try
                {
                    IsLoading = true;

                    if (IsGitRepositoryPicked == false)
                    {
                        _repositoryFilePersister = new SvnFilePersister(UrlTextBox);
                    }
                    else
                    {
                        _repositoryFilePersister = new GitFilePersister(UrlTextBox,!_isLocal);
                    }
                    _repositoryFilePersister.AddRepositoryToDataBase(DbService.Instance.SessionFactory);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message); //todo message
                }
                finally
                {
                    IsLoading = false;
                }
            }
            else
            {
                System.Windows.MessageBox.Show(ResourceManager.GetString("NoRepositoryPathError"), ResourceManager.GetString("Error"));
            }

        }

        public override void OnLoad()
        {
            try
            {

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ClearDataBase()
        {
            DbService.Instance.CreateDataBase();
            Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
        }
        #endregion

        #region BackgroundWorker
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            IsOpening = true;
            OpenRepository();
        }


        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                this.UrlTextBox = string.Empty;
                Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
                Messenger.Default.Send<RefreshMessageToFiltering>(new RefreshMessageToFiltering(true));
                ViewModelLocator.Instance.Main.OnLoad();
            }
        }


        private void DoClearWork(object sender, DoWorkEventArgs e)
        {
            IsOpening = false;
            IsLoading = true;
            ClearDataBase();
        }

        private void DoClearWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
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
