namespace FinanceAPI.Models.DynamicTable
{
    public class Actions
    {
        public string Detail { get; set; }
        public string Duplicate { get; set; }
        public string Edit { get; set; }
        public string New { get; set; }
        public string Save { get; set; } // Usado no dinamyc edit
        public string SaveNew { get; set; } // Usado no dinamyc edit
        public string Cancel { get; set; } // Usado no dinamyc edit

        public bool Remove { get; set; }
        public bool RemoveAll { get; set; }

        
    }
}
