using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.BusinessLogic.Controller;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogicTests.Controller
{
    [TestClass]
    public class TimeInvestmentControllerTests
    {
        [TestMethod]
        public void StartSpendingTimeTest()
        {
            var timeInvestmentController = new TimeInvestmentController();
            var expectedValue = true;
            var actualValue = false;

            timeInvestmentController.StartSpendingTime();
            actualValue = timeInvestmentController.StopwatchIsRunning;

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void StopSpendingTimeTest()
        {
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>());
            var expectedIntValue = 1;
            var expectedBoolValue = false;
            var actualIntValue = 0;
            var actualBoolValue = true;

            timeInvestmentController.StartSpendingTime();
            Thread.Sleep(1000 * expectedIntValue);
            timeInvestmentController.StopSpendingTime();
            actualIntValue = timeInvestmentController.TimeInvestments[0].InvestedTime.Seconds;
            actualBoolValue = timeInvestmentController.StopwatchIsRunning;

            Assert.AreEqual(expectedIntValue, actualIntValue);
            Assert.AreEqual(expectedBoolValue, actualBoolValue);
        }

        [TestMethod]
        public void SetDescriptionTest()
        {
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Today, TimeSpan.Zero)
            });
            var expectedAndInputValue = "Hello, honey!";
            var actualValue = "";

            timeInvestmentController.SetDescription(expectedAndInputValue, DateTime.Today);
            actualValue = timeInvestmentController.TimeInvestments[0].Description;

            Assert.AreEqual(expectedAndInputValue, actualValue);
        }

        [TestMethod]
        public void GetTimeInvestmentByDateTest()
        {
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Parse("11.03.2022"), TimeSpan.Zero, "No 1"),
                new TimeInvestment(DateTime.Parse("12.03.2022"), TimeSpan.Zero, "No 2"),
                new TimeInvestment(DateTime.Parse("13.03.2022"), TimeSpan.Zero, "No 3")
            });
            var inputData = DateTime.Parse("12.03.2022");
            var expectedData = new TimeInvestment(DateTime.Parse("12.03.2022"), TimeSpan.Zero, "No 2");
            TimeInvestment actualData;

            actualData = timeInvestmentController.GetTimeInvestmentByDate(DateTime.Parse("12.03.2022"));

            Assert.AreEqual(expectedData, actualData);
        }

        [TestMethod]
        public void GetInvestedTimeForDayTest()
        {
            const int DATA_COUNT = 2;
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Parse("12.03.2022"), TimeSpan.Parse("4:38:32")),
                new TimeInvestment(DateTime.Today, TimeSpan.Parse("3:30:30"))
            });
            TimeSpan[] expectedData = {TimeSpan.Parse("4:38:32"), TimeSpan.Parse("3:30:31")};
            TimeSpan[] actualData = new TimeSpan[DATA_COUNT];

            actualData[0] = timeInvestmentController.GetInvestedTimeForDay(timeInvestmentController.TimeInvestments[0].Date);
            timeInvestmentController.StartSpendingTime();
            Thread.Sleep(1000);
            timeInvestmentController.StopSpendingTime();
            actualData[1] = timeInvestmentController.GetInvestedTimeForDay(timeInvestmentController.TimeInvestments[1].Date);

            for (int i = 0; i < DATA_COUNT; i++)
                Assert.AreEqual(Math.Round(expectedData[i].TotalSeconds), Math.Round(actualData[i].TotalSeconds));
        }

        [TestMethod]
        public void GetInvestedTimeForWeekTest()
        {
            const int DATA_COUNT = 8;
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Parse("07.03.2022"), TimeSpan.Parse("1:0:0")),
                new TimeInvestment(DateTime.Parse("11.03.2022"), TimeSpan.Parse("1:0:0")),
                new TimeInvestment(DateTime.Parse("13.03.2022"), TimeSpan.Parse("1:0:0"))
            });
            DateTime[] inputData = new DateTime[DATA_COUNT]
            {
                DateTime.Parse("06.03.2022"), DateTime.Parse("07.03.2022"), DateTime.Parse("08.03.2022"),
                DateTime.Parse("09.03.2022"), DateTime.Parse("10.03.2022"), DateTime.Parse("11.03.2022"),
                DateTime.Parse("12.03.2022"), DateTime.Parse("13.03.2022")
            };
            TimeSpan[] expectedData = new TimeSpan[DATA_COUNT]
            {
                TimeSpan.Zero, TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0")
            };
            TimeSpan[] actualData = new TimeSpan[DATA_COUNT];

            for (int i = 0; i < DATA_COUNT; i++)
                actualData[i] = timeInvestmentController.GetInvestedTimeForWeek(inputData[i]);

            for (int i = 0; i < DATA_COUNT; i++)
                Assert.AreEqual(expectedData[i], actualData[i]);
        }

        [TestMethod]
        public void GetInvestedTimeForMonthTest()
        {
            const int DATA_COUNT = 29;
            var timeInvestmentController = new TimeInvestmentController(new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Parse("01.02.2022"), TimeSpan.Parse("1:0:0")),
                new TimeInvestment(DateTime.Parse("11.02.2022"), TimeSpan.Parse("1:0:0")),
                new TimeInvestment(DateTime.Parse("15.02.2022"), TimeSpan.Parse("1:0:0"))
            });
            DateTime[] inputData = new DateTime[DATA_COUNT]
            {
                DateTime.Parse("01.02.2022"), DateTime.Parse("02.02.2022"), DateTime.Parse("03.02.2022"),
                DateTime.Parse("04.02.2022"), DateTime.Parse("05.02.2022"), DateTime.Parse("06.02.2022"),
                DateTime.Parse("07.02.2022"), DateTime.Parse("08.02.2022"), DateTime.Parse("09.02.2022"),
                DateTime.Parse("10.02.2022"), DateTime.Parse("11.02.2022"), DateTime.Parse("12.02.2022"),
                DateTime.Parse("13.02.2022"), DateTime.Parse("14.02.2022"), DateTime.Parse("15.02.2022"),
                DateTime.Parse("16.02.2022"), DateTime.Parse("17.02.2022"), DateTime.Parse("18.02.2022"),
                DateTime.Parse("19.02.2022"), DateTime.Parse("20.02.2022"), DateTime.Parse("21.02.2022"),
                DateTime.Parse("22.02.2022"), DateTime.Parse("23.02.2022"), DateTime.Parse("24.02.2022"),
                DateTime.Parse("25.02.2022"), DateTime.Parse("26.02.2022"), DateTime.Parse("27.02.2022"),
                DateTime.Parse("28.02.2022"), DateTime.Parse("01.03.2022")
            };
            TimeSpan[] expectedData = new TimeSpan[DATA_COUNT]
            {
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"), TimeSpan.Parse("3:0:0"),
                TimeSpan.Parse("3:0:0"), TimeSpan.Zero
            };
            TimeSpan[] actualData = new TimeSpan[DATA_COUNT];

            for (int i = 0; i < DATA_COUNT; i++)
                actualData[i] = timeInvestmentController.GetInvestedTimeForMonth(inputData[i]);

            for (int i = 0; i < DATA_COUNT; i++)
                Assert.AreEqual(expectedData[i], actualData[i]);
        }

        [TestMethod]
        public void GetInvestedTimeByDateRangeTest()
        {
            var timeInvestments = new List<TimeInvestment>
            {
                new TimeInvestment(DateTime.Parse("01.03.2022"), TimeSpan.Parse("02:00:00")),
                new TimeInvestment(DateTime.Parse("05.03.2022"), TimeSpan.Parse("15:00:00"))
            };
            var expectedInvestedTime = TimeSpan.Parse("17:00:00");

            var timeInvestmentController = new TimeInvestmentController(timeInvestments);
            var actualInvestedTime =
                timeInvestmentController.GetInvestedTimeByDateRange(DateTime.Parse("01.03.2022"),
                    DateTime.Parse("05.03.2022"));

            Assert.AreEqual(expectedInvestedTime, actualInvestedTime);
        }
    }
}