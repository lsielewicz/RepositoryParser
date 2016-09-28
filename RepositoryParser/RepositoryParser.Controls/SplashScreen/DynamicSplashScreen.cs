using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RepositoryParser.Controls.SplashScreen
{
    public class DynamicSplashScreen : Window
    {
        public DynamicSplashScreen()
        {
            this.ShowInTaskbar = false;
            this.WindowStartupLocation  = WindowStartupLocation.Manual;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
           
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width)/2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height)/2;
        }

        public void Capture(string filePath)
        {
            this.Capture(filePath, new PngBitmapEncoder());
        }

        public void Capture(string filePath, BitmapEncoder encoder)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int) this.Width, (int) this.Height, 96, 96,
                PixelFormats.Pbgra32);
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stream = File.Create(filePath))
            {
                encoder.Save(stream);
            }
        }



    }
}
