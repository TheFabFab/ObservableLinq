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
    public class BasicTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cannot_change_queryable()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            subject.ToQueryable().Add(10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cannot_change_filtered()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            subject.ToQueryable().Where(x => x / 2 == 0).Add(10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cannot_change_projected()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            subject.ToQueryable().Select(x => x / 2).Add(10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cannot_change_sorted()
        {
            var subject = new ObservableCollection<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7 });
            subject.ToQueryable().OrderBy(x => x).Add(10);
        }

    }
}
