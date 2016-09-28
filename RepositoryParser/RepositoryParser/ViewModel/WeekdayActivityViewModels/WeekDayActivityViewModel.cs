using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.WeekdayActivityViewModels
{
    public class WeekDayActivityViewModel : ChartViewModelBase
    {
        public override async void FillChartData()
        {
            base.FillChartData();
            await Task.Run(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var itemSource = new List<ChartData>();
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                        var commitsDates = query.List<Commit>();
                        for (int i = 0; i <= 6; i++)
                        {
                            int commitsCount = commitsDates.Distinct().Count(commit => (int)commit.Date.DayOfWeek == i);

                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = this.GetWeekday(i),
                                ChartValue = commitsCount
                            });
                        }
                    }
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.AddSeriesToChartInstance(selectedRepository, itemSource);
                    }));
                   
                });
            });
            this.DrawChart();
            this.FillDataCollection();
            this.IsLoading = false;
        }

        private string GetWeekday(int number)
        {
            string weekday = $"Weekday{number+1}";
            return ResourceManager.GetString(weekday);
        }
 
    }
}
