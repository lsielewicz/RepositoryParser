using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate;
using NHibernate.Util;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.View;

namespace RepositoryParser.ViewModel
{
    public class PresentationViewModel : ViewModelBase  
    {
        #region private variables
        private bool _isUndocked;
        private UndockedPresentationWindowView _undockedWindow;
        private ObservableCollection<Commit> _commitsCollection;
        private RelayCommand _refreshCommand;
        private RelayCommand _exportFileCommand;
        private RelayCommand<object> _dockUndockPageCommand;
        private readonly ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        public PresentationView ViewInstance { get; set; }
        #endregion

        public PresentationViewModel()
        {
            CommitsColection = new ObservableCollection<Commit>();
            Messenger.Default.Register<RefreshMessageToPresentation>(this, x=> HandleRefreshMessage(x.Refresh));
            OnLoad();
            Messenger.Default.Register<DataMessageToDisplay>(this, x => HandleDataMessage(x.CommitList));
            RefreshList();
        }

        #region Getters/Setters

        public bool IsDocked
        {
            get { return !_isUndocked; }
        }

        public ObservableCollection<Commit> CommitsColection
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
            CommitsColection.Clear();
            list.ForEach(x => CommitsColection.Add(x));
        }
        #endregion


        #region Methods
        private void RefreshList()
        {
            if(CommitsColection != null && CommitsColection.Any())
                CommitsColection.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var commits = session.QueryOver<Commit>().List<Commit>();
                commits.ForEach(commit=>CommitsColection.Add(commit));
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
                List<Commit> tempList = CommitsColection.ToList();
                DataToCsv.CreateCSVFromGitCommitsList(tempList, filename);
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }

        private void OnLoad()
        {
            try
            {
                RefreshList();
                CommitsColection = new ObservableCollection<Commit>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HandleRefreshMessage(bool refresh)
        {
           // if(refresh)
               // RefreshList();
        }
        #endregion

    }
}
