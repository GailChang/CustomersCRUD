using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyService.Models;
//客戶服務控制器
namespace MyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        // Data Field
        private Models.NorthwindDB _northwindDB;
        //自訂建構子 注入依賴的 DbContext 物件
        public CustomersController(Models.NorthwindDB _northwindD)
        {
            this._northwindDB = _northwindD;
            Console.WriteLine(this._northwindDB);
        }
       

        //產生一個客戶物件 回應到前端
        [HttpGetAttribute()]
        [RouteAttribute("getcustomers")]  //序列化 物件時json 屬性採用 camel命名法
        [ProducesAttribute("application/json")] //Response Content-Type
        public Models.Customers getCustomers()
        {
            //建構客戶物件 直接回應物件 經由Middleware進行序列化
            return new Models.Customers()
            {
                CustomerId="0001",
                CompanyName="巨匠電腦",
                Address="台北市公園路",
                Phone="02-12345678",
                Country="中華民國"
            }; //物件初始化 
        }

        //採用QueryString 傳遞客戶編號進來 .找出相對客戶資料
        //https://hosted/xxx/xxx?paramName=parmaValue
        [HttpGetAttribute]
        [RouteAttribute("qrybyid/rawdata")] //rawdata 回應是JSON or XML
        public IActionResult customersQryById([FromQueryAttribute(Name ="cid")]String customerId)
        {
            // 進行相對客戶編號資料查詢
            // 1. 採用 LINQ 配合 DbContext (LINQ TO Entity Framework)
            var result = from c in _northwindDB.Customers
                         where c.CustomerId == customerId
                         select c;   // 屬於延遲查詢 Lazy query
            // 準備撈資料 可能有一筆 可能沒有(這時候才正式對資料庫進行查詢)
            Models.Customers customers = result.FirstOrDefault<Models.Customers>();
            if(customers == null)
            {
                // 建構訊息物件
                Models.Message msg = new Message()
                {
                    code = 400,
                    msg = $"{customerId} 查無客戶資料"
                };
                return this.StatusCode(400, msg);
            }   
            else
            {
                return this.Ok(customers);
            }

        }

        //傳遞國家別 採用Path當作參數方式 進行查詢 
        [HttpGetAttribute()]
        [RouteAttribute("qrybycountry/{coun}/rawdata")]
        public List<Models.Customers> customersQryByCountry([FromRouteAttribute(Name ="coun")]String country)
        {
            // 採用 LINQ To Entity 進行多筆查詢
            var result =(from c in _northwindDB.Customers
                        where c.Country == country
                        select c).ToList<Models.Customers>();

            return result;
        }

        //接受前端表單頁面後送postback (類似新增作業) 暈倒表單(輸入到暈倒的意思)
        [HttpPostAttribute]
        [RouteAttribute("customersadd")]
        public String customersAdd([FromFormAttribute] String customerId, [FromFormAttribute] String companyName, [FromFormAttribute] String address, [FromFormAttribute] String phone, [FromFormAttribute] String country)
        {
            return companyName;
        }


        //接受前端表單頁面後送postback (類似新增作業) 整包傳遞近來(JSON文件)
        [HttpPostAttribute]
        [RouteAttribute("customersadd/packing")]
        public String customersAdd2([FromFormAttribute] Customers customers)
        {
            customers.CompanyName = "Tibame";
            return customers.CompanyName;
        }

        //接受前端傳送Json文件進來，進行反序列化成物件 
        [HttpPostAttribute]
        [RouteAttribute("customeradd/rawdata")]
        [ConsumesAttribute("application/json")] //明確前端請求Request Header-application/json content-type
        public Models.Message customersAdd3([FromBodyAttribute] Customers customers)
        {
            //定義一個訊息物件
            Message msg = new Message();
            // 透過 Dbcontext應對資料表DbSet 新增傳遞近來Json 反序列畫物件
            _northwindDB.Customers.Add(customers);   // 在 Persistence 中進行物件的新增
            try
            {
                //同步更新回資料庫
                Int32 number = _northwindDB.SaveChanges();   //翻譯成相對增刪修語法送至資料庫執行          
                if(number > 0)
                {
                    msg.code = 200;
                    msg.msg = $"{customers.CustomerId} 客戶資料新增成功";
                }
            }
            catch(DbUpdateException ex)
            {
                //可以換 HttpResponse 回應的物件的 Http Status code為400???
                HttpResponse resp = this.Response;
                resp.StatusCode = 400;    //bad request 往往是資料格式或內容有問題
                msg.code = 400;
                msg.msg = $"{customers.CustomerId} 客戶資料新增失敗";
            }
            return msg;
        }

        //修改相對客戶資料Action
        [Route("Update/rawdata")]
        [HttpPut]     //修改資料的傳送方法
        [Consumes("application/json")]  //要求前端的Request Header MIME Type
        [Produces("application/json")] //要求回應的Response Header MIME Type
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Message), 400)]
        public Models.Message customersUpdate([FromBody] Customers customers)
        {
            // 使用 Dbcontext 進行物件參考
            _northwindDB.Customers.Add(customers); //維護狀態碼設定為 Added
            // 要去調整這一個剛剛加入的 Entity 狀態為修改狀態
            var entry = _northwindDB.Entry<Customers>(customers);
            // 調整為修改模式
            entry.State = EntityState.Modified;
            Console.WriteLine(entry.State);
            Message msg = new Message();
            // 同步更新到資料庫去
            try
            {
                Int32 rec = _northwindDB.SaveChanges();
                msg.code = 200;
                msg.msg = $"{customers.CustomerId} 客戶資料更新成功";
            }
            catch(DbUpdateException ex)
            {
                //更新失敗
                msg.code = 400;
                msg.msg = $"{customers.CustomerId} 客戶資料更新失敗" + ex.Message;
                //調整回應 Response Http Status Code
                this.Response.StatusCode = 400;   //Http Status 4xx 進入前端 axios catch...
            }
            return msg;
        }

        //刪除客戶資料 依照客戶編號http://xxx.xxx.xxx/xxx/xxx?cid=123
        [Route("delete/rawdata")]
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Customers), 200)]
        [ProducesResponseType(typeof(Message), 400)]
        public IActionResult customersDelete([FromQuery()] String customerId)
        {
            //先找再刪
            var result = (from c in _northwindDB.Customers
                          where c.CustomerId == customerId
                          select c).FirstOrDefault<Models.Customers>();
            //判斷是否有找到紀錄
            if(result == null)
            {
                //查無紀錄
                //建構message物件
                Message msg = new Message()
                {
                    code=400,
                    msg=$"查無客戶:{customerId} 紀錄"
                };
                return this.StatusCode(400, msg);
            }
            else
            {
                //找到了 Customers 活在 Persistence層
                _northwindDB.Entry<Customers>(result).State = EntityState.Deleted;
                //同步更新回資料庫
                try
                {
                    _northwindDB.SaveChanges();
                    //刪除成功
                    return this.Ok(result);
                }
                catch(DbUpdateException ex)
                {
                    //刪除失敗
                    Message msg = new Message()
                    {
                        code = 400,
                        msg = $"刪除客戶:{customerId} 失敗!!"
                    };
                    return this.StatusCode(400, msg);
                }
            }
        }
    }
}
