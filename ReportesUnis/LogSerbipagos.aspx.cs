using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Data;
using System.Xml;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace ReportesUnis
{
    public partial class LogSerbipagos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || ( !((List<string>)Session["Grupos"]).Contains("RLI_Serbi") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
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
                _clientW.Headers.Add("TipoConsulta", DropDownListTipo.SelectedValue.ToString());
                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/rptintserbipagoscampus/rptintserbipagoscampus");

                Models.SerbiPagos Datos = JsonConvert.DeserializeObject<Models.SerbiPagos>(json);


                DataSetLocalRpt dsReportes = new DataSetLocalRpt();
                foreach (Models.ItemSerbiPagos serbipago in Datos.Items)
                {/*
                    DataRow newFila = dsReportes.Tables["LogSerbiPagos"].NewRow();
                    newFila["dtfecharegistro"] = serbipago.dtfecharegistro.ToString();
                    newFila["tiposolicitud"] = serbipago.tiposolicitud.ToString();
                    newFila["txtxmlrequest"] = serbipago.txtxmlrequest.ToString();
                    newFila["txtxmlresponseerror"] = serbipago.txtxmlresponseerror.ToString();
                    dsReportes.Tables["LogInterHCMCS"].Rows.Add(newFila);*/

                    /*****************************************************************************************************************/
                    DataRow newFila = dsReportes.Tables["LogSerbiPagos"].NewRow();
                    
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.LoadXml(serbipago.txtxmlrequest.ToString());
                    
                    XmlDocument xmlDocumentDecode = new XmlDocument();
                    
                    xmlDocumentDecode.LoadXml(xmlDocument.InnerText);
                    
                    XmlNodeList listanodosenc = xmlDocumentDecode.SelectNodes("//encabezado");
                    XmlNodeList listanodosid = xmlDocumentDecode.SelectNodes("//identificador");
                    XmlNodeList listanodosval = xmlDocumentDecode.SelectNodes("//valor");

                    foreach (XmlNode node in listanodosenc)//datos encabezado
                    {
                        newFila["Convenio"] = node.ChildNodes[0].InnerText;
                        newFila["Proveedor"] = node.ChildNodes[1].InnerText;
                        newFila["AutorizaProveedor"] = node.ChildNodes[4].InnerText;
                        newFila["AutorizaBanco"] = node.ChildNodes[5].InnerText;
                    }

                    foreach (XmlNode node in listanodosid)//datos identificador
                    {
                        newFila["IdPersona"] = node.ChildNodes[0].InnerText;

                    }

                    foreach (XmlNode node in listanodosval)//datos de valores
                    {
                        newFila["Valor"] = node.ChildNodes[0].InnerText;
                    }

                    //Fecha y hora de la solicitud
                    newFila["Fecha"] = serbipago.dtfecharegistro.ToString();

                    //Tipo de solicitud
                    newFila["Tipo"] = serbipago.tiposolicitud.ToString();



                    newFila["Transacion"] = xmlDocument.InnerText;
                    newFila["Mensaje"] = serbipago.txtxmlresponseerror.ToString();
                    newFila["Usuario"] = Context.User.Identity.Name.ToString();


                    if (TextBoxID.Text != string.Empty && TextBoxID.Text.Trim() == newFila["IdPersona"].ToString().Trim())
                    {
                        dsReportes.Tables["LogSerbiPagos"].Rows.Add(newFila);
                    }
                    else
                    {
                        if (TextBoxID.Text == string.Empty)
                        {
                            dsReportes.Tables["LogSerbiPagos"].Rows.Add(newFila);
                        }
                    }


                    /*****************************************************************************************************************/

                }

                /*
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();
                DataSetSebiPagosTableAdapters.logTableAdapter tableLog = new DataSetSebiPagosTableAdapters.logTableAdapter();
                DataTable _tablaDatos = new DataTable();
                _tablaDatos = tableLog.GetDataLogFiltros(Convert.ToDateTime(TextBoxFechaIni.Text), Convert.ToDateTime(TextBoxFechaFin.Text).AddDays(1), Convert.ToByte(DropDownListTipo.SelectedValue.ToString()));

                */

                ReportDataSource DataReport = new ReportDataSource("DSLogSerbiPagos", dsReportes.Tables["LogSerbiPagos"].Rows);
                ReportViewerReporte.LocalReport.DataSources.Clear();
                ReportViewerReporte.LocalReport.DataSources.Add(DataReport);
                ReportViewerReporte.LocalReport.ReportPath = Server.MapPath("rptLogSerbipagos.rdlc");
                ReportViewerReporte.LocalReport.Refresh();
            }
            catch (Exception)
            {

                throw;
            }
            




        }
    }
}