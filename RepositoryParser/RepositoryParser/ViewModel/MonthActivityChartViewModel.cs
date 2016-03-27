using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
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
    public class MonthActivityChartViewModel : ViewModelBase
    {
        #region Fields
        private string _authorTextBox;
        private GitRepositoryService _gitRepoInstance;
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private string _filteringQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private RelayCommand _exportFileCommand;
        #endregion

        public MonthActivityChartViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this,
                x => HandleDataMessage(x.RepoInstance, x.FilteringQuery));
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
        }

        #region Getters/Setters
        public string AuthorTextBox
        {
            get { return _authorTextBox; }
            set
            {
                if (_authorTextBox != value)
                {
                    _authorTextBox = value;
                    RaisePropertyChanged("AuthorTextBox");
                }
            }
        }

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
        private void HandleDataMessage(GitRepositoryService gitRepo, string filteringQuery)
        {
            this._gitRepoInstance = gitRepo;
            this._filteringQuery = filteringQuery;
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
                string dateString = "";
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
                SQLiteCommand command = new SQLiteCommand(query, _gitRepoInstance.SqLiteInstance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["MonthCommits"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(GetMonth(i), count);
                    KeyCollection.Add(temp);
                }
            }

        }

        private string GetMonth(int number)
        {
            string Month = "";
            if (number == 1)
                Month = _resourceManager.GetString("Month1");
            else if (number == 2)
                Month = _resourceManager.GetString("Month2");
            else if (number == 3)
                Month = _resourceManager.GetString("Month3");
            else if (number == 4)
                Month = _resourceManager.GetString("Month4");
            else if (number == 5)
                Month = _resourceManager.GetString("Month5");
            else if (number == 6)
                Month = _resourceManager.GetString("Month6");
            else if (number == 7)
                Month = _resourceManager.GetString("Month7");
            else if (number == 8)
                Month = _resourceManager.GetString("Month8");
            else if (number == 9)
                Month = _resourceManager.GetString("Month9");
            else if (number == 10)
                Month = _resourceManager.GetString("Month10");
            else if (number == 11)
                Month = _resourceManager.GetString("Month11");
            else if (number == 12)
                Month = _resourceManager.GetString("Month12");
            return Month;
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
                MessageBox.Show(_resourceManager.GetString("ExportMessage"), _resourceManager.GetString("ExportTitle"));
            }
        }
        #endregion
    }
}
