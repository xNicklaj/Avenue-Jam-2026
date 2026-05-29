using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ItemConfigPopup : ContentWrapperPopup<ItemConfigPopup>
    {
        private static TabbyInput input = TabbyInput.Input(1);

        public override string xmlName => "ItemConfig";
        public CustomMenuEntry entry => data as CustomMenuEntry;

        private bool pickedHasOpened;


        public override void ProcessData(VisualElement r)
        {
            r.SelectComponent<ItemDataModifier>().ForEach(em => em.data = entry.data);
            r.Children().ForEach(c => c.SetPadding(1));

            var iconSelector = r.Q<ObjectField>("IconSelector");
            var iconColorSelector = r.Q<ColorField>("IconColorSelector");
            var backgroundColorSelector = r.Q<ColorField>("BackgroundColorSelector");
            
            if (entry is Item item)
            {
                iconSelector.value = IconLoader.GetIcon(entry.data.iconName);
                if (iconSelector.value is null)
                {
                    SetIcon(item, null);
                }
                iconSelector.RegisterCallback<ChangeEvent<Object>>(e =>
                {
                    SetIcon(item, (Texture2D)e.newValue);
                });

                iconColorSelector.value = UnityColors.IconColor(entry.data.iconColor);
                iconColorSelector.RegisterCallback<ChangeEvent<Color>>(e =>
                {
                    MenuDataSerializer.SetProperty(entry.data,i => i.iconColor = e.newValue);
                    item.SetIcon();
                });
            
                backgroundColorSelector.value = entry.data.backgroundColor;
                backgroundColorSelector.RegisterCallback<ChangeEvent<Color>>(e =>
                {
                    MenuDataSerializer.SetProperty(entry.data, i => i.backgroundColor = e.newValue);
                    item.SetBackgroundColor();
                });
            }
            else
            {
                r.Q("ItemButtons").Siblings().ForEach(s => s.Hide());
            }
            
            EditorApplication.delayCall += CloseIfRequired;
        }

        private void SetIcon(Item item, Texture2D icon)
        {
            MenuDataSerializer.SetProperty(entry.data,i => i.iconName = icon is null ? "" : icon.name);
            item.SetIcon();
        }
        
        public void OnGUI()
        {
            if (input.key == KeyCode.Escape)
            {
                Dispose();
            }
        }
        
        private void CloseIfRequired()
        {
            if (!SettingsPageIsOpen())
            {
                Dispose();
                return;
            }

            EditorApplication.delayCall += CloseIfRequired;
        }

        private bool SettingsPageIsOpen()
        {
            return UnityWindows.GetWindow<SettingsPage>() is { } nn && nn.IsOpen();
        }
        
        public override void OnLostFocus()
        {
            EditorApplication.update += CloseIf;
        }

        private void CloseIf()
        {
            EditorApplication.update -= CloseIf;
            
            if (focusedWindow is null)
                return;
            
            if (!focusedWindow.TypeName().Contains("ObjectSelector") && !focusedWindow.TypeName().Contains("ColorPicker"))
            {
                Dispose();
            }
        }
        
        public void OnEnable()
        {
            UnityWindows.GetWindows(Window.ObjectSelector).Concat(UnityWindows.GetWindows(Window.ColorPicker)).ForEach(w => w.Close());
        }
    }
}