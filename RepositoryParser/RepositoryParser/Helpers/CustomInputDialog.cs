using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace RepositoryParser.Helpers
{
    public class CustomInputDialog : BaseMetroDialog
    {
        public CustomInputDialog(MetroWindow view, MetroDialogSettings settings)
        {
            
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message",typeof(string),typeof(CustomInputDialog),new FrameworkPropertyMetadata(string.Empty));
        public new static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CustomInputDialog), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(string), typeof(CustomInputDialog), new FrameworkPropertyMetadata(string.Empty));

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public new string Title
        {
            get
            {
                return (string) GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty,value);
            }
        }

        public string Input
        {
            get
            {
                return (string) GetValue(InputProperty);
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        static CustomInputDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomInputDialog), new FrameworkPropertyMetadata(typeof(CustomInputDialog)));
        }
    }
}
