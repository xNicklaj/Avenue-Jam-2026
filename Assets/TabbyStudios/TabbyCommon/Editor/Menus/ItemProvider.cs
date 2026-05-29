using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public abstract class ItemProvider : VisualComponent
    {
        public bool disableCollapsingSeparators;
        
        public abstract List<CustomMenuEntry> GetItems(string path);
        public abstract void AddOtherComponents(CustomMenuEntry entry);
        public abstract void CreateMenuBehaviour();
        
        public CustomMenuEntry FromData(ItemData data)
        {
            if (data.isSeparator)
            {
                var sep = new Separator(data);
                if (disableCollapsingSeparators)
                    sep.collapse = false;
                return sep;
            }
            return new Item(data);
        }
    
        public List<CustomMenuEntry> FromData(List<ItemData> list)
        {
            return list.Select(FromData).ToList();
        }
    
        public List<CustomMenuEntry> FromData(MenuData menu)
        {
            return FromData(menu.items);
        }
    }
}