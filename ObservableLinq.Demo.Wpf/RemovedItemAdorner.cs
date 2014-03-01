using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ObservableLinq.Demo.Wpf
{
    public class RemovedItemAdorner : Adorner
    {
        private static DependencyProperty XProperty = TranslateTransform.XProperty.AddOwner(typeof(RemovedItemAdorner));
        private static DependencyProperty YProperty = TranslateTransform.YProperty.AddOwner(typeof(RemovedItemAdorner));

        private readonly Vector _offset;
        private readonly Border _border;

        public RemovedItemAdorner(UIElement adornedPanel, ContentPresenter adornedElement)
            : base(adornedPanel)
        {
            this.IsHitTestVisible = false;

            Width = Math.Ceiling(adornedElement.ActualWidth);
            Height = Math.Ceiling(adornedElement.ActualHeight);
            
            _offset = VisualTreeHelper.GetOffset(adornedElement);
            
            _border = new Border 
            { 
                Background = new VisualBrush(adornedElement), 
                Width = adornedElement.ActualWidth, 
                Height = adornedElement.ActualHeight,
                RenderTransform = new TranslateTransform
                {
                    X = _offset.X,
                    Y = _offset.Y,
                }
            };

            AddVisualChild(_border);

            BindingOperations.SetBinding(_border.RenderTransform, TranslateTransform.XProperty, new Binding { Source = this, Path = new PropertyPath(XProperty) });
            BindingOperations.SetBinding(_border.RenderTransform, TranslateTransform.YProperty, new Binding { Source = this, Path = new PropertyPath(YProperty) });

            Loaded += RemovedItemAdorner_Loaded;
        }

        private void RemovedItemAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = new Storyboard { Duration = new Duration(TimeSpan.FromMilliseconds(1300)) };

            var animationOpacity = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animationOpacity, _border);
            Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(animationOpacity);

            var animationX = new DoubleAnimation
            {
                From = _offset.X,
                To = _offset.X + 40,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationX, this);
            Storyboard.SetTargetProperty(animationX, new PropertyPath(XProperty));
            storyboard.Children.Add(animationX);

            var animationY = new DoubleAnimation
            {
                From = _offset.Y,
                To = _offset.Y - 20,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animationY, this);
            Storyboard.SetTargetProperty(animationY, new PropertyPath(YProperty));
            storyboard.Children.Add(animationY);

            storyboard.Completed += storyboard_Completed;
            storyboard.Begin(this);
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0: return _border;
                default: throw new ArgumentException();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _border.Measure(new Size(Width, Height));
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _border.Arrange(new Rect(0, 0, Width, Height));
            return base.ArrangeOverride(finalSize);
        }

        private void storyboard_Completed(object sender, EventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            adornerLayer.Remove(this);
        }
    }
}
