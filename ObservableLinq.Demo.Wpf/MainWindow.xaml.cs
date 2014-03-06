using MahApps.Metro.Controls;
using NGif;
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

namespace ObservableLinq.Demo.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            var encoder = new AnimatedGifEncoder();
            encoder.Start("output.gif");
            encoder.SetRepeat(0);

            foreach (var frame in Enumerable.Range(0, 10))
            {
                AddFrame(encoder);
                await Task.Delay(10);
            }
            
            encoder.Finish();
        }

        private void AddFrame(AnimatedGifEncoder encoder)
        {
            var rtb = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(this);
            var bitmap = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.FillRectangle(0, 0, bitmap.PixelWidth, bitmap.PixelWidth, Colors.Wheat);
            bitmap.Lock();
            rtb.CopyPixels(
                new Int32Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight),
                bitmap.BackBuffer,
                bitmap.BackBufferStride * bitmap.PixelHeight, bitmap.BackBufferStride);
            bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)ActualWidth, (int)ActualHeight));
            bitmap.Unlock();
            encoder.AddFrame(bitmap);
        }
    }
}
