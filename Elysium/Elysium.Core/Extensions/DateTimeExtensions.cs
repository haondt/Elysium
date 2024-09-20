using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var now = DateTime.UtcNow;
            var timespan = now - dateTime.ToUniversalTime();

            if (timespan.TotalSeconds < 1)
                return "just now";

            if (timespan.TotalSeconds < 60)
                return $"{timespan.Seconds}s";

            if (timespan.TotalMinutes < 60)
                return $"{timespan.Minutes}m";

            if (timespan.TotalHours < 24)
                return $"{timespan.Hours}h";

            if (timespan.TotalDays < 7)
                return $"{timespan.Days}d";

            if (timespan.TotalDays < 30)
            {
                int weeks = (int)(timespan.TotalDays / 7);
                return $"{weeks}w";
            }

            if (timespan.TotalDays < 365)
            {
                int months = (int)(timespan.TotalDays / 30);
                return $"{months}mo";
            }

            int years = (int)(timespan.TotalDays / 365);
            return $"{years}y";
        }
    }
}
