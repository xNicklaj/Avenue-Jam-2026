using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class NewsList : VisualComponent
    {
        public virtual string type => "";

        public string template = "NewsItem";

        private static Dictionary<string, string> classes = new()
        {
            {"fix", "bugfix-badge"},
            {"improvement", "improvement-badge"},
            {"feature", "feature-badge"},
            {"note", "note-badge"},
        };

        public override void OnAttach()
        {
            var list = TabbyCommonFiles.changelog.Where(item => item.type == type).ToList();
            if(type == "feature") list.AddRange(TabbyCommonFiles.changelog.Where(item => item.type == "note").ToList());
            
            if (list.IsEmpty())
            {
                target.Hide();
                return;
            }
            
            foreach (var item in list)
            {
                var e = AssetCache.LoadXml(template);
                e.Q<Label>("Title").text = item.title;
                e.Q<Label>("Text").text = item.text;
                e.Q("Badge").AddToClassList(classes[item.type]);
                target.AddElement(e);
                
            }
        }
    }

    public class FixList : NewsList
    {
        public override string type => "fix";
    }
    
    public class ImprovementList : NewsList
    {
        public override string type => "improvement";
    }
    
    public class FeatureList : NewsList
    {
        public override string type => "feature";
    }
}