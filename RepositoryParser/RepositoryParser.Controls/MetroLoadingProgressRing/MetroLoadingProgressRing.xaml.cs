using System.Windows;

namespace RepositoryParser.Controls.MetroLoadingProgressRing
{
    /// <summary>
    /// Interaction logic for MetroLoadingProgressRing.xaml
    /// </summary>
    public partial class MetroLoadingProgressRing
    {
        public static DependencyProperty IsDataLoadingFlagProperty = DependencyProperty.Register(
            "IsDataLoadingFlag", 
            typeof(bool),
            typeof(MetroLoadingProgressRing),
            new FrameworkPropertyMetadata(false));

        public static DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", 
            typeof(string),
            typeof(MetroLoadingProgressRing), 
            new PropertyMetadata(string.Empty));

        public bool IsDataLoadingFlag
        {
            get { return (bool)this.GetValue(IsDataLoadingFlagProperty); }
            set { this.SetValue(IsDataLoadingFlagProperty,value);}
        }

        public string Text
        {
            get { return (string) this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty,value);}
        }

        public MetroLoadingProgressRing()
        {
            InitializeComponent();
        }


    }
}
