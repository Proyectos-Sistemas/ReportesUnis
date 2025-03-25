using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class UnificacionActualización : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null)
            {
                Response.Redirect(@"~/Default.aspx");
            }
            else
            {
                List<string> grupos = (List<string>)Session["Grupos"];

                if (grupos.Contains("RLI_VistaEmpleados") )
                {
                    Response.Redirect(@"~/ActualizaciónEmpleados.aspx");
                }

                if (((List<string>)Session["Grupos"]).Contains("RLI_VistaAlumnos"))
                {
                    Response.Redirect(@"~/ActualizacionEstudiantes.aspx");
                }
            }

        }
    }
}