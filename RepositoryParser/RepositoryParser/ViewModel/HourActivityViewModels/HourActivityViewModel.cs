using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using De.TorstenMandelkow.MetroChart;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NHibernate.Criterion;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;


namespace RepositoryParser.ViewModel.HourActivityViewModels
{
    public class HourActivityViewModel : ChartViewModelBase
    {
        #region Variables
        private RelayCommand _exportFileCommand;
        #endregion

        public HourActivityViewModel()
        {
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        #region Methods

        public override async void FillChartData()
        {
            base.FillChartData();

            await Task.Run(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var itemSource = new List<ChartData>();
                    for (int i = 0; i <= 23; i++)
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
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
            }, CancellationToken.None);

            this.DrawChart();
            this.FillDataCollection();
            this.IsLoading = false;
        }

        #endregion
    }
}
