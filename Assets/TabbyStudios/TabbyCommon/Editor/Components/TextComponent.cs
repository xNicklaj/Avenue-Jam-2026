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
            FitText();
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

        public void FitText()
        {
            var textSize = TextSize().x;
            var width = target.resolvedStyle.width;
            
            //why is text calculation wrong while playing?
            if (textSize - width > 1 && !Application.isPlaying) //resolve style will round resolved valued
                text = TruncateWithDots(text, width/textSize);
        }

        private string TruncateWithDots(string input, float ratio)
        {
            if (input == null) return null;
            if (ratio <= 0f) return "...";

            int maxVisible = (int)(input.Length*ratio) - 3;
            if (maxVisible < 1) maxVisible = 1;

            if (maxVisible >= input.Length)
                return input;

            return input.Substring(0, maxVisible) + "...";
        }
    }
}