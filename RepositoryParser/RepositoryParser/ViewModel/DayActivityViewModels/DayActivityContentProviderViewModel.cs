using GalaSoft.MvvmLight.Command;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _openFilesAnalyseCommand;
        private RelayCommand _openCodeFrequencyCommand;

        #region Getters setters
        public override void OnLoad()
        {
            CurrentViewModel = null;
        }

        public RelayCommand OpenCodeFrequencyCommand
        {
            get
            {
                return _openCodeFrequencyCommand ?? (_openCodeFrequencyCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.DayCodeFrequencyViewModel);
                }));
            }
        }

        public RelayCommand OpenFilesAnalyseCommand
        {
            get
            {
                return _openFilesAnalyseCommand ?? (_openFilesAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.DayActivityFilesAnalyseViewModel);
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
                           this.NavigateTo(ViewModelLocator.Instance.DayActivity);
                       }));
            }
        }
        #endregion

    }
}
