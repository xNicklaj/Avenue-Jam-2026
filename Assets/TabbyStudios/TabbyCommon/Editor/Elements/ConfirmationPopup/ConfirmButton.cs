using System;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ConfirmButton : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }
        public Action action;
        
        public override void OnAttach()
        {
            RegisterMouseUp();
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            action();
            
            if(target.ContainingWindow() is TabbyWindow w && w.notDisposed)
                w.Dispose();
        }
    }
}