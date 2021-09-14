using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    [Table("tblEmployee")]
    public class clsEmployee : clsCommonProperty
    {

        public clsEmployee()
        {
            this.EntryDate = DateTime.Now;
            this.Statuss = enumStatus.Active;
            this.EntryUserID = 999;
        }


        [Key]
        public int EID { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string EName { get; set; }



        [Required]
        [Display(Name = "Employee Type")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string EType { get; set; }

        [Required]
        [Display(Name = "Employee Number")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string ENumber { get; set; }


        public String getEmployeeCode()
        {
            String Ecode = "EMP-" + EID.ToString().PadLeft(4, '0');
            return Ecode;
        }



    }
}