using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityFilesAnalyseViewModel : FilesChartViewModelBase
    {
        public override async void FillChartData()
        {
            base.FillChartData();

            await Task.Run(() =>
            {
                this.IsLoading = true;
                Parallel.ForEach(this.SelectedFilePaths, (selectedFilePath) =>
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        Changes changes = null;
                        var query =
                            FilteringHelper.Instance.GenerateQuery(session)
                                .JoinAlias(c => c.Changes, () => changes, JoinType.InnerJoin)
                                .Where(() => changes.Path == selectedFilePath);
                        var itemSource = new List<ChartData>();
                        for (int i = 1; i <= 12; i++)
                        {
                            var commitsCount =
                                query.Clone()
                                    .Where((commit) => commit.Date.Month == i)
                                    .Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;
                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = Path.GetFileName(selectedFilePath),
                                ChartKey = GetMonth(i),
                                ChartValue = commitsCount
                            });
                        }
                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            this.AddSeriesToChartInstance(Path.GetFileName(selectedFilePath), itemSource);
                        }));
                    }
                });
            });

            this.DrawChart();
            this.FillDataCollection();
            this.IsLoading = false;
        }

    }
}
