namespace TabbyStudios
{
    public class ContextIconData
    {
        private static string Prefix(string s) => TabbyContextData.Prefix(s);
        
        public static Map<string, string> iconMap = new()
        {
            {$"{Prefix("ProjectBrowser")}/Create","plus"},
            {$"{Prefix("ProjectBrowser")}/Delete","trash"},
            {$"{Prefix("ProjectBrowser")}/Refresh","refresh"},
            {$"{Prefix("ProjectBrowser")}/Reimport","refresh"},
            {$"{Prefix("ProjectBrowser")}/Reimport All","refresh"},
            {$"{Prefix("ProjectBrowser")}/Properties...","dots-horizontal"},
            {$"{Prefix("ProjectBrowser")}/Rename","edit"},
            {$"{Prefix("ProjectBrowser")}/Copy Path","copy"},
            {$"{Prefix("ProjectBrowser")}/Show in Explorer","picture-in-picture"},
            {$"{Prefix("ProjectBrowser")}/Import Package","zip-file"},
            {$"{Prefix("ProjectBrowser")}/Export Package...","zip-file"},
            {$"{Prefix("ProjectBrowser")}/Find References In Scene","search"},
            {$"{Prefix("ProjectBrowser")}/Find References In Project","search"},
            {$"{Prefix("ProjectBrowser")}/Extract From Prefab","cube-on-plane"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Folder","folder-plus"},
            {$"{Prefix("ProjectBrowser")}/Create/MonoBehaviour Script","hashtag"},
            {$"{Prefix("ProjectBrowser")}/Create/Material","sticker"},
        
            {$"{Prefix("ProjectBrowser")}/Create/2D","stop"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites","stop"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites/Triangle","triangle"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites/Square","stop"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites/Circle","circle"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites/Capsule","pill"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Sprites/Isometric Diamond","layers"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Tiles","grid"},
            {$"{Prefix("ProjectBrowser")}/Create/2D/Tile Palette","grid"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Scene","scene"},
            {$"{Prefix("ProjectBrowser")}/Create/Scene/Scene","scene"},
            {$"{Prefix("ProjectBrowser")}/Create/Scene/Prefab","cube-on-plane"},

        
            {$"{Prefix("ProjectBrowser")}/Create/Audio","speaker"},
            {$"{Prefix("ProjectBrowser")}/Create/Audio/Audio Mixer","settings-sliders"},

            {$"{Prefix("ProjectBrowser")}/Create/Scripting","code"},
            {$"{Prefix("ProjectBrowser")}/Create/Scripting/MonoBehaviour Script","hashtag"},
            {$"{Prefix("ProjectBrowser")}/Create/Scripting/Empty C# Script","hashtag"},
            {$"{Prefix("ProjectBrowser")}/Create/Scripting/Assembly Definition", "plug"},
            {$"{Prefix("ProjectBrowser")}/Create/Scripting/Assembly Definition Reference", "plug"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Testing/C# Test Script","hashtag"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Search","search"},
            {$"{Prefix("ProjectBrowser")}/Create/Timeline","film"},
            {$"{Prefix("ProjectBrowser")}/Create/Terrain","photo"},
            {$"{Prefix("ProjectBrowser")}/Create/Input Actions","keyboard"},
            {$"{Prefix("ProjectBrowser")}/Create/UI Toolkit","dashboard"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Text Core","indent-left"},
            {$"{Prefix("ProjectBrowser")}/Create/Text Core/Font Asset","f"},
        
            {$"{Prefix("ProjectBrowser")}/Create/TextMeshPro","indent-left"},
            {$"{Prefix("ProjectBrowser")}/Create/TextMeshPro/Font Asset","f"},
        
            {$"{Prefix("ProjectBrowser")}/Create/Rendering","video-camera"},
            {$"{Prefix("ProjectBrowser")}/Create/Rendering/Material","sticker"},
            {$"{Prefix("ProjectBrowser")}/Create/Rendering/Lightmap Parameters","settings-sliders"},
            {$"{Prefix("ProjectBrowser")}/Create/Rendering/Lighting Settings","settings-sliders"},
        
            {$"{Prefix("Hierarchy")}/Delete","trash"},
            {$"{Prefix("Hierarchy")}/Rename","edit"},
            {$"{Prefix("Hierarchy")}/Find References In Scene","search"},
            {$"{Prefix("Hierarchy")}/Cut","scissors"},
            {$"{Prefix("Hierarchy")}/Copy","copy"},
            {$"{Prefix("Hierarchy")}/Paste","clipboard"},
            {$"{Prefix("Hierarchy")}/Paste Special","clipboard"},
            {$"{Prefix("Hierarchy")}/Duplicate","union"},
            {$"{Prefix("Hierarchy")}/Create Empty","maximize"},
            {$"{Prefix("Hierarchy")}/Create Empty Parent","maximize"},
            {$"{Prefix("Hierarchy")}/Create Empty Child","maximize"},
            {$"{Prefix("Hierarchy")}/2D Object","stop"},
            {$"{Prefix("Hierarchy")}/3D Object","box"},
            {$"{Prefix("Hierarchy")}/Audio","speaker"},
            {$"{Prefix("Hierarchy")}/UI","dashboard"},
            {$"{Prefix("Hierarchy")}/UI Toolkit","dashboard"},
            {$"{Prefix("Hierarchy")}/Video","video"},
            {$"{Prefix("Hierarchy")}/Camera","video-camera"},
        
            {$"{Prefix("Hierarchy")}/Effects","spinner"},
            {$"{Prefix("Hierarchy")}/Effects/Particle System","spinner"},
        
            {$"{Prefix("Hierarchy")}/Light","sun"},
            {$"{Prefix("Hierarchy")}/Light/Point Light","sun"},
            
            {$"{Prefix("Hierarchy")}/2D Object/Sprites","stop"},
            {$"{Prefix("Hierarchy")}/2D Object/Sprites/Triangle","triangle"},
            {$"{Prefix("Hierarchy")}/2D Object/Sprites/Square","stop"},
            {$"{Prefix("Hierarchy")}/2D Object/Sprites/Circle","circle"},
            {$"{Prefix("Hierarchy")}/2D Object/Sprites/Capsule","pill"},
            {$"{Prefix("Hierarchy")}/2D Object/Sprites/Isometric Diamond","layers"},
        
            {$"{Prefix("Hierarchy")}/2D Object/Tilemap","grid"},
            {$"{Prefix("Hierarchy")}/2D Object/Tilemap/Rectangular","stop"},
            {$"{Prefix("Hierarchy")}/2D Object/Tilemap/Isometric","layers"},
            {$"{Prefix("Hierarchy")}/2D Object/Tilemap/Isometric Z as Y","layers"},
            
            {$"{Prefix("Hierarchy")}/Prefab","cube-on-plane"},
        
            {$"{Prefix("Hierarchy")}/3D Object/Cube","box"},
            {$"{Prefix("Hierarchy")}/3D Object/Sphere","globe"},
            {$"{Prefix("Hierarchy")}/3D Object/Cylinder","cylinder"},
            {$"{Prefix("Hierarchy")}/3D Object/Capsule","pill"},
            {$"{Prefix("Hierarchy")}/3D Object/Plane","stop"},
            {$"{Prefix("Hierarchy")}/3D Object/Quad","stop"},
            {$"{Prefix("Hierarchy")}/3D Object/Terrain","photo"},
            {$"{Prefix("Hierarchy")}/3D Object/Tree","green1"},
            
            {$"{Prefix("SceneView")}/Delete","trash"},
            {$"{Prefix("SceneView")}/Cut","scissors"},
            {$"{Prefix("SceneView")}/Copy","copy"},
            {$"{Prefix("SceneView")}/Paste","clipboard"},
            {$"{Prefix("SceneView")}/Duplicate","union"},
            {$"{Prefix("SceneView")}/Add Component...","plus"},
            {$"{Prefix("SceneView")}/Move to Grid Position","grid"},
            {$"{Prefix("SceneView")}/Move to View","cube-view"},
            {$"{Prefix("SceneView")}/Align with View","cube-view"},
            {$"{Prefix("SceneView")}/Properties...","dots-horizontal"},
            
        };
    }
}