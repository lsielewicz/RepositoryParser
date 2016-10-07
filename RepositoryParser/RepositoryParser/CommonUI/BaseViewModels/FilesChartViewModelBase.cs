using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using NHibernate.SqlCommand;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.CommonUI.BaseViewModels
{
    public abstract class FilesChartViewModelBase : ChartViewModelBase
    {
        public ObservableCollection<string> FilesPathsCollection { get; protected set; }
        public List<string> SelectedFilePaths { get; protected set; }

        private ListCollectionView _collectionView;

        public ICollectionView CollectionView
        {
            get { return this._collectionView; }
        }

        private string _pathFilteringCriteria;
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

        public string PathFilteringCriteria
        {
            get
            {
                return _pathFilteringCriteria;
            }
            set
            {
                if (_pathFilteringCriteria == value)
                    return;
                _pathFilteringCriteria = value;

                if (String.IsNullOrEmpty(value))
                    CollectionView.Filter = null;
                else
                {
                    CollectionView.Filter =
                        o =>
                        {
                            if (string.IsNullOrEmpty(_pathFilteringCriteria) || (o as string) == null)
                                return true;
                            var collection =
                                (o as string).IndexOf(PathFilteringCriteria, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                SelectedFilePaths.Any(s=> (o as string).IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
                            
                            return collection;
                        };
                }

                this.RaisePropertyChanged();
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
            _collectionView = new ListCollectionView(FilesPathsCollection);
        }

    }
}
