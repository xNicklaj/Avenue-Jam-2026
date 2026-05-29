using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;

namespace TabbyStudios
{
    public class MenuBuilder
    {
        public static DataNode BuildTree()
        {
            var tasks = new List<Task<MenuData>>();
            foreach (var path in ExtraData.topLevelPaths)
            {
                var items = GetItemData(path);
                var task = new Task<MenuData>(() => BuildMenu(path, items));
                tasks.Add(task);
                task.Start();
            }

            MenuData[] menus = Task.WhenAll(tasks).GetAwaiter().GetResult();
            
            var tree = PasteTrees(menus.Select(menu => menu.tree).ToList());
            tree.ForEach(FlattenChildPriorities);

            return tree;
        }

        public static MenuData BuildMenu(string path, List<ItemData> items)
        {
            var itemsWithMenuItem = GetGoodItemList();
            
            foreach (var item in items)
            {
                item.customizable = !ExtraData.nonCustomizablePaths.Contains(path);
                var fixedPath = TabbyAssets.MapToUnityPath(item.originalPath);
                if (itemsWithMenuItem.BinarySearch(fixedPath, StringComparer.Ordinal) > -1)
                {
                    item.hasMenuItem = true;
                }
            }
            
            var tree = DataNode.CreateRoot().FromList(items);
            tree.ForEach(InsertExtraSeparators);
            
            return new MenuData(tree);
        }

        
        public static MenuData BuildMenu(string path)
        {
            return BuildMenu(path, GetItemData(path));
        }
        
        public static List<ItemData> GetItemData(string path)
        {
            if (path == TabbyAssets.anonymousMenuPath)
                return ExtraData.GetItemData(path);
                
            if (ExtraData.GetExtraPaths().Contains(path))
            {
                return ExtraData.GetItemData(path);
            }
            else
            {
                return ProcessUnityData(GetDataFromUnity(path));
            }
        }

        public static List<ItemData> ProcessUnityData(List<ItemData> data)
        {
            //var menuPath = data.WithMin(item => item.originalPath.Length).originalPath;
            //Assert.IsFalse(menuPath.Contains("/"), $"This is expecting a full menu and data is missing a root, shortest path: {menuPath}");
            
            var processed = new List<ItemData>();
            int i = 0;
            foreach (var item in data)
            {
                var newItem = item;
                var originalItemPath = newItem.originalPath;
                
                var count = data.Slice(0, data.IndexOf(item)).Count(other => other.originalPath == item.originalPath);

                if (count != 0)
                {
                    newItem.isSeparator = true;
                    newItem.path = $"{originalItemPath}/separator{count-1}";
                    //newItem.originalPath = item.originalPath;
                }
                else
                {
                    newItem = FromMinimalItem(newItem);
                    newItem.isSeparator = false;
                }
    
                newItem.priority = i;
                processed.Add(newItem);
                i++;
            }
            
            //Assert.AreEqual(1, processed.Count(item => !item.originalPath.Contains("/")), "Menu ended up with multiple or no possible roots");
            return processed;
        }
        
        private static void InsertExtraSeparators(DataNode node)
        {
            var separators = new List<ItemData>();
            var children = node.Children();

            if (children.IsNullOrEmpty())
                return;
         
            int j = 0;
            for (int i = 0; i < children.Count-1; i++)
            {
                var item = children[i];
                var nextItem = children[i+1];

                if (nextItem.data.originalPriority - item.data.originalPriority  < 15)
                    continue;

                separators.Add(new ItemData
                {
                    path = $"{item.data.path.RemoveAfterLast("/")}/insertedSeparator{j}",
                    originalPath = $"{item.data.path.RemoveAfterLast("/")}/insertedSeparator{j}",
                    isSeparator = true,
                    originalPriority = item.data.originalPriority,
                    priority = item.data.priority+0.5f,
                });
                j++;
            }

            var nodes = separators.Select((sep, i) => new DataNode(sep)).ToList();
            separators.ForEach((sep,i) => node.Add(nodes[i]));
        }

        public static ItemData FromMinimalItem(ItemData minimal)
        {
            //var item = minimal.Copy();
            minimal.path = minimal.originalPath;
            minimal.displayName = minimal.autoDisplayName;
            
            if(minimal.iconName.IsNullOrEmpty())
                minimal.iconName = IconLoader.GetIconName(minimal.originalPath);
            
            minimal.priority = minimal.originalPriority;
            return minimal;
        }
        
        public static DataNode PasteTrees(List<DataNode> trees)
        {
            var first = trees.First();
            var rest = trees.Where(node => node != first);
            rest.ForEach(root => first.Add(root.Children().First()));
            return first;
        }

        public static bool FixTree(DataNode tree)
        {
            bool result = InsertNewNodesIntroTree(tree);
            if (Config.GetSetting<bool>("autoDeleteMenuItems"))
            {
                if (DeleteOldItems(tree))
                    result = true;
            }
            
            return result;
        }
        
        public static bool InsertNewNodesIntroTree(DataNode tree)
        {
            //must make sure not to insert unnecessary items into existing profiles if some detail about an item changes for example fixing a typo or updating unity
            
            var newTree = BuildTree();
            
            var flat = tree.Flatten();
            var newTreeUnityPaths = newTree.Flatten().Select(node => node.data.originalPath).ToList();
            var treeUnityPaths = flat.Select(node => node.data.originalPath).ToList();
            var newItems = newTreeUnityPaths.Except(treeUnityPaths).Select(path => newTree.Find(path).data).ToList();
            //todo maybe set the priority on new items to always show up
            
            if (newItems.Count == 0)
                return false;
            
            foreach (var item in newItems)
            {
                //Assert.IsFalse(item.originalPath.IsNullOrEmpty());
                var newParent = flat.FirstOrDefault(n => n.data.originalPath == item.originalPath.RemoveAfterLast("/") && !n.data.isSeparator);
                if(newParent is not null && newParent.parent is not null) 
                    item.path = $"{newParent.data.path}/{item.path.RemoveBeforeLast("/")}";
            }
            
            //sorting to guarantee we don't try to insert new children before new parents
            var sortedItems = newItems.OrderBy(item => item.path.Length).ToList();
            
            foreach (ItemData item in sortedItems)
            {
                tree.SafeInsert(item);
            }
            
            return true;
        }

        public static bool DeleteOldItems(DataNode tree)
        {
            var items = GetGoodItemList();
            
            bool result = false;
            foreach (var item in tree.Flatten())
            {
                var fixedPath = TabbyAssets.MapToUnityPath(item.data.originalPath);
                if (item.data.hasMenuItem && items.BinarySearch(fixedPath, StringComparer.Ordinal) < 0)
                {
                    item.parent.Remove(item);
                    result = true;
                }
            }

            return result;
        }

        private static List<string> GetGoodItemList()
        {
            var allItems = TypeCache.GetMethodsWithAttribute<MenuItem>().Select(t => ((MenuItem)t.GetCustomAttributes(typeof(MenuItem), true).First()).menuItem).ToList();
            var list = allItems.Where(item => !item.StartsWith("CONTEXT/")).ToList();
            list.Sort(StringComparer.Ordinal);
            return list;
        }
        
        public static List<string> RemoveShortcut(string[] options)
        {
            return options.Select(RemoveShortcut).ToList();
        }
        
        public static string RemoveShortcut(string o)
        {
            return $"{o.RemoveAfterLast(" _").RemoveAfterLast("%").RemoveAfterLast("#").RemoveAfterLast("&").RemoveTrailing(" ").RemoveTrailing("/")}";
        }
        
        private static bool IsPartOfNewMenu(ItemData data, List<ItemData> others, DataNode original)
        {
            foreach (var o in others)
            {
                if (original.Find(o.path) is null)
                    return true;
                
            }
            return others.Any(o => o.path == data.parentPath);
        }
        
        public static List<ItemData> GetDataFromUnity(string path)
        {
            return GetMenuItems(path).Select(item => new ItemData
            {
                originalPath = item.GetFieldValue<string>("m_Path"),
                executionPath = item.GetFieldValue<string>("m_Path"),
                originalPriority = item.GetFieldValue<int>("m_Priority"),
                isSeparator = item.GetFieldValue<bool>("m_IsSeparator"),
            }).ToList();
        }
        
        
        public static Func<string, List<object>> testItemGetter;
        private static List<object> GetMenuItems(string menuPath)
        {
            return testItemGetter is not null ? testItemGetter(menuPath) : BuildUnityItemPair(menuPath);
        }

        //only for testing
        public static List<object> GetMenuItemsFromUnityTestingOnly(string menuPath)
        {
            return BuildUnityItemPair(menuPath);
        }

        private static List<object> BuildUnityItemPair(string menuPath)
        {
            Assert.IsFalse(EditorUtil.doingInitializeOnLoad, "Calling on load returns broken data");
            return new []{ new UnityItemData { m_Path = menuPath }}
                .Concat(typeof(Menu).InvokeStaticMethod<Array>("GetMenuItems", menuPath, true, true).Cast<object>()).ToList();
        }
        
        private class UnityItemData
        {
            public string m_Path;
            public int m_Priority;
            public bool m_IsSeparator;

            public UnityItemData()
            {
                
            }

            public UnityItemData(string path)
            {
                m_Path = path;
            }
        }
        
        private static void FlattenChildPriorities(DataNode node)
        {
            node.Children().ForEach((c, i) => c.data.priority = i);
        }
        
        public static List<object> ExtractSubmenus(string menuPath)
        {
            return typeof(Menu).InvokeStaticMethod<Array>("ExtractSubmenus", menuPath).Cast<object>().ToList();
        }
    }
}