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
using NHibernate.Util;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.WeekdayActivityViewModels
{
    public class WeekDayActivityViewModel : ChartViewModelBase
    {


/*        private void FillCollection()
        {
            if (KeyCollection.Count > 0)
                KeyCollection.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var query = FilteringHelper.Instance.GenerateQuery(session);
                var commitsDates = query.Select(c => c.Date).List<DateTime>();
                for (int i = 0; i <= 6; i++)
                {
                    int commitsCount = commitsDates.Count(date => (int) date.DayOfWeek == i);
                    KeyCollection.Add(new KeyValuePair<string, int>(GetWeekday(i), commitsCount));
                }
            }
        }*/

        public override void FillChartData()
        {
            base.FillChartData();
            FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
            {
                var itemSource = new List<ChartData>();
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
                    var commitsDates = query.List<Commit>();
                    for (int i = 0; i <= 6; i++)
                    {
                        int commitsCount = commitsDates.Distinct().Count(commit=>(int)commit.Date.DayOfWeek == i);

                        itemSource.Add(new ChartData()
                        {
                            RepositoryValue = selectedRepository,
                            ChartKey = this.GetWeekday(i),
                            ChartValue = commitsCount
                        });
                    }
                }
                this.AddSeriesToChartInstance(selectedRepository, itemSource);
            });
            this.DrawChart();
            this.FillDataCollection();
        }

        private string GetWeekday(int number)
        {
            string weekday = $"Weekday{number+1}";
            return ResourceManager.GetString(weekday);
        }
 
    }
}
