using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
