using NGif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ObservableLinq.Demo.Wpf
{
    public class GifRecorder : IDisposable
    {
        private readonly TimeSpan _frameDelay;
        private readonly AnimatedGifEncoder _encoder;
        private readonly FrameworkElement _container;
        private readonly FrameworkElement _root;
        private readonly List<StoryboardController> _storyboardControllers;
        private readonly RenderTargetBitmap _renderTargetBitmap;
        private readonly WriteableBitmap _writeableBitmap;

        public GifRecorder(FrameworkElement container, FrameworkElement root, string fileName, int fps)
        {
            _container = container;
            _root = root;
            _frameDelay = TimeSpan.FromMilliseconds(1000.0 / fps);
            _storyboardControllers = new List<StoryboardController>();
            _renderTargetBitmap = new RenderTargetBitmap((int)_root.ActualWidth, (int)_root.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            _writeableBitmap = new WriteableBitmap((int)_root.ActualWidth, (int)_root.ActualHeight, 96, 96, PixelFormats.Pbgra32, null);

            _encoder = new AnimatedGifEncoder();
            _encoder.Start(fileName);
            _encoder.SetRepeat(0);
            _encoder.SetFrameRate(fps);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Dispose()
        {
            _encoder.Finish();
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        public Task RegisterStoryboard(Storyboard storyboard)
        {
            storyboard.Pause(_container);
            var controller = new StoryboardController(storyboard, _container, _frameDelay);
            _storyboardControllers.Add(controller);
            return controller.CompletedTask;
        }

        private async void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            await Task.Yield();

            AddOneFrame();

            List<StoryboardController> finishedControllers = new List<StoryboardController>();
            foreach (var controller in _storyboardControllers)
            {
                if (!controller.MoveNext())
                {
                    finishedControllers.Add(controller);
                }
            }

            _storyboardControllers.RemoveAll(c => finishedControllers.Contains(c));
        }

        private void AddOneFrame()
        {
            _renderTargetBitmap.Render(_root);
            _writeableBitmap.Lock();
            _renderTargetBitmap.CopyPixels(
                new Int32Rect(0, 0, _renderTargetBitmap.PixelWidth, _renderTargetBitmap.PixelHeight),
                _writeableBitmap.BackBuffer,
                _writeableBitmap.BackBufferStride * _writeableBitmap.PixelHeight, _writeableBitmap.BackBufferStride);
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight));
            _writeableBitmap.Unlock();

            _encoder.AddFrame(_writeableBitmap);
        }

        private class StoryboardController
        {
            private readonly Storyboard _storyboard;
            private readonly FrameworkElement _container;
            private readonly TimeSpan _frameDelay;
            private readonly TaskCompletionSource<object> _tcs;

            public StoryboardController(Storyboard storyboard, FrameworkElement container, TimeSpan frameDelay)
            {
                _storyboard = storyboard;
                _container = container;
                _frameDelay = frameDelay;
                _tcs = new TaskCompletionSource<object>();
            }

            public Storyboard Storyboard { get { return _storyboard; } }

            public Task CompletedTask { get { return _tcs.Task; } }

            public bool MoveNext()
            {
                if (_storyboard.GetCurrentState(_container) != ClockState.Filling)
                {
                    var currentTime = _storyboard.GetCurrentTime(_container);
                    if (currentTime.HasValue)
                    {
                        _storyboard.Seek(_container, currentTime.Value + _frameDelay, TimeSeekOrigin.BeginTime);
                    }

                    return true;
                }

                _tcs.SetResult(new object());
                return false;
            }
        }
    }
}
