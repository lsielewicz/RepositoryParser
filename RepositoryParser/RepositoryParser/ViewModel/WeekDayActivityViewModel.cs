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
    public class WeekDayActivityViewModel : ViewModelBase
    {
        #region Variables
        private GitRepository gitRepoInstance;
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private string filteringQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        #endregion


        public WeekDayActivityViewModel()
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

        private void HandleDataMessage(GitRepository gitRepo, string query)
        {
            this.gitRepoInstance = gitRepo;
            this.filteringQuery = query;
            FillCollection();
        }

        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 0; i <= 6; i++)
            {
                string dateString = "";
                dateString = Convert.ToString(i);

                string query = "SELECT COUNT(GitCommits.ID) AS \"WeekdayCommits\" FROM GitCommits";
                if (string.IsNullOrEmpty(MatchQuery(filteringQuery)))
                {
                    query += " where strftime('%w', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(filteringQuery) +
                             "and strftime('%w', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, gitRepoInstance.SqLiteInstance.Connection);
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
    }
}
