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
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using MessageBox = System.Windows.MessageBox;

namespace RepositoryParser.ViewModel
{
    public class DataBaseManagementViewModel : ViewModelBase
    {
        #region private variables
        private GitService _gitRepoService;
        private SvnService _svnRepoService;
        private string _urlTextBox = "";
        private bool _isCloneButtonEnabled = true;
        private bool _progressBarVisibility = false;
        private bool _isLocal = false;
        private bool _isOpening;
        private bool _isGitRepositoryPicked;
        private readonly ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private BackgroundWorker _worker;
        private BackgroundWorker clearDBWorker;
        private RelayCommand _startWorkCommand;
        private RelayCommand _asyncClearDBCommand;
        private RelayCommand _pickFileCommand;
        private RelayCommand _pickGitRepositoryCommand;
        private RelayCommand _pickSvnRepositoryCommand;
        #endregion

        public DataBaseManagementViewModel()
        {
            this._worker = new BackgroundWorker();
            this._worker.DoWork += this.DoWork;
            this._worker.RunWorkerCompleted += this.RunWorkerCompleted;

            this.clearDBWorker = new BackgroundWorker();
            this.clearDBWorker.DoWork += this.DoClearWork;
            this.clearDBWorker.RunWorkerCompleted += this.DoClearWorkCompleted;

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

        public bool ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged("ProgressBarVisibility");
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


        public RelayCommand AsyncClearDBCommand
        {
            get
            {
                return _asyncClearDBCommand ??
                       (_asyncClearDBCommand =
                           new RelayCommand(clearDBWorker.RunWorkerAsync, () => !clearDBWorker.IsBusy));
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
            fbd.Description = _resourceManager.GetString("PickFolderWithRepo");

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
                    ProgressBarVisibility = true;

                    if (IsGitRepositoryPicked == false)
                    {
                        _svnRepoService = new SvnService(UrlTextBox);
                        _svnRepoService.FillDataBase();
                    }
                    else
                    {
                        if (!_isLocal)
                        {
                            _gitRepoService = new GitService(UrlTextBox, true);
                            _gitRepoService.FillDataBase();
                        }
                        else
                        {
                            _gitRepoService = new GitService(UrlTextBox, false);
                            _gitRepoService.FillDataBase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                finally
                {
                    ProgressBarVisibility = false;
                }
            }
            else
            {
                System.Windows.MessageBox.Show(_resourceManager.GetString("NoRepositoryPathError"), _resourceManager.GetString("Error"));
            }

        }

        private void OnLoad()
        {
            try
            {
                _gitRepoService = new GitService();
                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    _gitRepoService.ConnectRepositoryToDataBase(true);
                else
                    _gitRepoService.ConnectRepositoryToDataBase();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ClearDataBase()
        {
            List<string> Transactions = new List<string>();
            Transactions.Add(CommitTable.deleteAllQuery);
            Transactions.Add(RepositoryTable.deleteAllQuery);
            Transactions.Add(BranchTable.deleteAllQuery);
            Transactions.Add(CommitForBranchTable.deleteAllQuery);
            Transactions.Add(BranchForRepoTable.deleteAllQuery);
            Transactions.Add(ChangesForCommitTable.deleteAllQuery);
            Transactions.Add(ChangesTable.deleteAllQuery);
            string[] TableName = new string[]
            {
                    "Commits",
                    "Repository",
                    "Branch",
                    "CommitForBranch",
                    "BranchForRepo",
                    "Changes",
                    "ChangesForCommit"
            };
            foreach (string name in TableName)
            {
                string delete = "delete from sqlite_sequence where name = '" + name + "'";
                Transactions.Add(delete);
            }
            SqLiteService.GetInstance().ExecuteTransaction(Transactions);


            Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
        }
        #endregion

        #region Messaging

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
            }
        }


        private void DoClearWork(object sender, DoWorkEventArgs e)
        {
            IsOpening = false;
            ProgressBarVisibility = true;
            ClearDataBase();
        }

        private void DoClearWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Messenger.Default.Send<RefreshMessageToPresentation>(new RefreshMessageToPresentation(true));
            Messenger.Default.Send<RefreshMessageToFiltering>(new RefreshMessageToFiltering(true));
            ProgressBarVisibility = false;
        }
        #endregion
    }
}
