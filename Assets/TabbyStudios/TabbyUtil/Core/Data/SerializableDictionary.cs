using System;
using System.Collections.Generic;
using UnityEngine;

namespace TabbyStudios
{
    [Serializable]
    public class SerializableDictionary<TKey> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new ();
        [SerializeField] private List<string> strings = new();

        [NonSerialized] public Dictionary<TKey, object> dict = new();

        public object this[TKey key]
        {
            get => dict[key];
            set => dict[key] = value;
        }
    
        public virtual void OnBeforeSerialize()
        {
            keys = new();
            strings = new();
            
            foreach (var pair in dict)
            {
                keys.Add(pair.Key);
                var type = pair.Value?.GetType();
                var primitive = type is not null && (type.IsPrimitive || type == typeof(string));
                strings.Add(primitive ? pair.Value?.ToString() ?? "" : JsonUtility.ToJson(pair.Value));
            }
        }

        public virtual void OnAfterDeserialize()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = TryParse(strings[i]);
            }
        }

        private object TryParse(string s)
        {
            if (bool.TryParse(s, out bool boolResult))
            
                return boolResult;

            if (float.TryParse(s, out float floatResult))
                return floatResult;
            
            return s;
        }
    
    }
}