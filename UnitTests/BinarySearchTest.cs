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
    public class BinarySearchTest
    {
        [TestMethod]
        public void Find_element()
        {
            var subject = new ObservableCollection<int>(new [] { 1, 2, 6, 6, 7, 0, 3, 4 });

            var result = subject.ToQueryable().OrderBy(i => i);

            var firstPosition = result.BinarySearch(0, i => i);

            Assert.AreEqual(0, firstPosition.Index);
            Assert.AreEqual(0, firstPosition.Offset);

            var beforeFirstPosition = result.BinarySearch(-1, i => i);

            Assert.AreEqual(-1, beforeFirstPosition.Index);
            Assert.AreEqual(1, beforeFirstPosition.Offset);

            var lastPosition = result.BinarySearch(7, i => i);

            Assert.AreEqual(7, lastPosition.Index);
            Assert.AreEqual(0, lastPosition.Offset);

            var afterLastPosition = result.BinarySearch(17, i => i);

            Assert.AreEqual(7, afterLastPosition.Index);
            Assert.AreEqual(1, afterLastPosition.Offset);

            var middlePosition = result.BinarySearch(3, i => i);

            Assert.AreEqual(3, middlePosition.Index);
            Assert.AreEqual(0, middlePosition.Offset);

            var missingPosition = result.BinarySearch(5, i => i);

            Assert.AreEqual(4, missingPosition.Index);
            Assert.AreEqual(1, missingPosition.Offset);
        }
    }
}
