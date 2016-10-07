﻿using System;
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

namespace RepositoryParser.ViewModel.MonthActivityViewModels.MonthActivityCodeFrequency
{
    public class MonthCodeFrequencyViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        public List<ExtendedChartSeries> AddedLinesChartList;
        public List<ExtendedChartSeries> DeletedLinesChartList;

        public CodeFrequencySubChartViewModel AddedChartViewModel { get; set; }
        public CodeFrequencySubChartViewModel DeletedChartViewModel { get; set; }

        public string SummaryString { get; set; }
        public ObservableCollection<CodeFrequencyDataRow> CodeFrequencyDataRows { get; private set; }
        #endregion

        public MonthCodeFrequencyViewModel()
        {
            this.AddedLinesChartList = new List<ExtendedChartSeries>();
            this.DeletedLinesChartList = new List<ExtendedChartSeries>();
            this.AddedChartViewModel = new CodeFrequencySubChartViewModel();
            this.DeletedChartViewModel = new CodeFrequencySubChartViewModel();
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

                    var months = Enumerable.Range(1, 12).ToList();

                    Parallel.ForEach(months, month =>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            int added = 0, deleted = 0;
                            var commits =
                                FilteringHelper.Instance.GenerateQuery(session, selectedRepository)
                                    .List<Commit>()
                                    .Where(c => c.Date.Month == month).Select(c => c.Id).Distinct().ToList();

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
                                ChartKey = this.GetMonth(month),
                                ChartValue = added,
                                NumericChartValue = month
                            });

                            deletedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = this.GetMonth(month),
                                ChartValue = deleted,
                                NumericChartValue = month
                            });

                            if (added != 0 || deleted != 0)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    this.CodeFrequencyDataRows.Add(new CodeFrequencyDataRow()
                                    {
                                        Repository = selectedRepository,
                                        ChartKey = this.GetMonth(month),
                                        AddedLines = added,
                                        DeletedLines = deleted,
                                        NumericChartKey = month
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
