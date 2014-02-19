using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.Linq
{
    public static class SelectMethods
    {
        public static IQueryableObservableCollection<TResult> Select<T, TResult>(this IQueryableObservableCollection<T> @this, Func<T, TResult> selector)
        {
            return new ProjectedObservableCollection<T, TResult>(@this, selector);
        }

        public static IQueryableObservableCollection<TResult> Select<T, TResult>(this IQueryableObservableCollection<T> @this, Func<T, int, TResult> selector)
        {
            throw new NotImplementedException();
        }

        private class ProjectedObservableCollection<T, TResult> : DerivedObservableCollection<T, TResult>
        {
            private readonly Func<T, TResult> _selector;
            private readonly IQueryableObservableCollection<T> _source;

            public ProjectedObservableCollection(IQueryableObservableCollection<T> source, Func<T, TResult> selector)
                : base(source, ((IEnumerable<T>)source).Select(selector))
            {
                _selector = selector;
                _source = source;
            }

            protected override void UpdateOnAdd(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnAdd(e);

                var insertIndex = e.NewStartingIndex;
                foreach (var item in e.NewItems.OfType<T>().Select(_selector))
                {
                    this.Insert(insertIndex++, item);
                }
            }

            protected override void UpdateOnRemove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnRemove(e);

                for (int idx = 0; idx < e.OldItems.Count; idx++)
                {
                    this.RemoveAt(e.OldStartingIndex);
                }
            }

            protected override void UpdateOnReplace(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReplace(e);

                var replaceIndex = e.OldStartingIndex;
                foreach (var item in e.NewItems.OfType<T>().Select(_selector))
                {
                    this[replaceIndex++] = item;
                }
            }

            protected override void UpdateOnMove(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnMove(e);

                for (int idx = 0; idx < e.OldItems.Count; idx++)
                {
                    this.Move(idx + e.OldStartingIndex, idx + e.NewStartingIndex);
                }
            }

            protected override void UpdateOnReset(NotifyCollectionChangedEventArgs e)
            {
                base.UpdateOnReset(e);

                using (this.GetNotificationLock())
                {
                    this.Clear();
                    var index = 0;
                    foreach (var item in ((IEnumerable<T>)_source).Select(_selector))
                    {
                        this.Insert(index++, item);
                    }
                }
            }
        }
    }
}
