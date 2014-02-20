using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class RewindMethods
    {
        public static IReadOnlyList<T> Rewind<T>(this IReadOnlyList<T> @this, NotifyCollectionChangedEventArgs change)
        {
            return new SherlockCollection<T>(@this, change);
        }

        private struct SherlockCollection<T> : IReadOnlyList<T>
        {
            private readonly IReadOnlyList<T> _collection;
            private readonly NotifyCollectionChangedEventArgs _change;

            public SherlockCollection(IReadOnlyList<T> currentState, NotifyCollectionChangedEventArgs change)
            {
                if (change.Action == NotifyCollectionChangedAction.Reset)
                {
                    throw new ArgumentException("'Reset' not supported.", "change");
                }

                _collection = currentState;
                _change = change;
            }

            public T this[int index]
            {
                get
                {
                    switch (_change.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            if (index < _change.NewStartingIndex)
                            {
                                return _collection[index];
                            }
                            else
                            {
                                return _collection[index + _change.NewItems.Count];
                            }
                        case NotifyCollectionChangedAction.Move:
                            if (index < Math.Min(_change.NewStartingIndex, _change.OldStartingIndex))
                            {
                                return _collection[index];
                            }
                            else if (index >= Math.Max(_change.NewStartingIndex + _change.NewItems.Count, _change.OldStartingIndex + _change.OldItems.Count))
                            {
                                return _collection[index - _change.OldItems.Count + _change.NewItems.Count];
                            }
                            else if (_change.NewStartingIndex < _change.OldStartingIndex)
                            {
                                if (index < _change.OldStartingIndex)
                                {
                                    return _collection[index + _change.NewItems.Count];
                                }
                                else
                                {
                                    return (T)_change.OldItems[index - _change.OldStartingIndex];
                                }
                            }
                            else
                            {
                                if (index < _change.OldStartingIndex + _change.OldItems.Count)
                                {
                                    return (T)_change.OldItems[index - _change.OldStartingIndex];
                                }
                                else
                                {
                                    return _collection[index - _change.OldItems.Count];
                                }
                            }
                        case NotifyCollectionChangedAction.Remove:
                            if (index < _change.OldStartingIndex)
                            {
                                return _collection[index];
                            }
                            else if (index < _change.OldStartingIndex + _change.OldItems.Count)
                            {
                                return (T)_change.OldItems[index - _change.OldStartingIndex];
                            }
                            else
                            {
                                return _collection[index - _change.OldItems.Count];
                            }
                        case NotifyCollectionChangedAction.Replace:
                            if (index < _change.OldStartingIndex)
                            {
                                return _collection[index];
                            }
                            else if (index < _change.OldStartingIndex + _change.OldItems.Count)
                            {
                                return (T)_change.OldItems[index - _change.OldStartingIndex];
                            }
                            else
                            {
                                return _collection[index];
                            }
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            public int Count
            {
                get
                {
                    switch (_change.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            return _collection.Count - _change.NewItems.Count;
                        case NotifyCollectionChangedAction.Move:
                            return _collection.Count;
                        case NotifyCollectionChangedAction.Remove:
                            return _collection.Count + _change.OldItems.Count;
                        case NotifyCollectionChangedAction.Replace:
                            return _collection.Count;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int index = 0; index < Count; index++)
                {
                    yield return this[index];
                }
            }

            Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
