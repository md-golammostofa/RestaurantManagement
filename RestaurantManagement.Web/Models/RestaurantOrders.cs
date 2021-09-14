using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestaurantManagement.Web.Models
{
    [Table("tblOrder")]
    public class clsOrder:clsCommonProperty
    {
        public clsOrder()
        {
            this.CustomerID = 999;
            this.TableID = 999;
            this.DiscountID = 999;

            this.TotalDiscount = 0;
            this.TotalAmount = 0;
            this.NetTotal = 0;

            this.EntryDate = DateTime.Now;
            this.Statuss = enumStatus.Active;
        }

        [Key]
        public int OrderID { get; set; }
        public string OrderCode { get; set; }

        //customer
        public Nullable<int> CustomerID { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }


        //table
        public Nullable<int> TableID { get; set; }
        public string TableName { get; set; }
        public string TableCode { get; set; }


        //discount
        public Nullable<int> DiscountID { get; set; }
        public string DiscountName { get; set; }
        public Nullable<decimal> TotalDiscount { get; set; }


        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> NetTotal { get; set; }


        public string OrderStatus { get; set; }
        public string OrderPostedBy { get; set; }
        public string OrderedTableName { get; set; }
        public Nullable<decimal> ReceivedAmount { get; set; }
        public Nullable<decimal> ReturnedAmount { get; set; }




        //
        public virtual ICollection<clsOrderDetails> tblOrderDetails { get; set; }
    }


    [Table("tblOrderDetails")]
    public class clsOrderDetails
    {
        public clsOrderDetails()
        {
            this.ProductID = 999;
            this.Quantity = 0;
            this.UnitPrice = 0;
            this.TotalAmount = 0;
        }
        [Key]
        public int OrderDetailsID { get; set; }

        [ForeignKey("clsOrder")]
        public int OrderID { get; set; }


        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string KitchenPrintStatus { get; set; }



        //
        public virtual clsOrder clsOrder { get; set; }
    }
   


    //[Table("tblOrder")]
    //public class clsOrder : clsCommonProperty
    //{
    //    public clsOrder()
    //    {
    //        this.EntryDate = DateTime.Now;
    //        this.Statuss = enumStatus.Active;
    //        this.EntryUserID = 999;
    //    }

    //    [Key]
    //    public int OrderID { get; set; }
    //    public string OrderCode { get; set; }

    //    //table
    //    public int TableID { get; set; }
    //    public string TableName { get; set; }
    //    public string TableCode { get; set; }


    //    //discount
    //    public int DiscountID { get; set; }
    //    public string DiscountName { get; set; }
    //    public decimal TotalDiscount { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public decimal NetTotal { get; set; }


    //    //
    //    public string CustomerName { get; set; }
    //    public string CustomerMobileNo { get; set; }
    //    public string CustomerEmail { get; set; }
    //    public string CustomerAddress { get; set; }


    //    public virtual ICollection<clsOrderDetails> tblOrderDetails { get; set; }
    //}


    //[Table("tblOrderDetails")]
    //public class clsOrderDetails
    //{

    //    [Key]
    //    public int OrderDetailsID { get; set; }

    //    [ForeignKey("clsOrder")]
    //    public int OrderID { get; set; }


    //    public int ItemID { get; set; }

    //    public string ItemName { get; set; }

    //    public string Description { get; set; }

    //    public decimal UnitPrice { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal Amount { get; set; }


    //    //
    //    public virtual clsOrder clsOrder { get; set; }
    //}

}