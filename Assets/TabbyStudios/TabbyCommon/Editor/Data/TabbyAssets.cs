using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class TabbyAssets
    {
        public static Type tabbyContextData, tabbyContextFiles;
        public static Type tabbyMenusData, tabbyMenusFiles;
        public static Type settingsPage;

        public static bool hasTabbyContext => tabbyContextFiles is not null;
        public static bool hasTabbyMenus => tabbyMenusFiles is not null;
        public static bool isTabbyContextFree => settingsPage is not null;
        
        public static string anonymousMenuPath = "_ANONYMOUS_MENU";
        public const string extraMenuPrefix = "_tabby_";
        
        static TabbyAssets()
        {
            foreach (var t in TypeCache.GetTypesDerivedFrom(typeof(FastCacheSearch)))
            {
                if (t.FullName == "TabbyStudios.TabbyContextData")
                    tabbyContextData = t;
                else if (t.FullName == "TabbyStudios.TabbyMenusData")
                    tabbyMenusData = t;
                else if (t.FullName == "TabbyStudios.TabbyContextFiles")
                    tabbyContextFiles = t;
                else if (t.FullName == "TabbyStudios.TabbyMenusFiles")
                    tabbyMenusFiles = t;
                else if (t.FullName == "TabbyStudios.SettingsPage")
                    settingsPage = t; 
            }
        }

        public static void CallOnAssetData(string method, params object[] args)
        {
            CallOnAssetData<object>(method, args);
        }
        
        public static List<T> CallOnAssetData<T>(string method, params object[] args)
        {
            var list = new List<T>();
            
            if (hasTabbyContext)
                list.Add((T)tabbyContextData.InvokeStaticMethod(method, args));
            
            if (hasTabbyMenus)
                list.Add((T)tabbyMenusData.InvokeStaticMethod(method, args));

            return list;
        }
        
        public static void DisposeSettingsPage()
        {
            UnityWindows.GetWindow(settingsPage).InvokeMethod("Dispose");
        }
        
        public static List<T> GetAssetDataList<T>(string method, params object[] args)
        {
            var list = new List<T>();
            
            if (hasTabbyContext)
                list.AddRange((List<T>)tabbyContextData.InvokeStaticMethod(method, args));
            
            if (hasTabbyMenus)
                list.AddRange((List<T>)tabbyMenusData.InvokeStaticMethod(method, args));

            return list;
        }

        public static void AddToSettingsPage(VisualElement uxml)
        {
            CallOnAssetData(nameof(AddToSettingsPage), uxml);
        }
        
        public static string MapToUnityPath(string path)
        {
            if (hasTabbyContext)
            {
                return tabbyContextData.InvokeStaticMethod<string>("MapToUnityPath", path);
            }

            return path;
        }
    }
    
}