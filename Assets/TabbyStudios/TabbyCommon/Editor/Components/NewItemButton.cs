using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class NewItemButton : ButtonComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void OnAttach()
        {
            base.OnAttach();
            target.tooltip = "Insert new item under";
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            Profiles.instance.menuSerializer.InsertUnder(data, isSeparator: false, userCreated: true);
        }
        
    }
}