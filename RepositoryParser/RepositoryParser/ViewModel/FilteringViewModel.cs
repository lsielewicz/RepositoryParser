using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;

namespace RepositoryParser.ViewModel
{
    public class FilteringViewModel : ViewModelBase
    {
        #region private fields
        private SqLiteService _localSqliteService;
        private ObservableCollection<string> _authorsCollection;
        private ObservableCollection<string> _branchCollection;
        private ObservableCollection<string> _repositoryCcollection;
        private readonly List<CommitTable> _commitsList;
        private readonly List<string> _authorsList;
        private List<BranchTable> _branchList;
        private List<RepositoryTable> _repoList;
        private string _fromDate;
        private string _toDate;
        private string _messageTextBox;
        private string _branchSelectedItem;
        private string _repositorySelectedItem;
        private string _authorsSelectedItem;
        private string _repoType;
        private bool _branchEnabled;

        private RelayCommand _clearFiltersCommand;
        private RelayCommand _sendDataCommand;
        private RelayCommand _onLoadCommand;

        public static string SelectedBranch { get; private set; }
        public static string SelectedRepo { get; private set; }
        #endregion

        public FilteringViewModel()
        {
            Messenger.Default.Register<RefreshMessageToFiltering>(this, x=>HandleRefreshMessage(x.Refresh));
            _authorsList = new List<string>();
            _commitsList = new List<CommitTable>();
            AuthorsCollection = new ObservableCollection<string>();
            BranchCollection = new ObservableCollection<string>();
            RepositoryCollection = new ObservableCollection<string>();
            OnLoad();
            ClearFilters();
        }

        #region Buttons getters
        public RelayCommand OnLoadCommand
        {
            get { return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(OnLoad)); }
        }

        public RelayCommand SendDataCommand
        {
            get { return _sendDataCommand ?? (_sendDataCommand = new RelayCommand(SendFilteredData)); }
        }

        public RelayCommand ClearFiltersCommand
        {
            get { return _clearFiltersCommand ?? (_clearFiltersCommand = new RelayCommand(ClearFilters)); }
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
                    this.SendFilteredData();
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
                    this.SendFilteredData();
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
                    this.SendFilteredData();
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        public ObservableCollection<string> AuthorsCollection
        {
            get { return _authorsCollection; }
            set
            {
                if (_authorsCollection != value)
                {
                    _authorsCollection = value;
                    RaisePropertyChanged("AuthorsCollection");
                }
            }
        }

        public string AuthorsSelectedItem
        {
            get { return _authorsSelectedItem; }
            set
            {
                if (_authorsSelectedItem != value)
                {
                    _authorsSelectedItem = value;
                    this.SendFilteredData();
                    RaisePropertyChanged("AuthorsSelectedItem");
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
                        FilteringViewModel.SelectedBranch = _branchSelectedItem;
                        this.SendFilteredData();
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
                    SelectedRepo = _repositorySelectedItem;
                    BranchEnabled = true;
                    var firstOrDefault = _repoList.FirstOrDefault(x => x.Name == _repositorySelectedItem);
                    if (firstOrDefault != null)
                        RepoType = firstOrDefault.Type;

                    GetBranches();
                    GetAuthors();
                    BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
                }
                RaisePropertyChanged("RepositorySelectedItem");
            }
        }

        #endregion

        #region Methods

        private void HandleRefreshMessage(bool refresh)
        {
            if (refresh)
            {
                OnLoad();
            }
        }

        private void GetBranches()
        {
            BranchCollection.Clear();
            if (String.IsNullOrEmpty(SelectedRepo))
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
                               "where Repository.Name='" + SelectedRepo + "'";
                BranchTable temp = new BranchTable();
                _branchList = temp.GetDataFromBase(SqLiteService.GetInstance().Connection, query);
                _branchList.ForEach(x => BranchCollection.Add(x.Name));
            }
        }

        private void GetRepositories()
        {
            RepositoryCollection.Clear();
            RepositoryTable temp = new RepositoryTable();
            _repoList = temp.GetDataFromBase(SqLiteService.GetInstance().Connection);
            _repoList.ForEach(x => RepositoryCollection.Add(x.Name));
        }

        private void GetAuthors()
        {
            _authorsList.Clear();
            AuthorsCollection.Clear();
            string query = "SELECT Author FROM Commits GROUP BY Author";
            if (!String.IsNullOrEmpty(SelectedRepo))
            {
                query = "select Author from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Repository.Name='" + SelectedRepo + "' " +
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
            _authorsList.Add(string.Empty);
            _authorsList.ForEach(x => AuthorsCollection.Add(x));

        }

        private void OnLoad()
        {
            _localSqliteService = SqLiteService.GetInstance();
            GetAuthors();
            GetBranches();
            GetRepositories();
            ClearFilters();
        }

        private void SendFilteredData()
        {
            string query = GenerateQuery();
            ExecuteRefresh(query);
        }


        private string GenerateQuery()
        {
            bool isCountiuned = false;
            string query = "SELECT * FROM Commits WHERE ";
            if (!String.IsNullOrEmpty(SelectedRepo) && !String.IsNullOrEmpty(SelectedBranch))
            {
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='" + SelectedBranch + "' " +
                        "and Repository.Name='" + SelectedRepo + "' " +
                        "and ";
                isCountiuned = true;
            }
            else if (!String.IsNullOrEmpty(SelectedRepo) &&
                     String.IsNullOrEmpty(SelectedBranch))
            {
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='master' " +
                        "and Repository.Name='" + SelectedRepo + "' " +
                        "and ";
                isCountiuned = true;
            }

            bool isAuthor = !String.IsNullOrEmpty(AuthorsSelectedItem);
            bool isFromDate = !String.IsNullOrEmpty(_fromDate);
            bool isToDate = !String.IsNullOrEmpty(_toDate);
            bool isMessage = !String.IsNullOrEmpty(MessageTextBox);

            if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false && isCountiuned == false)
                return "SELECT * FROM Commits";
            else if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false && isCountiuned == true)
            {
                string selectedBranch = SelectedBranch;
                if (String.IsNullOrEmpty(SelectedBranch))
                    selectedBranch = "master";
                query = "select * from Commits " +
                        "inner join CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                        "inner join BranchForRepo on CommitForBranch.NR_Branch=BranchForRepo.NR_GitBranch " +
                        "inner join Branch on CommitForBranch.NR_Branch=Branch.ID " +
                        "inner join Repository on BranchForRepo.NR_GitRepository=Repository.ID " +
                        "where " +
                        "Branch.Name='" + selectedBranch + "' " +
                        "and Repository.Name='" + SelectedRepo + "' ";
                return query;
            }
            else if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == true)
            {
                query += "Message LIKE '%" +
                        MessageTextBox + "%'";
                return query;
            }

            if (isAuthor == true && isFromDate == false && isToDate == false)
                query += "Author='" + AuthorsSelectedItem + "'";
            else if (isAuthor == true && isFromDate == true && isToDate == true)
            {
                query += "Author='" + AuthorsSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + _fromDate + "'" +
                " AND " +
                "Date <= " + "'" + _toDate + "'";
            }
            else if (isAuthor == true && isFromDate == true && isToDate == false)
            {
                query += "Author='" + AuthorsSelectedItem + "'" +
                " AND " +
                "Date >= " + "'" + _fromDate + "'" +
                " AND " +
                "Date <= " + "'" + "2150-01-01" + "'";
            }
            else if (isAuthor == true && isFromDate == false && isToDate == true)
            {
                query += "Author='" + AuthorsSelectedItem + "'" +
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
            _commitsList.Clear();
            SQLiteCommand command = new SQLiteCommand(query, _localSqliteService.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            int id = 1;
            while (reader.Read())
            {
                string message = Convert.ToString(reader["Message"]);
                string author = Convert.ToString(reader["Author"]);
                string date = Convert.ToString(reader["Date"]);
                string email = Convert.ToString(reader["Email"]);
                _commitsList.Add(new CommitTable(id, message, author, date, email));
                id++;
            }
            SendMessageToDisplay();
            SendMessageToDrawChart(query);
        }



        private void ClearFilters()
        {
            RepositorySelectedItem = RepositoryCollection.FirstOrDefault() ?? string.Empty;
            BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
            AuthorsSelectedItem = String.Empty;
            MessageTextBox = String.Empty;
            FromDate = String.Empty;
            ToDate = String.Empty;
        }
        #endregion

        #region Messages
        private void SendMessageToDisplay()
        {
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this._commitsList));
        }

        private void SendMessageToDrawChart(string queryToSend)
        {
            Messenger.Default.Send<ChartMessageLevel0>(new ChartMessageLevel0(_authorsList,queryToSend));
        }
        #endregion
    }
}
