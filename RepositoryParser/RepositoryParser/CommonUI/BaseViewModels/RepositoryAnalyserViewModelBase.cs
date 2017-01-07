using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using De.TorstenMandelkow.MetroChart;
using GalaSoft.MvvmLight;
using RepositoryParser.Configuration;
using RepositoryParser.Core.Models;
using RepositoryParser.Helpers;

namespace RepositoryParser.CommonUI.BaseViewModels
{
    public abstract class RepositoryAnalyserViewModelBase : ViewModelBase
    {
        private bool _isLoading;
        private ViewModelBase _currentViewModel;
        private readonly ResourceManager _resourceManager;
        protected RepositoryAnalyserViewModelBase()
        {
            _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        }

        public string GetLocalizedString(string resourceKey)
        {
            return this._resourceManager.GetString(resourceKey, ConfigurationService.Instance.CultureInfo);
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

        public FilteringHelper FilteringInstance
        {
            get { return FilteringHelper.Instance; }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value)
                    return;
                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        public virtual void OnLoad()
        {
            CurrentViewModel = null;
        }

        public virtual void NavigateTo(ViewModelBase viewModel)
        {
            this.CurrentViewModel = viewModel;
            var repositoryAnalyserViewModel = viewModel as RepositoryAnalyserViewModelBase;
            if (repositoryAnalyserViewModel != null)
            {
                repositoryAnalyserViewModel.OnLoad();
            }
        }

        public void RunAsyncOperation(Action toExecute, Action<bool> executeUponFinish)
        {
            var taskScheduler1 = TaskScheduler.Default;

            var task = Task.Factory.StartNew(toExecute, CancellationToken.None, TaskCreationOptions.LongRunning,
                taskScheduler1);

            task.ContinueWith(finished =>
            {
                if (!finished.IsFaulted)
                {
                    executeUponFinish(true);
                    return;
                }
            });
        }

        public void AddSeriesToChartCollection(IList collection ,string chartTitle, IEnumerable<ChartData> itemsSource)
        {
            collection.Add(new ExtendedChartSeries()
            {
                ChartTitle = chartTitle,
                ChartSeries = new ChartSeries()
                {
                    Caption = chartTitle,
                    DisplayMember = "ChartKey",
                    ValueMember = "ChartValue",
                    ItemsSource = null,
                    ToolTip = chartTitle
                },
                ItemsSource = itemsSource
            });
        }

        public int CountOfSelectedRepositories
        {
            get
            {
                return this.FilteringInstance.SelectedRepositories.Count;
            }
        }

        public int CountOfAuthors
        {
            get
            {
                return ViewModelLocator.Instance.Filtering.SelectedAuthors.Count != 0
                    ? ViewModelLocator.Instance.Filtering.SelectedAuthors.Count
                    : ViewModelLocator.Instance.Filtering.AuthorsCollection.Count;
            }
        }

        public string GetWeekday(int number)
        {
            string weekday = $"Weekday{number + 1}";
            return this.GetLocalizedString(weekday);
        }

        public string GetMonth(int number)
        {
            string month = $"Month{number}";
            return this.GetLocalizedString(month);
        }

    }
}
