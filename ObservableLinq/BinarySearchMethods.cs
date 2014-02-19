using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class BinarySearchMethods
    {
        public static ItemPosition BinarySearch<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, TKey key, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return BinarySearch(@this, key, keySelector, comparer, false, 0, @this.Count);
        }

        public static ItemPosition BinarySearch<TSource, TKey>(this IOrderedQueryableObservableCollection<TSource> @this, TKey key, Func<TSource, TKey> keySelector)
        {
            return BinarySearch(@this, key, keySelector, Comparer<TKey>.Default);
        }

        private static ItemPosition BinarySearch<TSource, TKey>(IOrderedQueryableObservableCollection<TSource> @this, TKey key, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending, int start, int count)
        {
            if (count <= 0) return new ItemPosition(start - 1, 1);

            int middle = start + count / 2;

            var middleKey = keySelector(@this[middle]);

            int comparison = comparer.Compare(key, middleKey) * (descending ? -1 : +1);

            switch (Math.Sign(comparison))
            {
                case -1:
                    return BinarySearch(@this, key, keySelector, comparer, descending, start, count / 2);
                case 1:
                    return BinarySearch(@this, key, keySelector, comparer, descending, start + count / 2 + 1, count - count / 2 - 1);
                default:
                    return new ItemPosition(middle, 0);
            }
        }

    }
}
