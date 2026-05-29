using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SettingsItemComponent : ItemComponent
    {
        public static bool blockNewMenus => Config.GetSetting<bool>("lockButton");
        private string scrollConfig => $"{entry.data.parentPath}_scroll_level";
    
        public override void Start()
        {
            base.Start();
            RegisterWheel();
            var shortcut =  target.Q<Label>("Shortcut");
            if (shortcut is not null)
                shortcut.style.display = DisplayStyle.None;
            SetVisibilityStyle(entry.data.shown);
        }

        private void SetVisibilityStyle(bool visible)
        {
            var multiplier = visible ? 1 : 0.5f;
            target.style.opacity = multiplier;
            foreach (var comp in target.SelectComponent<TextComponent>())
            {
                comp.opacity = multiplier;
            }
        }
        
        public override void OnMouseDown(MouseDownEvent e)
        {
            base.OnMouseDown(e);
            e.StopPropagation();

            if (!e.ctrlKey && !e.commandKey)
                return;
        
            var visible = MenuDataSerializer.SwitchVisible(entry.data);
            SetVisibilityStyle(visible);
        }

        public override void OnMouseEnter(MouseEnterEvent e)
        {
            if(!blockNewMenus)
                base.OnMouseEnter(e);
        }

        public override void OnWheel(WheelEvent e)
        {
            EditorUtil.UpdateDelayCall(() =>
            {
                var pos = entry.parentMenu.target.SelectFirstComponent<AbstractScroller>().offset;
                UnityWindows.GetWindow<SettingsPage>().BufferConfig(scrollConfig, pos);
            });
        }
    }
}