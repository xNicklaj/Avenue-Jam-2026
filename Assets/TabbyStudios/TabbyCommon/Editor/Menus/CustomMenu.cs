using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class CustomMenu : VisualComponent, RadioManager<CustomMenuEntry>
    {
        public string path;
        public MenuManager manager;
        public ItemProvider provider;
        public List<CustomMenuEntry> items => scroller.items.Select(i => i.SelectFirstOrDefaultComponent<CustomMenuEntry>()).Where(i => i is not null).ToList();
        private AbstractScroller scroller;
        
        public CustomMenu(MenuManager manager, string path, ItemProvider provider)
        {
            this.manager = manager;
            this.path = path;
            this.provider = provider;
        }
    
        //todo most move to Start Eventually otherwise we must always add provider BEFORE  custom menu
        public override void Awake()
        {
            RegisterGeometryChanged();
            
            scroller = target.SelectFirstComponent<AbstractScroller>();
            target.style.backgroundColor = UnityColors.defaultColor;

            provider.CreateMenuBehaviour();
            var createdItems = provider.GetItems(path);
            
            foreach (var item in createdItems)
            {
                item.parentMenu = this;
                var elem = item.CreateParent();
                elem.AddComponent(item);
                provider.AddOtherComponents(item);
                scroller.AddItem(item.target);
            }
        
        }
        
        public CustomMenuEntry Next(CustomMenuEntry item)
        {
            var index = items.IndexOf(item) + 1;
            return index < items.Count ? items[index] : null;
        }

        public void MoveSelection(int d)
        {
            var actualItems = items.Where(i => i is Item || i is SearchItem).ToList();
            if (actualItems.IsNullOrEmpty())
                return;


            var index = 0;
            var selected = actualItems.FirstOrDefault(item => item.selected);
            if (selected is not null)
                index = actualItems.IndexOf(selected) + d;
            
            if (index >= actualItems.Count || index < 0)
                return;
            
            actualItems[index].InvokeMethod("Select");
        }

        public void SelectCurrent()
        {
            var actualItems = items.Where(i => i is Item or SearchItem && i.GetFieldValue<bool>("selected")).ToList();
            if (actualItems.IsNullOrEmpty())
                return;

            actualItems.First().GetComponent<ItemComponent>().OnMouseEnter(null);
            actualItems.First().GetComponent<ItemComponent>().DoItemAction();
        }
    
        public CustomMenuEntry NextVisible(CustomMenuEntry item)
        {
            return items.FirstOrDefault(i => i.data.priority > item.data.priority && i.target.visible && i != item);
        }

        public void UncheckOthers(CustomMenuEntry t)
        {
            items.Where(i => i is Item || i is SearchItem).Where(item => item != t).ForEach(item => item.InvokeMethod("Deselect"));
        }

        public void ClearElements()
        {
            target.Pluck();
            target.Disable();
        }

        public void Refresh()
        {
            scroller.Clear();
            Awake();
        }
    }
}