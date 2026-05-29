using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ItemComponent : VisualComponent
    {
        public bool shouldCancel, blockOpen;
        protected bool pointerDown;

        public CustomMenuEntry entry;
    
        public override void OnAttach()
        {
            entry = GetComponent<CustomMenuEntry>();
            RegisterMouseDown();
            RegisterMouseUp();
            RegisterMouseEnter();
            RegisterMouseLeave();
        }

        public virtual void DoItemAction()
        {
            
        }

        public override void OnMouseDown(MouseDownEvent e)
        {
            pointerDown = true;
        }
    
        public override void OnMouseLeave(MouseLeaveEvent e)
        {
            pointerDown = false;
            shouldCancel = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            shouldCancel = true;
        }

        public override void OnMouseEnter(MouseEnterEvent e)
        {
            shouldCancel = false;
            RunAsync.Fire(Config.instance.GetInt("menuOpenDelay"), () =>
            {
                if (shouldCancel || blockOpen)
                    return;

                if (target?.GetComponentUpwards<CustomMenu>()?.provider is SearchItemProvider)
                    return;
                
                entry.TryOpenSubmenu();
            });        
        }
    }
}