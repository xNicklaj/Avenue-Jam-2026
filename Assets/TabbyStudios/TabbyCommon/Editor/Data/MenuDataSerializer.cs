using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public class MenuDataSerializer
    {
        private DataSaver saver;
        private Profiles profiles;
        private int saveQueue;
        public DataNode allMenus;

        public MenuDataSerializer(Profiles profiles)
        {
            this.profiles = profiles;
            saver = new(profiles);
            Init();
        }

        public void OnProfilePathChanged()
        {
            Init();
        }

        public void Init()
        {
            allMenus = saver.GetOrCreateTree();
            Assert.IsNotNull(allMenus);
            FixNewMenus();
        }

        public void Import(string path)
        {
            var name = TabbyFiles.GetFileNameWithoutExtension(path);
            var json = File.ReadAllText(path);
            var tree = saver.Deserialize(json);
            profiles.CreateProfile(name, tree);
            profiles.ChangeProfile(name);
        }

        public void Save()
        {
            saver.Write(allMenus);
        }

        public void DelayedSave()
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

        public MenuData GetMenu(string menuPath)
        {
            var anonymousPath = TabbyAssets.anonymousMenuPath;
            if (menuPath == anonymousPath)
            {
                if (allMenus.Find(anonymousPath) is not null)
                    allMenus.Remove(allMenus.Find(anonymousPath));
                MenuBuilder.PasteTrees(new List<DataNode> { allMenus, MenuBuilder.BuildMenu(menuPath).tree });
                return new MenuData(allMenus.FindAssert(anonymousPath));

                //return new MenuData(MenuBuilder.BuildMenu(menuPath).tree.Children().First());
            }

            if (ShouldUseNonCustomizableMenu(menuPath))
            {
                return GetNonCustomizableMenu(menuPath);
            }

            return new MenuData(allMenus.FindAssert(menuPath));
        }

        private MenuData GetNonCustomizableMenu(string menuPath)
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

        private bool ShouldUseNonCustomizableMenu(string path)
        {
            return path.Step("/", true).Any(p => ExtraData.nonCustomizablePaths.Contains(p));
        }

        private void FlattenPriorities(DataNode node)
        {
            if (node.parent is null)
                return;

            node.parent.Children().ForEach((c, i) => c.data.priority = i);
        }

        public void Reorder(ItemData data, ItemData before, ItemData after)
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

        public ItemData InsertUnder(ItemData callerData, bool isSeparator, bool userCreated = false)
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

        public ItemData Insert(string path)
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

        private float GetSqueezePriority(ItemData callerData)
        {
            var nextItem = GetNextItem(callerData);

            if (nextItem is null)
                return callerData.priority + 1;

            return (callerData.priority + nextItem.priority)/2f;
        }

        private ItemData GetNextItem(ItemData item)
        {
            DataNode node = allMenus.Find(item.parentPath);
            return node.ChildData().FirstOrDefault(data => data.priority >= item.priority && data.path != item.path);
        }

        public void SetPropertyRecursively(ItemData data, Action<ItemData> action)
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

        public void SetProperty(ItemData data, Action<ItemData> action)
        {
            ItemData item = allMenus.Find(data.path).data;
            action(item);
            DelayedSave();
        }

        public void FixNewMenus()
        {
            if (MenuBuilder.FixTree(allMenus))
            {
                Task.Run(() => saver.Write(allMenus));
            }
        }

        public bool SwitchVisible(ItemData data)
        {
            var savedItem = allMenus.Find(data.path).data;
            savedItem.shown = !data.shown;
            saver.Write(allMenus);
            return savedItem.shown;
        }

        public void DuplicateCurrentProfile(string newName)
        {
            saver.Write(allMenus, newName);
        }

        public void LoadDefaultProfile()
        {
            profiles.DeleteProfile(Profiles.defaultProfile);
            profiles.CreateProfile(Profiles.defaultProfile);
            profiles.ChangeProfile(Profiles.defaultProfile);
            Debug.Log("Loaded default profile");
        }

        public void FlushProfile()
        {
            //restore
            Debug.Log("Profile flushed");
        }

        private void OnMenusModified()
        {
            SettingsMenuManager.StaticRefresh();
        }

        public void ForceUpgrade()
        {
            allMenus = saver.GetOrCreateTree();
            FixNewMenus();
            Save();
        }
    }
}