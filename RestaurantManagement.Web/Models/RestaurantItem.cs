using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    [Table("tblItem")]
    public class clsItem:clsCommonProperty
    {
        public clsItem()
        {
            this.Statuss = enumStatus.Active;
            this.EntryDate = DateTime.Now;
            this.EntryUserID = 999;
        }

        [Key]
        public int ItemID { get; set; }

        [RegularExpression(@"^[^\\/:*;_\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string ItemName { get; set; }

        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string ItemCode { get; set; }

        public string Description { get; set; }


        public decimal UnitPrice { get; set; }
    }
}