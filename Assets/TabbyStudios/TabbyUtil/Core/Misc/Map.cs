using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class Map<K, V> : IEnumerable<KeyValuePair<K, V>>
    {
        
        private Dictionary<K, V> dict;
        public Dictionary<K, V>.KeyCollection Keys => dict.Keys;
        public Dictionary<K, V>.ValueCollection Values => dict.Values;

        public int Count => dict.Count;

        public void Add(K key, V value) => dict.Add(key, value);
        public bool ContainsKey(K key) => dict.ContainsKey(key);
        public bool Remove(K key) => dict.Remove(key);
        public void Clear() => dict.Clear();
        public bool TryGetValue(K key, out V value) => dict.TryGetValue(key, out value);
        public Func<V> defaultGenerator = () => default;

        public Map()
        {
            dict = new Dictionary<K, V>();
        }

        public Map(Dictionary<K, V> dictionary)
        {
            dict = new Dictionary<K, V>(dictionary);
        }

        public Map(IEnumerable<(K, V)> list)
        {
            dict = new Dictionary<K, V>(list.Select(kv => new KeyValuePair<K, V>(kv.Item1, kv.Item2)));
        }

        public Map(IEnumerable<KeyValuePair<K, V>> list)
        {
            dict = new Dictionary<K, V>(list);
        }

        public V this[K key]
        {
            get => dict[key];
            set => dict[key] = value;
        }

        public V GetOrInsert(K key, V onNotFound)
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }

            dict[key] = onNotFound;
            return onNotFound;
        }

        public V GetOrInsert(K key)
        {
            return GetOrInsert(key, defaultGenerator);
        }

        public V GetOrInsert(K key, Func<V> onNotFoundGenerator)
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }

            V defaultValue = onNotFoundGenerator();
            dict[key] = defaultValue;
            return defaultValue;
        }
        
        public V GetOrDefault(K key)
        {
            return dict.GetValueOrDefault(key, defaultGenerator());
        }

        public V GetOrDefault(K key, V _default)
        {
            return dict.GetValueOrDefault(key, _default);
        }
        
        public void Remove(Func<K, V, bool> pred)
        {
            dict.Remove(pred);
        }
        
        public static Map<K, V> CombineMaps(params Map<K, V>[] maps)
        {
            var result = new Map<K, V>();

            foreach (var map in maps)
            {
                foreach (var kv in map)
                {
                    if (result.TryGetValue(kv.Key, out var existingValue))
                    {
                        if (!EqualityComparer<V>.Default.Equals(existingValue, kv.Value))
                        {
                            throw new InvalidOperationException($"Conflicting values for key '{kv.Key}': '{existingValue}' vs '{kv.Value}'");
                        }
                    }
                    else
                    {
                        result[kv.Key] = kv.Value;
                    }
                }
            }

            return result;
        }

        public Map<K, V> Combine(params Map<K, V>[] maps)
        {
            return CombineMaps(maps.Append(this).ToArray());
        }
        
        public Dictionary<V, K> Inverse()
        {
            var inverse = new Map<V, K>();
            
            foreach (var kvp in dict)
            {
                if (inverse.ContainsKey(kvp.Value))
                    throw new InvalidOperationException($"Duplicate value '{kvp.Value}' found. Map is not injective.");
                inverse[kvp.Value] = kvp.Key;
            }

            return inverse;
        }

        public static implicit operator Dictionary<K, V>(Map<K, V> map)
        {
            return map.dict;
        }

        public static implicit operator Map<K, V>(Dictionary<K, V> dict)
        {
            return new Map<K, V>(dict);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}