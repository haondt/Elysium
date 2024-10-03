namespace Elysium.ActivityPub.Extensions
{
    public static class DateTimeExtensions
    {
        public static string AsXsdString(this DateTime dateTime)
        {
            return dateTime.ToString("o");
        }
    }
}
