using System;

namespace Saturn72.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTimeStamp(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        }

        public static string NullableDateTimeToString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("G") : "";
        }

        public static bool SecondTimeSpanHasPass(this DateTime sourceUtcDateTime, int seconds)
        {
            return sourceUtcDateTime.AddSeconds(seconds) < DateTime.UtcNow;
        }

    }
}
