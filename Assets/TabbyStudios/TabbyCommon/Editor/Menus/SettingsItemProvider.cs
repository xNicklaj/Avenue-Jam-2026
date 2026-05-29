using System.Collections.Generic;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SettingsItemProvider : ItemProvider
    {
        public bool showAsPreview = Config.instance.GetBool("previewButton");

        
        public override List<CustomMenuEntry> GetItems(string path)
        {
            var menu = Profiles.instance.menuSerializer.GetMenu(path);
            var objects = FromData(showAsPreview ? menu.shownItems : menu.items);
            return objects;
        }
    
        public override void AddOtherComponents(CustomMenuEntry entry)
        {
            if (showAsPreview)
            {
                AddPreview(entry);
            }
            else
            {
                AddNormal(entry);
            }
        }
    
        private void AddPreview(CustomMenuEntry entry)
        {
            entry.target.AddComponent<SettingsDragComponent>();
            entry.target.AddComponent<ItemConfigPopupOpener>();
            
            if (!entry.data.isSeparator)
            {
                var comp = entry.target.AddComponent<EditorItemComponent>();
                comp.shouldRun = false;
            }
        }

        private void AddNormal(CustomMenuEntry entry)
        {
            if (entry is Separator s)
            {
                s.collapse = false;
                entry.target.Q("Item").style.height = 10;
            }
        
            var shortcut = entry.target.Q("Shortcut");
            shortcut?.Hide();

            entry.target.AddComponent<SettingsItemComponent>();
            entry.target.AddComponent<SettingsDragComponent>();
            entry.target.AddComponent<ItemConfigPopupOpener>();

        }

        public override void CreateMenuBehaviour()
        {
            target.AddComponent<MenuDragManager>();
        }
    }
}