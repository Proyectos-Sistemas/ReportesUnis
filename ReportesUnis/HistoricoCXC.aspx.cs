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
    public partial class HistoricoCXC : System.Web.UI.Page
    {
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("HISTORICO_FINANZAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }

            if (!this.IsPostBack)
            {
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosCXCEncabezado"];
                GridView1.DataBind();

                //Requerido para que jQuery DataTables funcione.
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }



        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("cmdDetalle"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string code = GridView1.DataKeys[index].Value.ToString();

                dt = ViewState["mydatasource"] as DataTable;

                IEnumerable<DataRow> query = from i in dt.AsEnumerable()
                                             where i.Field<String>("code").Equals(code)
                                             select i;
                DataTable detailTable = query.CopyToDataTable<DataRow>();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");

                foreach (DataRow row in detailTable.Rows)
                {
                    //Nit del cliente
                    sb.Append(@"document.getElementById('NitCliente').innerHTML = '" + (row["nit"] ?? "").ToString() + "';");

                    //Razón social del cliente
                    sb.Append(@"document.getElementById('RazonSocialCliente').innerHTML = '" + (row["razonsocial"] ?? "").ToString() + "';");

                    //Nit del cliente
                    sb.Append(@"document.getElementById('UnidadNegocioCliente').innerHTML = '" + (row["name_un"] ?? "").ToString() + "';");


                    //Días de crédito
                    sb.Append(@"document.getElementById('DiasCreditoDocumento').innerHTML = '" + (row["diascredito"] ?? "").ToString() + "';");

                    //Fecha
                    sb.Append(@"document.getElementById('FechaDocumento').innerHTML = '" + (row["fecha"] ?? "").ToString() + "';");

                    //Tipo de documento
                    sb.Append(@"document.getElementById('TipoDeDocumento').innerHTML = '" + (row["documenttype"] ?? "").ToString() + "';");

                    //Número de documento
                    sb.Append(@"document.getElementById('NumeroDeDocumento').innerHTML = '" + (row["numdocument"] ?? "").ToString() + "';");

                    //Valor de documento
                    sb.Append(@"document.getElementById('ValorDeDocumento').innerHTML = '" + (row["valordocument"] ?? "").ToString() + "';");

                    //Saldo
                    sb.Append(@"document.getElementById('SaldoDocumento').innerHTML = '" + (row["saldo"] ?? "").ToString() + "';");


                    //Número de docuneto (encabezado)
                    sb.Append(@"document.getElementById('DocEncabezado').innerHTML = '" + (row["code"] ?? "").ToString() + "';");
                }

                //Detalle
                WebClient _clientWCXCD = new WebClient();
                _clientWCXCD.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                _clientWCXCD.Headers.Add("code_cobro", code);

                string json = _clientWCXCD.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricoFinanciero/HistoricosFinancieroDCCobrar");
                dynamic DetallesCXC = JsonConvert.DeserializeObject(json);


                // Obtiene una referencia a la tabla
                sb.Append(@"var tableRefT = document.getElementById('tblDetalleCXC').getElementsByTagName('tbody')[0];");

                int correlativoDetalleCXC = 0;
                foreach (var CXCD in DetallesCXC.items)

                {

                    sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoDetalleCXC + ");");
                    correlativoDetalleCXC++;

                    // Inserta una celda en la fila, en el índice 0
                    sb.Append(@"var newCell = newRow.insertCell(0);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXCD.header_ccobrar_code ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 1
                    sb.Append(@"var newCell = newRow.insertCell(1);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXCD.name_un ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 2
                    sb.Append(@"var newCell = newRow.insertCell(2);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXCD.numdocument ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 3
                    sb.Append(@"var newCell = newRow.insertCell(3);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + Convert.ToDateTime(CXCD.fecha).ToString("yyyy-MM-dd") + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 4
                    sb.Append(@"var newCell = newRow.insertCell(4);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXCD.documenttype ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 5
                    sb.Append(@"var newCell = newRow.insertCell(5);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXCD.numdocumentcobro ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 6
                    sb.Append(@"var newCell = newRow.insertCell(6);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (Convert.ToDecimal((CXCD.monto ?? "0").ToString())).ToString("0.00") + "');");
                    sb.Append(@"newCell.appendChild(newText);");
                }
                sb.Append(" DivCargandoNoVisible();");
                sb.Append("$('#currentdetail').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                           "ModalScript", sb.ToString(), false);


            }
        }

        protected void ButtonAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosCXCEncabezado"];
                GridView1.DataBind();

                //Requerido para que jQuery DataTables funcione.
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

                bool buscar = false;

                WebClient _clientW = new WebClient();

                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

                if (UnidadDeNegocio.Text != "")
                {
                    _clientW.Headers.Add("name_un", UnidadDeNegocio.Text.ToString());
                    buscar = true;
                }

                if (CodigoCliente.Text != "")
                {
                    _clientW.Headers.Add("cliente", CodigoCliente.Text.ToString());
                    buscar = true;
                }

                if (Nit.Text != "")
                {
                    _clientW.Headers.Add("nit", Nit.Text.ToString());
                    buscar = true;
                }

                if (RazonSocial.Text != "")
                {
                    _clientW.Headers.Add("razonsocial", RazonSocial.Text.ToString());
                    buscar = true;
                }

                if (TipoDocumento.Text != "")
                {
                    _clientW.Headers.Add("tipodoc", TipoDocumento.Text.ToString());
                    buscar = true;
                }

                if (NumeroDocumento.Text != "")
                {
                    _clientW.Headers.Add("numdoc", NumeroDocumento.Text.ToString());
                    buscar = true;
                }

                if (Valor.Text != "")
                {
                    _clientW.Headers.Add("valor", Valor.Text.ToString());
                    buscar = true;
                }

                if (Saldo.Text != "")
                {
                    _clientW.Headers.Add("saldo", Saldo.Text.ToString());
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
                    string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricoFinanciero/HistoricosFinancieroHCCobrar");


                    Models.CuentasXCobrar Datos = JsonConvert.DeserializeObject<Models.CuentasXCobrar>(json);

                    int contadorFila = 1;
                    foreach (Models.ItemCXC CXC in Datos.Items)
                    {
                        DataRow newFila = dsReportes.Tables["HistoricosCXCEncabezado"].NewRow();

                        newFila["code"] = (CXC.Code ?? "").ToString();
                        newFila["unidadnegocio_code"] = (CXC.Unidadnegocio_code ?? "").ToString();
                        newFila["name_un"] = (CXC.Name_un ?? "").ToString();
                        newFila["codecliente"] = (CXC.Codecliente ?? "").ToString();
                        newFila["nit"] = (CXC.Nit ?? "").ToString();
                        newFila["razonsocial"] = (CXC.Razonsocial ?? "").ToString();
                        newFila["fecha"] = (CXC.Fecha ?? "").ToString();
                        newFila["codtypedocument"] = (CXC.Codtypedocument ?? "").ToString();
                        newFila["documenttype"] = (CXC.Documenttype ?? "").ToString();
                        newFila["numdocument"] = (CXC.Numdocument ?? "").ToString();
                        newFila["diascredito"] = (CXC.Diascredito ?? "").ToString();
                        newFila["valordocument"] = (Convert.ToDecimal((CXC.Valordocument ?? "0").ToString())).ToString("0.00");
                        newFila["saldo"] = (Convert.ToDecimal((CXC.Saldo ?? "0").ToString())).ToString("0.00");
                        newFila["id"] = (CXC.Id ?? "").ToString();

                        dsReportes.Tables["HistoricosCXCEncabezado"].Rows.Add(newFila);
                        contadorFila++;
                    }

                    dt = dsReportes.Tables["HistoricosCXCEncabezado"];
                    ViewState["mydatasource"] = dsReportes.Tables["HistoricosCXCEncabezado"];
                    GridView1.DataSource = dsReportes.Tables["HistoricosCXCEncabezado"];
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}