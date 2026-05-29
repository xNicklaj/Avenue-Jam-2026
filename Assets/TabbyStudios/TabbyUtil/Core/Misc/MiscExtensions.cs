using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TabbyStudios
{
    public static class Cast
    {
        public static T As<T>(this object obj)
        {
            return (T)obj;
        }
    }
    
    public static class WeakReferenceExtensions
    {
        public static T Target<T>(this WeakReference<T> wk) where T : class
        {
            wk.TryGetTarget(out T target);
            return target;
        }
    }

    public static class ColorExtensions
    {
        public static Color SetR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }
        
        public static Color SetG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }
        
        public static Color SetB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }
        
        public static Color SetA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
    }
    
    public static class ActionExtensions
    {
        public static bool IsLambda(this Action action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1>(this Action<T1> action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1,T2>(this Action<T1,T2> action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1,T2,T3>(this Action<T1,T2,T3> action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1>(this Func<T1> action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1,T2>(this Func<T1,T2> action)
        {
            return InternalIsLambda(action.Method);
        }
    
        public static bool IsLambda<T1,T2,T3>(this Func<T1,T2,T3> action)
        {
            return InternalIsLambda(action.Method);
        }

        private static bool InternalIsLambda(MethodInfo methodInfo)
        {
            bool isCompilerGenerated = methodInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            bool hasCompilerGeneratedName = methodInfo.Name.Contains("<") && methodInfo.Name.Contains(">");
            return isCompilerGenerated || hasCompilerGeneratedName;
        }
    }
    
    public static class DictionaryExtensions 
    {
        public static void Remove<K, V>(this Dictionary<K, V> dict, Func<K, V, bool> pred)
        {
            var keysToRemove = dict.Where(kv => pred(kv.Key, kv.Value)).Select(kv => kv.Key);
            foreach (var key in keysToRemove)
            {
                dict.Remove(key);
            }
        }
        
        public static T SelectValue<K, V, T>(this Dictionary<K, V> dict, K key, Func<V, T> func)
        {
            return func(dict[key]);
        }
        
        public static T SelectValueOrDefault<K, V, T>(this Dictionary<K, V> dict, K key, Func<V, T> func, T _default)
        {
            return dict.ContainsKey(key) ? SelectValue(dict, key, func) : _default;
        }

        public static V SafeGet<K, V>(this Dictionary<K, V> dict, K key) where V : new()
        {
            if (!dict.ContainsKey(key))
                dict[key] = new V();
            
            return dict[key];
        }
    }
}