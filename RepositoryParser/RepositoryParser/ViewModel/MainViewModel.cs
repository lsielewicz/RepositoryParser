using System;
using System.Collections.Generic;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NHibernate;
using RepositoryParser.Core.Helpers;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private RelayCommand _openDataBaseManagementCommand;
        private RelayCommand _openPresentationCommand;
        private RelayCommand _closedEventCommand;
        private RelayCommand _openEventCommand;
        private RelayCommand _openFilteringCommand;
        private RelayCommand _openAnalysisCommand;
        private RelayCommand _goToStartScreenCommand;
        private List<string> _authorsList;
        private IQueryOver<Commit> _filteringQuery;
        private bool _isDataBaseEmpty;

        public MainViewModel()
        {
            CurrentViewModel = (new ViewModelLocator()).DataBaseManagement;
            CurrentViewModel = (new ViewModelLocator()).Filtering;
            CurrentViewModel = (new ViewModelLocator()).Presentation;
            CurrentViewModel = (new ViewModelLocator()).Analysis;
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

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
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
                return _openAnalysisCommand ?? (_openAnalysisCommand = new RelayCommand(OpenAnalysis));
            }
        }

        public RelayCommand OpenDataBaseManagementCommand
        {
            get
            {
                return _openDataBaseManagementCommand ??
                       (_openDataBaseManagementCommand = new RelayCommand(OpenDataBaseManagement));
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
                       (_openPresentationCommand = new RelayCommand(OpenPresentation));
            }
        }

        public RelayCommand ClosedEventCommand
        {
            get
            {
                return _closedEventCommand ?? (_closedEventCommand = new RelayCommand(ClosedEvent));
            }
        }

        public RelayCommand OpenFilteringCommand
        {
            get { return _openFilteringCommand ?? (_openFilteringCommand = new RelayCommand(OpenFiltering)); }
        }

        #endregion

        #region Methods

        private void OpenAnalysis()
        {
            CurrentViewModel = ViewModelLocator.Instance.Analysis;
        }

        private void OpenFiltering()
        {
            CurrentViewModel = ViewModelLocator.Instance.Filtering;
           // Messenger.Default.Send<RefreshMessageToFiltering>(new RefreshMessageToFiltering(true));
        }

        private void OnLoad()
        {
            try
            {
                GitService gitRepoService = new GitService();
                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    gitRepoService.ConnectRepositoryToDataBase(true);
                else
                    gitRepoService.ConnectRepositoryToDataBase();

                IsDataBaseEmpty = new DataBaseHelper().IsDataBaseEmpty(SqLiteService.GetInstance().Connection);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void OpenDataBaseManagement()
        {
            CurrentViewModel = ViewModelLocator.Instance.DataBaseManagement;
        }

        private void OpenPresentation()
        {
            CurrentViewModel = ViewModelLocator.Instance.Presentation;  
        }

        private void HandleDataMessage(List<string> authors, IQueryOver<Commit> filternigQuery)
        {
            _authorsList = authors;
            _filteringQuery = filternigQuery;
        }

        private void ClosedEvent()
        {
            CurrentViewModel = null;
        }
        #endregion
    }
}