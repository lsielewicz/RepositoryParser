using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel;

namespace RepositoryParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            StaticServiceProvider.InitializeServices(this);
            InitializeComponent();

        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            CancellationToken token = new CancellationToken();
            TaskScheduler uiSched = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew<Task>(async () =>
            {
                MetroDialogSettings settings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = (this.DataContext as MainViewModel).GetLocalizedString("Yes"),
                    NegativeButtonText = (this.DataContext as MainViewModel).GetLocalizedString("No"),
                    AnimateShow = true,
                    ColorScheme = MetroDialogColorScheme.Theme
                };
                MessageDialogResult result = await StaticServiceProvider.MetroWindowInstance.ShowMessageAsync(this.Title, (this.DataContext as MainViewModel).GetLocalizedString("DoYouReallyWantToExit"), MessageDialogStyle.AffirmativeAndNegative,settings);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
                Application.Current.Shutdown();

            }, token, TaskCreationOptions.None, uiSched);
        }
    }
}
