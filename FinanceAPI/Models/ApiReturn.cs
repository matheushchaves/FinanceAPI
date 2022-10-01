namespace FinanceAPI.Models
{
    public class ApiReturn
    {        
        public string Message { get; set; } = "";
        public string Code { get; set; } = "";
        public string DetailedMessage { get; set; } = "";
        public string Type { get; set; } = "warning";
        
    }
}
