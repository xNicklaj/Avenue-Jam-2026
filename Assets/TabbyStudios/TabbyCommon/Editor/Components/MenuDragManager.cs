using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class MenuDragManager : VisualComponent
    {

        public VisualElement dragElement;
        public SettingsDragComponent dragTarget => dragElement?.GetComponent<SettingsDragComponent>();
        public static MenuDragManager previous;
        public static bool mustHoldKey = Config.GetSetting<bool>("holdKeyToReorder");

        private static bool leftMouseHeld(MouseEnterEvent e) => e.pressedButtons % 2 == 1;
        private VisualElement menuView => target.FirstComponent<TabbyScrollView>();
    
         public override void OnMouseEnter(MouseEnterEvent e)
         {
             if (!leftMouseHeld(e))
                 return;
        
             if (!e.altKey && mustHoldKey)
                 return;
             
             if (previous?.dragTarget is null)
                 return;
             
             if (previous == this)
                 return;
        
             if (previous.dragTarget.entry.data.IsChildPath(target.GetComponentUpwards<CustomMenu>().path))
                 return;
        
             
             if (previous.dragTarget.isDragging)
             {
                 previous.dragTarget.StopDragging();
             }
        
             dragElement = previous.dragElement;
             previous.dragElement = null;
        
             dragElement.parent.Remove(dragElement);
             menuView.Add(dragElement);
        
             dragTarget.StartDragging(e.mousePosition);
         }
        
         public override void OnMouseLeave(MouseLeaveEvent e)
         {
             if (dragTarget is null)
                 return;
        
             previous = this;
         }
        
        
         public override void Start()
         {
             RegisterMouseEnter();
             RegisterMouseLeave();
         }
    
    
    }
}