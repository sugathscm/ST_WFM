using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WFM.UI.Startup))]
namespace WFM.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
