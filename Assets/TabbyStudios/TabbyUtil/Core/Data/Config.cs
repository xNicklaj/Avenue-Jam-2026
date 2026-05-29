using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Compilation;
using UnityEngine;

namespace TabbyStudios
 {
     [Serializable]
     public class Config : ISerializationCallbackReceiver
     {
         [SerializeField] 
         private ConfigDictionary values;
         private Dictionary<string, ConfigCall> calls = new();
         private static int saveQueue;
         
         private static string path = TabbyFiles.configPath;
         private static Config _instance;
         public static Config instance
         {
             get
             {
                 if (_instance is null)
                 {
                     _instance = SafeJson.FromPath<Config>(path);
                     Save();
                 }
                     
                 
                 return _instance;
             }
         }
         
         public static void SetSetting(string name, object value)
         {
             if(!HasSetting(name))
                 InsertSetting(name,value);
             
             instance.values[name] = value;
             Save();
             instance.calls[name].Invoke(value);
         }

         public static void SetSettingWithoutSave(string name, object value)
         {
             if (!HasSetting(name))
                 InsertSetting(name, value);

             instance.values[name] = value;
             instance.calls[name].Invoke(value);
         }

         public static void SetSettingDelayed(string name, object value)
         {
             if (!HasSetting(name))
                 InsertSetting(name, value);

             instance.values[name] = value;
             DelayedSave();
             instance.calls[name].Invoke(value);
         }
         
         private static void InsertSetting(string name, object value)
         {
             instance.values[name] = value;
             instance.calls[name] = new ConfigCall();
         }
         
         public static T GetSetting<T>(string name)
         {
             try
             {
                 if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                 {
                     return (T)instance.values[name];
                 }
                 
                 return SafeJson.FromJson<T>(instance.values[name].ToString());
             }
             
             catch (InvalidCastException)
             {
                 return (T)(object)Convert.ToInt32(instance.values[name]);
             }
         }
         
         public static T GetSettingOrDefault<T>(string name)
         {
             return HasSetting(name) ? GetSetting<T>(name) : default;
         }

         public static bool HasSetting(string name)
         {
             return instance.values.dict.ContainsKey(name);
         }
         
         public static void Flip(string name)
         {
             SetSetting(name, !GetSetting<bool>(name));
         }
         
         public static T Subscribe<T>(string name, Action<T> call)
         {
             instance.calls[name].Add(call);
             return GetSetting<T>(name);
         }
         
         public static T SubscribeAndCall<T>(string name, Action<T> call)
         {
             var setting = Subscribe(name, call);
             call(setting);
             return setting;
         }
         
         public static T Subscribe<T>(string name, Action<T> call, bool callImmediately)
         {
             return callImmediately ? SubscribeAndCall(name, call) : Subscribe(name, call);
         }
         
         public static void Unsubscribe<T>(string name, Action<T> call)
         {
             instance.calls[name].Remove(call);
         }
         
         public static void Save()
         {
             TabbyFiles.SafeWrite(path, SafeJson.ToJson(instance));
         }
         
         public static void DelayedSave()
         {
             saveQueue++;
             RunAsync.Fire(1000, () =>
             {
                 if (saveQueue == 1)
                 {
                     saveQueue = 0;
                     Save();
                 }
                 else
                 {
                     saveQueue--;
                 }
             });
         }
         
         public static void NewConfig()
         {
             TabbyFiles.DeleteFile(path);
             
             var newConfig = SafeJson.FromJson<Config>("");
             GC.Collect();
             RemoveGarbage();
             newConfig.calls = instance.calls;
             _instance = newConfig;
             
             var entries = new List<KeyValuePair<string, object>>(newConfig.values.dict);
             foreach (var value in entries)
             {
                 try
                 {
                     SetSetting(value.Key, value.Value); //trigger subscribers
                 }
                 catch
                 {
                     CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.None); //probably leaked calls, reload domain to get rid of them
                 }
             }
             
             Save();
         }

         public static void NewConfigDontNotify()
         {
             TabbyFiles.DeleteFile(path);
             _instance = SafeJson.FromJson<Config>("");
             Save();
         }

         public static void NotifyAll()
         {
             foreach (var pair in instance.calls)
             {
                 var call = pair.Value;
                 var name = pair.Key;
                 call.Invoke(GetSetting<object>(name));
             }
         }

         public void OnBeforeSerialize()
         {
             
         }
         
         public void OnAfterDeserialize()
         {
             if (values is null)
             {
                 values = new ConfigDictionary();
                 values.Init();
             }
             calls = InitCalls();
         }
         
         public static void RemoveGarbage()
         {
             instance.calls.ForEach(c => c.Value.RemoveGarbage());
         }
         
         private Dictionary<string, ConfigCall> InitCalls()
         {
             return new(values.dict.Keys.Select(name => new KeyValuePair<string, ConfigCall>(name, new ConfigCall())));
         }
         
         public static int TotalStorage()
         {
             RemoveGarbage();
             return instance.calls.Sum(kv => kv.Value.Count());
         }

         public static string GetString(string name) => GetSetting<string>(name);
         public static bool GetBool(string name) => GetSetting<bool>(name);
         public static int GetInt(string name) => GetSetting<int>(name);
         public static float GetFloat(string name) => GetSetting<float>(name);
     }
     
 }