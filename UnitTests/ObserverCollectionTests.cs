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
    public class ObserverCollectionTests
    {
        [TestMethod]
        public void Follows_observed_collection()
        {
            var subject = new ObservableCollection<int>(new[] { 11, 12, 13, 14, 15, 16 });

            var observer = new ObserverCollection<int>();

            observer.Observe(subject);
            EnumerableAssert.AreEqual(subject, observer);

            subject.Insert(3, 77);
            EnumerableAssert.AreEqual(subject, observer);

            subject.Add(21);
            EnumerableAssert.AreEqual(subject, observer);

            subject.RemoveAt(0);
            EnumerableAssert.AreEqual(subject, observer);

            subject.RemoveAt(subject.Count - 1);
            EnumerableAssert.AreEqual(subject, observer);

            subject.Move(3, 0);
            EnumerableAssert.AreEqual(subject, observer);

            subject.Move(1, 5);
            EnumerableAssert.AreEqual(subject, observer);

            subject[4] = 777;
            EnumerableAssert.AreEqual(subject, observer);
        }

        [TestMethod]
        public void Switches_to_new_observable()
        {
            var subject1 = new ObservableCollection<int>(new[] { 11, 12, 13, 14, 15, 16 });
            var subject2 = new ObservableCollection<int>(new[] { 13, 14, 15, 16, 17, 18 });

            var observer = new ObserverCollection<int>();

            observer.Observe(subject1);
            EnumerableAssert.AreEqual(subject1, observer);

            subject1.Insert(3, 77);
            EnumerableAssert.AreEqual(subject1, observer);

            observer.Observe(subject2);
            EnumerableAssert.AreEqual(subject2, observer);

            subject2.Insert(3, 77);
            EnumerableAssert.AreEqual(subject2, observer);
        }
    }
}
