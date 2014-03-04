using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public interface IObservableCollection<T> :
        IList<T>,
        ICollection<T>,
        IReadOnlyList<T>,
        IReadOnlyCollection<T>,
        IEnumerable<T>,
        IList,
        ICollection,
        IEnumerable,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        new T this[int index] { get; set; }
        new int Count { get; }
    }

    public interface IQueryableObservableCollection<T>
    {
    }

    public interface IOrderedQueryableObservableCollection<T> : IQueryableObservableCollection<T>
    {
    }

    public static class QueryableObservableCollection
    {
        public static IQueryableObservableCollection<T> ToQueryable<T>(this ObservableCollection<T> @this)
        {
            return new ReadOnlyObservableCollection<T>(@this);
        }

        public static IObservableCollection<T> ToObservable<T>(this IQueryableObservableCollection<T> @this)
        {
            var observable = @this as IObservableCollection<T>;
            System.Diagnostics.Debug.Assert(observable != null);
            if (observable == null) throw new InvalidOperationException("Must implement IObservableCollection<T>");
            return observable;
        }
    }
}
