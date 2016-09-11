using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _closedEventCommand;


        public DayActivityContentProviderViewModel()
        {
        }

        #region Getters setters


        public override void OnLoad()
        {
            CurrentViewModel = null;
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
            CurrentViewModel = ViewModelLocator.Instance.DayActivity;
            ViewModelLocator.Instance.DayActivity.OnLoad();
        }


        private void ClosedEvent()
        {
            CurrentViewModel = null;
        }
        #endregion
    }
}
