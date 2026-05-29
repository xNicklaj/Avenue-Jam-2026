using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SwitchVisibilityButton : ButtonComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        private bool visible;
    
        public override void OnAttach()
        {
            base.OnAttach();
            visible = data.shown;
            SetBackground();
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            visible = !data.shown;
            SetBackground();
            Profiles.instance.menuSerializer.SetProperty(data, data => data.shown = visible);
        }

        private void SetBackground()
        {
            target.style.backgroundImage = IconLoader.GetIcon(visible ? "eye" : "eye-off");
        }
    
    }
}