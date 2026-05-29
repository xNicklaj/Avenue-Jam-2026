using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class MenuToolbarContainer : VisualComponent, RadioManager<VisualElement>
    {
        public void UncheckOthers(VisualElement t)
        {
            foreach (var toolbar in target.Query().Where(v => v.HasComponent<ShowMenuToolbar>()).Build())
            {
                foreach (var c in toolbar.Children().Where(c => c!= t))
                {
                    c.RemoveFromClassList("selected");
                }
            }
        }
    }
    
    public class ShowMenuToolbarContainer : VisualComponent
    {
        
    }
    
    public class PreferencesTabContent : TabContent
    {
    
    }
    
    public class WelcomeTabContent : TabContent
    {

    }
}