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

            var originalTitle = Title;
            Title = "Recording...";

            using (GifRecorderManager.Instance.StartRecording(this, Root, "output.gif", 15))
            {
                AnimationManager.StartEntryAnimation(Callout, -10, 5);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                await Task.Delay(3000);

                AnimationManager.StartExitAnimation(Callout, 10, -5);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                _viewModel.Collection.RemoveAt(3);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();
            }

            Title = "Ready";
            await Task.Delay(3000);
            Title = originalTitle;

        }
    }
}
