using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.BusinessLogic.Controller;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogicTests.Controller
{
    [TestClass()]
    public class TimeInvestmentControllerTests
    {
        [TestMethod()]
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