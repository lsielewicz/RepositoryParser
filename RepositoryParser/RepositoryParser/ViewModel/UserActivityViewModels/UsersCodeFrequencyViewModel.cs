using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
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

            int sumAdded=0, sumDeleted=0;

            Parallel.ForEach(authors, author =>
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    int added= 0, deleted=0;
                    if (author == string.Empty)
                        return;

                    Changes changezzz = null;
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var changeContents =
                        query.JoinAlias(c => c.Changes, () => changezzz, JoinType.InnerJoin)
                            .Where(c => c.Author == author)
                            .SelectList(list => list.Select(() => changezzz.ChangeContent))
                            .List<string>();

                    changeContents.ForEach(changeContent =>
                    {
                        var colorServiceT = new DifferencesColoringService(changeContent, string.Empty);
                        colorServiceT.FillColorDifferences();

                        added += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Added);
                        deleted += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Deleted);
                    });
                    sumAdded += added;
                    sumDeleted += deleted;
                    _userCodeFreqList.Add(new UserCodeFrequency(author, added, deleted));
                }
            });


            _summaryList = new List<KeyValuePair<string, int>>()
                {
                    new KeyValuePair<string, int>(this.GetLocalizedString("Added"), sumAdded),
                    new KeyValuePair<string, int>(this.GetLocalizedString("Deleted"), sumDeleted)
                };
            SummaryString = this.GetLocalizedString("Added") + ": " + sumAdded + " " +
                            this.GetLocalizedString("Lines") + "\n" +
                            this.GetLocalizedString("Deleted") + ": " + sumDeleted + " " +
                            this.GetLocalizedString("Lines");
            
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

        private async void ExportFile()
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

                    await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                    {
                        MetroWindow = StaticServiceProvider.MetroWindowInstance,
                        DialogTitle = this.GetLocalizedString("Information"),
                        DialogMessage = this.GetLocalizedString("ExportMessage"),
                        OkButtonMessage = "Ok",
                        InformationType = InformationType.Information
                    });
                }
                else
                {
                    await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                    {
                        MetroWindow = StaticServiceProvider.MetroWindowInstance,
                        DialogTitle = this.GetLocalizedString("Error"),
                        DialogMessage = this.GetLocalizedString("ExportFailed"),
                        OkButtonMessage = "Ok",
                        InformationType = InformationType.Error
                    });
                }
            }
        }
        #endregion
    }
}
