using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    [Table("tblDRCR")]
    public class clsDRCR:clsCommonProperty
    {
        public clsDRCR()
        {
            this.Credit = 0;
            this.Balance = 0;
            this.EntryUserID = 999;
            this.EntryDate = DateTime.Now;
            this.Statuss = enumStatus.Active;
        }

        [Key]
        public int DRCRID { get; set; }

        [Required]
        [Display(Name = "Expense For")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string ExpenseFor { get; set; }


        [Display(Name = "Amount")]
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }


    }
}