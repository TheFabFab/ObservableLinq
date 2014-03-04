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
    public class DistinctTests
    {
        [TestMethod]
        public void Result_is_correct_initially()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
        }

        [TestMethod]
        public void Result_is_updated_after_Add()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Add(0);
            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
            subject.Add(1);
            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
            subject.Add(2);
            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
            subject.Add(7);
            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
            subject.Add(6);
            EnumerableAssert.AreEqual(result, 0, 1, 2, 7, 6);
        }

        [TestMethod]
        public void Result_is_updated_after_Insert()
        {
            var subject = new ObservableCollection<int>(new [] { 10, 11, 12, 11, 12, 10, 12, 17 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            var eventList = new List<NotifyCollectionChangedEventArgs>();

            result.CollectionChanged += (s, e) => eventList.Add(e);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Insert(2, 10);
            EnumerableAssert.AreEqual(result, 10, 11, 12, 17);
            Assert.AreEqual(0, eventList.Count);
            
            subject.Insert(1, 11);
            EnumerableAssert.AreEqual(result, 10, 11, 12, 17);
            Assert.AreEqual(0, eventList.Count);
            
            subject.Insert(1, 12);
            EnumerableAssert.AreEqual(result, 10, 12, 11, 17);
            Assert.AreEqual(1, eventList.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, eventList.Last().Action);
            
            subject.Insert(0, 17);
            EnumerableAssert.AreEqual(result, 17, 10, 12, 11);
            Assert.AreEqual(2, eventList.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, eventList.Last().Action);
        }

        [TestMethod]
        public void Result_is_updated_after_Remove()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Remove(0);
            EnumerableAssert.AreEqual(result, 1, 2, 0, 7);

            subject.Remove(0);
            EnumerableAssert.AreEqual(result, 1, 2, 7);

            subject.Remove(7);
            EnumerableAssert.AreEqual(result, 1, 2);
        }

        [TestMethod]
        public void Result_is_updated_after_Replace()
        {
            var subject = new ObservableCollection<int>(new [] { 10, 11, 12, 11, 12, 10, 12, 17 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            
            var events = new List<NotifyCollectionChangedEventArgs>();
            result.CollectionChanged += (s, e) => events.Add(e);

            subject[0] = 13;
            EnumerableAssert.AreEqual(result, 13, 11, 12, 10, 17);
            Assert.AreEqual(2, events.Count);

            subject[0] = 13;
            EnumerableAssert.AreEqual(result, 13, 11, 12, 10, 17);
            Assert.AreEqual(2, events.Count);

            subject[1] = 12;
            EnumerableAssert.AreEqual(result, 13, 12, 11, 10, 17);
            Assert.AreEqual(3, events.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Move()
        {
            var subject = new ObservableCollection<int>(new [] { 10, 11, 12, 11, 12, 10, 12, 17 });
            var result = subject.ToQueryable().Distinct().ToObservable();
            var expected = subject.Distinct();

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            var events = new Queue<NotifyCollectionChangedEventArgs>();
            result.CollectionChanged += (s, e) => events.Enqueue(e);

            subject.Move(4, 7);
            expected = subject.Distinct();
            EnumerableAssert.AreEqual(result, expected);
            Assert.AreEqual(0, events.Count);

            subject.Move(3, 0);
            expected = subject.Distinct();
            EnumerableAssert.AreEqual(result, expected);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, events.Dequeue().Action);

            subject.Move(2, 1);
            expected = subject.Distinct();
            EnumerableAssert.AreEqual(result, expected);
            Assert.AreEqual(0, events.Count);

            subject.Move(5, 1);
            expected = subject.Distinct();
            EnumerableAssert.AreEqual(result, expected);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, events.Dequeue().Action);
        }
    }
}
