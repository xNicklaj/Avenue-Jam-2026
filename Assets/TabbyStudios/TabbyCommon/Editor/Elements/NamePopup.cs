using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class NamePopup : TabbyWindow<NamePopup>
    {
        private static Vector2 windowPos;
    
        public static void ShowWindow(ProfilesTabContent caller,string oldName,VisualElement callerItem)
        {
            var window = CreateInstance<NamePopup>();
            window.positioner = new WindowPositioner(window);
            windows.Add(window);
            
            window.defaultMode = Mode.Popup;
            windowPos = callerItem.worldTransform.GetPosition();
            window.position = new Rect(windowPos,Vector2.zero);
            var content = AssetCache.LoadXml("NamePopup");
            window.rootVisualElement.Add(content);
            window.rootVisualElement.RegisterCallback<GeometryChangedEvent>(e => window.OnGeometryChanged());
            var textField = content.Q<TextField>();
            content.Q<Button>().clicked += () =>
            {
                window.Dispose();
                caller.FinishRename(oldName, textField.text);
            };
        
            window.Display();
            window.Focus();
        }
    
        public void OnGeometryChanged()
        {
            position = new Rect(new(800,400), new Vector2(200,70));
        }

        public override void OnLostFocus()
        {
            Dispose();
        }
    }
}