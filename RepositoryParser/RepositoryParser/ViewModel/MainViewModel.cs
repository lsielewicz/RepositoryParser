﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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
        private ObservableCollection<GitCommits> _commitsCollection;
        private string _urlTextBox = "";
        private GitRepositoryService GitRepoInstance;
        private bool _isCloneButtonEnabled = true;
        private bool _progressBarVisibility = false;
        private bool isLocal = false; 
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private BackgroundWorker worker;
        private RelayCommand startWorkCommand;
        private RelayCommand openRepositoryCommand;
        private static string selectedBranch;
        #endregion

        public MainViewModel()
        {
            Messenger.Default.Register<DataMessageToDisplay>(this, x => HandleDataMessage(x.CommitList));
            CommitsColection = new ObservableCollection<GitCommits>();

            PickFileCommand = new RelayCommand(PickFile);
            GoToPageAnalisysCommand = new RelayCommand(GoToPageAnalisys);
            SortCommand = new RelayCommand<object>(Sort);
            ClearDataBaseCommand = new RelayCommand(ClearDataBase);
            OnLoadCommand = new RelayCommand(OnLoad);
            RefreshCommand = new RelayCommand(RefreshList);
            ExportFileCommand = new RelayCommand(ExportFile);

            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoWork;
            this.worker.RunWorkerCompleted += this.RunWorkerCompleted;
        }

        #region Getters/Setters
        public static string SelectedBranch
        {
            get { return selectedBranch; }
            set
            {
                if (selectedBranch != value)
                    selectedBranch = value;
            }
        }

        private static string selectedRepo;



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

        public ObservableCollection<GitCommits> CommitsColection
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
        public RelayCommand StartWorkCommand
        {
            get
            {
                return startWorkCommand ??
                       (startWorkCommand = new RelayCommand(worker.RunWorkerAsync, () => !worker.IsBusy));
            }
        }

        public ICommand RefreshCommand { get; private set; }

        public ICommand PickFileCommand { get; private set; }
        public void PickFile()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            fbd.Description = _resourceManager.GetString("PickFolderWithRepo");

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                UrlTextBox = fbd.SelectedPath;
                isLocal = true;
            }
        }

        public RelayCommand OpenRepositoryCommand
        {
            get
            {
                return openRepositoryCommand ?? (openRepositoryCommand = new RelayCommand(OpenRepository));
            }
        }
        public void OpenRepository()
        {

            if (!string.IsNullOrEmpty(UrlTextBox))
            {
                try
                {
                    ProgressBarVisibility = true;

                    if (!isLocal)
                    {
                        GitRepoInstance = new GitRepositoryService();
                        GitRepoInstance.UrlRepoPath = UrlTextBox;
                        GitRepoInstance.isCloned = false;
                        GitRepoInstance.InitializeConnection();
                        GitRepoInstance.FillDataBase();


                    }
                    else
                    {
                        GitRepoInstance = new GitRepositoryService();
                        GitRepoInstance.isCloned = true;
                        GitRepoInstance.RepoPath = UrlTextBox;
                        GitRepoInstance.InitializeConnection();
                        GitRepoInstance.FillDataBase();

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
            GitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
        }
        public ICommand OnLoadCommand { get; set; }
        private void OnLoad()
        {
            try
            {
                GitRepoInstance = new GitRepositoryService();

                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    GitRepoInstance.ConnectRepositoryToDataBase(true);
                else
                    GitRepoInstance.ConnectRepositoryToDataBase();

                CommitsColection.Clear();
                GitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ICommand GoToPageAnalisysCommand { get; set; }

        private void GoToPageAnalisys()
        {
            AnalysisWindowView _analisysWindow = new AnalysisWindowView();
            _analisysWindow.Show();
            if (GitRepoInstance != null)
                SendMessageToAnalisys();
        }

        public ICommand ClearDataBaseCommand { get; set; }

        private void ClearDataBase()
        {
            if (GitRepoInstance != null)
            {
                List<string> Transactions = new List<string>();
                Transactions.Add(GitCommits.deleteAllQuery);
                Transactions.Add(GitRepositoryTable.deleteAllQuery);
                Transactions.Add(BranchTable.deleteAllQuery);
                Transactions.Add(CommitForBranchTable.deleteAllQuery);
                Transactions.Add(BranchForRepoTable.deleteAllQuery);
                Transactions.Add(ChangesForCommitTable.deleteAllQuery);
                Transactions.Add(ChangesTable.deleteAllQuery);
                string[] TableName = new string[]
                {
                    "GitCommits",
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
                GitRepoInstance.SqLiteInstance.ExecuteTransaction(Transactions);

            }
            RefreshList();
        }

        public ICommand ExportFileCommand { get; set; }
        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "GitCommitsFile";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                List<GitCommits> tempList = CommitsColection.ToList();
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

        private void HandleDataMessage(List<GitCommits> list)
        {
            CommitsColection.Clear();
            list.ForEach(x => CommitsColection.Add(x));
        }
        private void SendMessageToAnalisys()
        {
            Messenger.Default.Send<DataMessageToAnalisys>(new DataMessageToAnalisys(this.GitRepoInstance));
        }
        #endregion


        #region BackgroundWorker
        private void DoWork(object sender, DoWorkEventArgs e)
        {
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
                GitRepoInstance.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
            }
        }
        #endregion



    }
}