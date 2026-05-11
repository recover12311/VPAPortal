namespace VPAPortal.Data.Models
{
    public class PropertyType
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int SortOrder { get; set; }
        public List<PropertyItem> Items { get; set; } = new();
    }
}
