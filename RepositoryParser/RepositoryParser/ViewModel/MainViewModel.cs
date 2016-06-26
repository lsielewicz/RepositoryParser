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
        private static string selectedBranch;
        private static string selectedRepo;
        private ViewModelBase _currentViewModel;
        private RelayCommand _openDataBaseManagementCommand;
        private RelayCommand _openPresentationCommand;
        private RelayCommand _closedEventCommand;
        private RelayCommand _openEventCommand;
        private List<string> _authorsList;
        private string _filteringQuery;

        public MainViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.AuthorsList, x.FilteringQuery));
        }

        #region Getters setters
        public static string SelectedBranch
        {
            get { return selectedBranch; }
            set
            {
                if (selectedBranch != value)
                    selectedBranch = value;
            }
        }

        public static string SelectedRepo
        {
            get { return selectedRepo; }
            set
            {
                if (selectedRepo != value)
                {
                    selectedRepo = value;
                    selectedBranch = "";
                }
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
        #endregion

        #region Methods

        private void OnLoad()
        {
            
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