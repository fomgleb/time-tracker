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

        public bool StopwatchIsRunning => _stopwatch.IsRunning;

        /// <summary>
        /// All time investments. It always has today.
        /// </summary>
        private List<TimeInvestment> TimeInvestments { get; set; }

        /// <summary>
        /// Time investment counter.
        /// </summary>
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Rewrite previous data and save this.
        /// </summary>
        public TimeInvestmentController(List<TimeInvestment> timeInvestments)
        {
            TimeInvestments = timeInvestments;
            SaveTimeInvestments();
        }

        /// <summary>
        /// Load exiting data or create new empty one.
        /// </summary>
        public TimeInvestmentController()
        {
            if (!LoadTimeInvestments())
                TimeInvestments = new List<TimeInvestment> {TimeInvestment.TodaysZero()};
        }

        /// <summary>
        /// Load exiting data or create new empty one.
        /// </summary>
        /// <returns> Succeeded or failed. </returns>
        private bool LoadTimeInvestments()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0 && formatter.Deserialize(fileStream) is List<TimeInvestment> timeInvestments)
                {
                    TimeInvestments = timeInvestments;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Save data to file.
        /// </summary>
        public void SaveTimeInvestments()
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

            var todaysTimeInvestment = GetTimeInvestmentByDate(DateTime.Today);
            var indexOfTodaysTimeInvestment = TimeInvestments.IndexOf(todaysTimeInvestment);

            if (indexOfTodaysTimeInvestment == -1)
                throw new Exception("The time investments must contain todays time investment.");

            TimeInvestments[indexOfTodaysTimeInvestment] = TimeInvestments[indexOfTodaysTimeInvestment].AddInvestedTime(timeElapsed);

            SaveTimeInvestments();

            _stopwatch.Reset();
        }

        /// <summary>
        /// Set description in exiting time investment.
        /// </summary>
        /// <param name="description"> The description to set. </param>
        /// <param name="date"> Date of the desired time investment. </param>
        /// <exception cref="ArgumentNullException"> The description can't be null. </exception>
        public void SetDescription(string description, DateTime date)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description), "The description can't be null");

            var timeInvestment = TimeInvestments.SingleOrDefault(t => t.Date == date);
            var timeInvestmentIndex = TimeInvestments.IndexOf(timeInvestment);
            if (timeInvestmentIndex == -1)
                TimeInvestments.Add(new TimeInvestment(date, TimeSpan.Zero, description));
            else
                TimeInvestments[timeInvestmentIndex] = TimeInvestments[timeInvestmentIndex].SetDescription(description);
        }

        /// <summary>
        /// Get the time investment by date.
        /// </summary>
        public TimeInvestment GetTimeInvestmentByDate(DateTime date)
        {
            return TimeInvestments.SingleOrDefault(t => t.Date == date);
        }

        /// <summary>
        /// Returns invested time for day.
        /// </summary>
        public TimeSpan GetInvestedTimeForDay(DateTime date)
        {
            return GetInvestedTimeByDateRange(date, date);
        }

        /// <summary>
        /// Returns invested time for week using a date that is included in this week.
        /// </summary>
        public TimeSpan GetInvestedTimeForWeek(DateTime date)
        {
            var firstDateOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(-6) : date.AddDays(-((int)date.DayOfWeek - 1));
            var lastDateOfWeek = firstDateOfWeek.AddDays(6);
            return GetInvestedTimeByDateRange(firstDateOfWeek, lastDateOfWeek);
        }

        /// <summary>
        /// Returns invested time for month using a date that is included in this month.
        /// </summary>
        public TimeSpan GetInvestedTimeForMonth(DateTime date)
        {
            var firstDateOfMonth = date.AddDays(-date.Day - 1);
            var lastDateOfMonth = firstDateOfMonth.AddDays(DateTime.DaysInMonth(date.Year, date.Month) - 1);
            return GetInvestedTimeByDateRange(firstDateOfMonth, lastDateOfMonth);
        }

        /// <summary>
        /// Find how much time was invested in this time period.
        /// </summary>
        /// <param name="startDate"> Start of time period. </param>
        /// <param name="endDate">End of time period. </param>
        public TimeSpan GetInvestedTimeByDateRange(DateTime startDate, DateTime endDate)
        {
            var investedTime = TimeSpan.Zero;
            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var currentTimeInvestment = TimeInvestments.SingleOrDefault(t => t.Date == currentDate);
                investedTime += currentTimeInvestment.InvestedTime;

                if (currentDate == DateTime.Today)
                    investedTime += _stopwatch.Elapsed;
            }

            return investedTime;
        }
    }
}