using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TabbyStudios
{
    public interface MenuManager
    {
        public List<CustomMenu> menus { get; set; }

        public CustomMenu CreateMenu(string path, Vector2 position);
        public CustomMenu CreateMenu(CustomMenuEntry caller);
        
        public void ClearMenus(string parentItemPath);
        public void ClearMenu(string path);
        public void ClearMenus();
        
        public void RefreshMenu(string path);
        
        public List<CustomMenuEntry> GetEntries()
        {
            return menus.SelectMany(m => m.items).ToList();
        }

        public bool HasOpenMenu(string path)
        {
            return menus.Any(menu => menu.path == path);
        }
        
        public void ClearMenus(List<string> paths)
        {
            paths.ForEach(ClearMenu);
            menus = menus.Where(menu => !paths.Contains(menu.path)).ToList();
        }

    }
}
