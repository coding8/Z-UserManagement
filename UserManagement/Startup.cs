using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserManagement.Startup))]
namespace UserManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
