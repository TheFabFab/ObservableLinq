using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class WhereMethods
    {
        public static IQueryableObservableCollection<T> Where<T>(this IQueryableObservableCollection<T> @this, Func<T, bool> predicate)
        {
            return new FilteredObservableCollection<T>(@this.ToObservable(), item => predicate(item));
        }

        public static IQueryableObservableCollection<T> Where<T>(this IQueryableObservableCollection<T> @this, Func<T, int, bool> predicate)
        {
            throw new NotImplementedException();
        }

        private class FilteredObservableCollection<T> : DerivedObservableCollection<T, T>
        {
            private readonly Func<T, bool> _predicate;
            private readonly IObservableCollection<T> _source;

            public FilteredObservableCollection(IObservableCollection<T> source, Func<T, bool> predicate)
                : base(source, ((IEnumerable<T>)source).Where(predicate))
            {
                _source = source;
                _predicate = predicate;
            }

            protected override void UpdateOnMove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnMove(e);

                System.Diagnostics.Debug.Assert(e.NewItems.Count == e.OldItems.Count);

                var items = 
                    e.NewItems.
                    OfType<T>().
                    Select((item, idx) => new { Item = item, Index = idx }).
                    Where(x => _predicate(x.Item));

                var position = new ItemPosition(-1, 1);

                ItemPosition oldDestinationPosition = position, newDestinationPosition = position;
                for (int idx = 0; idx < _source.Count; idx++)
                {
                    if (idx == e.NewStartingIndex) newDestinationPosition = position;
                    if (idx == e.OldStartingIndex) oldDestinationPosition = position;

                    T item = default(T);

                    if (idx >= e.OldStartingIndex && idx < e.OldStartingIndex + e.OldItems.Count)
                    {
                        item = (T)e.OldItems[idx - e.OldStartingIndex];
                    }
                    else
                    {
                        int afterChangeIndex = idx;
                        if (afterChangeIndex >= e.OldStartingIndex)
                        {
                            afterChangeIndex = afterChangeIndex - e.OldItems.Count;
                        }
                        if (afterChangeIndex >= e.NewStartingIndex)
                        {
                            afterChangeIndex = afterChangeIndex + e.NewItems.Count;
                        }

                        item = _source[afterChangeIndex];
                    }

                    if (_predicate(item))
                    {
                        position = new ItemPosition(position.Index + 1, 0);
                    }
                    else
                    {
                        position = new ItemPosition(position.Index, position.Offset + 1);
                    }
                }

                foreach (var moved in items)
                {
                    this.Move(oldDestinationPosition.GetInsertionIndex() + moved.Index, newDestinationPosition.GetInsertionIndex() + moved.Index);
                }
            }

            protected override void UpdateOnReset(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReset(e);

                using (this.GetNotificationLock())
                {
                    this.Clear();
                    var index = 0;
                    foreach (var item in ((IEnumerable<T>)_source).Where(_predicate))
                    {
                        this.Insert(index++, item);
                    }
                }
            }

            protected override void UpdateOnReplace(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReplace(e);

                System.Diagnostics.Debug.Assert(e.NewItems.Count == e.OldItems.Count);

                int destinationIndex = 0, oldDestinationIndex = 0, newDestinationIndex = 0;
                for (int idx = 0; idx < _source.Count; idx++)
                {
                    if (idx == e.NewStartingIndex) newDestinationIndex = destinationIndex;
                    if (idx == e.OldStartingIndex) oldDestinationIndex = destinationIndex;

                    T item = _source[idx];

                    if (idx >= e.OldStartingIndex && idx < e.OldStartingIndex + e.OldItems.Count)
                    {
                        item = (T)e.OldItems[idx - e.OldStartingIndex];
                    }

                    if (_predicate(item))
                    {
                        destinationIndex++;
                    }
                }

                for (int idx = 0; idx < e.NewItems.Count; idx++)
                {
                    var newItem = (T)e.NewItems[idx];
                    var oldItem = (T)e.OldItems[idx];

                    var newItemIsIn = _predicate(newItem);
                    var oldItemIsIn = _predicate(oldItem);

                    if (oldItemIsIn && newItemIsIn)
                    {
                        this[oldDestinationIndex] = newItem;
                        oldDestinationIndex++;
                        newDestinationIndex++;
                    }
                    else if (oldItemIsIn)
                    {
                        RemoveAt(oldDestinationIndex);
                    }
                    else if (newItemIsIn)
                    {
                        Insert(newDestinationIndex, newItem);
                    }
                }
            }

            protected override void UpdateOnRemove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                var removeIndex = GetDestinationIndex(e.OldStartingIndex);
                int count = e.OldItems.OfType<T>().Where(_predicate).Count();
                for (int idx = 0; idx < count; idx++)
                {
                    this.RemoveAt(removeIndex);
                }
            }

            protected override void UpdateOnAdd(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnAdd(e);

                var insertIndex = GetDestinationIndex(e.NewStartingIndex);
                foreach (var item in e.NewItems.OfType<T>().Where(_predicate))
                {
                    this.Insert(insertIndex++, item);
                }
            }

            private int GetDestinationIndex(int sourceIndex)
            {
                var index = 0;

                for (int idx = 0; idx < sourceIndex; idx++)
                {
                    if (_predicate(_source[idx]))
                    {
                        index++;
                    }
                }

                return index;
            }
        }
    }
}
