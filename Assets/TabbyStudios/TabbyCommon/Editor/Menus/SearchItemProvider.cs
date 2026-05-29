using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class SearchItemProvider : ItemProvider
    {
        private string search;
        private EditorItemProvider provider = new();

        [Setting(false)]
        public static bool searchEverywhere;
        
        public SearchItemProvider(string search)
        {
            this.search = search.ToLower();
        }
        
        public override List<CustomMenuEntry> GetItems(string path)
        {
            if (search.IsNullOrEmpty())
            {
                return provider.GetItems(path);
            }
            else
            {
                var everywhere = Config.GetSetting<bool>(nameof(searchEverywhere));
                if (everywhere)
                {
                    var menu = MenuDataSerializer.GetMenu(DataNode.rootPath);
                    var list = menu.downwards.Where(item => item.displayName.ToLower().Contains(search) && !item.hasChildren).Select(data => new SearchItem(data))
                        .Where(item => item.data.path != TabbyAssets.anonymousMenuPath).ToList<CustomMenuEntry>();
                    return list.Count > 25 ? list.Slice(0, 25) : list;
                }
                else
                {
                    var menu = MenuDataSerializer.GetMenu(path);
                    var list =  menu.downwards.Where(item => item.displayName.ToLower().Contains(search) && !item.hasChildren).Select(data => new SearchItem(data))
                        .Where(item => item.data.path != TabbyAssets.anonymousMenuPath).ToList<CustomMenuEntry>();
                    return list.Count > 25 ? list.Slice(0, 25) : list;
                }
            }
        }
        
        public override void AddOtherComponents(CustomMenuEntry entry)
        {
            provider.AddOtherComponents(entry);
        }

        public override void CreateMenuBehaviour()
        {
            provider.CreateMenuBehaviour();
        }
    }
}