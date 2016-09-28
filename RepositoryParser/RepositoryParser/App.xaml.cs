using System.Windows;
using RepositoryParser.Helpers;

namespace RepositoryParser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreenHelper.StartApplicationWithSplashScreen(this);
        }
    }
}
