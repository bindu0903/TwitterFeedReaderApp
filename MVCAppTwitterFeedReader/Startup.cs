using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCAppTwitterFeedReader.Startup))]
namespace MVCAppTwitterFeedReader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
