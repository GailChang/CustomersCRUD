using Microsoft.EntityFrameworkCore;
//規劃一個 Persistence 持久層類別
namespace MyService.Models
{
    //規劃一個 Persistence 類別
    public class NorthwindDB: DbContext
    {
        // 自訂建構子 傳遞DbOptions 進來(執行階段配置連接字串來源)
        public NorthwindDB(DbContextOptions options) : base(options)
        {

        }
        // 定義屬性 Property Mapping Table 屬性名稱符合資料表名稱
        public DbSet<Models.Customers> Customers { get; set; }
    }
}
