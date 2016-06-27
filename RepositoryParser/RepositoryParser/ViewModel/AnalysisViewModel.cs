using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;

namespace RepositoryParser.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        #region private fields

        private List<string> _authorsList;
        private string _filteringQuery;
        private RelayCommand _openMonthActivityCommand;
        private RelayCommand _openUserActivityCommand;
        private ViewModelBase _currentViewModel;
        #endregion

        public AnalysisViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x=> HandleDataMessage(x.AuthorsList,x.FilteringQuery, x.IsFromContent));
            _authorsList = new List<string>();

            CurrentViewModel = new ViewModelLocator().MonthActivityContentProvider;
            CurrentViewModel = null;
        }

        #region Getters/Setters
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public RelayCommand OpenMonthActityCommand
        {
            get
            {
                return _openMonthActivityCommand ?? (_openMonthActivityCommand = new RelayCommand(OpenMonthActivity));
            }
        }

        public RelayCommand OpenUserActivityCommand
        {
            get
            {
                return _openUserActivityCommand ?? (_openUserActivityCommand = new RelayCommand(OpenUserActivity));
            }
        }
        #endregion

        #region Methods
        private void OpenMonthActivity()
        {
            CurrentViewModel = new ViewModelLocator().MonthActivityContentProvider;
            Messenger.Default.Send<DataMessageToCharts>(new DataMessageToCharts(_authorsList,_filteringQuery));
        }

        private void OpenUserActivity()
        {
            CurrentViewModel = new ViewModelLocator().UsersActivityContentProvider;
            Messenger.Default.Send<DataMessageToCharts>(new DataMessageToCharts(_authorsList, _filteringQuery));
        }
        #endregion


        #region Messages
        private void HandleDataMessage(List<string> authors, string filternigQuery, bool isTocontent)
        {
            if (isTocontent)
            {
                CurrentViewModel = null;
                _authorsList = authors;
                _filteringQuery = filternigQuery;
            }
        }
        #endregion
    }
}
