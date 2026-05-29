using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public static class MouseUtil
    {
        public static Vector2 MousePosition<T>(EventBase<T> e) where T : EventBase<T>, new()
        {
            Assert.IsTrue(e.target is VisualElement);
            var target = e.target as VisualElement;
            
            return target.PositionIgnoringEditorWindowTabBar() + target.ContainingWindow().position.position + e.originalMousePosition;
        }

        private static Vector2 EventPosition<T>(EventBase<T> e) where T : EventBase<T>, new()
        {
            if (e is IMouseEvent me)
                return me.mousePosition;
            if (e is IPointerEvent pe)
                return pe.position;

            return Vector2.zero;
        }
    }
}