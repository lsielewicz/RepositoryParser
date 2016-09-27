using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate.Util;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.View;

namespace RepositoryParser.ViewModel
{
    public class PresentationViewModel : RepositoryAnalyserViewModelBase  
    {
        #region private variables
        private bool _isInitiaized;
        private bool _isUndocked;
        private UndockedPresentationWindowView _undockedWindow;
        private ObservableCollection<Commit> _commitsCollection;
        private RelayCommand _refreshCommand;
        private RelayCommand _exportFileCommand;
        private RelayCommand<object> _dockUndockPageCommand;
        public PresentationView ViewInstance { get; set; }
        #endregion

        public PresentationViewModel()
        {
            CommitsCollection = new ObservableCollection<Commit>();
            Messenger.Default.Register<DataMessageToDisplay>(this, x => HandleDataMessage(x.CommitList));
           // RefreshList();
        }

        #region Getters/Setters

        public bool IsDocked
        {
            get { return !_isUndocked; }
        }

        public ObservableCollection<Commit> CommitsCollection
        {
            get
            {
                return _commitsCollection;

            }
            set
            {
                if (_commitsCollection != value)
                {
                    _commitsCollection = value;
                    RaisePropertyChanged("CommitsCollection");
                }
            }
        }
        #endregion

        #region Buttons getters
        public RelayCommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(RefreshList)); }
        }

        public RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
        }

        public RelayCommand<object> DockUndockPageCommand
        {
            get
            {
                return _dockUndockPageCommand ?? (_dockUndockPageCommand = new RelayCommand<object>((obj) =>
                {
                    if (!_isUndocked)
                    {
                        PresentationView presentationView = obj as PresentationView;
                        _undockedWindow = new UndockedPresentationWindowView();
                        if (presentationView != null)
                        {
                            presentationView.RootGrid.Children.Remove(presentationView.PresentationGrid);
                            _undockedWindow.PresentationGrid.Children.Add(presentationView.PresentationGrid);
                        }

                        _undockedWindow.Show();
                        _isUndocked = true;
                        RaisePropertyChanged("IsDocked");
                    }
                    else
                    {
                        _isUndocked = false;
                        RaisePropertyChanged("IsDocked");
                        UndockedPresentationWindowView undockedView = obj as UndockedPresentationWindowView;
                        if (undockedView != null && ViewInstance != null)
                        {
                            undockedView.PresentationGrid.Children.Remove(ViewInstance.PresentationGrid);
                            ViewInstance.RootGrid.Children.Add(ViewInstance.PresentationGrid);
                        }


                    }
                   
                }));
            }
        }
        #endregion

        #region Messages
        private void HandleDataMessage(List<Commit> list)
        {
            CommitsCollection.Clear();
            list.ForEach(x => CommitsCollection.Add(x));
        }
        #endregion


        #region Methods
        private void RefreshList()
        {
            if(CommitsCollection != null && CommitsCollection.Any())
                CommitsCollection.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var commits = session.QueryOver<Commit>().List<Commit>();
                commits.ForEach(commit=>CommitsCollection.Add(commit));
            }
        }

        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "CommitsFile";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                List<Commit> tempList = CommitsCollection.ToList();
                DataToCsv.CreateCSVFromGitCommitsList(tempList, filename);
                MessageBox.Show(ResourceManager.GetString("ExportMessage"), ResourceManager.GetString("ExportTitle"));
            }
        }

        public override void OnLoad()
        {
            try
            {
                if (!_isInitiaized)
                {
                    _isInitiaized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
