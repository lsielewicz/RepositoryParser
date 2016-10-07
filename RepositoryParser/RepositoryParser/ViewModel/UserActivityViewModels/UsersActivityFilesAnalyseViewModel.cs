using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class UsersActivityFilesAnalyseViewModel : FilesChartViewModelBase
    {
        private int _countOfAuthors;

        public override async void FillChartData()
        {
            base.FillChartData();

            await Task.Run(() =>
            {
                this.IsLoading = true;
                this.CountOfAuthors = 0;
                Parallel.ForEach(this.SelectedFilePaths, (selectedFilePath) =>
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        Changes changes = null;
                        var query =
                            FilteringHelper.Instance.GenerateQuery(session)
                                .JoinAlias(c => c.Changes, () => changes, JoinType.InnerJoin)
                                .Where(() => changes.Path == selectedFilePath);

                        var authors = GetAuthors(selectedFilePath);
                        this.CountOfAuthors += authors.Count;
                        var itemSource = new List<ChartData>();
                        //Parallel.ForEach(authors, (author) =>
                        authors.ForEach(author =>
                        {
                            var commitsCount =
                                query.Clone()
                                    .Where((commit) => commit.Author == author)
                                    .Select(Projections.CountDistinct<Commit>(x => x.Revision)).FutureValue<int>().Value;
                            itemSource.Add(new ChartData()
                            {
                                RepositoryValue = Path.GetFileName(selectedFilePath),
                                ChartKey = author,
                                ChartValue = commitsCount
                            });
                        });

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.AddSeriesToChartInstance(Path.GetFileName(selectedFilePath), itemSource);
                        });
                    }
                });
            });

            this.DrawChart();
            this.FillDataCollection();
            this.IsLoading = false;
        }

        private List<string> GetAuthors(string selectedFilePath)
        {
            List<string> authors = new List<string>();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                Changes changes = null;
                var authorsIds = FilteringHelper.Instance.GenerateQuery(session)
                    .JoinAlias(c => c.Changes, () => changes, JoinType.InnerJoin)
                    .Where(() => changes.Path == selectedFilePath)
                    .SelectList(list => list.SelectGroup(c => c.Author))
                    .List<string>();
               
                authorsIds.ForEach(author => authors.Add(author));
            }
            return authors;
        }
        public new int CountOfAuthors
        {
            get
            {
                return _countOfAuthors;
            }
            set
            {
                if (_countOfAuthors == value)
                    return;
                _countOfAuthors = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
