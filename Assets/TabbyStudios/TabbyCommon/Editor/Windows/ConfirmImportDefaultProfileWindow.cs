namespace TabbyStudios
{
    public class ConfirmImportDefaultProfileWindow: ConfirmationPopup<ConfirmImportDefaultProfileWindow>
    {
        public override void Confirm()
        {
            MenuDataSerializer.LoadDefaultProfile();
        }
    
        public override string WarningText()
        {
            return "Importing the default profile will override any existing profiles named Default";
        }
    }
}