using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.View;
using RepositoryParser.ViewModel;

namespace RepositoryParser.Core.ViewModel
{
    public class AnalisysWindowViewModel : ViewModelBase
    {
        private GitRepositoryService _localGitRepositoryService;
        private List<CommitTable> localList;
        private SqLiteService _localSqliteService;
        public ObservableCollection<string> _localCollection;
        private List<string> authorsList;
        private List<string> emailList;
        private List<BranchTable> _branchList;
        private string fromDate;
        private string toDate;
        private string messageTextBox;
        private ObservableCollection<string> branchCollection;
        private ObservableCollection<string> repositoryCcollection;
        private string branchSelectedItem;
        private string repositorySelectedItem;
        private bool branchEnabled = false;
        private List<RepositoryTable> _repoList;
        private string _comboBoxSelectedItem;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());

        private string _repoType;

        public AnalisysWindowViewModel()
        {
            emailList = new List<string>();
            authorsList = new List<string>();
            Messenger.Default.Register<DataMessageToAnalisys>(this, x => HandleDataMessage(x.GitRepoInstance));
            LocalCollection = new ObservableCollection<string>();
            BranchCollection = new ObservableCollection<string>();
            RepositoryCollection = new ObservableCollection<string>();
            localList = new List<CommitTable>();

            //buttons
            ChartCommand = new RelayCommand(Chart);
            SendDataCommand = new RelayCommand(SelectedData);
            MessageFilterCommnad = new RelayCommand(SearchMessage);
            MonthActivityWindowCommand = new RelayCommand(MonthActivityWindow);
            GoToDifferencesCommand = new RelayCommand(GoToDifferences);
            GoToDayChartWindowCommand = new RelayCommand(GoToDayChartWindow);
            GoToHourActivityWindowCommand = new RelayCommand(GoToHourActivityWindow);
            GoToWeekDayActivityWindowCommand = new RelayCommand(GoToWeekDayActivityWindow);

        }

        #region Messages

        private void HandleDataMessage(GitRepositoryService gitRepo)
        {
            _localGitRepositoryService = gitRepo;
            _localSqliteService = gitRepo.SqLiteInstance;
            getAuthors();
            getBranches();
            getRepositories();
        }

        private void SendMessageToDisplay()
        {
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this.localList));
        }

        private void SendMessageToDrawChart()
        {
            Messenger.Default.Send<DataMessageToCharts>(new DataMessageToCharts(this._localGitRepositoryService, authorsList,
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
            get { return branchEnabled; }
            set
            {
                if (branchEnabled != value)
                {
                    branchEnabled = value;
                    RaisePropertyChanged("BranchEnabled");
                }
            }
        }

        public ObservableCollection<string> BranchCollection
        {
            get { return branchCollection; }
            set
            {
                if (branchCollection != value)
                {
                    branchCollection = value;
                    RaisePropertyChanged("BranchCollection");
                }
            }
        }

        public ObservableCollection<string> RepositoryCollection
        {
            get { return repositoryCcollection; }
            set
            {
                if (repositoryCcollection != value)
                {
                    repositoryCcollection = value;
                    RaisePropertyChanged("RepositoryCollection");
                }
            }
        }

        public string MessageTextBox
        {
            get { return messageTextBox; }
            set
            {
                if (messageTextBox != value)
                {
                    messageTextBox = value;
                    RaisePropertyChanged("MessageTextBox");
                }
            }
        }

        public string FromDate
        {
            get { return fromDate; }
            set
            {
                if (fromDate != value)
                {
                    fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public string ToDate
        {
            get { return toDate; }
            set
            {
                if (toDate != value)
                {
                    toDate = value;
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
            get { return branchSelectedItem; }
            set
            {
                if (branchSelectedItem != value)
                {
                    branchSelectedItem = value;
                    if (branchSelectedItem != null)
                    {
                        BranchSelectedItemAction(branchSelectedItem);
                        MainViewModel.SelectedBranch = branchSelectedItem;
                        SendMessageToDrawChart();
                    }
                    RaisePropertyChanged("BranchSelectedItem");
                }
            }
        }

        public string RepositorySelectedItem
        {
            get { return repositorySelectedItem; }
            set
            {
                repositorySelectedItem = value;
                if (repositorySelectedItem != null)
                {
                    RepositorySelectedItemAction(repositorySelectedItem);
                    MainViewModel.SelectedRepo = repositorySelectedItem;
                    BranchEnabled = true;

                    var firstOrDefault = _repoList.FirstOrDefault(x => x.Name == repositorySelectedItem);
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

        public ICommand MessageFilterCommnad { get; set; }

        private void SearchMessage()
        {
            LocalCollection.Clear();
            string query = "SELECT * FROM Commits WHERE Message LIKE '%" +
                           MessageTextBox + "%'";
            ExecuteRefresh(query);
        }

        private void getBranches()
        {
            BranchCollection.Clear();
            if (string.IsNullOrEmpty(MainViewModel.SelectedRepo))
            {
                BranchCollection.Clear();
                BranchTable temp = new BranchTable();
                _branchList = temp.GetDataFromBase(_localGitRepositoryService.SqLiteInstance.Connection);
                _branchList.ForEach(x => BranchCollection.Add(x.Name));
            }
            else
            {
                string query = "select * from Branch " +
                               "inner join BranchForRepo on Branch.ID = BranchForRepo.NR_GitBranch " +
                               "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                               "where Repository.Name='" + MainViewModel.SelectedRepo + "'";
                BranchTable temp = new BranchTable();
                _branchList = temp.GetDataFromBase(_localGitRepositoryService.SqLiteInstance.Connection, query);
                _branchList.ForEach(x => BranchCollection.Add(x.Name));
            }
        }

        private void getRepositories()
        {
            RepositoryCollection.Clear();
            RepositoryTable temp= new RepositoryTable();
            _repoList = temp.GetDataFromBase(_localGitRepositoryService.SqLiteInstance.Connection);
            _repoList.ForEach(x => RepositoryCollection.Add(x.Name));
        }
        private void getAuthors()
        {
            authorsList.Clear();
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
                authorsList.Add(author);
            }
            //fill collection
            authorsList.Add("");
            authorsList.ForEach(x => LocalCollection.Add(x));

        }
        #endregion

        #region Buttons
        public ICommand ChartCommand { get; set; }

        private void Chart()
        {

            ChartWindowView _analisysWindow = new ChartWindowView();
            _analisysWindow.Show();

            SendMessageToDrawChart();
        }
        public ICommand GoToWeekDayActivityWindowCommand { get; set; }

        private void GoToWeekDayActivityWindow()
        {
            WeekDayActivityView _window = new WeekDayActivityView();
            _window.Show();
            SendMessageToDrawChart();
        }
        public ICommand GoToHourActivityWindowCommand { get; set; }

        private void GoToHourActivityWindow()
        {
            HourActivityView _window = new HourActivityView();
            _window.Show();
            SendMessageToDrawChart();
        }
        public ICommand GoToDifferencesCommand { get; set; }
        private void GoToDifferences()
        {
            DifferenceWindowView _diff = new DifferenceWindowView();
            _diff.Show();
            SendMessageToDrawChart();
        }
        public ICommand GoToDayChartWindowCommand { get; set; }

        private void GoToDayChartWindow()
        {
            DayActivityChartView _window = new DayActivityChartView();
            _window.Show();
            SendMessageToDrawChart();
        }
        public ICommand MonthActivityWindowCommand { get; set; }

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
        public ICommand SendDataCommand { get; set; }
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
            bool isFromDate = !string.IsNullOrEmpty(fromDate);
            bool isToDate = !string.IsNullOrEmpty(toDate);
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
                "Date >= " + "'" + fromDate + "'" +
                " AND " +
                "Date <= " + "'" + toDate + "'";
            }
            else if (isAuthor == true && isFromDate == true && isToDate == false)
            {
                query += "Author='" + ComboBoxSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + fromDate + "'" +
                " AND " +
                "Date <= " + "'" + "2150-01-01" + "'";
            }
            else if (isAuthor == true && isFromDate == false && isToDate == true)
            {
                query += "Author='" + ComboBoxSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + "1950-01-01" + "'" +
                " AND " +
                "Date <= " + "'" + toDate + "'"; ;
            }
            else if (isAuthor == false && isFromDate == true && isToDate == true)
            {
                query += "Date >= " + "'" + fromDate + "'" +
                        " AND " +
                        "Date <= " + "'" + toDate + "'";
            }
            else if (isAuthor == false && isFromDate == true && isToDate == false)
            {
                query += "Date >= " + "'" + fromDate + "'" +
                        " AND " +
                        "Date <= " + "'" + "2150-01-01" + "'";
            }
            else if (isAuthor == false && isFromDate == false && isToDate == true)
            {
                query += "Date >= " + "'" + "1950-01-01" + "'" +
                        " AND " +
                        "Date <= " + "'" + toDate + "'";
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
                CommitTable tempInstance = new CommitTable(id, message, author, date, email);
                localList.Add(tempInstance);
                id++;
            }
            SendMessageToDisplay();
        }
        #endregion
    }
}
