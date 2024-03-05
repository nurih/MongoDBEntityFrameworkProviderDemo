// using Microsoft.EntityFrameworkCore;
public static class DateTimeExtension
{
    public static string ISO8601(this DateTime dateTime)
    => dateTime.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
}
