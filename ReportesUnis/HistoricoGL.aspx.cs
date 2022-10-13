using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace ReportesUnis
{
    public partial class HistoricoGL : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("HISTORICO_FINANZAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin") ))
            {
                Response.Redirect(@"~/Default.aspx");
            }

            if (!this.IsPostBack)
            {
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosGL"];
                GridView1.DataBind();

                //Requerido para que jQuery DataTables funcione.
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }


        protected void ButtonAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosGL"];
                GridView1.DataBind();

                //Requerido para que jQuery DataTables funcione.
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

                DateTime FechaInicioBusqueda = Convert.ToDateTime(FechaInicio.Text);
                DateTime FechaFinBusqueda = Convert.ToDateTime(FechaFin.Text);

                //Solo permitir búsquedas por año
                if (FechaInicioBusqueda.AddYears(1) <= FechaFinBusqueda)
                {
                    string script = "alert(\"El rango máximo de búsqueda es de 1 año, ingrese una nueva fecha de inicio y fin.\");";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", script, true);
                }
                else
                {
                    bool buscar = false;

                    WebClient _clientW = new WebClient();

                    _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

                    if (Id.Text != "")
                    {
                        _clientW.Headers.Add("code", Id.Text.ToString());
                        buscar = true;
                    }


                    if (UnidadDeNegocio.Text != "")
                    {
                        _clientW.Headers.Add("unidad", UnidadDeNegocio.Text.ToString());
                        buscar = true;
                    }

                    if (CodigoCuentaContable.Text != "")
                    {
                        _clientW.Headers.Add("cc_code", CodigoCuentaContable.Text.ToString());
                        buscar = true;
                    }

                    if (Periodo.Text != "")
                    {
                        _clientW.Headers.Add("periodo", Periodo.Text.ToString());
                        buscar = true;
                    }

                    if (Descripcion.Text != "")
                    {
                        _clientW.Headers.Add("description", Descripcion.Text.ToString());
                        buscar = true;
                    }

                    if (FechaInicio.Text != "")
                    {
                        _clientW.Headers.Add("startdate", Convert.ToDateTime(FechaInicio.Text).ToString("yyyy/MM/dd"));
                        buscar = true;
                    }

                    if (FechaFin.Text != "")
                    {
                        _clientW.Headers.Add("enddate", Convert.ToDateTime(FechaFin.Text).ToString("yyyy/MM/dd"));
                        buscar = true;
                    }

                    if (buscar == true)
                    {
                        string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricoFinanciero/HistoricosFinancieroPC");


                        Models.ContabilidadGeneral Datos = JsonConvert.DeserializeObject<Models.ContabilidadGeneral>(json);

                        int contadorFila = 1;
                        foreach (Models.ItemContabilidadGeneral GL in Datos.Items)
                        {
                            DataRow newFila = dsReportes.Tables["HistoricosGL"].NewRow();

                            newFila["id"] = (GL.Id ?? "").ToString();
                            newFila["code"] = (GL.Code ?? "").ToString();
                            newFila["fecha"] = (GL.Fecha ?? "").ToString();
                            newFila["unidadnegocio_code"] = (GL.Unidadnegocio_code ?? "").ToString();
                            newFila["name_un"] = (GL.Name_un ?? "").ToString();
                            newFila["centrointegracion_code"] = (GL.Centrointegracion_code ?? "").ToString();
                            newFila["name_ci"] = (GL.Name_ci ?? "").ToString();
                            newFila["cuentacontable_code"] = (GL.Cuentacontable_code ?? "").ToString();
                            newFila["name_cc"] = (GL.Name_cc ?? "").ToString();
                            newFila["debe"] = (Convert.ToDecimal((GL.Debe ?? "0").ToString())).ToString("0.00");
                            newFila["haber"] = (Convert.ToDecimal((GL.Haber ?? "0").ToString())).ToString("0.00");
                            newFila["descripcion"] = (GL.Descripcion ?? "").ToString();
                            newFila["periodo"] = (GL.Periodo ?? "").ToString();

                            dsReportes.Tables["HistoricosGL"].Rows.Add(newFila);
                            contadorFila++;
                        }

                        ViewState["mydatasource"] = dsReportes.Tables["HistoricosGL"];
                        GridView1.DataSource = dsReportes.Tables["HistoricosGL"];
                        GridView1.DataBind();

                        //Requerido para que jQuery DataTables funcione.
                        GridView1.UseAccessibleHeader = true;
                        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

                    }
                    else
                    {
                        string script = "alert(\"Ingrese información para mostrar resultados.\");";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", script, true);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}