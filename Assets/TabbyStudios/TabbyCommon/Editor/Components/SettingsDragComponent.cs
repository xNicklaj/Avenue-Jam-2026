using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SettingsDragComponent : VisualComponent
    {
        public static bool mustHoldKey => Config.GetSetting<bool>("holdKeyToReorder");
    
        public MenuDragManager dragger;
        public CustomMenuEntry entry;
        public bool isDragging;
    
        private VisualElement after, before, fake, dragArea, itemList;
        
        public override void OnMouseDown(MouseDownEvent e)
        {
            if (e.ctrlKey || e.commandKey)
                return;
            
            if (!e.altKey && mustHoldKey)
                return;
        
            if (e.button != 0)
                return;
        
            StartDragging(e.mousePosition);
        }
        
        public override void OnMouseMove(MouseMoveEvent e)
        {
            if (!isDragging)
                return;
            
            UpdateTargetPosition(e.mousePosition);
        }
        
        public void StartDragging(Vector2 position)
        {
            target.CaptureMouse();
            isDragging = true;
            target.SetSize(target.UnscaledSize()*Config.GetSetting<float>("settingsMenuZoom"));
            dragger = target.GetComponentUpwards<MenuDragManager>();
            dragger.dragElement = target;
            itemList = dragger.target.FirstComponent<TabbyScrollView>();
        
            foreach(var comp in target.Root().SelectComponent<SettingsItemComponent>())
            {
                comp.blockOpen = true;
            }

            GetComponent<Item>().lockColor = true;
            
            InitFake();
        
            target.style.position = Position.Absolute;
        
            after = FindAfter();
        
            itemList.AddElement(fake);
            fake.PlaceInFront(after);
            itemList.Remove(target);
        
            dragArea = fake.Root().WhereComponent<CustomMenu>().First(c => c.Q().Contains(fake));
            dragArea.Add(target);
        
            UpdateTargetPosition(position);
            EditorUtil.UpdateDelayCall(1, () => UpdateTargetPosition(position));
            EditorUtil.UpdateDelayCall(2, () => UpdateTargetPosition(position));
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            target.ReleaseMouse();
            
            if (!isDragging)
                return;
            
            StopDragging();
        
            var beforeData = sb?.GetComponentUpwards<CustomMenuEntry>()?.data;
            var afterData = sa?.GetComponentUpwards<CustomMenuEntry>()?.data;
            
            if(beforeData != null || afterData !=  null)
                MenuDataSerializer.Reorder(entry.data, beforeData, afterData);
        }
        
        public void StopDragging()
        {
            isDragging = false;
            GetComponent<Item>().lockColor = false;
            
            foreach(var comp in target.Root().SelectComponent<SettingsItemComponent>())
            {
                comp.blockOpen = false;
            }
        
            target.parent.Remove(target);
            target.style.position = Position.Relative;
            target.transform.position = Vector3.zero;
            fake.parent.Add(target);
            target.PlaceBehind(fake);
            fake.parent.Remove(fake);
            fake.Clear();
        }
        
        private VisualElement sa, sb;
        private void ReorderVisualElements()
        {
            after = FindAfter();
            before = FindBefore();
            
            if(target.transform.position.y >= 0)
            {
                if (after is null)
                {
                    if (before is null)
                        return;
                    fake.PlaceInFront(before);
                }
                else
                {
                    fake.PlaceInFront(after);
                }
            }
            else
            {
                if (after is null && before is null) return;
                fake.PlaceBehind(after);
            }

            if (before is not null)
            {
                fake.SetAbsolutePositionRelative(new Vector2(0, -itemList.GetComponent<TabbyScrollView>().offset));
            }
            
            var children = itemList.Children().ToList();
            var index = children.IndexOf(fake);
            sa = index == children.Count - 1 ? null : children[index + 1];
            sb = index == 0 ? null : children[index - 1];
        }
        
        private void UpdateTargetPosition(Vector2 position)
        {
            ReorderVisualElements();
            var y = position.y + itemList.GetComponent<TabbyScrollView>().offset;
            var offset = dragArea.WorldPosition().y + 4;
            target.transform.position = new Vector3(target.LocalPosition().x + 10*target.Scale() ,(y-offset)/target.Scale() - target.Height()/2,0);
        }
        
        private VisualElement FindAfter()
        {
            var af =  itemList.Query("ItemWrapper").Build().FirstOrDefault(elem => elem.WorldPosition().y > target.WorldPosition().y - elem.Height());
            return af;
        }
        
        private VisualElement FindBefore()
        {
            var bef = itemList.Query("ItemWrapper").Build().LastOrDefault(elem => elem.WorldPosition().y < target.WorldPosition().y);
            return bef;
        }
        
        
        private void InitFake()
        {
            fake = new VisualElement();
            fake.SetBorder(4);
            fake.SetBorderColor(UnityColors.ItemHoverColor(entry.data.backgroundColor));
            fake.style.height = target.resolvedStyle.height;
            fake.style.minHeight = target.resolvedStyle.height;
        }
        
        public override void Start()
        {
            entry = GetComponent<CustomMenuEntry>();
            RegisterMouseDown();
            RegisterMouseUp();
            RegisterMouseMove();
        
        }
    }
}