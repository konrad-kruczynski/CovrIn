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

		public bool DivideIfNecessary(int offset)
		{
			var index = intervals.BinarySearch(new Interval(offset));
			if(index == -1)
			{
				return false;
			}
			if(index < 0)
			{
				index = (~index) - 1;
				if(offset >= intervals[index].End)
				{
					return false;
				}
			}
			var intervalToDivide = intervals[index];
			var left = new Interval(intervalToDivide.Start, offset - intervalToDivide.Start);
			var right = new Interval(offset, intervalToDivide.End - offset);
			intervals.RemoveAt(index);
			Insert(left);
			Insert(right);
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
			// TODO: prevent overlapping
			intervals.Add(interval);
			// TODO: insertion sort maybe?
			intervals.Sort();
		}

		private readonly List<Interval> intervals;
	}
}

