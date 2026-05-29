namespace TabbyStudios
{
    public class SettingsPageBase : VisualComponent
    {
        public override void Start()
        {
            target.SetBorderColor(UnityColors.randomBorder);
        }
    }
}