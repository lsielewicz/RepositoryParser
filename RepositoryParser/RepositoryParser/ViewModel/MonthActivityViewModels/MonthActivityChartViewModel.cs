using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate.Criterion;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityChartViewModel : ChartViewModelBase
    {
        public MonthActivityChartViewModel()
        {
        }

        #region Methods
/*        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();
            for (int i = 1; i <= 12; i++)
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var commitsCount =
                        query.Where(c => c.Date.Month == i).Select(Projections.RowCount()).FutureValue<int>().Value;
                    KeyCollection.Add(new KeyValuePair<string, int>(GetMonth(i),commitsCount));
                }
            }

        }*/

        public override void FillChartData()
        {
            base.FillChartData();
            FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
            {
                var itemSource = new List<ChartData>();
                for (int i = 1; i <= 12; i++)
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
                        var commitsCount =
                            query.Where(c => c.Date.Month == i).Select(Projections.RowCount()).FutureValue<int>().Value;

                        itemSource.Add(new ChartData()
                        {
                            RepositoryValue = selectedRepository,
                            ChartKey = this.GetMonth(i),
                            ChartValue = commitsCount
                        });
                    }
                }

                this.AddSeriesToChartInstance(selectedRepository, itemSource);
            });
            this.DrawChart();
            this.FillDataCollection();
        }

        private string GetMonth(int number)
        {
            string month = $"Month{number}";
            return ResourceManager.GetString(month); 
        }

        #endregion

    }
}
