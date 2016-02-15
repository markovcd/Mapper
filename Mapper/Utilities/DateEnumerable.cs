using System;
using System.Collections;
using System.Collections.Generic;

namespace Mapper.Utilities
{
    public enum Period { Daily, Monthly }

    class DateEnumerable : IEnumerable<DateTime>
    {
        public Period Period { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public int Interval { get; private set; }

        public DateEnumerable(Period period, DateTime from, DateTime to, int interval = 1)
        {
            Period = period;
            From = from;
            To = to;
            Interval = interval;
        }

        private DateTime Add(DateTime d)
        {
            switch (Period)
            {
                case Period.Daily:
                    return d.AddDays(Interval);
                case Period.Monthly:
                    return d.AddMonths(Interval);
            }

            throw new InvalidOperationException();
        }

        private IEnumerable<DateTime> Enumerate()
        {
            for (var d = From; d <= To; d = Add(d)) yield return d;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }
    }
}
