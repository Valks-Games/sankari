namespace Sankari;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

class IgnorePropsResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        if (prop.PropertyType == typeof(Godot.Object))
        {
            prop.Ignored = true;
        }
        return prop;
    }
}

public static class CollectionExtensions
{
    public static string Print<T>(this IEnumerable<T> value, bool newLine = true) =>
        value != null ? string.Join(newLine ? "\n" : ", ", value) : null;

    public static string PrintFull(this object v) =>
        JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings 
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver()
        });
        

    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (var element in value)
            action(element);
    }

    public static bool Duplicate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (!dict.ContainsKey(key))
            return false;

        Logger.LogWarning($"'{caller}' tried to add duplicate key '{key}' to dictionary\n" +
                            $"   at {path} line:{lineNumber}");

        return true;

    }

    public static bool DoesNotHave<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (dict.ContainsKey(key))
            return false;

        Logger.LogWarning($"'{caller}' tried to access non-existent key '{key}' from dictionary\n" +
                            $"   at {path} line:{lineNumber}");
        return true;

    }
}
