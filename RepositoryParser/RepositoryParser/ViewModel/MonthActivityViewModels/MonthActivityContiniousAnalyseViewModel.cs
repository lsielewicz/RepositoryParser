
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityContiniousAnalyseViewModel : ChartViewModelBase
    {

        public override async void FillChartData()
        {
            base.FillChartData();
            await Task.Run(() =>
            {
                this.IsLoading = true;
                this.CountOfRows = 0;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var itemSource = new List<ChartData>();
                    int maxYear = GetMaxYear(selectedRepository);
                    int minYear = GetMinYear(selectedRepository);

                    int countOfRows = Math.Abs(maxYear - minYear) != 0 ? Math.Abs(maxYear - minYear) : 1;
                    this.CountOfRows += (countOfRows * FilteringHelper.CountOfRepositories);

                    for (int j = minYear; j <= maxYear; j++)
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            using (var session = DbService.Instance.SessionFactory.OpenSession())
                            {
                                var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                                var commitsCount =
                                    query.Where(c => c.Date.Month == i && c.Date.Year == j)
                                        .Select(Projections.CountDistinct<Commit>(x => x.Revision))
                                        .FutureValue<int>()
                                        .Value;

                                itemSource.Add(new ChartData()
                                {
                                    RepositoryValue = selectedRepository,
                                    ChartKey = this.GetMonthAndYear(i,j),
                                    ChartValue = commitsCount
                                });
                            }
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

        private int GetMaxYear(string selectedRepository)
        {
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var maxYear =
                    FilteringHelper.Instance.GenerateQuery(session, selectedRepository)
                        .Select(Projections.ProjectionList().Add(Projections.Max<Commit>(c => c.Date.Year)))
                        .List<int>()
                        .First();
                return maxYear;
            }    
        }

        private int GetMinYear(string selectedRepository)
        {
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var maxYear =
                    FilteringHelper.Instance.GenerateQuery(session, selectedRepository)
                        .Select(Projections.ProjectionList().Add(Projections.Min<Commit>(c => c.Date.Year)))
                        .List<int>()
                        .First();
                return maxYear;
            }
        }
        

        private string GetMonthAndYear(int monthNumber, int yearNumber)
        {
            string dateString = $"Month{monthNumber}";
            return $"{ResourceManager.GetString(dateString)} {yearNumber}";
        }

        private int _countOfRows;
        public int CountOfRows
        {
            get { return _countOfRows; }
            set
            {
                if (_countOfRows == value)
                    return;
                _countOfRows = value;
                RaisePropertyChanged();
            }
        }
    }
}
