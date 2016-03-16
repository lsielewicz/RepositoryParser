﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;

namespace RepositoryParser.ViewModel
{
    public class HourActivityViewModel : ViewModelBase
    {
        #region Variables
        private GitRepository gitRepoInstance;
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private string filteringQuery;

        #endregion


        public HourActivityViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this,
                x => HandleDataMessage(x.RepoInstance, x.FilteringQuery));
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
            ExportFileCommand = new RelayCommand(ExportFile);
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

        private void HandleDataMessage(GitRepository gitRepo, string filteringQuery)
        {
            this.gitRepoInstance = gitRepo;
            this.filteringQuery = filteringQuery;
            FillCollection();
        }

        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 0; i <= 23; i++)
            {
                string dateString = "";
                if (i < 10)
                    dateString = "0" + i;
                else
                    dateString = Convert.ToString(i);

                string query = "SELECT COUNT(GitCommits.ID) AS \"CommitsHour\" FROM GitCommits";
                if (string.IsNullOrEmpty(MatchQuery(filteringQuery)))
                {
                    query += " where strftime('%H', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(filteringQuery) +
                             "and strftime('%H', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, gitRepoInstance.SqLiteInstance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["CommitsHour"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(dateString, count);
                    KeyCollection.Add(temp);
                }
            }

        }



        private string MatchQuery(string query)
        {
            Regex r = new Regex(@"(select \* from GitCommits)(.*)", RegexOptions.IgnoreCase);
            Match m = r.Match(query);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                    query = m.Groups[2].Value;
            }
            return query;
        }
        public ICommand ExportFileCommand { get; set; }

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
                MessageBox.Show("Pomyslnie wyeksportowano", "Eksport");
            }
        }
    }
}
