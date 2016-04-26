using Microsoft.Owin;
using Owin;
using WebSaleDistribute.Models;

[assembly: OwinStartupAttribute(typeof(WebSaleDistribute.Startup))]
namespace WebSaleDistribute
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // insert fixed data to db
            Migrations.Configuration.InitializeUsersManagements(ApplicationDbContext.Create());
        }
    }
}