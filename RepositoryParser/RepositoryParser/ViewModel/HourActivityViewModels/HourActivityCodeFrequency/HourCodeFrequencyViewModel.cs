using System;
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
using RepositoryParser.ViewModel.DayActivityViewModels.DayActivityCodeFrequency;

namespace RepositoryParser.ViewModel.HourActivityViewModels.HourActivityCodeFrequency
{
    public class HourCodeFrequencyViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        public List<ExtendedChartSeries> AddedLinesChartList;
        public List<ExtendedChartSeries> DeletedLinesChartList;

        public HourAddedChartViewModel AddedChartViewModel { get; set; }
        public HourDeletedChartViewModel DeletedChartViewModel { get; set; }

        public string SummaryString { get; set; }
        public ObservableCollection<CodeFrequencyDataRow> CodeFrequencyDataRows { get; private set; }
        #endregion

        public HourCodeFrequencyViewModel()
        {
            this.AddedLinesChartList = new List<ExtendedChartSeries>();
            this.DeletedLinesChartList = new List<ExtendedChartSeries>();
            this.AddedChartViewModel = new HourAddedChartViewModel();
            this.DeletedChartViewModel = new HourDeletedChartViewModel();
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

                    var hours = Enumerable.Range(0,24).ToList();

                    Parallel.ForEach(hours, hour =>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            int added = 0, deleted = 0;

                            Changes changezzz = null;
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                            var changeIds =
                               query.JoinAlias(c => c.Changes, () => changezzz, JoinType.InnerJoin)
                               .Where(c => c.Date.Hour == hour)
                               .SelectList(list => list.SelectGroup(() => changezzz.Id))
                               .List<int>();

                            changeIds.ForEach(changeId =>
                            {
                                using (var session2 = DbService.Instance.SessionFactory.OpenSession())
                                {
                                    var getChangeContent =
                                    session2.QueryOver<Changes>()
                                       .Where(c => c.Id == changeId)
                                       .Select(c => c.ChangeContent)
                                       .SingleOrDefault<string>();

                                    var colorServiceT = new DifferencesColoringService(getChangeContent, string.Empty);
                                    colorServiceT.FillColorDifferences();

                                    added += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Added);
                                    deleted += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Deleted);
                                }

                            });

                            sumAdded += added;
                            sumDeleted += deleted;

                            addedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = TimeSpan.FromHours(hour).ToString("hh':'mm"),
                                ChartValue = added,
                                NumericChartValue= hour
                            });

                            deletedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = TimeSpan.FromHours(hour).ToString("hh':'mm"),
                                ChartValue = deleted,
                                NumericChartValue = hour
                            });

                            if (added != 0 || deleted != 0)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    this.CodeFrequencyDataRows.Add(new CodeFrequencyDataRow()
                                    {
                                        Repository = selectedRepository,
                                        ChartKey = TimeSpan.FromHours(hour).ToString("hh':'mm"),
                                        AddedLines = added,
                                        DeletedLines = deleted,
                                        NumericChartKey = hour
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
