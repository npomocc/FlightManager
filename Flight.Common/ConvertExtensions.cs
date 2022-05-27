using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Flight.Common;

public static class ConvertExtensions
{
    public static Guid ToGuid(this string value)
    {
        try
        {
            return Guid.Parse(value);
        }
        catch
        {
            return Guid.Empty;
        }
    }
    public static int ToInt(this object? value, int @default = -1)
    {
        return value is null
            ? @default
            : int.TryParse(value.ToString(), out var result)
                ? result
                : @default;
    }

    public static int ToInt(this string? value, int @default = -1)
    {
        return value is null
            ? @default
            : (int.TryParse(value, out var result)
                ? result
                : @default);
    }

    public static long ToLong(this object? value, long @default = -1)
    {
        return value is null
            ? @default
            : long.TryParse(value.ToString(), out long result)
                ? result
                : @default;
    }

    public static long ToLong(this string? value, long @default = -1)
    {
        return value is null
            ? @default
            : long.TryParse(value, out long result)
                ? result
                : @default;
    }

    public static double? ToDouble(this object value)
    {
        var num = value.ToString();
        return double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f :
            double.TryParse(num, out f) ? f : null;
    }

    public static (decimal, string) ToDecimal(this string? str)
    {
        return string.IsNullOrWhiteSpace(str)
            ? (0, $"{nameof(str)} is empty")
            : !decimal.TryParse(str.Replace(".", ","), out var dec)
                ? (0, $"{nameof(str)}: '{str}' is bad")
                : (dec, string.Empty);
    }

    public static (decimal, string) ToDecimal(this object obj)
    {
        return obj.ToString().ToDecimal();
    }

    public static decimal AttributeToDecimal(this XElement? value, string attribute)
    {
        try
        {
            if (value is null) return -1;
            var attr = value.Attribute(attribute);
            var sum = attr?.Value ?? "-1";
            return Convert.ToDecimal(sum
                .Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)
                .Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public static DateTime ToDateTime(this string? src, bool isMin)
    {
        string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
            "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", "dd.MM.yyyy HH:mm:ss",
            "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", "yyyy-MM-dd'T'hh:mm:ss.fff",
            "M/d/yyyy h:mm", "M/d/yyyy h:mm", "dd.MM.yyyy",
            "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm", "yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm:ss'Z'"};
        var minDate = DateTime.Parse("01.01.1990");
        if (src is null)
            return isMin ? minDate : DateTime.MaxValue;
        if (src.Length == 8)
        {
            var day = src[..2];
            var month = src.Substring(3, 2);
            var year = src.Substring(6, 2);
            src = day + '.' + month + ".20" + year;
        }
        return DateTime.TryParseExact(src, formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime result)
            ? result
            : (isMin ? minDate : DateTime.MaxValue);
    }

    public static string GetSafeString(this byte[] arr, Encoding encoding)
    {
        try
        {
            return encoding.GetString(arr);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    public static byte[]? GetSafeBytes(this string? str, Encoding encoding)
    {
        try
        {
            return str != null ? encoding.GetBytes(str) : null;
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }
}