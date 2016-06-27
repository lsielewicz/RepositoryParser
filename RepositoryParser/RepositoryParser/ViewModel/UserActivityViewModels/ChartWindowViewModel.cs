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
    public class ChartWindowViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private List<string> _authorsList;
        private string _filteringQuery;
        private readonly ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources",Assembly.GetExecutingAssembly());
        private RelayCommand _exportFileCommand;
        #endregion

        #region Constructors
        public ChartWindowViewModel()
        {
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
            Messenger.Default.Register<ChartMessageLevel3UserActivity>(this, x => HandleDataMessage(x.AuthorsList, x.FilteringQuery));
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
                    RaisePropertyChanged("KeyCollection");
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
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion

        #region Methods
        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 0; i < _authorsList.Count; i++)
            {
                if (_authorsList[i] == "")
                    continue;
                string query = "select count(Commits.ID) AS \"AuthorCommits\" from Commits ";
                if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += "where Commits.Author='" + _authorsList[i] + "' ";
                }
                else
                {
                    if (_authorsList.Count == 1)
                        query += MatchQuery(_filteringQuery);
                    else
                        query += MatchQuery(_filteringQuery) + "and Commits.Author='" + _authorsList[i] + "' ";
                }



                SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["AuthorCommits"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(_authorsList[i], count);
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
                {
                    query = m.Groups[2].Value;
                }
            }
            return query;
        }

        private List<string> GetAuthors(string query)
        {
            List<string> newAuthorsList = new List<string>();
            query = "SELECT Author FROM Commits " + MatchQuery(query) + "Group by Author";

            SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                newAuthorsList.Add(Convert.ToString(reader["Author"]));
            }
            return newAuthorsList;
        }

        #endregion

        #region Messsages
        private void HandleDataMessage(List<string> authorsList, string filteringQuery)
        {
            this._filteringQuery = filteringQuery;
            if (this._authorsList != null && authorsList.Count > 0)
                this._authorsList.Clear();
            this._authorsList = GetAuthors(filteringQuery);

            FillCollection();
        }
        #endregion

    }
}
