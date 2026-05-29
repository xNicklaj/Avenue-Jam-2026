using System;
using System.Linq;
using UnityEngine;

namespace TabbyStudios
{
    [Serializable]
    public class ItemData
    {
        public string originalPath = "";
        public string executionPath = "";

        public bool shown = true;
        public bool deleted = false;
        public bool isSeparator = false;
        public bool useGradient = false;
        public bool hasMenuItem = false;
        
        public string displayName = "";
        public string path = "";
        public string showWhen = "";
        public string iconName = "";
        
        public Color iconColor;
        public Color backgroundColor;
        
        public float priority = 0;
        
        [NonSerialized] public float originalPriority = 0;
        [NonSerialized] public DataNode node;
        [NonSerialized] public bool customizable;
        
        public string parentPath => ParentPath();
        private string ParentPath()
        {
            var p = path.RemoveAfterLast("/");
            //Assert.AreNotEqual(path, p);
            return path.Contains("/") ? p : "";
        }
        
        
        public string autoDisplayName => path.RemoveBeforeLast("/");
        
        public bool IsChildPath(string childPath) => childPath.Contains(path + "/") || childPath == path;
        public bool hasChildren => node.Children().Where(c => !c.data.deleted).Count() > 0;
        public string key => path.Split("/").Last();

        public ItemData Copy()
        {
            var newItem =  JsonUtility.FromJson<ItemData>(JsonUtility.ToJson(this));
            newItem.originalPriority = originalPriority;
            return newItem;
        }
        
        public override string ToString()
        {
            return displayName.IsNullOrEmpty() ? autoDisplayName : displayName;
        }
        
    }
}