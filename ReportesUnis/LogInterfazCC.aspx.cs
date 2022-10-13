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
    public partial class LogInterfazCC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || ( !((List<string>)Session["Grupos"]).Contains("RLI_CRM_CS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
        }

        protected void ButtonAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient _clientW = new WebClient();
                _clientW.Headers.Add("nid", TextBoxID.Text);
                _clientW.Headers.Add("FechaIni", Convert.ToDateTime(TextBoxFechaIni.Text).ToString("yyyy-MM-dd"));
                _clientW.Headers.Add("FechaFin", Convert.ToDateTime(TextBoxFechaFin.Text).ToString("yyyy-MM-dd"));
                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/rptitfz/rptitfz/");

                Models.Contactos Datos = JsonConvert.DeserializeObject<Models.Contactos>(json);


                DataSetLocalRpt dsReportes = new DataSetLocalRpt();
                foreach (Models.Item contacto in Datos.items)
                {
                    DataRow newFila = dsReportes.Tables["LogInterCRMCS"].NewRow();
                    newFila["TipoNid"] = contacto.clave_documento_ident.ToString();
                    newFila["Nid"] = contacto.documento_identidad.ToString();
                    newFila["Nombre"] = contacto.nombres.ToString() + " " + contacto.apellido_paterno.ToString();
                    newFila["FechaEfectiva"] = contacto.fecha_efectiva.ToString();
                    newFila["FechaRegistro"] = contacto.fecha_registro.ToString("dd/MM/yyyy");
                    newFila["Correoe"] = contacto.direc_correo_electronico.ToString();
                    newFila["CicloAmin"] = contacto.ciclo_admision.ToString();
                    newFila["Status"] = contacto.descripcion_estatus.ToString();
                    newFila["Usuario"] = Context.User.Identity.Name.ToString();
                    newFila["IdCRM"] = contacto.id_party_id.ToString();
                    newFila["GradoAcad"] = contacto.grado_academico.ToString();


                    dsReportes.Tables["LogInterCRMCS"].Rows.Add(newFila);
                }

                ReportDataSource DataReport = new ReportDataSource("DSInCRMCS", dsReportes.Tables["LogInterCRMCS"].Rows);
                ReportViewerReporte.LocalReport.DataSources.Clear();
                ReportViewerReporte.LocalReport.DataSources.Add(DataReport);
                ReportViewerReporte.LocalReport.ReportPath = Server.MapPath("rptLogInterfazCC.rdlc");
                ReportViewerReporte.LocalReport.Refresh();
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}