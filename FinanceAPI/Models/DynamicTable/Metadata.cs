namespace FinanceAPI.Models.DynamicTable
{
    public class Metadata
    {
        public int Version { get; set; } = 1;
        public string Title { get; set; }

        public bool AutoRouter { get; set; } = true;

        public List<Field> Fields { get; set; }

        public Actions Actions { get; set; }
    }
}
