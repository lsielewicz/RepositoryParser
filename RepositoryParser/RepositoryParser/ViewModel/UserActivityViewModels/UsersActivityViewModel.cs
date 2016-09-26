using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

/*        private void FillCollection()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (KeyCollection != null && KeyCollection.Any())
                    KeyCollection.Clear();
            }));

            var authors = GetAuthors();
            authors.ForEach(author =>
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var commitCount =
                        query.Where(c => c.Author == author).Select(Projections.RowCount()).FutureValue<int>().Value;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        KeyCollection.Add(new KeyValuePair<string, int>(author, commitCount));
                    }));
                }
            });
        }*/

        public override void FillChartData()
        {
            base.FillChartData();
            FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
            {
                var itemSource = new List<ChartData>();
                var authors = GetAuthors(selectedRepository);
                authors.ForEach(author =>
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        var query = FilteringHelper.Instance.GenerateQuery(session,selectedRepository);
                        var commitsCount = query.Where(c => c.Author == author).Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;
                        
                        itemSource.Add(new ChartData()
                        {
                            RepositoryValue = selectedRepository,
                            ChartKey = author,
                            ChartValue = commitsCount
                        });
                    }
                });
                this.AddSeriesToChartInstance(selectedRepository, itemSource);
            });
            this.DrawChart();
            this.FillDataCollection();
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

/*        public override void OnLoad()
        {
            this.RunAsyncOperation(() =>
            {
                this.IsLoading = true;
                this.FillCollection();
            }, executeUponFinish =>
            {
                IsLoading = false;
            });
            
        }*/

        public int CountOfAuthors
        {
            get
            {
                return ViewModelLocator.Instance.Filtering.AuthorsCollection.Count;
            }
        }
    }
}
