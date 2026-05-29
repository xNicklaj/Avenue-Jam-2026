using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ConfirmationPopup<T> : ContentWrapperPopup<T> where T : ConfirmationPopup<T> 
    {
        public override string xmlName => "ConfirmationPopup";
    
        public override void ProcessData(VisualElement r)
        {
            r.SelectComponent<ItemDataModifier>().ForEach(em => em.data = data as ItemData);
        
            var confirmButton = r.SelectFirstComponent<ConfirmButton>();
            confirmButton.action = Confirm;

            var label = r.SelectFirstComponent<AreYouSureLabel>();
            label.text = ConfirmationText();

            var helpBox = r.Q("HelpBox").Q<Label>();
            var warning = WarningText();
            if (!warning.IsNullOrEmpty())
            {
                helpBox.text = warning;
            }
            else
            {
                helpBox.Hide();
            }
        }

        public virtual void Confirm()
        {
        
        }

        public virtual string ConfirmationText()
        {
            return "Are you sure?";
        }

        public virtual string WarningText()
        {
            return "";
        }
    }
}