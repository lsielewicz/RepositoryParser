using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SQLite;
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
    public class UsersCodeFrequencyViewModel : ViewModelBase
    {
        #region Fields
        private string _filteringQuery;
        private SqLiteService _sqLiteService;
        private DifferencesColoringService _colorService;
        private List<UserCodeFrequency> _userCodeFreqList;
        private BackgroundWorker _dataCalcWorker;
        private bool _progressBarVisibility;
        private ObservableCollection<KeyValuePair<string, int>> _addedLinesCollection;
        private ObservableCollection<KeyValuePair<string, int>> _deletedLinesCollection;
        private ObservableCollection<KeyValuePair<string, int>> _modifiedLinesCollection;
        private ObservableCollection<KeyValuePair<string, int>> _summaryLinesCollection;
        private ObservableCollection<UserCodeFrequency> _codeFreqCollection; 
        private List<KeyValuePair<string, int>> _summaryList;
        private ResourceManager _resourceManager;
        private string _summaryString;
        private RelayCommand _exportFileCommand;
        #endregion

        public UsersCodeFrequencyViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this,x=>HandleDataMessage(x.FilteringQuery));
            _sqLiteService=SqLiteService.GetInstance();
            _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
            _dataCalcWorker =new BackgroundWorker();
            _dataCalcWorker.DoWork += DoDataCalcWork;
            _dataCalcWorker.RunWorkerCompleted += DoDataCalcCompleted;
        }

        #region Getters/Setters

        public string SummaryString
        {
            get { return _summaryString; }
            set
            {
                _summaryString = value;
                RaisePropertyChanged("SummaryString");
            }
        }

        public ObservableCollection<UserCodeFrequency> CodeFreqCollection
        {
            get
            {
                return _codeFreqCollection; 
            }
            set
            {
                if (_codeFreqCollection != value)
                {
                    _codeFreqCollection = value;
                    RaisePropertyChanged("CodeFreqCollection");
                }
            }
        }

        public bool ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                if (_progressBarVisibility != value)
                {
                    _progressBarVisibility = value;
                    RaisePropertyChanged("ProgressBarVisibility");
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, int>> AddedLinesCollection
        {
            get
            {
                return _addedLinesCollection; 
            }
            set
            {
                if (_addedLinesCollection != value)
                {
                    _addedLinesCollection = value;
                    RaisePropertyChanged("AddedLinesCollection");
                }
            }
        }
        public ObservableCollection<KeyValuePair<string, int>> DeletedLinesCollection
        {
            get
            {
                return _deletedLinesCollection;
            }
            set
            {
                if (_deletedLinesCollection != value)
                {
                    _deletedLinesCollection = value;
                    RaisePropertyChanged("DeletedLinesCollection");
                }
            }
        }
        public ObservableCollection<KeyValuePair<string, int>> ModifiedLinesCollection
        {
            get
            {
                return _modifiedLinesCollection;
            }
            set
            {
                if (_modifiedLinesCollection != value)
                {
                    _modifiedLinesCollection = value;
                    RaisePropertyChanged("ModifiedLinesCollection");
                }
            }
        }
        public ObservableCollection<KeyValuePair<string, int>> SummaryLinesCollection
        {
            get
            {
                return _summaryLinesCollection;
            }
            set
            {
                if (_summaryLinesCollection != value)
                {
                    _summaryLinesCollection = value;
                    RaisePropertyChanged("SummaryLinesCollection");
                }
            }
        }

        #endregion

        #region Messages
        private void HandleDataMessage(string filteringQuery)
        {
            this._filteringQuery = filteringQuery;       
            if(!_dataCalcWorker.IsBusy)
                _dataCalcWorker.RunWorkerAsync();
        }
        #endregion

        #region Methods
        private void FillData()
        {
            _userCodeFreqList=new List<UserCodeFrequency>();
            List<string> authorsList = GetAuthors(_filteringQuery);
            int added, deleted, modified;
            int sumAdded, sumDeleted, sumModified;
            sumAdded = sumDeleted = sumModified = added = deleted = modified = 0; ;
            for (int i = 0; i < authorsList.Count; i++)
            {
                added = deleted = modified = 0;

                if (authorsList[i] == String.Empty)
                    continue;

                string query = "select * from Commits ";
                 if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += "where Commits.Author='" + authorsList[i] + "' ";
                }
                else
                {
                    if (authorsList.Count == 1)
                        query += MatchQuery(_filteringQuery);
                    else
                        query += MatchQuery(_filteringQuery) + "and Commits.Author='" + authorsList[i] + "' ";
                }

                SQLiteCommand command = new SQLiteCommand(query, _sqLiteService.Connection);
                SQLiteDataReader reader = command.ExecuteReader();

                List<int> idCommitList = new List<int>();
                while (reader.Read())
                {
                    idCommitList.Add(Convert.ToInt32(reader["ID"]));
                }

                foreach (int idCommit in idCommitList)
                {
                    string query2 =
                        String.Format(
                            "select * from Changes " +
                            "inner join ChangesForCommit on Changes.ID = ChangesForCommit.NR_Change " +
                            "inner join Commits on ChangesForCommit.NR_Commit=Commits.ID " +
                            "where Commits.ID = {0}",
                            idCommit);

                    SQLiteCommand command2 = new SQLiteCommand(query2, _sqLiteService.Connection);
                    SQLiteDataReader reader2 = command2.ExecuteReader();

                    List<ChangesTable> changesList = new List<ChangesTable>();
                    while (reader2.Read())
                    {
                        int id = Convert.ToInt32(reader2["ID"]);
                        string type = Convert.ToString(reader2["Type"]);
                        string path = Convert.ToString(reader2["Path"]);
                        string textA = Convert.ToString(reader2["TextA"]);
                        string textB = Convert.ToString(reader2["TextB"]);
                        
                        changesList.Add(new ChangesTable(id,type,path, textA,textB));
                    }

                    if (changesList.Count > 0)
                    {
                        foreach (var change in changesList)
                        {
                            _colorService = new DifferencesColoringService(change.TextA,change.TextB);
                            _colorService.FillColorDifferences();

                            _colorService.TextAList.ForEach(x =>
                            {
                                if (x.Color == ChangesColorModel.ChangeType.Added)
                                    added++;
                                else if (x.Color == ChangesColorModel.ChangeType.Deleted)
                                    deleted++;
                                else if (x.Color == ChangesColorModel.ChangeType.Modified && !String.IsNullOrWhiteSpace(x.Line))
                                    modified++;
                            });
                        }
                        
                    }
                }
                sumAdded += added;
                sumDeleted += deleted;
                sumModified += modified;
                _userCodeFreqList.Add(new UserCodeFrequency(authorsList[i],added,deleted,modified));
            }
            _summaryList = new List<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>(_resourceManager.GetString("Added"), sumAdded),
                new KeyValuePair<string, int>(_resourceManager.GetString("Deleted"), sumDeleted),
                new KeyValuePair<string, int>(_resourceManager.GetString("Modified"), sumModified)
            };
            SummaryString = _resourceManager.GetString("Added") + ": " + sumAdded + " " + _resourceManager.GetString("Lines") + "\n"+
                            _resourceManager.GetString("Deleted") + ": " + sumDeleted + " " + _resourceManager.GetString("Lines") + "\n" +
                            _resourceManager.GetString("Modified") + ": " + sumModified +" " + _resourceManager.GetString("Lines");
        }

        private void FillCollections()
        {
            AddedLinesCollection=new ObservableCollection<KeyValuePair<string, int>>();
            DeletedLinesCollection = new ObservableCollection<KeyValuePair<string, int>>();
            ModifiedLinesCollection = new ObservableCollection<KeyValuePair<string, int>>();
            SummaryLinesCollection = new ObservableCollection<KeyValuePair<string, int>>();
            CodeFreqCollection = new ObservableCollection<UserCodeFrequency>();
            _userCodeFreqList.ForEach(x =>
            {
                AddedLinesCollection.Add(new KeyValuePair<string, int>(x.User,x.AddedLines));
                DeletedLinesCollection.Add(new KeyValuePair<string, int>(x.User,x.DeletedLines));
                ModifiedLinesCollection.Add(new KeyValuePair<string, int>(x.User,x.ModifiedLines));
                CodeFreqCollection.Add(x);
            });
            _summaryList.ForEach(x=> SummaryLinesCollection.Add(x));
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

            SQLiteCommand command = new SQLiteCommand(query, _sqLiteService.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                newAuthorsList.Add(Convert.ToString(reader["Author"]));
            }
            return newAuthorsList;
        }
        #endregion

        #region BackgroundWorker
        private void DoDataCalcWork(object sender, DoWorkEventArgs e)
        {
            ProgressBarVisibility = true;
            FillData();
        }

        private void DoDataCalcCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillCollections();
            ProgressBarVisibility = false;
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
                if (_userCodeFreqList != null && !String.IsNullOrEmpty(_summaryString))
                {
                    // Save document
                    string filename = dlg.FileName;
                    DataToCsv.CreateSummaryChartCSV(_userCodeFreqList, _summaryString, filename);
                    MessageBox.Show(_resourceManager.GetString("ExportMessage"),
                        _resourceManager.GetString("ExportTitle"));
                }
                else
                {
                    MessageBox.Show("Export Failed", _resourceManager.GetString("ExportTitle"));
                }
            }
        }
        #endregion
    }
}
