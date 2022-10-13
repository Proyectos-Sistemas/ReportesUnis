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
    public partial class HistoricoCXP : System.Web.UI.Page
    {
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("HISTORICO_FINANZAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin") ))
            {
                Response.Redirect(@"~/Default.aspx");
            }

            if (!this.IsPostBack)
            {
                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosCXPEncabezado"];
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
                    //Nit del proveedor
                    sb.Append(@"document.getElementById('NitProveedor').innerHTML = '" + (row["nit"] ?? "").ToString() + "';");

                    //Razón social del proveedor
                    sb.Append(@"document.getElementById('RazonSocialProveedor').innerHTML = '" + (row["razonsocial"] ?? "").ToString() + "';");

                    //Nit del proveedor
                    sb.Append(@"document.getElementById('UnidadNegocioProveedor').innerHTML = '" + (row["name_un"] ?? "").ToString() + "';");


                    //Días de crédito
                    sb.Append(@"document.getElementById('DiasCreditoDocumento').innerHTML = '" + (row["diascredito"] ?? "").ToString() + "';");

                    //Fecha
                    sb.Append(@"document.getElementById('FechaDocumento').innerHTML = '" + (row["fecha"] ?? "").ToString() + "';");

                    //Tipo de documento
                    sb.Append(@"document.getElementById('TipoDeDocumento').innerHTML = '" + (row["typedocument"] ?? "").ToString() + "';");

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
                WebClient _clientWCXPD = new WebClient();
                _clientWCXPD.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                _clientWCXPD.Headers.Add("code_pagar", code);

                string json = _clientWCXPD.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricoFinanciero/HistoricosFinancieroDCPagar");
                dynamic DetallesCXP = JsonConvert.DeserializeObject(json);


                // Obtiene una referencia a la tabla
                sb.Append(@"var tableRefT = document.getElementById('tblDetalleCXP').getElementsByTagName('tbody')[0];");

                int correlativoDetalleCXP = 0;
                foreach (var CXPD in DetallesCXP.items)

                {

                    sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoDetalleCXP + ");");
                    correlativoDetalleCXP++;

                    // Inserta una celda en la fila, en el índice 0
                    sb.Append(@"var newCell = newRow.insertCell(0);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXPD.header_cpagar_code ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 1
                    sb.Append(@"var newCell = newRow.insertCell(1);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXPD.name_un ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 2
                    sb.Append(@"var newCell = newRow.insertCell(2);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXPD.numdocument ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 3
                    sb.Append(@"var newCell = newRow.insertCell(3);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + Convert.ToDateTime(CXPD.fecha).ToString("yyyy-MM-dd") + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 4
                    sb.Append(@"var newCell = newRow.insertCell(4);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXPD.documenttypepago ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 5
                    sb.Append(@"var newCell = newRow.insertCell(5);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (CXPD.numdocumentpago ?? "").ToString() + "');");
                    sb.Append(@"newCell.appendChild(newText);");

                    // Inserta una celda en la fila, en el índice 6
                    sb.Append(@"var newCell = newRow.insertCell(6);");

                    // Añade un nodo de texto a la celda
                    sb.Append(@"var newText = document.createTextNode('" + (Convert.ToDecimal((CXPD.monto ?? "0").ToString())).ToString("0.00") + "');");
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

                GridView1.DataSource = dsReportes.Tables["HistoricosCXPEncabezado"];
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

                if (CodigoProveedor.Text != "")
                {
                    _clientW.Headers.Add("cliente", CodigoProveedor.Text.ToString());
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
                    string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricoFinanciero/HistoricosFinancieroHCPagar");


                    Models.CuentasXPagar Datos = JsonConvert.DeserializeObject<Models.CuentasXPagar>(json);

                    int contadorFila = 1;
                    foreach (Models.ItemCXP CXP in Datos.Items)
                    {
                        DataRow newFila = dsReportes.Tables["HistoricosCXPEncabezado"].NewRow();

                        newFila["code"] = (CXP.Code ?? "").ToString();
                        newFila["code_un"] = (CXP.Code_un ?? "").ToString();
                        newFila["name_un"] = (CXP.Name_un ?? "").ToString();
                        newFila["codeproveedor"] = (CXP.Codeproveedor ?? "").ToString();
                        newFila["nit"] = (CXP.Nit ?? "").ToString();
                        newFila["razonsocial"] = (CXP.Razonsocial ?? "").ToString();
                        newFila["fecha"] = (CXP.Fecha ?? "").ToString();
                        newFila["codtypedocument"] = (CXP.Codtypedocument ?? "").ToString();
                        newFila["typedocument"] = (CXP.Typedocument ?? "").ToString();
                        newFila["numdocument"] = (CXP.Numdocument ?? "").ToString();
                        newFila["diascredito"] = (CXP.Diascredito ?? "").ToString();
                        newFila["descripcion"] = (CXP.Descripcion ?? "").ToString();
                        newFila["valordocument"] = (Convert.ToDecimal((CXP.Valordocument ?? "0").ToString())).ToString("0.00");
                        newFila["saldo"] = (Convert.ToDecimal((CXP.Saldo ?? "0").ToString())).ToString("0.00");
                        newFila["id"] = (CXP.Id ?? "").ToString();

                        dsReportes.Tables["HistoricosCXPEncabezado"].Rows.Add(newFila);
                        contadorFila++;
                    }

                    dt = dsReportes.Tables["HistoricosCXPEncabezado"];
                    ViewState["mydatasource"] = dsReportes.Tables["HistoricosCXPEncabezado"];
                    GridView1.DataSource = dsReportes.Tables["HistoricosCXPEncabezado"];
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