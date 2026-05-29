using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class NewSeparatorButton : ButtonComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void OnAttach()
        { 
            base.OnAttach();
            target.tooltip = "Insert new separator under";
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            Profiles.instance.menuSerializer.InsertUnder(data, isSeparator:true, userCreated: true);
        }
        
    }
}