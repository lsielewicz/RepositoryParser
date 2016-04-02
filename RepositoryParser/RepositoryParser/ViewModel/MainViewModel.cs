using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.View;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace RepositoryParser.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Variables
        private ObservableCollection<CommitTable> _commitsCollection;
        private GitRepositoryService _gitRepoInstance;
        private SvnService _svnRepoService;
        private string _urlTextBox = "";
        private bool _isCloneButtonEnabled = true;
        private bool _progressBarVisibility = false;
        private bool _isLocal = false;
        private bool _isOpening;
        private bool _isGitRepositoryPicked;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private BackgroundWorker _worker;
        private BackgroundWorker clearDBWorker;
        private static string selectedBranch;
        private static string selectedRepo;
        private RelayCommand _startWorkCommand;
        private RelayCommand _asyncClearDBCommand;
        private RelayCommand _refreshCommand;
        private RelayCommand _pickFileCommand;
        private RelayCommand _onLoadCommand;
        private RelayCommand _goToPageAnalysisCommand;
        private RelayCommand _exportFileCommand;
        private RelayCommand _pickGitRepositoryCommand;
        private RelayCommand _pickSvnRepositoryCommand;
        #endregion

        public MainViewModel()
        {
            Messenger.Default.Register<DataMessageToDisplay>(this, x => HandleDataMessage(x.CommitList));
            CommitsColection = new ObservableCollection<CommitTable>();
  
            SortCommand = new RelayCommand<object>(Sort);

            this._worker = new BackgroundWorker();
            this._worker.DoWork += this.DoWork;
            this._worker.RunWorkerCompleted += this.RunWorkerCompleted;

            this.clearDBWorker = new BackgroundWorker();
            this.clearDBWorker.DoWork += this.DoClearWork;
            this.clearDBWorker.RunWorkerCompleted += this.DoClearWorkCompleted;
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

        public static string SelectedBranch
        {
            get { return selectedBranch; }
            set
            {
                if (selectedBranch != value)
                    selectedBranch = value;
            }
        }




        public static string SelectedRepo
        {
            get { return selectedRepo; }
            set
            {
                if (selectedRepo != value)
                {
                    selectedRepo = value;
                    selectedBranch = "";
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

        public ObservableCollection<CommitTable> CommitsColection
        {
            get
            {
                return _commitsCollection;

            }
            set
            {
                if (_commitsCollection != value)
                {
                    _commitsCollection = value;
                    sortDataView = new CollectionViewSource();
                    sortDataView.Source = _commitsCollection;
                    RaisePropertyChanged("CommitsCollection");
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

        #region Buttons

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

        public RelayCommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(RefreshList)); }
        }

        public RelayCommand PickFileCommand
        {
            get { return _pickFileCommand ?? (_pickFileCommand = new RelayCommand(PickFile));
            }
        }

        public RelayCommand OnLoadCommand
        {
            get { return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(OnLoad)); }
        }

        public RelayCommand GoToPageAnalysisCommand
        {
            get { return _goToPageAnalysisCommand ?? (_goToPageAnalysisCommand = new RelayCommand(GoToPageAnalisys)); }
        }

        public RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
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

        #region Methods

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
                //_isLocal = true;
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
                            _gitRepoInstance = new GitRepositoryService(UrlTextBox,true);
                            _gitRepoInstance.FillDataBase();
                        }
                        else
                        {
                            _gitRepoInstance = new GitRepositoryService(UrlTextBox,false);
                            _gitRepoInstance.FillDataBase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    ProgressBarVisibility = false;
                }
            }
            else
            {
                MessageBox.Show(_resourceManager.GetString("NoRepositoryPathError"), _resourceManager.GetString("Error"));
            }

        }

        private void RefreshList()
        {
            CommitsColection.Clear();
            _gitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
        }
        private void OnLoad()
        {
            try
            {
                _gitRepoInstance = new GitRepositoryService();

                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    _gitRepoInstance.ConnectRepositoryToDataBase(true);
                else
                    _gitRepoInstance.ConnectRepositoryToDataBase();

                CommitsColection.Clear();
                _gitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void GoToPageAnalisys()
        {
            AnalysisWindowView _analisysWindow = new AnalysisWindowView();
            _analisysWindow.Show();
            if (_gitRepoInstance != null)
                SendMessageToAnalisys();
        }


        private void ClearDataBase()
        {
            
            if (_gitRepoInstance != null)
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
                _gitRepoInstance.SqLiteInstance.ExecuteTransaction(Transactions);

            }
           // RefreshList();
        }

        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "CommitsFile";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                List<CommitTable> tempList = CommitsColection.ToList();
                DataToCsv.CreateCSVFromGitCommitsList(tempList, filename);
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion

        #region Sorting
        private CollectionViewSource sortDataView;
        private string sortColumn;
        private ListSortDirection sortDirection;
        public ICommand SortCommand
        {
            get;
            private set;
        }

        public ListCollectionView SortDataView
        {
            get
            {
                return (ListCollectionView)sortDataView.View;
            }
        }
        public void Sort(object parameter)
        {
            string column = parameter as string;
            if (sortColumn == column)
            {
                sortDirection = sortDirection == ListSortDirection.Descending ?
                    ListSortDirection.Ascending :
                    ListSortDirection.Descending;
            }
            else
            {
                sortColumn = column;
                sortDirection = ListSortDirection.Descending;
            }
            if (sortDataView != null)
            {
                sortDataView.SortDescriptions.Clear();
                sortDataView.SortDescriptions.Add(new SortDescription(sortColumn, sortDirection));
            }

        }
        #endregion

        #region Messages

        private void HandleDataMessage(List<CommitTable> list)
        {
            CommitsColection.Clear();
            list.ForEach(x => CommitsColection.Add(x));
        }
        private void SendMessageToAnalisys()
        {
            Messenger.Default.Send<DataMessageToAnalisys>(new DataMessageToAnalisys(this._gitRepoInstance));
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
                CommitsColection.Clear();
                _gitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
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
            RefreshList();
            ProgressBarVisibility = false;
        }
        #endregion

    }
}