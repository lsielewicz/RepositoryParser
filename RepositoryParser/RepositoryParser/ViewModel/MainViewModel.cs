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
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.Helpers;
using RepositoryParser.View;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

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
        private string _filteringQuery;

        public MainViewModel()
        {
            Messenger.Default.Register<ChartMessageLevel0>(this, x => HandleDataMessage(x.AuthorsList, x.FilteringQuery));

            CurrentViewModel = (new ViewModelLocator()).DataBaseManagement;
            CurrentViewModel = (new ViewModelLocator()).Filtering;
            CurrentViewModel = (new ViewModelLocator()).Presentation;
            CurrentViewModel = (new ViewModelLocator()).Analysis;
            CurrentViewModel = null;
        }

        #region Getters setters

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
            Messenger.Default.Send<ChartMessageLevel1>(new ChartMessageLevel1(_authorsList,_filteringQuery));
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

        private void HandleDataMessage(List<string> authors, string filternigQuery)
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