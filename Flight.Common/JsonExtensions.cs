using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Flight.Common;

public static class JsonExtensions
{
    public static string? SafeSerialize<T>(this T obj)
    {
        if (obj is null)
            return null;
        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static T? SafeDeserialize<T>(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return default;
        try
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        catch
        {
            return default;
        }
    }

    public static (T, Exception) SafeDeserialize2<T>(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return (default, null)!;
        try
        {
            return (JsonConvert.DeserializeObject<T>(str), null)!;
        }
        catch (Exception exception)
        {
            return (default, exception)!;
        }
    }

    public static T? SafeFileDeserialize<T>(this string fileName)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
        }
        catch
        {
            return default;
        }
    }
    public static (T, Exception) SafeFileDeserialize2<T>(this string fileName)
    {
        try
        {
            return (JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName)), null)!;
        }
        catch (Exception exception)
        {
            return (default, exception)!;
        }
    }

    public static (T, Exception) SafeDeserialize<T>(this JSchema schema, string str)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str)) return default;
            var validatingReader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(str)))
            {
                Schema = schema
            };
            var serializer = new JsonSerializer();
            var obj = serializer.Deserialize<T>(validatingReader);
            return (obj, null)!;
        }
        catch (Exception exception)
        {
            return (default, exception)!;
        }
    }
    public static (T, Exception) SafeGetJson<T>(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return (default, null)!;
        try
        {
            var (obj, exception) = SafeDeserialize<T>(GetJsonSchema<T>(), str);
            return (obj, exception);
        }
        catch (Exception exception)
        {
            return (default, exception)!;
        }
    }
    public static JSchema GetJsonSchema<T>() => new JSchemaGenerator().Generate(typeof(T));

    public static (bool, Exception) Validate(this string str, string strSchema)
    {
        try
        {
            var schema = JSchema.Parse(strSchema);
            var json = JObject.Parse(str);
            return (json.IsValid(schema), null)!;
        }
        catch (Exception exception)
        {
            return (default, exception);
        }
    }
}