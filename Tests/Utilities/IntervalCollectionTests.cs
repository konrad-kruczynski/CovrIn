using System;
using NUnit.Framework;
using System.Collections.Generic;
using CovrIn.Utilities;
using System.Linq;

namespace CovrIn.Tests.Utilities
{
    [TestFixture]
    public class IntervalCollectionTests
    {
        [Test]
        public void ShouldInsertElements()
        {
            var intervalsToAdd = new List<Interval>
            {
                new Interval(38, 2),
                new Interval(0, 3),
                new Interval(60, 10),
                new Interval(23, 2),
                new Interval(13, 2),
                new Interval(38, 2)
            };
            //fill all gaps
            var expectedIntervals = intervals.Union(intervalsToAdd).OrderBy(x => x.Start);
            AddIntervals(intervalsToAdd);
            CollectionAssert.AreEqual(expectedIntervals, intervalCollection);
        }

        /// <summary>
        /// This tests if the collection does a split at the beginning of the added symbol.
        /// It does not check for splitting at its end.
        /// </summary>
        [Test]
        public void ShouldSplitInterval()
        {
            var intervalsToAdd = new List<Interval>
            {
                new Interval(22, 1),
                new Interval(47, 1),
                new Interval(10, 3),
                new Interval(41, 7),
                new Interval(28, 10),
                new Interval(55, 5)
            };
            AddIntervals(intervalsToAdd);
            CollectionAssert.AreEqual(new []
            {
                new Interval(3, 7),
                new Interval(10, 3),
                new Interval(15, 7),
                new Interval(22, 1),
                new Interval(25, 3),
                new Interval(28, 10),
                new Interval(40, 1),
                new Interval(41, 6),
                new Interval(47, 1),
                new Interval(50, 5),
                new Interval(55, 5)
            }, intervalCollection);
        }

        /// <summary>
        /// Uses 3-long intervals to cause exceptions.
        /// </summary>
        /// <param name="intervalStart">Symbol beginnings.</param>
        [Test, Ignore]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotOverlapIntervals(
            [Values(1, 12, 21, 23, 36, 39, 47, 58)] int intervalStart)
        {
            AddIntervals(new []{ new Interval(intervalStart, 3) });
        }

        [Test]
        public void ShouldSortIntervals()
        {
            intervalCollection.Insert(100, 1);
            intervalCollection.Insert(80, 1);
            intervalCollection.Insert(90, 1);
            intervalCollection.Insert(70, 1);
            intervalCollection.Insert(50, 1);

            var originalOrder = intervalCollection.ToArray();
            var sortedOrder = originalOrder.OrderBy(x => x.Start).ToArray();
            CollectionAssert.AreEqual(sortedOrder, originalOrder);
        }

        [SetUp]
        public void TestSetUp()
        {
            intervalCollection = new IntervalCollection();
            foreach(var interval in intervals)
            {
                intervalCollection.Insert(interval.Start, interval.Length);
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            intervals = new List<Interval>();
            intervals.Add(new Interval(3, 10));
            intervals.Add(new Interval(15, 8));
            intervals.Add(new Interval(25, 13));
            intervals.Add(new Interval(40, 8));
            intervals.Add(new Interval(50, 10));
        }

        private void AddIntervals(IEnumerable<Interval> intervalsToAdd)
        {
            foreach(var interval in intervalsToAdd)
            {
                int unused;
                if(!intervalCollection.DivideIfNecessary(interval.Start, out unused))
                {
                    intervalCollection.Insert(interval.Start, interval.Length);
                }
            }
        }

        private IntervalCollection intervalCollection;
        private List<Interval> intervals;
    }
}
