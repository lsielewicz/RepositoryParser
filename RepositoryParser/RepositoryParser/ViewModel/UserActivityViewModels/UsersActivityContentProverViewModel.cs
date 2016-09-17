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
    public class UsersActivityContentProverViewModel : RepositoryAnalyserViewModelBase
    {

        private RelayCommand _openChartViewCommand;
        private RelayCommand _openCodeFrequencyCommand;

        #region Getters setters
        public RelayCommand OpenChartViewCommand
        {
            get
            {
                return _openChartViewCommand ??
                       (_openChartViewCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.UsersActivity);
                       }));
            }
        }

        public RelayCommand OpenCodeFrequencyCommand
        {
            get
            {
                return _openCodeFrequencyCommand ?? (_openCodeFrequencyCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.UsersCodeFrequency);
                }));
            }
        }
        #endregion
    }
}
