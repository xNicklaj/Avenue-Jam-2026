using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{

    public class ViewChangelogButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://assetstore.unity.com/packages/tools/utilities/tabby-context-enhanced-context-menus-311600#releases";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class DiscordButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://discord.gg/fBX3yAs8BG";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class MoreByTabbyStudiosButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://assetstore.unity.com/publishers/113033";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class ImportExamplesButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {

            var tabbyContextExamples = TabbyAssets.tabbyContextFiles?.GetMemberValue<string>("examplesPath");
            var tabbyMenusExamples = TabbyAssets.tabbyMenusFiles?.GetMemberValue<string>("examplesPath");

            var files = new List<string>();
            
            if(tabbyContextExamples is not null)
                files.AddRange(Directory.GetFiles(tabbyContextExamples));
            
            if(tabbyMenusExamples is not null)
                files.AddRange(Directory.GetFiles(tabbyMenusExamples));

            
            foreach (var file in files)
            {
                if (!file.EndsWith(".json"))
                    continue;

                File.Copy(file.ReplaceMultiple("\\", "/"), $"{TabbyCommonFiles.profilesFolder}/{Path.GetFileNameWithoutExtension(file)}.json", overwrite:true);
            }
            
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Example profiles imported");
        }
    }
    
    public class LoadDefaultProfileButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            ConfirmImportDefaultProfileWindow.Create(MouseUtil.MousePosition(e)).Display();
        }
    }
    
    public class NewProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            target.GetComponentUpwards<ProfilesTabContent>().NewProfile();
        }
    }
    
    public class ResetToDefaultButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            ResetPreferencesPopup.Create(e.mousePosition);
        }
    }
    
    public class DeleteProfileButton : IconButton
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            target.GetComponentUpwards<ProfilesTabContent>().DeleteProfile(Config.GetSetting<string>("profile"));
        }
    }
    
    public class DeleteItemButton : ButtonComponent, ItemDataModifier
    {
        public ItemData data { get; set; }
    
        public override void OnMouseUp(MouseUpEvent e)
        {
            DeleteItemPopupWindow.Create(MouseUtil.MousePosition(e), data).Display();
        }
    }
    
    public class DuplicateProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            target.GetComponentUpwards<ProfilesTabContent>().DuplicateCurrentProfile();
        }
    }
    
    public class AssetStoreButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://u3d.as/3u2C";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class DocumentationButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            if (TabbyAssets.hasTabbyContext && TabbyAssets.tabbyContextFiles.GetMemberValue("documentationPath") is string contextPath && File.Exists(contextPath)) 
            {
                var dir = Directory.GetCurrentDirectory();
                contextPath = Path.Combine(dir, contextPath);
                Process.Start(new ProcessStartInfo(contextPath) { UseShellExecute = true });
            }
            
            if (TabbyAssets.hasTabbyMenus && TabbyAssets.tabbyMenusFiles.GetMemberValue("documentationPath") is string menusPath && File.Exists(menusPath)) 
            {
                var dir = Directory.GetCurrentDirectory();
                contextPath = Path.Combine(dir, menusPath);
                Process.Start(new ProcessStartInfo(contextPath) { UseShellExecute = true });
            }
        }
    }
    
    public class RenameProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            target.GetComponentUpwards<ProfilesTabContent>().RenameProfile();
        }
    }
    
    public class MenuToolbarButton : VisualComponent
    {
        public string menuPath;
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            var manager = (SettingsMenuManager)target.FindComponent<CustomMenu>().manager;
            var button = (Button)target;
            Config.SetSetting(nameof(ShowMenuToolbar.lastShowMenuInMenusTab), menuPath);
            button.AddToClassList("selected");
            manager.ClearMenus();
            manager.CreateMenu(menuPath, Vector2.zero);
        }
    }
    
}