using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogic.Controller
{
    public class TimeInvestmentController
    {
        private const string SAVE_FILE_NAME = "TimeInvestments.dat";

        /// <summary>
        /// All time investments (To get correct today`s invested time - use TodaysTimeInvestment)
        /// </summary>
        public List<TimeInvestment> TimeInvestments { get; }

        /// <summary>
        /// Correct today's invested time.
        /// </summary>
        public TimeSpan TodaysInvestedTime
        {
            get
            {
                var todaysSavedTimeInvestment = TimeInvestments.SingleOrDefault(i => i.Date == DateTime.Today);
                if (todaysSavedTimeInvestment == null)
                    throw new NullReferenceException("Today's time investment can't be null.");

                var elapsedStopwatchTime = _stopwatch.Elapsed;
                return todaysSavedTimeInvestment.InvestedTime + elapsedStopwatchTime;
            }
        }

        /// <summary>
        /// Time investment counter.
        /// </summary>
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Create new data or load exiting data.
        /// </summary>
        public TimeInvestmentController()
        {
            TimeInvestments = LoadTimeInvestments();
            if (TimeInvestments.SingleOrDefault(i => i.Date == DateTime.Today) == null)
                TimeInvestments.Add(new TimeInvestment(DateTime.Today, TimeSpan.Zero));
        }

        private List<TimeInvestment> LoadTimeInvestments()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0 && formatter.Deserialize(fileStream) is List<TimeInvestment> timeInvestments)
                    return timeInvestments;
                return new List<TimeInvestment>();
            }
        }

        private void SaveTimeInvestments()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, TimeInvestments);
            }
        }

        /// <summary>
        /// Start stopwatch.
        /// </summary>
        public void StartSpendingTime()
        {
            _stopwatch.Start();
        }

        /// <summary>
        /// Stop stopwatch and save data to file.
        /// </summary>
        public void StopSpendingTime()
        {
            var timeElapsed = _stopwatch.Elapsed;

            var todayTimeInvestment = TimeInvestments.SingleOrDefault(i => i.Date == DateTime.Today);
            if (todayTimeInvestment == null)
            {
                TimeInvestments.Add(new TimeInvestment(DateTime.Today, TimeSpan.Zero));
                todayTimeInvestment = TimeInvestments[TimeInvestments.Count - 1];
            }
            todayTimeInvestment.InvestedTime += timeElapsed;

            SaveTimeInvestments();

            _stopwatch.Reset();
        }

        public void AddDescription(string description, DateTime date)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description), "Argument can't be null");

            var timeInvestmentByDate = TimeInvestments.SingleOrDefault(t => t.Date == date);
            if (timeInvestmentByDate != null) timeInvestmentByDate.Description = description;
        }

        public TimeInvestment GetTimeInvestmentByDate(DateTime dateTime)
        {
            return TimeInvestments.SingleOrDefault(i => i.Date == dateTime);
        }

        public TimeSpan GetInvestedTimeByDateRange(DateTime startDate, DateTime endDate)
        {
            TimeSpan investedTime = TimeSpan.Zero;
            for (DateTime i = startDate; i <= endDate; i.AddDays(1))
            {
                investedTime += GetTimeInvestmentByDate(i).InvestedTime;
            }

            return investedTime;
        }
    }
}