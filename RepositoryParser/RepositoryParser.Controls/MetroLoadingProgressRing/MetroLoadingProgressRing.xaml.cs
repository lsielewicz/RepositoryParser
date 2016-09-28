using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RepositoryParser.Controls.Annotations;

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
