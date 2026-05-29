using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ItemConfigPopupOpener : VisualComponent
    {
        private CustomMenuEntry entry;
        
        public override void OnAttach()
        {
            entry = GetComponent<CustomMenuEntry>();
            RegisterMouseUp();
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                var pos = MouseUtil.MousePosition(e) + new Vector2(0, 20);
                ItemConfigPopup.Create(pos, entry).Display();
            }
        }
    }
}