using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCPopupCRUD.Startup))]
namespace MVCPopupCRUD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
