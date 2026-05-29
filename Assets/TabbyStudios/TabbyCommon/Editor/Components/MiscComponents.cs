namespace TabbyStudios
{
    public interface RadioManager<T>
    {
        void UncheckOthers(T t);
    }

    public interface ItemDataModifier
    {
        public ItemData data { get; set; }
    }
    
    public class EmptySpace : VisualComponent
    {
        
    }

    public class MenuCustomizationContainer : VisualComponent
    {

    }

    public class ProfileButtons : VisualComponent
    {

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