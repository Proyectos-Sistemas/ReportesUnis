using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace ReportesUnis
{
    public class Global : HttpApplication
    {

        public static string _strdc = null;
        public static string _strdc1 = null;
        public static string _struser = null;
        public static string _strdPass = null;


        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _strdc = Util.LeerArchivoRecursos(1, Server.MapPath(@"conf.dat"));
            _strdc1 = Util.LeerArchivoRecursos(2, Server.MapPath(@"conf.dat"));
            _struser = Util.LeerArchivoRecursos(3, Server.MapPath(@"conf.dat"));
            _strdPass = Util.LeerArchivoRecursos(4, Server.MapPath(@"conf.dat"));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "https://localhost:44382");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, CONNECT, TRACE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }

    }
}