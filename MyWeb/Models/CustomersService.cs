namespace MyWeb.Models
{
    //用來封裝遠端關於客戶資料處理服務位址
    public class CustomersService
    {
        public String qryById { get; set; }
        public String qryByCountry { get; set; }
        public String add { get; set; }
        public String modify { get; set; }
        public String delete { get; set; }
    }
}
