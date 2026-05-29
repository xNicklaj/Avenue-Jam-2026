using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SettingsPage : TabbyWindow<SettingsPage>, FastCacheSearch
    {
        private VisualElement tabBar;
        private Toggle verticalTabsToggle;
        private VisualElement tabContentContainer;

        [Setting(1024f)]
        private static float settingsPageWidth;
        
        [Setting(720f)]
        private static float settingsPageHeight;
        
        [Setting(450f)]
        private static float settingsPageX;
        
        [Setting(200f)]
        private static float settingsPageY;

        private static Rect lastSeenRect;
        
        public override void OnCreate()
        {
            AssemblyReloadEvents.beforeAssemblyReload += ClearWindows;

            size = GetSize();
            pos = GetPosition();
            minSize = new Vector2(1024, 640);
            titleContent = new GUIContent("Tabby Context");
        
            VisualElement uxml = AssetCache.LoadXml("SettingsPage");
            StyleSheet themeStyleSheet = AssetCache.LoadUss(EditorGUIUtility.isProSkin ? "DarkMode" : "LightMode");
            uxml.styleSheets.Add(themeStyleSheet);

            //LayoutCalculator.CalculateLayout(uxml, position.size);
            TabbyAssets.AddToSettingsPage(uxml);
            
            rootVisualElement.AddElement(uxml);
        }
        
        private void SetAllToAbsolute(VisualElement root)
        {
            if (root == null) return;

            root.style.position = Position.Absolute;

            foreach (var child in root.Children())
            {
                SetAllToAbsolute(child);
            }
        }

        public void OnGUI()
        {
            lastSeenRect = position;
            
            var e = Event.current;
            if (e is null)
                return;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.L)
            {
                Config.instance.Flip("lockButton");
            }
            
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.P)
            {
                Config.instance.Flip("previewButton");
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Config.instance.Set(nameof(settingsPageX), lastSeenRect.x);
            Config.instance.Set(nameof(settingsPageY), lastSeenRect.y);
            Config.instance.Set(nameof(settingsPageWidth), lastSeenRect.width);
            Config.instance.Set(nameof(settingsPageHeight), lastSeenRect.height);
        }
        
        private static Vector2 GetPosition()
        {
            return new Vector2(Config.instance.GetFloat(nameof(settingsPageX)), Config.instance.GetFloat(nameof(settingsPageY)));
        }

        private static Vector2 GetSize()
        {
            return new Vector2(Config.instance.GetFloat(nameof(settingsPageWidth)), Config.instance.GetFloat(nameof(settingsPageHeight)));
        }
    }
}
