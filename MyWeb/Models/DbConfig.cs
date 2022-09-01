namespace MyWeb.Models
{
    //Entity Class 封裝資料存取有關的環境 變成應用系統獨一無二的物件
    //獨一無二 稱呼為Singleton Pattern
    public class DbConfig
    {
        //自動屬性
        public String connectionString { get; set; }
        public String userName { get; set; }
        public String password { get; set; }

    }
}
