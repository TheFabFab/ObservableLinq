using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class EnumerableAssert
    {
        public static void AreEqual<T>(IEnumerable<T> actual, params T[] args)
        {
            AreEqual(args, actual);
        }

        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var enumExpected = expected.GetEnumerator();
            var enumActual = actual.GetEnumerator();
            int count = 0;
            while (true)
            {
                bool moveExpectedSuccess = enumExpected.MoveNext();
                bool moveActualSuccess = enumActual.MoveNext();

                if (moveExpectedSuccess && moveActualSuccess)
                {
                    Assert.AreEqual(enumExpected.Current, enumActual.Current);
                }
                else if (!moveExpectedSuccess && !moveActualSuccess)
                {
                    break;
                }
                else
                {
                    Assert.Fail("Expected:<{0}> items. Actual:<{1}> items.", expected.Count(), actual.Count());
                }

                count++;
            }
        }
    }
}
