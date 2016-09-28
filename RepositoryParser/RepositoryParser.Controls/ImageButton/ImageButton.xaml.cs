using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RepositoryParser.Controls.ImageButton
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : Button
    {
        public static DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource",typeof(ImageSource),typeof(ImageButton),new FrameworkPropertyMetadata(null));
        public static DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text",typeof(String),typeof(ImageButton),new FrameworkPropertyMetadata(String.Empty));
        public static DependencyProperty ImageHeightProperty = DependencyProperty.RegisterAttached("ImageHeight",typeof(double), typeof(ImageButton), new PropertyMetadata(32.0));
        public static DependencyProperty ImageWidthProperty = DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(32.0));
        public static DependencyProperty IsTileProperty = DependencyProperty.RegisterAttached("IsTile", typeof(bool), typeof(ImageButton), new PropertyMetadata(false));

        public bool IsTile
        {
            get
            {
                return (bool) this.GetValue(IsTileProperty);
            }
            set
            {
                this.SetValue(IsTileProperty,value);
            }
        }

        public double ImageHeight
        {
            get
            {
                return (double)this.GetValue(ImageHeightProperty);
            }
            set
            {
                this.SetValue(ImageHeightProperty,value);
            }
        }

        public double ImageWidth
        {
            get
            {
                return (double) this.GetValue(ImageWidthProperty);
            }
            set
            {
                this.SetValue(ImageWidthProperty,value);
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

        public String Text
        {
            get
            {
                return this.GetValue(TextProperty).ToString();
            }
            set
            {
                this.SetValue(TextProperty,value);
            }
        }

        public ImageButton()
        {
            InitializeComponent();
        }

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

    }
}
