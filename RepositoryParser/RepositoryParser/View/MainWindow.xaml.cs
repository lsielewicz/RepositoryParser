using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.Helpers;

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
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    AnimateShow = true,
                    ColorScheme = MetroDialogColorScheme.Theme
                };
                MessageDialogResult result = await StaticServiceProvider.MetroWindowInstance.ShowMessageAsync(this.Title, "Do you really want to exit?", MessageDialogStyle.AffirmativeAndNegative,settings);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
                Application.Current.Shutdown();

            }, token, TaskCreationOptions.None, uiSched);
        }
    }
}
