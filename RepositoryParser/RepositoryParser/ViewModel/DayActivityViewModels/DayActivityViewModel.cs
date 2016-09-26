using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate.Criterion;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityViewModel : ChartViewModelBase
    {
        #region Constructor

        public DayActivityViewModel()
        {
        }

        #endregion


        #region Methodds
/*        private void FillDataCollection()
        {
            if (KeyCollection != null && KeyCollection.Any())
                KeyCollection.Clear();
           

                for (int i = 1; i <= 31; i++)
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session);
                        var commitsCount =
                            query.Where(c => c.Date.Day == i).Select(Projections.RowCount()).FutureValue<int>().Value;

                        KeyCollection.Add(new KeyValuePair<int, int>(i, commitsCount));

                    }
                }            
        }*/

        public override void FillChartData()
        {
            base.FillChartData();

            FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
            {
                var itemSource = new List<ChartData>();
                for (int i = 1; i <= 31; i++)
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
                        var commitsCount =
                            query.Where(c => c.Date.Day == i).Select(Projections.RowCount()).FutureValue<int>().Value;

                        itemSource.Add(new ChartData()
                        {
                            RepositoryValue = selectedRepository,
                            ChartKey = i.ToString(),
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


        public override void OnLoad()
        {
            //FillDataCollection();
        }
    }
}
