using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Util;
using RepositoryParser.Core.Messages;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers.Enums;

namespace RepositoryParser.ViewModel
{
    public class FilteringViewModel : ViewModelBase
    {
        #region private fields
        private ObservableCollection<string> _authorsCollection;
        private ObservableCollection<string> _branchCollection;
        private ObservableCollection<string> _repositoryCollection;
        private Dictionary<string, string> _repositoryTypeDictionary;
        private readonly List<Commit> _commitsList;
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
        private RelayCommand<object> _clearSpecifiedFilterCommand;

        public static string SelectedBranch { get; private set; }
        public static string SelectedRepo { get; private set; }
        #endregion

        public FilteringViewModel()
        {
            Messenger.Default.Register<RefreshMessageToFiltering>(this, x=>HandleRefreshMessage(x.Refresh));
            _commitsList = new List<Commit>();
            _repositoryTypeDictionary = new Dictionary<string, string>();
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

        public RelayCommand<object> ClearSpecifiedFilterFilterCommand
        {
            get
            {
                return _clearSpecifiedFilterCommand ?? (_clearSpecifiedFilterCommand = new RelayCommand<object>(
                    (param) =>
                    {
                        if (param is FilteringColumn)
                        {
                            if ((FilteringColumn) param == FilteringColumn.AuthorsColumn)
                            {
                                this.AuthorsSelectedItem = null;
                            }
                            else if ((FilteringColumn) param == FilteringColumn.DateColumn)
                            {
                                FromDate = string.Empty;
                                ToDate = string.Empty;
                            }
                            else if ((FilteringColumn) param == FilteringColumn.MessageSearchingColumn)
                            {
                                MessageTextBox = string.Empty;
                            }
                    }
                    }));
            }
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
            get
            {
                return _branchCollection;
            }
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
            get { return _repositoryCollection; }
            set
            {
                if (_repositoryCollection != value)
                {
                    _repositoryCollection = value;
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
                    var firstOrDefault = RepositoryCollection.FirstOrDefault(x => x == _repositorySelectedItem);
                    if (firstOrDefault != null)
                    {
                        RepoType = _repositoryTypeDictionary[RepositorySelectedItem];
                    }


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
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                Repository repository = null;
                if (BranchCollection != null && BranchCollection.Any())
                    BranchCollection.Clear();

                if (string.IsNullOrEmpty(SelectedRepo))
                {
                    var branches =
                        session.QueryOver<Branch>().List<Branch>();
                    branches.ForEach(branch => BranchCollection.Add(branch.Name));
                }
                else
                {
                    var branches =
                        session.QueryOver<Branch>()
                            .JoinQueryOver(branch => branch.Repository, () => repository)
                            .Where(() => repository.Name == SelectedRepo)
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .List<Branch>();
                    branches.ForEach(branch => BranchCollection.Add(branch.Name));
                }
            }
        }

        private void GetRepositories()
        {
            if(RepositoryCollection != null && RepositoryCollection.Any())
                RepositoryCollection.Clear();
            if(_repositoryTypeDictionary != null && _repositoryTypeDictionary.Any())
                _repositoryTypeDictionary.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var repositories =
                    session.QueryOver<Repository>().List<Repository>();
                repositories.ForEach(repository =>
                {
                    RepositoryCollection.Add(repository.Name);
                    _repositoryTypeDictionary.Add(repository.Name,repository.Type);
                });
                
            }
        }

        private void GetAuthors()
        {
            AuthorsCollection.Clear();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                if (string.IsNullOrEmpty(SelectedRepo))
                {
                    var authors =
                        session.QueryOver<Commit>()
                            .SelectList(list => list.SelectGroup(commit => commit.Author))
                            .List<string>();
                    authors.ForEach(author => AuthorsCollection.Add(author));
                }
                else
                {
                    Branch branch=null;
                    Repository repository=null;
                    var authors =
                        session.QueryOver<Commit>()
                            .SelectList(list => list.SelectGroup(commit => commit.Author))
                            .JoinQueryOver(commit => commit.Branches, () => branch)
                            .JoinQueryOver(() => branch.Repository, () => repository)
                            .Where(() => repository.Name == SelectedRepo).List<string>();
                    authors.ForEach(author => AuthorsCollection.Add(author));
                }
                
            }
        }

        private void OnLoad()
        {

            GetRepositories();
            GetBranches();
            GetAuthors();
            ClearFilters();
        }

        private void SendFilteredData()
        {
            IQueryOver<Commit> query = GenerateQuery(DbService.Instance.SessionFactory.OpenSession());
            ExecuteRefresh(query);
        }


        private IQueryOver<Commit,Commit> GenerateQuery(ISession session)
        {
            bool isCountiuned = false;

            Repository repository = null;
            Branch branch = null;

            var query = session.QueryOver<Commit>();
            if (!String.IsNullOrEmpty(SelectedRepo) && !String.IsNullOrEmpty(SelectedBranch))
            {
                query =
                    query.JoinAlias(commit => commit.Branches, () => branch, JoinType.LeftOuterJoin)
                            .JoinAlias(() => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                            .Where(() => branch.Name == SelectedBranch && repository.Name == SelectedRepo)
                            .TransformUsing(Transformers.DistinctRootEntity).Clone();

                isCountiuned = true;
            }
            else if (!String.IsNullOrEmpty(SelectedRepo) &&
                        String.IsNullOrEmpty(SelectedBranch))
            {
                query =
                    query.JoinAlias(commit => commit.Branches, () => branch, JoinType.LeftOuterJoin)
                        .JoinAlias(() => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                        .Where(() => repository.Name == SelectedRepo)
                        .TransformUsing(Transformers.DistinctRootEntity)
                        .Clone();
                isCountiuned = true;
            }

            bool isAuthor = !String.IsNullOrEmpty(AuthorsSelectedItem);
            bool isFromDate = !String.IsNullOrEmpty(_fromDate);
            bool isToDate = !String.IsNullOrEmpty(_toDate);
            bool isMessage = !String.IsNullOrEmpty(MessageTextBox);

            if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false &&
                isCountiuned == false)
                return session.QueryOver<Commit>();
            if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == false && isCountiuned)
            {
                string selectedBranch = SelectedBranch;
                if (String.IsNullOrEmpty(SelectedBranch))
                    selectedBranch = "master";
                query =
                    query.Where(() => branch.Name == SelectedBranch || branch.Name == "trunk" && repository.Name == SelectedRepo)
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .Clone();
                return query;
            }
            else if (isAuthor == false && isFromDate == false && isToDate == false && isMessage == true)
            {
                query = query.Where(Restrictions.On<Commit>(c => c.Message).IsLike(MessageTextBox, MatchMode.Anywhere));
                return query;
            }

            if (isAuthor == true && isFromDate == false && isToDate == false)
            {
                query = query.Where(commit => commit.Author == AuthorsSelectedItem);
            }
            else if (isAuthor == true && isFromDate == true && isToDate == true)
            {
                query =
                    query.Where(
                        commit =>
                            commit.Author == AuthorsSelectedItem && commit.Date >= DateTime.Parse(_fromDate) &&
                            commit.Date <= DateTime.Parse(_toDate));
            }
            else if (isAuthor == true && isFromDate == true && isToDate == false)
            {
/*                    query += "Author='" + AuthorsSelectedItem + "'" +
                            " AND " +
                            "Date >= " + "'" + _fromDate + "'" +
                            " AND " +
                            "Date <= " + "'" + "2150-01-01" + "'";*/
                query =
                    query.Where(
                        commit => commit.Author == AuthorsSelectedItem && commit.Date >= DateTime.Parse(_fromDate)); //todo check
            }
            else if (isAuthor == true && isFromDate == false && isToDate == true)
            {
/*                    query += "Author='" + AuthorsSelectedItem + "'" +
                            " AND " +
                            "Date >= " + "'" + "1950-01-01" + "'" +
                            " AND " +
                            "Date <= " + "'" + _toDate + "'";*/
                query =
                    query.Where(
                        commit => commit.Author == AuthorsSelectedItem && commit.Date <= DateTime.Parse(_toDate));
                ;
            }
            else if (isAuthor == false && isFromDate == true && isToDate == true)
            {
                query =
                    query.Where(
                        commit => commit.Date >= DateTime.Parse(_fromDate) && commit.Date <= DateTime.Parse(_toDate));
            }
            else if (isAuthor == false && isFromDate == true && isToDate == false)
            {
            /*    query += "Date >= " + "'" + _fromDate + "'" +
                            " AND " +
                            "Date <= " + "'" + "2150-01-01" + "'";*/
                query = query.Where(commit => commit.Date >= DateTime.Parse(_fromDate));
            }
            else if (isAuthor == false && isFromDate == false && isToDate == true)
            {
/*                    query += "Date >= " + "'" + "1950-01-01" + "'" +
                            " AND " +
                            "Date <= " + "'" + _toDate + "'";*/
                query = query.Where(commit => commit.Date <= DateTime.Parse(_toDate));
            }

            if (isMessage)
            {
                query = query.Where(Restrictions.On<Commit>(c=>c.Message).IsLike(MessageTextBox, MatchMode.Anywhere));
            }

            return query;
            
        }


        private void ExecuteRefresh(IQueryOver<Commit> query)
        {
            if(_commitsList != null && _commitsList.Any())
                _commitsList.Clear();

            SendMessageToDisplay();
        }



        private void ClearFilters()
        {
            RepositorySelectedItem = RepositoryCollection.FirstOrDefault() ?? string.Empty;
            BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
            AuthorsSelectedItem = null;
            MessageTextBox = String.Empty;
            FromDate = String.Empty;
            ToDate = String.Empty;
        }
        #endregion

        #region Messages
        private void SendMessageToDisplay()
        {
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                if (_commitsList != null && _commitsList.Any())
                    _commitsList.Clear();

                var commits = GenerateQuery(session).List();
                commits.ForEach(commit=>_commitsList.Add(commit));
            }
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this._commitsList));
        }

        #endregion
    }
}
