using UnityEditorInternal;
using UnityEngine;

namespace TabbyStudios
{
    public class ScreenPixels
    {
        public static Vector2 minWindowSize;
        
        static ScreenPixels()
        {
            #if UNITY_2022_1_OR_NEWER
            minWindowSize = new Vector2(100, 50);
            #else
            minWindowSize = new Vector2(100,100);
            #endif
        }
        
        public static Texture2D ReadPixels(Rect rect)
        {
            int width = (int)rect.width;
            int height = (int)rect.height;
            int x = (int)rect.x;
            int y = (int)rect.y;

            Color[] pixels = InternalEditorUtility.ReadScreenPixel(new Vector2(x, y), width, height);
            
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.SetPixels(pixels);
            texture.Apply(false);
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
            return texture;
        }
    }
    
}