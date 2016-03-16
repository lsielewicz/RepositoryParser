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
using System.Windows.Controls.DataVisualization.Charting;
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
    public class ChartWindowViewModel : ViewModelBase
    {
        #region Variables
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private ObservableCollection<PieSeries> _columnSeriesCollection;
        private GitRepository localIGitRepository;
        private List<string> authorsList;
        private string filteringQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources",Assembly.GetExecutingAssembly());

        #endregion

        #region Constructors
        public ChartWindowViewModel()
        {
            //authorsList=new List<string>();
            ExportFileCommand = new RelayCommand(ExportFile);
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.RepoInstance, x.AuthorsList, x.FilteringQuery));
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

        #region Buttons
        public ICommand ExportFileCommand { get; set; }
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
            for (int i = 0; i < authorsList.Count; i++)
            {
                if (authorsList[i] == "")
                    continue;
                string query = "select count(GitCommits.ID) AS \"AuthorCommits\" from GitCommits ";
                if (string.IsNullOrEmpty(MatchQuery(filteringQuery)))
                {
                    query += "where Gitcommits.Author='" + authorsList[i] + "' ";
                }
                else
                {
                    if (authorsList.Count == 1)
                        query += MatchQuery(filteringQuery);
                    else
                        query += MatchQuery(filteringQuery) + "and Gitcommits.Author='" + authorsList[i] + "' ";
                }



                SQLiteCommand command = new SQLiteCommand(query, localIGitRepository.SqLiteInstance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader["AuthorCommits"]);
                    KeyValuePair<string, int> temp = new KeyValuePair<string, int>(authorsList[i], count);
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
                {
                    query = m.Groups[2].Value;
                }
            }
            return query;
        }

        private List<string> GetAuthors(string query)
        {
            List<string> newAuthorsList = new List<string>();
            query = "SELECT Author FROM GitCommits " + MatchQuery(query) + "Group by Author";

            SQLiteCommand command = new SQLiteCommand(query, localIGitRepository.SqLiteInstance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                newAuthorsList.Add(Convert.ToString(reader["Author"]));
            }
            return newAuthorsList;
        }

        #endregion

        #region Messsages
        private void HandleDataMessage(GitRepository repo, List<string> authorsList, string filteringQuery)
        {
            this.localIGitRepository = repo;
            this.filteringQuery = filteringQuery;
            if (this.authorsList != null && authorsList.Count > 0)
                this.authorsList.Clear();
            this.authorsList = GetAuthors(filteringQuery);

            FillCollection();
        }
        #endregion



    }
}
