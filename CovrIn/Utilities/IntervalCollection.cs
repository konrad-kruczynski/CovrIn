using System;
using System.Collections.Generic;

namespace CovrIn.Utilities
{
    internal sealed class IntervalCollection : IEnumerable<Interval>
    {
        public IntervalCollection()
        {
            intervals = new List<Interval>();
        }

        public bool DivideIfNecessary(int offset, out int nextIntervalStart)
        {
            var index = intervals.BinarySearch(new Interval(offset));
            if(index == -1)
            {
                nextIntervalStart = intervals.Count > 0 ? intervals[0].Start : -1;
                return false;
            }
            if(index < 0)
            {
                index = (~index) - 1;
                if(offset >= intervals[index].End)
                {
                    index++;
                    nextIntervalStart = index == intervals.Count ? -1 : intervals[index].Start;
                    return false;
                }
            }
            var intervalToDivide = intervals[index];
            var left = new Interval(intervalToDivide.Start, offset - intervalToDivide.Start);
            var right = new Interval(offset, intervalToDivide.End - offset);
            intervals.RemoveAt(index);
            Insert(left);
            Insert(right);
            nextIntervalStart = -1;
            return true;
        }

        public void Insert(int offset, int length)
        {
            Insert(new Interval(offset, length));
        }

        public IEnumerator<Interval> GetEnumerator()
        {
            foreach(var interval in intervals)
            {
                yield return interval;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Insert(Interval interval)
        {
            if(interval.IsEmpty)
            {
                return;
            }
            intervals.Add(interval);
            intervals.Sort();
        }

        private readonly List<Interval> intervals;
    }
}

