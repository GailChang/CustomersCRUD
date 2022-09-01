using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
//客戶資料維護控制器(MVC)
namespace MyWeb.Controllers
{
    public class CustomersController : Controller
    {
        //注入自訂 Singleton
        private Models.DbConfig _dbConfig;
        //Customer Service位址
        private Models.CustomersService _customersService;
        
        //建構子注入
        public CustomersController(Models.DbConfig _dbConfig, Models.CustomersService _customersService)
        {
            this._dbConfig = _dbConfig;
            this._customersService = _customersService;
        }
        //產生一個頁面 產生國家別清單 提供給前端渲染下拉式功能表
        public IActionResult customersQryByCountry()
        {
            //透過自訂的服務物件 取出連接字串
            String connectionString = _dbConfig.connectionString;
            //使用 ADO.NET 直接存取資料庫
            //1.建立連接物件 開啟連接 注入依賴-建構子注入或者使用屬性注入
            SqlConnection conn = new SqlConnection (connectionString);
            // 如果用屬性注入:
            //SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = connectionString;
            //--------------------
            //連接物件開啟連接
            conn.Open();
            //2.連接物件拖來你自己喜歡的命令物件 (兩種方法)
             SqlCommand comm = conn.CreateCommand();
            //SqlCommand comm = new SqlCommand();
            //連接物件與命令互動
            //comm.Connection = conn;
            //--------------------
            //下命令 (採用 SQL Pass Through -SPT 採用 Native SQL 整理成字串送過去執行)
            comm.CommandText = "Select distinct Country From Customers order by Country";
            //命令類型
            comm.CommandType = System.Data.CommandType.Text;   //預設 可以省略(因為是下SQL查詢語法)
            //執行線上讀取 執行查詢
            SqlDataReader reader = comm.ExecuteReader();
            //SqlDataReader reader = null;
            //reader = comm.ExecuteReader();
            //--------------------
            //逐筆讀取 封裝成集合物件 List<String>
            List<String> data = new List<String>();
            while(reader.Read())  //Read():逐筆fetching 後端查詢結果
            {
                // SqlDataReader[] 索引子(Indexer)指定查詢輸出欄位
                data.Add(reader["Country"].ToString());
            }
            //關閉連接
            conn.Close();
            //拿個袋子(動態類型)裝起來
            ViewBag.Data = data;
            //持續服務位址
            ViewBag.QryByCountry = _customersService.qryByCountry;
            //持續修改資料服務端位址
            ViewBag.Update = _customersService.modify;
            //刪除服務位址持續畫面去
            ViewBag.Delete = _customersService.delete;

            return View("custqry");
        }

    }
}
