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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ObservableLinq.Demo.Wpf
{
    /// <summary>
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow : Window
    {
        private readonly MainViewModel _viewModel;
        public DemoWindow()
        {
            DataContext = _viewModel = new MainViewModel();
            InitializeComponent();
            Loaded += DemoWindow_Loaded;
        }

        private async void DemoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = System.Windows.SizeToContent.Manual;
            await Task.Delay(3000);

            using (var recorder = new GifRecorder(this, Root, "output2.gif", 10))
            {
                var sb = AnimationManager.StartEntryAnimation(this, Callout);

                var originalTitle = Title;
                Title = "Recording...";
                await recorder.RegisterStoryboard(sb);
                Title = "Ready";
                await Task.Delay(3000);
                Title = originalTitle;
            }

            //var encoder = new AnimatedGifEncoder();
            //encoder.Start("output.gif");
            //encoder.SetRepeat(0);
            //encoder.SetFrameRate(10);


            //await Task.Delay(100);

            //while (sb.GetCurrentState(this) != ClockState.Filling)
            //{
            //    var currentTime = sb.GetCurrentTime(this);
            //    Title = String.Format("{0} ms completed.", currentTime.Value.TotalMilliseconds);
            //    AddFrame(encoder, Root);
            //    sb.Seek(this, currentTime.Value + TimeSpan.FromMilliseconds(100), TimeSeekOrigin.BeginTime);
            //    await Task.Delay(17);
            //}

            //Title = title;

            //await Task.Delay(3000);
            //AnimationManager.StartExitAnimation(this, Callout);
            //await Task.Delay(3000);
            //_viewModel.Collection.RemoveAt(3);

            //encoder.Finish();
        }

        private void AddFrame(AnimatedGifEncoder encoder, FrameworkElement root)
        {
            var rtb = new RenderTargetBitmap((int)root.ActualWidth, (int)root.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(root);
            var bitmap = new WriteableBitmap((int)root.ActualWidth, (int)root.ActualHeight, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.Lock();
            rtb.CopyPixels(
                new Int32Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight),
                bitmap.BackBuffer,
                bitmap.BackBufferStride * bitmap.PixelHeight, bitmap.BackBufferStride);
            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
            encoder.AddFrame(bitmap);
        }
    }
}
