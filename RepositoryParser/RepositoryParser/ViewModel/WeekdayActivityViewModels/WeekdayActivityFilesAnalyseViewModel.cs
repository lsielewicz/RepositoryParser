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

namespace RepositoryParser.ViewModel.WeekdayActivityViewModels
{
    public class WeekdayActivityFilesAnalyseViewModel : FilesChartViewModelBase
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
                        var commits =
                            FilteringHelper.Instance.GenerateQuery(session)
                                .JoinAlias(c => c.Changes, () => changes, JoinType.InnerJoin)
                                .Where(() => changes.Path == selectedFilePath).List<Commit>();
                        var itemSource = new List<ChartData>();
                        //var commits = query
                        for (int i = 0; i <= 6; i++)
                        {
                            int commitsCount = commits.Distinct().Count(commit => (int)commit.Date.DayOfWeek == i);

                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = Path.GetFileName(selectedFilePath),
                                ChartKey = GetWeekday(i),
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

        private string GetWeekday(int number)
        {
            string weekday = $"Weekday{number + 1}";
            return ResourceManager.GetString(weekday);
        }
    }
}
