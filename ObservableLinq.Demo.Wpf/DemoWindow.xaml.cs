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
            _viewModel.Observer.Observe(null);
            
            foreach (var item in new[] { 1, 2, 3, 4, 5, 6, 7, 8 })
            {
                _viewModel.Collection.Add(item);
            }

            InitializeComponent();            
            
            Loaded += DemoWindow_Loaded;
        }

        private async void DemoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = System.Windows.SizeToContent.Manual;
            await GifRecorderManager.Instance.CurrentStoryboardsCompleted();
            
            _viewModel.Collection.Clear();

            var originalTitle = Title;
            Title = "Recording...";

            using (GifRecorderManager.Instance.StartRecording(this, Root, "basic_projection.gif", 10))
            {
                foreach (var item in new[] { 1, 2, 3, 4, 5, 6, 7, 8 })
                {
                    _viewModel.Collection.Add(item);
                }

                AnimationManager.StartEntryAnimation(SourceLabel);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                AnimationManager.StartEntryAnimation(TargetLabel);
                AnimationManager.StartEntryAnimation(Label1);
                _viewModel.SelectedOption = _viewModel.DataOptions[6];

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                //await AnimationManager.Pause(1000);

                System.Diagnostics.Debug.WriteLine("Removing");
                AnimationManager.StartExitAnimation(Label1);
                AnimationManager.StartEntryAnimation(Label2);
                _viewModel.Collection.RemoveAt(3);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                //await AnimationManager.Pause(1000);

                AnimationManager.StartExitAnimation(Label2);
                AnimationManager.StartEntryAnimation(Label3);
                _viewModel.Collection.Insert(0, 4);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                // await AnimationManager.Pause(1000);

                AnimationManager.StartExitAnimation(Label3);
                AnimationManager.StartEntryAnimation(Label4);
                _viewModel.Collection.Move(0, 3);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();

                //await AnimationManager.Pause(1000);

                AnimationManager.StartExitAnimation(Root, 10, -5);

                await GifRecorderManager.Instance.CurrentStoryboardsCompleted();
            }

            Close();
        }
    }
}
