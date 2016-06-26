using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;

namespace RepositoryParser.ViewModel
{
    public class PresentationViewModel : ViewModelBase  
    {
        #region private variables
        private ObservableCollection<CommitTable> _commitsCollection;
        private GitService _gitRepoService;
        private RelayCommand _refreshCommand;
        private RelayCommand _exportFileCommand;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        #endregion


        public PresentationViewModel()
        {
            Messenger.Default.Register<DataMessageToDisplay>(this, x => HandleDataMessage(x.CommitList));
            CommitsColection = new ObservableCollection<CommitTable>();
            RefreshList();
        }

        #region Getters/Setters
        public ObservableCollection<CommitTable> CommitsColection
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
        #endregion

        #region Buttons actions

        #endregion

        #region Messages
        private void HandleDataMessage(List<CommitTable> list)
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
            _gitRepoService.GetDataFromBase().ForEach(x => CommitsColection.Add(x));
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
                List<CommitTable> tempList = CommitsColection.ToList();
                DataToCsv.CreateCSVFromGitCommitsList(tempList, filename);
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion

    }
}
