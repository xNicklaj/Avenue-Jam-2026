using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class DisplayNameField : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void Start()
        {
            var field = target.Q<TextField>();
            field.value = data.displayName;
            field.RegisterCallback<ChangeEvent<string>>(Rename);
        }

        private void Rename(ChangeEvent<string> e)
        {
            MenuDataSerializer.SetProperty(data, item => item.displayName = e.newValue);
        }

    }
}