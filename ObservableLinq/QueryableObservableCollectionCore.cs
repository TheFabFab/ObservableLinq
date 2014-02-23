using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    internal class QueryableObservableCollectionCore<T> : IQueryableObservableCollection<T>
    {
        private readonly ObservableCollection<T> _wrappedCollection;
        private int _notificationsLockCount;

        public QueryableObservableCollectionCore(ObservableCollection<T> wrappedCollection)
        {
            _wrappedCollection = wrappedCollection;
            _wrappedCollection.CollectionChanged += _wrappedCollection_CollectionChanged;
            ((INotifyPropertyChanged)_wrappedCollection).PropertyChanged += QueryableObservableCollectionCore_PropertyChanged;
        }

        public QueryableObservableCollectionCore(IEnumerable<T> collection)
            : this(new ObservableCollection<T>(collection))
        {
        }

        public event Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected T this[int index]
        {
            get { return _wrappedCollection[index]; }
            set { _wrappedCollection[index] = value; }
        }

        protected void Insert(int index, T item)
        {
            _wrappedCollection.Insert(index, item);
        }

        protected void RemoveAt(int index)
        {
            _wrappedCollection.RemoveAt(index);
        }

        protected void Add(T item)
        {
            _wrappedCollection.Add(item);
        }

        protected void Move(int oldIndex, int newIndex)
        {
            _wrappedCollection.Move(oldIndex, newIndex);
        }

        protected void Clear()
        {
            _wrappedCollection.Clear();
        }

        protected bool Remove(T item)
        {
            return _wrappedCollection.Remove(item);
        }

        private void QueryableObservableCollectionCore_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_notificationsLockCount == 0)
            {
                PropertyChanged(this, e);
            }
        }

        private void _wrappedCollection_CollectionChanged(object sender, Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_notificationsLockCount == 0)
            {
                CollectionChanged(this, e);
            }
        }

        public IDisposable GetNotificationLock()
        {
            _notificationsLockCount++;
            return new NotificationLock(this);
        }

        private void UnlockNotification()
        {
            _notificationsLockCount--;
            if (_notificationsLockCount == 0)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        T IObservableCollection<T>.this[int index]
        {
            get
            {
                return _wrappedCollection[index];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public int Count
        {
            get { return _wrappedCollection.Count; }
        }

        public int IndexOf(T item)
        {
            return _wrappedCollection.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        T IList<T>.this[int index]
        {
            get
            {
                return _wrappedCollection[index];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Contains(T item)
        {
            return _wrappedCollection.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _wrappedCollection.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return _wrappedCollection.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new InvalidOperationException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _wrappedCollection.GetEnumerator();
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return _wrappedCollection.GetEnumerator();
        }

        T IReadOnlyList<T>.this[int index]
        {
            get { return _wrappedCollection[index]; }
        }

        int IReadOnlyCollection<T>.Count
        {
            get { return _wrappedCollection.Count; }
        }

        int Collections.IList.Add(object value)
        {
            throw new InvalidOperationException();
        }

        void Collections.IList.Clear()
        {
            throw new InvalidOperationException();
        }

        bool Collections.IList.Contains(object value)
        {
            return ((Collections.IList)_wrappedCollection).Contains(value);
        }

        int Collections.IList.IndexOf(object value)
        {
            return ((Collections.IList)_wrappedCollection).IndexOf(value);
        }

        void Collections.IList.Insert(int index, object value)
        {
            throw new InvalidOperationException();
        }

        bool Collections.IList.IsFixedSize
        {
            get { return ((Collections.IList)_wrappedCollection).IsFixedSize; }
        }

        bool Collections.IList.IsReadOnly
        {
            get { return ((Collections.IList)_wrappedCollection).IsReadOnly; }
        }

        void Collections.IList.Remove(object value)
        {
            throw new InvalidOperationException();
        }

        void Collections.IList.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        object Collections.IList.this[int index]
        {
            get
            {
                return ((Collections.IList)_wrappedCollection)[index];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        void Collections.ICollection.CopyTo(Array array, int index)
        {
            ((Collections.ICollection)_wrappedCollection).CopyTo(array, index);
        }

        int Collections.ICollection.Count
        {
            get { return _wrappedCollection.Count; }
        }

        bool Collections.ICollection.IsSynchronized
        {
            get { return ((Collections.ICollection)_wrappedCollection).IsSynchronized; }
        }

        object Collections.ICollection.SyncRoot
        {
            get { return ((Collections.ICollection)_wrappedCollection).SyncRoot; }
        }

        private class NotificationLock : IDisposable
        {
            private readonly QueryableObservableCollectionCore<T> _parent;

            public NotificationLock(QueryableObservableCollectionCore<T> parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                _parent.UnlockNotification();
            }
        }
    }
}
