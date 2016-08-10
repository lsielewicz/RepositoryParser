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

namespace RepositoryParser.Controls.ImageButton
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : Button
    {
        public DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource",typeof(ImageSource),typeof(ImageButton),new FrameworkPropertyMetadata(null));
        public DependencyProperty TextProperty = DependencyProperty.Register("Text",typeof(String),typeof(ImageButton),new FrameworkPropertyMetadata(String.Empty));
        public DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight",typeof(double), typeof(ImageButton), new PropertyMetadata(32.0));
        public DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(32.0));

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
                this.SetValue(ImageSourceProperty,value);
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
    }
}
