using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class AnonymousMenuBuilder : FastCacheSearch
    {
        public static string anonymousMenuPath = TabbyAssets.anonymousMenuPath;

        public static List<ItemData> AnonymousMenuData()
        {
            return MenuBuilder.ProcessUnityData(FormatAsUnityData());
        }
        
        public static List<ItemData> FormatAsUnityData()
        {
            var items = new LinkedList<ItemData>();

            var anonymousItems = MenuPatcher.items;
            for (int i = 0; i < anonymousItems.Count; i++)
            {
                var t = anonymousItems[i];
                var item = new ItemData();
                item.isSeparator = t.separator;

                if (item.isSeparator)
                {
                    item.originalPath = $"{anonymousMenuPath}/{(t.path.IsNullOrEmpty() ? "" : t.path+"/")}separator{i}";
                }
                else
                {
                    item.originalPath = $"{anonymousMenuPath}/{t.path}".RemoveTrailing("/");
                }

                item.originalPriority = i;
                item.priority = i;
                item.executionPath = item.originalPath;
                item.path = item.originalPath;
                if (t.selected)
                {
                    item.iconName = "check";
                }
                
                items.AddLast(item);
            }

            items.AddFirst(new ItemData(){originalPath = anonymousMenuPath});
            var itemList = items.ToList();
            var allItems = DataNode.CreateRoot().FromList(itemList).Children().First().Flatten().Select(node => node.data).ToList();
            allItems.ForEach(i => i.originalPath = i.path);

            foreach (var item in allItems)
            {
                if (item.originalPriority == 0)
                {
                    var child = allItems.FirstOrDefault(i => i.originalPath.RemoveAfterLast("/") == item.originalPath);
                    if (child is not null)
                    {
                        item.originalPriority = child.originalPriority;
                    }
                }
                
                if (item.isSeparator)
                {
                    item.path = item.path.RemoveAfterLast("/");
                    item.originalPath = item.path.RemoveAfterLast("/");
                    item.executionPath = item.path.RemoveAfterLast("/");
                }
            }

            allItems = allItems.OrderBy(data => data.originalPriority).ToList();
            
            return allItems;
        }
    }
}