using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class UsersActivityViewModel : ChartViewModelBase
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
                    var authors = GetAuthors(selectedRepository);
                    authors.ForEach(author =>
                    {
                        using (var session = DbService.Instance.SessionFactory.OpenSession())
                        {
                            var query = FilteringHelper.Instance.GenerateQuery(session, selectedRepository);
                            var commitsCount = query.Where(c => c.Author == author).Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;

                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = selectedRepository,
                                ChartKey = author,
                                ChartValue = commitsCount
                            });
                        }
                    });

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

        private List<string> GetAuthors(string selectedRepository)
        {
            List<string> authors = new List<string>();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
                var authorsIds = query.SelectList(list => list.SelectGroup(c => c.Author)).List<string>();
                authorsIds.ForEach(author=>authors.Add(author));
            }
            return authors;
        }

        public int CountOfAuthors
        {
            get
            {
                return ViewModelLocator.Instance.Filtering.AuthorsCollection.Count;
            }
        }
    }
}
