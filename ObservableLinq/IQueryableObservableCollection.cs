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

    public interface IQueryableObservableCollection<T> : IObservableCollection<T>
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
    }
}
