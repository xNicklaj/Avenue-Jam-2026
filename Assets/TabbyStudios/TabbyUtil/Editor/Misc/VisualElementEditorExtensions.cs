using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public static class VisualElementEditorExtensions
    {
        public static EditorWindow ContainingWindow(this VisualElement e)
        {
            var root = e.Root();
            return UnityWindows.GetWindows<EditorWindow>().FirstOrDefault(w => w.rootVisualElement == root);
        } 
    }
}