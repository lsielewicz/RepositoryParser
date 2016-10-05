/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RepositoryParser"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using RepositoryParser.ViewModel.DayActivityViewModels;
using RepositoryParser.ViewModel.DayActivityViewModels.DayActivityCodeFrequency;
using RepositoryParser.ViewModel.HourActivityViewModels;
using RepositoryParser.ViewModel.MonthActivityViewModels;
using RepositoryParser.ViewModel.UserActivityViewModels;
using RepositoryParser.ViewModel.UserActivityViewModels.UsersActivityCodeFrequency;
using RepositoryParser.ViewModel.WeekdayActivityViewModels;

namespace RepositoryParser.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Singleton

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new ViewModelLocator();
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<UsersActivityViewModel>();
            SimpleIoc.Default.Register<MonthActivityChartViewModel>();
            SimpleIoc.Default.Register<DifferenceWindowViewModel>();
            SimpleIoc.Default.Register<DayActivityViewModel>();
            SimpleIoc.Default.Register<WeekDayActivityViewModel>();
            SimpleIoc.Default.Register<HourActivityViewModel>();
            SimpleIoc.Default.Register<ChartOfChangesViewModel>();
            SimpleIoc.Default.Register<UsersCodeFrequencyViewModel>();
            SimpleIoc.Default.Register<UsersActivityContentProverViewModel>();
            SimpleIoc.Default.Register<DayActivityContentProviderViewModel>();
            SimpleIoc.Default.Register<HourActivityContentProviderViewModel>();
            SimpleIoc.Default.Register<WeekdayActivityContentProviderViewModel>();
            SimpleIoc.Default.Register<MonthActivityContentProviderViewModel>();
            SimpleIoc.Default.Register<PresentationViewModel>();
            SimpleIoc.Default.Register<DataBaseManagementViewModel>();
            SimpleIoc.Default.Register<FilteringViewModel>();
            SimpleIoc.Default.Register<AnalysisViewModel>();
            SimpleIoc.Default.Register<HourActivityFilesAnalyseViewModel>();
            SimpleIoc.Default.Register<DayActivityFilesAnalyseViewModel>();
            SimpleIoc.Default.Register<MonthActivityFilesAnalyseViewModel>();
            SimpleIoc.Default.Register<UsersActivityFilesAnalyseViewModel>();
            SimpleIoc.Default.Register<WeekdayActivityFilesAnalyseViewModel>();
            SimpleIoc.Default.Register<MonthActivityContiniousAnalyseViewModel>();
            SimpleIoc.Default.Register<WeekdayActivityContiniousAnalyseViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<DayCodeFrequencyViewModel>();
        }

        public DayCodeFrequencyViewModel DayCodeFrequencyViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DayCodeFrequencyViewModel>();
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public WeekdayActivityContiniousAnalyseViewModel WeekdayActivityContiniousAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WeekdayActivityContiniousAnalyseViewModel>();
            }
        }

        public MonthActivityContiniousAnalyseViewModel MonthActivityContiniousAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MonthActivityContiniousAnalyseViewModel>();
            }
        }

        public WeekdayActivityFilesAnalyseViewModel WeekdayActivityFilesAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WeekdayActivityFilesAnalyseViewModel>();
            }
        }

        public UsersActivityFilesAnalyseViewModel UsersActivityFilesAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UsersActivityFilesAnalyseViewModel>();
            }
        }

        public MonthActivityFilesAnalyseViewModel MonthActivityFilesAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MonthActivityFilesAnalyseViewModel>();
            }
        }
        public DayActivityFilesAnalyseViewModel DayActivityFilesAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DayActivityFilesAnalyseViewModel>();
            }
        }

        public HourActivityFilesAnalyseViewModel HourActivityFilesAnalyseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HourActivityFilesAnalyseViewModel>();
            }
        }

        public AnalysisViewModel Analysis
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AnalysisViewModel>();
            }
        }

        public FilteringViewModel Filtering
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FilteringViewModel>();
            }
        }

        public DataBaseManagementViewModel DataBaseManagement
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DataBaseManagementViewModel>();
            }
        }

        public PresentationViewModel Presentation
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PresentationViewModel>();
            }
        }

        public WeekdayActivityContentProviderViewModel WeekdayActivityContentProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WeekdayActivityContentProviderViewModel>();
            }
        }

        public MonthActivityContentProviderViewModel MonthActivityContentProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MonthActivityContentProviderViewModel>();
            }
        }

        public HourActivityContentProviderViewModel HourActivityContentProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HourActivityContentProviderViewModel>();
            }
        }

        public DayActivityContentProviderViewModel DayActivityContentProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DayActivityContentProviderViewModel>();
            }
        }

        public UsersActivityContentProverViewModel UsersActivityContentProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UsersActivityContentProverViewModel>();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public UsersActivityViewModel UsersActivity
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UsersActivityViewModel>();
            }
        }

        public MonthActivityChartViewModel MonthActivity
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MonthActivityChartViewModel>();
            }
        }

        public DifferenceWindowViewModel Difference
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DifferenceWindowViewModel>();
            }
        }

        public DayActivityViewModel DayActivity
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DayActivityViewModel>();

            }
        }

        public WeekDayActivityViewModel WeekdayActivity
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WeekDayActivityViewModel>();
            }
        }

        public HourActivityViewModel HourActivity
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HourActivityViewModel>();

            }
        }

        public ChartOfChangesViewModel ChartOfChanges
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChartOfChangesViewModel>();
                
            }
        }

        public UsersCodeFrequencyViewModel UsersCodeFrequency
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UsersCodeFrequencyViewModel>();
                
            }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}