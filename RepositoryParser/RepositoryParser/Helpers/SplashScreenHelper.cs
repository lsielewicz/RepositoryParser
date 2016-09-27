using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RepositoryParser.Helpers
{
    public class SplashScreenHelper
    {
        public static void StartApplicationWithSplashScreen(App app)
        {
            RepositoryParser.Controls.SplashScreen.SplashScreen splashScreen = new RepositoryParser.Controls.SplashScreen.SplashScreen();
            splashScreen.Show();

            var startupTask = new Task(() =>
            {
                splashScreen.Dispatcher.BeginInvoke(
                    (Action)(() => splashScreen.Message = "Loading Data"));
                //todo load settings
                Properties.Resources.Culture = new CultureInfo("en-En");
            });

            startupTask.ContinueWith(t =>
            {

                MainWindow mainWindow = new MainWindow();
                mainWindow.Loaded += (sender, args) => splashScreen.Close();
                app.MainWindow = mainWindow;
                mainWindow.Show();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupTask.Start();
        }
    }
}
