using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class AreYouSureLabel : VisualComponent
    {
        public ItemData data { get; set; }
        
        private Label label => target as Label;
        public string text
        {
            get => label.text;
            set => label.text = value;
        }
    }
}