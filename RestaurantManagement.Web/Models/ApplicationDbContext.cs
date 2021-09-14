using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RestaurantManagement.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //tables
        public DbSet<clsTable> tblTable { get; set; }
        public DbSet<clsEmployee> tblEmployee { get; set; }
        public DbSet<clsItem> tblItem { get; set; }
        public DbSet<clsDiscount> tblDiscount { get; set; }

        public DbSet<clsOrder> tblOrder { get; set; }
        public DbSet<clsOrderDetails> tblOrderDetails { get; set; }

        public DbSet<clsDRCR> tblDRCR { get; set; }
        


        //
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        
    }
}