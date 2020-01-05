using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestShop.WebUI.Startup))]
namespace TestShop.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
