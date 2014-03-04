using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class WhereTests
    {
        [TestMethod]
        public void Result_is_filtered_initially()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Where(i => i % 2 == 0).ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public void Result_updated_after_Add()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Where(i => i % 2 == 0).ToObservable();

            subject.Add(8);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(8, result[4]);
            Assert.AreEqual(5, result.Count);

            subject.Add(9);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(8, result[4]);
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void Result_updated_after_Remove()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Where(i => i % 2 == 0).ToObservable();

            subject.Remove(7);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Remove(4);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(6, result[2]);
            Assert.AreEqual(3, result.Count);

            subject.Remove(6);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(2, result.Count);

            subject.Remove(0);

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(1, result.Count);

            subject.Remove(2);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Result_updated_after_Replace()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Where(i => i % 2 == 0).ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject[3] = 4;

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(6, result[4]);
            Assert.AreEqual(5, result.Count);

            subject[3] = 5;

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject[0] = 10;

            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject[0] = 1;

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
            Assert.AreEqual(3, result.Count);

            subject[2] = 1;

            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(6, result[1]);
            Assert.AreEqual(2, result.Count);

            subject[6] = 1;

            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(1, result.Count);

            subject[4] = 1;

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Result_updated_after_Move()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var result = subject.ToQueryable().Where(i => i % 2 == 0).ToObservable();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Move(1, 5);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(0, 2);

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(0, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(0, 7);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
            Assert.AreEqual(2, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(7, 0);

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(0, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(2, 0);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(5, 1);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);
        }
    }
}
