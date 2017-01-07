using GalaSoft.MvvmLight.Command;
using NHibernate.Criterion;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel
{
    public class MainViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openDataBaseManagementCommand;
        private RelayCommand _openPresentationCommand;
        private RelayCommand _openEventCommand;
        private RelayCommand _openFilteringCommand;
        private RelayCommand _openAnalysisCommand;
        private RelayCommand _goToStartScreenCommand;
        private RelayCommand _openSettingsCommand;
        private bool _isDataBaseEmpty;

        public MainViewModel()
        {
            this.FilteringInstance.Initialize();
            CurrentViewModel = ViewModelLocator.Instance.Presentation;
            CurrentViewModel = ViewModelLocator.Instance.Filtering;
            CurrentViewModel = ViewModelLocator.Instance.DataBaseManagement;
            CurrentViewModel = ViewModelLocator.Instance.Analysis;
            CurrentViewModel = null;
        }

        #region Getters setters

        public bool IsDataBaseEmpty
        {
            get
            {
                return _isDataBaseEmpty;
            }
            set
            {
                if (_isDataBaseEmpty == value)
                    return;
                _isDataBaseEmpty = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ?? (_openAnalysisCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.SettingsViewModel);
                }));
            }
        }

        public RelayCommand GoToStartScreenCommand
        {
            get { return _goToStartScreenCommand ?? (_goToStartScreenCommand = new RelayCommand(() =>
            {
                CurrentViewModel = null;
            })); }
        }

        public RelayCommand OpenAnalysisCommand
        {
            get
            {
                return _openAnalysisCommand ?? (_openAnalysisCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.Analysis);
                       }));
            }
        }

        public RelayCommand OpenDataBaseManagementCommand
        {
            get
            {
                return _openDataBaseManagementCommand ??
                       (_openDataBaseManagementCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.DataBaseManagement);
                       }));
            }
        }

        public RelayCommand OpenEventCommand
        {
            get
            {
                return _openEventCommand ?? (_openEventCommand = new RelayCommand(OnLoad));
                
            }
        }

        public RelayCommand OpenPresentationCommand
        {
            get
            {
                return _openPresentationCommand ??
                       (_openPresentationCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.Presentation);
                       }));
            }
        }

        public RelayCommand OpenFilteringCommand
        {
            get { return _openFilteringCommand ?? (_openFilteringCommand = new RelayCommand(() =>
                         {
                             this.NavigateTo(ViewModelLocator.Instance.Filtering);
                         })); }
        }

        #endregion

        public override void OnLoad()
        {
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var repositoriesCount =
                    session.QueryOver<Repository>().Select(Projections.RowCount()).FutureValue<int>().Value;
                this.IsDataBaseEmpty = repositoriesCount == 0;
            }
        }

    }
}