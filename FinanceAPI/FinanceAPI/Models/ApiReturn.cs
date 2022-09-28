namespace FinanceAPI.Models
{
    public class ApiReturn
    {
        public bool Sucess { get; set; } = false;
        public string Message { get; set; } = "";
        public string Detail { get; set; } = "";
    }
}
