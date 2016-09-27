using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace RepositoryParser.ViewModel
{
    public abstract class RepositoryAnalyserViewModelBase : ViewModelBase
    {
        private bool _isLoading;
        private ViewModelBase _currentViewModel;
        protected readonly ResourceManager ResourceManager;
        protected RepositoryAnalyserViewModelBase()
        {
            ResourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
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
    }
}
