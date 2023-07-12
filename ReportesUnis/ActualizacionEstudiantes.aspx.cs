using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Interop;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Oracle.ManagedDataAccess.Client;
using Windows.Devices.Sensors;
using Windows.Media.Capture;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Web.Services;
using System.Web.Script.Services;
using System.Net.Http;
using ReportesUnis.API;
using System.Globalization;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Controls;
namespace ReportesUnis
{
    public partial class ActualizacionEstudiantes : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string mensaje = "";
        int controlPantalla;
        ConsumoAPI api = new ConsumoAPI();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "OPTIONS")
            {
                // Handle preflight request
                Response.StatusCode = 204; // No Content
                Response.End();
            }

            // Add CORS headers to the response
            Response.AppendHeader("Access-Control-Allow-Origin", "*"); // Replace "*" with the specific origin if needed
            Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");

            LeerInfoTxt();
            LeerPathApex();
            controlPantalla = PantallaHabilitada("Carnetización Masiva");
            txtExiste.Text = controlPantalla.ToString();
            if (controlPantalla >= 1)
            {
                //TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");
                TextUser.Text = "2676467470101";
                if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_VistaAlumnos") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
                {
                    Response.Redirect(@"~/Default.aspx");
                }
                if (!IsPostBack)
                {
                    BtnReload.Visible = false;
                    controlPantalla = PantallaHabilitada("Semana");
                    if (controlPantalla >= 1)
                    {
                        Page.ClientScript.RegisterStartupScript(GetType(), "CheckCameraAccess", "checkCameraAccess();", true);
                        LeerInfoTxtSQL();
                        LeerInfoTxtPath();
                        llenadoPais();
                        //llenadoPaisnit();
                        llenadoDepartamento();
                        //llenadoDepartamentoNit();
                        llenadoState();
                        //llenadoStateNIT();
                        mostrarInformación();
                        if (txtNit.Text == "CF")
                        {
                            txtNit.Enabled = false;
                            RadioButtonNombreSi.Checked = true;
                            ValidarNIT.Enabled = false;
                            //Combos.Visible = false;

                        }
                        else
                        {
                            RadioButtonNombreNo.Checked = true;
                            TxtDiRe1.Enabled = true;
                            TxtDiRe2.Enabled = true;
                            TxtDiRe3.Enabled = true;
                            ValidarNIT.Enabled = true;
                        }

                        /*AlmacenarFotografia();
                        fotoAlmacenada();*/

                        if (String.IsNullOrEmpty(txtCarne.Text))
                        {
                            BtnActualizar.Visible = false;
                            lblActualizacion.Text = "El usuario utilizado no se encuentra registrado como estudiante";
                            CmbPais.SelectedValue = "Guatemala";
                            tabla.Visible = false;
                        }
                    }

                }
                else
                {
                    lblActualizacion.Text = "La pantalla de actualización está disponible únicamente de Lunes a Viernes.";
                    controlCamposVisibles();
                }
            }

            else
            {
                lblActualizacion.Text = "¡IMPORTANTE! Esta página no está disponible, ¡Permanece atento a nuevas fechas para actualizar tus datos!";
                controlCamposVisibles();
            }
        }

        //Metodos
        void LeerInfoTxt()
        {
            string rutaCompleta = CurrentDirectory + "conexion.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                TxtURL.Text = line;
                file.Close();
            }
        }

        void LeerPathApex()
        {
            string rutaCompleta = CurrentDirectory + "urlApex.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtApex.Text = line;
                file.Close();
            }
        }

        void LeerInfoTxtSQL()
        {
            string rutaCompleta = CurrentDirectory + "conexionSQL.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                TxtURLSql.Text = line;
                file.Close();
            }
        }
        void LeerInfoTxtPath()
        {
            string rutaCompleta = CurrentDirectory + "PathAlmacenamiento.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath.Text = line;
                file.Close();
            }
        }
        void controlCamposVisibles()
        {
            CargaFotografia.Visible = false;
            tabla.Visible = false;
            tbactualizar.Visible = false;
            InfePersonal.Visible = false;
        }

        private void mostrarInformación()
        {
            string constr = TxtURL.Text;
            var dia = "";
            var mes = "";
            var anio = "";
            var bday = "";
            var apellidoEx = "0";
            int posicion = 0;
            int posicion2 = 0;
            int largoApellido = 0;
            int excepcionApellido = 0;
            string emplid = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT EMPLID FROM SYSADM.PS_PERS_NID PN " +
                    //"WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " + //---1581737080101
                    "WHERE PN.NATIONAL_ID ='2993196360101'";
                    //"WHERE PN.NATIONAL_ID ='4681531'";
                    //"WHERE PN.NATIONAL_ID ='2327809510101'";
                    //"WHERE PN.NATIONAL_ID ='3682754340101' ";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        emplid = reader["EMPLID"].ToString();
                    }

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, " +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, CNT, MUNICIPIO_NIT, DEPARTAMENTO_NIT, STATE_NIT, PAIS_NIT, STATE FROM ( " +
                                        "SELECT PD.EMPLID, PN.NATIONAL_ID CARNE,  PD.FIRST_NAME, " +
                                        "PD.LAST_NAME, PD.SECOND_LAST_NAME, PN.NATIONAL_ID DPI, PN.NATIONAL_ID_TYPE, PP.PHONE , " +
                                        "TO_CHAR(PD.BIRTHDATE,'YYYY-MM-DD') BIRTHDATE, " +
                                        "APD.DESCR CARRERA, AGT.DESCR FACULTAD, " +
                                        "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'No Consta' END STATUS, " +
                                        "(SELECT NATIONAL_ID FROM SYSADM.PS_PERS_NID WHERE NATIONAL_ID_TYPE= 'NITREC' AND EMPLID = '" + emplid + "') NIT," +
                                        "(SELECT PNA.FIRST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "') NOMBRE_NIT, " +
                                        "(SELECT PNA.LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "') APELLIDO_NIT, " +
                                        "(SELECT SECOND_LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "') CASADA_NIT, " +
                                        "(SELECT ADDRESS1 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') DIRECCION1_NIT, " +
                                        "(SELECT ADDRESS2 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') DIRECCION2_NIT, " +
                                        "(SELECT ADDRESS3 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') DIRECCION3_NIT, " +
                                        "(SELECT C.DESCR FROM SYSADM.PS_ADDRESSES PA JOIN SYSADM.PS_COUNTRY_TBL C ON PA.COUNTRY = C.COUNTRY AND PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') PAIS_NIT, " +
                                        "(SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') MUNICIPIO_NIT, " +
                                        "(SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "') DEPARTAMENTO_NIT, " +
                                        "(SELECT ST.STATE FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ) STATE_NIT, " +
                                        "A.ADDRESS1 DIRECCION, A.ADDRESS2 DIRECCION2, A.ADDRESS3 DIRECCION3, " +
                                        "REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO, ST.STATE, " +
                                        "TT.TERM_BEGIN_DT, ROW_NUMBER() OVER (PARTITION BY PD.EMPLID ORDER BY 18 DESC) CNT, C.DESCR PAIS " +
                                        "FROM SYSADM.PS_PERS_DATA_SA_VW PD " +
                                        "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID AND ADDRESS_TYPE= 'HOME'" +
                                        "AND A.EFFDT =( " +
                                        "    SELECT " +
                                        "        MAX(EFFDT) " +
                                        "    FROM " +
                                        "        SYSADM.PS_ADDRESSES A2 " +
                                        "    WHERE " +
                                        "        A.EMPLID = A2.EMPLID " +
                                        "        AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE " +
                                        ") " +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                        "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID " +
                                        "AND SE.STDNT_ENRL_STATUS = 'E' " +
                                        "AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                        "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID " +
                                        "AND CT.STRM = SE.STRM " +
                                        "AND CT.ACAD_CAREER = SE.ACAD_CAREER " +
                                        "AND SE.INSTITUTION = CT.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                        "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                        "AND CT.INSTITUTION = APD.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                        "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                        "AND CT.INSTITUTION = TT.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_PHONE PP ON PD.EMPLID = PP.EMPLID " +
                                        "AND PP.PHONE_TYPE = 'HOME' " +
                                        "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON A.COUNTRY = C.COUNTRY " +
                                       //"WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " + //---1581737080101
                                       //"WHERE PN.NATIONAL_ID ='3682754340101' " + // de la cerda
                                       "WHERE PN.NATIONAL_ID ='2993196360101' " + // De Tezanos Rustrián
                                                                                  //"WHERE PN.NATIONAL_ID ='4681531' " + // pasaporte
                                                                                  //"WHERE PN.NATIONAL_ID ='2327809510101' " + // DE LEON
                                                                                  //"WHERE PN.NATIONAL_ID ='2993196360101' " + // 
                                       ") WHERE CNT = 1";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtCarne.Text = reader["EMPLID"].ToString();
                        txtNombre.Text = reader["FIRST_NAME"].ToString();
                        txtNInicial.Text = reader["FIRST_NAME"].ToString().Trim();
                        txtApellido.Text = reader["LAST_NAME"].ToString();
                        txtCasada.Text = reader["SECOND_LAST_NAME"].ToString();
                        txtCInicial.Text = reader["SECOND_LAST_NAME"].ToString();
                        txtAInicial.Text = reader["LAST_NAME"].ToString();
                        TxtApellidoR.Text = reader["APELLIDO_NIT"].ToString();
                        TxtNombreR.Text = reader["NOMBRE_NIT"].ToString();
                        TxtCasadaR.Text = reader["CASADA_NIT"].ToString();
                        txtNit.Text = reader["NIT"].ToString();
                        TrueNit.Text = reader["NIT"].ToString();
                        State.Text = reader["STATE"].ToString();
                        StateNIT.Text = reader["STATE_NIT"].ToString();
                        largoApellido = txtAInicial.Text.Length;// + " " + posicion.ToString();

                        if ((txtApellido.Text.Substring(0, 5)).ToUpper().Equals("DE LA"))
                        {
                            posicion = txtApellido.Text.Substring(6, largoApellido - 6).IndexOf(" ");
                            txtContaador.Text = txtAInicial.Text.Length.ToString() + " " + posicion.ToString();
                            txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 6);
                        }
                        else
                        {
                            posicion = reader["LAST_NAME"].ToString().IndexOf(" ");
                            if (posicion > 0)
                            {
                                apellidoEx = divisionApellidos(reader["LAST_NAME"].ToString().Substring(0, posicion));
                                txtContaador.Text = apellidoEx.ToString();
                                excepcionApellido = apellidoEx.ToString().IndexOf("    }");
                                txtContaador.Text = apellidoEx.ToString().Substring(excepcionApellido - 3, 1);
                                if (apellidoEx.ToString().Substring(excepcionApellido - 3, 1).Equals("1"))
                                {
                                    posicion2 = txtApellido.Text.Substring(posicion + 1, largoApellido - (posicion + 1)).IndexOf(" ");
                                    txtContaador.Text = posicion2.ToString();
                                    txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 1 + posicion2);
                                }
                            }
                            if (txtPrimerApellido.Text.IsNullOrWhiteSpace())
                            {
                                txtPrimerApellido.Text = getBetween(txtApellido.Text, "", " ");
                            }
                        }

                        txtDPI.Text = reader["DPI"].ToString();
                        CmbEstado.SelectedValue = reader["STATUS"].ToString();

                        bday = reader["BIRTHDATE"].ToString();
                        anio = bday.Substring(0, 4);
                        mes = bday.Substring(5, 2);
                        dia = bday.Substring(8, 2);
                        txtCumple.Text = dia + "-" + mes + "-" + anio;

                        txtDireccion.Text = reader["DIRECCION"].ToString();
                        TrueDir.Text = reader["DIRECCION"].ToString();
                        txtDireccion2.Text = reader["DIRECCION2"].ToString();
                        txtDireccion3.Text = reader["DIRECCION3"].ToString();
                        TxtDiRe1.Text = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2.Text = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3.Text = reader["DIRECCION3_NIT"].ToString();
                        if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                            CmbPais.SelectedValue = reader["PAIS"].ToString();
                        else
                            CmbPais.SelectedValue = "";
                        llenadoDepartamento();
                        CmbDepartamento.SelectedValue = reader["DEPARTAMENTO"].ToString();
                        llenadoMunicipio();
                        CmbMunicipio.SelectedValue = reader["MUNICIPIO"].ToString();

                        if (!String.IsNullOrEmpty(reader["PAIS_NIT"].ToString()) && RadioButtonNombreNo.Checked)
                        {
                            llenadoPaisnit();
                            CmbPaisNIT.SelectedValue = reader["PAIS_NIT"].ToString();
                            llenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO_NIT"].ToString();
                            llenadoMunicipioNIT();
                            CmbMunicipioNIT.SelectedValue = reader["MUNICIPIO_NIT"].ToString();
                        }
                        else if (RadioButtonNombreSi.Checked)
                        {
                            llenadoPaisnit();
                            if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                                CmbPaisNIT.SelectedValue = reader["PAIS"].ToString();
                            else
                                CmbPaisNIT.SelectedValue = "";
                            llenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO"].ToString();
                            llenadoMunicipioNIT();
                            CmbMunicipioNIT.SelectedValue = reader["MUNICIPIO"].ToString();
                        }
                        else
                        {
                            llenadoPaisnit();
                            llenadoDepartamentoNit();
                            llenadoMunicipioNIT();
                        }
                        txtTelefono.Text = reader["PHONE"].ToString();
                        TruePhone.Text = reader["PHONE"].ToString();
                        txtCarrera.Text = reader["CARRERA"].ToString();
                        txtFacultad.Text = reader["FACULTAD"].ToString();
                        UserEmplid.Text = reader["EMPLID"].ToString();
                    }

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT NOMBRE_COMPLETO FROM UNIS_INTERFACES.TBL_FACULTADES WHERE NOMBRE_CAMPUS ='" + txtFacultad.Text.TrimEnd().TrimStart() + "'";
                    OracleDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        txtFacultad.Text = reader2["NOMBRE_COMPLETO"].ToString();
                    }
                    con.Close();

                    /*if (txtNit.Text == "CF")
                    {
                        Combos.Visible = false;
                    }*/
                    fotoAlmacenada();
                }
            }
        }

        private void fotoAlmacenada()
        {
            string constr = TxtURL.Text;
            int contador;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                    OracleDataReader reader3 = cmd.ExecuteReader();
                    while (reader3.Read())
                    {
                        contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                        if (contador > 0)
                        {
                            ImgBase.Visible = true;
                            ImgBase.ImageUrl = "~/Usuarios/UltimasCargas/" + txtCarne.Text + ".jpg";
                            byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/" + txtCarne.Text + ".jpg");
                            string base64String = Convert.ToBase64String(imageBytes);
                            string script = $@"<script type='text/javascript'>
                                            document.getElementById('urlPath').value = '{base64String}';
                                            </script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "SetUrlPathValue", script);
                        }
                    }
                    con.Close();

                }
            }
        }
        public class DatosDepartamento
        {
            public string Texto { get; set; }
            public string Valor { get; set; }
        }

        protected void CmbDeptos()
        {
            string constr = TxtURL.Text;
            string query = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPais.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
            using (OracleConnection con = new OracleConnection(constr))

            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatosDepartamento datosDepartamento = new DatosDepartamento();
                            datosDepartamento.Texto = reader.GetString(0);
                            datosDepartamento.Valor = reader.GetString(0);
                        }
                    }

                }
            }

        }
        protected void llenadoDepartamento()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPais.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDepartamento.DataSource = ds;
                        CmbDepartamento.DataTextField = "DEPARTAMENTO";
                        CmbDepartamento.DataValueField = "DEPARTAMENTO";
                        CmbDepartamento.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamento.DataTextField = "";
                        CmbDepartamento.DataValueField = "";
                    }
                }
            }
        }
        public void llenadoDepartamentoNit()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPaisNIT.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDepartamentoNIT.DataSource = ds;
                        CmbDepartamentoNIT.DataTextField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataValueField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamentoNIT.DataTextField = "";
                        CmbDepartamentoNIT.DataValueField = "";
                    }
                }
            }
        }

        protected void DeptoCF()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO " +
                        "REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO FROM SYSADM.PS_STATE_TBL ST  " +
                        "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                        "WHERE ST=STATE ='" + State.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                        "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        CmbDepartamentoNIT.DataTextField = "DEPARTAMENTO";
                        CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamentoNIT.DataTextField = "";
                        CmbMunicipioNIT.DataValueField = "";
                    }
                }
            }
        }
        public void llenadoMunicipioNIT()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(CmbDepartamentoNIT.SelectedValue.ToString()))
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                            "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamentoNIT.SelectedValue + "') " +
                            "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                        else
                        {
                            CmbMunicipioNIT.DataSource = "-";
                            CmbMunicipioNIT.DataTextField = "-";
                            CmbMunicipioNIT.DataValueField = "-";
                        }
                    }
                    catch (Exception)
                    {
                        CmbMunicipioNIT.DataSource = "-";
                        CmbMunicipioNIT.DataTextField = "-";
                        CmbMunicipioNIT.DataValueField = "-";
                    }
                }
            }
        }
        protected void llenadoMunicipio()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                        "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamento.SelectedValue + "') " +
                        "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbMunicipio.DataSource = ds;
                        CmbMunicipio.DataTextField = "MUNICIPIO";
                        CmbMunicipio.DataValueField = "MUNICIPIO";
                        CmbMunicipio.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbMunicipio.DataSource = "-";
                        CmbMunicipio.DataTextField = "-";
                        CmbMunicipio.DataValueField = "-";
                    }
                }
            }
        }
        protected void llenadoPais()
        {
            string where = "";
            if (!String.IsNullOrEmpty(CmbPais.Text))
                where = "WHERE COUNTRY='" + CmbPais.Text + "'";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' PAIS, ' ' COUNTRY FROM DUAL UNION SELECT * FROM (SELECT DESCR AS PAIS, COUNTRY FROM SYSADM.PS_COUNTRY_TBL " + where + ")PAIS ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbPais.DataSource = ds;
                    CmbPais.DataTextField = "PAIS";
                    CmbPais.DataValueField = "PAIS";
                    CmbPais.DataBind();
                    con.Close();
                }
            }
        }
        public void llenadoPaisnit()
        {
            string where = "";
            if (CmbPaisNIT.Text != " " && !String.IsNullOrEmpty(CmbPaisNIT.Text))
            {
                if (CmbPaisNIT.Text.Length == 3)
                    where = "WHERE COUNTRY='" + CmbPaisNIT.Text + "'";
                else
                    where = "WHERE DESCR='" + CmbPaisNIT.Text + "'";
            }
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' PAIS, ' ' COUNTRY FROM DUAL UNION SELECT * FROM (SELECT DESCR AS PAIS, COUNTRY FROM SYSADM.PS_COUNTRY_TBL " + where + ")PAIS ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbPaisNIT.DataSource = ds;
                    CmbPaisNIT.DataTextField = "PAIS";
                    CmbPaisNIT.DataValueField = "PAIS";
                    CmbPaisNIT.DataBind();
                    con.Close();
                }
            }
        }
        protected void llenadoState()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (!String.IsNullOrEmpty(CmbMunicipio.SelectedValue))
                    {
                        string descrip = "";
                        if (CmbPais.SelectedValue == "Guatemala")
                        {
                            descrip = CmbMunicipio.SelectedValue + "-" + CmbDepartamento.SelectedValue;
                        }
                        else
                        {
                            descrip = CmbDepartamento.SelectedValue;
                        }
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + descrip.TrimEnd('-') + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            State.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                    else
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbDepartamento.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            State.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                }
            }
        }
        protected void llenadoStateNIT()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (!String.IsNullOrEmpty(CmbMunicipioNIT.SelectedValue))
                    {
                        string descrip = "";
                        if (CmbPais.SelectedValue == "Guatemala")
                        {
                            descrip = CmbMunicipioNIT.SelectedValue + "-" + CmbDepartamentoNIT.SelectedValue;
                        }
                        else
                        {
                            descrip = CmbDepartamentoNIT.SelectedValue;
                        }
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + descrip.TrimEnd('-') + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNIT.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                    else
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbDepartamentoNIT.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNIT.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                }
            }
        }
        protected string estadoCivil()
        {
            var VALOR = CmbEstado.SelectedValue.Substring(0, 1).ToString();
            if (VALOR.Equals("S"))
            {
                VALOR = "S";
            }
            else if (VALOR.Equals("C"))
            {
                VALOR = "M";
            }
            else
            {
                VALOR = "U";
            }
            return VALOR;
        }
        protected int PantallaHabilitada(string PANTALLA)
        {
            txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE " +
                        "WHERE TO_CHAR(SYSDATE,'YYYY-MM-DD') " +
                        "BETWEEN FECHA_INICIO AND FECHA_FIN " +
                        "AND PANTALLA ='" + PANTALLA + "'";
            string constr = TxtURL.Text;
            string control = "0";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE " +
                        "WHERE TO_CHAR(SYSDATE,'YYYY-MM-DD') " +
                        "BETWEEN FECHA_INICIO AND FECHA_FIN " +
                        "AND PANTALLA ='" + PANTALLA + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONTADOR"].ToString();
                        }

                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return Convert.ToInt32(control);
        }

        private string actualizarInformacion()
        {
            string cameraAvailable = hdnCameraAvailable.Value;
            bool archivoDescargado = false;
            if (cameraAvailable == "true")
            {
                int contador = 0;

                if (txtAInicial.Text == txtApellido.Text && txtNInicial.Text == txtNombre.Text && txtCInicial.Text == txtCasada.Text)
                {
                    txtAccion.Text = "1";
                    txtTipoAccion.Text = "1.1";
                    txtConfirmacion.Text = "02"; //VALIDACIÓN DE FOTOGRAFÍA

                    if (RadioButtonNombreNo.Checked)
                    {
                        if (!CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                        {
                            ValidacionParaIngreso();
                        }
                        else
                        {
                            mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        TxtNombreR.Text = txtNombre.Text;
                        TxtApellidoR.Text = txtApellido.Text;
                        TxtCasadaR.Text = txtCasada.Text;
                        TxtDiRe1.Text = txtDireccion.Text;
                        TxtDiRe2.Text = txtDireccion2.Text;
                        TxtDiRe3.Text = txtDireccion3.Text;
                        txtNit.Text = "CF";
                        ValidacionParaIngreso();
                    }

                }
                else
                {
                    if (FileUpload2.HasFile)
                    {
                        foreach (HttpPostedFile uploadedFile in FileUpload2.PostedFiles)
                        {
                            contador++;
                            string nombreArchivo = txtCarne.Text + "(" + contador + ").jpg";
                            string ruta = CurrentDirectory + "/Usuarios/DPI/" + nombreArchivo;
                            uploadedFile.SaveAs(ruta);
                        }
                        txtAccion.Text = "1";
                        txtTipoAccion.Text = "1.1";
                        txtConfirmacion.Text = "01"; //Requiere confirmación de operador 
                        txtCantidadImagenesDpi.Text = contador.ToString();
                        IngresoDatos();
                    }
                    else
                    {
                        if (txtAInicial.Text != txtApellido.Text || txtNInicial.Text != txtNombre.Text || txtCInicial.Text != txtCasada.Text)
                        {
                            CargaDPI.Attributes["style"] = "display: block";
                            string script = "<script>Documentos();</script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        }
                        fotoAlmacenada();
                    }
                }
                return mensaje;
            }
            else
            {
                lblActualizacion.ForeColor = System.Drawing.Color.Black;
                lblActualizacion.Text = "La cámara no tiene permisos disponibles. <br /> " +
                    "Para asignar los permisos correspondientes, descargué el manual y siga las instrucciones, al finalizar, haga clic en el botón de Recargar Página.<br />";
                mensaje = "0";
                controlCamposVisibles();
                BtnDownload.Visible = true;
                BtnReload.Visible = true;
            }
            return mensaje;

        }

        public void DescargaArchivo()
        {
            //CurrentDirectory + "/Usuarios/FotosConfirmación/
            string rutaArchivo = CurrentDirectory + "/Manuales/";
            string nombreArchivo = "Manual borrador.docx";
            // Configurar las cabeceras de la respuesta
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);

            // Descargar el archivo
            Response.TransmitFile(rutaArchivo + nombreArchivo);
            Response.Flush();
        }

        //Eventos       
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoState();
            fotoAlmacenada();
        }
        protected void CmbMunicipioNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoStateNIT();
            fotoAlmacenada();
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoMunicipio();
            llenadoState();
            fotoAlmacenada();
        }
        protected void CmbDepartamentoNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoMunicipioNIT();
            llenadoStateNIT();
            fotoAlmacenada();
        }

        public static int ContarEspacios(string texto)
        {
            int contador = 0;
            string letra;

            for (int i = 0; i < texto.Length; i++)
            {
                letra = texto.Substring(i, 1);

                if (letra == " ")
                {
                    contador++;
                }
            }

            return contador;
        }

        protected void ValidacionParaIngreso()
        {
            if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(CmbPais.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text) && !String.IsNullOrEmpty(TxtNombreR.Text) && !String.IsNullOrEmpty(TxtApellidoR.Text))
            {
                IngresoDatos();
            }
            else
            {
                mensaje = "No puede enviarse información vacía y es necesario seleccionar el estado civil, un país y también ingresar un departamento y un muncipio";
            }
        }
        protected string IngresoDatos()
        {
            if (!Request.Form["urlPath"].IsNullOrWhiteSpace())
            {
                if (String.IsNullOrEmpty(txtNit.Text))
                {
                    txtNit.Text = "CF";
                }

                try
                {
                    txtNombreAPEX.Text = null;
                    string constr = TxtURL.Text;
                    string codPais = "";
                    string codPaisNIT = "";
                    string ec = estadoCivil();
                    string RegistroCarne = "0";
                    var apellidoEx = "0";
                    int posicion = 0;
                    int posicion2 = 0;
                    int largoApellido = txtApellido.Text.Length;
                    int excepcionApellido = 0;
                    int espaciosApellido = ContarEspacios(txtApellido.Text);
                    int espaciosNombre = ContarEspacios(txtNombre.Text);
                    string[] nombres = txtNombre.Text.TrimEnd(' ').Split(' ');
                    int nombresTotal = nombres.Length;
                    if ((txtApellido.Text.Substring(0, 5)).ToUpper().Equals("DE LA"))
                    {
                        posicion = txtApellido.Text.Substring(6, largoApellido - 6).IndexOf(" ");
                        txtContaador.Text = txtAInicial.Text.Length.ToString() + " " + posicion.ToString();
                        txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 6);
                    }
                    else
                    {
                        posicion = txtApellido.Text.IndexOf(" ");
                        if (posicion > 0)
                        {
                            apellidoEx = divisionApellidos(txtApellido.ToString().Substring(0, posicion));
                            txtContaador.Text = apellidoEx.ToString();
                            excepcionApellido = apellidoEx.ToString().IndexOf("    }");
                            txtContaador.Text = apellidoEx.ToString().Substring(excepcionApellido - 3, 1);
                            if (apellidoEx.ToString().Substring(excepcionApellido - 3, 1).Equals("1"))
                            {
                                posicion2 = txtApellido.Text.Substring(posicion + 1, largoApellido - (posicion + 1)).IndexOf(" ");
                                txtContaador.Text = posicion2.ToString();
                                txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 1 + posicion2);
                            }
                            if (txtPrimerApellido.Text.IsNullOrWhiteSpace())
                            {
                                txtPrimerApellido.Text = getBetween(txtApellido.Text, "", " ");
                            }
                        }
                    }

                    if (nombresTotal > 1)
                    {
                        for (int i = 1; i < nombresTotal; i++)
                        {
                            txtNombreAPEX.Text = txtNombreAPEX.Text + " " + nombres[i];
                        }
                    }

                    txtNombreAPEX.Text.TrimStart(' ');
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        con.Open();
                        OracleTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        using (OracleCommand cmd = new OracleCommand())
                        {

                            cmd.Transaction = transaction;
                            //Obtener codigo país
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT COUNTRY FROM SYSADM.PS_COUNTRY_TBL WHERE DESCR = '" + CmbPais.SelectedValue + "'";
                            OracleDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                codPais = reader["COUNTRY"].ToString();
                            }

                            //Obtener codigo país
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT COUNTRY FROM SYSADM.PS_COUNTRY_TBL WHERE DESCR = '" + CmbPaisNIT.SelectedValue + "'";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                codPaisNIT = reader["COUNTRY"].ToString();
                            }

                            //SE VALIDA QUE NO EXISTA INFORMACIÓN REGISTRADA
                            cmd.Transaction = transaction;
                            cmd.Connection = con;
                            txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET =SUBSTR('" + txtCarne.Text + "',0,13)";
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET =SUBSTR('" + txtCarne.Text + "',0,13)";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                RegistroCarne = reader["CONTADOR"].ToString();
                            }
                            //txtExiste.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARGO = '" + txtCarrera.Text + "' AND FACULTAD ='" + txtFacultad.Text + "' AND CARNET =SUBSTR('" + txtCarne.Text + "',0,9)";
                            txtExiste.Text = RegistroCarne.ToString() + " registros";

                            if (RegistroCarne == "0")
                            {
                                string nombreArchivo = txtCarne.Text + ".jpg";
                                string ruta = txtPath.Text + nombreArchivo;
                                //string fileName = Context.User.Identity.Name.Replace("@unis.edu.gt", "") + ".jpg";
                                SaveCanvasImage(Request.Form["urlPath"], txtPath.Text, txtCarne.Text + ".jpg");
                                if (txtConfirmacion.Text == "01")
                                {
                                    SaveCanvasImage(Request.Form["urlPath"], CurrentDirectory + "/Usuarios/FotosConfirmación/", txtCarne.Text + ".jpg");
                                }
                                else
                                {
                                    SaveCanvasImage(Request.Form["urlPath"], CurrentDirectory + "/Usuarios/Fotos/", txtCarne.Text + ".jpg");
                                }

                                cmd.Transaction = transaction;
                                //Obtener codigo país
                                txtExiste3.Text = txtPrimerApellido.Text + " insert";
                                if (espaciosApellido > 0)
                                {
                                    if (txtApellido.Text.Length - txtPrimerApellido.Text.Length - 1 > 0)
                                    {
                                        txtApellidoAPEX.Text = txtApellido.Text.Substring((txtPrimerApellido.Text.Length + 1), (txtApellido.Text.Length - txtPrimerApellido.Text.Length - 1));
                                    }
                                    else
                                    {
                                        txtApellidoAPEX.Text = " ";
                                    }
                                }
                                else
                                {
                                    txtPrimerApellido.Text = txtApellido.Text;
                                    txtApellidoAPEX.Text = " ";
                                }

                                if (String.IsNullOrEmpty(StateNIT.Text))
                                    StateNIT.Text = State.Text;

                                cmd.Connection = con;
                                cmd.CommandText = "SELECT 'INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte, Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, " +
                                                //txtInsert.Text = "SELECT 'INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte, Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, " +
                                                " Fec_Emision, NO_CTA_BI, ID_AGENCIA, CONFIRMACION,TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_R) VALUES ('''" +
                                                "||'" + txtPrimerApellido.Text + "'''||'," + //APELLIDO1
                                                "''" + txtApellidoAPEX.Text + //APELLIDO2
                                                "'','||''''||SUBSTR(CARNE,0,13)||''''||','" + //CARNE
                                                "||''''||CEDULA||''''||','" + //CEDULA
                                                "||'''" + txtCasada.Text + "'''||','" +// APELLIDO DE CASADA
                                                "||''''||UPPER(DEPARTAMENTO)||''''||','" + //DEPARTAMENTO DE RESIDENCIA
                                                "||''''||SUBSTR(DIRECCION,0,29)||''''||','" + // DIRECCION
                                                "||''''||EMAIL||''''||','" + // CORREO ELECTRONICO
                                                "||STATUS||','" + // ESTADO CIVIL
                                                "||'''" + txtFacultad.Text + "'''||','" + // FACULTAD
                                                "||''''||BIRTHDATE||''''||','" + //FECHA DE NACIMIENTO
                                                "||''''||FLAG_CED||''''||','" +
                                                "||''''||FLAG_DPI||''''||','" +
                                                "||''''||FLAG_PAS||''''||','" +
                                                "||''''||UPPER(MUNICIPIO)||''''||'," + //MUNICIPIO DE RESIDENCIA
                                                "''" + txtNit.Text + "'''||','" +//NIT
                                                "||''''||DPI||''''||','" + // NO_CUI
                                                "||''''||PASAPORTE||''''||','" + // NUMERO DE PASAPORTE
                                                "||'''" + nombres[0].ToString() + "'''||','" + //NOMBRE1
                                                "||'''" + txtNombreAPEX.Text + "'''||','" +// NOMBRE 2
                                                "||''''||FIRST_NAME||' '||'" + txtPrimerApellido.Text + "'||''''||','" + //APELLIDO DE IMPRESION
                                                "||''''||PLACE||''''||','" + // PAIS NACIONALIDAD
                                                "||''''||PROF||''''||','" + // PROFESION
                                                "||SEX||'," + // SEXO
                                                "NULL," + //TELEFONO
                                                "NULL," + //ZONA
                                                "" + txtAccion.Text + ",'" + //ACCION
                                                "||''''||SUBSTR(PHONE,0,8)||''''||','" + //CELULAR
                                                "||CODIGO_BARRAS||','" + //CODIGO DE BARRAS
                                                "||''''||CONDMIG||''''||','" + //CONDICION MIGRANTE
                                                "||'2022,'" + //ID  UNIVERSIDAD
                                                "||''''||PAIS_PASAPORTE||''''||','" + //PAIS PASAPORTE
                                                "'" + txtTipoAccion.Text +  //TIPO_ACCION
                                                "'','||2||'," + //TIPO PERSONA
                                                "''" + codPaisNIT + "'''||','" + // PAIS NIT
                                                "||''''||DEPARTAMENTO_CUI||''''||','" + // DEPARTAMENTO CUI
                                                "||''''||MUNICIPIO_CUI||''''||'," + //MUNICIPIO CUI
                                                "1," + //VALIDAR ENVIO
                                                "'||'''" + ruta + "'''||'," + //PATH
                                                "NULL," + //CODIGO
                                                "NULL,'" + // DEPARTAMENTO
                                                "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_HORA
                                                "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_ENTREGA
                                                "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_SOLICITADO
                                                "||TIPO_DOCUMENTO||','" + //TIPO DOCUMENTO
                                                "||'''" + txtCarrera.Text + "'''||','" + //CARGO
                                                "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||'" +//FECHA_EMISION
                                                ", 0," + //NO CTA BI
                                                " 2002," +//ID AGENCIA
                                                "" + txtConfirmacion.Text + "," + txtCantidadImagenesDpi.Text +// confirmación operador
                                                ",'||'''" + TxtNombreR.Text + "'''||','" + //NOMBRE
                                                "||'''" + TxtApellidoR.Text + "'''||','" +
                                                "||'''" + TxtCasadaR.Text + "'''||','" +
                                                "||'''" + TxtDiRe1.Text + "'''||','" +
                                                "||'''" + TxtDiRe2.Text + "'''||','" +
                                                "||'''" + TxtDiRe3.Text + "'''||','" +
                                                "||'''" + StateNIT.Text + "'''||','" +
                                                "||'''" + CmbPais.Text + "'''||')'" +
                                                " AS INS " +
                                                "FROM ( SELECT " +
                                                "DISTINCT PD.EMPLID CARNE, " +
                                                "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) CODIGO_BARRAS, " +
                                                "REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+') FIRST_NAME, " +
                                                "SUBSTR(PD.FIRST_NAME,  LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))+2, LENGTH(PD.FIRST_NAME)-LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))) SECOND_NAME, " +
                                                "PD.LAST_NAME, PD.BIRTHCOUNTRY," +
                                                "PD.SECOND_LAST_NAME, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,0,9)" +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,0,9) ELSE '' END DPI, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,12,2) " +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,12,2) ELSE '' END MUNICIPIO_CUI," +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN  SUBSTR(PN.NATIONAL_ID,10,2) " +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,10,2) ELSE '' END DEPARTAMENTO_CUI," +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                                "    WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_DPI, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN C.DESCR WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN C.DESCR ELSE NULL END PAIS_PASAPORTE, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '2' " +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '2'" +
                                                "     WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '3' ELSE ' ' END TIPO_DOCUMENTO," +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_CED, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE NULL END PASAPORTE, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_PAS, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN 'RESIDENTE PERM' ELSE NULL END CONDMIG, " +
                                                "PPD.PHONE, " +
                                                "TO_CHAR(PD.BIRTHDATE, 'DD-MM-YYYY') BIRTHDATE, " +
                                                //"APD.DESCR CARRERA, " +
                                                "AGT.DESCR FACULTAD, " +
                                                "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE '' END SEX, " +
                                                "CASE WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'Condición Migrante' WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE C.DESCR END PLACE," +
                                                "CASE WHEN PD.MAR_STATUS = 'M' THEN '2' WHEN PD.MAR_STATUS = 'S' THEN '1' ELSE '' END STATUS, " +
                                                "(select REPLACE(A1.ADDRESS1,'|' , ' ') || ' ' ||  REPLACE(A1.ADDRESS2,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) DIRECCION, " +
                                                " (select REPLACE(A1.ADDRESS3,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) ZONA, " +
                                                "REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO, " +
                                                "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, " +
                                                "'ESTUDIANTE' PROF, " +
                                                "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = PD.EMPLID AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) EMAIL " +
                                                "FROM " +
                                                "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                                "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                                "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON C.COUNTRY = PD.BIRTHCOUNTRY " +
                                                "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                                "AND A.EFFDT =(SELECT MAX(EFFDT) FROM SYSADM.PS_ADDRESSES A2 WHERE A.EMPLID = A2.EMPLID AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE) " +
                                                "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                                "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                                "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID AND SE.STDNT_ENRL_STATUS = 'E' AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                                "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID AND CT.STRM = SE.STRM AND CT.ACAD_CAREER = SE.ACAD_CAREER AND SE.INSTITUTION = CT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                                //"--WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                                                //"WHERE PN.NATIONAL_ID ='4681531')" +
                                                "WHERE PN.NATIONAL_ID ='2993196360101')" +
                                                //"WHERE PN.NATIONAL_ID ='2327809510101')" +
                                                //"WHERE PN.NATIONAL_ID ='2990723550101')" +
                                                //"WHERE PN.NATIONAL_ID ='3682754340101')" +
                                                "WHERE CODIGO_BARRAS=DPI||DEPARTAMENTO_CUI||MUNICIPIO_CUI OR CODIGO_BARRAS=PASAPORTE OR CODIGO_BARRAS=CEDULA " +
                                                "ORDER BY 1 ASC";
                                //--4681531 PASAPORTE
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    txtInsert.Text = reader["INS"].ToString();
                                }
                                cmd.Transaction = transaction;
                                cmd.Connection = con;
                            };

                            try
                            {
                                if (String.IsNullOrEmpty(State.Text))
                                    State.Text = " ";
                                if (String.IsNullOrEmpty(txtDireccion2.Text))
                                    txtDireccion2.Text = " ";
                                if (String.IsNullOrEmpty(txtDireccion3.Text))
                                    txtDireccion3.Text = " ";
                                if (String.IsNullOrEmpty(StateNIT.Text))
                                    StateNIT.Text = " ";
                                if (String.IsNullOrEmpty(TxtDiRe2.Text))
                                    TxtDiRe2.Text = " ";
                                if (String.IsNullOrEmpty(TxtDiRe3.Text))
                                    TxtDiRe3.Text = " ";
                                if (String.IsNullOrEmpty(txtCasada.Text))
                                    txtCasada.Text = " ";
                                if (String.IsNullOrEmpty(TxtCasadaR.Text))
                                    TxtCasadaR.Text = " ";
                                if (String.IsNullOrEmpty(TxtApellidoR.Text))
                                    TxtApellidoR.Text = " ";
                                if (String.IsNullOrEmpty(TxtNombreR.Text))
                                    TxtNombreR.Text = " ";
                                if (RadioButtonNombreSi.Checked)
                                {
                                    TxtNombreR.Text = txtNombre.Text;
                                    TxtApellidoR.Text = txtApellido.Text;
                                    TxtCasadaR.Text = txtCasada.Text;
                                    TxtDiRe1.Text = txtDireccion.Text;
                                    TxtDiRe2.Text = txtDireccion2.Text;
                                    TxtDiRe3.Text = txtDireccion3.Text;
                                    txtNit.Text = "CF";
                                }
                                //Telefono y direccion
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_DATA PPD SET PPD.PHONE = '" + txtTelefono.Text + "', PPD.STATE =  '" + State.Text + "', " +
                                    "PPD.ADDRESS1 = '" + txtDireccion.Text + "', PPD.ADDRESS2 = '" + txtDireccion2.Text + "', PPD.ADDRESS3 = '" + txtDireccion3.Text + "', PPD.COUNTRY = '" + codPais + "',LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "' WHERE PPD.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                                //Numero de Telefono
                                if (!String.IsNullOrEmpty(TruePhone.Text))
                                {
                                    //TruePhone.Text = "UPDATE SYSADM.PS_PERSONAL_PHONE PP SET PP.PHONE = '" + txtTelefono.Text + "' WHERE PP.EMPLID = '" + UserEmplid.Text + "' AND PP.PHONE_TYPE='HOME'";
                                    cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_PHONE PP SET PP.PHONE = '" + txtTelefono.Text + "'" +
                                                        "WHERE PP.EMPLID = '" + UserEmplid.Text + "' AND PP.PHONE_TYPE='HOME'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "INSERT INTO SYSADM.PS_PERSONAL_PHONE (EMPLID, PHONE_TYPE,COUNTRY_CODE,EXTENSION,PHONE,PREF_PHONE_FLAG) VALUES ('" + UserEmplid.Text + "', 'HOME',' ',' ',  '" + txtTelefono.Text + "', 'Y')";
                                    cmd.ExecuteNonQuery();
                                }
                                //Direccion
                                if (!String.IsNullOrEmpty(TrueDir.Text))
                                {
                                    cmd.CommandText = "UPDATE SYSADM.PS_ADDRESSES A SET A.STATE =  '" + State.Text + "', " +
                                        "A.ADDRESS1 = '" + txtDireccion.Text + "', " +
                                        "A.ADDRESS2 = '" + txtDireccion2.Text + "', " +
                                        "A.ADDRESS3 = '" + txtDireccion3.Text + "', " +
                                        "A.COUNTRY = '" + codPais + "', LASTUPDOPRID ='" + TextUser.Text + "',  LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "' WHERE A.EMPLID = '" + UserEmplid.Text + "' AND ADDRESS_TYPE ='HOME'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES (EMPLID, ADDRESS_TYPE,COUNTY,CITY,NUM1, NUM2, HOUSE_TYPE, ADDR_FIELD1, ADDR_FIELD2, ADDR_FIELD3,POSTAL,GEO_CODE,IN_CITY_LIMIT,ADDRESS1_AC,ADDRESS2_AC,ADDRESS3_AC,CITY_AC,REG_REGION,EFFDT,EFF_STATUS,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,STATE,LASTUPDDTTM,LASTUPDOPRID) " +
                                        "VALUES('" + UserEmplid.Text + "', 'HOME',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'A', '" + codPais + "', '" + txtDireccion.Text + "', '" + txtDireccion2.Text + "', '" + txtDireccion3.Text + "', ' ','" + State.Text + "', '" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "','" + TextUser.Text + "')";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES_SA (LOC_ADDR_DATA,LOC_ADDR, LOC_ADDR_LINE, LOC_ADDR_TYPE, EXT_ORG_ID, DESCR_EXT_ORG, DESCR_ORG_LOCATION, CONTACT_NAME, DATA_SOURCE, EMPLID, ADDRESS_TYPE,EFFDT,ORG_LOCATION,MAINT_ADDR_MANUAL,MAINT_OTHER_MANUAL,ORG_CONTACT,SEASONAL_ADDR) " +
                                        "VALUES(' ',' ',' ',' ',' ',' ',' ',' ',' ','" + UserEmplid.Text + "', 'HOME', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 0, 'N','N',0,'N')";
                                    cmd.ExecuteNonQuery();
                                }
                                //Estado Civil
                                cmd.CommandText = "UPDATE SYSADM.PS_PERS_DATA_EFFDT PD SET PD.MAR_STATUS = '" + ec + "', " +
                                                    "LASTUPDDTTM = SYSDATE , " +
                                                    "LASTUPDOPRID = '" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "'" +
                                                    " WHERE PD.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();

                                if (!String.IsNullOrEmpty(TxtNombreR.Text))
                                {
                                    if (txtAInicial.Text == txtApellido.Text && txtNInicial.Text == txtNombre.Text && txtCInicial.Text == txtCasada.Text)
                                    {
                                        if (!String.IsNullOrEmpty(TrueNit.Text))
                                        {
                                            //TruePhone.Text = "UPDATE SYSADM.PS_PERSONAL_PHONE PP SET PP.PHONE = '" + txtTelefono.Text + "' WHERE PP.EMPLID = '" + UserEmplid.Text + "' AND PP.PHONE_TYPE='HOME'";
                                            //ACTUALIZA NOMBRE DEL NIT
                                            cmd.CommandText = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = REPLACE('" + TxtApellidoR.Text + " " + TxtCasadaR.Text + "," + TxtNombreR.Text + "','  ',' ') , " +
                                                                "PN.LAST_NAME_SRCH =REPLACE(UPPER('" + TxtApellidoR.Text + "'),' ',''), " +
                                                                "PN.FIRST_NAME_SRCH=REPLACE(UPPER('" + TxtNombreR.Text + "'),' ',''), " +
                                                                "LAST_NAME ='" + TxtApellidoR.Text + "', FIRST_NAME='" + TxtNombreR.Text + "', " +
                                                                "SECOND_LAST_NAME='" + TxtCasadaR.Text + "', SECOND_LAST_SRCH=REPLACE(UPPER('" + TxtCasadaR.Text + "'),' ','')||' ', " +
                                                                "NAME_DISPLAY='" + TxtNombreR.Text + " " + TxtApellidoR.Text + " " + TxtCasadaR.Text + "', " +
                                                                "NAME_FORMAL='" + TxtNombreR.Text + " " + TxtApellidoR.Text + " " + TxtCasadaR.Text + "', " +
                                                                "NAME_DISPLAY_SRCH =UPPER(REPLACE('" + TxtNombreR.Text + TxtApellidoR.Text + TxtCasadaR.Text + "',' ',''))," +
                                                                "LASTUPDDTTM = SYSDATE, " +
                                                                "LASTUPDOPRID = '" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "' " +
                                                                "WHERE PN.EMPLID = '" + UserEmplid.Text + "' AND NAME_TYPE IN 'REC'";
                                            cmd.ExecuteNonQuery();

                                            //ACTUALIZA NIT
                                            cmd.CommandText = "UPDATE SYSADM.PS_PERS_NID PN SET PN.NATIONAL_ID = '" + txtNit.Text + "', " +
                                                                "LASTUPDDTTM = SYSDATE, " +
                                                                "LASTUPDOPRID = '" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "'" +
                                                                " WHERE PN.NATIONAL_ID_TYPE = 'NITREC' AND PN.EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            //ACTUALIZA DIRECCION DEL NIT
                                            cmd.CommandText = "UPDATE SYSADM.PS_ADDRESSES A SET A.STATE =  '" + StateNIT.Text + "', " +
                                                "A.ADDRESS1 = '" + TxtDiRe1.Text + "', " +
                                                "A.ADDRESS2 = '" + TxtDiRe2.Text + "', " +
                                                "A.ADDRESS3 = '" + TxtDiRe3.Text + "', " +
                                                "A.COUNTRY = '" + codPais + "', LASTUPDOPRID ='" + TextUser.Text + "',  LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") +
                                                "' WHERE A.EMPLID = '" + UserEmplid.Text + "' AND ADDRESS_TYPE ='REC'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            cmd.CommandText = "INSERT INTO SYSADM.PS_NAMES (EMPLID, NAME_TYPE, EFFDT, EFF_STATUS, COUNTRY_NM_FORMAT, NAME, NAME_INITIALS, NAME_PREFIX, NAME_SUFFIX, NAME_ROYAL_PREFIX, NAME_ROYAL_SUFFIX, NAME_TITLE, LAST_NAME_SRCH, FIRST_NAME_SRCH, LAST_NAME, FIRST_NAME, MIDDLE_NAME, SECOND_LAST_NAME, SECOND_LAST_SRCH, NAME_AC, PREF_FIRST_NAME, PARTNER_LAST_NAME, PARTNER_ROY_PREFIX, LAST_NAME_PREF_NLD, NAME_DISPLAY, NAME_FORMAL, NAME_DISPLAY_SRCH, LASTUPDDTTM, LASTUPDOPRID) " +
                                                "VALUES('" + UserEmplid.Text + "','REC','01/01/00','A','MEX',REPLACE('" + TxtApellidoR.Text + " " + TxtCasadaR.Text + "," + TxtNombreR.Text + "','  ',' '),' ',' ',' ',' ',' ',' '," +
                                                "REPLACE(UPPER('" + TxtApellidoR.Text + "'),' ',''),REPLACE(UPPER('" + TxtNombreR.Text + "'),' ',''),'" + TxtApellidoR.Text + "','" + TxtNombreR.Text + "',' ','" + TxtCasadaR.Text + "',REPLACE(UPPER('" + TxtCasadaR.Text + "'),' ','')||' '," +
                                                "' ',' ',' ',' ','1','" + TxtNombreR.Text + " " + TxtApellidoR.Text + " " + TxtCasadaR.Text + "','" + TxtNombreR.Text + " " + TxtApellidoR.Text + " " + TxtCasadaR.Text + "',REPLACE(UPPER('" + TxtNombreR.Text + TxtApellidoR.Text + TxtCasadaR.Text + "'),' ',''),SYSDATE,'" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "INSERT INTO SYSADM.PS_PERS_NID (EMPLID, COUNTRY, NATIONAL_ID_TYPE, NATIONAL_ID, SSN_KEY_FRA, PRIMARY_NID, TAX_REF_ID_SGP, LASTUPDDTTM, LASTUPDOPRID) " +
                                                "VALUES ('" + UserEmplid.Text + "','GTM','NITREC','" + txtNit.Text + "',' ','N','N',SYSDATE,'" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES (EMPLID, ADDRESS_TYPE,COUNTY,CITY,NUM1, NUM2, HOUSE_TYPE, ADDR_FIELD1, ADDR_FIELD2, ADDR_FIELD3,POSTAL,GEO_CODE,IN_CITY_LIMIT," +
                                                "ADDRESS1_AC,ADDRESS2_AC,ADDRESS3_AC,CITY_AC,REG_REGION,EFFDT,EFF_STATUS,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,STATE,LASTUPDDTTM,LASTUPDOPRID) " +
                                               "VALUES('" + UserEmplid.Text + "', 'REC',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ', '" + DateTime.Now.ToString("dd/MM/yyyy")
                                               + "', 'A', '" + codPais + "', '" + TxtDiRe1.Text + "', '" + TxtDiRe2.Text + "', '" + TxtDiRe3.Text + "', ' ','" + StateNIT.Text + "', '" +
                                               DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "','" + TextUser.Text + "')";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES_SA (LOC_ADDR_DATA,LOC_ADDR, LOC_ADDR_LINE, LOC_ADDR_TYPE, EXT_ORG_ID, DESCR_EXT_ORG, DESCR_ORG_LOCATION, " +
                                                "CONTACT_NAME, DATA_SOURCE, EMPLID, ADDRESS_TYPE,EFFDT,ORG_LOCATION,MAINT_ADDR_MANUAL,MAINT_OTHER_MANUAL,ORG_CONTACT,SEASONAL_ADDR) " +
                                                "VALUES(' ',' ',' ',' ',' ',' ',' ',' ',' ','" + UserEmplid.Text + "', 'REC', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 0, 'N','N',0,'N')";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        llenadoPaisnit();
                                    }
                                }


                                if (!txtInsert.Text.IsNullOrWhiteSpace() && RegistroCarne == "0")
                                {
                                    cmd.CommandText = txtInsert.Text;
                                    cmd.ExecuteNonQuery();
                                }

                                mensaje = "Su información fue actualizada correctamente";
                                FileUpload2.Visible = false;
                                CargaDPI.Visible = false;
                                mostrarInformación();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception x)
                            {
                                transaction.Rollback();
                                mensaje = "-Ocurrió un problema al actualizar su información " + x;
                                fotoAlmacenada();
                            }
                        }
                    }
                }
                catch (Exception X)
                {
                    mensaje = "Ocurrió un problema al actualizar su información" + X;
                }
                AlmacenarFotografia();
                Thread.Sleep(1000);
                fotoAlmacenada();
            }
            else
            {
                lblActualizacion.Text = "Es necesario tomar una fotografía.";
                mensaje = "Es necesario tomar una fotografía.";
            }
            return mensaje;
        }

        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            string informacion = actualizarInformacion();

            if (informacion != "0")
            {

                if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(CmbPais.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                {
                    if (RadioButtonNombreNo.Checked)
                    {
                        if (!CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                        {
                            IngresoActualizacion(informacion);
                        }
                        else
                        {
                            AlmacenarFotografia();
                            fotoAlmacenada();
                            lblActualizacion.Text = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        IngresoActualizacion(informacion);
                    }
                }

            }
        }

        protected void IngresoActualizacion(string informacion)
        {
            if ((informacion != "No puede enviarse información vacía y es necesario seleccionar el estado civil, un país, un departamento y un muncipio" || informacion != "No puede enviarse información vacía y es necesario cargar una fotografía, seleccionar el estado civil, un país, un departamento y un muncipio") && txtNInicial.Text == txtNombre.Text && txtAInicial.Text == txtApellido.Text && txtCInicial.Text == txtCasada.Text)
            {
                informacion = informacion + Upload(Request.Form["urlPath"]);
            }
            else if (txtNInicial.Text != txtNombre.Text || txtAInicial.Text != txtApellido.Text || txtCInicial.Text != txtCasada.Text)
            {
                if (FileUpload2.HasFiles)
                {
                    informacion = informacion + Upload(Request.Form["urlPath"]);
                }
            }
            lblActualizacion.Text = informacion;
        }
        protected string Upload(string ImagenData)
        {
            string mensaje = "";
            try
            {
                string FechaHoraInicioEjecución = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                int ContadorArchivos = 0;
                int ContadorArchivosCorrectos = 0;
                int ContadorArchivosConError = 0;

                bool Error = false;

                //Ruta del archivo que guarda la bitácora
                string RutaBitacora = Request.PhysicalApplicationPath + "Logs\\";
                //Nombre del archiov que guarda la bitácora
                string ArchivoBitacora = RutaBitacora + FechaHoraInicioEjecución.Replace("/", "").Replace(":", "") + ".txt";


                //Se crea un nuevo archivo para guardar la bitacora de la ejecución
                CrearArchivoBitacora(ArchivoBitacora, FechaHoraInicioEjecución);

                //Guadar encabezado de la bitácora
                GuardarBitacora(ArchivoBitacora, "                              Informe de ejecución de importación de fotografías Campus Fecha: " + FechaHoraInicioEjecución + "              ");
                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "Nombre del archivo                    EMPLID                      Estado                 Descripción                                    ");
                GuardarBitacora(ArchivoBitacora, "------------------------------------  --------------------------  ---------------------  ------------------------------------------------------------");


                string constr = TxtURL.Text;
                string EmplidFoto = txtCarne.Text;
                string EmplidExisteFoto = "";
                string mensajeValidacion = "";
                //Nombre de la fotografía cargada (Sin extensión)
                string NombreFoto = "2990723550101";//Context.User.Identity.Name.Replace("@unis.edu.gt", ""); 
                                                    //string NombreFoto = Context.User.Identity.Name.Replace("@unis.edu.gt", "");

                //Busca si la persona ya tiene fotografía registrada para proceder a actualizar
                using (OracleConnection conEmplid = new OracleConnection(constr))
                {
                    try
                    {
                        OracleCommand cmdEmplid = new OracleCommand();
                        cmdEmplid.CommandText = "SELECT DISTINCT EMPLID FROM SYSADM.PS_EMPL_PHOTO WHERE EMPLID = '" + EmplidFoto + "'";
                        cmdEmplid.Connection = conEmplid;
                        conEmplid.Open();
                        OracleDataReader reader = cmdEmplid.ExecuteReader();

                        while (reader.Read())
                        {
                            EmplidExisteFoto = reader["EMPLID"].ToString();
                        }
                        conEmplid.Close();
                    }
                    catch (OracleException ex)
                    {
                        mensajeValidacion = "Error con la base de datos de Campus, no se registró la fotografía en Campus. " + ex.Message;
                        GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                        if (Error == false)
                        {
                            ContadorArchivosConError++;
                        }
                    }
                }

                if (Request.Form["urlPath"].Contains("data:image/jpeg;base64,"))
                {
                    int largo = 0;
                    largo = ImagenData.Length;
                    ImagenData = ImagenData.Substring(23, largo - 23).ToString();
                }
                byte[] bytes = Convert.FromBase64String(ImagenData);

                using (OracleConnection con = new OracleConnection(constr))
                {
                    string query = "";

                    using (OracleCommand cmd = new OracleCommand(query))
                    {

                        if (EmplidExisteFoto != "") //Se actualiza la fotografía
                        {
                            cmd.CommandText = "UPDATE SYSADM.PS_EMPL_PHOTO SET PSIMAGEVER=(TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), EMPLOYEE_PHOTO=:Fotografia WHERE EMPLID = '" + EmplidFoto + "'";
                            mensajeValidacion = "La fotografía se actualizó correctamente en Campus.";
                            mensaje = " y la fotografía fue almacenada correctamente.";
                        }
                        else //Se registra la nueva fotografía
                        {
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EMPL_PHOTO VALUES ('" + EmplidFoto + "', (TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), :Fotografia)";
                            mensajeValidacion = "La fotografía se registró correctamente en Campus.";
                            mensaje = " y la fotografía fue almacenada correctamente.";
                        }

                        cmd.Connection = con;
                        cmd.Parameters.Add(new OracleParameter("Fotografia", bytes));
                        try
                        {
                            con.Open();

                            int FilasAfectadas = cmd.ExecuteNonQuery();
                            con.Close();
                            if (FilasAfectadas == 0)
                            {
                                mensajeValidacion = "Error con la base de datos de Campus, no se registró la fotografía en Campus";
                                GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                if (Error == false)
                                {
                                    ContadorArchivosConError++;
                                    Error = true;
                                }
                            }
                            else
                            {
                                GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "  " + EmplidFoto.PadRight(26) + "  Correcto               " + mensajeValidacion.PadRight(60));
                                ContadorArchivosCorrectos++;
                            }
                        }
                        catch (OracleException ex)
                        {
                            mensajeValidacion = "Error con la base de datos de Campus, no se registró la fotografía en Campus. " + ex.Message;
                            GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                            if (Error == false)
                            {
                                ContadorArchivosConError++;
                            }
                        }
                    }
                }

                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "-----------------------------------------------------------------------------------------------");
                GuardarBitacora(ArchivoBitacora, "Total de archivos: " + ContadorArchivos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos cargados correctamente: " + ContadorArchivosCorrectos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos con error: " + ContadorArchivosConError.ToString());

            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                mensaje = ". Ocurrió un error al cargar la imagen";
            }
            return mensaje;
        }

        //Función para guardar bitacora en el archivo .txt
        public void GuardarBitacora(string ArchivoBitacora, string DescripcionBitacora)
        {
            //Guarda nueva línea para el registro de bitácora en el serividor
            File.AppendAllText(ArchivoBitacora, DescripcionBitacora + Environment.NewLine);
        }

        //Crea un archivo .txt para guardar bitácora
        public void CrearArchivoBitacora(string archivoBitacora, string FechaHoraEjecución)
        {
            using (StreamWriter sw = File.CreateText(archivoBitacora)) ;
        }

        protected void CmbPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoDepartamento();
            llenadoMunicipio();
            llenadoState();
            fotoAlmacenada();
        }
        protected void CmbPaisNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmacenarFotografia();
            Thread.Sleep(1000);
            llenadoDepartamentoNit();
            llenadoMunicipioNIT();
            llenadoStateNIT();
            fotoAlmacenada();
        }
        public string divisionApellidos(string apellido)
        {
            WebClient _clientW = new WebClient();
            _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            _clientW.Headers.Add("apellido", apellido);
            string json = _clientW.DownloadString(txtApex.Text + "unis_interfaces/Centralizador/ExcepcionesApellidos");
            dynamic respuesta = JsonConvert.DeserializeObject(json).ToString();

            return respuesta;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public string SaveCanvasImage(string imageData, string folderPath, string fileName)
        {
            if (!String.IsNullOrEmpty(imageData))
            {
                int largo = 0;
                largo = imageData.Length;
                imageData = imageData.Substring(23, largo - 23);
                try
                {
                    // Nombre del archivo de imagen
                    //string NombreFoto = "3682754340101";//Context.User.Identity.Name.Replace("@unis.edu.gt", ""); 
                    //string fileName = Context.User.Identity.Name.Replace("@unis.edu.gt", "") + ".jpg";

                    // Ruta de la carpeta donde se almacenará la imagen
                    //string fileName = Context.User.Identity.Name.Replace("@unis.edu.gt", "") + ".jpg";

                    // Ruta completa del archivo
                    string filePath = Path.Combine(folderPath, fileName);

                    // Guardar la imagen en el servidor
                    byte[] imageBytes = Convert.FromBase64String(Convert.ToString(imageData));
                    File.WriteAllBytes(filePath, imageBytes);

                    return "Imagen guardada correctamente.";
                }
                catch (Exception ex)
                {
                    return "Error al guardar la imagen: " + ex.Message;
                }
            }
            return "";
        }

        private string consultaNit(string nit)
        {

            var credenciales = "d39c940b-c65f-4502-8a94-f1d9109076ab";
            var body = "{\"Credenciales\" : \"" + credenciales + "\",\"NIT\":\"" + nit + "\"}";
            string respuesta = api.PostNit("https://felaupar.azurewebsites.net/api/NIT?code=Cw1Olqw4rH0EyEgZdqLD9gQqy62AF5FpAfuBL5spyZoajzNGjhme4A==&Credenciales=d39c940b-c65f-4502-8a94-f1d9109076ab", body);
            return respuesta;

        }

        private void AlmacenarFotografia()
        {
            //lblActualizacion.Text = "";
            if (!Request.Form["urlPath"].IsNullOrWhiteSpace())
            {
                //SaveCanvasImage(Request.Form["urlPath"], CurrentDirectory + "/Usuarios/UltimasCargas/", txtCarne.Text + ".jpg");
                int ExisteFoto;
                string constr = TxtURL.Text;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        //Obtener fotografia
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET = '" + txtCarne.Text + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ExisteFoto = Convert.ToInt16(reader["CONTADOR"]);

                            try
                            {
                                cmd.Connection = con;
                                //Numero de Telefono
                                if (ExisteFoto > 0)
                                {
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE SET FOTOGRAFIA = 'Existe'" +
                                                        "WHERE CARNET = '" + txtCarne.Text + "'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE (FOTOGRAFIA, CARNET) VALUES ('Existe', '" + txtCarne.Text + "')";
                                    cmd.ExecuteNonQuery();
                                }

                                SaveCanvasImage(Request.Form["urlPath"], CurrentDirectory + "/Usuarios/UltimasCargas/", txtCarne.Text + ".jpg");
                                transaction.Commit();
                            }
                            catch (Exception x)
                            {
                                transaction.Rollback();
                                fotoAlmacenada();
                            }
                        }
                    }
                }
            }
            //lblActualizacion.Text = "Debe de tomar una fotografía.";
        }

        [WebMethod]
        public static object GetChildDropDownData(string CmbPais)
        {

            string rutaCompleta = AppDomain.CurrentDomain.BaseDirectory + "conexion.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                file.Close();
            }
            using (OracleConnection connection = new OracleConnection(line))
            {
                connection.Open();
                OracleCommand command = new OracleCommand("SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPais + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO", connection);
                command.Parameters.Add(new OracleParameter("selectedValue", CmbPais));
                OracleDataReader reader = command.ExecuteReader();

                // Construir una lista de objetos con los datos para el segundo DropDownList
                var dataList = new System.Collections.Generic.List<object>();
                while (reader.Read())
                {
                    var dataItem = new
                    {
                        Value = reader["Value"].ToString(),
                        Text = reader["Text"].ToString()
                    };
                    dataList.Add(dataItem);
                }

                return new { d = dataList };
            }
        }

        protected void txtNit_TextChanged(object sender, EventArgs e)
        {
            string respuesta;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            respuesta = consultaNit(txtNit.Text);
            string constr = TxtURL.Text;

            if (respuesta == "BadRequest" || respuesta == "1")
            {
                TxtNombreR.Text = null;
                TxtApellidoR.Text = null;
                TxtCasadaR.Text = null;
                llenadoPaisnit();
                CmbPaisNIT.SelectedValue = " ";
                llenadoDepartamentoNit();
                CmbDepartamentoNIT.SelectedValue = " ";
                llenadoMunicipioNIT();
                CmbMunicipioNIT.Text = "-";
                CmbMunicipioNIT.Enabled = false;
                CmbDepartamentoNIT.Enabled = false;
                CmbPaisNIT.Enabled = false;
                txtNit.Enabled = true;
                ValidarNIT.Enabled = true;
                int ExisteNitValidacion = 0;

                //ALMACENAMIENTO DE INFORMACIÓN DE NIT PARA VALIDACION POSTERIOR
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        //Obtener fotografia
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST WHERE EMPLID = '" + UserEmplid.Text + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ExisteNitValidacion = Convert.ToInt16(reader["CONTADOR"]);
                        }
                        cmd.Transaction = transaction;
                        try
                        {
                            if (ExisteNitValidacion == 0)
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST (NIT, EMPLID) VALUES ('" + txtNit.Text + "', '" + UserEmplid.Text + "')";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            else
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST SET NIT = '" + txtNit.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                        }
                        catch (Exception x)
                        {
                            transaction.Rollback();
                        }

                    }
                }

                lblActualizacion.Text = "El NIT sera validado más adelante";
            }
            else if (respuesta != "1" && respuesta != "BadRequest")
            {
                string NIT = getBetween(respuesta, "\"NIT\": \"", "\",");
                string Direccion = getBetween(respuesta, "\"Direccion\": \"", "\",");
                string apellido1;
                string apellido2;
                string apellidoCasada;
                string nombre1;
                string nombre2;
                int largo;
                if (NIT != "CF")
                {
                    string nombreRespuesta = getBetween(respuesta, "\"NOMBRE\": \"", "\",") + ",";
                    string cadena = nombreRespuesta;
                    TxtDiRe1.Text = Direccion;
                    int contadorComas = cadena.Count(c => c == ',');
                    if (contadorComas >= 5)
                    {
                        apellido1 = getBetween(nombreRespuesta, "", ",");
                        apellido2 = getBetween(nombreRespuesta, apellido1 + ",", ",");
                        apellidoCasada = getBetween(nombreRespuesta, apellido2 + ",", ",");
                        nombre1 = getBetween(nombreRespuesta, apellido1 + "," + apellido2 + "," + apellidoCasada + ",", ",");
                        nombre2 = getBetween(nombreRespuesta, nombre1 + ",", ",");
                        TxtNombreR.Text = textInfo.ToTitleCase(nombre1 + " " + nombre2);
                        TxtApellidoR.Text = apellido1 + " " + apellido2;
                        TxtCasadaR.Text = apellidoCasada;
                    }
                    else
                    {
                        nombreRespuesta = nombreRespuesta.TrimEnd(',');
                        largo = nombreRespuesta.Length;

                        if (largo < 31)
                        {
                            TxtNombreR.Text = nombreRespuesta;
                        }
                        else if (largo > 30 && largo < 61)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, largo - 30);
                        }
                        else if (largo > 30 && largo < 91)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, 30);
                            TxtCasadaR.Text = nombreRespuesta.Substring(60, largo - 60);
                        }
                        else if (largo > 90)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, 30);
                            TxtCasadaR.Text = nombreRespuesta.Substring(60, 30);
                        }
                    }

                    lblActualizacion.Text = "";
                    AlmacenarFotografia();
                    fotoAlmacenada();
                    CargaDPI.Visible = true;

                    //UpdatePanel1.Update();
                }
                else
                {
                    string nit = txtNit.Text;
                    txtNit.Text = nit;
                    lblActualizacion.Text = "El NIT no existe";
                }
            }

        }


        protected void RadioButtonNombreSi_CheckedChanged(object sender, EventArgs e)
        {
            TxtNombreR.Text = txtNombre.Text;
            TxtApellidoR.Text = txtApellido.Text;
            TxtCasadaR.Text = txtCasada.Text;
            TxtDiRe1.Text = txtDireccion.Text;
            TxtDiRe2.Text = txtDireccion2.Text;
            TxtDiRe3.Text = txtDireccion3.Text;
            txtNit.Text = "CF";
            txtNit.Enabled = false;
            ValidarNIT.Enabled = false;
            TxtDiRe1.Enabled = false;
            TxtDiRe2.Enabled = false;
            TxtDiRe3.Enabled = false;
            CargaDPI.Visible = true;
        }

        protected void RadioButtonNombreNo_CheckedChanged(object sender, EventArgs e)
        {
            /*llenadoPaisnit();
            llenadoDepartamentoNit();
            llenadoMunicipioNIT();*/
        }

        protected void BtnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/ActualizacionEstudiantes.aspx");
        }

        protected void BtnDownload_Click(object sender, EventArgs e)
        {        
            // Descargar el archivo
            string archivoDescarga = CurrentDirectory + "/Manuales/Manual borrador.docx";
            string nombreArchivo = "Manual borrador.docx";
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nombreArchivo + "\"");
            Response.WriteFile(archivoDescarga);
            Response.End();
        }
    }
}