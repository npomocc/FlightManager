using System.Text;

namespace Flight.Common;

public static class ExceptionsExtensions
{
    public static string Trace(this Exception exception, bool includeStack = false)
    {
        var stringBuilder = new StringBuilder();
        var prev = exception;
        stringBuilder.Append($"Type: [{exception.GetType()}]. ");
        stringBuilder.Append($"Message: {exception.Message}, target: {exception.TargetSite}, source: {exception.Source}");
        if (includeStack)
            stringBuilder.Append($", {exception.StackTrace}.");
        while (exception != null)
        {
            if (prev != exception)
            {
                stringBuilder.Append($"Type: [{exception.GetType()}]. ");
                stringBuilder.Append($"Message: {exception.Message}, target: {exception.TargetSite}, source: {exception.Source}");
                if (includeStack)
                    stringBuilder.Append($", {exception.StackTrace}.");
                prev = exception;
            }
            exception = exception.InnerException;
        }
        return stringBuilder.ToString();
    }
}