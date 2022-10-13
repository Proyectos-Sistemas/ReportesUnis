using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Data;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace ReportesUnis
{
    public partial class LogInterfazHCMCS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_HCM_CS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
        }

        protected void ButtonAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient _clientW = new WebClient();
                _clientW.Headers.Add("FechaIni", Convert.ToDateTime(TextBoxFechaIni.Text).ToString("yyyy-MM-dd"));
                _clientW.Headers.Add("FechaFin", Convert.ToDateTime(TextBoxFechaFin.Text).ToString("yyyy-MM-dd"));
                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/rptinthcmcampus/rptinthcmcampus");

                Models.Empleados Datos = JsonConvert.DeserializeObject<Models.Empleados>(json);


                DataSetLocalRpt dsReportes = new DataSetLocalRpt();
                foreach (Models.ItemEmpleado empleado in Datos.Items)
                {
                    DataRow newFila = dsReportes.Tables["LogInterHCMCS"].NewRow();
                    newFila["TipoProceso"] = empleado.tipoproceso.ToString();
                    newFila["EstadoProceso"] = empleado.estadoproceso.ToString();
                    newFila["Nombres"] = empleado.firstname.ToString();
                    newFila["Apellidos"] = empleado.lastname.ToString();
                    newFila["PersonId"] = empleado.personid.ToString();
                    newFila["IdentificadorNacional"] = empleado.nationalidentifiernumber.ToString();
                    newFila["Estado"] = empleado.estatus.ToString();
                    newFila["DescripcionEstado"] = empleado.descripcion_estatus.ToString();
                    newFila["FechaCreacion"] = empleado.fecha_creacion.ToString("dd/MM/yyyy");
                    newFila["FechaModificacion"] = empleado.fecha_ultima_actualizacion.ToString("dd/MM/yyyy");
                    newFila["Usuario"] = Context.User.Identity.Name.ToString();

                    dsReportes.Tables["LogInterHCMCS"].Rows.Add(newFila);
                }

                ReportDataSource DataReport = new ReportDataSource("DSInHCMCS", dsReportes.Tables["LogInterHCMCS"].Rows);
                ReportViewerReporte.LocalReport.DataSources.Clear();
                ReportViewerReporte.LocalReport.DataSources.Add(DataReport);
                ReportViewerReporte.LocalReport.ReportPath = Server.MapPath("rptLogInterfazHCMCS.rdlc");
                ReportViewerReporte.LocalReport.Refresh();
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}