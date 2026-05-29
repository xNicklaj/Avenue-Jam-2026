using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class EditorMenuManager : MenuManager
    {
        
        private static string xmlName = "CustomMenu";
        
        public List<CustomMenuWindow> windows = new();

        public List<CustomMenu> menus
        {
            get => windows.Select(w => w.menu).ToList();
            set { }
        }

        public static bool windowClosingEnabled = true;
        
        public CustomMenu CreateMenu(CustomMenuEntry caller)
        {
            Assert.IsNotNull(caller.target);
            return CreateMenu(caller.data.path, CustomMenuWindow.CreatePos(caller, 1));
        }

        public CustomMenu CreateMenu(string path, Vector2 position)
        {
            try
            {
                var window = CustomMenuWindow.Create(position);
                
                var xml = AssetCache.LoadXml(xmlName);
                
                var menuElement = xml.First("CustomMenu");
                TryAddBar(menuElement, path);
                var provider = menuElement.AddComponent<EditorItemProvider>(); 
                var menu = menuElement.AddComponent<CustomMenu>(this,path,provider);
                window.menu = menu;
                window.rootVisualElement.AddElement(xml);
                
                windows.Add(window);
                DisableClosing();

                var layout = xml.GetComponentDownwards<CustomMenuLayout>();
                layout.Calculate();
                window.size = layout.calculatedSize;
                window.screenPosition = position;
                window.Display();
                return menu;
            }
            catch
            {
                EditorApplication.delayCall += () => UnityWindows.GetWindows<CustomMenuWindow>().ForEach(w => w.Destroy());
                throw;
            }
        }

        private void TryAddBar(VisualElement menuElement, string path)
        { 
            if (!Config.GetSetting<bool>("useSearchBar"))
                return;
            
            if(!path.Contains("/") || Config.GetSetting<bool>("useSearchBarOnSubmenus"))
            {
                var bar = menuElement.SelectFirstComponent<SearchBar>();
                bar.EnableBar();
                if(Config.GetSetting<bool>("alwaysShowSearchBar"))
                    bar.ShowBar();
            }
        }
        
        public void ClearMenus(string parentItemPath)
        {
            var toClose = windows.Where(w => !parentItemPath.Contains(w.menu.path + "/") && w.menu.path != parentItemPath).ToList();
            
            windows = windows.Except(toClose).ToList();
            toClose.ForEach(w => w.Dispose());
            DisableClosing();
            
            //parent window can be null since clear is also called when the menu is being opened for the first time
            
            windows.FirstOrDefault(w => w.menu.path == parentItemPath.RemoveAfterLast("/"))?.Focus();
        }
        
        public void ClearMenus()
        {
            windows.ForEach(w => w?.Dispose());
            windows = new();
        }

        public bool HasOpenMenu(string path)
        {
            return menus.Any(menu => menu.path == path);
        }
        
        
        public void DisableClosing()
        {
            windowClosingEnabled = false;
            EditorUtil.DelayCall(1, () => windowClosingEnabled = true);
        }
        
        public void FocusChanged()
        {
            if (EditorWindow.focusedWindow is null || EditorWindow.focusedWindow.IsA<CustomMenuWindow>())
                return;

            if (!windowClosingEnabled)
                return;
            
            ClearMenus();
        }
        
        public void RefreshMenu(string path)
        {
    
        }

        public void ClearMenu(string path)
        {
    
        }

    }
}