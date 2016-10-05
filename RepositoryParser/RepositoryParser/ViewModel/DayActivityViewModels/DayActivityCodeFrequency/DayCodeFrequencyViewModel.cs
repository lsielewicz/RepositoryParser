using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.UserActivityViewModels.UsersActivityCodeFrequency;

namespace RepositoryParser.ViewModel.DayActivityViewModels.DayActivityCodeFrequency
{
    public class DayCodeFrequencyViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        public List<ExtendedChartSeries> AddedLinesChartList;
        public List<ExtendedChartSeries> DeletedLinesChartList;

        public DayAddedChartViewModel AddedChartViewModel { get; set; }
        public DayDeletedChartViewModel DeletedChartViewModel { get; set; }

        public string SummaryString { get; set; }
        public ObservableCollection<CodeFrequencyDataRow> CodeFrequencyDataRows { get; private set; }
        #endregion

        public DayCodeFrequencyViewModel()
        {
            this.AddedLinesChartList = new List<ExtendedChartSeries>();
            this.DeletedLinesChartList = new List<ExtendedChartSeries>();
            this.AddedChartViewModel = new DayAddedChartViewModel();
            this.DeletedChartViewModel = new DayDeletedChartViewModel();
            this.CodeFrequencyDataRows = new ObservableCollection<CodeFrequencyDataRow>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            this.FillData();
        }

        private async void FillData()
        {
            if (this.AddedLinesChartList != null && this.AddedLinesChartList.Any())
                this.AddedLinesChartList.Clear();
            if (this.DeletedLinesChartList != null && this.DeletedLinesChartList.Any())
                this.DeletedLinesChartList.Clear();
            if (this.CodeFrequencyDataRows != null && this.CodeFrequencyDataRows.Any())
                this.CodeFrequencyDataRows.Clear();

            int sumAdded = 0, sumDeleted = 0;

            await Task.Run(new Action(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var addedItemsSource = new List<ChartData>();
                    var deletedItemsSource = new List<ChartData>();

                    var days = Enumerable.Range(1, 31).ToList();

                    Parallel.ForEach(days, day=>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            int added = 0, deleted = 0;

                            Changes changezzz = null;
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                            var changeContents =
                                query.JoinAlias(c => c.Changes, () => changezzz, JoinType.InnerJoin)
                                    .Where(c => c.Date.Day == day)
                                    .SelectList(list => list.Select(() => changezzz.ChangeContent))
                                    .List<string>();

                            changeContents = changeContents.Distinct().ToList();
                            changeContents.ForEach(changeContent =>
                            {
                                var colorServiceT = new DifferencesColoringService(changeContent, string.Empty);
                                colorServiceT.FillColorDifferences();

                                added += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Added);
                                deleted += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Deleted);
                            });
                            sumAdded += added;
                            sumDeleted += deleted;

                            addedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = day.ToString(),
                                ChartValue = added
                            });

                            deletedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = day.ToString(),
                                ChartValue = deleted
                            });

                            if (added != 0 || deleted != 0)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    this.CodeFrequencyDataRows.Add(new CodeFrequencyDataRow()
                                    {
                                        Repository = selectedRepository,
                                        NumericChartKey = day,
                                        AddedLines = added,
                                        DeletedLines = deleted
                                    });
                                });
                            }
                        }
                    });
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.AddSeriesToChartCollection(AddedLinesChartList, selectedRepository, addedItemsSource);
                        this.AddSeriesToChartCollection(DeletedLinesChartList, selectedRepository, deletedItemsSource);
                    });
                });

            
            }));

            SummaryString = this.GetLocalizedString("Added") + ": " + sumAdded + " " +
                            this.GetLocalizedString("Lines") + "\n" +
                            this.GetLocalizedString("Deleted") + ": " + sumDeleted + " " +
                            this.GetLocalizedString("Lines");
            this.RaisePropertyChanged("SummaryString");

            this.AddedChartViewModel.RedrawChart(this.AddedLinesChartList);
            this.DeletedChartViewModel.RedrawChart(this.DeletedLinesChartList);
            this.IsLoading = false;
        }

    }
}
