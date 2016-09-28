using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;
using EnumerableExtensions = NHibernate.Util.EnumerableExtensions;

namespace RepositoryParser.ViewModel
{
    public abstract class FilesChartViewModelBase : ChartViewModelBase
    {
        public ObservableCollection<string> FilesPathsCollection { get; protected set; }
        public List<string> SelectedFilePaths { get; protected set; }

        private RelayCommand _selecteFilePathItemChanged;
        public RelayCommand SelectedFileItemChanged
        {
            get
            {
                return _selecteFilePathItemChanged ?? (_selecteFilePathItemChanged = new RelayCommand(() =>
                {
                    this.FillChartData();
                }));
            }
        }
        private async void FillFilesCollection()
        {
            FilesPathsCollection = new ObservableCollection<string>();
            await Task.Run(() =>
            {
                this.IsLoading = true;
                FilteringHelper.Instance.SelectedRepositories.ForEach(selectedRepository =>
                {
                    using (var session = DbService.Instance.SessionFactory.OpenSession())
                    {
                        Changes changesAlias = null;
                        var changesPaths =
                           FilteringHelper.Instance.GenerateQuery(session, selectedRepository)
                                .JoinAlias(c => c.Changes, () => changesAlias, JoinType.InnerJoin)
                                .SelectList(list => list.Select(() => changesAlias.Path))
                                .List<string>();

                        changesPaths = changesPaths.Distinct().ToList();
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var changePath in changesPaths)
                            {
                                this.FilesPathsCollection.Add(changePath);
                            }
                        }));
                    }
                });
            });

            this.IsLoading = false;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            this.FillFilesCollection();
            this.SelectedFilePaths = new List<string>();
        }

    }
}
