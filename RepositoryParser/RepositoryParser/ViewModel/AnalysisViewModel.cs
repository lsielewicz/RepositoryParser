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
        private RelayCommand _openWeekdayActivityCommand;
        private RelayCommand _openDayActivityCommand;
        private RelayCommand _openHourActivityCommand;
        private RelayCommand _openDifferencesCommand;
        private ViewModelBase _currentViewModel;
        #endregion

        public AnalysisViewModel()
        {
            Messenger.Default.Register<ChartMessageLevel1>(this, x=> HandleDataMessage(x.AuthorsList,x.FilteringQuery));
            _authorsList = new List<string>();

            CurrentViewModel = new ViewModelLocator().MonthActivityContentProvider;
            CurrentViewModel = new ViewModelLocator().UsersActivityContentProvider;
            CurrentViewModel = new ViewModelLocator().WeekdayActivityContentProvider;
            CurrentViewModel = new ViewModelLocator().HourActivityContentProvider;
            CurrentViewModel = new ViewModelLocator().DayActivityContentProvider;
            CurrentViewModel = new ViewModelLocator().Difference;
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

        public RelayCommand OpenDifferenesCommand
        {
            get
            {
                return _openDifferencesCommand ?? (_openDifferencesCommand = new RelayCommand(OpenDifferences));
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

        public RelayCommand OpenWeekdayActivityCommand
        {
            get
            {
                return _openWeekdayActivityCommand ?? (_openWeekdayActivityCommand = new RelayCommand(OpenWeekdayActivity));
            }
        }

        public RelayCommand OpenDayActivityCommand
        {
            get
            {
                return _openDayActivityCommand ?? (_openDayActivityCommand = new RelayCommand(OpenDayActivity));
            }
        }

        public RelayCommand OpenHourActivityCommand
        {
            get
            {
                return _openHourActivityCommand ?? (_openHourActivityCommand = new RelayCommand(OpenHourActivity));
            }
        }
        #endregion

        #region Methods

        private void OpenDifferences()
        {
            CurrentViewModel = new ViewModelLocator().Difference;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_filteringQuery));
        }

        private void OpenDayActivity()
        {
            CurrentViewModel = new ViewModelLocator().DayActivityContentProvider;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_authorsList, _filteringQuery));
        }

        private void OpenHourActivity()
        {
            CurrentViewModel = new ViewModelLocator().HourActivityContentProvider;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_authorsList, _filteringQuery));
        }

        private void OpenWeekdayActivity()
        {
            CurrentViewModel = new ViewModelLocator().WeekdayActivityContentProvider;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_authorsList, _filteringQuery));
        }

        private void OpenMonthActivity()
        {
            CurrentViewModel = new ViewModelLocator().MonthActivityContentProvider;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_authorsList,_filteringQuery));
        }

        private void OpenUserActivity()
        {
            CurrentViewModel = new ViewModelLocator().UsersActivityContentProvider;
            Messenger.Default.Send<ChartMessageLevel2>(new ChartMessageLevel2(_authorsList, _filteringQuery));
        }
        #endregion


        #region Messages
        private void HandleDataMessage(List<string> authors, string filternigQuery)
        {
                CurrentViewModel = null;
                _authorsList = authors;
                _filteringQuery = filternigQuery;
        }
        #endregion
    }
}
