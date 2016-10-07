using GalaSoft.MvvmLight.Command;
using RepositoryParser.CommonUI;
using RepositoryParser.CommonUI.BaseViewModels;

namespace RepositoryParser.ViewModel.UserActivityViewModels
{
    public class UsersActivityContentProverViewModel : RepositoryAnalyserViewModelBase
    {

        private RelayCommand _openChartViewCommand;
        private RelayCommand _openCodeFrequencyCommand;
        private RelayCommand _openFilesAnalyseCommand;

        public RelayCommand OpenFilesAnalyseCommand
        {
            get
            {
                return _openFilesAnalyseCommand ?? (_openFilesAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.UsersActivityFilesAnalyseViewModel);
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

    }
}
