using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    internal class DerivedObservableCollection<TSource, TResult> : ReadOnlyObservableCollection<TResult>
    {
        public DerivedObservableCollection(IObservableCollection<TSource> source, IEnumerable<TResult> collection)
            : base(collection)
        {
            source.CollectionChanged += source_CollectionChanged;
        }

        public DerivedObservableCollection(IObservableCollection<TSource> source)
            : this(source, new TResult[]{})
        {
        }

        private void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    UpdateOnAdd(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    UpdateOnRemove(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UpdateOnReplace(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    UpdateOnMove(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateOnReset(e);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        protected virtual void UpdateOnAdd(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void UpdateOnRemove(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void UpdateOnReplace(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void UpdateOnMove(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void UpdateOnReset(NotifyCollectionChangedEventArgs e)
        {
        }

    }
}
