namespace Shared
{
    using System;

    public struct DateTimeRange
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public static bool operator ==(DateTimeRange a, DateTimeRange b)
            => a.Start == b.Start && a.End == b.End;
        public static bool operator !=(DateTimeRange a, DateTimeRange b)
            => a.Start != b.Start && a.End != b.End;

        public override string ToString()
            => $"{Start} - {End}";

        public override bool Equals(object obj)
            => obj is DateTimeRange range &&
               Start == range.Start &&
               End == range.End;

        public override int GetHashCode()
            => HashCode.Combine(Start, End);
    }
}
