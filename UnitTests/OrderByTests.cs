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
    public class OrderByTests
    {
        [TestMethod]
        public void Result_is_correct_initially()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 5, 6, 7, 0, 3, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[4]);
            Assert.AreEqual(6, result[6]);
            Assert.AreEqual(8, result.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Add()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 6, 7, 0, 3, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Add(9);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[4]);
            Assert.AreEqual(6, result[6]);
            Assert.AreEqual(9, result[8]);
            Assert.AreEqual(9, result.Count);

            subject.Add(8);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[4]);
            Assert.AreEqual(6, result[6]);
            Assert.AreEqual(8, result[8]);
            Assert.AreEqual(9, result[9]);
            Assert.AreEqual(10, result.Count);

            subject.Add(5);

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[4]);
            Assert.AreEqual(6, result[6]);
            Assert.AreEqual(7, result[8]);
            Assert.AreEqual(8, result[9]);
            Assert.AreEqual(9, result[10]);
            Assert.AreEqual(11, result.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Insert()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Insert(1, 10);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(10, result[4]);
            Assert.AreEqual(5, result.Count);

            subject.Insert(3, -1);

            Assert.AreEqual(-1, result[0]);
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(6, result[4]);
            Assert.AreEqual(10, result[5]);
            Assert.AreEqual(6, result.Count);

            subject.Insert(4, 7);

            Assert.AreEqual(-1, result[0]);
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(2, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(6, result[4]);
            Assert.AreEqual(7, result[5]);
            Assert.AreEqual(10, result[6]);
            Assert.AreEqual(7, result.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Move()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Move(1, 3);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject.Move(3, 0);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Remove()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject.Remove(2);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
            Assert.AreEqual(3, result.Count);

            subject.Remove(1);

            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(6, result[1]);
            Assert.AreEqual(2, result.Count);

            subject.Remove(6);

            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(1, result.Count);

            subject.Remove(4);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Result_is_updated_after_Replace()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject[1] = 3;

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject[1] = 5;

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(5, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);

            subject[3] = 0;

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(5, result[2]);
            Assert.AreEqual(6, result[3]);
            Assert.AreEqual(4, result.Count);
        }
    }
}
