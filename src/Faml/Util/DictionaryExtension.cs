using System.Collections.Generic;

namespace Faml.Util
{
    public static class DictionaryExtension
    {
        public static TValue GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue: class
        {
            if (! dictionary.TryGetValue(key, out TValue value))
                return null;
            return value;
        }
    }
}