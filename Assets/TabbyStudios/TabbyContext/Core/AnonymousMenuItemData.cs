namespace TabbyStudios
{
    public struct AnonymousMenuItemData
    {
        public string path;
        public bool selected;
        public bool separator;

        public AnonymousMenuItemData(string path, bool selected, bool separator)
        {
            this.path = path;
            this.selected = selected;
            this.separator = separator;
        }
    }
}