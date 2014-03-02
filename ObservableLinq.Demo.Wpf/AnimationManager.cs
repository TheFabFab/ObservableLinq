﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ObservableLinq.Demo.Wpf
{
    public static class AnimationManager
    {
        private static readonly Duration _exitDuration = new Duration(TimeSpan.FromMilliseconds(2500));
        private static readonly Duration _repositionDuration = new Duration(TimeSpan.FromMilliseconds(1650));
        private static readonly TimeSpan _initialStagger = TimeSpan.FromMilliseconds(10);
        private static readonly TimeSpan _stagger = TimeSpan.FromMilliseconds(150);

        private static bool _isInitialBatch;
        private static DateTime _lastAnimationTime = DateTime.MinValue;
        private static TimeSpan _currentStagger;

        private static TimeSpan CalculateStagger()
        {
            if (DateTime.UtcNow - _lastAnimationTime > _stagger)
            {
                _isInitialBatch = _lastAnimationTime == DateTime.MinValue;
                _currentStagger = TimeSpan.Zero;
            }
            else
            {
                _currentStagger = _currentStagger + (_isInitialBatch ? _initialStagger : _stagger);
            }

            _lastAnimationTime = DateTime.UtcNow;
            return _currentStagger;
        }

        public static Storyboard StartEntryAnimation(FrameworkElement containingObject, FrameworkElement animatedObject)
        {
            var translateTransform = animatedObject.RenderTransform as TranslateTransform;

            animatedObject.Opacity = 0.0;
            animatedObject.SetCurrentValue(FrameworkElement.OpacityProperty, 0.0);

            if (translateTransform == null)
            {
                animatedObject.RenderTransform = translateTransform = new TranslateTransform(-40, 10);
            }
            else
            {
                translateTransform.SetCurrentValue(TranslateTransform.XProperty, translateTransform.X - 40);
                translateTransform.SetCurrentValue(TranslateTransform.YProperty, translateTransform.X + 10);
            }

            var storyboard = new Storyboard { Duration = _repositionDuration, BeginTime = CalculateStagger() };

            var animationOpacity = new DoubleAnimation
            {
                From = 0,
                To = 1,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationOpacity, animatedObject);
            Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(animationOpacity);

            var animationX = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationX, animatedObject);
            Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(animationX);

            var animationY = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationY, animatedObject);
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(animationY);

            storyboard.Begin(containingObject);

            return storyboard;
        }

        public static Storyboard StartExitAnimation(FrameworkElement containingObject, FrameworkElement animatedObject)
        {
            var translateTransform = animatedObject.RenderTransform as TranslateTransform;

            if (translateTransform == null)
            {
                animatedObject.RenderTransform = translateTransform = new TranslateTransform(0, 0);
            }

            var storyboard = new Storyboard { Duration = _exitDuration, BeginTime = CalculateStagger() };

            var animationOpacity = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animationOpacity, animatedObject);
            Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(animationOpacity);

            var animationX = new DoubleAnimation
            {
                By = 60,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationX, animatedObject);
            Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(animationX);

            var animationY = new DoubleAnimation
            {
                By = -20,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationY, animatedObject);
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(animationY);

            storyboard.Begin(containingObject);
            return storyboard;
        }

        public static Storyboard StartRepositionAnimation(FrameworkElement containingObject, FrameworkElement animatedObject, Vector offset)
        {
            var translateTransform = animatedObject.RenderTransform as TranslateTransform;

            if (translateTransform == null)
            {
                animatedObject.RenderTransform = translateTransform = new TranslateTransform(-offset.X, -offset.Y);
            }
            else
            {
                translateTransform.SetCurrentValue(TranslateTransform.XProperty, translateTransform.X - offset.X);
                translateTransform.SetCurrentValue(TranslateTransform.YProperty, translateTransform.Y - offset.Y);
            }

            var storyboard = new Storyboard { Duration = _repositionDuration, BeginTime = CalculateStagger() };

            var animationX = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            if (translateTransform.X == 0)
            {
                animationX = new DoubleAnimation
                {
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
                To = 0,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationY, animatedObject);
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(animationY);

            storyboard.Begin(containingObject);
            return storyboard;
        }
    }
}