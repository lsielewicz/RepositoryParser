using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        public override void FillChartData()
        {
            base.FillChartData();

            FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
            {
                var itemSource = new List<ChartData>();
                for (int i = 0; i <= 23; i++)
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
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

                this.AddSeriesToChartInstance(selectedRepository, itemSource);
            });
            this.DrawChart();
            this.FillDataCollection();
        }

        #endregion
    }
}
