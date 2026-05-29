using System;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class AreYouSureLabel : VisualComponent
    {
        public ItemData data { get; set; }
        
        private Label label => target as Label;
        public string text
        {
            get => label.text;
            set => label.text = value;
        }
    }

    public class CancelButton : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void OnAttach()
        {
            RegisterMouseUp();
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            if (target.ContainingWindow() is TabbyWindow w)
                w.Dispose();
        }
    }

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

            if (target.ContainingWindow() is TabbyWindow w && w.notDisposed)
                w.Dispose();
        }
    }
}