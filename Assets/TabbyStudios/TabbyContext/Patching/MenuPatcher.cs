using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using SelectMenuItemFunction = UnityEditor.EditorUtility.SelectMenuItemFunction;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class MenuPatcher : FastCacheSearch
    {
        public static List<AnonymousMenuItemData> items;
        
        public static Map<string, Map<string, Action<Object[]>>> calls = new()
        {       
            {$"{TabbyContextData.Prefix("ProjectBrowser")}",new()},
            {$"{TabbyContextData.Prefix("Hierarchy")}",new()},
            {$"{TabbyContextData.Prefix("SceneView")}",new()},
        };

        static MenuPatcher()
        {
            if (Config.instance.GetBool("fallbackInputHandling")) return;
            
            Patcher.Prefix(typeof(EditorUtility).GetMethodInfo("Internal_DisplayCustomMenu"), typeof(MenuPatcher).GetMethodInfo("InterceptCustomMenu"));
            Patcher.Prefix(typeof(EditorUtility).GetMethodInfo("Internal_DisplayPopupMenu"), typeof(MenuPatcher).GetMethodInfo("InterceptPopupMenu"));

            EditorUtil.DoAfterLoad(() => calls["SceneView"] = 
                new(MenuBuilder.GetDataFromUnity("Assets").Select(i => i.executionPath).Distinct().Select(p => (p, new Action<Object[]>(_ => 
                { if(!p.IsNullOrEmpty()) EditorApplication.ExecuteMenuItem(p); })))));
        }
        
        public static bool InterceptPopupMenu(Rect position, string menuItemPath)
        {
            var menu = TabbyContextData.InferPopupMenu(menuItemPath.RemoveTrailing("/"));
            
            if (menu == "" || EditorInputHandler.ShouldUseDefault())
                return true;

            EditorUtil.UpdateDelayCall(() => EditorInputHandler.LaunchMenu(menu, position.position));
            return false;
        }
    
        public static bool InterceptCustomMenu(Rect screenPosition, string[] options, bool[] separator, int[] selected, SelectMenuItemFunction callback)
        {
            if (EditorInputHandler.ShouldUseDefault())
                return true;

            if (BlockInspectMenu(options))
                return false;
            
            var list = MenuBuilder.RemoveShortcut(options.Where(o => o is not null).ToArray());
            var menu = TabbyContextData.InferCustomMenu(list);
            
            if (menu == "")
            {
                if (Config.instance.GetBool("useAnonymousMenus"))
                {
                    items = list.Select((o, i) => new AnonymousMenuItemData(o, selected?.Contains(i) ?? false, separator[i])).ToList();
                    menu = TabbyAssets.anonymousMenuPath;
                    screenPosition.x += 1;
                    screenPosition.y += 18;
                }
                else
                {
                    return true;
                }
            }
            
            var dict = MenuCalls.calls.GetOrInsert(menu, () => new());
            
            foreach (var option in list)
            {
                if (!dict.ContainsKey(option))
                {
                    var i = list.IndexOf(option);
                    var optionPath = $"{menu}/{ModifiedName(option)}".ToLower();
                    dict[optionPath] = selection => callback(selection, list.ToArray(), i);
                }
            }

            EditorUtil.UpdateDelayCall(() => EditorInputHandler.LaunchMenu(menu, screenPosition.position));
            return false;
        }
        
        private static bool BlockInspectMenu(string[] options)
        {
            #if !Unity_2022_1_OR_NEWER
            if (options.Length == 1 && options[0] == "Inspect Element" && UnityWindows.GetWindow(TabbyAssets.settingsPage) is not null)
                return true;
            #endif
            
            return false;
        }

        private static string ModifiedName(string name)
        {
            if (name.StartsWith("Added GameObject/Apply to Prefab")) //this should have a test
            {
                return "Added GameObject/Apply to Prefab";
            }

            return name;
        }
    }
}