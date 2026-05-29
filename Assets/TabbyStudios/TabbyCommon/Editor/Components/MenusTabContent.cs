using NUnit.Framework;
using UnityEngine;

namespace TabbyStudios
{
    public class MenusTabContent : TabContent
    {
        public SettingsMenuManager manager;

        public override void OnAttach()
        {
            manager = new(target);
        }
    
        public override void SetVisible(bool visible)
        {
            if (visible)
            {
                manager.CreateMenu(Config.instance.GetString(nameof(ShowMenuToolbar.lastShowMenuInMenusTab)),new Vector2(0,0));
            }

            else
            {
                Assert.NotNull(manager);
                manager.ClearMenus();
            }
        
            base.SetVisible(visible);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            manager.Dispose();
        }
    }
}