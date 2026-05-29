using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CancelButton : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }
    
        public override void OnAttach()
        {
            RegisterMouseUp();
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            if(target.ContainingWindow() is TabbyWindow w)
                w.Dispose();
        }
    }
}