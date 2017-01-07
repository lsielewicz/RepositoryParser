using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;


namespace RepositoryParser.ViewModel.HourActivityViewModels
{
    public class HourActivityViewModel : ChartViewModelBase
    {
        public override async void FillChartData()
        {
            base.FillChartData();

            await Task.Run(() =>
            {
                this.IsLoading = true;
                this.FilteringInstance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var itemSource = new List<ChartData>();
                    for (int i = 0; i <= 23; i++)
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            var query = this.FilteringInstance.GenerateQuery(session, selectedRepository);
                            var commitsCount =
                                query.Where(c => c.Date.Hour == i).Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;

                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = TimeSpan.FromHours(i).ToString("hh':'mm"),
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

    }
}
