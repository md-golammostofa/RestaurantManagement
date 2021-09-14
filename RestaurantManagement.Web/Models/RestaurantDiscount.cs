using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    [Table("tblDiscount")]
    public class clsDiscount:clsCommonProperty
    {
        [Key]
        public int DiscountID { get; set; }

        [Display(Name = "Name")]
        [RegularExpression(@"^[^\\/:_*;\.\)\(]+$", ErrorMessage = "The characters '_',':', '.' ';', '*', '/' and '\' are not allowed")]
        public string DiscountName { get; set; }


        [Display(Name = "Percentage(%)")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        [Range(typeof(decimal), "0", "100")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal DiscountPercentage { get; set; }

    }
}