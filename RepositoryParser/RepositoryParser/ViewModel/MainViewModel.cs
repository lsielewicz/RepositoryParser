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
        private ViewModelBase _currentViewModel;
        private RelayCommand _openDataBaseManagementCommand;
        private RelayCommand _openPresentationCommand;
        private RelayCommand _closedEventCommand;
        private RelayCommand _openEventCommand;
        private RelayCommand _openFilteringCommand;
        private List<string> _authorsList;
        private string _filteringQuery;

        public MainViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.AuthorsList, x.FilteringQuery));

            CurrentViewModel = (new ViewModelLocator()).DataBaseManagement;
            CurrentViewModel = (new ViewModelLocator()).Presentation;
            CurrentViewModel = (new ViewModelLocator()).Filtering;
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

        private void OpenFiltering()
        {
            CurrentViewModel = (new ViewModelLocator()).Filtering;
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
            CurrentViewModel = (new ViewModelLocator()).DataBaseManagement;
        }

        private void OpenPresentation()
        {
            CurrentViewModel = (new ViewModelLocator()).Presentation;
        }


        private void HandleDataMessage(List<string> authors, string filternigQuery)
        {
            CurrentViewModel = null;
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