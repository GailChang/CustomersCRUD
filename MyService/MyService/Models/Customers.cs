using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyService.Models
{
    //採用 Annotation 進行映對資料表描述
    //客戶類別 Entity
    [TableAttribute(name:"Customers")]
    //Entity Class(POCO)
    public class Customers
    {
        //自動屬性設定 
        [ColumnAttribute(name: "CustomerID")]
        [MaxLengthAttribute(5)]
        [KeyAttribute]
        [RequiredAttribute()]

        public String CustomerId { set; get; }

        [ColumnAttribute(name: "CompanyName")]
        [RequiredAttribute()]
        public String CompanyName { set; get; }

        [ColumnAttribute(name: "Address")]
        public String? Address { set; get; }

        [ColumnAttribute(name: "Phone")]
        public String? Phone { set; get; }

        [ColumnAttribute(name: "Country")]
        public String? Country { set; get; }

        public override string ToString()
        {
            return $"客戶編號:{CustomerId} 公司行號 :{CompanyName}";
        }

    }
}
