using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TabbyStudios
{
    public static class TabbyCommonFiles
    {
        public static string separator = "/";

        public static string rootName = "TabbyCommon";
        public static string tabbyPath;
        public static string profilesFolder => $"{tabbyPath}/Data/Profiles";
        public static List<string> profiles => TabbyFiles.SafeGetFiles(profilesFolder, "*.json").Select(Path.GetFileNameWithoutExtension).ToList();
        
        public static string ProfilePath(string profile) => $"{profilesFolder}/{profile}.json";
        public static string UxmlPath(string name) => AssetPath(name, "uxml");
        public static string IconPath(string name) => AssetPath(name, "png");
        public static string UssPath(string name) => AssetPath(name, "uss");
        
        public static Map<string, List<string>> assetFolders;
        
        public static string version = "1.8.3";

        public static List<(string type, string title, string text)> changelog = new()
        {
            ("fix", "", "Fixed menus not opening when hovering over an item after deleting all text from the search bar"),
            ("fix", "", "Fixed edits being saved to the wrong profile after resetting all settings to default"),
            ("fix", "", "Fixed menus occasionally displaying as the wrong size for a frame when opening"),
            ("fix", "", "Fixed exception when opening menus in the left column of the project browser in some cases"),
            ("fix", "1.8.1", "Fixed issue with some items in the hierarchy being broken after 1.8.0"),
            ("fix", "1.8.1", "Fixed modifier key to bring up default menu not working with compatibility mode"),
            ("fix", "1.8.1", "Fixed exception when trying to open menus with no project browser open"),
            ("fix", "1.8.2", "Fixed issue with menu items that have the same name as its parent menu"),
            ("fix", "1.8.2", "Now correctly able to select menu items with enter key"),
            ("fix", "1.8.2", "Fixed Open Asset In Context option not working correctly with compatibility mode"),
            ("fix", "1.8.3", "Selecting menu items with enter key now working in search mode"),
            ("fix", "1.8.3", "Create Empty parent now correctly works with multiple game objects"),
            ("fix", "1.8.3", "Included some fixes that were accidentally missing from 1.8.2"),

            ("improvement", "", "Settings page now retains its last position when reopened"),
            ("improvement", "", "Minor performance improvements when opening menus for the first time after a domain reload"),
            ("improvement", "1.8.2", "Improved performance for hierarchy context menus"),
            ("improvement", "1.8.2", "Reduced impact on domain reload times"),
            ("improvement", "1.8.2", "Multiple other minor performance improvements"),
            ("improvement", "1.8.3", "Improved menu responsiveness inside the settings window"),

            ("feature", "", "Added new option to save formatted JSON"),
            ("feature", "", "Added new option to hide the search bar by default and automatically show it when typing"),
            ("feature", "", "Added new option to automatically delete items created by [MenuItem] when the attribute is removed"),
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
                if (File.Exists(path))
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