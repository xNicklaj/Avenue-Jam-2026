using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class HierarchyData
    {
        private static string menuPath = TabbyContextData.Prefix("Hierarchy");
        
        public static List<ItemData> HierarchyItemData()
        {
            var data = MenuBuilder.GetDataFromUnity("GameObject");
            UpgradeItems(data);
            var processedData = MenuBuilder.ProcessUnityData(data);
            
            RemoveUnwantedHierarchyItems(processedData);
            AddExtraHierarchyItems(processedData);
            return processedData;
        }
        
        private static void RemoveUnwantedHierarchyItems(List<ItemData> items)
        {
            items.RemoveAll(item => unwantedGameObjectItems.Contains(item.path));
        }

        private static void UpgradeItems(List<ItemData> data)
        {
            foreach (var item in data)
            {
                foreach (var field in Q.A("path", "originalPath", "executionPath"))
                {
                    var value = item.GetFieldValue<string>(field);
                    if (value.StartsWith("GameObject"))
                    {
                        var name = value.RemoveAfterFirst("/");
                        item.SetFieldValue(field, value.ReplaceFirst(name, menuPath));
                    }
                }
            }
        }
    
        private static void AddExtraHierarchyItems(List<ItemData> items)
        {
            var editItems = MenuBuilder.GetItemData("Edit");
            var chosen = editItems.Where(item => editItemsToKeep.Keys.Any(k => k.Split("/")[^1] == item.path.Split("/")[^1])).ToList();
            chosen = chosen.Select(c => MenuBuilder.FromMinimalItem(TransformEditItem(c, editItemsToKeep))).ToList();
            items.AddRange(chosen);
            var extraData = ExtraHierarchyItemList(items);
            extraData = extraData.Select(MenuBuilder.FromMinimalItem).ToList();
            
            items.AddRange(extraData);
        
            items.Sort((i1,i2) => (int)i1.priority == (int)i2.priority ? 0 : (i1.priority < i2.priority ? -1 : 1));
        }
        
        public static ItemData TransformEditItem(ItemData item, Dictionary<string,float> keep)
        {
            var transformed = item.Copy();
            transformed.originalPath = item.originalPath.ReplaceFirst("Edit/", $"{menuPath}/");
            transformed.path = transformed.originalPath;
            transformed.originalPriority = keep[transformed.path];
            transformed.priority = keep[transformed.path];
            transformed = MenuBuilder.FromMinimalItem(transformed);
            return transformed;
        }
        
        private static List<string> unwantedGameObjectItems => new()
        {
            $"{menuPath}/Center On Children",
            $"{menuPath}/Set as first sibling",
            $"{menuPath}/Set as last sibling",
            $"{menuPath}/Create Empty Child",
        };
    
        private static Dictionary<string,float> editItemsToKeep => new ()
        {
            {$"{menuPath}/Cut",-99},
            {$"{menuPath}/Copy",-98},
            {$"{menuPath}/Paste",-97},
            {$"{menuPath}/Paste Special",-96},
            {$"{menuPath}/Rename",-95},
            {$"{menuPath}/Duplicate",-94},
            {$"{menuPath}/Delete",-93},
            {$"{menuPath}/Select All",-50},
            {$"{menuPath}/Deselect All",-49},
            {$"{menuPath}/Invert Selection",-48},
            {$"{menuPath}/Select Children",-47},
            {$"{menuPath}/Paste Special/Paste as Child (Keep Local Transform)",0},
            {$"{menuPath}/Paste Special/Paste as Child (Keep World Transform)",1},
        };
        
        public static List<ItemData> ExtraHierarchyItemList(List<ItemData> others)
        {
            List<ItemData> list = new();
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Find References In Scene",
                executionPath = "GameObject/Find References In Scene",
                originalPriority = others.First(item => item.path.Contains("Select Children")).priority + 16,
            });        
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Set As Default Parent",
                executionPath = "GameObject/Set As Default Parent",
                originalPriority = others.First(item => item.path.Contains("Select Children")).priority + 32,
            }); 
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab",
                originalPriority = others.First(item => item.path.Contains("Select Children")).priority + 33,
            }); 
    
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Open Asset In Context",
                executionPath =  "GameObject/Prefab/Open Asset In Context",
                originalPriority = 0,
            }); 
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Open Asset In Isolation",
                executionPath =  "GameObject/Prefab/Open Asset In Isolation",
                originalPriority = 1,
            }); 
    
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "GameObject/Prefab/separator1",
                originalPriority = 2,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Select Asset",
                executionPath = "GameObject/Prefab/Select Asset",
                originalPriority = 3,
            }); 
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Select Root",
                executionPath = "GameObject/Prefab/Select Root",
                originalPriority = 4,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "GameObject/Prefab/separator2",
                originalPriority = 5,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Replace",
                executionPath = "GameObject/Prefab/Replace",
                originalPriority = 6,
            }); 
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Replace And Keep Overrides",
                executionPath = "GameObject/Prefab/Replace And Keep Overrides",
                originalPriority = 7,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = true,
                originalPath = "GameObject/Prefab/separator3",
                originalPriority = 8,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Unpack",
                executionPath =  "GameObject/Prefab/Unpack",
                originalPriority = 10,
            });
    
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Prefab/Unpack Completely",
                executionPath =  "GameObject/Prefab/Unpack Completely",
                originalPriority = 11,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Added GameObject",
                originalPriority = list.First(item => item.originalPath == "GameObject/Prefab").originalPriority + 1,
            });

            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Added GameObject/Apply to Prefab",
                executionPath = "GameObject/Added GameObject/Apply to Prefab",
                originalPriority = 0,
            });
            
            list.Add(new ItemData()
            {
                isSeparator = false,
                originalPath = "GameObject/Added GameObject/Revert",
                executionPath = "GameObject/Added GameObject/Revert",
                originalPriority = 1,
            });
            

            UpgradeItems(list);
            
            return list;
        }
    }
}