using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ConnectIn.Startup))]
namespace ConnectIn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
