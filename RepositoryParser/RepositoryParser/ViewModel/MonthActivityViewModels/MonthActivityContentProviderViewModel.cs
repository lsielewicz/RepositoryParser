using GalaSoft.MvvmLight.Command;


namespace RepositoryParser.ViewModel.MonthActivityViewModels
{
    public class MonthActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;

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
