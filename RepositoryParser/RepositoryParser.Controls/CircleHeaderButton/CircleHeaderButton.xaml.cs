using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RepositoryParser.Controls.CircleHeaderButton
{
    /// <summary>
    /// Interaction logic for CircleHeaderButton.xaml
    /// </summary>
    public partial class CircleHeaderButton : Button
    {
        public static DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(CircleHeaderButton), new FrameworkPropertyMetadata(null));
        public static DependencyProperty ImageHeightProperty = DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(CircleHeaderButton), new PropertyMetadata(32.0));
        public static DependencyProperty ImageWidthProperty = DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(CircleHeaderButton), new PropertyMetadata(32.0));

        public double ImageHeight
        {
            get
            {
                return (double)this.GetValue(ImageHeightProperty);
            }
            set
            {
                this.SetValue(ImageHeightProperty, value);
            }
        }

        public double ImageWidth
        {
            get
            {
                return (double)this.GetValue(ImageWidthProperty);
            }
            set
            {
                this.SetValue(ImageWidthProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return this.GetValue(ImageSourceProperty) as ImageSource;
            }
            set
            {
                this.SetValue(ImageSourceProperty, value);
            }
        }

        static CircleHeaderButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleHeaderButton), new FrameworkPropertyMetadata(typeof(CircleHeaderButton)));
        }

        public CircleHeaderButton()
        {
            InitializeComponent();
        }
    }
}
