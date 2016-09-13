using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.Core.Enums;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class UsersCodeFrequencyViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields

        private DifferencesColoringService _colorService;
        private readonly BackgroundWorker _dataCalcWorker;
        private ObservableCollection<KeyValuePair<string, int>> _addedLinesCollection;
        private ObservableCollection<KeyValuePair<string, int>> _deletedLinesCollection;
        private ObservableCollection<KeyValuePair<string, int>> _summaryLinesCollection;
        private ObservableCollection<UserCodeFrequency> _codeFreqCollection;
        private List<UserCodeFrequency> _userCodeFreqList;
        private List<KeyValuePair<string, int>> _summaryList;
        private string _summaryString;
        private RelayCommand _exportFileCommand;
        private RelayCommand _closedEventCommand;

        #endregion

        public UsersCodeFrequencyViewModel()
        {
            _dataCalcWorker = new BackgroundWorker();
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
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<UserCodeFrequency> CodeFreqCollection
        {
            get { return _codeFreqCollection; }
            set
            {
                if (_codeFreqCollection != value)
                {
                    _codeFreqCollection = value;
                    RaisePropertyChanged("CodeFreqCollection");
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, int>> AddedLinesCollection
        {
            get { return _addedLinesCollection; }
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
            get { return _deletedLinesCollection; }
            set
            {
                if (_deletedLinesCollection != value)
                {
                    _deletedLinesCollection = value;
                    RaisePropertyChanged("DeletedLinesCollection");
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, int>> SummaryLinesCollection
        {
            get { return _summaryLinesCollection; }
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

        public override void OnLoad()
        {
            if (!_dataCalcWorker.IsBusy)
                _dataCalcWorker.RunWorkerAsync();
        }

        #endregion

        #region Methods

        private void FillData()
        {
            _userCodeFreqList = new List<UserCodeFrequency>();
            List<string> authors = GetAuthors();
            int added, deleted;
            int sumAdded, sumDeleted;
            sumAdded = sumDeleted = added = deleted = 0;

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                session.FlushMode = FlushMode.Never;
                var query1 = FilteringHelper.Instance.GenerateQuery(session);
                foreach (var author in authors)
                {
                    var query = query1.Clone();
                    added = deleted = 0;
                    if (author == string.Empty)
                        continue;

                    /*var commits =
                    FilteringHelper.Instance.GenerateQuery(session)
                        .Where(commit => commit.Author == author).List<Commit>();

                commits.ForEach(commit =>
                {
                    commit.Changes.ForEach(change =>
                    {
                        _colorService = new DifferencesColoringService(change.ChangeContent, string.Empty);
                        _colorService.FillColorDifferences();

                        _colorService.TextAList.ForEach(x =>
                        {
                            if (x.Color == ChangeType.Added)
                                added++;
                            else if (x.Color == ChangeType.Deleted)
                                deleted++;
                        });
                    });
                });*/
                    var commitsIds =
                        query
                            .Where(commit => commit.Author == author)
                            .SelectList(list => list.Select(c => c.Id)).List<int>();

                    foreach (var id in commitsIds)
                    {
                        var changes = session
                            .QueryOver<Changes>()
                            .JoinQueryOver(c => c.Commit)
                            .Where(commit => commit.Id == id)
                            .List<Changes>();
                        changes.ForEach(change =>
                        {
                            _colorService = new DifferencesColoringService(change.ChangeContent, string.Empty);
                            _colorService.FillColorDifferences();

                            _colorService.TextAList.ForEach(x =>
                            {
                                if (x.Color == ChangeType.Added)
                                    added++;
                                else if (x.Color == ChangeType.Deleted)
                                    deleted++;
                            });
                        });

                    }




                    sumAdded += added;
                    sumDeleted += deleted;
                    _userCodeFreqList.Add(new UserCodeFrequency(author, added, deleted));

                    _summaryList = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>(ResourceManager.GetString("Added"), sumAdded),
                        new KeyValuePair<string, int>(ResourceManager.GetString("Deleted"), sumDeleted)
                    };
                    SummaryString = ResourceManager.GetString("Added") + ": " + sumAdded + " " +
                                    ResourceManager.GetString("Lines") + "\n" +
                                    ResourceManager.GetString("Deleted") + ": " + sumDeleted + " " +
                                    ResourceManager.GetString("Lines");
                }

                /* for (int i = 0; i < authorsList.Count; i++)
            {
                added = deleted = 0;

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
                                if (x.Color == ChangeType.Added)
                                    added++;
                                else if (x.Color == ChangeType.Deleted)
                                    deleted++;
                            });
                        }
                        
                    }
                }
                sumAdded += added;
                sumDeleted += deleted;
                _userCodeFreqList.Add(new UserCodeFrequency(authorsList[i],added,deleted));
            }*/
                _summaryList = new List<KeyValuePair<string, int>>()
                {
                    new KeyValuePair<string, int>(ResourceManager.GetString("Added"), sumAdded),
                    new KeyValuePair<string, int>(ResourceManager.GetString("Deleted"), sumDeleted),
                };
                SummaryString = ResourceManager.GetString("Added") + ": " + sumAdded + " " +
                                ResourceManager.GetString("Lines") + "\n" +
                                ResourceManager.GetString("Deleted") + ": " + sumDeleted + " " +
                                ResourceManager.GetString("Lines");

            }
        }


        private void FillCollections()
        {
            AddedLinesCollection= new ObservableCollection<KeyValuePair<string, int>>();
            DeletedLinesCollection = new ObservableCollection<KeyValuePair<string, int>>();
            SummaryLinesCollection = new ObservableCollection<KeyValuePair<string, int>>();
            CodeFreqCollection = new ObservableCollection<UserCodeFrequency>();
            _userCodeFreqList.ForEach(x =>
            {
                AddedLinesCollection.Add(new KeyValuePair<string, int>(x.User,x.AddedLines));
                DeletedLinesCollection.Add(new KeyValuePair<string, int>(x.User,x.DeletedLines));
                CodeFreqCollection.Add(x);
            });
            _summaryList.ForEach(x=> SummaryLinesCollection.Add(x));
        }

        private List<string> GetAuthors()
        {
            List<string> authors = new List<string>();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var query = FilteringHelper.Instance.GenerateQuery(session);
                var authorsIds = query.SelectList(list => list.SelectGroup(c => c.Author)).List<string>();
                authorsIds.ForEach(author => authors.Add(author));
            }
            return authors;
        }
        #endregion

        #region BackgroundWorker
        private void DoDataCalcWork(object sender, DoWorkEventArgs e)
        {
            IsLoading = true;
            FillData();
        }

        private void DoDataCalcCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillCollections();
            IsLoading = false;
        }
        #endregion

        #region Buttons getters
        public RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
        }

        public RelayCommand ClosedEventCommand
        {
            get { return _closedEventCommand ?? (_closedEventCommand = new RelayCommand(ClosedEvent)); }
        }
        #endregion

        #region Buttons actions

        private void ClosedEvent()
        {
            if(AddedLinesCollection!=null)
                this.AddedLinesCollection.Clear();
            if(DeletedLinesCollection!=null)
                this.DeletedLinesCollection.Clear();
            if(CodeFreqCollection!=null)
                this.CodeFreqCollection.Clear();
            if(SummaryLinesCollection!=null)
                this.SummaryLinesCollection.Clear();
            this.SummaryString = string.Empty;
        }

        private void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "CodeFrequencyData";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                var userCodeFreqList = CodeFreqCollection.ToList();
                if (!String.IsNullOrEmpty(_summaryString))
                {
                    // Save document
                    string filename = dlg.FileName;
                    DataToCsv.CreateSummaryChartCSV(userCodeFreqList, _summaryString, filename);
                    MessageBox.Show(ResourceManager.GetString("ExportMessage"),
                        ResourceManager.GetString("ExportTitle"));
                }
                else
                {
                    MessageBox.Show(ResourceManager.GetString("ExportFailed"), ResourceManager.GetString("ExportTitle"));
                }
            }
        }
        #endregion
    }
}
