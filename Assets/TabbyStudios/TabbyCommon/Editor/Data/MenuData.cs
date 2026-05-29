using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    [Serializable]
    public class MenuData
    {
        public DataNode tree;
        public List<ItemData> items =>  tree.ChildData().Where(item => !item.deleted).ToList();
        public List<ItemData> shownItems => tree.ChildData().Where(item => item.shown && !item.deleted).ToList();
        public List<ItemData> downwards => tree.Flatten().Where(node => node.data is not null).Select(node => node.data).ToList();
        
        public MenuData(DataNode tree)
        {
            Assert.IsNotNull(tree);
            this.tree = tree;
        }

        public MenuData()
        {
            
        }

        public DataNode Find(string menuPath)
        {
            return tree.Find(menuPath);
        }
    }
}