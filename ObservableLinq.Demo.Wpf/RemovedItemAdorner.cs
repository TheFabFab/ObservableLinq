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
        private readonly Border _border;

        public RemovedItemAdorner(UIElement adornedPanel, FrameworkElement adornedElement)
            : base(adornedPanel)
        {
            this.IsHitTestVisible = false;

            Width = Math.Ceiling(adornedElement.ActualWidth);
            Height = Math.Ceiling(adornedElement.ActualHeight);
            
            var offset = VisualTreeHelper.GetOffset(adornedElement);
            
            _border = new Border 
            { 
                Background = new VisualBrush(adornedElement), 
                Width = adornedElement.ActualWidth, 
                Height = adornedElement.ActualHeight,
                RenderTransform = new TranslateTransform
                {
                    X = offset.X,
                    Y = offset.Y,
                }
            };

            AddVisualChild(_border);

            Loaded += RemovedItemAdorner_Loaded;
        }

        private void RemovedItemAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            var animatedObject = this._border;

            var storyboard = AnimationManager.StartExitAnimation(this, animatedObject);
            storyboard.Completed += storyboard_Completed;
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
