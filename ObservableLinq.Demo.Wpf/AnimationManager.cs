using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ObservableLinq.Demo.Wpf
{
    public class AnimationManager : DependencyObject
    {
        public static readonly DependencyProperty IsAnimationRootProperty =
            DependencyProperty.RegisterAttached(
                "IsAnimationRoot",
                typeof(bool),
                typeof(AnimationManager),
                new FrameworkPropertyMetadata(
                    false,
                    IsAnimationRootPropertyChanged));

        public static readonly DependencyProperty AnimationDelayProperty =
            DependencyProperty.RegisterAttached(
                "AnimationDelay",
                typeof(double),
                typeof(AnimationManager),
                new FrameworkPropertyMetadata(0.0, AnimationDelayPropertyChanged));

        private static readonly DependencyProperty AnimationRootProperty =
            DependencyProperty.RegisterAttached(
                "AnimationRoot",
                typeof(AnimationManagerCore),
                typeof(AnimationManager),
                new FrameworkPropertyMetadata(
                    null, 
                    FrameworkPropertyMetadataOptions.Inherits));

        private static bool _isInitialBatch;

        public static bool GetIsAnimationRoot(DependencyObject source)
        {
            return (bool)source.GetValue(IsAnimationRootProperty);
        }

        public static void SetIsAnimationRoot(DependencyObject target, bool value)
        {
            target.SetValue(IsAnimationRootProperty, value);
        }

        public static double GetAnimationDelay(DependencyObject source)
        {
            return (double)source.GetValue(AnimationDelayProperty);
        }

        public static void SetAnimationDelay(DependencyObject target, double value)
        {
            target.SetValue(AnimationDelayProperty, value);
        }

        public static async Task Pause(int delayMilliseconds)
        {
            var storyboard = new Storyboard { Duration = new Duration(TimeSpan.FromMilliseconds(delayMilliseconds)) };
            GifRecorderManager.Instance.RegisterStoryboard(storyboard);
            await GifRecorderManager.Instance.CurrentStoryboardsCompleted();
        }
        
        public static Task StartEntryAnimation(FrameworkElement animatedObject, double xDelta = -40, double yDelta = 10)
        {
            var tcs = new TaskCompletionSource<object>();
            var root = GetAnimationRoot(animatedObject);
            if (root != null)
            {
                var storyboard = root.CreateEntryAnimation(animatedObject, xDelta, yDelta);
                storyboard.Completed += (s, e) => tcs.TrySetResult(null);
                GifRecorderManager.Instance.RegisterStoryboard(storyboard);
            }

            return tcs.Task;
        }

        public static Task StartExitAnimation(FrameworkElement animatedObject, double xDelta = 60, double yDelta = -20)
        {
            var tcs = new TaskCompletionSource<object>();
            var root = GetAnimationRoot(animatedObject);
            if (root != null)
            {
                var storyboard = root.CreateExitAnimation(animatedObject, xDelta, yDelta);
                storyboard.Completed += (s, e) => tcs.SetResult(null);
                GifRecorderManager.Instance.RegisterStoryboard(storyboard);
            }

            return tcs.Task;
        }

        public static Task StartRepositionAnimation(FrameworkElement animatedObject, Vector offset)
        {
            var tcs = new TaskCompletionSource<object>();
            var root = GetAnimationRoot(animatedObject);
            if (root != null)
            {
                var storyboard = root.CreateRepositionAnimation(animatedObject, offset);
                storyboard.Completed += (s, e) => tcs.SetResult(null);
                GifRecorderManager.Instance.RegisterStoryboard(storyboard);
            }

            return tcs.Task;
        }

        private static void IsAnimationRootPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                SetAnimationRoot(sender, new AnimationManagerCore());
            }
            else
            {
                sender.ClearValue(AnimationRootProperty);
            }
        }

        private static void AnimationDelayPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var root = GetAnimationRoot(sender);
            if (root != null)
            {
                root.Delay = TimeSpan.FromMilliseconds((double)e.NewValue);
            }
        }

        private static AnimationManagerCore GetAnimationRoot(DependencyObject source)
        {
            return (AnimationManagerCore)source.GetValue(AnimationRootProperty);
        }

        private static void SetAnimationRoot(DependencyObject target, AnimationManagerCore value)
        {
            target.SetValue(AnimationRootProperty, value);
        }

        private class AnimationManagerCore
        {
            private static readonly Duration _exitDuration = new Duration(TimeSpan.FromMilliseconds(1300));
            private static readonly Duration _repositionDuration = new Duration(TimeSpan.FromMilliseconds(1300));
            private static readonly TimeSpan _initialStagger = TimeSpan.FromMilliseconds(10);
            private static readonly TimeSpan _stagger = TimeSpan.FromMilliseconds(50);

            private DateTime _lastAnimationTime = DateTime.MinValue;
            private TimeSpan _currentStagger;

            public TimeSpan Delay { get; set; }

            private TimeSpan CalculateStagger()
            {
                if (DateTime.UtcNow - _lastAnimationTime > _stagger)
                {
                    _isInitialBatch = _lastAnimationTime == DateTime.MinValue;
                    _currentStagger = Delay;
                }
                else
                {
                    _currentStagger = _currentStagger + (_isInitialBatch ? _initialStagger : _stagger);
                }

                _lastAnimationTime = DateTime.UtcNow;
                return _currentStagger;
            }

            public Storyboard CreateEntryAnimation(FrameworkElement animatedObject, double xDelta, double yDelta)
            {
                var translateTransform = animatedObject.RenderTransform as TranslateTransform;

                animatedObject.Opacity = 0.0;
                animatedObject.SetCurrentValue(FrameworkElement.OpacityProperty, 0.0);

                if (translateTransform == null)
                {
                    animatedObject.RenderTransform = translateTransform = new TranslateTransform(xDelta, yDelta);
                }
                else
                {
                    translateTransform.SetCurrentValue(TranslateTransform.XProperty, translateTransform.X + xDelta);
                    translateTransform.SetCurrentValue(TranslateTransform.YProperty, translateTransform.X + yDelta);
                }

                var storyboard = new Storyboard { FillBehavior = FillBehavior.Stop };

                var animationOpacity = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    From = 0,
                    To = 1,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationOpacity, animatedObject);
                Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UIElement.OpacityProperty));
                storyboard.Children.Add(animationOpacity);

                var animationX = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    To = 0,                    
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationX, animatedObject);
                Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(animationX);

                var animationY = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    To = 0,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationY, animatedObject);
                Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                storyboard.Children.Add(animationY);

                storyboard.Completed += (s, e) =>
                {
                    animatedObject.Opacity = 1.0;
                    translateTransform.X = translateTransform.X;
                    translateTransform.Y = translateTransform.Y;
                };

                return storyboard;
            }

            public Storyboard CreateExitAnimation(FrameworkElement animatedObject, double xDelta, double yDelta)
            {
                var translateTransform = animatedObject.RenderTransform as TranslateTransform;

                if (translateTransform == null)
                {
                    animatedObject.RenderTransform = translateTransform = new TranslateTransform(0, 0);
                }

                var storyboard = new Storyboard();

                var animationOpacity = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    To = 0,
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(animationOpacity, animatedObject);
                Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UIElement.OpacityProperty));
                storyboard.Children.Add(animationOpacity);

                var animationX = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    By = xDelta,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationX, animatedObject);
                Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(animationX);

                var animationY = new DoubleAnimation
                {
                    Duration = _exitDuration,
                    BeginTime = CalculateStagger(),
                    By = yDelta,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationY, animatedObject);
                Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                storyboard.Children.Add(animationY);

                return storyboard;
            }

            public Storyboard CreateRepositionAnimation(FrameworkElement animatedObject, Vector offset)
            {
                var translateTransform = animatedObject.RenderTransform as TranslateTransform;

                if (translateTransform == null)
                {
                    animatedObject.RenderTransform = translateTransform = new TranslateTransform(-offset.X, -offset.Y);
                }
                else
                {
                    translateTransform.X = translateTransform.X - offset.X;
                    translateTransform.Y = translateTransform.Y - offset.Y;
                }

                var storyboard = new Storyboard { FillBehavior = FillBehavior.Stop };

                var animationX = new DoubleAnimation
                {
                    Duration = _repositionDuration,
                    BeginTime = CalculateStagger(),
                    To = 0,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                if (translateTransform.X == 0)
                {
                    animationX = new DoubleAnimation
                    {
                        Duration = _repositionDuration,
                        BeginTime = CalculateStagger(),
                        From = 0,
                        To = translateTransform.Y / 4 + Math.Sign(translateTransform.Y) * animatedObject.ActualWidth,
                        AccelerationRatio = .5,
                        DecelerationRatio = .5,
                        SpeedRatio = 2,
                        AutoReverse = true,
                    };
                }

                Storyboard.SetTarget(animationX, animatedObject);
                Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(animationX);

                var animationY = new DoubleAnimation
                {
                    Duration = _repositionDuration,
                    BeginTime = CalculateStagger(),
                    To = 0,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animationY, animatedObject);
                Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                storyboard.Children.Add(animationY);

                storyboard.Completed += (s, e) =>
                {
                    animatedObject.Opacity = 1.0;
                    translateTransform.X = translateTransform.X;
                    translateTransform.Y = translateTransform.Y;
                };

                return storyboard;
            }
        }
    }
}
