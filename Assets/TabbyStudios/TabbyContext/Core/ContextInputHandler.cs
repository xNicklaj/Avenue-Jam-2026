using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class ContextInputHandler
    {
        static ContextInputHandler()
        {
            if (!Config.GetSetting<bool>("fallbackInputHandling")) return;

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }
        
        private static void ProjectWindowItemOnGUI(string s, Rect r)
        {
            if (Event.current.type == EventType.ContextClick && !EditorInputHandler.ShouldUseDefault())
            {
                Event.current.Use();
                EditorInputHandler.LaunchMenu("_tabby_ProjectBrowser", GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            }
        }
        
        private static void HierarchyWindowItemOnGUI(int i, Rect r)
        {
            if (Event.current.type == EventType.ContextClick && !EditorInputHandler.ShouldUseDefault())
            {
                Event.current.Use();
                EditorInputHandler.LaunchMenu("_tabby_Hierarchy", GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            }
        }
    }
}