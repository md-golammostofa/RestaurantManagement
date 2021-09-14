using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagement.Web.Models
{
    [Table("tblTable")]
    public class clsTable:clsCommonProperty
    {
        public clsTable()
        {
            this.EntryDate = DateTime.Now;
            this.Statuss = enumStatus.Active;
            this.EntryUserID = 999;
        }

        [Key]
        public int TableID { get; set; }

        [Required]
        [Display(Name = "Table Name")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string  TableName { get; set; }


        [Required]
        [Display(Name = "Table Code")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [Remote("IsAlreadyExisted", "RestaurantTable", HttpMethod = "POST", ErrorMessage = "Already Existed!")]
        public string TableCode { get; set; }


        //method
        //public string GetTableCode()
        //{
        //    return "TBL-"+ this.TableID.ToString();
        //}
    }

    public class clsTableVM
    {

        public int TableID { get; set; }

        public string TableName { get; set; }

        public string TableCode { get; set; }

        public string TblBackColor { get; set; }
        public string TblBooked { get; set; }

        public int EID { get; set; }
        public string EName { get; set; }


    }

    // for top moving
    public class clsTableTM
    {




        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitPrice { get; set; }
        public string TopMoving { get; set; }

    


    }

    // for Recent Invices or list 
    public class clsTableRL
    {




        public string OrderID{ get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }




    }



}