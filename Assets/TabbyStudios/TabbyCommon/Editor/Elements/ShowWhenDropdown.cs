using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ShowWhenDropdown : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }
    
        private DropdownField dropdown => target as DropdownField;

        private Dictionary<string,string> inverse;
        private Dictionary<string, string> textMap = new()
        {
            { "", "Always" },
            { "AnythingSelected", "Anything is selected" },
            { "OneObjectSelected", "One object is selected" },
            { "OnePrefabIsSelected", "One prefab is selected" },
            { "PrefabSelected", "Only prefabs are selected" },
            { "PartOfPrefabSelected", "Any part of a prefab is selected" },
        };
    
        public override void Start()
        {
            inverse = textMap.ToDictionary(pair => pair.Value, pair => pair.Key);
            dropdown.value = textMap[data.showWhen];
            target.RegisterCallback<ChangeEvent<string>>(e => OnChange(e.newValue));
        }

        public void OnChange(string value)
        {
            MenuDataSerializer.SetProperty(data, data => data.showWhen = inverse[value]);
        }
    }
}