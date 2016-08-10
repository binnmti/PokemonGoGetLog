using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PokemonGoGetLog.Startup))]
namespace PokemonGoGetLog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
