using System.Collections.Generic;
using UnityEditor;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public static class ExtraData
    {
        public static List<string> topLevelPaths = new();
        
        static ExtraData()
        {
            if (TabbyAssets.tabbyMenusFiles is not null)
            {
                topLevelPaths.AddRange(TabbyAssets.tabbyMenusData.InvokeStaticMethod<List<string>>("TopLevelMenus"));
            }
            if (TabbyAssets.tabbyContextData is not null)
            {
                topLevelPaths.AddRange(TabbyAssets.tabbyContextData.InvokeStaticMethod<List<string>>("TopLevelMenus"));
            }
        }

        public static List<string> nonCustomizablePaths = new()
        {
            "Window/Panels",
            "File/Open Recent Scene",
        };
        
        public static List<ItemData> GetItemData(string path)
        {
            if (TabbyAssets.tabbyContextData?.InvokeStaticMethod("GetItemData", path) is List<ItemData> result)
                return result;

            return null;
        }

        public static List<string> GetExtraPaths()
        {
            if (TabbyAssets.tabbyContextData?.InvokeStaticMethod("TopLevelMenus") is List<string> result)
                return result;

            return new();
        }
    }
}