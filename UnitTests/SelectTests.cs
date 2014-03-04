using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace UnitTests
{
    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void Initial_Select_Is_Correct()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(14, result[7]);
        }

        [TestMethod]
        public void Select_Is_Updated_On_Add()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            subject.Add(8);
            Assert.AreEqual(16, result[8]);
        }

        [TestMethod]
        public void Select_Is_Updated_On_Insert()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            subject.Insert(3, 8);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(16, result[3]);
            Assert.AreEqual(6, result[4]);
        }

        [TestMethod]
        public void Select_Is_Updated_On_Remove()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            subject.Remove(5);
            Assert.AreEqual(12, result[5]);
            subject.RemoveAt(5);
            Assert.AreEqual(14, result[5]);
        }

        [TestMethod]
        public void Select_Is_Updated_On_Replace()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            subject[3] = 8;
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(16, result[3]);
            Assert.AreEqual(8, result[4]);
        }

        [TestMethod]
        public void Select_Is_Updated_On_Move()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Select(i => i * 2).ToObservable();
            subject.Move(3, 7);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(8, result[3]);
            Assert.AreEqual(10, result[4]);
            Assert.AreEqual(6, result[7]);
        }
    }
}
