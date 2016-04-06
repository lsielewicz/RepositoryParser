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
    public class WeekDayActivityViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private string _filteringQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private RelayCommand _exportFileCommand;
        #endregion

        public WeekDayActivityViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this,
                x => HandleDataMessage(x.FilteringQuery));
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
        private void HandleDataMessage(string query)
        {
            this._filteringQuery = query;
            FillCollection();
        }
        #endregion

        #region Methods
        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 0; i <= 6; i++)
            {
                string dateString = "";
                dateString = Convert.ToString(i);

                string query = "SELECT COUNT(Commits.ID) AS \"WeekdayCommits\" FROM Commits";
                if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += " where strftime('%w', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(_filteringQuery) +
                             "and strftime('%w', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["WeekdayCommits"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(GetWeekday(i), count);
                    KeyCollection.Add(temp);
                }
            }

        }

        private string GetWeekday(int number)
        {
            string Weekday = "";
            if (number == 1)
                Weekday = _resourceManager.GetString("Weekday1");
            else if (number == 2)
                Weekday = _resourceManager.GetString("Weekday2");
            else if (number == 3)
                Weekday = _resourceManager.GetString("Weekday3");
            else if (number == 4)
                Weekday = _resourceManager.GetString("Weekday4");
            else if (number == 5)
                Weekday = _resourceManager.GetString("Weekday5");
            else if (number == 6)
                Weekday = _resourceManager.GetString("Weekday6");
            else if (number == 0)
                Weekday = _resourceManager.GetString("Weekday7");
            return Weekday;
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

        #region Buttons actions
        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "WeekdayData";
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
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion
    }
}
