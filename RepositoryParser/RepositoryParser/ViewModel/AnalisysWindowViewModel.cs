using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.View;

namespace RepositoryParser.ViewModel
{
    public class AnalisysWindowViewModel : ViewModelBase
    {
        #region Fields
       // private GitRepositoryService _localGitRepositoryService;
        private SqLiteService _localSqliteService;
        private ObservableCollection<string> _localCollection;
        private ObservableCollection<string> _branchCollection;
        private ObservableCollection<string> _repositoryCcollection;
        private List<CommitTable> localList;
        private List<string> _authorsList;
        private List<BranchTable> _branchList;
        private List<RepositoryTable> _repoList;
        private string _fromDate;
        private string _toDate;
        private string _messageTextBox;
        private string _branchSelectedItem;
        private string _repositorySelectedItem;
        private string _comboBoxSelectedItem;
        private string _repoType;
        private bool _branchEnabled = false;
  
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private RelayCommand _clearFiltersCommand;

        private RelayCommand _chartCommand;
        private RelayCommand _sendDataCommand;
        private RelayCommand _messageFilterCommand;
        private RelayCommand _monthActivityWindowCommand;
        private RelayCommand _goToDifferencesCommand;
        private RelayCommand _goToDayChartWindowCommand;
        private RelayCommand _goToHourActivityCommand;
        private RelayCommand _goToWeekDayActivityWindowCommand;
        private RelayCommand _goToUsersCodeFrequencyCommand;
        private RelayCommand _onLoadCommand;
        #endregion
        public AnalisysWindowViewModel()
        {
            _authorsList = new List<string>();
            LocalCollection = new ObservableCollection<string>();
            BranchCollection = new ObservableCollection<string>();
            RepositoryCollection = new ObservableCollection<string>();
            localList = new List<CommitTable>();
        }
        #region Buttons getters

        public RelayCommand OnLoadCommand
        {
            get
            {
                return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(OnLoad));   
            }
        }

        public RelayCommand GoToUsersCodeFrequencyCommand
        {
            get
            {
                return _goToUsersCodeFrequencyCommand ??
                       (_goToUsersCodeFrequencyCommand = new RelayCommand(GoToUsersCodeFrequency));
            }
        }
        public RelayCommand ChartCommand
        {
            get { return _chartCommand ?? (_chartCommand = new RelayCommand(Chart)); }
        }

        public RelayCommand SendDataCommand
        {
            get { return _sendDataCommand ?? (_sendDataCommand = new RelayCommand(SelectedData)); }
        }

        public RelayCommand MessageFilterCommnad
        {
            get { return _messageFilterCommand ?? (_messageFilterCommand = new RelayCommand(SearchMessage)); }
        }

        public RelayCommand MonthActivityWindowCommand
        {
            get { return _monthActivityWindowCommand ??
                (_monthActivityWindowCommand = new RelayCommand(MonthActivityWindow));
            }
        }

        public RelayCommand GoToDifferencesCommand
        {
            get { return _goToDifferencesCommand ?? (_goToDifferencesCommand = new RelayCommand(GoToDifferences)); }
        }

        public RelayCommand GoToDayChartWindowCommand
        {
            get
            {
                return _goToDayChartWindowCommand ?? (_goToDayChartWindowCommand = new RelayCommand(GoToDayChartWindow));
            }
        }

        public RelayCommand GoToHourActivityWindowCommand
        {
            get
            {
                return _goToHourActivityCommand ?? (_goToHourActivityCommand = new RelayCommand(GoToHourActivityWindow));
            }
        }

        public RelayCommand GoToWeekDayActivityWindowCommand
        {
            get
            {
                return _goToWeekDayActivityWindowCommand ??
                       (_goToWeekDayActivityWindowCommand = new RelayCommand(GoToWeekDayActivityWindow));
            }
        }
        #endregion

        #region Messages
        private void SendMessageToDisplay()
        {
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this.localList));
        }

        private void SendMessageToDrawChart()
        {
            Messenger.Default.Send<DataMessageToCharts>(new DataMessageToCharts(_authorsList,
                GenerateQuery()));
        }

        #endregion

        #region Getters/Setters

        public string RepoType
        {
            get { return _repoType; }
            set
            {
                if (_repoType != value)
                {
                    _repoType = value;
                    RaisePropertyChanged("RepoType");
                }
            }
        }
        public bool BranchEnabled
        {
            get { return _branchEnabled; }
            set
            {
                if (_branchEnabled != value)
                {
                    _branchEnabled = value;
                    RaisePropertyChanged("BranchEnabled");
                }
            }
        }

        public ObservableCollection<string> BranchCollection
        {
            get { return _branchCollection; }
            set
            {
                if (_branchCollection != value)
                {
                    _branchCollection = value;
                    RaisePropertyChanged("BranchCollection");
                }
            }
        }

        public ObservableCollection<string> RepositoryCollection
        {
            get { return _repositoryCcollection; }
            set
            {
                if (_repositoryCcollection != value)
                {
                    _repositoryCcollection = value;
                    RaisePropertyChanged("RepositoryCollection");
                }
            }
        }

        public string MessageTextBox
        {
            get { return _messageTextBox; }
            set
            {
                if (_messageTextBox != value)
                {
                    _messageTextBox = value;
                    RaisePropertyChanged("MessageTextBox");
                }
            }
        }

        public string FromDate
        {
            get { return _fromDate; }
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public string ToDate
        {
            get { return _toDate; }
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        public ObservableCollection<string> LocalCollection
        {
            get { return _localCollection; }
            set
            {
                if (_localCollection != value)
                {
                    _localCollection = value;
                    RaisePropertyChanged("LocalCollection");
                }
            }
        }

        public string ComboBoxSelectedItem
        {
            get { return _comboBoxSelectedItem; }
            set
            {
                if (_comboBoxSelectedItem != value)
                {
                    _comboBoxSelectedItem = value;
                    /*if (_comboBoxSelectedItem != null)
                        ComboBox(_comboBoxSelectedItem);*/
                    RaisePropertyChanged("ComboBoxSelectedItem");
                }
            }
        }

        public string BranchSelectedItem
        {
            get { return _branchSelectedItem; }
            set
            {
                if (_branchSelectedItem != value)
                {
                    _branchSelectedItem = value;
                    if (_branchSelectedItem != null)
                    {
                        BranchSelectedItemAction(_branchSelectedItem);
                        MainViewModel.SelectedBranch = _branchSelectedItem;
                        SendMessageToDrawChart();
                    }
                    RaisePropertyChanged("BranchSelectedItem");
                }
            }
        }

        public string RepositorySelectedItem
        {
            get { return _repositorySelectedItem; }
            set
            {
                _repositorySelectedItem = value;
                if (_repositorySelectedItem != null)
                {
                    RepositorySelectedItemAction(_repositorySelectedItem);
                    MainViewModel.SelectedRepo = _repositorySelectedItem;
                    BranchEnabled = true;

                    var firstOrDefault = _repoList.FirstOrDefault(x => x.Name == _repositorySelectedItem);
                    if (firstOrDefault != null)
                        RepoType=firstOrDefault.Type;

                    getBranches();
                    getAuthors();

                }
                RaisePropertyChanged("RepositorySelectedItem");
            }
        }

        #endregion

        #region Methods

        private void BranchSelectedItemAction(string selecteditem)
        {
            localList.Clear();
            string query = "SELECT * FROM Commits " +
                           "INNER JOIN CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                           "INNER JOIN BRANCH on CommitForBranch.NR_Branch=Branch.ID " +
                           "WHERE Branch.Name='" + selecteditem + "'";

            ExecuteRefresh(query);
        }

        private void RepositorySelectedItemAction(string selecteditem)
        {
            localList.Clear();
            string query = "SELECT * FROM Commits " +
                           "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                           "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                           "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                           "WHERE Repository.Name='" + selecteditem + "'";
            ExecuteRefresh(query);

        }




        private void getBranches()
        {
            BranchCollection.Clear();
            if (string.IsNullOrEmpty(MainViewModel.SelectedRepo))
            {
                BranchCollection.Clear();
                BranchTable temp = new BranchTable();
                _branchList = temp.GetDataFromBase(SqLiteService.GetInstance().Connection);
                _branchList.ForEach(x => BranchCollection.Add(x.Name));
            }
            else
            {
                string query = "select * from Branch " +
                               "inner join BranchForRepo on Branch.ID = BranchForRepo.NR_GitBranch " +
                               "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                               "where Repository.Name='" + MainViewModel.SelectedRepo + "'";
                BranchTable temp = new BranchTable();
                _branchList = temp.GetDataFromBase(SqLiteService.GetInstance().Connection, query);
                _branchList.ForEach(x => BranchCollection.Add(x.Name));
            }
        }

        private void getRepositories()
        {
            RepositoryCollection.Clear();
            RepositoryTable temp= new RepositoryTable();
            _repoList = temp.GetDataFromBase(SqLiteService.GetInstance().Connection);
            _repoList.ForEach(x => RepositoryCollection.Add(x.Name));
        }
        private void getAuthors()
        {
            _authorsList.Clear();
            LocalCollection.Clear();
            string query = "SELECT Author FROM Commits GROUP BY Author";
            if (!string.IsNullOrEmpty(MainViewModel.SelectedRepo))
            {
                query = "select Author from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Repository.Name='" + MainViewModel.SelectedRepo + "' " +
                        "group by Author ";
            }

            SQLiteCommand command = new SQLiteCommand(query, _localSqliteService.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string author = Convert.ToString(reader["Author"]);
                _authorsList.Add(author);
            }
            //fill collection
            _authorsList.Add("");
            _authorsList.ForEach(x => LocalCollection.Add(x));

        }
        #endregion

        #region Buttons actions

        private void OnLoad()
        {
            _localSqliteService = SqLiteService.GetInstance();
            getAuthors();
            getBranches();
            getRepositories();
        }

        private void GoToUsersCodeFrequency()
        {
            UsersCodeFrequencyView _window = new UsersCodeFrequencyView();
            _window.Show();
            SendMessageToDrawChart();
        }

        private void SearchMessage()
        {
            LocalCollection.Clear();
            string query = "SELECT * FROM Commits WHERE Message LIKE '%" +
                           MessageTextBox + "%'";
            ExecuteRefresh(query);
        }

        private void Chart()
        {

            ChartWindowView _analisysWindow = new ChartWindowView();
            _analisysWindow.Show();

            SendMessageToDrawChart();
        }

        private void GoToWeekDayActivityWindow()
        {
            WeekDayActivityView _window = new WeekDayActivityView();
            _window.Show();
            SendMessageToDrawChart();
        }

        private void GoToHourActivityWindow()
        {
            HourActivityView _window = new HourActivityView();
            _window.Show();
            SendMessageToDrawChart();
        }

        private void GoToDifferences()
        {
            DifferenceWindowView _diff = new DifferenceWindowView();
            _diff.Show();
            SendMessageToDrawChart();
        }

        private void GoToDayChartWindow()
        {
            DayActivityChartView _window = new DayActivityChartView();
            _window.Show();
            SendMessageToDrawChart();
        }

        private void MonthActivityWindow()
        {
            MonthActivityChart _monthWindow = new MonthActivityChart();
            _monthWindow.Show();
            SendMessageToDrawChart();
        }

        private void ComboBox(string temp)
        {
            var _author = temp;
            string authorVariable = _author;
            ExecuteRefresh(authorVariable);

        }

        private void SelectedData()
        {
            localList.Clear();
            string query = GenerateQuery();
            ExecuteRefresh(query);
        }


        private string GenerateQuery()
        {
            bool isCountiuned = false;
            string query = "SELECT * FROM Commits WHERE ";
            if (!string.IsNullOrEmpty(MainViewModel.SelectedRepo) && !string.IsNullOrEmpty(MainViewModel.SelectedBranch))
            {
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='" + MainViewModel.SelectedBranch + "' " +
                        "and Repository.Name='" + MainViewModel.SelectedRepo + "' " +
                        "and ";
                isCountiuned = true;
            }
            else if (!string.IsNullOrEmpty(MainViewModel.SelectedRepo) &&
                     string.IsNullOrEmpty(MainViewModel.SelectedBranch))
            {
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='master' " +
                        "and Repository.Name='" + MainViewModel.SelectedRepo + "' " +
                        "and ";
                isCountiuned = true;
            }

            bool isAuthor = !string.IsNullOrEmpty(ComboBoxSelectedItem);
            bool isFromDate = !string.IsNullOrEmpty(_fromDate);
            bool isToDate = !string.IsNullOrEmpty(_toDate);
            bool isMessage = !string.IsNullOrEmpty(MessageTextBox);

            if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false && isCountiuned == false)
                return "SELECT * FROM Commits";
            else if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false && isCountiuned == true)
            {
                string selectedBranch = MainViewModel.SelectedBranch;
                if (string.IsNullOrEmpty(MainViewModel.SelectedBranch))
                    selectedBranch = "master";
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='" + selectedBranch + "' " +
                        "and Repository.Name='" + MainViewModel.SelectedRepo + "' ";
                return query;
            }
            else if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == true)
            {
                query += "Message LIKE '%" +
                        MessageTextBox + "%'";
                return query;
            }

            if (isAuthor == true && isFromDate == false && isToDate == false)
                query += "Author='" + ComboBoxSelectedItem + "'";
            else if (isAuthor == true && isFromDate == true && isToDate == true)
            {
                query += "Author='" + ComboBoxSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + _fromDate + "'" +
                " AND " +
                "Date <= " + "'" + _toDate + "'";
            }
            else if (isAuthor == true && isFromDate == true && isToDate == false)
            {
                query += "Author='" + ComboBoxSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + _fromDate + "'" +
                " AND " +
                "Date <= " + "'" + "2150-01-01" + "'";
            }
            else if (isAuthor == true && isFromDate == false && isToDate == true)
            {
                query += "Author='" + ComboBoxSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + "1950-01-01" + "'" +
                " AND " +
                "Date <= " + "'" + _toDate + "'"; ;
            }
            else if (isAuthor == false && isFromDate == true && isToDate == true)
            {
                query += "Date >= " + "'" + _fromDate + "'" +
                        " AND " +
                        "Date <= " + "'" + _toDate + "'";
            }
            else if (isAuthor == false && isFromDate == true && isToDate == false)
            {
                query += "Date >= " + "'" + _fromDate + "'" +
                        " AND " +
                        "Date <= " + "'" + "2150-01-01" + "'";
            }
            else if (isAuthor == false && isFromDate == false && isToDate == true)
            {
                query += "Date >= " + "'" + "1950-01-01" + "'" +
                        " AND " +
                        "Date <= " + "'" + _toDate + "'";
            }

            if (isMessage)
                query += " AND Message LIKE '%" +
                           MessageTextBox + "%'";

            return query;
        }


        private void ExecuteRefresh(string query)
        {
            SQLiteCommand command = new SQLiteCommand(query, _localSqliteService.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            int id = 1;
            while (reader.Read())
            {
                string message = Convert.ToString(reader["Message"]);
                string author = Convert.ToString(reader["Author"]);
                string date = Convert.ToString(reader["Date"]);
                string email = Convert.ToString(reader["Email"]);
               // CommitTable tempInstance = new CommitTable(id, message, author, date, email);
                localList.Add(new CommitTable(id,message,author,date,email));
                id++;
            }
            SendMessageToDisplay();
        }

        public RelayCommand ClearFiltersCommand
        {
            get { return _clearFiltersCommand ?? (_clearFiltersCommand = new RelayCommand(ClearFilters)); }
        }
        private void ClearFilters()
        {
            MainViewModel.SelectedBranch=String.Empty;
            MainViewModel.SelectedRepo = String.Empty;
            BranchSelectedItem = String.Empty;
            RepositorySelectedItem = String.Empty;
            ComboBoxSelectedItem = String.Empty;
            BranchEnabled = false;
            RepoType = String.Empty;
            MessageTextBox=String.Empty;
            FromDate = String.Empty;
            ToDate = String.Empty;

            SendMessageToDrawChart();
        }
        #endregion
    }
}
