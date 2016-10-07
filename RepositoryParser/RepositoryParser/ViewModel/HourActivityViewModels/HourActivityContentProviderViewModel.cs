using GalaSoft.MvvmLight.Command;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;

namespace RepositoryParser.ViewModel.HourActivityViewModels
{
    public class HourActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _openFilesAnalyseCommand;
        private RelayCommand _openCodeFrequencyCommand;
        #region Getters setters

        public RelayCommand OpenCodeFrequencyCommand
        {
            get
            {
                return _openCodeFrequencyCommand ?? (_openCodeFrequencyCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.HourCodeFrequencyViewModel);
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
                           this.NavigateTo(ViewModelLocator.Instance.HourActivity);
                       }));
            }
        }

        public RelayCommand OpenFilesAnalyseCommand
        {
            get
            {
                return _openFilesAnalyseCommand ?? (_openFilesAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.HourActivityFilesAnalyseViewModel);
                }));
            }
        }
        #endregion
    }
}
