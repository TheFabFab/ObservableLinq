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

            public DistinctObservableCollection(IQueryableObservableCollection<T> source, IEqualityComparer<T> comparer)
                : base(source, ((IEnumerable<T>)source).Distinct(comparer))
            {
                _source = source;
                _comparer = comparer;
            }

            public DistinctObservableCollection(IQueryableObservableCollection<T> source)
                : this(source, null)
            {
            }

            protected override void UpdateOnAdd(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnAdd(e);

                foreach (var item in e.NewItems.OfType<T>())
                {
                    if (!this.Contains(item))
                    {
                        Add(item);
                    }
                }
            }

            protected override void UpdateOnRemove(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                foreach (var item in e.OldItems.OfType<T>())
                {
                    if (!_source.Contains(item))
                    {
                        Remove(item);
                    }
                }
            }
        }
    }
}
