using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityFilesAnalyseViewModel : FilesChartViewModelBase
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
                        for (int i = 1; i <= 31; i++)
                        {
                            var commitsCount =
                                query.Clone()
                                    .Where((commit) => commit.Date.Day == i)
                                    .Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;
                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = Path.GetFileName(selectedFilePath),
                                ChartKey = TimeSpan.FromHours(i).ToString("hh':'mm"),
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
