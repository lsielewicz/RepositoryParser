using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.UserActivityViewModels.UsersActivityCodeFrequency
{
    public class UsersCodeFrequencyViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        public List<ExtendedChartSeries> AddedLinesChartList;
        public List<ExtendedChartSeries> DeletedLinesChartList;
        
        public UsersAddedChartViewModel AddedChartViewModel { get; set; }
        public UsersDeletedChartViewModel DeletedChartViewModel { get; set; }
        
        public string SummaryString { get; set; }
        public ObservableCollection<CodeFrequencyDataRow> CodeFrequencyDataRows { get; private set; }
        #endregion

        public UsersCodeFrequencyViewModel()
        {
            this.AddedLinesChartList = new List<ExtendedChartSeries>();
            this.DeletedLinesChartList = new List<ExtendedChartSeries>();
            this.AddedChartViewModel = new UsersAddedChartViewModel();
            this.DeletedChartViewModel = new UsersDeletedChartViewModel();
            CodeFrequencyDataRows = new ObservableCollection<CodeFrequencyDataRow>();
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

            int sumAdded=0, sumDeleted=0;

            await Task.Run(new Action(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var addedItemsSource = new List<ChartData>();
                    var deletedItemsSource = new List<ChartData>();

                    List<string> authors = GetAuthors(selectedRepository);

                    Parallel.ForEach(authors, author =>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            int added = 0, deleted = 0;
                            if (author == string.Empty)
                                return;

                            Changes changezzz = null;
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                            var changeIds =
                                query.JoinAlias(c => c.Changes, () => changezzz, JoinType.InnerJoin)
                                .Where(c => c.Author == author)
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
                                ChartKey = author,
                                ChartValue = added
                            });

                            deletedItemsSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = author,
                                ChartValue = deleted
                            });

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                this.CodeFrequencyDataRows.Add(new CodeFrequencyDataRow()
                                {
                                    Repository = selectedRepository,
                                    ChartKey = author,
                                    AddedLines = added,
                                    DeletedLines = deleted
                                });
                            });
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

        private List<string> GetAuthors(string selectedRepository)
        {
            List<string> authors = new List<string>();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                var authorsIds = query.SelectList(list => list.SelectGroup(c => c.Author)).List<string>();
                authorsIds.ForEach(author => authors.Add(author));
            }
            return authors;
        }
    }
}
