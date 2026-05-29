using System;
using UnityEngine;

namespace TabbyStudios
{
    public class SafeJson
    {
        public static T FromPath<T>(string path) where T : class, new()
        {
            var json = TabbyFiles.SafeRead(path);
            var t = FromJson<T>(json);
            if (t is null)
            {
                var nt = new T();
                TabbyFiles.SafeWrite(path,JsonUtility.ToJson(nt));
                return nt;
            }

            return t;
        }
        
        public static void WriteJson(object obj, string path)
        {
            TabbyFiles.SafeWrite(path, ToJson(obj));
        }
        
        public static T FromJson<T>(string json)
        {
            if (json.IsNullOrEmpty())
            {
                var t = Activator.CreateInstance<T>();
                if(t is ISerializationCallbackReceiver r) 
                    r.OnAfterDeserialize();
                return t;
            }
            
            return JsonUtility.FromJson<T>(json);
        }
        
        public static string ToJson(object obj)
        {
            return ToJson(obj, prettyPrint:Config.GetSetting<bool>("jsonPrettyPrint"));
        }
        
        public static string ToJson(object obj, bool prettyPrint)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }
        
    }
}