using GalaSoft.MvvmLight.Command;

namespace RepositoryParser.ViewModel.DayActivityViewModels
{
    public class DayActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _closedEventCommand;

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
                       (_openChartViewCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.DayActivity);
                       }));
            }
        }
        #endregion

    }
}
