using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class RewindTests
    {
        [TestMethod]
        public void On_remove()
        {
            var original = new[] {10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};

            ObservableCollection<int> subject;
            IReadOnlyList<int> sherlock;
            var changes = new Queue<NotifyCollectionChangedEventArgs>();

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.RemoveAt(3);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.RemoveAt(0);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.RemoveAt(8);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);
        }

        [TestMethod]
        public void On_insert()
        {
            var original = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            ObservableCollection<int> subject;
            IReadOnlyList<int> sherlock;
            var changes = new Queue<NotifyCollectionChangedEventArgs>();

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Insert(3, 777);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Insert(0, 777);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Insert(8, 777);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);
        }

        [TestMethod]
        public void On_move()
        {
            var original = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            ObservableCollection<int> subject;
            IReadOnlyList<int> sherlock;
            var changes = new Queue<NotifyCollectionChangedEventArgs>();

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Move(3, 0);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Move(0, 7);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject.Move(8, 0);
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);
        }

        [TestMethod]
        public void On_replace()
        {
            var original = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            ObservableCollection<int> subject;
            IReadOnlyList<int> sherlock;
            var changes = new Queue<NotifyCollectionChangedEventArgs>();

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject[3] = 777;
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject[0] = 777;
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);

            subject = new ObservableCollection<int>(original);
            subject.CollectionChanged += (s, e) => changes.Enqueue(e);

            subject[8] = 777;
            sherlock = subject.Rewind(changes.Dequeue());
            EnumerableAssert.AreEqual(original, sherlock);
        }
    }
}
