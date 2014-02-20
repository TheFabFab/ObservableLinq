using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class OrderByMethods
    {
        public static IOrderedQueryableObservableCollection<TSource> OrderBy<TSource, TKey>(this IQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            return new SortedObservableCollection<TSource, TKey>(@this, keySelector, false);
        }

        public static IOrderedQueryableObservableCollection<TSource> OrderBy<TSource, TKey>(this IQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new SortedObservableCollection<TSource, TKey>(@this, keySelector, comparer, false);
        }

        public static IOrderedQueryableObservableCollection<TSource> OrderByDescending<TSource, TKey>(this IQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            return new SortedObservableCollection<TSource, TKey>(@this, keySelector, true);
        }

        public static IOrderedQueryableObservableCollection<TSource> OrderByDescending<TSource, TKey>(this IQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new SortedObservableCollection<TSource, TKey>(@this, keySelector, comparer, true);
        }

        public static IOrderedQueryableObservableCollection<TSource> ThenBy<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }

        public static IOrderedQueryableObservableCollection<TSource> ThenBy<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }

        public static IOrderedQueryableObservableCollection<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }

        public static IOrderedQueryableObservableCollection<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }

        private class SortedObservableCollection<TSource, TKey> : DerivedObservableCollection<TSource, TSource>, IOrderedQueryableObservableCollection<TSource>
        {
            private readonly IQueryableObservableCollection<TSource> _source;
            private readonly Func<TSource, TKey> _keySelector;
            private readonly IComparer<TKey> _comparer;
            private readonly bool _descending;

            public SortedObservableCollection(IQueryableObservableCollection<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending)
                : base(
                    source,
                    descending ?
                    ((IEnumerable<TSource>)source).OrderByDescending(keySelector) :
                    ((IEnumerable<TSource>)source).OrderBy(keySelector))
            {
                _source = source;
                _keySelector = keySelector;
                _comparer = comparer;
                _descending = descending;
            }

            public SortedObservableCollection(IQueryableObservableCollection<TSource> source, Func<TSource, TKey> keySelector, bool descending)
                : this(source, keySelector, Comparer<TKey>.Default, descending)
            {
            }

            protected override void UpdateOnAdd(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnAdd(e);

                foreach (var item in e.NewItems.OfType<TSource>())
                {
                    var position = this.BinarySearch(_keySelector(item), _keySelector);
                    Insert(position.GetInsertionIndex(), item);
                }
            }

            protected override void UpdateOnRemove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                foreach (var item in e.OldItems.OfType<TSource>())
                {
                    var position = this.BinarySearch(_keySelector(item), _keySelector);
                    if (position.Offset == 0)
                    {
                        RemoveAt(position.Index);
                    }
                }
            }

            protected override void UpdateOnMove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnMove(e);

                // Nothing to do here
            }

            protected override void UpdateOnReplace(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReplace(e);

                foreach (var item in e.OldItems.OfType<TSource>())
                {
                    var position = this.BinarySearch(_keySelector(item), _keySelector);
                    if (position.Offset == 0)
                    {
                        RemoveAt(position.Index);
                    }
                }

                foreach (var item in e.NewItems.OfType<TSource>())
                {
                    var position = this.BinarySearch(_keySelector(item), _keySelector);
                    Insert(position.GetInsertionIndex(), item);
                }
            }

            protected override void UpdateOnReset(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReset(e);

                using (GetNotificationLock())
                {
                    Clear();

                    foreach (
                        var item in
                        _descending ?
                        ((IEnumerable<TSource>)_source).OrderByDescending(_keySelector) :
                        ((IEnumerable<TSource>)_source).OrderBy(_keySelector))
                    {
                        Add(item);
                    }
                }
            }
        }
    }
}
