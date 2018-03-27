using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lands.BackEnd.Startup))]
namespace Lands.BackEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
