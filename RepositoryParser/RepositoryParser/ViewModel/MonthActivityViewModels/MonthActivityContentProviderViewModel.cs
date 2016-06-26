﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;

namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityContentProviderViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private RelayCommand _openChartViewCommand;
        private RelayCommand _closedEventCommand;
        private List<string> _authorsList;
        private string _filteringQuery;

        public MonthActivityContentProviderViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleDataMessage(x.AuthorsList, x.FilteringQuery));
            CurrentViewModel = new ViewModelLocator().MonthActivity;
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
           
            Messenger.Default.Send<DataMessageToCharts>(new DataMessageToCharts(_filteringQuery));
            CurrentViewModel = (new ViewModelLocator()).MonthActivity;
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
