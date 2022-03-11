using System;

namespace TimeTracker.BusinessLogic.Extensions
{
    public static class TimeSpanExtension
    {
        public static string ToStringWithoutDays(this TimeSpan timeSpan)
        {
            var seconds = timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : $"0{timeSpan.Seconds}";
            var minutes = timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : $"0{timeSpan.Minutes}";
            var hours = timeSpan.Hours + timeSpan.Days * 24 > 9 ? (timeSpan.Hours + timeSpan.Days * 24).ToString() : $"0{timeSpan.Hours}";

            return $"{hours}:{minutes}:{seconds}";
        }
    }
}
