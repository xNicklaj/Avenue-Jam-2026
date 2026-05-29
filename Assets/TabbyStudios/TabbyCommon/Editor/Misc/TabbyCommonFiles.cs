using System;
using System.Collections.Generic;

namespace TabbyStudios
{
    public static class TabbyCommonFiles
    {
        public static string separator = "/";

        public static string rootName = "TabbyCommon";
        public static string tabbyPath;
        
        public static string UxmlPath(string name) => AssetPath(name, "uxml");
        public static string IconPath(string name) => AssetPath(name, "png");
        public static string UssPath(string name) => AssetPath(name, "uss");
        
        public static Map<string, List<string>> assetFolders;
        
        public static string version = "1.9.1";

        public static List<(string type, string title, string text)> changelog = new()
        {
            ("fix", "", "Fixed dropdown menus appearing behind the dropdown selector"),
            ("fix", "", "Item icon configuration now works correctly with Unity's advanced search picker"),
            ("fix", "", "Fixed show conditions failing when making a search in the project browser"),
            ("fix", "", "Fixed show conditions not working in Unity 6.3"),
            ("fix", "", "Fixed menu item reordering issues in Unity 6.3"),
            ("fix", "", "Fixed deprecated API warnings for Unity 6.3"),
            ("fix", "1.9.1", "Fixed menus being too large for menus with one or few items"),
            ("fix", "1.9.1", "Fixed memory leak when using rounded menu corners or menu fade in animation"),
            ("fix", "1.9.1", "Fixed display conditions not working correctly in some cases"),
            ("fix", "1.9.1", "Added missing display conditions for multiple menu items"),
            ("fix", "1.9.1", "Added missing properties item to hierarchy menu"),
            
            ("improvement", "", "Improved menu behavior on multi-monitor setups"),
            ("improvement", "", "Improved performance when customizing menus"),
            ("improvement", "", "Changes no longer trigger recompilation when you have uncompiled changes"),
            ("improvement", "", "Removed profiles tab and moved it to the menus tab"),
            ("improvement", "1.9.1", "Redesigned menu UI for improved visuals and more compact menus"), 
            ("improvement", "1.9.1", "Various performance improvements"), 
            
            ("feature", "", "Added menu fade-in animation"),
            ("feature", "", "Added rounded corner menus"),
            ("feature", "", "Preferences and profiles now stored internally with ability to export and share between projects"),
            ("note", "Important:", "When updating to version 1.9, you must manually import your old profiles located under TabbyCommon/Data/Profiles"),

            ("feature", "1.9.1", "New option to limit amount of items per menu for better performance. Items can still be accessed through search"),
        };
        
        static TabbyCommonFiles()
        {
            var current = TabbyFiles.ScriptDirUnityPath(nameof(TabbyCommonFiles));
            tabbyPath = $"{current.RemoveAfterLast(rootName + separator)}{rootName}";
            
            assetFolders = new()        
            {
                {"png", new(){$"{tabbyPath}/Res/Icons"}},
                {"uss", new(){$"{tabbyPath}/Res/Uss"}},
                {"uxml", new(){$"{tabbyPath}/Res/Uxml"}},
            };
        }
        
        public static string AssetPath(string name, string extension)
        {
            if (name.IsNullOrEmpty())
                return "";
            
            foreach (var folder in assetFolders[extension])
            {
                var path = $"{folder}/{name}.{extension}";
                //if (File.Exists(path)) //save 0.1ms? almost nothing
                    return path;
            }

            throw new Exception($"{extension} does not exist: {name}");
        }

        public static void AddFolderPath(string path, string extension)
        {
            assetFolders[extension].Add(path);
        }
    }
}