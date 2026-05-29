using System;

namespace TabbyStudios
{
    public static class ContextItemShowConditions
    {
        private static string Prefix(string s) => TabbyContextData.Prefix(s);
        
        public static Map<string, Func<bool>> defaultConditions = new()
        {
            {$"{Prefix("Hierarchy")}/Move To View", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Align With View", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Align View to Selected", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Move View to Selected", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Toggle Active State", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Clear Parent", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Set as first sibling", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Find References In Scene", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Set As Default Parent", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Make Parent", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Create Empty Parent", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Create Empty Child", EditorItemShowCondition.GameObjectsAreSelected },
            {$"{Prefix("Hierarchy")}/Prefab", EditorItemShowCondition.PartOfPrefabSelected},
            {$"{Prefix("Hierarchy")}/Prefab/Open Asset In Context", EditorItemShowCondition.HasOnePartOfPrefab},
            {$"{Prefix("Hierarchy")}/Prefab/Open Asset In Isolation", EditorItemShowCondition.HasOnePartOfPrefab},
            {$"{Prefix("Hierarchy")}/Prefab/Select Asset", EditorItemShowCondition.HasOnePartOfPrefab},
            {$"{Prefix("Hierarchy")}/Prefab/Select Root", EditorItemShowCondition.PartOfPrefabSelected},
            {$"{Prefix("Hierarchy")}/Prefab/Unpack", EditorItemShowCondition.PrefabSelected},
            {$"{Prefix("Hierarchy")}/Prefab/Unpack Completely", EditorItemShowCondition.PrefabSelected},
            {$"{Prefix("Hierarchy")}/Prefab/Replace", EditorItemShowCondition.PrefabSelected},
            {$"{Prefix("Hierarchy")}/Prefab/Replace And Keep Overrides", EditorItemShowCondition.PrefabSelected},
            {$"{Prefix("Hierarchy")}/Added GameObject", EditorItemShowCondition.IsAddedGameObject},
            {$"{Prefix("Hierarchy")}/Added GameObject/Apply to Prefab", EditorItemShowCondition.IsAddedGameObject},
            {$"{Prefix("Hierarchy")}/Added GameObject/Revert", EditorItemShowCondition.IsAddedGameObject},
            
            {$"{Prefix("Hierarchy")}/Cut", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Copy", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Rename", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Paste", EditorItemShowCondition.HasClipboard},
            {$"{Prefix("Hierarchy")}/Paste Special", EditorItemShowCondition.CanPasteSpecial},
            {$"{Prefix("Hierarchy")}/Duplicate", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Delete", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Deselect All", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Invert Selection", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Select Children", EditorItemShowCondition.GameObjectsAreSelected},
            {$"{Prefix("Hierarchy")}/Properties...", EditorItemShowCondition.GameObjectsAreSelected},
            
            {$"{Prefix("SceneView")}/Transform", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("SceneView")}/Prefab", EditorItemShowCondition.PrefabSelected},
            
            {$"{Prefix("ProjectBrowser")}/Open", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Rename", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Open Scene Additive", EditorItemShowCondition.NotFolderSelected},
            {$"{Prefix("ProjectBrowser")}/Create UPM Package...", EditorItemShowCondition.NotFolderSelected},
            {$"{Prefix("ProjectBrowser")}/Export As UPM Package...", EditorItemShowCondition.NotFolderSelected},
            {$"{Prefix("ProjectBrowser")}/View in Package Manager", EditorItemShowCondition.NotFolderSelected },
            {$"{Prefix("ProjectBrowser")}/Extract Material SubAsset", EditorItemShowCondition.NotFolderSelected },
            {$"{Prefix("ProjectBrowser")}/Find References in Scene", EditorItemShowCondition.NotFolderSelected },
            {$"{Prefix("ProjectBrowser")}/Find References in Project", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Create/Prefab Variant", EditorItemShowCondition.AnythingSelected},
        };
    }
}