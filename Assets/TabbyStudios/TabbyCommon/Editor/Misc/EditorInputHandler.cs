using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public static class EditorInputHandler
    {
        public static TabbyInput input = TabbyInput.Input(1);
        public static EditorMenuManager manager { get; } = new();
        
        private static bool didSceneViewDrag;
        
        static EditorInputHandler()
        {
            EditorApplication.update += CloseMenusOnMinimize;
            UnityWindows.onFocusChanged += TrackFocusedWindow;
            
            #if !UNITY_2023_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
            #endif
        }
        
        private static void OnSceneGUI(SceneView view)
        {
            if (Event.current.button != 1) return;
            
            if (Event.current.type == EventType.MouseDrag)
                didSceneViewDrag = true;
            
            if (Event.current.type == EventType.MouseDown)
            {
                didSceneViewDrag = false;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (!didSceneViewDrag)
                {
                    manager.CreateMenu("_tabby_SceneView", GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                }

                didSceneViewDrag = false;
            }
        }
        
        private static void CloseMenusOnMinimize()
        {
            if (UnityWindows.EditorMinimized() && Config.GetSetting<bool>("closeMenusOnAltTab"))
                ClearMenus();
        }
        
        public static void ClearMenus()
        {
            manager.ClearMenus();
        }
        
        public static void LaunchMenu(string path, Vector2 screenPos)
        {
            manager.ClearMenus();
            manager.CreateMenu(path, screenPos);
        }
        
        public static void LaunchMenuAndKeepNoFocus(string path, Vector2 screenPos)
        {
            manager.ClearMenus();
            manager.CreateMenu(path, screenPos);
        }
        
        public static bool ShouldUseDefault()
        {
            if (input?.current?.modifiers is null)
                return true;
            
            var split = input.current.modifiers.ToString().Split();
            var shiftClick = Config.GetSetting<string>("defaultMenuDropdown") == split[0] && split.Length == 1;
            var dontUseMenus = !Config.GetSetting<bool>("useCustomMenus");
            return shiftClick || dontUseMenus;
        }

        public static void TrackFocusedWindow()
        {
            var focusedWindow = EditorWindow.focusedWindow;
            
            if (focusedWindow is null || !focusedWindow.IsA<CustomMenuWindow>())
            {
                manager.FocusChanged();
            }
        }
        
    }
    
}