using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class Separator : CustomMenuEntry
    {
        private static string xmlName = "Separator";
        public bool collapse = true;

        public Separator(ItemData data) : base(data)
        {
        
        }
        
        public override VisualElement CreateParent()
        {
            return AssetCache.LoadXml(xmlName);
        }

        public override void Start()
        {
            base.Start();
            ForceHideNext();
        }

        private void ForceHideNext()
        {
            if (!collapse || !target.visible) return;

            var nextSeparator = parentMenu.NextVisible(this);
            if (nextSeparator is null)
            {
                if (parentMenu.Next(this) is null)
                {
                    target.Hide();
                }
                return;
            }

            if (nextSeparator is Separator s)
            {
                s.Hide();
                s.ForceHideNext();
            }
        }
    }
}