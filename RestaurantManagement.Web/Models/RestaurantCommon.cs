using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    public enum enumStatus { Active, Inactive }    //enum 0   1  2
    public enum enumRoles { Normal=1,Admin=2 }    //enum 0   1  2

    public class clsLink
    {
        public string TextL { get; set; }
        public string ActionL { get; set; }
        public string ControllerL { get; set; }
    }
    public class clsAdminIndex
    {
        public List<clsLink> LinkList { get; set; }
        public string AdminOrNormal { get; set; }
    }
    public class clsCommonProperty
    {


        [Display(Name = "Remarks")]
        [RegularExpression(@"^[^\\/:*;\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string Remarks { get; set; }


        [Display(Name = "Status")]
        [EnumDataType(typeof(enumStatus))]
        public enumStatus Statuss { get; set; }                    //enum


        [Display(Name = "User ID")]
        public int EntryUserID { get; set; }


        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MMM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> EntryDate { get; set; }


    }
}