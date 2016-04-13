using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSaleDistribute.Startup))]
namespace WebSaleDistribute
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
