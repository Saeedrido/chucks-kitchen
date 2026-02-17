namespace ChuksKitchen.Infrastructure.Helpers;

public static class DateTimeHelper
{
    public static bool IsExpired(DateTime expiry)
    {
        return DateTime.UtcNow > expiry;
    }

    public static bool IsWithinLastMinutes(DateTime? dateTime, int minutes)
    {
        if (!dateTime.HasValue)
            return false;

        var timeDifference = DateTime.UtcNow - dateTime.Value;
        return timeDifference.TotalMinutes <= minutes;
    }

    public static string GetNigerianTime()
    {
        var nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("WAT");
        var nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nigeriaTimeZone);
        return nigeriaTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
