using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class UsersActivityContentProverViewModel : ViewModelBase
    {

        private ViewModelBase _currentViewModel;
        private RelayCommand _openChartViewCommand;
        private RelayCommand _openCodeFrequencyCommand;
        private RelayCommand _closedEventCommand;
        private List<string> _authorsList;
        private string _filteringQuery; 

        public UsersActivityContentProverViewModel()
        {
            Messenger.Default.Register<ChartMessageLevel2>(this, x=> HandleDataMessage(x.AuthorsList, x.FilteringQuery));
            CurrentViewModel = new ViewModelLocator().UsersCodeFrequency;
            CurrentViewModel = new ViewModelLocator().Chart;
            CurrentViewModel = null;
        }

        #region Getters setters
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public RelayCommand OpenChartViewCommand
        {
            get
            {
                return _openChartViewCommand ??
                       (_openChartViewCommand = new RelayCommand(OpenChartView));
            }
        }

        public RelayCommand OpenCodeFrequencyCommand
        {
            get
            {
                return _openCodeFrequencyCommand ?? (_openCodeFrequencyCommand = new RelayCommand(OpenCodeFrequency));
            }
        }

        public RelayCommand ClosedEventCommand
        {
            get
            {
                return _closedEventCommand ?? (_closedEventCommand = new RelayCommand(ClosedEvent));
            }
        }
        #endregion

        #region Methods

        private void OpenChartView()
        {
            CurrentViewModel = (new ViewModelLocator()).Chart;
            Messenger.Default.Send<ChartMessageLevel3UserActivity>(new ChartMessageLevel3UserActivity(_authorsList, _filteringQuery));
        }

        private void OpenCodeFrequency()
        {
            CurrentViewModel = (new ViewModelLocator()).UsersCodeFrequency;
            Messenger.Default.Send<ChartMessageLevel3UserFrequencyCode>(new ChartMessageLevel3UserFrequencyCode(_filteringQuery));
        }

        private void HandleDataMessage(List<string> authors, string filternigQuery)
        {
            CurrentViewModel = null;
            _authorsList = authors;
            _filteringQuery = filternigQuery;
        }

        private void ClosedEvent()
        {
            CurrentViewModel = null;
        }
        #endregion
    }
}
