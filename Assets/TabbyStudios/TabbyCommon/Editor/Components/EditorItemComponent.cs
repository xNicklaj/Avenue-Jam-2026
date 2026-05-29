using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class EditorItemComponent : ItemComponent
    {
        public bool shouldRun = true;
    
        public override void OnAttach()
        {
            base.OnAttach();
            
            //RegisterKeyDown(); don't register, let the parent menu handle it
            
            if (!EditorItemShowCondition.ShouldShow(entry.data))
            {
                entry.DisplayAsConditionNotMet();
            }
        }

        public override void DoItemAction()
        {
            if (entry.conditionMet)
                Run();
        }

        public override void OnMouseDown(MouseDownEvent e)
        {
            if (e is null) return;
            
            if (e.button != 0 || !entry.conditionMet)
                return;
            
            base.OnMouseDown(e);
            Run();
        }

        public override void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
            {
                Run();
            }
        }

        private void Run()
        {
            if (!entry.data.hasChildren && shouldRun)
            {
                target.GetComponentUpwards<CustomMenu>().manager.ClearMenus();
                ItemRunner.Run(entry.data.executionPath);
            }
        }
        
    }
}