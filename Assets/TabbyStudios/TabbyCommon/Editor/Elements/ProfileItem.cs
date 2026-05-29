using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ProfileItem : VisualComponent
    {
    
        public string profile;
        public string currentProfile;
    
        public ProfileItem(string profile)
        {
            this.profile = profile;
            currentProfile = Config.Subscribe<string>("profile", Uncheck);
        }

        public void Uncheck(string newProfile)
        {
            currentProfile = newProfile;
            if (currentProfile != profile)
            {
                target.SetBorderColor(new StyleColor(new Color(0,0,0,0)));
            }
            else
            {
                target.SetBorderColor(new StyleColor(new Color(0.5f,0.5f,0.5f,1)));
            }
        }

        public override void Awake()
        {
            RegisterGeometryChanged();
            
            var label = target.Query().Build().First(e => e.Name() == "Name").GetComponent<TextComponent>();
            label.text = profile;
            label.fontColor = UnityColors.textColor;
            Uncheck(Config.GetSetting<string>("profile"));
            
            target.Q("ArrowContainer").visible = false;
            
            target.SetBorder(1);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        public override void OnGeometryChanged(GeometryChangedEvent e)
        {
            var textComp = target.SelectFirstComponent<TextComponent>();
            textComp.text = profile;
            textComp.target.style.minWidth = 90;
        }
        
        public virtual void OnPointerUp(PointerUpEvent e)
        {
            
        }

        public virtual void OnPointerDown(PointerDownEvent e)
        {
            if (File.Exists(TabbyCommonFiles.ProfilePath(profile)))
            {
                Config.SetSetting("profile",profile);
                Debug.Log($"Selected profile: {profile}");
            }
        }
    
        public virtual void OnPointerLeave(PointerLeaveEvent e)
        {
            target.style.backgroundColor = new Color(0, 0, 0, 0);
        }

        public virtual void OnPointerEnter(PointerEnterEvent e)
        {
            target.style.backgroundColor = UnityColors.defaultHoveredColor;
        }
    }
}