
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using RepositoryParser.Controls.SplashScreen;
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
