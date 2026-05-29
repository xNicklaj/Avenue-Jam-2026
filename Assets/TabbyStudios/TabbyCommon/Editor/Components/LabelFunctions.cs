using System.Text.RegularExpressions;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class VersionLabel : VisualComponent
    {
        public override void OnAttach()
        {
            target.As<Label>().text = $"Version {TabbyCommonFiles.version}";
        }
    }
    
    public class WhatsNewInLabel : VisualComponent
    {
        public override void OnAttach()
        {
            target.As<Label>().text = $"What's new in version {new Regex(@"(\d+\.\d+)\.\d+").Match(TabbyCommonFiles.version).Groups[1]}";
        }
    }
}