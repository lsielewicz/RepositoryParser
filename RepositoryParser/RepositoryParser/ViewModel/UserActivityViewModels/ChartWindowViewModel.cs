using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NHibernate.Criterion;
using NHibernate.Util;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class ChartWindowViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private RelayCommand _exportFileCommand;
        #endregion

        #region Constructors
        public ChartWindowViewModel()
        {
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
        }
        #endregion

        #region Getters/Setters
        public ObservableCollection<KeyValuePair<string, int>> KeyCollection
        {
            get
            {
                return _keyCollection;
            }
            set
            {
                if (_keyCollection != value)
                {
                    _keyCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Buttons getters
        public RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
        }
        #endregion

        #region Buttons actions
        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "UserActivityChartView";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                Dictionary<string, int> tempDictionary = KeyCollection.ToDictionary(a => a.Key, a => a.Value);
                DataToCsv.CreateCSVFromDictionary(tempDictionary, filename);
                MessageBox.Show(ResourceManager.GetString("ExportMessage"), ResourceManager.GetString("ExportTitle"));
            }
        }
        #endregion

        #region Methods
        private void FillCollection()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (KeyCollection != null && KeyCollection.Any())
                    KeyCollection.Clear();
            }));

            var authors = GetAuthors();
            authors.ForEach(author =>
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var commitCount =
                        query.Where(c => c.Author == author).Select(Projections.RowCount()).FutureValue<int>().Value;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        KeyCollection.Add(new KeyValuePair<string, int>(author, commitCount));
                    }));
                }
            });
        }

        private List<string> GetAuthors()
        {
            List<string> authors = new List<string>();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var query = FilteringHelper.Instance.GenerateQuery(session);
                var authorsIds = query.SelectList(list => list.SelectGroup(c => c.Author)).List<string>();
                authorsIds.ForEach(author=>authors.Add(author));
            }
            return authors;
        }
        #endregion

        public override void OnLoad()
        {
            this.RunAsyncOperation(() =>
            {
                this.IsLoading = true;
                this.FillCollection();
            }, executeUponFinish =>
            {
                IsLoading = false;
            });
            
        }
    }
}
