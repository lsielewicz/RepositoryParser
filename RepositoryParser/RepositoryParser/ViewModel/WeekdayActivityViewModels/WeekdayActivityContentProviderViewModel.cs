using GalaSoft.MvvmLight.Command;

namespace RepositoryParser.ViewModel.WeekdayActivityViewModels
{
    public class WeekdayActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;

        public RelayCommand OpenChartViewCommand
        {
            get
            {
                return _openChartViewCommand ??
                       (_openChartViewCommand = new RelayCommand(() =>
                       {
                           this.NavigateTo(ViewModelLocator.Instance.WeekdayActivity);
                       }));
            }
        }
    }
}
