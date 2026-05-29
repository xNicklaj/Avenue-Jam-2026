using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public static class EditorWindowExtensions
    {
        private static float barHeight = 25;
        
        public static Vector2 Position(this EditorWindow w)
        {
            return w.Rect().position;
        }
        
        public static Vector2 Size(this EditorWindow w)
        {
            return w.Rect().size;
        }
        
        public static Rect Rect(this EditorWindow w)
        {
            if (w.GetFieldValue("m_Parent")?.GetType().Name == "DockArea")
            {
                return new Rect(w.position.position.AddY(barHeight), w.position.size);
            }

            return w.position;
        }
        
        public static void Destroy(this EditorWindow w)
        {
            try
            {
                w.Close();
            }
            catch
            {
                Object.DestroyImmediate(w);
            }
        }
    }
}