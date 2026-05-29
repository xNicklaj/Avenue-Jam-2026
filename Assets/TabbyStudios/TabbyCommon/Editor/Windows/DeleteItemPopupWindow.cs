namespace TabbyStudios
{
    public class DeleteItemPopupWindow : ConfirmationPopup<DeleteItemPopupWindow>
    {
        private ItemData item => data as ItemData;
        
        public override void Confirm()
        {
            Profiles.instance.menuSerializer.SetPropertyRecursively(item, data => data.deleted = true);
        }

        public override string ConfirmationText()
        {
            return $"Are you sure you want to delete item {item.displayName}?";
        }

        public override string WarningText()
        {
            return "It's recommended to hide items instead so they may be re-enabled later if needed";
        }
    }
}