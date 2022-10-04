namespace Sankari;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public static class CollectionExtensions
{
    public static string Print<T>(this IEnumerable<T> value, bool newLine = true) =>
        value != null ? string.Join(newLine ? "\n" : ", ", value) : null;

    public static string PrintFull(this object v) =>
        JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver() // ignore all Godot props
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

	/// <summary>
	/// Checks if any raycasts in a collection is colliding
	/// </summary>
	/// <param name="raycasts">Collection of raycasts to check</param>
	/// <returns>True if any ray cast is colliding, else false</returns>
	public static bool IsAnyRayCastColliding(List<RayCast2D> raycasts)
	{
		foreach (var raycast in raycasts)
			if (raycast.IsColliding())
				return true;
		return false;
	}

    private class IgnorePropsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            var ignoredProps = new Type[]
            {
                typeof(Godot.Object),
                typeof(Node),
                typeof(NodePath)
            };

            foreach (var ignoredProp in ignoredProps) 
            {
                if (ignoredProp.GetProperties().Contains(member))
                    prop.Ignored = true;

                if (prop.PropertyType == ignoredProp || prop.PropertyType.IsSubclassOf(ignoredProp))
                    prop.Ignored = true;
            }

            return prop;
        }
    }
}
