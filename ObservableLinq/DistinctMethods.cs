using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class DistinctMethods
    {
        public static IQueryableObservableCollection<T> Distinct<T>(this IQueryableObservableCollection<T> @this, IEqualityComparer<T> comparer)
        {
            return new DistinctObservableCollection<T>(@this, comparer);
        }

        public static IQueryableObservableCollection<T> Distinct<T>(this IQueryableObservableCollection<T> @this)
        {
            return new DistinctObservableCollection<T>(@this);
        }

        public static IOrderedQueryableObservableCollection<T> Distinct<T>(this IOrderedQueryableObservableCollection<T> @this, IEqualityComparer<T> comparer)
        {
            throw new NotImplementedException();
        }

        public static IOrderedQueryableObservableCollection<T> Distinct<T>(this IOrderedQueryableObservableCollection<T> @this)
        {
            throw new NotImplementedException();
        }

        private class DistinctObservableCollection<T> : DerivedObservableCollection<T, T>
        {
            private readonly IEqualityComparer<T> _comparer;
            private readonly IQueryableObservableCollection<T> _source;
            private readonly IList<int> _index;

            public DistinctObservableCollection(IQueryableObservableCollection<T> source, IEqualityComparer<T> comparer)
                : base(source)
            {
                _source = source;
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _index = new List<int>();

                for (int index = 0; index < source.Count; index++)
                {
                    T item = source[index];
                    if (!this.Contains(item, comparer))
                    {
                        _index.Add(index);
                        Add(item);
                    }
                }
            }

            public DistinctObservableCollection(IQueryableObservableCollection<T> source)
                : this(source, null)
            {
            }

            protected override void UpdateOnAdd(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnAdd(e);

                var indexInSource = e.NewStartingIndex;

                foreach (var item in e.NewItems.OfType<T>())
                {
                    UpdateOnInsert(indexInSource, item);

                    indexInSource++;
                }
            }

            private void UpdateOnInsert(int indexInSource, T item)
            {
                var indexInTarget = _index.BinarySearch(indexInSource).GetInsertionIndex();
                int currentIndex = IndexOf(item);

                if (currentIndex == -1)
                {
                    Insert(indexInTarget, item);
                    _index.Insert(indexInTarget, indexInSource);
                }
                else if (indexInTarget < currentIndex)
                {
                    _index.Remove(currentIndex);
                    Move(currentIndex, indexInTarget);
                    _index.Insert(indexInTarget, indexInSource);
                }
            }

            protected override void UpdateOnRemove(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                var indexInSource = e.OldStartingIndex;

                foreach (var item in e.OldItems.OfType<T>())
                {
                    UpdateOnRemove(indexInSource, item);

                    indexInSource++;
                }
            }

            private void UpdateOnRemove(int indexInSource, T item)
            {
                var indexInTarget = _index.BinarySearch(indexInSource).GetInsertionIndex();

                int newIndexInSource =
                    _source
                    .Skip(indexInSource + 1)
                    .Select((x, index) => new { Item = x, Index = indexInSource + 1 + index })
                    .Where(x => _comparer.Equals(x.Item, item))
                    .Select(x => x.Index)
                    .DefaultIfEmpty(-1)
                    .First();

                if (newIndexInSource == -1)
                {
                    RemoveAt(indexInTarget);
                    _index.RemoveAt(indexInTarget);
                }
                else
                {
                    _index.RemoveAt(indexInTarget);
                    var newIndexInTarget = _index.BinarySearch(newIndexInSource).GetInsertionIndex();

                    Move(indexInTarget, newIndexInTarget);
                    _index.Insert(newIndexInTarget, newIndexInSource);
                }
            }

            protected override void UpdateOnReplace(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReplace(e);

                for (int indexInEvent = 0; indexInEvent < e.NewItems.Count; indexInEvent++)
                {
                    int indexInSource = e.NewStartingIndex + indexInEvent;

                    var oldItem = (T)e.OldItems[indexInEvent];
                    var newItem = (T)e.NewItems[indexInEvent];

                    if (!_comparer.Equals(oldItem, newItem))
                    {
                        UpdateOnRemove(indexInSource, oldItem);
                        UpdateOnInsert(indexInSource, newItem);
                    }
                }
            }

            protected override void UpdateOnMove(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnMove(e);

                for (int indexInEvent = 0; indexInEvent < e.NewItems.Count; indexInEvent++)
                {
                    var item = (T)e.NewItems[indexInEvent];

                    int oldIndexInTarget = IndexOf(item);
                    int newIndexInSource = _source.IndexOf(item);
                    int newIndexInTarget = _index.BinarySearch(newIndexInSource).GetInsertionIndex();

                    System.Diagnostics.Debug.Assert(newIndexInSource != -1);

                    if (newIndexInTarget < oldIndexInTarget)
                    {
                        _index.RemoveAt(oldIndexInTarget);
                        Move(oldIndexInTarget, newIndexInTarget);
                        _index.Insert(newIndexInTarget, newIndexInSource);
                    }
                }
            }

            protected override void UpdateOnReset(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReset(e);

                Clear();
            }
        }
    }
}
