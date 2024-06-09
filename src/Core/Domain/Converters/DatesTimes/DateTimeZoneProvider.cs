namespace Domain.Converters.DatesTimes
{
    public class DateTimeZoneProvider
    {
        public static DateTime GetBrasiliaDateTimeZone()
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
