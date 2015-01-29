using System;

namespace CovrIn.Utilities
{
    public struct Interval : IComparable<Interval>
    {
        public Interval(int start, int length = 0) : this()
        {
            this.Start = start;
            this.Length = length;
        }

        public int Start { get; private set; }

        public int Length { get; private set; }

        public int End
        {
            get
            {
                return Start + Length;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Length == 0;
            }
        }

        public int CompareTo(Interval other)
        {
            return Start - other.Start;
        }

        public override string ToString()
        {
            return string.Format("[Interval: <{0}, {1}>]", Start, End);
        }
    }
}

