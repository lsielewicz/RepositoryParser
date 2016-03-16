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
    public class DayActivityViewModel : ViewModelBase
    {
        #region Variables

        private GitRepository gitRepository;
        private string filteringQuery;
        private ObservableCollection<KeyValuePair<int, int>> keyCollection;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        #endregion

        #region Constructor

        public DayActivityViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.RepoInstance, x.FilteringQuery));
            KeyCollection = new ObservableCollection<KeyValuePair<int, int>>();
            ExportFileCommand = new RelayCommand(ExportFile);
        }
        #endregion

        #region Getters/Setters

        public ObservableCollection<KeyValuePair<int, int>> KeyCollection
        {
            get
            {
                return keyCollection;
            }
            set
            {
                if (keyCollection != value)
                {
                    keyCollection = value;
                    RaisePropertyChanged("KeyCollection");
                }
            }
        }
        #endregion
        #region Methods

        private void HandleDataMessage(GitRepository repo, string query)
        {
            this.gitRepository = repo;
            this.filteringQuery = query;
            FillDataCollection();
        }

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

                string query = "SELECT COUNT(GitCommits.ID) AS \"DayCommits\" FROM GitCommits";
                if (string.IsNullOrEmpty(MatchQuery(filteringQuery)))
                {
                    query += " where strftime('%d', Date) = " +
                             "'" + dateString + "'";
                }
                else
                {
                    query += MatchQuery(filteringQuery) +
                             "and strftime('%d', Date) =" +
                             "'" + dateString + "'";
                }
                SQLiteCommand command = new SQLiteCommand(query, gitRepository.SqLiteInstance.Connection);
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
            Regex r = new Regex(@"(select \* from GitCommits)(.*)", RegexOptions.IgnoreCase);
            Match m = r.Match(query);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                    query = m.Groups[2].Value;
            }
            return query;
        }
        #endregion

        #region Buttons
        public ICommand ExportFileCommand { get; set; }

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
                MessageBox.Show("Pomyslnie wyeksportowano", "Eksport");
            }
        }
        #endregion
    }
}
