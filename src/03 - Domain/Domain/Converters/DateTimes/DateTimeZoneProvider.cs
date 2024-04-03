namespace Domain.Converters.DateTimes
{
    public class DateTimeZoneProvider
    {
        public static DateTime GetBrasiliaTimeZone(DateTime dateTime)
            => TimeZoneInfo.ConvertTimeFromUtc(dateTime,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
