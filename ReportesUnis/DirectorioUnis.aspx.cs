using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class DirectorioUnis : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DIRECTORIO") && !((List<string>)Session["Grupos"]).Contains("RLI_VistaEmpleados")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            IframDirectorio.Attributes.Add("src", "https://directorioviejo.unis.edu.gt/");
        }
    }
}