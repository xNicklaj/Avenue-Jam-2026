using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class SceneViewData
    {
        public static List<ItemData> SceneViewItemData()
        {
            List<ItemData> list = new();
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView",
                originalPriority = 0,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Cut",
                executionPath = "SceneView/Cut",
                originalPriority = 0,
            });

            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Copy",
                executionPath = "SceneView/Copy",
                originalPriority = 1,
            });

            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Paste",
                executionPath = "SceneView/Paste",
                originalPriority = 2,
            });

            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Duplicate",
                executionPath = "SceneView/Duplicate",
                originalPriority = 3,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Delete",
                executionPath = "SceneView/Delete",
                originalPriority = 4,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "SceneView/separator1",
                originalPriority = 5,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Move to View",
                executionPath = "SceneView/Move to View",
                originalPriority = 6,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Align with View",
                executionPath = "SceneView/Align with View",
                originalPriority = 7,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Move to Grid Position",
                executionPath = "SceneView/Move to Grid Position",
                originalPriority = 8,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "SceneView/separator2",
                originalPriority = 9,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Isolate",
                executionPath = "SceneView/Isolate",
                originalPriority = 10,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "SceneView/separator3",
                originalPriority = 11,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Add Component...",
                executionPath = "SceneView/Add Component...",
                originalPriority = 12,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "SceneView/Properties...",
                executionPath = "SceneView/Properties...",
                originalPriority = 13,
            });
            
            // list.Add(new ItemData()
            // {
            //     isSeparator = true,
            //     originalPath = "SceneView/separator4",
            //     originalPriority = 14,
            // });
            //
            // list.Add(new ItemData()
            // {
            //     isSeparator = false,
            //     originalPath = "SceneView/Prefab",
            //     executionPath = "SceneView/Prefab",
            //     originalPriority = 15,
            // });
            //
            // list.Add(new ItemData()
            // {
            //     isSeparator = true,
            //     originalPath = "SceneView/separator5",
            //     originalPriority = 16,
            // });
            //
            // list.Add(new ItemData()
            // {
            //     isSeparator = false,
            //     originalPath = "SceneView/Transform",
            //     executionPath = "SceneView/Transform",
            //     originalPriority = 17,
            // });

            foreach (var item in list)
            {
                item.originalPath = TabbyContextData.Prefix(item.originalPath);
                item.executionPath = TabbyContextData.Prefix(item.executionPath);
            }
            
            return list.Select(MenuBuilder.FromMinimalItem).ToList();
        }
    }
}