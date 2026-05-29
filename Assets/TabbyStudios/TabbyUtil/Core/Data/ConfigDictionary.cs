using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabbyStudios
{

    [Serializable]
    public class ConfigDictionary : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<string, object>>
    {
        [Serializable]
        private class StringEntry
        {
            public string key;
            public string value;
        }

        [Serializable]
        private class BoolEntry
        {
            public string key;
            public bool value;
        }

        [Serializable]
        private class IntEntry
        {
            public string key;
            public int value;
        }

        [Serializable]
        private class FloatEntry
        {
            public string key;
            public float value;
        }

        [SerializeField] private List<StringEntry> stringEntries = new List<StringEntry>();
        [SerializeField] private List<BoolEntry> boolEntries = new List<BoolEntry>();
        [SerializeField] private List<IntEntry> intEntries = new List<IntEntry>();
        [SerializeField] private List<FloatEntry> floatEntries = new List<FloatEntry>();

        // Single runtime dictionary for all values
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        // Set method - works with any supported type
        public void Set(string key, object value)
        {
            if (value is string || value is bool || value is int || value is float)
            {
                dict[key] = value;
            }
            else
            {
                Debug.LogWarning($"ConfigDictionary: Unsupported type {value?.GetType().Name}");
            }
        }

        // Generic Get with default value
        public T Get<T>(string key, T defaultValue = default)
        {
            if (dict.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            return defaultValue;
        }

        // Convenience Get methods
        public string GetString(string key, string defaultValue = "")
        {
            return Get(key, defaultValue);
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return Get(key, defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return Get(key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            return Get(key, defaultValue);
        }

        // Check if key exists
        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        // Remove a key
        public bool Remove(string key)
        {
            return dict.Remove(key);
        }

        // Clear all entries
        public void Clear()
        {
            dict.Clear();
        }

        // Get count of all entries
        public int Count => dict.Count;

        // Prepare for serialization - convert dict to lists
        public void OnBeforeSerialize()
        {
            stringEntries.Clear();
            boolEntries.Clear();
            intEntries.Clear();
            floatEntries.Clear();

            foreach (var kvp in dict)
            {
                switch (kvp.Value)
                {
                    case string strValue:
                        stringEntries.Add(new StringEntry { key = kvp.Key, value = strValue });
                        break;
                    case bool boolValue:
                        boolEntries.Add(new BoolEntry { key = kvp.Key, value = boolValue });
                        break;
                    case int intValue:
                        intEntries.Add(new IntEntry { key = kvp.Key, value = intValue });
                        break;
                    case float floatValue:
                        floatEntries.Add(new FloatEntry { key = kvp.Key, value = floatValue });
                        break;
                }
            }
        }

        // Rebuild dictionary from lists after deserialization
        public void OnAfterDeserialize()
        {
            dict.Clear();

            foreach (var entry in stringEntries)
                dict[entry.key] = entry.value;

            foreach (var entry in boolEntries)
                dict[entry.key] = entry.value;

            foreach (var entry in intEntries)
                dict[entry.key] = entry.value;

            foreach (var entry in floatEntries)
                dict[entry.key] = entry.value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}