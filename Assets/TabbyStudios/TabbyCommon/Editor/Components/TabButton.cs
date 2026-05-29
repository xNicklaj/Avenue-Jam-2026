using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class TabButton : ButtonComponent
    {
        public TabGroup group;
        private VisualElement content;
        
        private TabContent tabContent => content?.GetComponentDownwards<TabContent>();
        private bool hasOpenTab;
        public virtual string contentName => $"{target.name.Split("-")[0]}Content-";
        
        public override void OnAttach()
        {
            group = target.GetComponentUpwards<TabGroup>();
            content = target.Root().Q(contentName);
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            if (hasOpenTab)
                return;
            SetClicked(true);
            group.ClickButton(target);
        }
        
        public void SetClicked(bool clicked)
        {
            if (hasOpenTab == clicked)
                return;
            hasOpenTab = clicked;
            if (clicked)
            {
                Config.instance.Set("lastOpenTab",target.Name());
            }

            SetStyleOnContent(clicked);
        }

        private void SetStyleOnContent(bool visible)
        {
            if (tabContent is not null)
            {
                tabContent.SetVisible(visible);
            }
            else
            {
                content.SetVisible(visible);
            }
        }
    }
}