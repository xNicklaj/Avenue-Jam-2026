using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class DataSaver
    {
        private Profiles profiles;

        public DataSaver(Profiles profiles)
        {
            this.profiles = profiles;
        }

        public DataNode GetOrCreateTree()
        {
            var data = profiles.GetData(profiles.currentProfile);

            if (data.IsNullOrEmpty())
            {
                var items = MenuBuilder.BuildTree();
                Write(items);
                return items;
            }
            
            return Deserialize(data);
        }
        
        public void Write(DataNode tree)
        {
            Write(tree, profiles.currentProfile);
        } 
        
        public void Write(DataNode tree, string name)
        {
            var items = tree.Flatten().Select(item => item.data).Where(data => !data.path.StartsWith(TabbyAssets.anonymousMenuPath)).ToList();
            string result = SafeJson.ToJson(new Profile(items, TabbyCommonFiles.version));
            
            profiles.SaveData(name, result);
        }
        

        public DataNode Deserialize(string json)
        {
            var profile = SafeJson.FromJson<Profile>(json);
            if (Upgrade(profile))
            {
                var tree = DataNode.CreateRoot().FromList(profile.list);
                Write(tree);
            }
            return DataNode.CreateRoot().FromList(profile.list);
        }

        private bool Upgrade(Profile profile)
        {
            if (profile.version == TabbyCommonFiles.version)
                return false;
            
            var toRemove = new List<ItemData>();

            var rootSeen = false;
            foreach (var item in profile.list)
            {
                if (item.path == "_TABBY_ROOT" || item.originalPath == "_TABBY_ROOT")
                {
                    if (rootSeen)
                    {
                        toRemove.Add(item);
                    }
                    else
                    {
                        item.path = "";
                        item.originalPath = "";
                        rootSeen = true;
                    }
                }
            }

            if (profile.profileVersion < "1.5.2")
            {
                foreach (var item in profile.list)
                {
                    MoveToContextMenu(item);
                }
            }
            
            if (profile.profileVersion < "1.8.2")
            {
                Config.instance.Set("settingsMenuZoom", 1f);
            }
            
            profile.SetVersion(TabbyCommonFiles.version);
            profile.list.RemoveRange(toRemove);
            
            return true;
        }

        private static Map<string, string> remap = new()
        {
            { "GameObject", "Hierarchy" },
            { "Assets", "ProjectBrowser" },
            { "SceneView", "SceneView" },
        };
        
        private void MoveToContextMenu(ItemData item)
        {
            foreach (var field in new[]{"path", "originalPath", "executionPath"})
            {
                var value = item.GetFieldValue<string>(field);
                if (remap.Keys.Any(a => value.StartsWith(a)))
                {
                    var name = value.RemoveAfterFirst("/");
                    item.SetFieldValue(field, value.ReplaceFirst(name, RemapOnMoveToContextMenu(name)));
                }
            }
        }

        private string RemapOnMoveToContextMenu(string path)
        {
            return $"{TabbyAssets.extraMenuPrefix}{(path == "GameObject" ? "Hierarchy" : path == "Assets" ? "ProjectBrowser" : path)}";
        }
    }
}