namespace TabbyStudios
{
    public class TabContentContainer : VisualComponent
    {
        public override void OnAttach()
        {
            target.SelectComponent<TabContent>().ForEach(c => c.Hide());
        }
    }
}