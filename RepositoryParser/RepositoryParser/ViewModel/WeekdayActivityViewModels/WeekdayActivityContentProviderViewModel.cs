using GalaSoft.MvvmLight.Command;

namespace RepositoryParser.ViewModel.WeekdayActivityViewModels
{
    public class WeekdayActivityContentProviderViewModel : RepositoryAnalyserViewModelBase
    {
        private RelayCommand _openChartViewCommand;
        private RelayCommand _openFilesAnalyseCommand;
        private RelayCommand _openContiniousAnalyseCommand;

        public RelayCommand OpenContiniousAnalyseCommand
        {
            get
            {
                return _openContiniousAnalyseCommand ?? (_openContiniousAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.WeekdayActivityContiniousAnalyseViewModel);
                }));
            }
        }

        public RelayCommand OpenFilesAnalyseCommand
        {
            get
            {
                return _openFilesAnalyseCommand ?? (_openFilesAnalyseCommand = new RelayCommand(() =>
                {
                    this.NavigateTo(ViewModelLocator.Instance.WeekdayActivityFilesAnalyseViewModel);
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
                           this.NavigateTo(ViewModelLocator.Instance.WeekdayActivity);
                       }));
            }
        }
    }
}
