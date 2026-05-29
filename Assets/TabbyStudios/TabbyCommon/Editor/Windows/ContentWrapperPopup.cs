using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ContentWrapperPopup<T> : TabbyWindow<T> where T : ContentWrapperPopup<T>
    {
        public virtual string xmlName => "";
        public object data { get; set; }
        
        public static T Create(Vector2 pos, object data)
        {
            T window = CreateInstance();
            window.pos = pos;
            window.data = data;
            window.OnCreate();
            return window;
        }

        public override void OnCreate()
        {
            size = new Vector2(700, 700); // large initially so visual element can setup correctly
            defaultMode = Mode.Popup;
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
            position = new Rect(pos, new Vector2(style.width,style.height));
        }
        
    }
    
}