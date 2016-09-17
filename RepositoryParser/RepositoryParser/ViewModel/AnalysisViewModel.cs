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
    public class AnalysisViewModel : RepositoryAnalyserViewModelBase
    {
        #region private fields
        private RelayCommand _openMonthActivityCommand;
        private RelayCommand _openUserActivityCommand;
        private RelayCommand _openWeekdayActivityCommand;
        private RelayCommand _openDayActivityCommand;
        private RelayCommand _openHourActivityCommand;
        private RelayCommand _openDifferencesCommand;
        #endregion

        #region Getters/Setters
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
            this.NavigateTo(ViewModelLocator.Instance.Difference);
        }

        private void OpenDayActivity()
        {
            this.NavigateTo(ViewModelLocator.Instance.DayActivityContentProvider);
        }

        private void OpenHourActivity()
        {
            this.NavigateTo(ViewModelLocator.Instance.HourActivityContentProvider);
        }

        private void OpenWeekdayActivity()
        {
           this.NavigateTo(ViewModelLocator.Instance.WeekdayActivityContentProvider);
        }

        private void OpenMonthActivity()
        {
            this.NavigateTo(ViewModelLocator.Instance.MonthActivityContentProvider);
        }

        private void OpenUserActivity()
        {
            this.NavigateTo(ViewModelLocator.Instance.UsersActivityContentProvider);
        }
        #endregion
    }
}
