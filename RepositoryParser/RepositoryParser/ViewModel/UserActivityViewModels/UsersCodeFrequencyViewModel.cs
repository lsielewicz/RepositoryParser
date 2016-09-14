using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Util;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Configuration;
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

                    var commits = query.Where(commit => commit.Author == author).List<Commit>();
                    foreach (var commit in commits)
                    {
                        commit.Changes.ForEach(change =>
                        {
                            _colorService = new DifferencesColoringService(change.ChangeContent, string.Empty);
                            _colorService.FillColorDifferences();

                            added += _colorService.TextAList.Count(x => x.Color == ChangeType.Added);
                            deleted += _colorService.TextAList.Count(x => x.Color == ChangeType.Deleted);
                        });
                    }

                    sumAdded += added;
                    sumDeleted += deleted;
                    _userCodeFreqList.Add(new UserCodeFrequency(author, added, deleted));
                }

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
