using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public sealed class DataNode
    {
        private List<DataNode> children = new();
        private Map<string, DataNode> childrenDict = new();
        
        public DataNode parent { get; private set; }
        public int startingDepth;
        public bool isRoot; //todo ensure is no longer root when added
        public int Count => Flatten().Count;

        public const string rootPath = "";
        
        private ItemData _data;
        public ItemData data
        {
            get => _data;
            private set
            {
                Assert.IsNotNull(value);
                if(!isRoot)
                    Assert.IsFalse(value.path.IsNullOrEmpty());
                _data = value;
            }
        }
        
        private DataNode()
        {
            
        }
        
        public DataNode(ItemData data)
        {
            this.data = data;
        }
        
        public DataNode(ItemData data, int startingDepth)
        {
            this.data = data;
            this.startingDepth = startingDepth;
        }

        public static DataNode CreateRoot()
        {
            var node = new DataNode();
            node.isRoot = true;
            node.data = new ItemData() { path = "" };
            return node;
        }
        
        public DataNode FromList(List<ItemData> list)
        {
            Assert.IsTrue(this.isRoot, "FromList must be called from a root node");
            list = list.OrderBy(t => t.path.Length).ToList();
            var first = list.First();
            if (first.path == "")
            {
                data = first;
                list.RemoveAt(0);
            }
            
            foreach (var item in list)
            {
                SafeInsert(item);
            }

            return this;
        }

        public DataNode Insert(ItemData data)
        {
            var newNode = new DataNode(data);
            var parentPath = data.parentPath;
            var newParent = Find(parentPath);
            if (newParent is null)
                 throw new Exception($"No parent for node with path {newNode.data.path}");
            newParent.Add(newNode);
            return newNode;
        }
        
        public DataNode SafeInsert(ItemData data)
        {
            var steps = data.path.Step("/");
            foreach (var step in steps)
            {
                if (Find(step) is null)
                    Insert(new ItemData { path = step });
            }

            return Insert(data);
        }
        
        
        public void MoveNode(string oldPath, string newPath)
        {
            if (oldPath == newPath)
                return;
        
            var existing = Find(newPath);
            var oldParentPath = oldPath.RemoveAfterLast("/");
        
            var newFixedPath = newPath;
            if (existing is not null)
                newFixedPath = $"{newPath}__duplicate__{existing.parent.Children().Count(c => c.data.key == newPath.Split("/").Last())}";
        
        
            var node = Root().Find(oldPath);
        
            node.parent.Remove(node);
            node.data.path = newFixedPath;
        
            var newParentPath = newFixedPath.RemoveAfterLast("/");
            var newParent = Find(newParentPath);
            newParent.Add(node);
            node.Flatten().Where(n => n != node).ForEach(n => n.data.path =  $"{StringExtensions.ReplaceFirst(n.data.path, oldParentPath+"/",newParentPath+"/")}");

        }
        
        public DataNode Find(string path)
        {
            return InternalFind(path, Depth());
        }
        
        public DataNode FindAssert(string path)
        {
            var result = Find(path);
            Assert.IsNotNull(result, $"Couldn't find {path} in tree");
            return result;
        }
        
        private DataNode InternalFind(string path, int depth)
        {
            if (path == data.path) return this;
            var step = path.RemoveAfterOccurence('/', depth + 1);
            var stept = step.AddTrailing("/");
            var c = children.FirstOrDefault(c => c.data.path.AddTrailing("/").StartsWith(stept));
            return c?.InternalFind(path, depth + 1);
        }
        
        public DataNode Add(DataNode child)
        {
            child.parent = this;
            child.data.node = child;
            if (childrenDict.ContainsKey(child.data.key))
                children.Remove(c => c.data.key == child.data.key);
            children.Add(child);
            childrenDict[child.data.key] = child;
            
            return child;
        }

        public void Remove(DataNode child)
        {
            if (!childrenDict.ContainsKey(child.data.key))
                return;
            
            children.Remove(child);
            childrenDict.Remove(child.data.key);
            child.parent = null;
        }
        
        public List<DataNode> Flatten()
        {
            var result = new List<DataNode> { this };
            foreach (var child in children)
            {
                result.AddRange(child.Flatten());
            }
            return result;
        }
        
        public DataNode Root()
        {
            return isRoot ? this : parent.Root();
        }

        private bool DescendsFromNodeWithStartingDepth()
        {
            return parent is null ? false : parent.startingDepth != 0 ? true : parent.DescendsFromNodeWithStartingDepth();
        }

        public List<DataNode> Children()
        {
            return children.OrderBy(c => c.data.priority).ToList();
        }
        
        public void ForEach(Action<DataNode> action)
        {
            action(this);
            Children().ForEach(c => c.ForEach(action));
        }
        
        public List<ItemData> ChildData()
        {
            return Children().Select(child => child.data).ToList();
        }
        
        public string CreateNewPathForChild()
        {
            return $"{data.path}/{CreateNewNameForChild()}";
        }
    
        public string CreateNewNameForChild()
        {
            string prefix = "New Item";
            return $"{prefix} {Children().Count(c => c.data.key.RemoveDigits().RemoveTrailing(" ").EndsWith(prefix))}";
        }
        
        public void ReplaceChildren(List<DataNode> list)
        {
            this.children = list;
            this.childrenDict = new Map<string, DataNode>(list.Select(n => (n.data.key, n)));
        }

        public int Depth()
        {
            return parent is null ? 0 : 1 + parent.Depth();
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}