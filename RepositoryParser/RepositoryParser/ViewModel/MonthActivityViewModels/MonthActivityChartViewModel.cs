using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate.Criterion;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityChartViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private RelayCommand _exportFileCommand;
        #endregion

        public MonthActivityChartViewModel()
        {

            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
        }

        #region Getters/Setters
        public ObservableCollection<KeyValuePair<string, int>> KeyCollection
        {
            get { return _keyCollection; }
            set
            {
                if (_keyCollection != value)
                {
                    _keyCollection = value;
                    RaisePropertyChanged("KeyCollection");
                }
            }
        }
        #endregion

        #region Messages

        public override void OnLoad()
        {
            FillCollection();
        }
        #endregion

        #region Methods
        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 1; i <= 12; i++)
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var commitsCount =
                        query.Where(c => c.Date.Month == i).Select(Projections.RowCount()).FutureValue<int>().Value;
                    KeyCollection.Add(new KeyValuePair<string, int>(GetMonth(i),commitsCount));
                }
               /* string dateString = "";
                if (i < 10)
                    dateString = "0" + i;
                else
                    dateString = Convert.ToString(i);

                string query = "SELECT COUNT(Commits.ID) AS \"MonthCommits\" FROM Commits";
                if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += " where strftime('%m', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(_filteringQuery) +
                             "and strftime('%m', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["MonthCommits"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(GetMonth(i), count);
                    KeyCollection.Add(temp);
                }*/
            }

        }

        private string GetMonth(int number)
        {
            string month = $"Month{number}";
            return ResourceManager.GetString(month); 
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
            dlg.FileName = "MonthActivityChartView";
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
    }
}
