using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TabbyStudios
{
    public class MenuCalls
    {
        public static Map<string, Map<string, Action<Object[]>>> calls => 
            TabbyAssets.hasTabbyContext ? TabbyAssets.tabbyContextData.InvokeStaticMethod<Map<string, Map<string, Action<Object[]>>>>("GetCalls") : new();
        
        public static bool TryCall(string path)
        {
            try
            {
                var root = path.RemoveAfterFirst("/");
                if (!calls.ContainsKey(root)) return false;
                calls[root][path.ToLower()](Selection.objects);
                return true;
            }
            catch
            {
                Debug.Log("Some context menus may not be fully finalized for this version of Unity... More updates coming soon");
                throw;
            }
        }
    }
}