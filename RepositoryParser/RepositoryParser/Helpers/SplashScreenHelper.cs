using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
