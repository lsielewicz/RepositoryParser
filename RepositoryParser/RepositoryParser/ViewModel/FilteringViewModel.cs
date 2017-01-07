using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Util;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Messages;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;
using RepositoryParser.Helpers.Enums;
using RepositoryTypeEnum = RepositoryParser.DataBaseManagementCore.Configuration.RepositoryType;

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
        private RelayCommand _selectedRepositoriesItemChanged;
        private RelayCommand _selectedAuthorsItemChanged;
        private RelayCommand<object> _clearSpecifiedFilterCommand;
        private bool _isInitialized;

        public List<string> SelectedRepositories
        {
            get { return this.FilteringInstance.SelectedRepositories; }
            set
            {
                this.FilteringInstance.SelectedRepositories = value;
                RaisePropertyChanged();
            }
        }

        public List<string> SelectedAuthors
        {
            get { return this.FilteringInstance.SelectedAuthors; }
            set
            {
                this.FilteringInstance.SelectedAuthors = value;
                RaisePropertyChanged();
            }
        }


        public RelayCommand SelectedRepositoriesItemChanged
        {
            get
            {
                return _selectedRepositoriesItemChanged ?? (_selectedRepositoriesItemChanged = new RelayCommand(() =>
                {
                    if (SelectedRepositories == null)
                        return;
                    if (this.SelectedRepositories != null && this.SelectedRepositories.Count == 0)
                    {
                        this.SendMessageToDisplay();
                        this.BranchCollection.Clear();
                        this.AuthorsCollection.Clear();
                        this.RepositoryType = null;
                        this.RaisePropertyChanged("SelectedRepositories");
                        return;
                    }
                    if(this.SelectedRepositories != null && this.SelectedRepositories.Count == 1)
                    {
                        this.BranchesEnabled = true;
                        GetBranches();
                        var repositoryType = RepositoryCollection.FirstOrDefault(x => x == SelectedRepositories.First());
                        if (repositoryType != null)
                        {
                            this.RepositoryType = _repositoryTypeDictionary[SelectedRepositories.First()];
                        }
                        BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
                    }
                    else
                    {
                        if (SelectedRepositories.All(r => _repositoryTypeDictionary[r] == RepositoryTypeEnum.Git))
                            RepositoryType = RepositoryTypeEnum.Git;
                        else if (SelectedRepositories.All(r => _repositoryTypeDictionary[r] == RepositoryTypeEnum.Svn))
                            RepositoryType = RepositoryTypeEnum.Svn;
                        else
                            RepositoryType = "Mixed";
                           
                        this.BranchSelectedItem = null;
                        this.BranchCollection.Clear();
                        this.BranchesEnabled = false;
                    }
                    this.GetAuthors();
                    this.SendMessageToDisplay();
                    this.RaisePropertyChanged("SelectedRepositories");
                }));
            }
        }



        public RelayCommand SelectedAuthorsItemChanged
        {
            get
            {
                return _selectedAuthorsItemChanged ?? (_selectedAuthorsItemChanged = new RelayCommand(() =>
                {
                    this.SendMessageToDisplay();
                    this.RaisePropertyChanged("SelectedAuthors");
                }));
            }
        }
        #endregion

        public FilteringViewModel()
        {
            this.FilteringInstance.SelectedRepositories = new List<string>();
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
                                this.SelectedAuthors = new List<string>();
                                this.SelectedAuthorsItemChanged.Execute(this);
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
            get { return this.FilteringInstance.MessageCriteria; }
            set
            {
                if (this.FilteringInstance.MessageCriteria != value)
                {
                    this.FilteringInstance.MessageCriteria = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
                }
            }
        }

        public string FromDate
        {
            get { return this.FilteringInstance.DateFrom; }
            set
            {
                if (this.FilteringInstance.DateFrom != value)
                {
                    this.FilteringInstance.DateFrom = value;
                    this.SendMessageToDisplay();
                    RaisePropertyChanged();
                }
            }
        }

        public string ToDate
        {
            get { return this.FilteringInstance.DateTo; }
            set
            {
                if (this.FilteringInstance.DateTo != value)
                {
                    this.FilteringInstance.DateTo = value;
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



        public string BranchSelectedItem
        {
            get { return this.FilteringInstance.SelectedBranch; }
            set
            {
                if (this.FilteringInstance.SelectedBranch != value)
                {
                    this.FilteringInstance.SelectedBranch = value;
                    if (this.FilteringInstance.SelectedBranch != null)
                    {
                        this.SendMessageToDisplay();
                    }
                    RaisePropertyChanged();
                }
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
            if (this.FilteringInstance.SelectedRepositories == null || !this.FilteringInstance.SelectedRepositories.Any() ||
                this.FilteringInstance.SelectedRepositories.First() == null)
                return;

            if(BranchCollection != null && BranchCollection.Any())
                BranchCollection.Clear();

                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    Repository repository = null;
                    if (BranchCollection != null && BranchCollection.Any())
                        BranchCollection.Clear();

                    if (string.IsNullOrEmpty(this.FilteringInstance.SelectedRepositories.First()))
                    {
                        var branches =
                            session.QueryOver<Branch>().List<Branch>();
                        branches.ForEach(branch => BranchCollection.Add(branch.Name));
                    }
                    else
                    {
                        var branches =
                            session.QueryOver<Branch>()
                                .JoinAlias(branch => branch.Repository, () => repository, JoinType.InnerJoin)
                                .Where(() => repository.Name == this.FilteringInstance.SelectedRepositories.First())
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

        private async void GetAuthors()
        {
            if (this.AuthorsCollection.Any())
            {
                AuthorsCollection.Clear();
            }

            await Task.Run(() =>
            {
                this.SelectedRepositories.ToList().ForEach(selectedRepository =>
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        if (string.IsNullOrEmpty(selectedRepository))
                        {
                            var authors =
                                session.QueryOver<Commit>()
                                    .SelectList(list => list.SelectGroup(commit => commit.Author))
                                    .List<string>();
                            authors.ForEach(author => AuthorsCollection.Add(author));
                        }
                        else
                        {
                            Branch branch = null;
                            Repository repository = null;
                            var authors =
                                session.QueryOver<Commit>()
                                    .SelectList(list => list.SelectGroup(commit => commit.Author))
                                    .JoinAlias(commit => commit.Branches, () => branch, JoinType.InnerJoin)
                                    .JoinAlias(() => branch.Repository, () => repository, JoinType.InnerJoin)
                                    .Where(() => repository.Name == selectedRepository).List<string>();

                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                authors.ForEach(author => AuthorsCollection.Add(author));
                            }));
                        }

                    }
                });
            });

        }

        public void ResetInitialization()
        {
            _isInitialized = false;
        }

        public override void OnLoad()
        {
            if (!_isInitialized)
            {
                GetRepositories();
                _isInitialized = true;
            }
          
        }

        private void ClearFilters()
        {
            SelectedRepositories = new List<string>();
            this.SelectedRepositoriesItemChanged.Execute(this);
            BranchSelectedItem = BranchCollection.FirstOrDefault() ?? string.Empty;
            SelectedAuthors = new List<string>();
            this.SelectedAuthorsItemChanged.Execute(this);
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

                if (this.SelectedRepositories != null && this.SelectedRepositories.Any())
                {
                    var commits = this.FilteringInstance.GenerateQuery(session).List();
                    commits.ForEach(commit => _commitsList.Add(commit));
                }

            }
            Messenger.Default.Send<DataMessageToDisplay>(new DataMessageToDisplay(this._commitsList));
        }
        #endregion
    }
}
