using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class TabbyScrollView : VisualComponent, AbstractScroller
    {
        private Vector2 scrollPos;
        public float sensitivity = 8;

        public List<VisualElement> elements => target.Children().ToList();

        public List<VisualElement> items => elements;

        public float offset
        {
            get => scrollPos.y;
            set => Scroll(new Vector2(scrollPos.x, value));
        }
        
        public override void Start()
        {
            target.RegisterCallback<WheelEvent>(OnScroll);
            target.style.overflow = Overflow.Hidden;
        }
        
        public void AddItem(VisualElement item)
        {
            target.AddElement(item);
        }

        public void OnScroll(WheelEvent e)
        {
            var totalItemHeight = elements.Sum(item => item.UnscaledHeight() + item.resolvedStyle.marginTop + item.resolvedStyle.marginBottom);
            var result = Mathf.Clamp(scrollPos.y + sensitivity*e.delta.y, 0, totalItemHeight - target.UnscaledHeight() + 2*target.Padding());
            Scroll(new Vector2(0, result));
        }
        
        public void Scroll(Vector2 pos)
        {
            if (Mathf.Abs(scrollPos.y - pos.y) <= 1) return;
            scrollPos = pos;
            foreach (var child in target.Children())
            {
                child.SetAbsolutePositionRelative(-pos*target.Scale());
            }   
        }
        
        public void Clear()
        {
            foreach (var item in elements)
            {
                item.Pluck();
                item.Disable();
            }

            elements.Clear();
        }

        public void EnsureItemInView(VisualElement item)
        {
            
        }
    }
}