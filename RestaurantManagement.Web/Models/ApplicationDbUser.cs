using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RestaurantManagement.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //extra property
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string UserFullName { get; set; }


        [Display(Name = "Role")]
        public string RUserRole { get; set; }                    //enum


        [Display(Name = "DOB")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MMM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DOB { get; set; }


        //
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("UserFullName", UserFullName));
            userIdentity.AddClaim(new Claim("RUserRole", RUserRole));
            
            return userIdentity;
        }
    }

    public static class IdentityExtensions
    {
        public static string GetUserFullName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("UserFullName");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue("UserFullName");
            }
            return null;
        }
        public static string GetRUserRole(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("RUserRole");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue("RUserRole");
            }
            return null;
        }
        
    }


}