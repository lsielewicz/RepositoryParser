using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityViewModel : ChartViewModelBase
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
                    for (int i = 1; i <= 31; i++)
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                            var commitsCount =
                                query.Where(c => c.Date.Day == i).Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;

                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = i.ToString(),
                                ChartValue = commitsCount
                            });
                        }
                    }
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.AddSeriesToChartInstance(selectedRepository, itemSource);
                    }));
                });
            },CancellationToken.None);
           
            this.DrawChart();
            this.FillDataCollection();
            this.IsLoading = false;
        }

 
    }
}
