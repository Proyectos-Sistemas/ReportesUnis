using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Owin;

namespace ReportesUnis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IdentityModelEventSource.ShowPII = true;
            ConfigureAuth(app);
        }
    }
}
