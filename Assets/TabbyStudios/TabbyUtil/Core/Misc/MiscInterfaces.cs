using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public interface AbstractScreenSpace
    {
        public Vector2 ScreenPosition(VisualElement e);
    }
    
    public interface AbstractScroller
    {
        public List<VisualElement> items { get; }

        public float offset { get; set; }
        public void AddItem(VisualElement item);
        public void Clear();
    }

    public interface IOpacity
    {
        public float opacity { get; set; }
    }
    
    public class ButtonComponent : VisualComponent
    {
        public override void Start()
        {
            RegisterMouseUp();
        }
    }
    
    public class ValueChanger : VisualComponent
    {
        
    }

    public interface FastCacheSearch
    {
        //speeds up type cache search
    }
    
}