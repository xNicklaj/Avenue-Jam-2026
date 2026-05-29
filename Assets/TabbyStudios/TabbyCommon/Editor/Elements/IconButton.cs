using UnityEngine;

#pragma warning disable

namespace TabbyStudios
{
    public class IconButton : ButtonComponent
    {
        public override void OnAttach()
        {
            base.OnAttach();
        
            target.style.width = 24;
            target.style.height = 24;

            target.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
    }
}