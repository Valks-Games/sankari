using System.Reflection;

namespace Sankari;

public static class ReflectionUtils
{
    public static Dictionary<TKey, TValue> LoadInstances<TKey, TValue>(string prefix) =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(TValue).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance).Cast<TValue>()
            .ToDictionary(x => (TKey)Enum.Parse(typeof(TKey), x.GetType().Name.Replace(prefix, "")), x => x);
}
