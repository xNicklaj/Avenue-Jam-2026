using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace TabbyStudios
{
    public class DataSaver
    {
        public string currentProfilePath => TabbyCommonFiles.ProfilePath(Profiles.currentProfile);
        
        public DataNode Read()
        {
            if(!File.Exists(currentProfilePath))
            {
                var items = MenuBuilder.BuildTree();
                Write(items);
                return items;
            }

            return Deserialize(File.ReadAllText(currentProfilePath));
        }
        
        public void Write(DataNode tree)
        {
            Write(tree, Profiles.currentProfile);
        } 
        
        public void Write(DataNode tree, string name)
        {
            var path = TabbyCommonFiles.ProfilePath(name);
            var items = tree.Flatten().Select(item => item.data).Where(data => !data.path.StartsWith(TabbyAssets.anonymousMenuPath)).ToList();
            string result = SafeJson.ToJson(new Profile(items, TabbyCommonFiles.version));
            Directory.CreateDirectory(TabbyCommonFiles.profilesFolder);
            File.WriteAllText(path,result);
            
            AssetDatabase.Refresh();
        }
        
        public void DeleteProfile(string name)
        {
            var path = TabbyCommonFiles.ProfilePath(name);
            if(File.Exists(path))
            {
                File.Delete(path);
                AssetDatabase.Refresh();
            }
        }

        public void Flush()
        {
            if(File.Exists(TabbyCommonFiles.ProfilePath(Profiles.defaultProfile)))
            {
                File.Delete(TabbyCommonFiles.ProfilePath(Profiles.defaultProfile));
                AssetDatabase.Refresh();
            }
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
                Config.SetSetting("settingsMenuZoom", 1f);
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
            foreach (var field in Q.A("path", "originalPath", "executionPath"))
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