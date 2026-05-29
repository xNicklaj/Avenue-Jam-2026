using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class Config
    {
        public static Config instance { get; }

        public string id;
        private Map<string, List<object>> calls = new();
        private string GetKey(string key) => $"{id}_{key}";

        static Config()
        {
            var config = "__tabby_preferences";
            if (!EditorPrefs.HasKey(config))
            {
                EditorPrefs.SetString(config, "tabbyPreferences");
            }
            
            instance = new Config();
        }

        public Config(string id)
        {
            this.id = id;
            Init();
        }
        
        public Config()
        {
            id = EditorPrefs.GetString("__tabby_preferences");
            Init();
        }

        private void Init()
        {
            foreach (var pair in GetDefaults())
            {
                var key = GetKey(pair.Key);
                if (!EditorPrefs.HasKey(key))
                {
                    var val = pair.Value;
                    if (val is bool b) EditorPrefs.SetBool(key, b);
                    else if (val is string s) EditorPrefs.SetString(key, s);
                    else if (val is int i) EditorPrefs.SetInt(key, i);
                    else if (val is float f) EditorPrefs.SetFloat(key, f);
                    else throw new Exception($"Key {key} had value {val ?? "<null>"} unsupported type {val?.GetType()}");
                }
            }
        }
        
        public void Export(string folder)
        {
            var defaultMap = GetDefaults();
            var map = new ConfigDictionary();

            foreach (var pair in defaultMap)
            {
                var k = GetKey(pair.Key);
                var val = pair.Value;
                if (val is bool) map.Set(pair.Key, EditorPrefs.GetBool(k));
                else if (val is string) map.Set(pair.Key, EditorPrefs.GetString(k));
                else if (val is int) map.Set(pair.Key, EditorPrefs.GetInt(k));
                else if (val is float) map.Set(pair.Key, EditorPrefs.GetFloat(k));
            }

            var path = $"{folder}/{id}.json";
            File.WriteAllText(path, SafeJson.ToJson(map));
            Debug.Log($"Preferences file exported to {path}");
        }

        public void Import(string path)
        {
            var map = SafeJson.FromPath<ConfigDictionary>(path);
            foreach (var (key, val) in map)
            {
                SetObject(key, val);
            }

            Debug.Log($"Imported preferences file from {path}");
        }
        
        public void ResetDefaults()
        {
            foreach (var pair in GetDefaults())
            {
                SetObject(pair.Key, pair.Value);
            }
        }

        public Map<string, object> GetDefaults()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("Tabby"));
            var types = assemblies.SelectMany(a => a.GetTypes()).ToList();
            var fields = types.SelectMany(t => t.GetFieldInfos()).Where(f => f.GetCustomAttributes(typeof(SettingAttribute)).Any()).ToList();
            var tuples = fields.Select(f => (f, f.GetCustomAttributes<SettingAttribute>().First())).ToList();
            var result = new Map<string, object>(tuples.Select(t => new KeyValuePair<string, object>(t.f.Name, t.Item2.defaultValue)));
            return result;
        }

        public bool GetBool(string key)
        {
            return EditorPrefs.GetBool(GetKey(key));
        }

        public string GetString(string key)
        {
            return EditorPrefs.GetString(GetKey(key));
        }

        public int GetInt(string key)
        {
            return EditorPrefs.GetInt(GetKey(key));
        }

        public float GetFloat(string key)
        {
            return EditorPrefs.GetFloat(GetKey(key));
        }

        public T Get<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(bool)) return (T)(object)GetBool(key);
            if (type == typeof(string)) return (T)(object)GetString(key);
            if (type == typeof(int)) return (T)(object)GetInt(key);
            if (type == typeof(float)) return (T)(object)GetFloat(key);
            throw new Exception($"Unsupported type {type.Name} on get {key}");
        }
        
        public void Set(string key, bool value)
        {
            var k = GetKey(key);
            //Assert.IsTrue(EditorPrefs.HasKey(k));
            EditorPrefs.SetBool(k, value);
            InvokeCalls(key, value);
        }

        public void Set(string key, string value)
        {
            var k = GetKey(key);
            //Assert.IsTrue(EditorPrefs.HasKey(k));
            EditorPrefs.SetString(k, value);
            InvokeCalls(key, value);
        }

        public void Set(string key, int value)
        {
            var k = GetKey(key);
            //Assert.IsTrue(EditorPrefs.HasKey(k));
            EditorPrefs.SetInt(k, value);
            InvokeCalls(key, value);
        }

        public void Set(string key, float value)
        {
            var k = GetKey(key);
            //Assert.IsTrue(EditorPrefs.HasKey(k));
            EditorPrefs.SetFloat(k, value);
            InvokeCalls(key, value);
        }

        public void SetObject(string key, object value)
        {
            if (value is bool b) Set(key, b);
            else if (value is string s) Set(key, s);
            else if (value is int i) Set(key, i);
            else if (value is float f) Set(key, f);
            else throw new Exception($"Key {key} had value {value ?? "<null>"} unsupported type {value?.GetType()}");
        }

        public void Delete(string key)
        {
            EditorPrefs.DeleteKey(GetKey(key));
            calls.Remove(key);
        }

        private void InvokeCalls(string key, object value)
        {
            if (calls.TryGetValue(key, out var list))
            {
                foreach (var call in list)
                {
                    call.InvokeMethod("Invoke", value);
                }
            }
        }

        public T Subscribe<T>(string key, Action<T> call)
        {
            var list = calls.GetOrInsert(key, () => new());
            list.Add(call);
            return Get<T>(key);
        }

        public void Unsubscribe<T>(string key, Action<T> call)
        {
            if (calls.TryGetValue(key, out var list))
            {
                list.Remove(call);
            }
        }
        
        public void Clear()
        {
            foreach (var pair in GetDefaults())
            {
                EditorPrefs.DeleteKey(GetKey(pair.Key));
            }
        }

        public bool HasKey(string key)
        {
            return EditorPrefs.HasKey(GetKey(key));
        }

        public void Flip(string key)
        {
            Set(key, !GetBool(key));
        }

        public int TotalCalls()
        {
            return calls.Sum(pair => pair.Value.Count);
        }
    }
}