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
                _comparer = comparer ?? EqualityComparer<T>.Default;
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
                    UpdateOnInsert(_source.Rewind(e), indexInSource, item);

                    indexInSource++;
                }
            }

            private void UpdateOnInsert(IReadOnlyList<T> originalSource, int newSourceIndex, T item)
            {
                int originalSourceIndex = 0;
                int sourceIndex = 0;
                int targetIndex = 0;
                int firstSourceIndex = -1;
                
                int newTargetIndex = -1;
                int oldTargetIndex = -1;

                while (sourceIndex < _source.Count)
                {
                    if (sourceIndex == newSourceIndex + 1)
                    {
                        originalSourceIndex--;
                    }
                    else if (oldTargetIndex == -1 && originalSourceIndex < originalSource.Count && _comparer.Equals(originalSource[originalSourceIndex], item))
                    {
                        oldTargetIndex = targetIndex;
                    }

                    if (firstSourceIndex == -1 && _comparer.Equals(_source[sourceIndex], item))
                    {
                        firstSourceIndex = sourceIndex;
                        newTargetIndex = targetIndex;
                    }

                    if (targetIndex < this.Count && _comparer.Equals(_source[sourceIndex], this[targetIndex]))
                    {
                        targetIndex++;
                    }

                    sourceIndex++;
                    originalSourceIndex++;
                }

                System.Diagnostics.Debug.Assert(newTargetIndex != -1);

                if (oldTargetIndex == -1)
                {
                    Insert(newTargetIndex, item);
                }
                else if (newTargetIndex < oldTargetIndex)
                {
                    Move(oldTargetIndex, newTargetIndex);
                }
            }

            protected override void UpdateOnRemove(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                var indexInSource = e.OldStartingIndex;

                foreach (var item in e.OldItems.OfType<T>())
                {
                    UpdateOnRemove(_source.Rewind(e), indexInSource, item);

                    indexInSource++;
                }
            }

            private void UpdateOnRemove(IReadOnlyList<T> originalSource, int oldSourceIndex, T item)
            {
                int targetIndex = 0;
                int firstSourceIndex = -1;

                int newTargetIndex = -1;
                int oldTargetIndex = -1;

                for (int index = 0; index < originalSource.Count; index++)
                {
                    int sourceIndex = index < oldSourceIndex ? index : index - 1;
                    int originalSourceIndex = index;

                    if (firstSourceIndex == -1 && 
                        sourceIndex > -1 &&
                        _comparer.Equals(_source[sourceIndex], item))
                    {
                        firstSourceIndex = sourceIndex;
                        newTargetIndex = targetIndex;
                    }

                    if (targetIndex < this.Count && 
                        sourceIndex > -1 &&
                        _comparer.Equals(_source[sourceIndex], this[targetIndex]))
                    {
                        targetIndex++;
                    }

                    if (index == oldSourceIndex)
                    {
                        oldTargetIndex = targetIndex;
                        targetIndex++;
                    }
                }

                System.Diagnostics.Debug.Assert(oldTargetIndex != -1);
                if (newTargetIndex == -1)
                {
                    RemoveAt(oldTargetIndex);
                }
                else
                {
                    Move(oldTargetIndex, newTargetIndex - 1);
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
                        UpdateOnReplace(_source.Rewind(e), indexInSource, newItem, oldItem);
                    }
                }
            }

            private void UpdateOnReplace(IReadOnlyList<T> originalSource, int sourceIndexOfItem, T newItem, T oldItem)
            {
                int targetIndex = 0;

                int newTargetIndexForNewItem = -1;
                int oldTargetIndexForNewItem = -1;

                int newTargetIndexForOldItem = -1;
                int oldTargetIndexForOldItem = -1;

                for (int sourceIndex = 0; sourceIndex < originalSource.Count; sourceIndex++)
                {
                    if (newTargetIndexForNewItem == -1 &&
                        _comparer.Equals(_source[sourceIndex], newItem))
                    {
                        newTargetIndexForNewItem = targetIndex;
                    }

                    if (newTargetIndexForOldItem == -1 &&
                        _comparer.Equals(_source[sourceIndex], oldItem))
                    {
                        newTargetIndexForOldItem = targetIndex;
                    }

                    if (oldTargetIndexForOldItem == -1 &&
                        _comparer.Equals(originalSource[sourceIndex], oldItem))
                    {
                        oldTargetIndexForOldItem = targetIndex;
                    }

                    if (oldTargetIndexForNewItem == -1 &&
                        _comparer.Equals(originalSource[sourceIndex], newItem))
                    {
                        oldTargetIndexForNewItem = targetIndex;
                    }

                    if (targetIndex < this.Count &&
                        _comparer.Equals(originalSource[sourceIndex], this[targetIndex]))
                    {
                        targetIndex++;
                    }
                }

                System.Diagnostics.Debug.Assert(oldTargetIndexForOldItem > -1);

                if (newTargetIndexForOldItem == -1)
                {
                    RemoveAt(oldTargetIndexForOldItem);
                }
                else
                {
                    Move(oldTargetIndexForOldItem, newTargetIndexForOldItem - 1);
                }

                if (oldTargetIndexForNewItem == -1)
                {
                    Insert(newTargetIndexForNewItem, newItem);
                }
                else if (oldTargetIndexForNewItem > newTargetIndexForOldItem)
                {
                    Move(oldTargetIndexForNewItem + 1, newTargetIndexForNewItem);
                }
            }

            protected override void UpdateOnMove(Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnMove(e);

                for (int indexInEvent = 0; indexInEvent < e.NewItems.Count; indexInEvent++)
                {
                    int newIndexInSource = e.NewStartingIndex + indexInEvent;
                    int oldIndexInSource = e.OldStartingIndex + indexInEvent;

                    var item = (T)e.OldItems[indexInEvent];

                    UpdateOnMove(_source.Rewind(e), newIndexInSource, oldIndexInSource, item);
                }
            }

            private void UpdateOnMove(IReadOnlyList<T> originalSource, int sourceIndexOfNewItem, int sourceIndexOfOldItem, T item)
            {
                int targetIndex = 0;

                int newTargetIndexForItem = -1;
                int oldTargetIndexForItem = -1;

                for (int sourceIndex = 0; sourceIndex < originalSource.Count; sourceIndex++)
                {
                    if (newTargetIndexForItem == -1 &&
                        _comparer.Equals(_source[sourceIndex], item))
                    {
                        newTargetIndexForItem = targetIndex;
                    }

                    if (oldTargetIndexForItem == -1 &&
                        _comparer.Equals(originalSource[sourceIndex], item))
                    {
                        oldTargetIndexForItem = targetIndex;
                    }

                    if (targetIndex < this.Count &&
                        _comparer.Equals(originalSource[sourceIndex], this[targetIndex]))
                    {
                        targetIndex++;
                    }
                }

                System.Diagnostics.Debug.Assert(newTargetIndexForItem > -1);
                System.Diagnostics.Debug.Assert(oldTargetIndexForItem > -1);

                if (newTargetIndexForItem != oldTargetIndexForItem)
                {
                    Move(oldTargetIndexForItem, newTargetIndexForItem);
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
