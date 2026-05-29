using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    //[InitializeOnLoad]
    public static class MenuDataSerializer
    {
        private static string currentProfile;
        private static DataNode allMenus;
        private static DataSaver saver = new();

        private static int saveQueue;
        
        [Setting(false)]
        private static bool onMenusModified;

        static MenuDataSerializer()
        {
            currentProfile = Config.Subscribe<string>("profile", OnProfilePathChanged);
            Init();
        }
        
        private static void OnProfilePathChanged(string newValue)
        {
            currentProfile = newValue;
            Init();
        }
        
        private static void Init()
        {
            allMenus = saver.Read();
            Assert.IsNotNull(allMenus);
            FixNewMenus();
        }
        
        public static void Save()
        {
            saver.Write(allMenus);
        }
        
        public static void DelayedSave()
        {
            saveQueue++;
            RunAsync.Fire(1000, () =>
            {
                if (saveQueue == 1)
                {
                    saveQueue = 0;
                    Save();
                }
                else
                {
                    saveQueue--;
                }
            });
        }
    
        public static MenuData GetMenu(string menuPath)
        {
            var anonymousPath = TabbyAssets.anonymousMenuPath;
            if (menuPath == anonymousPath)
            {
                if(allMenus.Find(anonymousPath) is not null)
                    allMenus.Remove(allMenus.Find(anonymousPath));
                MenuBuilder.PasteTrees(Q.L(allMenus, MenuBuilder.BuildMenu(menuPath).tree));
                return new MenuData(allMenus.FindAssert(anonymousPath));
            }
            
            if (ShouldUseNonCustomizableMenu(menuPath))
            {
                return GetNonCustomizableMenu(menuPath);
            }
    
            return new MenuData(allMenus.FindAssert(menuPath));
        }

        private static MenuData GetNonCustomizableMenu(string menuPath)
        {
            var miniTree = MenuBuilder.BuildMenu(menuPath).Find(menuPath); 
            var menu = allMenus.Find(menuPath);
            
            if (menu is null)
            {
                allMenus.Add(miniTree);
            }
            else
            {
                menu.ReplaceChildren(miniTree.Children());
            }
                
            return new MenuData(allMenus.Find(menuPath));
        }

        private static bool ShouldUseNonCustomizableMenu(string path)
        {
            return path.Step("/", true).Any(p => ExtraData.nonCustomizablePaths.Contains(p));
        }

        private static void FlattenPriorities(DataNode node)
        {
            if (node.parent is null)
                return;

            node.parent.Children().ForEach((c, i) => c.data.priority = i);
        }

        public static void Reorder(ItemData data, ItemData before, ItemData after)
        {
            var savedNode = allMenus.Find(data.path);
            FlattenPriorities(savedNode);
            var savedItem = savedNode.data;
            
            float newPriority;
            if (before is not null)
            {
                newPriority = before.priority + 0.5f;
            }
            else
            {
                newPriority = -1f;
            }
            
            savedItem.priority = newPriority;
            var oldPath = savedItem.path;
            var newPath = $"{(before ?? after).path.RemoveAfterLast("/")}/{savedItem.path.Split("/").Last()}";
            allMenus.MoveNode(oldPath, newPath);
            saver.Write(allMenus);
            OnMenusModified();
        }
    
        public static ItemData InsertUnder(ItemData callerData, bool isSeparator, bool userCreated = false)
        {
            DataNode node = allMenus.Find(callerData.parentPath);
            ItemData item = new ItemData();
            item.path = node.CreateNewPathForChild();
            item.priority = GetSqueezePriority(callerData);
            item.displayName = item.autoDisplayName;
            item.isSeparator = isSeparator;
            node.Add(new DataNode(item));
            FlattenPriorities(node);
            saver.Write(allMenus);
            OnMenusModified();
            return item;
        }
        
        public static ItemData Insert(string path)
        {
            ItemData item = new ItemData();
            item.path = path;
            DataNode node = allMenus.Find(item.parentPath);
            item.displayName = item.autoDisplayName;
            node.Add(new DataNode(item));
            FlattenPriorities(node);
            saver.Write(allMenus);
            OnMenusModified();
            return item;
        }

        private static float GetSqueezePriority(ItemData callerData)
        {
            var nextItem = GetNextItem(callerData);

            if (nextItem is null)
                return callerData.priority + 1;
            
            return (callerData.priority + nextItem.priority) / 2f;
        }

        private static ItemData GetNextItem(ItemData item)
        {
            DataNode node = allMenus.Find(item.parentPath);
            return node.ChildData().FirstOrDefault(data => data.priority >= item.priority && data.path != item.path);
        }
        
        public static void SetPropertyRecursively(ItemData data, Action<ItemData> action)
        {
            var node = allMenus.Find(data.path);
            action(node.data);

            foreach (var child in node.Children())
            {
                SetPropertyRecursively(child.data, action);
            }
            
            OnMenusModified();
            DelayedSave();
        }
        
        public static void SetProperty(ItemData data, Action<ItemData> action)
        {
            ItemData item = allMenus.Find(data.path).data;
            action(item);
            OnMenusModified();
            DelayedSave();
        }
        
        public static void FixNewMenus()
        {
            if (MenuBuilder.FixTree(allMenus))
            {
                Task.Run(() => saver.Write(allMenus));
            }
        }
    
        public static bool SwitchVisible(ItemData data)
        {
            var savedItem = allMenus.Find(data.path).data;
            savedItem.shown = !data.shown;
            saver.Write(allMenus);
            return savedItem.shown;
            
        }

        public static void NewProfile(string name)
        {
            var items = MenuBuilder.BuildTree();
            saver.Write(items,name);
        }
    
        public static void DeleteProfile(string name)
        {
            saver.DeleteProfile(name);
        }
    
        public static void DuplicateCurrentProfile(string newName)
        {
            saver.Write(allMenus,newName);
        }
        
        public static void LoadDefaultProfile()
        {
            saver.Flush();
            Config.SetSetting("profile",Profiles.defaultProfile);
            Debug.Log("Loaded default profile");
        }
        
        public static void FlushProfile()
        {
            saver.Flush();
            Debug.Log("Profile flushed");
        }

        private static void OnMenusModified()
        {
            Config.SetSettingWithoutSave(nameof(onMenusModified), true);
        }

        public static void ForceUpgrade()
        {
            allMenus = saver.Read();
            FixNewMenus();
            Save();
        }
        
    }
}