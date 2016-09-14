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
using RepositoryParser.Helpers;
using RepositoryParser.Helpers.Enums;

namespace RepositoryParser.ViewModel
{
    public class FilteringViewModel : RepositoryAnalyserViewModelBase
    {
        #region private fields
        private ObservableCollection<string> _authorsCollection;
        private ObservableCollection<string> _branchCollection;
        private ObservableCollection<string> _repositoryCollection;
        private readonly Dictionary<string, string> _repositoryTypeDictionary;
        private readonly List<Commit> _commitsList;
        private bool _branchesEnabled;
        private string _repositoryType;
        private RelayCommand _clearFiltersCommand;
        private RelayCommand _sendDataCommand;
        private RelayCommand _onLoadCommand;
        private RelayCommand<object> _clearSpecifiedFilterCommand;
        #endregion

        public FilteringViewModel()
        {
            Messenger.Default.Register<RefreshMessageToFiltering>(this, x=>HandleRefreshMessage(x.Refresh));
            _commitsList = new List<Commit>();
            _repositoryTypeDictionary = new Dictionary<string, string>();

            AuthorsCollection = new ObservableCollection<string>();
            BranchCollection = new ObservableCollection<string>();
            RepositoryCollection = new ObservableCollection<string>();
        }

        #region Buttons getters
        public RelayCommand OnLoadCommand
        {
            get { return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(OnLoad)); }
        }

        public RelayCommand SendDataCommand
        {
            get { return _sendDataCommand ?? (_sendDataCommand = new RelayCommand(SendMessageToDisplay)); }
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
        public string RepositoryType
        {
            get { return _repositoryType; }
            set
            {
                if (_repositoryType != value)
                {
                    _repositoryType = value;
                    RaisePropertyChanged();
                }
            }
        }
        public bool BranchesEnabled
        {
            get { return _branchesEnabled; }
            set
            {
                if (_branchesEnabled != value)
                {
                    _branchesEnabled = value;
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }

        public string MessageTextBox
        {
            get { return FilteringHelper.Instance.MessageCriteria; }
            set
            {
                if (FilteringHelper.Instance.MessageCriteria != value)
                {
                    FilteringHelper.Instance.MessageCriteria = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
                }
            }
        }

        public string FromDate
        {
            get { return FilteringHelper.Instance.DateFrom; }
            set
            {
                if (FilteringHelper.Instance.DateFrom != value)
                {
                    FilteringHelper.Instance.DateFrom = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
                }
            }
        }

        public string ToDate
        {
            get { return FilteringHelper.Instance.DateTo; }
            set
            {
                if (FilteringHelper.Instance.DateTo != value)
                {
                    FilteringHelper.Instance.DateTo = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }

        public string AuthorsSelectedItem
        {
            get { return FilteringHelper.Instance.SelectedAuthor; }
            set
            {
                if (FilteringHelper.Instance.SelectedAuthor != value)
                {
                    FilteringHelper.Instance.SelectedAuthor = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
                }
            }
        }

        public string BranchSelectedItem
        {
            get { return FilteringHelper.Instance.SelectedBranch; }
            set
            {
                if (FilteringHelper.Instance.SelectedBranch != value)
                {
                    FilteringHelper.Instance.SelectedBranch = value;
                    if (FilteringHelper.Instance.SelectedBranch != null)
                    {
                        this.SendMessageToDisplay();
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public string RepositorySelectedItem
        {
            get { return FilteringHelper.Instance.SelectedRepository; }
            set
            {
                FilteringHelper.Instance.SelectedRepository = value;
                if (FilteringHelper.Instance.SelectedRepository != null)
                {
                    BranchesEnabled = true;
                    var firstOrDefault = RepositoryCollection.FirstOrDefault(x => x == FilteringHelper.Instance.SelectedRepository);
                    if (firstOrDefault != null)
                    {
                        RepositoryType = _repositoryTypeDictionary[RepositorySelectedItem];
                    }

                    GetBranches();
                    GetAuthors();
                    BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
                }
                RaisePropertyChanged();
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
            if(BranchCollection != null && BranchCollection.Any())
                BranchCollection.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                Repository repository = null;
                if (BranchCollection != null && BranchCollection.Any())
                    BranchCollection.Clear();

                if (string.IsNullOrEmpty(FilteringHelper.Instance.SelectedRepository))
                {
                    var branches =
                        session.QueryOver<Branch>().List<Branch>();
                    branches.ForEach(branch => BranchCollection.Add(branch.Name));
                }
                else
                {
                    var branches =
                        session.QueryOver<Branch>()
                            .JoinAlias(branch => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                            .Where(() => repository.Name == FilteringHelper.Instance.SelectedRepository)
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
                if (string.IsNullOrEmpty(FilteringHelper.Instance.SelectedRepository))
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
                            .JoinAlias(commit => commit.Branches, () => branch, JoinType.LeftOuterJoin)
                            .JoinAlias(() => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                            .Where(() => repository.Name == FilteringHelper.Instance.SelectedRepository).List<string>();
                    authors.ForEach(author => AuthorsCollection.Add(author));
                }
                
            }
        }

        public override void OnLoad()
        {
            GetRepositories();
            GetBranches();
            GetAuthors();
            ClearFilters();
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

                var commits = FilteringHelper.Instance.GenerateQuery(session).List();
                commits.ForEach(commit=>_commitsList.Add(commit));
            }
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this._commitsList));
        }
        #endregion
    }
}
