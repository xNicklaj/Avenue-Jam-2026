namespace TabbyStudios
{
    public class TabContent : VisualComponent
    {
        public virtual string title => target.Name().RemoveAfterLast("Tab");
        
        public override void OnAttach()
        {
            target.SetPadding(4);
        }
    }
}
