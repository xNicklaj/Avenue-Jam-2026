using System.Collections.Generic;

namespace TabbyStudios
{
    public class EditorItemProvider : ItemProvider
    {
        public override List<CustomMenuEntry> GetItems(string path)
        {
            var menu = Profiles.instance.menuSerializer.GetMenu(path);
            var objects = FromData(menu.shownItems);
            return objects;
        }

        public override void AddOtherComponents(CustomMenuEntry entry)
        {
            if (!entry.data.isSeparator)
            {
                entry.target.AddComponent<EditorItemComponent>();
            }
        }

        public override void CreateMenuBehaviour()
        {
        
        }
    }
}