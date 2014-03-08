using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ObservableLinq.Demo.Wpf
{
    public class AnimatingPanel : VirtualizingStackPanel
    {
        private static DependencyProperty OffsetProperty = DependencyProperty.RegisterAttached("Offset", typeof(Vector), typeof(AnimatingPanel), new PropertyMetadata(new Vector(Double.NaN, Double.NaN)));
        private Dictionary<int, Vector> _movedItemOffsets = new Dictionary<int, Vector>();

        public AnimatingPanel()
        {
        }

        private static Vector GetOffset(DependencyObject child)
        {
            return (Vector)child.GetValue(OffsetProperty);
        }

        private static void SetOffset(DependencyObject child, Vector value)
        {
            child.SetValue(OffsetProperty, value);
        }

        protected override void OnItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    System.Diagnostics.Debug.Assert(args.ItemUICount == 1);
                    OnChildRemoved(args.Position.Index);
                    break;
            }

            if (args.Action == NotifyCollectionChangedAction.Move)
            {
                // We need to temporarily save the offsets because the base implementation
                // doesn't always respect 'Move'
                System.Diagnostics.Debug.Assert(args.ItemUICount == 1);
                var oldIndex = args.OldPosition.Index + args.OldPosition.Offset;
                var newIndex = args.Position.Index + args.Position.Offset;
                var child = InternalChildren[oldIndex];
                _movedItemOffsets[newIndex] = VisualTreeHelper.GetOffset(child);
            }

            base.OnItemsChanged(sender, args);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var desiredSize = base.MeasureOverride(constraint);

            // Reapply the offset of children that should have been moved,
            // but were re-created, in fact
            foreach (var movedItemOffsetNode in _movedItemOffsets)
            {
                var child = InternalChildren[movedItemOffsetNode.Key];
                SetOffset(child, movedItemOffsetNode.Value);
            }

            _movedItemOffsets.Clear();

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            arrangeSize = base.ArrangeOverride(arrangeSize);

            foreach (var child in this.InternalChildren.OfType<FrameworkElement>())
            {
                var oldOffset = GetOffset(child);
                var newOffset = VisualTreeHelper.GetOffset(child);
                if (EqualityComparer<Vector>.Default.Equals(oldOffset, new Vector(Double.NaN, Double.NaN)))
                {
                    AnimationManager.StartEntryAnimation(child);
                }
                else if (oldOffset != newOffset)
                {
                    AnimationManager.StartRepositionAnimation(child, newOffset - oldOffset);
                }

                SetOffset(child, newOffset);
            }

            return arrangeSize;
        }

        protected override void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {
            e.UIElement.ClearValue(OffsetProperty);
            base.OnCleanUpVirtualizedItem(e);
        }

        private void OnChildRemoved(int removedIndex)
        {
            System.Diagnostics.Debug.Assert(removedIndex >= 0 && removedIndex < InternalChildren.Count);
            var removedChild = this.InternalChildren[removedIndex] as FrameworkElement;
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer.Add(new RemovedItemAdorner(this, removedChild));
        }
    }
}
