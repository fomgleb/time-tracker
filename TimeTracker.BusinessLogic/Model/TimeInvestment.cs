using System;

namespace TimeTracker.BusinessLogic.Model
{
    [Serializable]
    public class TimeInvestment
    {
        private DateTime _date;
        private TimeSpan _investedTime;
        private string _description;

        /// <summary>
        /// The date the time was invested.
        /// </summary>
        public DateTime Date
        {
            get => _date;
            private set
            {
                if (value > DateTime.Today)
                    throw new ArgumentOutOfRangeException(nameof(value), "The date can't be greater than today.");
                _date = value;
            }
        }

        /// <summary>
        /// How much time invested was.
        /// </summary>
        public TimeSpan InvestedTime
        {
            get => _investedTime;
            set
            {
                if (value.TotalSeconds < 0 || value.TotalHours > 24)
                    throw new ArgumentOutOfRangeException(nameof(value), "The invested time can't be negative and greater than 24 hours.");
                _investedTime = value;
            }
        }

        /// <summary>
        /// Where was the time invested.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value ?? throw new ArgumentNullException(nameof(value), "The description can't be null.");
        }

        /// <summary>
        /// Create new time investment.
        /// </summary>
        /// <param name="date"> Investment date. </param>
        /// <param name="investedTime"> How much time to invest. </param>
        public TimeInvestment(DateTime date, TimeSpan investedTime)
        {
            Date = date;
            InvestedTime = investedTime;
        }
    }
}