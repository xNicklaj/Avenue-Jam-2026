using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CustomMenuEntry : VisualComponent
    {
        [Setting(false)]
        private static bool hideDisabledItems;
        
        public ItemData data;
        public CustomMenu parentMenu;
        public MenuManager manager => parentMenu.manager;
        public bool selected;
        public bool conditionMet = true;
        
        public CustomMenuEntry(ItemData data)
        {
            this.data = data;
        }

        public virtual VisualElement CreateParent()
        {
            return null;
        }
        
        public void TryOpenSubmenu()
        {
            if (manager.HasOpenMenu(data.path))
                return;

            manager.ClearMenus(data.path);
            if (data.hasChildren)
            {
                manager.CreateMenu(caller:this);
            }
        }
        
        public override string ToString()
        {
            return data.path;
        }

        public virtual void DisplayAsConditionNotMet()
        {
            conditionMet = false;
            if (!Config.GetSetting<bool>(nameof(hideDisabledItems)))
            {
                target.style.opacity = 0.5f;
                foreach (var comp in target.SelectComponent<TextComponent>())
                {
                    comp.opacity = 0.5f;
                }
            }
            else
            {
                target.Hide();
            }
        }
        
    }
}