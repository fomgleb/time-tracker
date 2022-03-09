using System;

namespace TimeTracker.BusinessLogic.Model
{
    [Serializable]
    public struct TimeInvestment
    {
        /// <summary>
        /// The date the time was invested.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// How much time invested was.
        /// </summary>
        public TimeSpan InvestedTime { get; }

        /// <summary>
        /// Where was the time invested.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Time investment with Date - today, Invested Time - zero, and empty description.
        /// </summary>
        public static TimeInvestment TodaysZero()
        {
            return new TimeInvestment(DateTime.Today, TimeSpan.Zero, "");
        }

        /// <summary>
        /// Create new time investment.
        /// </summary>
        public TimeInvestment(DateTime date, TimeSpan investedTime) : this(date, investedTime, "") { }

        /// <summary>
        /// Create new time investment.
        /// </summary>
        public TimeInvestment(DateTime date, TimeSpan investedTime, string description)
        {
            if (investedTime.TotalSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(investedTime), "The invested time can't be negative.");
            if (description == null)
                throw new ArgumentNullException(nameof(description), "The description can't be null.");

            Date = date;
            InvestedTime = investedTime;
            Description = description;
        }

        public TimeInvestment AddInvestedTime(TimeSpan time)
        {
            if (InvestedTime.TotalHours + time.TotalHours < 0 || InvestedTime.TotalHours + time.TotalHours > 24)
                throw new ArgumentOutOfRangeException(nameof(time),
                    "The invested time filed can't be fewer than zero and greater than 24 hours");
            return new TimeInvestment(Date, InvestedTime + time, Description);
        }

        public TimeInvestment SetDescription(string description)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description), "The description can't be null.");
            return new TimeInvestment(Date, InvestedTime, description);
        }
    }
}