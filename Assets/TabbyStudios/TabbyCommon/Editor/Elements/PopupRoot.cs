namespace TabbyStudios
{
    public class PopupRoot : VisualComponent
    {
        public override void OnAttach()
        {
            target.SetBorder(1);
            target.SetBorderColor(UnityColors.randomBorder);
            target.SetPadding(6);
            target.style.flexShrink = 1;
        }
    }
}