using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class TabGroup : VisualComponent
    {
        public virtual string defaultTab => Config.GetSetting<string>("lastOpenTab");
    
        public void ClickButton(VisualElement e)
        {
            target.Children().Where(t => t != e).ForEach(t => t.GetComponent<TabButton>()?.SetClicked(false));
        }
    
        public override void Start()
        {
            var b = target.Children().FirstOrDefault(c => c.Name() == defaultTab)?.GetComponentDownwards<TabButton>();
            if(b is not null)
                b.SetClicked(true);
            else target.Children().FirstOrDefault(c => c.GetComponentDownwards<TabButton>() is not null)?.GetComponentDownwards<TabButton>().SetClicked(true);
        }
    
    }
}