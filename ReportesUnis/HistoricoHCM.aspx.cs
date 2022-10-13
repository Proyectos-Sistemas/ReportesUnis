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
    public partial class HistoricoHCM : System.Web.UI.Page
    {
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("HISTORICO_HCM") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin") ))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!this.IsPostBack)
            {
                WebClient _clientW = new WebClient();
                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricosHCM/H_DEPARTAMENTOS");
                dynamic Departamentos = JsonConvert.DeserializeObject(json);

                ListItem i;
                foreach (var departamento in Departamentos.items)
                {
                    i = new ListItem(departamento.departmentname.ToString(), departamento.departmentname.ToString());
                    Departamento.Items.Add(i);
                }

                DataSetLocalRpt dsReportes = new DataSetLocalRpt();

                GridView1.DataSource = dsReportes.Tables["HistoricosHCM"];
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
                                             where i.Field<String>("idRegistro").Equals(code)
                                             select i;
                DataTable detailTable = query.CopyToDataTable<DataRow>();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");

                string PersonNumber = "";
                string AssignmentNumber = "";

                string NombreEmpleado = "";
                string PrimerNombreEmpleado = "";
                string SegundoNombreEmpleado = "";
                string PrimerApellidoEmpleado = "";
                string SegundoApellidoEmpleado = "";
                string ApellidoCasadaEmpleado = "";

                string DireccionEmpleado = "";

                int correlativoTelefono = 0;
                int correlativoCorreo = 0;

                // Obtiene una referencia a la tabla
                sb.Append(@"var tableRefT = document.getElementById('tblNumeroTelefono').getElementsByTagName('tbody')[0];");

                foreach (DataRow row in detailTable.Rows)
                {
                    PersonNumber = (row["personnumber"] ?? "").ToString();
                    AssignmentNumber = (row["assignmentnumber"] ?? "").ToString();

                    PrimerNombreEmpleado = (row["firstname"] ?? "").ToString();

                    if ((row["middlenames"] ?? "").ToString() != "")
                    {
                        SegundoNombreEmpleado = (" " + row["middlenames"] ?? "").ToString();
                    }

                    if ((row["lastname"] ?? "").ToString() != "")
                    {
                        PrimerApellidoEmpleado = (" " + row["lastname"] ?? "").ToString();
                    }

                    if ((row["previouslastname"] ?? "").ToString() != "")
                    {
                        SegundoApellidoEmpleado = (" " + row["previouslastname"] ?? "").ToString();
                    }

                    if ((row["nameinformation1"] ?? "").ToString() != "")
                    {
                        ApellidoCasadaEmpleado = (" " + row["nameinformation1"] ?? "").ToString();
                    }

                    NombreEmpleado = (PrimerNombreEmpleado + SegundoNombreEmpleado + PrimerApellidoEmpleado + SegundoApellidoEmpleado + ApellidoCasadaEmpleado);

                    //Nombre del empleado
                    sb.Append(@"document.getElementById('NombreEmpleado').innerHTML = '" + NombreEmpleado + "';");

                    DireccionEmpleado = (row["addressline1"] ?? "").ToString();

                    //Dirección del empleado
                    sb.Append(@"document.getElementById('DireccionEmpleado').innerHTML = '" + DireccionEmpleado + "';");

                    //Número de asignación del empleado
                    sb.Append(@"document.getElementById('CodigoAsignacion').innerHTML = '" + AssignmentNumber + "';");


                    //Teléfonos
                    if ((row["phonetype"] ?? "").ToString() != "" || (row["phonenumber"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;
                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["phonetype1"] ?? "").ToString() != "" || (row["phonenumber1"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype1"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber1"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["phonetype2"] ?? "").ToString() != "" || (row["phonenumber2"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype2"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber2"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["phonetype3"] ?? "").ToString() != "" || (row["phonenumber3"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype3"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber3"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["phonetype4"] ?? "").ToString() != "" || (row["phonenumber4"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype4"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber4"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["phonetype5"] ?? "").ToString() != "" || (row["phonenumber5"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefT.insertRow(" + correlativoTelefono + ");");
                        correlativoTelefono++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoTelefono + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonetype5"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["phonenumber5"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    // Obtiene una referencia a la tabla
                    sb.Append(@"var tableRefC = document.getElementById('tblCorreoElectronico').getElementsByTagName('tbody')[0];");

                    //Correos electrónicos
                    if ((row["emailtype"] ?? "").ToString() != "" || (row["emailaddress"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefC.insertRow(" + correlativoCorreo + ");");
                        correlativoCorreo++;
                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoCorreo + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailtype"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailaddress"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["emailtype1"] ?? "").ToString() != "" || (row["emailaddress1"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefC.insertRow(" + correlativoCorreo + ");");
                        correlativoCorreo++;
                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoCorreo + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailtype1"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailaddress1"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    if ((row["emailtype2"] ?? "").ToString() != "" || (row["emailaddress2"] ?? "").ToString() != "")
                    {
                        sb.Append(@"var newRow = tableRefC.insertRow(" + correlativoCorreo + ");");
                        correlativoCorreo++;
                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + correlativoCorreo + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailtype2"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + (row["emailaddress2"] ?? "").ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    //Relación laboral
                    sb.Append(@"document.getElementById('sTipoDeTrabajador').innerHTML = '" + (row["persontypecode"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sPuestoDeTrabajador').innerHTML = '" + (row["jobname"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sGradoDeTrabajador').innerHTML = '" + (row["gradename"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sDepartamentoDeTrabajador').innerHTML = '" + (row["departmentname"] ?? "").ToString() + "';");

                    //Detalle de contrato
                    sb.Append(@"document.getElementById('sTipoDeContrato').innerHTML = '" + (row["workertype"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sFCeseDeContrato').innerHTML = '" + (row["effectiveenddate"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sFInicioDeContrato').innerHTML = '" + (row["effectivestartdate"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sFFinDeContrato').innerHTML = '" + (row["effectiveenddate"] ?? "").ToString() + "';");

                    //Información biográfica
                    sb.Append(@"document.getElementById('sFechaNacimiento').innerHTML = '" + (row["dateofbirth"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sPaisNacimiento').innerHTML = '" + (row["countryofbirth"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sRegionNacimiento').innerHTML = '" + (row["regionofbirth"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sMuniciioNacimiento').innerHTML = '" + (row["townofbirth"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sGrupoSanguineo').innerHTML = '" + (row["bloodtype"] ?? "").ToString() + "';");

                    //Información Legislativa
                    sb.Append(@"document.getElementById('sSexo').innerHTML = '" + (row["sex"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sEstadoCivil').innerHTML = '" + (row["maritalstatus"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sNivelEducacion').innerHTML = '" + (row["highesteducationlevel"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sOrigenEtnico').innerHTML = '" + (row["ethnicity"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sReligion').innerHTML = '" + (row["religion"] ?? "").ToString() + "';");

                    //Datos bancarios
                    sb.Append(@"document.getElementById('sNombreBanco').innerHTML = '" + (row["bankname"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sNumeroCuenta').innerHTML = '" + (row["accountnumber"] ?? "").ToString() + "';");
                    sb.Append(@"document.getElementById('sTipoCuenta').innerHTML = '" + (row["accounttype"] ?? "").ToString() + "';");
                }


                if (PersonNumber != "")
                {
                    //Se obtienen los identificadores nacionales

                    WebClient _clientW = new WebClient();
                    _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _clientW.Headers.Add("personnumber", PersonNumber);
                    string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricosHCM/H_NIDENT");
                    dynamic IdentificadoresNacionales = JsonConvert.DeserializeObject(json);


                    int contador = 0;

                    // Obtiene una referencia a la tabla
                    sb.Append(@"var tableRef = document.getElementById('tblIdenNacional').getElementsByTagName('tbody')[0];");

                    foreach (var IdentificadorNacional in IdentificadoresNacionales.items)
                    {
                        //Identificadores nacionales
                        sb.Append(@"var newRow = tableRef.insertRow(" + contador + "); ");
                        contador++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + contador + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + IdentificadorNacional.nationalidentifiertype.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + IdentificadorNacional.nationalidentifiernumber.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");


                    }

                    //Se obtienen los identificadores externos

                    WebClient _clientWIE = new WebClient();
                    _clientWIE.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _clientWIE.Headers.Add("personnumber", PersonNumber);
                    string jsonIE = _clientWIE.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricosHCM/H_EIDENT");
                    dynamic IdentificadoresExternos = JsonConvert.DeserializeObject(jsonIE);

                    // Obtiene una referencia a la tabla
                    sb.Append(@"var tableRef = document.getElementById('tblIdenExterno').getElementsByTagName('tbody')[0];");

                    int contadorIE = 0;

                    foreach (var IdentificadorExterno in IdentificadoresExternos.items)
                    {
                        //Identificadores nacionales
                        sb.Append(@"var newRow = tableRef.insertRow(" + contadorIE + "); ");
                        contadorIE++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + contadorIE + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + IdentificadorExterno.externalidentifiertype.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + IdentificadorExterno.externalidentifiernumber.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                    }

                    //Se obtienen los salarios

                    WebClient _clientWS = new WebClient();
                    _clientWS.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _clientWS.Headers.Add("AssignmentNumber", AssignmentNumber);
                    string jsonS = _clientWS.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricosHCM/H_SALARY");
                    dynamic Salarios = JsonConvert.DeserializeObject(jsonS);

                    // Obtiene una referencia a la tabla
                    sb.Append(@"var tableRef = document.getElementById('tblSalarios').getElementsByTagName('tbody')[0];");

                    int contadorS = 0;

                    foreach (var Salario in Salarios.items)
                    {
                        //Identificadores nacionales
                        sb.Append(@"var newRow = tableRef.insertRow(" + contadorS + "); ");
                        contadorS++;

                        sb.Append(@"var headerCell = document.createElement('TH');");
                        sb.Append(@"headerCell.innerHTML = '" + contadorS + "';");
                        sb.Append(@"newRow.appendChild(headerCell);");

                        // Inserta una celda en la fila, en el índice 1
                        sb.Append(@"var newCell = newRow.insertCell(1);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + Salario.datefrom.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 2
                        sb.Append(@"var newCell = newRow.insertCell(2);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + Salario.dateto.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");

                        // Inserta una celda en la fila, en el índice 3
                        sb.Append(@"var newCell = newRow.insertCell(3);");

                        // Añade un nodo de texto a la celda
                        sb.Append(@"var newText = document.createTextNode('" + Salario.salaryamount.ToString() + "');");
                        sb.Append(@"newCell.appendChild(newText);");
                    }

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

                GridView1.DataSource = dsReportes.Tables["HistoricosHCM"];
                GridView1.DataBind();

                //Requerido para que jQuery DataTables funcione.
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

                bool buscar = false;

                WebClient _clientW = new WebClient();

                _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

                if (FechaNacimiento.Text != "")
                {
                    _clientW.Headers.Add("dateofbirth", Convert.ToDateTime(FechaNacimiento.Text).ToString("dd/MM/yyyy"));
                    buscar = true;
                }

                if (Departamento.Text != "")
                {
                    _clientW.Headers.Add("departmentname", Departamento.Text.ToString());
                    buscar = true;
                }

                if (Apellidos.Text != "")
                {
                    _clientW.Headers.Add("lastname", Apellidos.Text.ToString());
                    buscar = true;
                }

                if (Nombres.Text != "")
                {
                    _clientW.Headers.Add("name", Nombres.Text.ToString());
                    buscar = true;
                }

                if (NationalIdentifier.Text != "")
                {
                    _clientW.Headers.Add("numberident", NationalIdentifier.Text.ToString());
                    buscar = true;
                }

                if (PersonNumber.Text != "")
                {
                    _clientW.Headers.Add("personnumber", PersonNumber.Text.ToString());
                    buscar = true;
                }

                if (buscar == true)
                {
                    string json = _clientW.DownloadString("https://apex.unis.edu.gt:8443/ords/unis_interfaces/HistoricosHCM/HISTORICOS");


                    Models.EmpleadosH Datos = JsonConvert.DeserializeObject<Models.EmpleadosH>(json);

                    int contadorFila = 1;
                    foreach (Models.ItemEmpleadoH empleado in Datos.Items)
                    {
                        DataRow newFila = dsReportes.Tables["HistoricosHCM"].NewRow();

                        newFila["personnumber"] = (empleado.Personnumber ?? "").ToString();
                        newFila["effectivestartdate"] = (empleado.Effectivestartdate).ToString("dd/MM/yyyy");
                        newFila["lastname"] = (empleado.Lastname ?? "").ToString();
                        newFila["previouslastname"] = (empleado.Previouslastname ?? "").ToString();
                        newFila["firstname"] = (empleado.Firstname ?? "").ToString();
                        newFila["middlenames"] = (empleado.Middlenames ?? "").ToString();
                        newFila["nameinformation1"] = (empleado.Nameinformation1 ?? "").ToString();
                        newFila["nationalidentifiertype"] = (empleado.Nationalidentifiertype ?? "").ToString();
                        newFila["nationalidentifiernumber"] = (empleado.Nationalidentifiernumber ?? "").ToString();
                        newFila["dateofbirth"] = (empleado.Dateofbirth ?? "").ToString();
                        newFila["maritalstatus"] = (empleado.Maritalstatus ?? "").ToString();
                        newFila["sex"] = (empleado.Sex ?? "").ToString();
                        newFila["ethnicity"] = (empleado.Ethnicity ?? "").ToString();
                        newFila["religion"] = (empleado.Religion ?? "").ToString();
                        newFila["bloodtype"] = (empleado.Bloodtype ?? "").ToString();
                        newFila["townofbirth"] = (empleado.Townofbirth ?? "").ToString();
                        newFila["countryofbirth"] = (empleado.Countryofbirth ?? "").ToString();
                        newFila["regionofbirth"] = (empleado.Regionofbirth ?? "").ToString();
                        newFila["legislationcode"] = (empleado.Legislationcode ?? "").ToString();
                        newFila["highesteducationlevel"] = (empleado.Highesteducationlevel ?? "").ToString();
                        newFila["addressline1"] = (empleado.Addressline1 ?? "").ToString();
                        newFila["addladdressattribute3"] = (empleado.Addladdressattribute3 ?? "").ToString();
                        newFila["postalcode"] = (empleado.Postalcode ?? "").ToString();
                        newFila["townorcity"] = (empleado.Townorcity ?? "").ToString();
                        newFila["country"] = (empleado.Country ?? "").ToString();
                        newFila["addresstype"] = (empleado.Addresstype ?? "").ToString();
                        newFila["emailaddress"] = (empleado.Emailaddress ?? "").ToString();
                        newFila["emailtype"] = (empleado.Emailtype ?? "").ToString();
                        newFila["phonenumber"] = (empleado.Phonenumber ?? "").ToString();
                        newFila["phonetype"] = (empleado.Phonetype ?? "").ToString();
                        newFila["workertype"] = (empleado.Workertype ?? "").ToString();
                        newFila["actioncode"] = (empleado.Actioncode ?? "").ToString();
                        newFila["effectiveenddate"] = (empleado.Effectiveenddate).ToString();
                        newFila["assignmentcategory"] = (empleado.Assignmentcategory ?? "").ToString();
                        newFila["workercategory"] = (empleado.Workercategory ?? "").ToString();
                        newFila["hourlysalariedcode"] = (empleado.Hourlysalariedcode ?? "").ToString();
                        newFila["hourlysalariedname"] = (empleado.Hourlysalariedname ?? "").ToString();
                        newFila["gradecode"] = (empleado.Gradecode ?? "").ToString();
                        newFila["gradename"] = (empleado.Gradename ?? "").ToString();
                        newFila["positioncode"] = (empleado.Positioncode ?? "").ToString();
                        newFila["positionname"] = (empleado.Positionname ?? "").ToString();
                        newFila["jobcode"] = (empleado.Jobcode ?? "").ToString();
                        newFila["jobname"] = (empleado.Jobname ?? "").ToString();
                        newFila["locationcode"] = (empleado.Locationcode ?? "").ToString();
                        newFila["departmentname"] = (empleado.Departmentname ?? "").ToString();
                        newFila["frequency"] = (empleado.Frequency ?? "").ToString();
                        newFila["normalhours"] = (empleado.Normalhours ?? "").ToString();
                        newFila["fullparttime"] = (empleado.Fullparttime ?? "").ToString();
                        newFila["persontypecode"] = (empleado.Persontypecode ?? "").ToString();
                        newFila["bankname"] = (empleado.Bankname ?? "").ToString();
                        newFila["accountnumber"] = (empleado.Accountnumber ?? "").ToString();
                        newFila["accounttype"] = (empleado.Accounttype ?? "").ToString();
                        newFila["salaryamount"] = (empleado.Salaryamount ?? "").ToString();
                        newFila["externalidentifiernumber"] = (empleado.Externalidentifiernumber ?? "").ToString();
                        newFila["externalidentifiertype"] = (empleado.Externalidentifiertype ?? "").ToString();
                        newFila["assignmentnumber"] = (empleado.Assignmentnumber ?? "").ToString();
                        newFila["emailaddress1"] = (empleado.Emailaddress1 ?? "").ToString();
                        newFila["emailaddress2"] = (empleado.Emailaddress2 ?? "").ToString();
                        newFila["phonenumber1"] = (empleado.Phonenumber1 ?? "").ToString();
                        newFila["phonenumber2"] = (empleado.Phonenumber2 ?? "").ToString();
                        newFila["phonenumber3"] = (empleado.Phonenumber3 ?? "").ToString();
                        newFila["phonenumber4"] = (empleado.Phonenumber4 ?? "").ToString();
                        newFila["phonenumber5"] = (empleado.Phonenumber5 ?? "").ToString();
                        newFila["emailtype1"] = (empleado.Emailtype1 ?? "").ToString();
                        newFila["emailtype2"] = (empleado.Emailtype2 ?? "").ToString();
                        newFila["phonetype1"] = (empleado.Phonetype1 ?? "").ToString();
                        newFila["phonetype2"] = (empleado.Phonetype2 ?? "").ToString();
                        newFila["phonetype3"] = (empleado.Phonetype3 ?? "").ToString();
                        newFila["phonetype4"] = (empleado.Phonetype4 ?? "").ToString();
                        newFila["phonetype5"] = (empleado.Phonetype5 ?? "").ToString();
                        newFila["idRegistro"] = contadorFila;

                        dsReportes.Tables["HistoricosHCM"].Rows.Add(newFila);
                        contadorFila++;
                    }

                    dt = dsReportes.Tables["HistoricosHCM"];
                    ViewState["mydatasource"] = dsReportes.Tables["HistoricosHCM"];
                    GridView1.DataSource = dsReportes.Tables["HistoricosHCM"];
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