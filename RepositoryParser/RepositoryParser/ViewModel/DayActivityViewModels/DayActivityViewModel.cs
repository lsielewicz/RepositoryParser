using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
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
    public class DayActivityViewModel : ViewModelBase
    {
        #region Fields
        private string _filteringQuery;
        private ObservableCollection<KeyValuePair<int, int>> _keyCollection;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private RelayCommand _exportFileCommand;
        #endregion

        #region Constructor
        public DayActivityViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.FilteringQuery));
            KeyCollection = new ObservableCollection<KeyValuePair<int, int>>();
        }
        #endregion

        #region Getters/Setters
        public ObservableCollection<KeyValuePair<int, int>> KeyCollection
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
                    RaisePropertyChanged("KeyCollection");
                }
            }
        }
        #endregion

        #region Messages
        private void HandleDataMessage(string query)
        {
            this._filteringQuery = query;
            FillDataCollection();
        }
        #endregion

        #region Methodds
        private void FillDataCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 1; i <= 31; i++)
            {

                string dateString = "";
                if (i < 10)
                    dateString = "0" + i;
                else
                    dateString = Convert.ToString(i);

                string query = "SELECT COUNT(Commits.ID) AS \"DayCommits\" FROM Commits";
                if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += " where strftime('%d', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(_filteringQuery) +
                             "and strftime('%d', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["DayCommits"]);
                    KeyValuePair<int, int> temp = new KeyValuePair<int, int>(i, count);
                    KeyCollection.Add(temp);
                }
            }
        }
        private string MatchQuery(string query)
        {
            Regex r = new Regex(@"(select \* from Commits)(.*)", RegexOptions.IgnoreCase);
            Match m = r.Match(query);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                    query = m.Groups[2].Value;
            }
            return query;
        }
        #endregion

        #region Buttons getters
        public RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
        }
        #endregion

        #region Buttons action
        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "DayActivityData";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                Dictionary<int, int> tempDictionary = KeyCollection.ToDictionary(a => a.Key, a => a.Value);
                DataToCsv.CreateCSVFromDictionary(tempDictionary, filename);
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion

    }
}
