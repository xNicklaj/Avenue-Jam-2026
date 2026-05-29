using System;

namespace TabbyStudios
{
    public static class ContextItemShowConditions
    {
        private static string Prefix(string s) => TabbyContextData.Prefix(s);
        
        public static Map<string, Func<bool>> defaultConditions = new()
        {
            {$"{Prefix("Hierarchy")}/Move To View", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Align With View", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Align View to Selected", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Move View to Selected", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Toggle Active State", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Clear Parent", EditorItemShowCondition.HasParent},
            {$"{Prefix("Hierarchy")}/Set as first sibling", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Find References In Scene", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Set As Default Parent", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Make Parent", EditorItemShowCondition.NoChildParent},
            {$"{Prefix("Hierarchy")}/Create Empty Parent", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("Hierarchy")}/Create Empty Child", EditorItemShowCondition.AnythingSelected},
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
            
            {$"{Prefix("SceneView")}/Transform", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("SceneView")}/Prefab", EditorItemShowCondition.PrefabSelected},
            
            {$"{Prefix("ProjectBrowser")}/Open", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Rename", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Open Scene Additive", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/View in Package Manager", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Find References in Scene", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Find References in Project", EditorItemShowCondition.AnythingSelected},
            {$"{Prefix("ProjectBrowser")}/Create/Prefab Variant", EditorItemShowCondition.AnythingSelected},
        };
    }
}