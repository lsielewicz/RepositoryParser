using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.HourActivityViewModels.HourActivityCodeFrequency
{
    public class HourCodeFrequencyViewModel : CodeFrequencyViewModelBase
    {
        public override async void FillData()
        {
            this.ClearCollections();
            int sumAdded = 0, sumDeleted = 0;

            await Task.Run(new Action(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var addedItemsSource = new List<ChartData>();
                    var deletedItemsSource = new List<ChartData>();

                    var hours = Enumerable.Range(0, 24).ToList();

                    Parallel.ForEach(hours, hour =>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            int added = 0, deleted = 0;
                            var commits =
                                FilteringHelper.Instance.GenerateQuery(session, selectedRepository)
                                    .List<Commit>()
                                    .Where(c => c.Date.Hour == hour).Select(c => c.Id).Distinct().ToList();

                            using (var session2 = DbService.Instance.SessionFactory.OpenSession())
                            {
                                Commit commitAlias = null;
                                var changes =
                                    session2.QueryOver<Changes>()
                                        .JoinAlias(change => change.Commit, () => commitAlias, JoinType.InnerJoin)
                                        .WhereRestrictionOn(() => commitAlias.Id).IsIn(commits).List<Changes>();

                                changes.ForEach(change =>
                                {
                                    var colorServiceT = new DifferencesColoringService(change.ChangeContent, string.Empty);
                                    colorServiceT.FillColorDifferences();

                                    added += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Added);
                                    deleted += colorServiceT.TextAList.Count(x => x.Color == ChangeType.Deleted);
                                });
                            }

                            sumAdded += added;
                            sumDeleted += deleted;

                            addedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = TimeSpan.FromHours(hour).ToString("hh':'mm"),
                                ChartValue = added,
                                NumericChartValue = hour
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
