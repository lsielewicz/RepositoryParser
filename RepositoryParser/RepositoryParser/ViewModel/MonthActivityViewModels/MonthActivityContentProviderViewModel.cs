﻿using GalaSoft.MvvmLight.Command;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;


namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _openFilesAnalyseCommand;
        private RelayCommand _openContiniousAnalyseCommand;
        private RelayCommand _openCodeFrequencyCommand;

        public RelayCommand OpenCodeFrequencyCommand
        {
            get
            {
                return this._openCodeFrequencyCommand ?? (this._openCodeFrequencyCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.MonthCodeFrequencyViewModel);
                }));
            }
        }

        public RelayCommand OpenContiniousAnalyseCommand
        {
            get
            {
                return this._openContiniousAnalyseCommand ?? (_openContiniousAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.MonthActivityContiniousAnalyseViewModel);
                }));
            }
        }

        public RelayCommand OpenFilesAnalyseCommand
        {
            get
            {
                return _openFilesAnalyseCommand ?? (_openFilesAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.MonthActivityFilesAnalyseViewModel);
                }));
            }
        }

        public RelayCommand OpenChartViewCommand
        {
            get
            {
                return _openChartViewCommand ??
                       (_openChartViewCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.MonthActivity);
                       }));
            }
        }


    }
}
