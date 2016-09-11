using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace RepositoryParser.ViewModel
{
    public abstract class RepositoryAnalyserViewModelBase : ViewModelBase
    {
        private bool _isLoading;
        private ViewModelBase _currentViewModel;

        protected RepositoryAnalyserViewModelBase()
        {
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

        public abstract void OnLoad();

    }
}
