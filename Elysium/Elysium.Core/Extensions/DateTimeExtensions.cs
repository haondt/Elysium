namespace Elysium.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var now = DateTime.UtcNow;
            var timespan = now - dateTime;

            if (timespan.TotalSeconds < 1)
                return "just now";

            if (timespan.TotalSeconds < 60)
                return $"{(int)timespan.TotalSeconds}s";

            if (timespan.TotalMinutes < 60)
                return $"{(int)timespan.TotalMinutes}m";

            if (timespan.TotalHours < 24)
                return $"{(int)timespan.TotalHours}h";

            if (timespan.TotalDays < 7)
                return $"{(int)timespan.TotalDays}d";

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
