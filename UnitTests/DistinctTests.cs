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
    public class DistinctTests
    {
        [TestMethod]
        public void Result_is_correct_initially()
        {
            var subject = new ObservableCollection<int>(new int[] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            EnumerableAssert.AreEqual(result, 0, 1, 2, 7);
        }

        [TestMethod]
        public void Result_is_updated_after_Add()
        {
            var subject = new ObservableCollection<int>(new int[] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct();
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
        public void Result_is_updated_after_Remove()
        {
            var subject = new ObservableCollection<int>(new int[] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct();
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
            var subject = new ObservableCollection<int>(new int[] { 0, 1, 2, 1, 2, 0, 2, 7 });
            var result = subject.ToQueryable().Distinct();
            Assert.IsInstanceOfType(result, typeof(IQueryableObservableCollection<int>));

            subject[0] = 3;
            EnumerableAssert.AreEqual(result, 3, 1, 2, 0, 7);
        }
    }
}
