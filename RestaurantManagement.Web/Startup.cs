using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestaurantManagement.Web.Startup))]
namespace RestaurantManagement.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
