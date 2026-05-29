using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class TextComponent : VisualComponent, IOpacity
    {
        public IMGUIContainer container;
        
        private GUIStyle style;
        private GUIContent content;

        public string text
        {
            get => content.text;
            set => content.text = value;
        }

        public int fontSize
        {
            get => style.fontSize;
            set => style.fontSize = value;
        }

        public Font font
        {
            get => style.font;
            set => style.font = value;
        }

        public FontStyle fontStyle
        {
            get => style.fontStyle;
            set => style.fontStyle = value;
        }

        public Color fontColor
        {
            get => style.normal.textColor;
            set => style.normal.textColor = value;
        }

        public float opacity
        {
            get => target.style.opacity.value;
            set
            {
                target.style.opacity = value;
                fontColor = fontColor.SetA(value);
            }
        }

        
        public TextComponent()
        {
            Init();
        }
        
        public TextComponent(string text)
        {
            Init();
            this.text = text;
        }

        private void Init()
        {
            container = new(DrawText);
            container.style.overflow = Overflow.Hidden;
            container.style.textOverflow = TextOverflow.Clip;
            style = new();
            style.alignment = TextAnchor.MiddleLeft;
            style.clipping = TextClipping.Clip;
            content = new();
        }

        public override void Awake()
        {
            RegisterGeometryChanged();
        }

        public Vector2 TextSize()
        {
            return style.CalcSize(content);
        }

        public override void OnAttach()
        {
            target.AddElement(container);
        }

        public override void OnGeometryChanged(GeometryChangedEvent e)
        {
            container.style.width = target.resolvedStyle.width;
            container.style.height = target.resolvedStyle.height;
        }

        public void AdjustSize()
        {
            var size = TextSize();
            target.style.width = size.x;
            target.style.height = size.y;
        }

        private void DrawText()
        {
            GUI.Box(new Rect(0,0,target.resolvedStyle.width,target.resolvedStyle.height), content, style);
        }
    }
}