using GalaSoft.MvvmLight.Command;

namespace RepositoryParser.ViewModel.HourActivityViewModels
{
    public class HourActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;

        #region Getters setters
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

        #endregion
    }
}
