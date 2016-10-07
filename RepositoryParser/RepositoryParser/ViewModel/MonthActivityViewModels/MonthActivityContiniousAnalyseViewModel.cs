
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using RepositoryParser.CommonUI.BaseViewModels;
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
                List<int> alreadyAddedYears = new List<int>();
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    var itemSource = new List<ChartData>();
                    int maxYear = GetMaxYear(selectedRepository);
                    int minYear = GetMinYear(selectedRepository);

                    if (!CheckIfYearsAreAlreadyAdded(alreadyAddedYears, minYear, maxYear))
                    {
                        int countOfYears = Math.Abs(maxYear - minYear) != 0 ? Math.Abs(maxYear - minYear) : 1;
                        this.CountOfRows += (countOfYears * 12);
                    }

                    for (int year = minYear; year <= maxYear; year++)
                    {
                        alreadyAddedYears.Add(year);
                        for (int month = 1; month <= 12; month++)
                        {
                            using (var session = DbService.Instance.SessionFactory.OpenSession())
                            {
                                var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                                var commitsCount =
                                    query.Where(c => c.Date.Month == month && c.Date.Year == year)
                                        .Select(Projections.CountDistinct<Commit>(x => x.Revision))
                                        .FutureValue<int>()
                                        .Value;

                                itemSource.Add(new ChartData()
                                {
                                    RepositoryValue = selectedRepository,
                                    ChartKey = this.GetMonthAndYear(month,year),
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

        private bool CheckIfYearsAreAlreadyAdded(List<int> list, int minValue, int maxValue)
        {
            bool result = true;
            for (int i = minValue; i <= maxValue; i++)
            {
                result &= list.Contains(i);
            }
            return result;
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
            return $"{this.GetLocalizedString(dateString)} {yearNumber}";
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
