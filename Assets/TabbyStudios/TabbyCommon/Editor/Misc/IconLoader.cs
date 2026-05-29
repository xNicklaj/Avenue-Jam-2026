using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class IconLoader : EditorWindow
    {
        public static Dictionary<string, string> defaultIconNames;
        private static Dictionary<string, Texture2D> defaultIcons;

        public static Texture2D gradient;
    
        static IconLoader()
        {
            defaultIconNames = BuildDefaultNames();
            defaultIcons = new(defaultIconNames.Values.Distinct().Select(name => new KeyValuePair<string,Texture2D>(name,LoadIcon(name))));
            gradient = LoadIcon("gradient");
        }
    
        public static Texture2D GetIcon(string name)
        {
            var icon = defaultIcons.GetValueOrDefault(name, null);
            if (icon is not null)
                return icon;

            Texture2D loadedIcon = null;
            try
            {
                loadedIcon = LoadIcon(name);
            }
            catch
            {
                Debug.Log($"<color=#ffff00>Icons must currently be placed inside TabbyStudios/TabbyCommon/Res/Icons</color>");
                return null;
            }
            

            defaultIcons[name] = loadedIcon; 
            return defaultIcons[name];
        }
        
        public static List<Texture2D> GetIcons()
        {
            return defaultIcons.Values.ToList();
        }
    
        public static string GetIconName(string menuPath)
        {
            return defaultIconNames.GetValueOrDefault(menuPath, "");
        }
    
        private static Texture2D LoadIcon(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(TabbyCommonFiles.IconPath(name));
        }

        public static Map<string, string> BuildDefaultNames()
        {
            var context = TabbyAssets.hasTabbyContext ? TabbyAssets.tabbyContextData.InvokeStaticMethod<Map<string, string>>("GetIconMap") : new();
            var menus = TabbyAssets.hasTabbyMenus ? TabbyAssets.tabbyMenusData.InvokeStaticMethod<Map<string, string>>("GetIconMap") : new();

            return Map<string,string>.CombineMaps(context, menus, common);
        }

        private static Map<string, string> common = new()
        {
            { "__notamenu1", "eye" },
            { "__notamenu2", "eye-off" },
            { "__notamenu3", "chevron-right" },
            { "__notamenu4", "gradient" },
            { "__notamenu5", "plus" },
            { "__notamenu6", "trash" },
            { "__notamenu7", "plus-minus" },
            { "__notamenu8", "check" },
        };
        
    }
}