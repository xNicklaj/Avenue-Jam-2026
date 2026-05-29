using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TabbyStudios
{
    public static class AssetCache
    {
        private static Dictionary<string, Object> assets = new();

        public static T Load<T>(string path) where T : Object
        {
            if (assets.ContainsKey(path))
                return (T)assets[path];
        
            var loaded = AssetDatabase.LoadAssetAtPath<T>(path);
            assets[path] = loaded;
            return loaded;
        }
        
        public static VisualElement LoadXml(string name)
        {
            return Load<VisualTreeAsset>(TabbyCommonFiles.UxmlPath(name)).Create();
        }
        
        public static StyleSheet LoadUss(string name)
        {
            return Load<StyleSheet>(TabbyCommonFiles.UssPath(name));
        }
        
        public static VisualTreeAsset LoadVisualTree(string path)
        {
            return Load<VisualTreeAsset>(path);
        }

    }
}