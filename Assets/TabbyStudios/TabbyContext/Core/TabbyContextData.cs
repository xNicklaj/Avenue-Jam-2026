using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TabbyStudios
{
    public class TabbyContextData : FastCacheSearch
    {
        public static string Prefix(string name) => name.AddLeading(TabbyAssets.extraMenuPrefix);
        private static Type cachedPatcher;
        
        private static Map<string, string> pathMap = new ()
        {
            {Prefix("ProjectBrowser"), "Assets"},
            {Prefix("Hierarchy"), "GameObject"},
        };
        
        private static Map<string, Func<List<ItemData>>> map = new ()
        {
            {Prefix("SceneView"), SceneViewData.SceneViewItemData},
            {Prefix("Hierarchy"), HierarchyData.HierarchyItemData},
            {Prefix("ProjectBrowser"), ProjectBrowserItemData},
        };

        static TabbyContextData()
        {
            var builder = TypeCache.GetTypesDerivedFrom<FastCacheSearch>().FirstOrDefault(t => t.FullName == "TabbyStudios.AnonymousMenuBuilder");
            if(builder is not null)
            {
                map[TabbyAssets.anonymousMenuPath] = () => builder.InvokeStaticMethod<List<ItemData>>("AnonymousMenuData");
            }
        }
        
        public static List<ItemData> GetItemData(string path)
        {
            //return map.GetOrDefault(path)();
            return map.GetOrDefault(path)?.Invoke();
        }
        
        public static string MapToUnityPath(string path)
        {
            var root = path.RemoveAfterFirst("/");
            return pathMap.ContainsKey(root) ? path.ReplaceFirst(root, pathMap[root]) : path;
        }
        
        public static string InferCustomMenu(List<string> options)
        {
            if (new[] { "Isolate", "Add Component..." }.All(options.Contains))
                return Prefix("SceneView");

            if (new[] { "Cut", "Copy", "Paste", "3D Object", "Camera", "Light" }.All(options.Contains))
                return Prefix("Hierarchy");
            
            return "";
        }
        
        public static string InferPopupMenu(string menuItemPath)
        {
            if (menuItemPath == "Assets")
                return Prefix("ProjectBrowser");
            
            return "";
        }

        private static List<ItemData> ProjectBrowserItemData()
        {
            var data = MenuBuilder.ProcessUnityData(MenuBuilder.GetDataFromUnity("Assets"));
            UpgradeItems(data);
            return data;
        }

        public static List<string> TopLevelMenus()
        {
            return map.Keys.Except(TabbyAssets.anonymousMenuPath).ToList();
        }

        public static Map<string, string> GetIconMap()
        {
            return ContextIconData.iconMap;
        }

        public static Map<string, Func<bool>> GetDefaultShowConditions()
        {
            return ContextItemShowConditions.defaultConditions;
        }
        
        public static Map<string, Map<string, Action<Object[]>>> GetCalls()
        {
            cachedPatcher ??= TypeCache.GetTypesDerivedFrom<FastCacheSearch>().FirstOrDefault(t => t.FullName == "TabbyStudios.MenuPatcher");
            return cachedPatcher?.GetMemberValue<Map<string, Map<string, Action<Object[]>>>>("calls");
        }

        public static void AddToSettingsPage(VisualElement uxml)
        {
            var toolbar = new VisualElement{name = "ShowMenuToolbar"};
            toolbar.AddToClassList("bottom-toolbar");
            toolbar.AddComponent<ShowMenuToolbar>(TopLevelMenus());
            uxml.SelectFirstComponent<ShowMenuToolbarContainer>().target.AddElement(toolbar);
        }
        
        private static void UpgradeItems(List<ItemData> data)
        {
            foreach (var item in data)
            {
                foreach (var field in new[]{"path", "originalPath"})
                {
                    var value = item.GetFieldValue<string>(field);
                    item.SetFieldValue(field, value.ReplaceFirst("Assets", Prefix("ProjectBrowser")));
                }
                
                item.iconName = IconLoader.GetIconName(item.path);
            }
        }
    }
}