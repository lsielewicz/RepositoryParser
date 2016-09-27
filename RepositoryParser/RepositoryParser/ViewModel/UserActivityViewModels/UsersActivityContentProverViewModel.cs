using GalaSoft.MvvmLight.Command;

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
