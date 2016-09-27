using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace RepositoryParser.Helpers
{
    public class StaticServiceProvider
    {
        public static MetroWindow MetroWindowInstance { get; private set; }

        public static Task<MessageDialogResult> ShowMetroMessageAsync(string title, string message, MessageDialogStyle style)
        {
            return MetroWindowInstance.ShowMessageAsync(title, message, style);
        }

        public static Task ShowMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings)
        {
            return MetroWindowInstance.ShowMetroDialogAsync(dialog,settings);
        }

        public static Task<ProgressDialogController> ShowProgressAsync(string title, string message, bool isCancelable=true, MetroDialogSettings metroDialogSettings=null)
        {
            return MetroWindowInstance.ShowProgressAsync(title, message, isCancelable, metroDialogSettings);
        }

        public static Task<LoginDialogData> ShowLoginMessageAsync(string title, string message, LoginDialogSettings loginDialogSettings)
        {
            return MetroWindowInstance.ShowLoginAsync(title, message, loginDialogSettings);
        }

        private StaticServiceProvider()
        {
            
        }

        public static void InitializeServices(MetroWindow metroWindow)
        {
            MetroWindowInstance = metroWindow;
        }

    }
}
