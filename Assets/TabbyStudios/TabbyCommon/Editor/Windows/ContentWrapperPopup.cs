using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ContentWrapperPopup<T> : TabbyWindow<T> where T : ContentWrapperPopup<T>
    {
        public virtual string xmlName => "";
        public object data { get; set; }

        public Vector2 windowPos;
        
        public static T Create(Vector2 pos, object data)
        {
            T window = Create();
            window.data = data;
            window.OnCreate(pos);
            return window;
        }

        public override void OnCreate(Vector2 pos)
        {
            defaultMode = Mode.Popup;
            windowPos = pos;
            rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            VisualElement uxml = AssetCache.LoadXml(xmlName);
            ProcessData(uxml);
            rootVisualElement.AddElement(uxml);
            Display();
        }
        
    
        public virtual void ProcessData(VisualElement r)
        {

        }

        public override void OnLostFocus()
        {
            Dispose();
        }
    
        public void OnGeometryChanged(GeometryChangedEvent e)
        {
            var style = rootVisualElement.Children().First().resolvedStyle;
            position = new Rect(windowPos, new Vector2(style.width,style.height));
        }
        
    }
    
}