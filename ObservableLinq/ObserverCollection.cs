using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public class ObserverCollection<T> : ReadOnlyObservableCollection<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private INotifyCollectionChanged _currentSubject;

        public ObserverCollection(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public ObserverCollection() : this(null) { }

        public void Observe(IList<T> subject)
        {            
            if (_currentSubject != null)
            {
                _currentSubject.CollectionChanged -= _currentSubject_CollectionChanged;
                _currentSubject = null;
            }

            for (int index = 0; index < subject.Count; index++ )
            {
                var item = subject[index];

                var oldIndex =
                    this
                        .Skip(index)
                        .Select((x, idx) => new { Item = x, Index = idx })
                        .Where(x => _comparer.Equals(x.Item, item))
                        .Select(x => index + x.Index)
                        .DefaultIfEmpty(-1)
                        .First();

                if (oldIndex == -1)
                {
                    Insert(index, item);
                }
                else
                {
                    Move(oldIndex, index);
                }
            }

            while (this.Count > subject.Count)
            {
                RemoveAt(subject.Count);
            }

            _currentSubject = subject as INotifyCollectionChanged;

            if (_currentSubject != null)
            {
                _currentSubject.CollectionChanged += _currentSubject_CollectionChanged;
            }
        }

        private void _currentSubject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    UpdateOnAdd(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    UpdateOnMove(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    UpdateOnRemove(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UpdateOnReplace(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateOnReset(e);
                    break;
            }
        }

        private void UpdateOnAdd(NotifyCollectionChangedEventArgs e)
        {
            var insertIndex = e.NewStartingIndex;
            foreach (var item in e.NewItems.OfType<T>())
            {
                this.Insert(insertIndex++, item);
            }
        }

        private void UpdateOnRemove(NotifyCollectionChangedEventArgs e)
        {
            for (int idx = 0; idx < e.OldItems.Count; idx++)
            {
                this.RemoveAt(e.OldStartingIndex);
            }
        }

        private void UpdateOnReplace(NotifyCollectionChangedEventArgs e)
        {
            var replaceIndex = e.OldStartingIndex;
            foreach (var item in e.NewItems.OfType<T>())
            {
                this[replaceIndex++] = item;
            }
        }

        private void UpdateOnMove(NotifyCollectionChangedEventArgs e)
        {
            for (int idx = 0; idx < e.OldItems.Count; idx++)
            {
                this.Move(idx + e.OldStartingIndex, idx + e.NewStartingIndex);
            }
        }

        private void UpdateOnReset(NotifyCollectionChangedEventArgs e)
        {
            this.Clear();
        }
    }
}
