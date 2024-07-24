using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Web.Services;
using ReportesUnis.API;
using System.Globalization;
using System.Xml;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Text;
using NPOI.Util;
using System.Web.UI.WebControls.WebParts;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.Word;
using System.Web.UI.WebControls;
using System.Security.Principal;
namespace ReportesUnis
{
    public partial class ActualizacionGeneralEstudiantes : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        ConsumoAPI api = new ConsumoAPI();
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        int auxConsulta = 0;
        int contadorUP = 0;
        int contadorUD = 0;
        int respuestaPatch = 0;
        string respuestaMensajePatch = "";
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigCampus.dat");

        protected void Page_Load(object sender, EventArgs e)
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            ControlCamposVisibles(true);
            LeerInfoTxt();
            LeerPathApex();
            LeerCredencialesNIT();
            LeerVersionesSOAPCampus();

            TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");

            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_VistaAdmisiones") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LeerInfoTxtSQL();
                LlenadoPais();
                LlenadoDepartamento();
                LlenadoState();
                LlenadoPaisNacimiento();
                txtControlBit.Text = "0";
                txtControlNR.Text = "0";
                txtControlAR.Text = "0";
                LlenarHospital();
                LoadDataDocumentos();
                LoadDataContactos();
                LlenarAlergias();
                LlenarAntecedentes();
            }
        }

        //FUNCIONES
        void LeerInfoTxt()
        {
            string rutaCompleta = CurrentDirectory + "conexion.txt";
            //string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string line = file.ReadToEnd();
                TxtURL.Text = line;
                file.Close();
            }
        }
        void LeerCredencialesNIT()
        {
            string rutaCompleta = CurrentDirectory + "CredencialesNIT.txt";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                CREDENCIALES_NIT.Value = linea1;
                URL_NIT.Value = linea2;
                file.Close();
            }
        }
        void LeerVersionesSOAPCampus()
        {
            string rutaCompleta = CurrentDirectory + "VersionesCampus.txt";

            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                string linea4 = file.ReadLine();
                VersionUP.Value = linea4;
                VersionUD.Value = linea2;
                file.Close();
            }
        }
        void LeerPathApex()
        {
            string rutaCompleta = CurrentDirectory + "urlApex.txt";
            //string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string line = file.ReadToEnd();
                txtApex.Text = line;
                file.Close();
            }
        }
        void LeerInfoTxtSQL()
        {
            string rutaCompleta = CurrentDirectory + "conexionSQL.txt";
            //string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string line = file.ReadToEnd();
                TxtURLSql.Text = line;
                file.Close();
            }
        }
        public void ControlCamposVisibles(bool Condicion)
        {
            Informacion.Visible = Condicion;
            tabla.Visible = Condicion;
            tbactualizar.Visible = Condicion;
            InfePersonal.Visible = Condicion;
        }
        private string MostrarInformación(string emplid)
        {
            string constr = TxtURL.Text;
            var apellidoEx = "0";
            int posicion = 0;
            int posicion2 = 0;
            int largoApellido = 0;
            int excepcionApellido = 0;
            string DPI = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT NATIONAL_ID FROM SYSADM.PS_PERS_NID PN " +
                    "WHERE EMPLID ='" + emplid + "' ";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DPI = reader["NATIONAL_ID"].ToString();
                    }

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, \r\n" +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, MUNICIPIO_NIT, DEPARTAMENTO_NIT, STATE_NIT, PAIS_NIT, STATE, EMAILUNIS,EMAILPERSONAL, BIRTHCOUNTRY, MUNICIPIO_NAC, DEPARTAMENTO_NAC, BIRTHPLACE, BIRTHSTATE FROM ( \r\n" +
                                        "SELECT PD.EMPLID, PN.NATIONAL_ID CARNE,  PD.FIRST_NAME, \r\n" +
                                        "PD.LAST_NAME, PD.SECOND_LAST_NAME, PN.NATIONAL_ID DPI, PN.NATIONAL_ID_TYPE, \r\n " +
                                        "COALESCE(" +
                                        "        (SELECT PP.PHONE FROM SYSADM.PS_PERSONAL_PHONE PP WHERE PP.EMPLID = PD.EMPLID AND PP.PHONE_TYPE = 'HOME' FETCH FIRST 1 ROWS ONLY), \r\n " +
                                        "        (SELECT PP.PHONE FROM SYSADM.PS_PERSONAL_PHONE PP WHERE PP.EMPLID = PD.EMPLID AND PP.PHONE_TYPE = 'CEL1' FETCH FIRST 1 ROWS ONLY) \r\n" +
                                        "   ) AS PHONE, \r\n" +
                                        "TO_CHAR(PD.BIRTHDATE,'YYYY-MM-DD') BIRTHDATE, PD.BIRTHPLACE, PD.BIRTHSTATE, \r\n" +
                                        "(SELECT BIRTHCOUNTRY FROM SYSADM.PS_PERS_DATA_SA_VW WHERE EMPLID ='" + emplid + "') BIRTHCOUNTRY, \r\n" +
                                        "APD.DESCR CARRERA, AGT.DESCR FACULTAD, \r\n" +
                                        "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'No Consta' END STATUS, \r\n" +
                                        "(SELECT EXTERNAL_SYSTEM_ID FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NIT, \r\n" +
                                        "(SELECT PNA.FIRST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NOMBRE_NIT, \r\n" +
                                        "(SELECT PNA.LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) APELLIDO_NIT, \r\n" +
                                        "(SELECT SECOND_LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY PNA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) CASADA_NIT, \r\n" +
                                        "(SELECT ADDRESS1 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION1_NIT, \r\n" +
                                        "(SELECT ADDRESS2 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION2_NIT, \r\n" +
                                        "(SELECT ADDRESS3 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION3_NIT, \r\n" +
                                        "(SELECT C.DESCR FROM SYSADM.PS_ADDRESSES PA JOIN SYSADM.PS_COUNTRY_TBL C ON PA.COUNTRY = C.COUNTRY AND PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) PAIS_NIT, \r\n" +
                                        "(SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) MUNICIPIO_NIT, \r\n" +
                                        "(SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_PERS_DATA_SA_VW PD ON ST.STATE = PD.BIRTHSTATE AND ST.COUNTRY = PD.BIRTHCOUNTRY WHERE PD.EMPLID='" + emplid + "' ) MUNICIPIO_NAC, \r\n" +
                                        "(SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DEPARTAMENTO_NIT, \r\n" +
                                        "COALESCE((SELECT SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_PERS_DATA_SA_VW PD ON ST.STATE = PD.BIRTHSTATE AND ST.COUNTRY = PD.BIRTHCOUNTRY WHERE PD.EMPLID='" + emplid + "' ),' ') DEPARTAMENTO_NAC, \r\n" +
                                        "(SELECT ST.STATE FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) STATE_NIT, \r\n" +
                                        "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) EMAILUNIS , \r\n" +
                                        "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND EMAIL.E_ADDR_TYPE IN ('HOM1') FETCH FIRST 1 ROWS ONLY) EMAILPERSONAL , \r\n" +
                                        "A.ADDRESS1 DIRECCION, A.ADDRESS2 DIRECCION2, A.ADDRESS3 DIRECCION3, \r\n" +
                                        "NVL(REGEXP_SUBSTR(ST.DESCR, '[^-]+'), ' ') AS MUNICIPIO , NVL(SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)), ' ') DEPARTAMENTO, ST.STATE,  \r\n" +
                                        "TT.TERM_BEGIN_DT, C.DESCR PAIS \r\n" +
                                        "FROM SYSADM.PS_PERS_DATA_SA_VW PD \r\n" +
                                        "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID \r\n" +
                                        "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID AND ADDRESS_TYPE= 'HOME' \r\n" +
                                        "AND A.EFFDT =( \r\n" +
                                        "    SELECT \r\n" +
                                        "        MAX(EFFDT) \r\n" +
                                        "    FROM \r\n" +
                                        "        SYSADM.PS_ADDRESSES A2 \r\n" +
                                        "    WHERE \r\n" +
                                        "        A.EMPLID = A2.EMPLID \r\n" +
                                        "        AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE \r\n" +
                                        ") \r\n" +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID \r\n" +
                                        "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE \r\n" +
                                        "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID \r\n" +
                                        "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG \r\n" +
                                        "AND CT.ACAD_CAREER = APD.ACAD_CAREER \r\n" +
                                        "AND CT.INSTITUTION = APD.INSTITUTION \r\n" +
                                        "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP \r\n" +
                                        "AND APD.INSTITUTION = AGT.INSTITUTION \r\n" +
                                        "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM \r\n" +
                                        "AND CT.INSTITUTION = TT.INSTITUTION \r\n" +
                                        "AND (SYSDATE BETWEEN TT.TERM_BEGIN_DT AND TT.TERM_END_DT) \r\n" +
                                        //"LEFT JOIN SYSADM.PS_PERSONAL_PHONE PP ON PD.EMPLID = PP.EMPLID \r\n" +
                                        //"AND PP.PHONE_TYPE = 'HOME' \r\n" +
                                        "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON A.COUNTRY = C.COUNTRY \r\n" +
                                        "WHERE PN.NATIONAL_ID ='" + DPI + "' \r\n" +
                                        "ORDER BY CT.FULLY_ENRL_DT DESC \r\n" +
                                        "FETCH FIRST 1 ROWS ONLY \r\n" +
                                       ") ";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtCarne.Text = reader["EMPLID"].ToString();
                        txtNombre.Text = reader["FIRST_NAME"].ToString().TrimEnd();
                        txtNInicial.Value = reader["FIRST_NAME"].ToString().Trim();
                        txtApellido.Text = reader["LAST_NAME"].ToString().TrimEnd();
                        txtCasada.Text = reader["SECOND_LAST_NAME"].ToString().TrimEnd();
                        txtCInicial.Value = reader["SECOND_LAST_NAME"].ToString();
                        txtAInicial.Value = reader["LAST_NAME"].ToString().TrimEnd();
                        TxtApellidoR.Text = reader["APELLIDO_NIT"].ToString();
                        InicialNR2.Value = reader["APELLIDO_NIT"].ToString();
                        TxtNombreR.Text = reader["NOMBRE_NIT"].ToString();
                        InicialNR1.Value = reader["NOMBRE_NIT"].ToString();
                        TxtCasadaR.Text = reader["CASADA_NIT"].ToString();
                        InicialNR3.Value = reader["CASADA_NIT"].ToString();
                        txtNit.Text = reader["NIT"].ToString();
                        TrueNit.Value = reader["NIT"].ToString();
                        ControlCF.Value = reader["NIT"].ToString();
                        State.Text = reader["STATE"].ToString();
                        StateNIT.Text = reader["STATE_NIT"].ToString();
                        largoApellido = txtAInicial.Value.Length;
                        EmailUnis.Text = reader["EMAILUNIS"].ToString();
                        TxtCorreoPersonal.Text = reader["EMAILPERSONAL"].ToString();
                        TrueEmail.Text = reader["EMAILPERSONAL"].ToString();
                        TxtLugarNac.Text = reader["BIRTHPLACE"].ToString();

                        if (txtApellido.Text.Length > 4)
                        {
                            if (txtApellido.Text.Length > 6)
                            {
                                if ((txtApellido.Text.Substring(0, 6)).ToUpper().Equals("DE LA "))
                                {
                                    posicion = txtApellido.Text.Substring(6, largoApellido - 6).IndexOf(" ");
                                    txtContaador.Text = txtAInicial.Value.Length.ToString() + " " + posicion.ToString();
                                    txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 6);
                                }
                            }
                            else if (txtApellido.Text.Length > 7)
                            {
                                if ((txtApellido.Text.Substring(0, 7)).ToUpper().Equals("DE LAS "))
                                {
                                    posicion = txtApellido.Text.Substring(7, largoApellido - 7).IndexOf(" ");
                                    txtContaador.Text = txtAInicial.Value.Length.ToString() + " " + posicion.ToString();
                                    txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 7);
                                }

                            }

                        }
                        else
                        {
                            posicion = reader["LAST_NAME"].ToString().IndexOf(" ");
                            if (posicion > 0)
                            {
                                apellidoEx = DivisionApellidos(reader["LAST_NAME"].ToString().Substring(0, posicion));
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
                                txtPrimerApellido.Text = GetBetween(txtApellido.Text, "", " ");
                            }
                        }

                        txtDPI.Text = reader["DPI"].ToString();
                        CmbEstado.SelectedValue = reader["STATUS"].ToString();

                        if (CmbEstado.SelectedValue.Substring(0, 1).ToString().Equals("S"))
                        {
                            TrueEstadoCivil.Value = "S";
                        }
                        else if (CmbEstado.SelectedValue.Substring(0, 1).ToString().Equals("C"))
                        {
                            TrueEstadoCivil.Value = "M";
                        }
                        else
                        {
                            TrueEstadoCivil.Value = "U";
                        }

                        txtCumple.Text = reader["BIRTHDATE"].ToString();
                        txtDireccion.Text = reader["DIRECCION"].ToString().Length > 54 ? reader["DIRECCION"].ToString().Substring(0, 54) : reader["DIRECCION"].ToString();
                        TrueDir.Text = reader["DIRECCION"].ToString().Length > 54 ? reader["DIRECCION"].ToString().Substring(0, 54) : reader["DIRECCION"].ToString();
                        txtDireccion2.Text = reader["DIRECCION2"].ToString();
                        string direccion3 = reader["DIRECCION3"].ToString();
                        if (direccion3.Contains("Zona"))
                        {
                            direccion3 = direccion3.Replace("Zona", "");
                        }
                        txtDireccion3.Text = direccion3;
                        TxtDiRe1.Text = reader["DIRECCION1_NIT"].ToString().Length > 54 ? reader["DIRECCION1_NIT"].ToString().Substring(0, 54) : reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2.Text = reader["DIRECCION2_NIT"].ToString().Length > 54 ? reader["DIRECCION2_NIT"].ToString().Substring(0, 54) : reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3.Text = reader["DIRECCION3_NIT"].ToString().Length > 54 ? reader["DIRECCION3_NIT"].ToString().Substring(0, 54) : reader["DIRECCION3_NIT"].ToString();
                        if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                        {
                            CmbPais.SelectedValue = reader["PAIS"].ToString();
                            LlenadoDepartamento();
                            CmbDepartamento.SelectedValue = reader["DEPARTAMENTO"].ToString();
                            LlenadoMunicipio();
                            CmbMunicipio.SelectedValue = reader["MUNICIPIO"].ToString();
                        }
                        else
                        {
                            CmbPais.SelectedValue = " ";
                        }
                        PaisNacimiento.Value = reader["BIRTHCOUNTRY"].ToString();
                        LugarNacimiento.Value = reader["BIRTHPLACE"].ToString();
                        StateNacimiento.Value = reader["BIRTHSTATE"].ToString().Trim();

                        if (!String.IsNullOrEmpty(reader["BIRTHCOUNTRY"].ToString()))
                        {
                            CmbPaisNacimiento.SelectedValue = reader["BIRTHCOUNTRY"].ToString();
                            LlenadoDepartamentoNac();
                            CmbDeptoNacimiento.SelectedValue = reader["DEPARTAMENTO_NAC"].ToString();
                            LlenadoMunicipioNacimiento();
                            if (!String.IsNullOrEmpty(reader["MUNICIPIO_NAC"].ToString()))
                                CmbMuncNacimiento.SelectedValue = reader["MUNICIPIO_NAC"].ToString();
                        }
                        else
                        {
                            CmbPaisNacimiento.SelectedValue = " ";
                        }

                        if (!String.IsNullOrEmpty(reader["PAIS_NIT"].ToString()))
                        {
                            LlenadoPaisnit();
                            CmbPaisNIT.SelectedValue = reader["PAIS_NIT"].ToString();
                            PaisNit.Text = reader["PAIS_NIT"].ToString();
                            LlenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO_NIT"].ToString();
                            DepartamentoNit.Text = reader["DEPARTAMENTO_NIT"].ToString();
                            LlenadoMunicipioNIT();
                            MunicipioNit.Text = reader["MUNICIPIO_NIT"].ToString();
                        }
                        else if (RadioButtonNombreSi.Checked)
                        {
                            LlenadoPaisnit();
                            if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                                CmbPaisNIT.SelectedValue = reader["PAIS"].ToString();
                            else
                                CmbPaisNIT.SelectedValue = "";
                            LlenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO"].ToString();
                            LlenadoMunicipioNIT();
                            CmbMunicipioNIT.SelectedValue = reader["MUNICIPIO"].ToString();
                        }
                        else
                        {
                            LlenadoPaisnit();
                            LlenadoDepartamentoNit();
                            LlenadoMunicipioNIT();
                        }
                        txtTelefono.Text = reader["PHONE"].ToString();
                        TruePhone.Text = reader["PHONE"].ToString();
                        txtCarrera.Text = reader["CARRERA"].ToString();
                        txtFacultad.Text = reader["FACULTAD"].ToString();
                        UserEmplid.Text = reader["EMPLID"].ToString();

                        if (TxtNombreR.Text == "\r\n" || TxtNombreR.Text == "\n")
                        {
                            TxtNombreR.Text = null;
                        }
                        if (TxtApellidoR.Text == "\r\n" || TxtApellidoR.Text == "\n")
                        {
                            TxtApellidoR.Text = null;
                        }
                        if (TxtCasadaR.Text == "\r\n" || TxtCasadaR.Text == "\n")
                        {
                            TxtCasadaR.Text = null;
                        }
                        if (InicialNR1.Value == "\r\n" || InicialNR1.Value == "\n")
                        {
                            InicialNR1.Value = null;
                        }
                        if (InicialNR2.Value == "\r\n" || InicialNR2.Value == "\n")
                        {
                            InicialNR2.Value = null;
                        }
                        if (InicialNR3.Value == "\r\n" || InicialNR3.Value == "\n")
                        {
                            InicialNR3.Value = null;
                        }
                    }

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT NOMBRE_COMPLETO FROM UNIS_INTERFACES.TBL_FACULTADES WHERE NOMBRE_CAMPUS ='" + txtFacultad.Text.TrimEnd().TrimStart() + "'";
                    OracleDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        txtFacultad.Text = reader2["NOMBRE_COMPLETO"].ToString();
                    }

                    cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();
                        if (!String.IsNullOrEmpty(EFFDT_A_NIT.Value))
                        {
                            if (EFFDT_A_NIT.Value.Length == 9)
                            {
                                EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }
                    }

                    cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EFFDT_A.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();
                        if (!String.IsNullOrEmpty(EFFDT_A.Value))
                        {
                            if (EFFDT_A.Value.Length == 9)
                            {
                                EFFDT_A.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }
                    }

                    cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_PERS_DATA_EFFDT WHERE EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();
                        if (!String.IsNullOrEmpty(EFFDT_EC.Value))
                        {
                            if (EFFDT_EC.Value.Length == 9)
                            {
                                EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }
                    }

                    cmd.CommandText = "SELECT SEX, HIGHEST_EDUC_LVL, FT_STUDENT FROM SYSADM.PS_PERS_DATA_EFFDT WHERE EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SEX_EC.Value = reader["SEX"].ToString();
                        HIGH_LVL.Value = reader["HIGHEST_EDUC_LVL"].ToString();
                        FT_STUDENT.Value = reader["FT_STUDENT"].ToString();
                    }

                    cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();
                        if (!String.IsNullOrEmpty(EFFDT_NameR.Value))
                        {
                            if (EFFDT_NameR.Value.Length == 9)
                            {
                                EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }
                    }
                    con.Close();
                }
            }
            return emplid;
        }
        protected void LlenadoDepartamento()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            CmbDepartamento.Items.Clear();
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    if (CmbPais.Text != "Guatemala")
                    {
                        cmd.CommandText = "SELECT ' ' AS DEPARTAMENTO FROM DUAL UNION SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                        "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                        "WHERE CT.DESCR ='" + CmbPais.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                        "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                        "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                        "WHERE CT.DESCR ='" + CmbPais.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                        "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }

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
            ISESSION.Value = "0";
            banderaSESSION.Value = "1";
        }
        public void LlenadoDepartamentoNit()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            CmbDepartamentoNIT.Items.Clear();
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    if (CmbPaisNIT.Text != "Guatemala")
                    {
                        cmd.CommandText = "SELECT ' ' AS DEPARTAMENTO FROM DUAL UNION SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                        "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                        "WHERE CT.DESCR ='" + CmbPaisNIT.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                        "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                        "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                        "WHERE CT.DESCR ='" + CmbPaisNIT.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                        "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }

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
                        CmbDepartamentoNIT.DataTextField = " ";
                        CmbDepartamentoNIT.DataValueField = " ";
                    }
                }
            }
            ISESSION.Value = "0";
            banderaSESSION.Value = "1";
        }
        public void LlenadoDepartamentoNac()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            CmbDeptoNacimiento.Items.Clear();
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    if (CmbPaisNacimiento.Text != "Guatemala")
                    {
                        cmd.CommandText = "SELECT ' ' AS DEPARTAMENTO FROM DUAL UNION " +
                            "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                            "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                            "WHERE CT.COUNTRY ='" + CmbPaisNacimiento.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                            "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                            "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                            "WHERE CT.COUNTRY ='" + CmbPaisNacimiento.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL " +
                            "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";
                    }

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDeptoNacimiento.DataSource = ds;
                        CmbDeptoNacimiento.DataTextField = "DEPARTAMENTO";
                        CmbDeptoNacimiento.DataValueField = "DEPARTAMENTO";
                        CmbDeptoNacimiento.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamentoNIT.DataTextField = " ";
                        CmbDepartamentoNIT.DataValueField = " ";
                    }
                }
            }
            ISESSION.Value = "0";
            banderaSESSION.Value = "1";
        }
        public void LlenadoMunicipioNIT()
        {
            string constr = TxtURL.Text;
            CmbMunicipioNIT.Items.Clear();
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
                            if (CmbPaisNIT.Text != "Guatemala")
                            {
                                cmd.CommandText = "SELECT ' ' AS MUNICIPIO , ' ' AS STATE  FROM DUAL UNION SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                                "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamentoNIT.SelectedValue + "') " +
                                "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                                "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamentoNIT.SelectedValue + "') " +
                                "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            }
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
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT ' ' MUNICIPIO FROM DUAL";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                    }
                    catch (Exception)
                    {
                        CmbMunicipioNIT.DataSource = " ";
                        CmbMunicipioNIT.DataTextField = " ";
                        CmbMunicipioNIT.DataValueField = " ";
                    }
                }
            }
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
        }
        protected void LlenadoMunicipioNacimiento()
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            CmbMuncNacimiento.Items.Clear();
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(CmbDeptoNacimiento.SelectedValue) && CmbDeptoNacimiento.SelectedValue != " ")
                        {
                            cmd.Connection = con;
                            if (CmbPaisNacimiento.Text != "GTM")
                            {
                                cmd.CommandText = "SELECT ' ' AS MUNICIPIO , ' ' AS STATE  FROM DUAL UNION SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                                "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDeptoNacimiento.SelectedValue + "') " +
                                "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                                "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDeptoNacimiento.SelectedValue + "') " +
                                "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            }
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMuncNacimiento.DataSource = ds;
                            CmbMuncNacimiento.DataTextField = "MUNICIPIO";
                            CmbMuncNacimiento.DataValueField = "MUNICIPIO";
                            CmbMuncNacimiento.DataBind();
                            con.Close();
                        }
                        else
                        {
                            CmbMuncNacimiento.DataSource = " ";
                            CmbMuncNacimiento.DataTextField = " ";
                            CmbMuncNacimiento.DataValueField = " ";
                        }
                    }
                    catch (Exception)
                    {
                        CmbMuncNacimiento.DataSource = "-";
                        CmbMuncNacimiento.DataTextField = "-";
                        CmbMuncNacimiento.DataValueField = "-";
                    }
                }
            }
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
        }
        protected void LlenadoMunicipio()
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            CmbMunicipio.Items.Clear();
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        if (CmbPais.Text != "Guatemala")
                        {
                            cmd.CommandText = "SELECT ' ' AS MUNICIPIO, ' ' AS STATE FROM DUAL UNION SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                            "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamento.SelectedValue + "') " +
                            "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                        }
                        else
                        {
                            cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                            "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamento.SelectedValue + "') " +
                            "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                        }
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
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
        }
        protected void LlenadoPais()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
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
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
        }
        public void LlenadoPaisnit()
        {
            banderaSESSION.Value = "1";
            string where = "";
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
            banderaSESSION.Value = "1";
        }
        protected void LlenadoPaisNacimiento()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string where = "";
            if (!String.IsNullOrEmpty(CmbPaisNacimiento.Text))
                where = "WHERE COUNTRY='" + CmbPaisNacimiento.Text + "'";
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
                    CmbPaisNacimiento.DataSource = ds;
                    CmbPaisNacimiento.DataTextField = "PAIS";
                    CmbPaisNacimiento.DataValueField = "COUNTRY";
                    CmbPaisNacimiento.DataBind();
                    con.Close();
                }
            }
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
        }
        protected void LlenadoState()
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
        protected void LlenadoStateNIT()
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
                        if (CmbPaisNIT.SelectedValue == "Guatemala")
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
        protected void LlenadoStateNac()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (!String.IsNullOrEmpty(CmbMuncNacimiento.SelectedValue))
                    {
                        string descrip = "";
                        if (CmbPaisNacimiento.SelectedValue == "GTM")
                        {
                            descrip = CmbMuncNacimiento.SelectedValue + "-" + CmbDeptoNacimiento.SelectedValue;
                        }
                        else
                        {
                            descrip = CmbDeptoNacimiento.SelectedValue;
                        }
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + descrip.TrimEnd('-') + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNacimiento.Value = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                    else if (!String.IsNullOrEmpty(StateNacimiento.Value))
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbDeptoNacimiento.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNacimiento.Value = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                }
            }
        }
        protected string EstadoCivil()
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
        public string EnvioCorreo(string nombre, string dpiViejo, string dpiNuevo, string pasViejo, string pasNuevo, string dpiPrincipalNuevo, string dpiPrincipalAnterior, string passPrincipalNuevo, string passPrincipalAnterior)
        {
            string htmlBody = LeerBodyEmail("bodyCambioDocumentoIdentificacion.txt");
            string[] datos = LeerInfoEmail("datosCambioDocumentoIdentificacion.txt");
            string[] credenciales = LeerCredencialesMail();
            string control = "0";
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Actualización Documento de Identificación", credenciales[3]));
            email.To.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.Subject = datos[0];

            htmlBody = htmlBody.Replace("{nombre}", nombre);
            //Se valida que exista cambio en el DPI, siendo el DPI el principal
            if (!String.IsNullOrEmpty(dpiViejo.Trim()) && !String.IsNullOrEmpty(dpiNuevo.Trim()) && (dpiPrincipalAnterior == "Y" && dpiPrincipalNuevo == "Y"))
            {
                htmlBody = htmlBody.Replace("{dpi}", "Modificación de DPI: <br>DPI anterior: <strong>" + dpiViejo + "</strong>. <br> DPI nuevo: <strong>" + dpiNuevo + "</strong> .");
                htmlBody = htmlBody.Replace("{principal}", "");
            }
            else
            {
                htmlBody = htmlBody.Replace("{dpi}", "");
            }
            //Se valida que exista cambio en el Pasaporte , siendo el Pasaporte el principal
            if (!String.IsNullOrEmpty(pasViejo.Trim()) && !String.IsNullOrEmpty(pasNuevo.Trim()) && (passPrincipalAnterior == "Y" && passPrincipalNuevo == "Y"))
            {
                htmlBody = htmlBody.Replace("{pasaporte}", "Modificación de Pasaporte: <br>Pasaporte anterior: <strong>" + pasViejo + "</strong>. <br> Pasaporte nuevo: <strong>" + pasNuevo + " </strong> .");
                htmlBody = htmlBody.Replace("{principal}", "");
            }
            else
            {
                htmlBody = htmlBody.Replace("{pasaporte}", "");
            }

            //Se valida un cambio en el pasaporte
            if (dpiPrincipalAnterior != dpiPrincipalNuevo && passPrincipalAnterior != passPrincipalNuevo && dpiNuevo == dpiViejo && pasViejo == pasNuevo)
            {
                htmlBody = htmlBody.Replace("{principal}", " El documento principal ha cambiado por lo que el usuario debe de modificarse con los datos del DPI <br> Los datos son los siguientes : <br>DPI: <strong>" + dpiNuevo + "</strong> <br>Pasaporte : <strong>" + pasNuevo + " </strong> .");
            }

            if (dpiPrincipalAnterior != dpiPrincipalNuevo && passPrincipalAnterior != passPrincipalNuevo && dpiNuevo != dpiViejo && pasViejo == pasNuevo)
            {
                htmlBody = htmlBody.Replace("{principal}", " El documento principal ha cambiado por lo que el usuario debe de modificarse con los datos del DPI <br> Los datos son los siguientes : <br>DPI anterior: <strong>" + dpiViejo + "</strong> <br>DPI nuevo: <strong>" + dpiNuevo + "</strong> <br>Pasaporte : <strong>" + pasNuevo + " </strong> .");
            }
            if (dpiPrincipalAnterior != dpiPrincipalNuevo && passPrincipalAnterior != passPrincipalNuevo && dpiNuevo != dpiViejo && pasViejo != pasNuevo)
            {
                htmlBody = htmlBody.Replace("{principal}", " El documento principal ha cambiado por lo que el usuario debe de modificarse con los datos del DPI <br> Los datos son los siguientes : <br>DPI anterior: <strong>" + dpiViejo + "</strong> <br>DPI nuevo: <strong>" + dpiNuevo + "</strong> <br>Pasaporte : <strong>" + pasViejo + " </strong> .");
            }

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(credenciales[1], credenciales[2]);

                    smtp.Send(email);
                    smtp.Disconnect(true);

                    log("Envío de Correo", "Correcto", "El Correo fue enviado con exito.", "Envio de correo para informar modificacion de documento de identificación.");
                }
                catch (Exception ex)
                {
                    log("Envío de Correo", "Error", ex.Message, "Envio de correo para informar modificacion de documento de identificación.");
                    lblActualizacion.Text = ex.ToString();
                    control = "1";
                }
            }
            return control;
        }
        public string LeerBodyEmail(string archivo)
        {
            string rutaCompleta = CurrentDirectory + "/Emails/ActualizacionGeneral/" + archivo;
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                file.Close();
            }
            return line;
        }
        public string[] LeerInfoEmail(string archivo)
        {
            string rutaCompleta = CurrentDirectory + "/Emails/ActualizacionGeneral/" + archivo;
            string[] datos;
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                string linea4 = file.ReadLine();
                string subjet = linea2;
                string to = linea4;
                file.Close();

                // Corrección: Inicializa un nuevo array y asigna los valores
                datos = new string[] { subjet, to };
            }
            return datos;
        }
        public string[] LeerCredencialesMail()
        {
            string rutaCompleta = CurrentDirectory + "/Emails/Credenciales.txt";
            string[] datos;
            string nombre = "";
            string correo = "";
            string pass = "";
            string correoVisible = "";
            using (StreamReader file = new StreamReader(rutaCompleta, Encoding.UTF8))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                string linea4 = file.ReadLine();
                string linea5 = file.ReadLine();
                string linea6 = file.ReadLine();


                nombre = linea2;
                correo = linea4;
                pass = linea6;
                correoVisible = linea4;
                file.Close();

                // Corrección: Inicializa un nuevo array y asigna los valores
                datos = new string[] { nombre, correo, pass, correoVisible };
            }
            return datos;
        }
        public string DivisionApellidos(string apellido)
        {
            WebClient _clientW = new WebClient();
            _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            _clientW.Headers.Add("apellido", apellido);
            string json = _clientW.DownloadString(txtApex.Text + "unis_interfaces/Centralizador/ExcepcionesApellidos");
            dynamic respuesta = JsonConvert.DeserializeObject(json).ToString();
            return respuesta;
        }
        public static string GetBetween(string strSource, string strStart, string strEnd)
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
        private string ConsultaNit(string nit)
        {
            var body = "{\"Credenciales\" : \"" + CREDENCIALES_NIT.Value + "\",\"NIT\":\"" + nit + "\"}";
            string respuesta = api.PostNit(URL_NIT.Value, body);
            return respuesta;
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
        static string[] DividirEnArray(string cadena)
        {
            // Dividir la cadena en un array de strings usando los espacios como delimitadores
            string[] arrayDePalabras = cadena.Split(' ');
            return arrayDePalabras;
        }
        public void ConsultaNombre(string NombreBusqueda)
        {
            // Generar el patrón de búsqueda para cubrir tanto con tilde como sin tilde
            string pattern = GenerarPatron(NombreBusqueda);

            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT EMPLID, NAME FROM SYSADM.PS_PERSONAL_VW " +
                    "WHERE REGEXP_LIKE(NAME, '" + pattern + "', 'i')";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                        ExisteBusqueda.Value = "1";
                    }
                    else
                    {
                        ExisteBusqueda.Value = "0";
                    }
                }
            }
        }

        // Método para generar el patrón de búsqueda que cubre variaciones con y sin tilde
        private string GenerarPatron(string nombre)
        {
            StringBuilder pattern = new StringBuilder();
            pattern.Append(".*");

            foreach (char c in nombre)
            {
                switch (char.ToLower(c))
                {
                    case 'a':
                        pattern.Append("[aá]");
                        break;
                    case 'e':
                        pattern.Append("[eé]");
                        break;
                    case 'i':
                        pattern.Append("[ií]");
                        break;
                    case 'o':
                        pattern.Append("[oó]");
                        break;
                    case 'u':
                        pattern.Append("[uú]");
                        break;
                    case 'á':
                        pattern.Append("[aá]");
                        break;
                    case 'é':
                        pattern.Append("[eé]");
                        break;
                    case 'í':
                        pattern.Append("[ií]");
                        break;
                    case 'ó':
                        pattern.Append("[oó]");
                        break;
                    case 'ú':
                        pattern.Append("[uú]");
                        break;
                    default:
                        pattern.Append("[" + char.ToUpper(c) + char.ToLower(c) + "]");
                        break;
                }
            }

            pattern.Append(".*");
            return pattern.ToString();
        }

        public void ConsultarDocumento(string Documento)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = " SELECT EMPLID, NAME FROM SYSADM.PS_PERSONAL_VW " +
                            "WHERE EMPLID = (SELECT EMPLID FROM SYSADM.PS_PERS_NID PN " +
                            "WHERE PN.NATIONAL_ID = '" + Documento + "' " +
                            "FETCH FIRST 1 ROWS ONLY)";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                        ExisteBusqueda.Value = "1";
                    }
                    else
                    {
                        ExisteBusqueda.Value = "0";
                    }
                }
            }
        }
        public void ConsultarId(string Id)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = " SELECT EMPLID, NAME FROM SYSADM.PS_PERSONAL_VW " +
                            "WHERE EMPLID LIKE '%" + Id + "'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                        ExisteBusqueda.Value = "1";
                    }
                    else
                    {
                        ExisteBusqueda.Value = "0";
                    }
                }
            }
        }
        private void LoadData()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("EMPLID");
            dt.Columns.Add("NAME");

            dr["EMPLID"] = String.Empty;
            dr["NAME"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewBusqueda.DataSource = dt;
            this.GridViewBusqueda.DataBind();
        }
        private void LoadDataDocumentos()
        {
            DataTable dt = new DataTable();
            DataRow drDPI, drPasaporte;

            dt.Columns.Add("País");
            dt.Columns.Add("TipoDocumento");
            dt.Columns.Add("Documento");
            dt.Columns.Add("PRIMARY_NID");

            drDPI = dt.NewRow();
            drDPI["País"] = String.Empty;
            drDPI["TipoDocumento"] = "DPI";
            drDPI["Documento"] = String.Empty;
            drDPI["PRIMARY_NID"] = String.Empty;
            dt.Rows.Add(drDPI);

            // Fila para Pasaporte
            drPasaporte = dt.NewRow();
            drPasaporte["País"] = String.Empty;
            drPasaporte["TipoDocumento"] = "Pasaporte";
            drPasaporte["Documento"] = String.Empty;
            drPasaporte["PRIMARY_NID"] = String.Empty;
            dt.Rows.Add(drPasaporte);

            this.GridViewDocumentos.DataSource = dt;
            this.GridViewDocumentos.DataBind();
        }
        private void LoadDataContactos()
        {
            DataTable dt = new DataTable();
            DataRow dr1, dr2;

            dt.Columns.Add("PrincipalCE");
            dt.Columns.Add("Parentesco");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Teléfono");
            dt.Columns.Add("PRIMARY_CONTACT");

            // Fila para Contacto1
            dr1 = dt.NewRow();
            dr1["PrincipalCE"] = String.Empty;
            dr1["Parentesco"] = String.Empty;
            dr1["Nombre"] = String.Empty;
            dr1["Teléfono"] = String.Empty;
            dr1["PRIMARY_CONTACT"] = String.Empty;
            dt.Rows.Add(dr1);

            // Fila para Contacto2
            dr2 = dt.NewRow();
            dr2["PrincipalCE"] = String.Empty;
            dr2["Parentesco"] = String.Empty;
            dr2["Nombre"] = String.Empty;
            dr2["Teléfono"] = String.Empty;
            dr2["PRIMARY_CONTACT"] = String.Empty;
            dt.Rows.Add(dr2);

            this.GridViewContactos.DataSource = dt;
            this.GridViewContactos.DataBind();
        }
        protected string ValidacionAccesoVista(string carnet)
        {
            string constr = TxtURL.Text;
            string facultad = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT COD_FACULTAD " +
                        "FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                        "WHERE DPI ='" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "'  " +
                        "AND COD_FACULTAD = ( " +
                            "SELECT AGT.DESCRSHORT " +
                            "FROM SYSADM.PS_STDNT_CAR_TERM CT " +
                            "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.ACAD_PROG_PRIMARY = APD.ACAD_PROG " +
                            "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                            "AND CT.INSTITUTION = APD.INSTITUTION " +
                            "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                            "AND APD.INSTITUTION = AGT.INSTITUTION " +
                            "JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                            "AND CT.INSTITUTION = TT.INSTITUTION " +
                            "AND (SYSDATE BETWEEN TT.TERM_BEGIN_DT AND TT.TERM_END_DT) " +
                            "WHERE EMPLID ='" + carnet + "'" +
                            "FETCH FIRST 1 ROWS ONLY" +
                        ")";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        facultad = reader["COD_FACULTAD"].ToString();
                    }
                }
            }
            return facultad;
        }
        private void LlenadoGridDocumentos()
        {
            string constr = TxtURL.Text;
            ExisteDPI.Value = "0";
            ExistePasaporte.Value = "0";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT COUNTRY AS Pais, NATIONAL_ID_TYPE as TipoDocumento, NATIONAL_ID as Documento, PRIMARY_NID " +
                        "FROM SYSADM.PS_PERS_NID " +
                    "WHERE EMPLID = '" + txtCarne.Text + "'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();

                    DataTable dt = (DataTable)GridViewDocumentos.DataSource;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string tipoDocumento = reader["TipoDocumento"].ToString();
                            if (tipoDocumento == "DPI")
                            {
                                dt.Rows[0]["País"] = reader["Pais"].ToString();
                                dt.Rows[0]["Documento"] = reader["Documento"].ToString();
                                DOCUMENTO1_INICIAL.Value = reader["Documento"].ToString();
                                dt.Rows[0]["PRIMARY_NID"] = reader["PRIMARY_NID"].ToString();
                                DOCUMENTO1_PRINCIPAL_INICIAL.Value = reader["PRIMARY_NID"].ToString();
                                ExisteDPI.Value = "1";
                            }
                            else if (tipoDocumento == "PAS")
                            {
                                dt.Rows[1]["País"] = reader["Pais"].ToString();
                                dt.Rows[1]["Documento"] = reader["Documento"].ToString();
                                DOCUMENTO2_INCIAL.Value = reader["Documento"].ToString();
                                dt.Rows[1]["PRIMARY_NID"] = reader["PRIMARY_NID"].ToString();
                                DOCUMENTO2_PRINCIPAL_INICIAL.Value = reader["PRIMARY_NID"].ToString();
                                ExistePasaporte.Value = "1";
                            }
                        }
                    }

                    GridViewDocumentos.DataSource = dt;
                    GridViewDocumentos.DataBind();
                }
            }
        }
        protected void LlenarDDLPais(DropDownList ddl)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand("SELECT COUNTRY, DESCR FROM SYSADM.PS_COUNTRY_TBL", con))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Seleccione un país", "")); // Elemento predeterminado

                        while (reader.Read())
                        {
                            string country = reader["COUNTRY"].ToString();
                            string descr = reader["DESCR"].ToString();
                            ddl.Items.Add(new ListItem(descr, country));
                        }
                    }
                }
            }
        }
        protected void LlenarParentezco(DropDownList ddl)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand("SELECT PARENTESCO, ID_CAMPUS FROM UNIS_INTERFACES.TBL_RELACIONES_FAMILIARES", con))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Seleccione una opción", "")); // Elemento predeterminado

                        while (reader.Read())
                        {
                            string ID = reader["ID_CAMPUS"].ToString();
                            string PARENTESCO = reader["PARENTESCO"].ToString();
                            ddl.Items.Add(new ListItem(PARENTESCO, ID));
                        }
                    }
                }
            }
        }
        protected void LlenarHospital()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' DESC_HOSP FROM DUAL UNION SELECT DESC_HOSP FROM SYSADM.PS_XL_CAT_HOSPITAL ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbHospital.DataSource = ds;
                    CmbHospital.DataTextField = "DESC_HOSP";
                    CmbHospital.DataValueField = "DESC_HOSP";
                    CmbHospital.DataBind();
                    con.Close();
                }
            }
        }
        protected void LlenarAntecedentes()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' FIELDNAME, ' ' XLATLONGNAME FROM DUAL UNION SELECT FIELDNAME, XLATLONGNAME FROM SYSADM.PS_XL_CAT_ENFER ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbAntecedentes.DataSource = ds;
                    CmbAntecedentes.DataTextField = "XLATLONGNAME";
                    CmbAntecedentes.DataValueField = "FIELDNAME";
                    CmbAntecedentes.DataBind();
                    con.Close();
                }
            }
        }
        protected void LlenarAlergias()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' FIELDNAME FROM DUAL UNION SELECT FIELDNAME FROM SYSADM.PS_XL_CAT_ALERGIAS ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbAlergias.DataSource = ds;
                    CmbAlergias.DataTextField = "FIELDNAME";
                    CmbAlergias.DataValueField = "FIELDNAME";
                    CmbAlergias.DataBind();
                    con.Close();
                }
            }
        }
        private void LlenadoContactosEmergencia()
        {
            string constr = TxtURL.Text;
            int contador = 0;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT CONTACT_NAME AS Nombre, PHONE as Teléfono, RELATIONSHIP as Parentesco, PRIMARY_CONTACT " +
                        "FROM SYSADM.PS_EMERGENCY_CNTCT " +
                    "WHERE EMPLID = '" + txtCarne.Text + "'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    DataTable dt = (DataTable)GridViewContactos.DataSource;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (contador == 0)
                            {
                                dt.Rows[0]["Nombre"] = reader["Nombre"].ToString();
                                txtNombreE1_Inicial.Value = reader["Nombre"].ToString();
                                dt.Rows[0]["Teléfono"] = reader["Teléfono"].ToString();
                                dt.Rows[0]["Parentesco"] = reader["Parentesco"].ToString();
                                dt.Rows[0]["PRIMARY_CONTACT"] = reader["PRIMARY_CONTACT"].ToString();
                                contador++;
                            }
                            else if (contador == 1)
                            {
                                dt.Rows[1]["Nombre"] = reader["Nombre"].ToString();
                                txtNombreE2_Inicial.Value = reader["Nombre"].ToString();
                                dt.Rows[1]["Teléfono"] = reader["Teléfono"].ToString();
                                dt.Rows[1]["Parentesco"] = reader["Parentesco"].ToString();
                                dt.Rows[1]["PRIMARY_CONTACT"] = reader["PRIMARY_CONTACT"].ToString();
                                contador++;
                            }
                        }
                    }

                    GridViewContactos.DataSource = dt;
                    GridViewContactos.DataBind();
                }
            }
        }
        protected string ContactoEmergenciaCampus(string nombre1, string parentesco1, string telefono1, string principal1, string nombre2, string parentesco2, string telefono2, string principal2, string nombre1_a, string nombre2_a)
        {
            string parentesco1_campus = null;
            string parentesco2_campus = null;

            string SelectParentesco1 = "SELECT ID_CAMPUS " +
                        "FROM UNIS_INTERFACES.TBL_RELACIONES_FAMILIARES " +
                    "WHERE PARENTESCO = '" + parentesco1 + "'";

            string SelectParentesco2 = "SELECT ID_CAMPUS " +
                        "FROM UNIS_INTERFACES.TBL_RELACIONES_FAMILIARES " +
                    "WHERE PARENTESCO = '" + parentesco2 + "'";

            string constr = TxtURL.Text;
            int control = 0;
            string Errores = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = SelectParentesco1;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        parentesco1_campus = reader["ID_CAMPUS"].ToString();
                    }

                    cmd.CommandText = SelectParentesco2;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        parentesco2_campus = reader["ID_CAMPUS"].ToString();
                    }

                    if (principal1 == "N" && principal2 == "Y")
                    {
                        principal1 = "Y";
                    }

                    string InsertContacto1 = "INSERT INTO SYSADM.PS_EMERGENCY_CNTCT (EMPLID, CONTACT_NAME, PHONE, PRIMARY_CONTACT, RELATIONSHIP, SAME_ADDRESS_EMPL,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,CITY,NUM1,NUM2,HOUSE_TYPE,ADDR_FIELD1,ADDR_FIELD2,ADDR_FIELD3,COUNTY,STATE,POSTAL,GEO_CODE,IN_CITY_LIMIT,COUNTRY_CODE,SAME_PHONE_EMPL,ADDRESS_TYPE,PHONE_TYPE,EXTENSION) " +
                    "VALUES ('" + txtEmplid.Value + "', '" + nombre1 + "', '" + telefono1 + "', '" + principal1 + "', '" + parentesco1_campus + "', 'N',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','N',' ',' ',' ')";

                    string UpdateContacto1 = "UPDATE SYSADM.PS_EMERGENCY_CNTCT SET " +
                        "CONTACT_NAME = '" + nombre1 + "', " +
                        "PRIMARY_CONTACT = '" + principal1 + "', " +
                        "PHONE = '" + telefono1 + "', " +
                        "RELATIONSHIP = '" + parentesco1_campus + "' " +
                        "WHERE EMPLID ='" + txtEmplid.Value + "'" +
                        "AND CONTACT_NAME = '" + nombre1_a + "'";

                    string InsertContacto2 = "INSERT INTO SYSADM.PS_EMERGENCY_CNTCT (EMPLID, CONTACT_NAME, PHONE, PRIMARY_CONTACT, RELATIONSHIP,SAME_ADDRESS_EMPL,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,CITY,NUM1,NUM2,HOUSE_TYPE,ADDR_FIELD1,ADDR_FIELD2,ADDR_FIELD3,COUNTY,STATE,POSTAL,GEO_CODE,IN_CITY_LIMIT,COUNTRY_CODE,SAME_PHONE_EMPL,ADDRESS_TYPE,PHONE_TYPE,EXTENSION) " +
                    "VALUES ('" + txtEmplid.Value + "', '" + nombre2 + "', '" + telefono2 + "', '" + principal2 + "', '" + parentesco2_campus + "', 'N', ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','N',' ',' ',' ')";

                    string UpdateContacto2 = "UPDATE SYSADM.PS_EMERGENCY_CNTCT SET " +
                        "CONTACT_NAME = '" + nombre2 + "', " +
                        "PRIMARY_CONTACT = '" + principal2 + "', " +
                        "PHONE = '" + telefono2 + "', " +
                        "RELATIONSHIP = '" + parentesco2_campus + "' " +
                        "WHERE EMPLID ='" + txtEmplid.Value + "'" +
                        "AND CONTACT_NAME = '" + nombre2_a + "'";

                    try

                    {
                        if (String.IsNullOrEmpty(txtNombreE1_Inicial.Value) || txtNombreE1_Inicial.Value == "")
                        {
                            cmd.CommandText = InsertContacto1;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = UpdateContacto1;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception x)
                    {
                        control++;
                        Errores = "||" + x.Message;
                    }

                    try
                    {
                        if (String.IsNullOrEmpty(txtNombreE2_Inicial.Value) || txtNombreE2_Inicial.Value == "")
                        {
                            cmd.CommandText = InsertContacto2;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = UpdateContacto2;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception x)
                    {
                        control++;
                        Errores = "||" + x.Message;
                    }

                    if (control == 0)
                    {
                        transaction.Commit();
                        log("Función ContactoEmergenciaCampus", "Correcto", "Los contactos de emergencia fueron almacenados de forma correcta", "ContactoEmergenciaCampus");

                    }
                    else
                    {
                        transaction.Rollback();
                        log("Función ContactoEmergenciaCampus", "Error", Errores, "ContactoEmergenciaCampus");
                    }
                    con.Close();
                }
            }
            return control.ToString();
        }
        private void LlenadoDatosMedicos()
        {
            string constr = TxtURL.Text;
            EmplidAtencion.Value = null;

            //Diccionario de datos para hacer coincidir los valores de la lista del tipo de sangre
            var bloodTypeMapping = new Dictionary<string, string>
            {
                { "O+", "OP" },
                { "O-", "ON" },
                { "A+", "AP" },
                { "A-", "AN" },
                { "B+", "BP" },
                { "B-", "BN" },
                { "AB+", "ABP" },
                { "AB-", "ABN" }
            };

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT HOSPITAL_TRASLADO, NRO_AFILIACION, SEGURO_MEDICO, TIPO_SANGRE, EMPLID, CARRO_CAMPUS, OTRO_HOSPITAL " +
                        "FROM SYSADM.PS_UNIS_ATEN_EMERG " +
                    "WHERE EMPLID = '" + txtCarne.Text + "'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CmbHospital.SelectedValue = reader["HOSPITAL_TRASLADO"].ToString();
                        TxtAfiliacion.Text = reader["NRO_AFILIACION"].ToString().TrimEnd();
                        TxtCarro.Text = reader["CARRO_CAMPUS"].ToString().Trim();
                        TxtSeguro.Text = reader["SEGURO_MEDICO"].ToString().Trim();
                        string tipoSangre = reader["TIPO_SANGRE"].ToString();

                        if (bloodTypeMapping.ContainsKey(tipoSangre))
                        {
                            CmbSangre.SelectedValue = bloodTypeMapping[tipoSangre];
                        }
                        EmplidAtencion.Value = reader["EMPLID"].ToString();
                        TxtOtroHospital.Text = reader["OTRO_HOSPITAL"].ToString().Trim();
                    }

                }
            }
        }
        private void LlenadoDatosAlergias()
        {
            string constr = TxtURL.Text;

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    // Concatenar valores de ALERGIAS
                    cmd.CommandText = "SELECT DISTINCT(ALERGIAS) " +
                                      "FROM SYSADM.PS_UNIS_RG_ALERGIA " +
                                      "WHERE EMPLID = :emplid";
                    cmd.Parameters.Add(new OracleParameter("emplid", txtCarne.Text));
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    StringBuilder sb = new StringBuilder();
                    HashSet<string> uniqueValues = new HashSet<string>();

                    while (reader.Read())
                    {
                        string value = reader["ALERGIAS"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(value) && uniqueValues.Add(value))
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(",");
                            }
                            sb.Append(value);
                        }
                    }
                    reader.Close();

                    // Concatenar valores de OTRA_ALERGIA
                    cmd.CommandText = "SELECT DISTINCT(OTRA_ALERGIA) " +
                                      "FROM SYSADM.PS_UNIS_RG_ALERGIA " +
                                      "WHERE EMPLID = :emplid";
                    reader = cmd.ExecuteReader();
                    StringBuilder sb2 = new StringBuilder();
                    uniqueValues.Clear(); // Limpiar el conjunto para el segundo conjunto de valores

                    while (reader.Read())
                    {
                        string value = reader["OTRA_ALERGIA"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(value) && uniqueValues.Add(value))
                        {
                            if (sb2.Length > 0)
                            {
                                sb2.Append(",");
                            }
                            sb2.Append(value);
                        }
                    }
                    reader.Close();

                    string resultado = sb2.ToString();
                    TxtOtrasAlergias.Text = resultado.Trim();
                    seleccionadosInicialAlergia.Value = sb.ToString();
                    seleccionadosInicialOtrosAlergia.Value = resultado;

                    // Asignar valores a CmbAlergias
                    SelectValuesInListBox(sb.ToString(), CmbAlergias);
                }
            }
        }
        private void LlenadoDatosEnfermedades()
        {
            string constr = TxtURL.Text;
            //EmplidAtencion.Value = null;

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    // Concatenar valores de ANTECEDENTES
                    cmd.CommandText = "SELECT DISTINCT(ANTECEDENTES_MED) " +
                                      "FROM SYSADM.PS_UNIS_RG_ANT_MED " +
                                      "WHERE EMPLID = :emplid";
                    cmd.Parameters.Add(new OracleParameter("emplid", txtCarne.Text));
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    StringBuilder sb = new StringBuilder();
                    HashSet<string> uniqueValues = new HashSet<string>();

                    while (reader.Read())
                    {
                        string value = reader["ANTECEDENTES_MED"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(value) && uniqueValues.Add(value))
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(",");
                            }
                            sb.Append(value);
                        }
                    }
                    reader.Close();

                    // Concatenar valores de OTRO ANTECEDENTE
                    cmd.CommandText = "SELECT DISTINCT(OTRO_ANTECEDENTE) " +
                                      "FROM SYSADM.PS_UNIS_RG_ANT_MED " +
                                      "WHERE EMPLID = :emplid";
                    reader = cmd.ExecuteReader();
                    StringBuilder sb2 = new StringBuilder();
                    uniqueValues.Clear(); // Limpiar el conjunto para el segundo conjunto de valores

                    while (reader.Read())
                    {
                        string value = reader["OTRO_ANTECEDENTE"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(value) && uniqueValues.Add(value))
                        {
                            if (sb2.Length > 0)
                            {
                                sb2.Append(",");
                            }
                            sb2.Append(value);
                        }
                    }
                    reader.Close();

                    string resultado = sb2.ToString();
                    TxtOtrosAntecedentesM.Text = resultado.Trim();
                    seleccionadosInicialAntecedentes.Value = sb.ToString();
                    seleccionadosInicialOtrosAntecedentes.Value = resultado;

                    // Asignar valores a CmbAlergias
                    SelectValuesInListBox(sb.ToString(), CmbAntecedentes);
                }
            }
        }
        private void SelectValuesInListBox(string values, ListBox listBox)
        {
            if (!string.IsNullOrWhiteSpace(values))
            {
                string[] items = values.Split(',');

                foreach (string item in items)
                {
                    string trimmedItem = item.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedItem))
                    {
                        ListItem listItem = listBox.Items.FindByText(trimmedItem);
                        if (listItem != null)
                        {
                            listItem.Selected = true;
                        }
                    }
                }
            }
        }
        
        protected string IngresoDatosGenerales()
        {
            txtNombreAPEX.Text = null;
            string constr = TxtURL.Text;
            string codPais = "";
            string codPaisNIT = "";
            string ec = EstadoCivil();
            int largoApellido = txtApellido.Text.Length;
            int espaciosApellido = ContarEspacios(txtApellido.Text);
            int espaciosNombre = ContarEspacios(txtNombre.Text);
            string[] nombres = txtNombre.Text.TrimEnd(' ').Split(' ');
            int nombresTotal = nombres.Length;
            string mensaje = null;

            if (txtAInicial.Value == "\r\n")
            {
                txtAInicial.Value = null;
            }
            if (txtNInicial.Value == "\r\n")
            {
                txtNInicial.Value = null;
            }
            if (txtCInicial.Value == "\r\n")
            {
                txtCInicial.Value = null;
            }

            if (String.IsNullOrEmpty(txtNit.Text))
            {
                txtNit.Text = "CF";
            }

            if (RadioButtonNombreSi.Checked && (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value) || ControlCF.Value != "CF"))
            {
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                txtNit.Text = "CF";
            }


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

                    //Obtener codigo país nit
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNTRY FROM SYSADM.PS_COUNTRY_TBL WHERE DESCR = '" + CmbPaisNIT.SelectedValue + "'";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        codPaisNIT = reader["COUNTRY"].ToString();
                    }



                    cmd.Transaction = transaction;
                    txtExiste3.Text = txtPrimerApellido.Text + " insert";
                    
                    if (String.IsNullOrEmpty(StateNIT.Text))
                        StateNIT.Text = State.Text;


                    if (RadioButtonNombreSi.Checked && ((InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim()) || String.IsNullOrEmpty(InicialNR1.Value)))
                    {
                        TxtNombreR.Text = txtNombre.Text;
                        TxtApellidoR.Text = txtApellido.Text;
                        TxtCasadaR.Text = txtCasada.Text;
                        TxtDiRe1.Text = txtDireccion.Text;
                        TxtDiRe2.Text = txtDireccion2.Text;
                        TxtDiRe3.Text = txtDireccion3.Text;
                        txtNit.Text = "CF";
                    }


                    if (String.IsNullOrEmpty(codPaisNIT))
                        codPaisNIT = codPais;

                    cmd.Connection = con;
                    string consultaUP = "1";
                    string consultaUD = "1";
                    int contarTelefono = 0;
                    try
                    {
                        //contador telefonos HOME
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT (*) CONTADOR FROM SYSADM.PS_PERSONAL_PHONE WHERE EMPLID = '" + txtCarne.Text + "' AND PHONE_TYPE ='HOME'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            contarTelefono = Convert.ToInt16(reader["CONTADOR"].ToString());
                        }
                        //Numero de Telefono
                        if (!String.IsNullOrEmpty(TruePhone.Text) && contarTelefono > 0)
                        { //UPDATE
                            UD_PERSONAL_PHONE.Value = "<COLL_PERSONAL_PHONE> \n" +
                                                        "                                            <EMPLID>" + txtCarne.Text + @"</EMPLID>" +
                                                        "\n" +
                                                        "                                            <KEYPROP_PHONE_TYPE>HOME</KEYPROP_PHONE_TYPE> \n" +
                                                        "                                            <PROP_PHONE>" + txtTelefono.Text + @"</PROP_PHONE>" +
                                                        "\n" +
                                                        "                                            <PROP_PREF_PHONE_FLAG>Y</PROP_PREF_PHONE_FLAG> \n" +
                                                     "                                         </COLL_PERSONAL_PHONE> \n";
                            contadorUD++;
                        }
                        else
                        {//INSERT
                            UP_PERSONAL_PHONE.Value = "<COLL_PERSONAL_PHONE> \n" +
                                                        "                                            <EMPLID>" + txtCarne.Text + @"</EMPLID>" +
                                                        "\n" +
                                                        "                                            <KEYPROP_PHONE_TYPE>HOME</KEYPROP_PHONE_TYPE> \n" +
                                                        "                                            <PROP_PHONE>" + txtTelefono.Text + @"</PROP_PHONE>" +
                                                        "\n" +
                                                        "                                            <PROP_PREF_PHONE_FLAG>Y</PROP_PREF_PHONE_FLAG> \n" +
                                                     "                                         </COLL_PERSONAL_PHONE> \n";
                            contadorUP++;
                        }

                        //EMAIL PERSONAL
                        if (!String.IsNullOrEmpty(TrueEmail.Text))
                        {//UPDATE

                            UD_EMAIL_ADDRESSES.Value = "<COLL_EMAIL_ADDRESSES>\n" +
                                                            "                                            <KEYPROP_E_ADDR_TYPE>HOM1</KEYPROP_E_ADDR_TYPE> \n" +
                                                            "                                            <PROP_EMAIL_ADDR>" + TxtCorreoPersonal.Text + @"</PROP_EMAIL_ADDR> " +
                                                            "\n" +
                                                            "                                            <PROP_PREF_EMAIL_FLAG>N</PROP_PREF_EMAIL_FLAG> \n" +
                                                         "                                         </COLL_EMAIL_ADDRESSES> \n";
                            contadorUD++;
                        }
                        else
                        {//INSERT
                            UP_EMAIL_ADDRESSES.Value = "<COLL_EMAIL_ADDRESSES>\n" +
                                                            "                                            <KEYPROP_E_ADDR_TYPE>HOM1</KEYPROP_E_ADDR_TYPE> \n" +
                                                            "                                            <PROP_EMAIL_ADDR>" + TxtCorreoPersonal.Text + @"</PROP_EMAIL_ADDR> " +
                                                            "\n" +
                                                            "                                            <PROP_PREF_EMAIL_FLAG>N</PROP_PREF_EMAIL_FLAG> \n" +
                                                         "                                         </COLL_EMAIL_ADDRESSES> \n";
                            contadorUP++;
                        }

                        //Direccion
                        int ContadorDirecciones = 0;
                        int ContadorEffdtDirecciones = 0;
                        string EffdtDireccionUltimo = "";
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + UserEmplid.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtDirecciones = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + UserEmplid.Text + "' " +
                             " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtDireccionUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        if (!String.IsNullOrEmpty(EffdtDireccionUltimo))
                        {

                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + UserEmplid.Text + "' " +
                                "AND ADDRESS1 ='" + txtDireccion.Text + "' AND ADDRESS2 = '" + txtDireccion2.Text + "' AND ADDRESS3 = '" + txtDireccion3.Text + "'" +
                                "AND COUNTRY='" + codPais + "' AND STATE ='" + State.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionUltimo).ToString("dd/MM/yyyy") + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorDirecciones = Convert.ToInt16(reader["CONTADOR"]);
                            }
                        }
                        else
                        {
                            ContadorDirecciones = 0;
                        }

                        if (txtNit.Text == "CF" && (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value)))
                        {
                            StateNIT.Text = State.Text;
                        }

                        if (EffdtDireccionUltimo != Hoy && ContadorDirecciones == 0 && ContadorEffdtDirecciones == 0)
                        {//INSERT
                            UP_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                    "                                            <COLL_ADDRESSES> \n" +
                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                      "\n" +
                                                      "                                                <PROP_COUNTRY>" + codPais + @"</PROP_COUNTRY> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS3>" + txtDireccion3.Text + @"</PROP_ADDRESS3> " +
                                                      "\n" +
                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                      "\n" +
                                                    "                                            </COLL_ADDRESSES> \n" +
                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUP++;
                        }
                        else if (EffdtDireccionUltimo == Hoy && ContadorDirecciones > 0 && ContadorEffdtDirecciones > 0)
                        {
                            //UPDATE
                            UD_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                    "                                            <COLL_ADDRESSES> \n" +
                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                      "\n" +
                                                      "                                                <PROP_COUNTRY>" + codPais + @"</PROP_COUNTRY> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS3>" + txtDireccion3.Text + @"</PROP_ADDRESS3> " +
                                                      "\n" +
                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                      "\n" +
                                                    "                                            </COLL_ADDRESSES> \n" +
                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD++;
                        }
                        else
                        {
                            //UPDATE
                            UD_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                    "                                            <COLL_ADDRESSES> \n" +
                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                      "                                                <KEYPROP_EFFDT>" + EffdtDireccionUltimo + @"</KEYPROP_EFFDT> " +
                                                      "\n" +
                                                      "                                                <PROP_COUNTRY>" + codPais + @"</PROP_COUNTRY> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS3>" + txtDireccion3.Text + @"</PROP_ADDRESS3> " +
                                                      "\n" +
                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                      "\n" +
                                                    "                                            </COLL_ADDRESSES> \n" +
                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD++;
                        }

                        //Estado Civil
                        if (TrueEstadoCivil.Value != ec)
                        {
                            if (EFFDT_EC.Value != Hoy)
                            {
                                UP_PERS_DATA_EFFDT.Value = "<COLL_PERS_DATA_EFFDT>\n" +
                                    "\n" +
                                                    "                                            <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "\n" +
                                                    "                                             <PROP_MAR_STATUS>" + ec + @"</PROP_MAR_STATUS>" +
                                                    "\n" +
                                                     "                                            <PROP_SEX>" + SEX_EC.Value + "</PROP_SEX>" +
                                                    "\n" +
                                                     "                                            <PROP_HIGHEST_EDUC_LVL>" + HIGH_LVL.Value + "</PROP_HIGHEST_EDUC_LVL>" +
                                                    "\n" +
                                                     "                                            <PROP_FT_STUDENT>" + FT_STUDENT.Value + "</PROP_FT_STUDENT>" +
                                                    "\n" +
                                                    "                                            </COLL_PERS_DATA_EFFDT>";
                                contadorUP++;
                            }
                            else
                            {
                                UD_PERS_DATA_EFFDT.Value = "<COLL_PERS_DATA_EFFDT>" +
                                                    " <KEYPROP_EFFDT>" + EFFDT_EC.Value + @"</KEYPROP_EFFDT>" +
                                                    " <PROP_MAR_STATUS>" + ec + @"</PROP_MAR_STATUS>" +
                                                     "</COLL_PERS_DATA_EFFDT>";
                                contadorUD++;
                            }
                        }

                        if (!String.IsNullOrEmpty(TxtNombreR.Text))
                        {
                            if (txtAInicial.Value == txtApellido.Text && txtNInicial.Value == txtNombre.Text && txtCInicial.Value.TrimEnd() == txtCasada.Text)
                            {
                                int ContadorNombreNit = 0;
                                int ContadorEffdtNombreNit = 0;
                                int ContadorEffdtNit = 0;
                                int ContadorEffdtDirecionNit = 0;
                                string EffdtDireccionNitUltimo = "";
                                string EffdtNombreNitUltimo = "";
                                string EffdtNitUltimo = "";
                                int ContadorDirecionNit = 0;
                                int ContadorNit = 0;
                                int ContadorNit2 = 0;
                                string EFFDT_SYSTEM = "";

                                string ApellidoAnterior = "";
                                string ApellidoCAnterior = "";

                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND  EMPLID = '" + UserEmplid.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    ContadorEffdtDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                                }

                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + UserEmplid.Text + "' " +
                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    EffdtDireccionNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                                }

                                if (!String.IsNullOrEmpty(EffdtDireccionNitUltimo))
                                {
                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + UserEmplid.Text + "' " +
                                        "AND ADDRESS1 ='" + TxtDiRe1.Text + "' AND ADDRESS2 = '" + TxtDiRe2.Text + "' AND ADDRESS3 = '" + TxtDiRe3.Text + "' " +
                                        "AND COUNTRY='" + codPaisNIT + "' AND STATE ='" + StateNIT.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                        " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                                    }
                                }
                                else
                                {
                                    ContadorDirecionNit = 0;
                                }

                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE = 'REC' AND EMPLID = '" + UserEmplid.Text + "' " +
                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    EffdtNombreNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                                }

                                cmd.CommandText = "SELECT EFFDT AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' AND EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    EFFDT_SYSTEM = reader["CONTADOR"].ToString();
                                }

                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + UserEmplid.Text + "'" +
                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    EffdtNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("dd-MM-yyyy")).ToString();
                                }

                                if (!String.IsNullOrEmpty(EffdtNitUltimo))
                                {
                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND  EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' AND EMPLID = '" + UserEmplid.Text + "' AND EFFDT='" + EffdtNitUltimo + "'";
                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                                    }
                                }
                                else
                                {
                                    ContadorNit = 0;
                                    EffdtNitUltimo = Hoy;
                                }

                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSKEY WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + UserEmplid.Text + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    ContadorNit2 = Convert.ToInt16(reader["CONTADOR"]);
                                }

                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                    "AND EFFDT ='" + HoyEffdt + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    ContadorEffdtNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                                }
                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + UserEmplid.Text + "' " +
                                    "AND EFFDT ='" + HoyEffdt + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    ContadorEffdtNit = Convert.ToInt16(reader["CONTADOR"]);
                                }

                                if (!String.IsNullOrEmpty(EffdtNombreNitUltimo))
                                {
                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoR.Text + "' " +
                                        "AND FIRST_NAME='" + TxtNombreR.Text + "' AND SECOND_LAST_NAME='" + TxtCasadaR.Text + "' " +
                                        "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                                    }

                                    cmd.CommandText = "SELECT LAST_NAME , SECOND_LAST_NAME FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ApellidoAnterior = reader["LAST_NAME"].ToString();
                                        ApellidoCAnterior = reader["SECOND_LAST_NAME"].ToString();
                                    }
                                }
                                else
                                {
                                    ContadorNombreNit = 0;
                                }

                                string FechaEfectiva = "";
                                if (EFFDT_NameR.Value.IsNullOrWhiteSpace())
                                    FechaEfectiva = "1900-01-01";
                                else
                                    FechaEfectiva = EFFDT_NameR.Value;
                                TxtApellidoR.Text.Replace(Environment.NewLine, string.Empty);
                                TxtNombreR.Text.Replace(Environment.NewLine, string.Empty);
                                TxtCasadaR.Text.Replace(Environment.NewLine, string.Empty);
                                TxtApellidoR.Text = System.Text.RegularExpressions.Regex.Replace(TxtApellidoR.Text, @"\s+", " "); ;
                                TxtNombreR.Text = System.Text.RegularExpressions.Regex.Replace(TxtNombreR.Text, @"\s+", " "); ;
                                TxtCasadaR.Text = System.Text.RegularExpressions.Regex.Replace(TxtCasadaR.Text, @"\s+", " ");

                                TxtApellidoR.Text = TxtApellidoR.Text.TrimEnd();
                                TxtNombreR.Text = TxtNombreR.Text.TrimEnd();
                                TxtCasadaR.Text = TxtCasadaR.Text.TrimEnd();

                                if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit == 0)
                                {//INSERT
                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                    {
                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                        {
                                            UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "        <COLL_NAMES>" +
                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                            "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                            "        </COLL_NAMES>" +
                                                            "      </COLL_NAME_TYPE_VW>";
                                        }
                                        else
                                        {
                                            UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "        <COLL_NAMES>" +
                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                            "        </COLL_NAMES>" +
                                                            "      </COLL_NAME_TYPE_VW>";
                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                            {
                                                //ACTUALIZA NIT
                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                    " WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                           "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                           "        <COLL_NAMES>" +
                                                           "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                           "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                           "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                           "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                           "        </COLL_NAMES>" +
                                                           "      </COLL_NAME_TYPE_VW>";
                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }

                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }
                                    }
                                    contadorUP++;
                                }
                                else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit > 0 && ContadorEffdtNombreNit > 0)
                                {//UPDATE
                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                    {
                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                        {
                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "        <COLL_NAMES>" +
                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                            "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                            "        </COLL_NAMES>" +
                                                            "      </COLL_NAME_TYPE_VW>";
                                        }
                                        else
                                        {
                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "        <COLL_NAMES>" +
                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                            "        </COLL_NAMES>" +
                                                            "      </COLL_NAME_TYPE_VW>";

                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                            {
                                                //ACTUALIZA NIT
                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "        <COLL_NAMES>" +
                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                            "        </COLL_NAMES>" +
                                                            "      </COLL_NAME_TYPE_VW>";
                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }

                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }
                                    }

                                    contadorUD++;
                                }
                                else
                                {
                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                    {
                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                        {

                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                    "        <COLL_NAMES>" +
                                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                    "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                    "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                    "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                    "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                                    "        </COLL_NAMES>" +
                                                                    "      </COLL_NAME_TYPE_VW>";
                                        }
                                        else
                                        {

                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                    "        <COLL_NAMES>" +
                                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                    "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                    "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                    "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                    "          </COLL_NAMES>" +
                                                                    "      </COLL_NAME_TYPE_VW>";
                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                            {
                                                //ACTUALIZA NIT
                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                   "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                   "        <COLL_NAMES>" +
                                                                   "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                   "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                   "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                   "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                   "          </COLL_NAMES>" +
                                                                   "      </COLL_NAME_TYPE_VW>";
                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                    " WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }

                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                        {
                                            //ACTUALIZA NIT
                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + UserEmplid.Text + "' " +
                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                        }
                                    }
                                    contadorUD++;
                                }

                                if (EffdtNitUltimo != HoyEffdt && ContadorNit == 0)
                                {
                                    //INSERTA EL NIT
                                    cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) " +
                                    "VALUES ('" + UserEmplid.Text + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + txtNit.Text + "')";
                                    cmd.ExecuteNonQuery();


                                    if (ContadorNit2 == 0)
                                    {
                                        cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                        "VALUES ('" + UserEmplid.Text + "','NRE')";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else if (EffdtNitUltimo == HoyEffdt && ContadorNit > 0)
                                {
                                    //ACTUALIZA NIT
                                    cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' " +
                                                        " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + UserEmplid.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                                    cmd.ExecuteNonQuery();

                                }
                                else
                                {
                                    //ACTUALIZA NIT
                                    cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' " +
                                                        " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + UserEmplid.Text + "' AND EFFDT ='" + EffdtNitUltimo + "'";
                                    cmd.ExecuteNonQuery();
                                }


                                if (EffdtDireccionNitUltimo != Hoy && ContadorDirecionNit == 0 && ContadorEffdtDirecionNit == 0)
                                {//INSERTA
                                    UP_ADDRESSES_NIT.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                    "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                    "                                            <COLL_ADDRESSES> \n" +
                                                      "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                      "\n" +
                                                      "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                      "\n" +
                                                      "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
                                                      "\n" +
                                                    "                                            </COLL_ADDRESSES> \n" +
                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                    contadorUP++;
                                }
                                else if (EffdtDireccionNitUltimo == Hoy && ContadorDirecionNit > 0 && ContadorEffdtDirecionNit > 0)
                                {//ACTUALIZA
                                    UD_ADDRESSES_NIT.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                    "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                    "                                            <COLL_ADDRESSES> \n" +
                                                      "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                      "\n" +
                                                      "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                      "\n" +
                                                      "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                      "\n" +
                                                      "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
                                                      "\n" +
                                                    "                                            </COLL_ADDRESSES> \n" +
                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                    contadorUD++;
                                }
                                else
                                {//UPDATE
                                    UD_ADDRESSES_NIT.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                        "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                        "                                            <COLL_ADDRESSES> \n" +
                                                          "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                          "                                                <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT> " +
                                                          "\n" +
                                                          "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                          "\n" +
                                                          "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                          "\n" +
                                                          "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                          "\n" +
                                                          "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                          "\n" +
                                                          "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
                                                          "\n" +
                                                        "                                            </COLL_ADDRESSES> \n" +
                                                     "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                    contadorUD++;
                                }
                            }
                            else
                            {
                                LlenadoPaisnit();
                            }
                        }

                        //LUGAR DE NACIMIENTO
                        if (String.IsNullOrEmpty(LugarNacimiento.Value))
                        {
                            UP_BIRTHPLACE.Value = "<PROP_BIRTHPLACE>" + TxtLugarNac.Text + "</PROP_BIRTHPLACE>";
                            contadorUP++;
                        }
                        else
                        {
                            UD_BIRTHPLACE.Value = "<PROP_BIRTHPLACE>" + TxtLugarNac.Text + "</PROP_BIRTHPLACE>";
                            contadorUD++;
                        }

                        //PAIS DE NACIMIENTO
                        if (String.IsNullOrEmpty(PaisNacimiento.Value))
                        {
                            UP_BIRTHCOUNTRY.Value = "<PROP_BIRTHCOUNTRY>" + CmbPaisNacimiento.SelectedValue + "</PROP_BIRTHCOUNTRY>";
                            contadorUP++;
                        }
                        else
                        {
                            UD_BIRTHCOUNTRY.Value = "<PROP_BIRTHCOUNTRY>" + CmbPaisNacimiento.SelectedValue + "</PROP_BIRTHCOUNTRY>";
                            contadorUD++;
                        }

                        //STATE NACIMIENTO
                        LlenadoStateNac();

                        if (String.IsNullOrEmpty(StateNacimiento.Value) && !String.IsNullOrEmpty(CmbMuncNacimiento.SelectedValue))
                        {
                            UP_BIRTHSTATE.Value = "<PROP_BIRTHSTATE>" + StateNacimiento.Value + "</PROP_BIRTHSTATE>";
                            contadorUP++;
                        }
                        else if (!String.IsNullOrEmpty(StateNacimiento.Value))
                        {
                            UD_BIRTHSTATE.Value = "<PROP_BIRTHSTATE>" + StateNacimiento.Value + "</PROP_BIRTHSTATE>";
                            contadorUD++;
                        }

                        //FECHA NACIMIENTO
                        if (String.IsNullOrEmpty(txtCumple.Text))
                        {
                            UP_BIRTHDATE.Value = "<PROP_BIRTHDATE>" + txtCumple.Text + "</PROP_BIRTHDATE>";
                            contadorUP++;
                        }
                        else
                        {
                            UD_BIRTHDATE.Value = "<PROP_BIRTHDATE>" + txtCumple.Text + "</PROP_BIRTHDATE>";
                            contadorUD++;
                        }

                        //NOMBRES
                        int ContadorNombre = 0;
                        int ContadorDirecion = 0;
                        int ContadorEffdtNombre = 0;
                        string EffdtNombreUltimo = "";
                        string vchrApellidosCompletos = (txtApellido + " " + txtCasada.Text).TrimEnd();

                        string EFFDT_Name = "";

                        if (txtCasada.Text.IsNullOrWhiteSpace())
                        {
                            txtCasada.Text = " ";
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE != 'REC' AND EMPLID = '" + txtCarne.Text + "' " +
                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        OracleDataReader reader1 = cmd.ExecuteReader();
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            EffdtNombreUltimo = (Convert.ToDateTime(reader1["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE" +
                            " NAME = '" + vchrApellidosCompletos + "," + txtNombre.Text + "' " +
                            "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreUltimo).ToString("dd/MM/yyyy") + "' " +
                            "AND NAME_TYPE != 'REC' AND EMPLID = '" + txtCarne.Text + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE !='REC' AND EMPLID = '" + txtCarne.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            EFFDT_Name = reader1["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_Name.Length == 9)
                            {
                                EFFDT_Name = reader1["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader1["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader1["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_Name = reader1["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader1["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader1["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'HOME' AND EMPLID = '" + txtCarne.Text + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorDirecion = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'PRI' AND PN.EMPLID = '" + txtCarne.Text + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorEffdtNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }
                        if (EffdtNombreUltimo != Hoy && ContadorNombre == 0 && ContadorEffdtNombre == 0)
                        {
                            // INSERT
                            if (!txtApellido.Text.IsNullOrWhiteSpace())
                            {
                                if (!txtCasada.Text.IsNullOrWhiteSpace())
                                {
                                    UP_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UP_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UP_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UP_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                            }
                            else
                            {
                                UP_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                UP_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }
                            contadorUP++;
                        }
                        else if (EffdtNombreUltimo == Hoy && ContadorNombre > 0 && ContadorEffdtNombre > 0)
                        {
                            if (!txtApellido.Text.IsNullOrWhiteSpace())
                            {
                                if (!txtCasada.Text.IsNullOrWhiteSpace())
                                {
                                    // ACTUALIZAR
                                    UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                            }
                            else
                            {
                                UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }
                            contadorUD++;
                        }
                        else
                        {
                            // ACTUALIZAR
                            if (!txtApellido.Text.IsNullOrWhiteSpace())
                            {
                                if (!txtCasada.Text.IsNullOrWhiteSpace())
                                {
                                    UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "          <PROP_SECOND_LAST_NAME>" + txtCasada.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                        "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "        <COLL_NAMES>" +
                                                        "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                        "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                        "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                        "          <PROP_LAST_NAME>" + txtApellido.Text + @"</PROP_LAST_NAME>" +
                                                        "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                        "        </COLL_NAMES>" +
                                                        "      </COLL_NAME_TYPE_VW>";
                                }
                            }
                            else
                            {
                                UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + txtNombre.Text + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }
                            contadorUD++;
                        }


                        auxConsulta = 0;
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }

                        auxConsulta = 1;
                        if (contadorUD > 0)
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            transaction.Commit();
                            con.Close();
                            mensaje = "0";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                        }
                        else
                        {
                            transaction.Rollback();
                            mensaje = "1";
                            log("Función IngresoDatosGenerales", "ERROR", " SOAP: " + Variables.soapBody, "ALMACENAMIENTO SOAP");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                        }
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        log("Función IngresoDatosGenerales", "ERROR", x.Message +"-" + Variables.soapBody, "IngresoDatosGenerales");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                    }
                }
            }
            return mensaje;
        }
        protected (string UP_Doc, string UD_Doc) RecorrerDocumentos()
        {
            string UP_PROP_NID = "";
            string UD_PROP_NID = "";
            foreach (GridViewRow row in GridViewDocumentos.Rows)
            {
                // Accede a los controles de la fila actual                
                DropDownList DDLPais = (DropDownList)row.FindControl("DDLPais");
                TextBox TxtNroDocumento = (TextBox)row.FindControl("TxtNroDocumento");

                // Accede a los valores de los controles
                string pais = DDLPais.SelectedValue;
                string tipoDocumento = row.Cells[2].Text;
                string documento = TxtNroDocumento.Text;

                if (tipoDocumento == "Pasaporte")
                    tipoDocumento = "PAS";

                if (!String.IsNullOrEmpty(documento))
                {
                    if (ExisteDPI.Value == "0" && tipoDocumento == "DPI")
                    {
                        UP_PROP_NID = "<COLL_PERS_NID>\n" +
                                    "   <KEYPROP_COUNTRY>" + pais + "</KEYPROP_COUNTRY> \n" +
                                    "   <KEYPROP_NATIONAL_ID_TYPE>" + tipoDocumento + "</KEYPROP_NATIONAL_ID_TYPE> \n" +
                                    "   <PROP_PRIMARY_NID>" + DOCUMENTO1_PRINCIPAL.Value + "</PROP_PRIMARY_NID>\n " +
                                    "   <PROP_TAX_REF_ID_SGP>N</PROP_TAX_REF_ID_SGP>\n " +
                                    "   <PROP_NATIONAL_ID>" + documento + "</PROP_NATIONAL_ID>\n " +
                                    "</COLL_PERS_NID>\n " + UP_PROP_NID;

                        PAIS_DOCUMENTO1.Value = pais;
                        TIPO_DOCUMENTO1.Value = tipoDocumento;
                        DOCUMENTO1.Value = documento;
                        contadorUP++;
                    }
                    else if (tipoDocumento == "DPI")
                    {
                        UD_PROP_NID = "<COLL_PERS_NID>\n" +
                                    "   <KEYPROP_COUNTRY>" + pais + "</KEYPROP_COUNTRY> \n" +
                                    "   <KEYPROP_NATIONAL_ID_TYPE>" + tipoDocumento + "</KEYPROP_NATIONAL_ID_TYPE> \n" +
                                    "   <PROP_PRIMARY_NID>" + DOCUMENTO1_PRINCIPAL.Value + "</PROP_PRIMARY_NID>\n " +
                                    "   <PROP_TAX_REF_ID_SGP>N</PROP_TAX_REF_ID_SGP>\n " +
                                    "   <PROP_NATIONAL_ID>" + documento + "</PROP_NATIONAL_ID>\n " +
                                    "</COLL_PERS_NID>\n " + UD_PROP_NID;

                        PAIS_DOCUMENTO1.Value = pais;
                        TIPO_DOCUMENTO1.Value = tipoDocumento;
                        DOCUMENTO1.Value = documento;
                        contadorUD++;
                    }
                    if (ExistePasaporte.Value == "0" && tipoDocumento == "PAS")
                    {
                        UP_PROP_NID = "<COLL_PERS_NID>\n" +
                                    "   <KEYPROP_COUNTRY>" + pais + "</KEYPROP_COUNTRY> \n" +
                                    "   <KEYPROP_NATIONAL_ID_TYPE>" + tipoDocumento + "</KEYPROP_NATIONAL_ID_TYPE> \n" +
                                    "   <PROP_PRIMARY_NID>" + DOCUMENTO2_PRINCIPAL.Value + "</PROP_PRIMARY_NID>\n " +
                                    "   <PROP_TAX_REF_ID_SGP>N</PROP_TAX_REF_ID_SGP>\n " +
                                    "   <PROP_NATIONAL_ID>" + documento + "</PROP_NATIONAL_ID>\n " +
                                    "</COLL_PERS_NID>\n " + UP_PROP_NID;

                        PAIS_DOCUMENTO2.Value = pais;
                        TIPO_DOCUMENTO2.Value = tipoDocumento;
                        DOCUMENTO2.Value = documento;
                        contadorUP++;
                    }
                    else if (tipoDocumento == "PAS")
                    {
                        UD_PROP_NID = "<COLL_PERS_NID>\n" +
                                    "   <KEYPROP_COUNTRY>" + pais + "</KEYPROP_COUNTRY> \n" +
                                    "   <KEYPROP_NATIONAL_ID_TYPE>" + tipoDocumento + "</KEYPROP_NATIONAL_ID_TYPE> \n" +
                                    "   <PROP_PRIMARY_NID>" + DOCUMENTO2_PRINCIPAL.Value + "</PROP_PRIMARY_NID>\n " +
                                    "   <PROP_TAX_REF_ID_SGP>N</PROP_TAX_REF_ID_SGP>\n " +
                                    "   <PROP_NATIONAL_ID>" + documento + "</PROP_NATIONAL_ID>\n " +
                                    "</COLL_PERS_NID>\n " + UD_PROP_NID;

                        PAIS_DOCUMENTO2.Value = pais;
                        TIPO_DOCUMENTO2.Value = tipoDocumento;
                        DOCUMENTO2.Value = documento;
                        contadorUD++;
                    }
                }
            }

            return (UP_PROP_NID, UD_PROP_NID);
        }
        protected (string seleccionadosCRM, string seleccionadosCampus) DatosAlergias()
        {
            List<string> selectedValuesCRM = new List<string>();
            List<string> selectedValuesCampus = new List<string>();
            string CRMseleccionados = null;
            string Campusseleccionados = null;

            // Recorrer los items del DropDownList y agregar los seleccionados a la lista
            foreach (ListItem item in CmbAlergias.Items)
            {
                if (item.Selected)
                {
                    selectedValuesCRM.Add(item.Text);
                    selectedValuesCampus.Add(item.Value);
                }
            }
            CRMseleccionados = string.Join(",", selectedValuesCRM);
            Campusseleccionados = string.Join(",", selectedValuesCampus);
            return (CRMseleccionados, Campusseleccionados);
        }
        protected (string seleccionadosCRM, string seleccionadosCampus) DatosEnfermedades()
        {
            List<string> selectedValuesCRM = new List<string>();
            List<string> selectedValuesCampus = new List<string>();
            string CRMseleccionados = null;
            string Campusseleccionados = null;

            // Recorrer los items del DropDownList y agregar los seleccionados a la lista
            foreach (ListItem item in CmbAntecedentes.Items)
            {
                if (item.Selected)
                {
                    selectedValuesCRM.Add(item.Text);
                    selectedValuesCampus.Add(item.Value);
                }
            }
            CRMseleccionados = string.Join(",", selectedValuesCRM);
            Campusseleccionados = string.Join(",", selectedValuesCampus);
            return (CRMseleccionados, Campusseleccionados);
        }
        protected string AlmacenarAlergiasCampus(string datos)
        {
            string[] valores = datos.Split(',');
            string[] valoresAnteriores = seleccionadosInicialAlergia.Value.Split(',');
            string[] valoresOtros = TxtOtrasAlergias.Text.Split(',');
            string[] valoresOtrosAnteriores = seleccionadosInicialOtrosAlergia.Value.Split(',');
            string constr = TxtURL.Text;
            string Errores = null;
            int control = 0;

            // Encontrar diferencias entre valores y valoresAnteriores
            var nuevasAlergias = valores.Except(valoresAnteriores).ToArray();
            var alergiasEliminadas = valoresAnteriores.Except(valores).ToArray();

            // Encontrar diferencias entre valoresOtros y valoresOtrosAnteriores
            var nuevasOtrasAlergias = valoresOtros.Except(valoresOtrosAnteriores).ToArray();
            var otrasAlergiasEliminadas = valoresOtrosAnteriores.Except(valoresOtros).ToArray();

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                foreach (string valor in nuevasAlergias)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;

                        if (valor != "Otra")
                        {
                            try
                            {
                                if (!seleccionadosInicialAlergia.Value.Contains(valor))
                                {
                                    cmd.CommandText = "INSERT INTO SYSADM.PS_UNIS_RG_ALERGIA (EMPLID,ALERGIAS, OTRA_ALERGIA) VALUES ('" + txtCarne.Text + "', '" + valor + "', ' ')";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception x)
                            {
                                control++;
                                Errores = "||" + x.Message;
                            }
                        }
                        else
                        {
                            foreach (string Otrovalor in nuevasOtrasAlergias)
                            {
                                try
                                {
                                    if (!seleccionadosInicialOtrosAlergia.Value.Contains(Otrovalor))
                                    {
                                        cmd.CommandText = "INSERT INTO SYSADM.PS_UNIS_RG_ALERGIA (EMPLID,ALERGIAS,OTRA_ALERGIA) VALUES ('" + txtCarne.Text + "', '" + valor + "','" + Otrovalor + "')";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception x)
                                {
                                    control++;
                                    Errores = "||" + x.Message;
                                }

                            }
                        }
                    }
                }

                foreach (string valor in alergiasEliminadas)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;

                        try
                        {
                            cmd.CommandText = "DELETE SYSADM.PS_UNIS_RG_ALERGIA WHERE EMPLID = '" + txtCarne.Text + "' AND ALERGIAS = '" + valor + "'";
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception x)
                        {
                            control++;
                            Errores = "||" + x.Message;
                        }
                    }
                }

                foreach (string Otrovalor in otrasAlergiasEliminadas)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;

                        if (!alergiasEliminadas.Contains("Otra"))
                        {
                            try
                            {
                                cmd.CommandText = "DELETE SYSADM.PS_UNIS_RG_ALERGIA WHERE EMPLID = '" + txtCarne.Text + "' AND  OTRA_ALERGIA = '" + Otrovalor + "'";
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception x)
                            {
                                control++;
                                Errores = "||" + x.Message;
                            }
                        }
                    }
                }

                if (control == 0)
                {
                    transaction.Commit();
                    log("Función AlmacenarAlergiasCampus", "Correcto", "Las alergias fueron almacenadas de forma correcta", "AlmacenarAlergiasCampus");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                }
                else
                {
                    transaction.Rollback();
                    log("Función AlmacenarAlergiasCampus", "Error", Errores, "AlmacenarAlergiasCampus");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }
                con.Close();

            }
            return control.ToString();
        }
        protected string AlmacenarAntecedentesCampus(string datos)
        {
            string[] valores = datos.Split(',');
            string[] valoresAnteriores = seleccionadosInicialAntecedentes.Value.Split(',');
            string[] valoresOtros = TxtOtrosAntecedentesM.Text.Split(',');
            string[] valoresOtrosAnteriores = seleccionadosInicialOtrosAntecedentes.Value.Split(',');
            string constr = TxtURL.Text;
            int control = 0;
            string Errores = null;

            // Encontrar diferencias entre valores y valoresAnteriores
            var nuevasEnfermedades = valores.Except(valoresAnteriores).ToArray();
            var enfermedadesEliminadas = valoresAnteriores.Except(valores).ToArray();

            // Encontrar diferencias entre valoresOtros y valoresOtrosAnteriores
            var nuevasOtrasEnfermedades = valoresOtros.Except(valoresOtrosAnteriores).ToArray();
            var otrasEnfermedadesEliminadas = valoresOtrosAnteriores.Except(valoresOtros).ToArray();


            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                foreach (string valor in nuevasEnfermedades)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;
                        if (valor != "Otra")
                        {
                            try
                            {
                                if (!seleccionadosInicialAntecedentes.ToString().Contains(valor))
                                {
                                    cmd.CommandText = "INSERT INTO SYSADM.PS_UNIS_RG_ANT_MED (EMPLID,ANTECEDENTES_MED, OTRO_ANTECEDENTE) VALUES ('" + txtCarne.Text + "', '" + valor + "', ' ')";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception x)
                            {
                                control++;
                                Errores = "||" + x.Message;
                            }
                        }
                        else
                        {
                            foreach (string Otrovalor in nuevasOtrasEnfermedades)
                            {
                                try
                                {
                                    if (!seleccionadosInicialAntecedentes.ToString().Contains(valor))
                                    {
                                        cmd.CommandText = "INSERT INTO SYSADM.PS_UNIS_RG_ANT_MED (EMPLID,ANTECEDENTES_MED, OTRO_ANTECEDENTE) VALUES ('" + txtCarne.Text + "', '" + valor + "','" + Otrovalor + "')";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception x)
                                {
                                    control++;
                                    Errores = "||" + x.Message;
                                }
                            }
                        }

                    }
                }

                foreach (string valor in enfermedadesEliminadas)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;

                        try
                        {
                            cmd.CommandText = "DELETE SYSADM.PS_UNIS_RG_ANT_MED WHERE EMPLID = '" + txtCarne.Text + "' AND ANTECEDENTES_MED = '" + valor + "'";
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception x)
                        {
                            control++;
                            Errores = "||" + x.Message;
                        }
                    }
                }

                foreach (string Otrovalor in otrasEnfermedadesEliminadas)
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.Connection = con;

                        if (!enfermedadesEliminadas.Contains("Otra"))
                        {
                            try
                            {
                                cmd.CommandText = "DELETE SYSADM.PS_UNIS_RG_ANT_MED WHERE EMPLID = '" + txtCarne.Text + "' AND  OTRO_ANTECEDENTE = '" + Otrovalor + "'";
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception x)
                            {
                                control++;
                                Errores = "||" + x.Message;
                            }
                        }
                    }
                }

                if (control == 0)
                {
                    transaction.Commit();
                    log("Función AlmacenarAntecedentesCampus", "Correcto", "Los antecedentes fueron almacenados de forma correcta", "AlmacenarAntecedentesCampus");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                }
                else
                {
                    transaction.Rollback();
                    log("Función AlmacenarAntecedentesCampus", "Error", Errores, "AlmacenarAntecedentesCampus");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }
                con.Close();

            }
            return control.ToString();
        }
        protected string AlmacenarEmergencias()
        {
            string constr = TxtURL.Text;
            //EmplidAtencion.Value = null;
            string registro = null;
            int control = 0;

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    // Concatenar valores de ALERGIAS
                    cmd.CommandText = "SELECT COUNT(EMPLID) AS REGISTROS " +
                                      "FROM SYSADM.PS_UNIS_ATEN_EMERG " +
                                      "WHERE EMPLID = :emplid";
                    cmd.Parameters.Add(new OracleParameter("emplid", txtCarne.Text));
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        registro = reader["REGISTROS"].ToString().Trim();
                    }

                    string afiliacion = string.IsNullOrEmpty(TxtAfiliacion.Text) ? " " : TxtAfiliacion.Text;
                    string seguro = string.IsNullOrEmpty(TxtSeguro.Text) ? " " : TxtSeguro.Text;
                    string carro = string.IsNullOrEmpty(TxtCarro.Text) ? " " : TxtCarro.Text;
                    string otroHospital = string.IsNullOrEmpty(TxtOtroHospital.Text) ? " " : TxtOtroHospital.Text;

                    if (registro == "0")
                    {
                        try
                        {
                            cmd.CommandText = "INSERT INTO SYSADM.PS_UNIS_ATEN_EMERG (EMPLID, NRO_AFILIACION, SEGURO_MEDICO, TIPO_SANGRE, CARRO_CAMPUS, ANTECEDENTES_MED, HOSPITAL_TRASLADO, OTRO_HOSPITAL) " +
                                                "VALUES ('" + txtCarne.Text + "', '" + afiliacion + "', ' " + seguro + "','" + CmbSangre.SelectedItem + "', '" + carro + "', ' ', '" + CmbHospital.SelectedItem + "', '"+otroHospital+"')";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            control++;
                        }
                    }
                    else
                    {
                        try
                        {
                            cmd.CommandText = "UPDATE SYSADM.PS_UNIS_ATEN_EMERG SET " +
                                                                "NRO_AFILIACION= '" + afiliacion + "', " +
                                                                "SEGURO_MEDICO= '" + seguro + "', " +
                                                                "TIPO_SANGRE= '" + CmbSangre.SelectedItem + "', " +
                                                                "CARRO_CAMPUS ='" + carro + "', " +
                                                                "OTRO_HOSPITAL ='" + otroHospital + "', " +
                                                                "HOSPITAL_TRASLADO ='" + CmbHospital.SelectedItem + "' " +
                                                                "WHERE EMPLID= '" + txtCarne.Text + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            log("Función AlmacenarEmergencias", "Correcto", "Los datos fueron almacenados correctamente", "AlmacenarEmergencias");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                        }
                        catch (Exception x)
                        {
                            transaction.Rollback();
                            control++; log("Función AlmacenarEmergencias", "Error", x.Message, "AlmacenarEmergencias");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                        }
                    }
                }
            }

            return control.ToString();
        }
        protected string AlmacenamientoApex()
        {
            string constr = TxtURL.Text;
            //EmplidAtencion.Value = null;
            string Estadoc = EstadoCivil();
            int control = 0;
            string query = "INSERT INTO UNIS_INTERFACES.TBL_ACTUALIZACION_ALUMNOS (" +
                                                        " CARNET, " +
                                                        " FACULTAD, " +
                                                        " CARRERA, " +
                                                        " CORREO_INSTITUCIONAL, " +
                                                        " FECHA_NACIMIENTO, " +
                                                        " LUGAR_NACIMIENTO, " +
                                                        " PAIS_NACIMIENTO, " +
                                                        " DEPTO_NACIMIENTO, " +
                                                        " MUNCIP_NACIMIENTO, " +
                                                        " STATE_NACIMIENTO, " +
                                                        " NOMBRES, " +
                                                        " APELLIDOS, " +
                                                        " APELLIDO_CASADA, " +
                                                        " DIRECCION1, " +
                                                        " DIRECCION2, " +
                                                        " ZONA, " +
                                                        " PAIS, " +
                                                        " DEPARTAMENTO, " +
                                                        " MUNICIPIO, " +
                                                        " STATE, " +
                                                        " TELEFONO, " +
                                                        " CORREO_PERSONAL, " +
                                                        " ESTADO_CIVIL, " +
                                                        " NIT, " +
                                                        " NOMBRE1_NIT, " +
                                                        " NOMBRE2_NIT, " +
                                                        " NOMBRE3_NIT, " +
                                                        " DIRECCION1_NIT, " +
                                                        " DIRECCION2_NIT, " +
                                                        " DIRECCION3_NIT, " +
                                                        " PAIS_NIT, " +
                                                        " DEPTO_NIT, " +
                                                        " MUNCIP_NIT, " +
                                                        " STATE_NIT, " +
                                                        " DOCUMENTO1_PRINCIPAL, " +
                                                        " PAIS_DOCUMENTO1, " +
                                                        " TIPO_DOCUMENTO1, " +
                                                        " DOCUMENTO1, " +
                                                        " DOCUMENTO2_PRINCIPAL, " +
                                                        " PAIS_DOCUMENTO2, " +
                                                        " TIPO_DOCUMENTO2, " +
                                                        " DOCUMENTO2, " +
                                                        " SEGURO_MEDIGO, " +
                                                        " NRO_AFILIACION, " +
                                                        " TIPO_SANGRE, " +
                                                        " HOSPITAL_TRASLADO, " +
                                                        " OTRO_HOSPITAL, " +
                                                        " ANTECEDENTES, " +
                                                        " OTROS_ANTECEDENTES, " +
                                                        " ALERGIAS, " +
                                                        " OTRAS_ALERGIAS, " +
                                                        " CONTACTO1_PRINCIPAL, " +
                                                        " PARENTESCO_CONTACTO1, " +
                                                        " NOMBRE_CONTACTO1, " +
                                                        " TELEFONO_CONTACTO1, " +
                                                        " CONTACTO2_PRINCIPAL, " +
                                                        " PARENTESCO_CONTACTO2, " +
                                                        " NOMBRE_CONTACTO2, " +
                                                        " TELEFONO_CONTACTO2, " +
                                                        " TALLA_SUDADERO, " +
                                                        " DATOS_CARRO, " +
                                                        " FECHA_REGISTRO, " +
                                                        " USUARIO_MODIFICO) " +
                                                        "VALUES( " +
                                                        "'" + txtCarne.Text + "' , " +
                                                        "'" + txtFacultad.Text + "' , " +
                                                        "'" + txtCarrera.Text + "' , " +
                                                        "'" + EmailUnis.Text + "' , " +
                                                        "'" + Convert.ToDateTime(txtCumple.Text).ToString("dd/MM/yyy") + "' , " +
                                                        "'" + TxtLugarNac.Text + "' , " +
                                                        "'" + CmbPaisNacimiento.SelectedValue + "' , " +
                                                        "'" + CmbDeptoNacimiento.SelectedValue + "' , " +
                                                        "'" + CmbMuncNacimiento.SelectedValue + "' , " +
                                                        "'" + StateNacimiento.Value + "' , " +
                                                        "'" + txtNombre.Text + "' , " +
                                                        "'" + txtApellido.Text + "' , " +
                                                        "'" + txtCasada.Text + "' , " +
                                                        "'" + txtDireccion.Text + "' , " +
                                                        "'" + txtDireccion2.Text + "' , " +
                                                        "'" + txtDireccion3.Text + "' , " +
                                                        "'" + CmbPais.SelectedValue + "' , " +
                                                        "'" + CmbDepartamento.SelectedValue + "' , " +
                                                        "'" + CmbMunicipio.SelectedValue + "' , " +
                                                        "'" + State.Text + "' , " +
                                                        "'" + txtTelefono.Text + "' , " +
                                                        "'" + TxtCorreoPersonal.Text + "' , " +
                                                        "'" + Estadoc + "' , " +
                                                        "'" + txtNit.Text + "' , " +
                                                        "'" + TxtNombreR.Text + "' , " +
                                                        "'" + TxtApellidoR.Text + "' , " +
                                                        "'" + TxtCasadaR.Text + "' , " +
                                                        "'" + TxtDiRe1.Text + "' , " +
                                                        "'" + TxtDiRe2.Text + "' , " +
                                                        "'" + TxtDiRe3.Text + "' , " +
                                                        "'" + CmbPaisNIT.SelectedValue + "' , " +
                                                        "'" + CmbDepartamentoNIT.SelectedValue + "' , " +
                                                        "'" + CmbMunicipioNIT.SelectedValue + "' , " +
                                                        "'" + StateNIT.Text + "' , " +
                                                        "'" + DOCUMENTO1_PRINCIPAL.Value + "' , " +
                                                        "'" + PAIS_DOCUMENTO1.Value + "' , " +
                                                        "'" + TIPO_DOCUMENTO1.Value + "' , " +
                                                        "'" + DOCUMENTO1.Value + "' , " +
                                                        "'" + DOCUMENTO2_PRINCIPAL.Value + "' , " +
                                                        "'" + PAIS_DOCUMENTO2.Value + "' , " +
                                                        "'" + TIPO_DOCUMENTO2.Value + "' , " +
                                                        "'" + DOCUMENTO2.Value + "' , " +
                                                        "'" + TxtSeguro.Text + "' , " +
                                                        "'" + TxtAfiliacion.Text + "' , " +
                                                        "'" + CmbSangre.SelectedItem + "' , " +
                                                        "'" + CmbHospital.SelectedItem + "' , " +
                                                        "'" + TxtOtroHospital.Text + "' , " +
                                                        "'" + seleccionadosAntecedentes_CRM.Value + "' , " +
                                                        "'" + TxtOtrosAntecedentesM.Text + "' , " +
                                                        "'" + seleccionadosAlergia_CRM.Value + "' , " +
                                                        "'" + TxtOtrasAlergias.Text + "' , " +
                                                        "'" + CE_Principal1.Value + "' , " +
                                                        "'" + CE_parentesco1.Value + "' , " +
                                                        "'" + CE_nombre1.Value + "' , " +
                                                        "'" + CE_telefono1.Value + "' , " +
                                                        "'" + CE_Principal2.Value + "' , " +
                                                        "'" + CE_parentesco2.Value + "' , " +
                                                        "'" + CE_nombre2.Value + "' , " +
                                                        "'" + CE_telefono2.Value + "' , " +
                                                        "'" + CmbTalla.SelectedItem + "' , " +
                                                        "'" + TxtCarro.Text + "' , " +
                                                        "SYSDATE , " +
                                                        "'" + TextUser.Text + "'" +
                                                        ") ";

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    try
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        log("Función AlmacenamientoApex", "Correcto", "La informacioón fue almacenada de forma correcta", "AlmacenamientoApex");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                    }
                    catch (Exception X)
                    {
                        transaction.Rollback();
                        log("Función AlmacenamientoApex", "Error", X.Message, "AlmacenamientoApex");
                        control++;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                    }

                }
            }

            return control.ToString();
        }
        private static void CuerpoConsultaUD(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRI, string COLL_NAMES_PRF, string COLL_NAMES_NIT, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE, string COLL_EMAIL_ADDRESSES, string PROP_BIRTHCOUNTRY, string PROP_BIRTHPLACE, string PROP_BIRTHDATE, string PROP_BIRTHSTATE, string PROP_NID, string VersionUD)
        {
            //Crea el cuerpo que se utiliza para hacer PATCH en CAMPUS
            Variables.soapBody = @"<?xml version=""1.0""?>
                                 <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:m64=""http://xmlns.oracle.com/Enterprise/Tools/schemas/" + VersionUD + @""">
                                    <soapenv:Header xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                                    <wsse:Security soap:mustUnderstand=""1"" xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                                      <wsse:UsernameToken wsu:Id=""UsernameToken-1"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                                        <wsse:Username>" + Usuario + @"</wsse:Username>
                                        <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">" + Pass + @"</wsse:Password>
                                      </wsse:UsernameToken>
                                    </wsse:Security>
                                  </soapenv:Header>
                                   <soapenv:Body>
                                      <Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                         <KEYPROP_EMPLID>" + EMPLID + @"</KEYPROP_EMPLID>
                                         " + PROP_BIRTHCOUNTRY + @"
                                         " + PROP_BIRTHPLACE + @"
                                         " + PROP_BIRTHDATE + @"
                                         " + PROP_BIRTHSTATE + @"
                                         " + PROP_NID + @"
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES_PRF + @"
                                         " + COLL_NAMES_PRI + @"
                                         " + COLL_NAMES_NIT + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                      </Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUP(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRF, string COLL_NAMES_PRI, string COLL_NAMES_NIT, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE, string COLL_EMAIL_ADDRESSES, string PROP_BIRTHCOUNTRY, string PROP_BIRTHPLACE, string PROP_BIRTHDATE, string PROP_BIRTHSTATE, string PROP_NID, string VersionUP)
        {
            //Crea el cuerpo que se utiliza para hacer POST en CAMPUS
            Variables.soapBody = @"<?xml version=""1.0""?>
                                 <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:m64=""http://xmlns.oracle.com/Enterprise/Tools/schemas/" + VersionUP + @""">
                                    <soapenv:Header xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                                    <wsse:Security soap:mustUnderstand=""1"" xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                                      <wsse:UsernameToken wsu:Id=""UsernameToken-1"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                                        <wsse:Username>" + Usuario + @"</wsse:Username>
                                        <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">" + Pass + @"</wsse:Password>
                                      </wsse:UsernameToken>
                                    </wsse:Security>
                                  </soapenv:Header>
                                   <soapenv:Body>
                                      <Update__CompIntfc__CI_PERSONAL_DATA>
                                         <KEYPROP_EMPLID>" + EMPLID + @"</KEYPROP_EMPLID>
                                         " + PROP_BIRTHCOUNTRY + @"
                                         " + PROP_BIRTHPLACE + @"
                                         " + PROP_BIRTHDATE + @"
                                         " + PROP_BIRTHSTATE + @"
                                         " + PROP_NID + @"
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES_PRF + @"
                                         " + COLL_NAMES_PRI + @"
                                         " + COLL_NAMES_NIT + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                      </Update__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private void log(string PROCESO, string ESTADO, string DESCRIPCION, string UBICACION)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    try
                    {
                        cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_UPDATE_ALUMNO (PROCESO, ESTADO, DESCRIPCION_ESTADO, UBICACION_ERROR, FECHA_EJECUCION, EMPLID_ALUMNO, ID_USUARIO) VALUES ('" + PROCESO + "','" + ESTADO + "','" + DESCRIPCION + "','" + UBICACION + "',SYSDATE,'" + txtCarne.Text + "','" + TextUser.Text + "')";
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }

                }
            }
        }
        private (string Departamento, string Municipio, string País) DatosResidencia()
        {
            string constr = TxtURL.Text;
            string depto = null;
            string mun = null;
            string pais = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT GEOGRAPHY_NAME_CRM " +
                        "FROM UNIS_INTERFACES.OPT_CONTACT_CATALOG_DEPT " +
                        "WHERE GEOGRAPHY_NAME_CRM ='" + CmbDepartamento.SelectedValue + "'  ";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        depto = reader["GEOGRAPHY_NAME_CRM"].ToString();
                    }

                    cmd.CommandText = "SELECT GEOGRAPHY_NAME_CRM " +
                        "FROM UNIS_INTERFACES.OPT_CONTACT_CATALOG_MUNICP " +
                        "WHERE STATE_CAMPUS ='" + State.Text + "'  ";
                    cmd.Connection = con;
                    OracleDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        mun = reader2["GEOGRAPHY_NAME_CRM"].ToString();
                    }


                    cmd.CommandText = "SELECT GEOGRAPHY_CODE_CRM " +
                        "FROM UNIS_INTERFACES.OPT_CONTACT_CATALOG_PAIS " +
                        "WHERE DESCRIPCION_CAMPUS ='" + CmbPais.Text + "'  ";
                    cmd.Connection = con;
                    OracleDataReader reader3 = cmd.ExecuteReader();
                    while (reader3.Read())
                    {
                        pais = reader3["GEOGRAPHY_CODE_CRM"].ToString();
                    }

                }
            }

            return (depto, mun, pais);
        }

        //EVENTOS       
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoState();
            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbMunicipioNac_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbMunicipioNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoStateNIT();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoMunicipio();
            LlenadoState();
            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbDepartamentoNac_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoMunicipioNacimiento();
            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbDepartamentoNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoMunicipioNacimiento();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            string constr = TxtURL.Text;
            string PartyNumber = null;
            string getInfo = null;
            string body = null;
            var Residencia = DatosResidencia();
            string DeptoResidencia = Residencia.Departamento;
            string MunicResidencia = Residencia.Municipio;
            string PaisResidencia = Residencia.País;
            string parentesco1 = null;
            string nombre1 = null;
            string telefono1 = null;
            string parentesco2 = null;
            string nombre2 = null;
            string telefono2 = null;
            string pais1 = null;
            string nroDocumento1 = null;
            string pais2 = null;
            string nroDocumento2 = null;
            string DocumentoCRM = null;
            string TipoDocumentoCRM = null;
            bool isPrincipalD1 = false;
            string PrincipalD1 = null;
            bool isPrincipalD2 = false;
            string PrincipalD2 = null;
            bool isPrincipalC1 = false;
            string PrincipalC1 = null;
            bool isPrincipalC2 = false;
            string PrincipalC2 = null;
            string EstadoCivilCRM = null;
            string resultados = null;

            if (GridViewDocumentos.Rows.Count >= 2)
            {
                // Obtener la primera fila
                GridViewRow row1 = GridViewDocumentos.Rows[0];
                RadioButton rdbPrincipal1 = (RadioButton)row1.FindControl("RBDocPrincipal");
                DropDownList ddlPais1 = (DropDownList)row1.FindControl("DDLPais");
                TextBox txtNroDocumento1 = (TextBox)row1.FindControl("TxtNroDocumento");

                // Variables para la primera fila
                isPrincipalD1 = rdbPrincipal1.Checked;
                pais1 = ddlPais1.SelectedValue;
                nroDocumento1 = txtNroDocumento1.Text;

                // Obtener la segunda fila
                GridViewRow row2 = GridViewDocumentos.Rows[1];
                RadioButton rdbPrincipal2 = (RadioButton)row2.FindControl("RBDocPrincipal");
                DropDownList ddlPais2 = (DropDownList)row2.FindControl("DDLPais");
                TextBox txtNroDocumento2 = (TextBox)row2.FindControl("TxtNroDocumento");

                // Variables para la segunda fila
                isPrincipalD2 = rdbPrincipal2.Checked;
                pais2 = ddlPais2.SelectedValue;
                nroDocumento2 = txtNroDocumento2.Text;

            }

            if (GridViewContactos.Rows.Count >= 2)
            {
                // Obtener la primera fila
                GridViewRow row1 = GridViewContactos.Rows[0];
                RadioButton rdbPrincipal1 = (RadioButton)row1.FindControl("RBContPrincipal");
                DropDownList ddlParentesco1 = (DropDownList)row1.FindControl("CmbPatentesco");
                TextBox txtNombre1 = (TextBox)row1.FindControl("TxtNombreE");
                TextBox txtTelefono1 = (TextBox)row1.FindControl("TxtTelefonoE");

                // Variables para la primera fila
                isPrincipalC1 = rdbPrincipal1.Checked;
                if (!String.IsNullOrEmpty(ddlParentesco1.SelectedValue))
                {
                    parentesco1 = "\"" + ddlParentesco1.SelectedItem.Text + "\"";
                }
                nombre1 = txtNombre1.Text;
                telefono1 = txtTelefono1.Text;

                // Obtener la segunda fila
                GridViewRow row2 = GridViewContactos.Rows[1];
                RadioButton rdbPrincipal2 = (RadioButton)row2.FindControl("RBContPrincipal");
                DropDownList ddlParentesco2 = (DropDownList)row2.FindControl("CmbPatentesco");
                TextBox txtNombre2 = (TextBox)row2.FindControl("TxtNombreE");
                TextBox txtTelefono2 = (TextBox)row2.FindControl("TxtTelefonoE");

                // Variables para la segunda fila
                isPrincipalC2 = rdbPrincipal2.Checked;
                if (!String.IsNullOrEmpty(ddlParentesco2.SelectedValue))
                {
                    parentesco2 = "\"" + ddlParentesco2.SelectedItem.Text + "\"";
                }
                else
                {
                    parentesco2 = "null";
                }
                nombre2 = txtNombre2.Text;
                telefono2 = txtTelefono2.Text;
            }



            if (String.IsNullOrEmpty(txtNit.Text) || txtNit.Text == "")
                txtNit.Text = "CF";

            if (!String.IsNullOrEmpty(nroDocumento1.Trim()))
            {
                DocumentoCRM = nroDocumento1;
                TipoDocumentoCRM = "CUI";
            }
            else
            {
                DocumentoCRM = nroDocumento2;
                TipoDocumentoCRM = "PAS";
            }

            if (isPrincipalC1)
            {
                PrincipalC1 = "Y";
                PrincipalC2 = "N";
            }
            else
            {
                PrincipalC1 = "N";
                PrincipalC2 = "Y";
            }

            if (isPrincipalD1)
            {
                PrincipalD1 = "Y";
                PrincipalD2 = "N";
            }
            else
            {
                PrincipalD1 = "N";
                PrincipalD2 = "Y";
            }

            CE_nombre1.Value = string.IsNullOrEmpty(nombre1) ? " " : nombre1;
            CE_nombre2.Value = string.IsNullOrEmpty(nombre2) ? " " : nombre2;
            CE_nroDocumento1.Value = string.IsNullOrEmpty(nroDocumento1) ? " " : nroDocumento1;
            CE_nroDocumento2.Value = string.IsNullOrEmpty(nroDocumento2) ? " " : nroDocumento2;
            CE_pais1.Value = string.IsNullOrEmpty(pais1) ? " " : pais1;
            CE_pais2.Value = string.IsNullOrEmpty(pais2) ? " " : pais2;
            CE_parentesco1.Value = string.IsNullOrEmpty(parentesco1) ? " " : parentesco1.Replace("\"", "");
            CE_parentesco2.Value = string.IsNullOrEmpty(parentesco2) ? " " : parentesco2.Replace("\"", "");
            CE_telefono1.Value = string.IsNullOrEmpty(telefono1) ? " " : telefono1;
            CE_telefono2.Value = string.IsNullOrEmpty(telefono2) ? " " : telefono2;
            CE_Principal1.Value = string.IsNullOrEmpty(PrincipalC1) ? " " : PrincipalC1;
            CE_Principal2.Value = string.IsNullOrEmpty(PrincipalC2) ? " " : PrincipalC2;

            if (CmbEstado.SelectedValue.Substring(0, 1).ToString().Equals("C"))
            {
                EstadoCivilCRM = "M";
            }
            else
            {
                EstadoCivilCRM = "T";
            }


            if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value))
            {
                PaisNit.Text = CmbPais.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
            }

            var respuestaDocumentos = RecorrerDocumentos();
            UP_IDENTIFICACION.Value = respuestaDocumentos.UP_Doc;
            UD_IDENTIFICACION.Value = respuestaDocumentos.UD_Doc;
            resultados = IngresoDatosGenerales();

            if (resultados == "0")
            {
                string texto;
                seleccionadosAlergia_Campus.Value = DatosAlergias().seleccionadosCampus;
                seleccionadosAlergia_CRM.Value = DatosAlergias().seleccionadosCRM;
                seleccionadosAntecedentes_CRM.Value = DatosEnfermedades().seleccionadosCRM;
                seleccionadosAntecedentes_Campus.Value = DatosEnfermedades().seleccionadosCampus;
                texto = seleccionadosAlergia_CRM.Value.Trim();
                if (texto.EndsWith(","))
                {
                    seleccionadosAlergia_CRM.Value = texto.Substring(0, texto.Length - 1);
                    seleccionadosAlergia_Campus.Value = texto.Substring(0, texto.Length - 1);
                }
                texto = seleccionadosAntecedentes_CRM.Value.Trim();

                if (texto.EndsWith(","))
                {
                    seleccionadosAntecedentes_CRM.Value = texto.Substring(0, texto.Length - 1);
                    seleccionadosAntecedentes_Campus.Value = texto.Substring(0, texto.Length - 1);
                }

                if (!String.IsNullOrEmpty(seleccionadosAlergia_Campus.Value))
                    resultados = AlmacenarAlergiasCampus(seleccionadosAlergia_Campus.Value);

                if (resultados == "0")
                {
                    if (!String.IsNullOrEmpty(seleccionadosAntecedentes_Campus.Value))
                        resultados = AlmacenarAntecedentesCampus(seleccionadosAntecedentes_Campus.Value);

                    if (resultados == "0")
                    {
                        resultados = AlmacenarEmergencias();
                        if (resultados == "0")
                        {
                            resultados = AlmacenamientoApex();

                            if (resultados == "0")
                            {
                                //ACTUALIZACION EN CRM
                                LimpiarVariables();
                                if (!String.IsNullOrEmpty(DOCUMENTO1_INICIAL.Value))
                                    getInfo = ConsultaGet(DOCUMENTO1_INICIAL.Value);
                                else
                                    getInfo = ConsultaGet(DOCUMENTO2_INCIAL.Value);
                                
                                PartyNumber = GetBetween(getInfo, "PartyNumber\" : \"", "\",");
                                
                                if (PartyNumber == "" && DOCUMENTO2_INCIAL.Value != "")
                                    getInfo = ConsultaGet(DOCUMENTO2_INCIAL.Value);

                                PartyNumber = GetBetween(getInfo, "PartyNumber\" : \"", "\",");
                                string FechaCumple = Convert.ToDateTime(txtCumple.Text).ToString("yyyy-MM-dd");
                                string parentesco1_crm = "-";
                                string parentesco2_crm = "-";

                                if (!String.IsNullOrEmpty(parentesco1))
                                {
                                    string SelectParentesco1 = "SELECT ID_CRM " +
                                                "FROM UNIS_INTERFACES.TBL_RELACIONES_FAMILIARES " +
                                            "WHERE PARENTESCO = '" + parentesco1.Replace("\"", "") + "'";

                                    string SelectParentesco2 = "SELECT ID_CRM " +
                                                "FROM UNIS_INTERFACES.TBL_RELACIONES_FAMILIARES " +
                                            "WHERE PARENTESCO = '" + parentesco2.Replace("\"", "") + "'";

                                    using (OracleConnection con = new OracleConnection(constr))
                                    {
                                        con.Open();
                                        OracleTransaction transaction;
                                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                        using (OracleCommand cmd = new OracleCommand())
                                        {
                                            cmd.Connection = con;

                                            cmd.CommandText = SelectParentesco1;
                                            OracleDataReader reader2 = cmd.ExecuteReader();
                                            while (reader2.Read())
                                            {
                                                parentesco1_crm = reader2["ID_CRM"].ToString();
                                            }

                                            cmd.CommandText = SelectParentesco2;
                                            reader2 = cmd.ExecuteReader();
                                            while (reader2.Read())
                                            {
                                                parentesco2_crm = reader2["ID_CRM"].ToString();
                                            }

                                            con.Close();
                                        }
                                    }
                                }

                                string talla = " ";
                                if (String.IsNullOrEmpty(CmbTalla.SelectedValue))
                                    talla = "-";
                                else
                                    talla = CmbTalla.SelectedValue;

                                string hospital = " ";
                                if (String.IsNullOrEmpty(CmbHospital.SelectedItem.Value) || CmbHospital.SelectedItem.Value == " ")
                                    hospital = "-";
                                else
                                    hospital = CmbHospital.SelectedItem.Value;

                                if (String.IsNullOrEmpty(seleccionadosAlergia_CRM.Value))
                                    seleccionadosAlergia_CRM.Value = "-";

                                if (String.IsNullOrEmpty(seleccionadosAntecedentes_CRM.Value))
                                    seleccionadosAntecedentes_CRM.Value = "-";

                                body = "{\r\n    " +
                                    "\"FirstName\": \"" + txtNombre.Text + "\",\r\n    " +
                                    "\"LastName\": \"" + txtApellido.Text + "\",\r\n    " +
                                    "\"MiddleName\": \"\",\r\n    " +
                                    "\"UniqueNameSuffix\": \"" + txtCasada.Text + "\",\r\n    " +
                                    "\"TaxpayerIdentificationNumber\": \"" + DocumentoCRM + "\",\r\n    " +
                                    "\"DateOfBirth\": \"" + FechaCumple + "\",\r\n    " +
                                    "\"MaritalStatus\": \"" + EstadoCivilCRM + "\",\r\n    " +
                                    "\"MobileNumber\": \"" + txtTelefono.Text + "\",\r\n    " +
                                    "\"EmailAddress\": \"" + TxtCorreoPersonal.Text + "\",\r\n    " +
                                    "\"AddressElementAttribute3\": \"Zona " + txtDireccion3.Text + "\",\r\n    " +
                                    "\"AddressLine1\": \"" + txtDireccion.Text + "\",\r\n    " +
                                    "\"AddressLine2\": \"" + txtDireccion2.Text.TrimEnd() + "\",\r\n    " +
                                    "\"City\": \"" + MunicResidencia + "\",\r\n    " +
                                    "\"Country\": \"" + PaisResidencia + "\",\r\n    " +
                                    "\"County\": \"" + DeptoResidencia + "\",\r\n    " +
                                    "\"PostalCode\": null,\r\n    " +
                                    "\"PersonDEO_TipoDeDocumentoDeIdentidad_c\": \"" + TipoDocumentoCRM + "\",\r\n    " +
                                    "\"PersonDEO_InformacionCarro_c\": \"" + TxtCarro.Text + "\",\r\n    " +
                                    "\"PersonDEO_TallaSudadero_c\": \"" + talla + "\",\r\n    " +
                                    "\"PersonDEO_T1_PaisDeNacimiento_c\": \"" + CmbPaisNacimiento.Text + "\",\r\n    " +
                                    "\"PersonDEO_NumeroDeIdentificacionTributaria_c\": \"" + txtNit.Text + "\",\r\n    " +
                                    "\"PersonDEO_ContactoDeEmergencia1_c\": \"" + nombre1 + "\",\r\n    " +
                                    "\"PersonDEO_ContactoDeEmergencia2_c\": \"" + nombre2 + "\",\r\n    " +
                                    "\"PersonDEO_ParentescoContactoEmergencia1_c\": \"" + parentesco1_crm + "\",\r\n    " +
                                    "\"PersonDEO_ParentescoContactoEmergencia2_c\": \"" + parentesco2_crm + "\",\r\n    " +
                                    "\"PersonDEO_TelefonoContactoEmergencia1_c\": \"" + telefono1 + "\",\r\n    " +
                                    "\"PersonDEO_TelefonoContactoEmergencia2_c\": \"" + telefono2 + "\",\r\n    " +
                                    "\"PersonDEO_HospitalTraslado_c\": \"" + hospital + "\",\r\n    " +
                                    "\"PersonDEO_OtroHospital_c\": \"" + TxtOtroHospital.Text + "\",\r\n    " +
                                    "\"PersonDEO_ListaAlergias_c\": \"" + seleccionadosAlergia_CRM.Value + "\",\r\n    " +
                                    "\"PersonDEO_Alergias_c\": \"" + TxtOtrasAlergias.Text + "\",\r\n    " +
                                    "\"PersonDEO_Enfermedades_c\": \"" + seleccionadosAntecedentes_CRM.Value + "\",\r\n    " +
                                    "\"PersonDEO_AntecedentesMedicos_c\": \"" + TxtOtrosAntecedentesM.Text + "\",\r\n    " +
                                    "\"PersonDEO_TipoDeSangre_c\": \"" + CmbSangre.SelectedValue + "\",\r\n    " +
                                    "\"PersonDEO_SeguroMedico_c\": \"" + TxtSeguro.Text + "\",\r\n    " +
                                    "\"PersonDEO_NroDeAfiliacion_c\": \"" + TxtAfiliacion.Text + "\"\r\n    " +
                                    "}";
                                // resultados = EnvioCorreo(txtNombre.Text + " " + txtApellido.Text, DOCUMENTO1_INICIAL.Value, DOCUMENTO1.Value, DOCUMENTO2_INCIAL.Value, DOCUMENTO2.Value,, DOCUMENTO1_PRINCIPAL.Value, DOCUMENTO1_PRINCIPAL_INICIAL.Value, DOCUMENTO2_PRINCIPAL.Value, DOCUMENTO2_PRINCIPAL_INICIAL.Value);

                                //Actualiza por medio del metodo PATCH
                                if (!String.IsNullOrEmpty(PartyNumber))
                                    UpdatePatch(body, PartyNumber);
                                if (respuestaPatch == 0)
                                {
                                    log("Actualización en CRM", "Correcto", "La información se actualizo correctamente", "Actualización información de contacto en CRM");
                                    //ACTUALIZACION CONTACTOS DE EMERGENCIA EN CAMPUS
                                    if (!String.IsNullOrEmpty(nombre1))
                                        resultados = ContactoEmergenciaCampus(nombre1, CE_parentesco1.Value, telefono1, PrincipalC1, nombre2, CE_parentesco2.Value, telefono2, PrincipalC2, txtNombreE1_Inicial.Value, txtNombreE2_Inicial.Value);
                                    if (resultados == "0")
                                    {

                                        if (resultados == "0")
                                        {
                                            if ((DOCUMENTO1_PRINCIPAL.Value != PrincipalD1 || DOCUMENTO1_INICIAL.Value != DOCUMENTO1.Value || DOCUMENTO2_PRINCIPAL.Value != PrincipalD2 || DOCUMENTO2_INCIAL.Value != DOCUMENTO2.Value) && !EmailUnis.Text.IsNullOrWhiteSpace())// != "" || EmailUnis.Text != " "))
                                            {
                                                resultados = EnvioCorreo(txtNombre.Text + " " + txtApellido.Text, DOCUMENTO1_INICIAL.Value, DOCUMENTO1.Value, DOCUMENTO2_INCIAL.Value, DOCUMENTO2.Value, DOCUMENTO1_PRINCIPAL.Value, DOCUMENTO1_PRINCIPAL_INICIAL.Value, DOCUMENTO2_PRINCIPAL.Value, DOCUMENTO2_PRINCIPAL_INICIAL.Value);

                                                if (resultados == "0")
                                                {
                                                    log("Actualizacion General de información", "Correcto", "El proceso finalizó correctamente", "Actualizacion General de información");
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                                                }
                                            }
                                            else
                                            {
                                                log("Actualizacion General de información", "Correcto", "El proceso finalizó correctamente", "Actualizacion General de información");
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                    }
                                }
                                else
                                {
                                    log("Actualización en CRM", "Error", respuestaMensajePatch, "Actualización información de contacto en CRM");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }
            }
        }
        protected void CmbPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoDepartamento();
            LlenadoMunicipio();
            LlenadoState();
            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }

            if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value))
            {
                PaisNit.Text = CmbPais.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
            }


            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbPaisNac_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenadoDepartamentoNac();
            LlenadoMunicipioNacimiento();
            if (RadioButtonNombreSi.Checked && ControlCF.Value != "CF")
            {
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtDireccion3.Text;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre.Text;
                TxtApellidoR.Text = txtApellido.Text;
                TxtCasadaR.Text = txtCasada.Text;
            }

            if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value))
            {
                PaisNit.Text = CmbPais.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
            }


            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbPaisNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbPaisNIT.SelectedValue != " ")
            {
                LlenadoDepartamentoNit();
                LlenadoMunicipioNIT();
                LlenadoStateNIT();
            }
        }
        protected void TxtNit_TextChanged(object sender, EventArgs e)
        {
            TxtNombreR.Text = "";
            TxtApellidoR.Text = "";
            TxtCasadaR.Text = "";
            TxtDiRe1.Text = "";
            TxtDiRe2.Text = "";
            TxtDiRe3.Text = "";
            string respuesta;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            respuesta = ConsultaNit(txtNit.Text);
            string constr = TxtURL.Text;

            if (respuesta.Equals("BadRequest") || respuesta.Equals("1"))
            {
                TxtNombreR.Text = null;
                TxtApellidoR.Text = null;
                TxtCasadaR.Text = null;
                LlenadoPaisnit();
                CmbPaisNIT.SelectedValue = " ";
                LlenadoDepartamentoNit();
                CmbDepartamentoNIT.SelectedValue = " ";
                LlenadoMunicipioNIT();
                CmbMunicipioNIT.Text = " ";
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
                        //Obtener nit pendiene
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
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }

                lblActualizacion.Text = "El NIT sera validado más adelante";
                TxtDiRe1.Text = " ";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
            }
            else if (respuesta != "1" && respuesta != "BadRequest")
            {
                string NIT = GetBetween(respuesta, "\"NIT\": \"", "\",");
                string Direccion = GetBetween(respuesta, "\"Direccion\": \"", "\",");
                string apellido1;
                string apellido2;
                string apellidoCasada;
                string nombre1;
                string nombre2;
                int largo;
                if (NIT != "CF")
                {
                    string nombreRespuesta = GetBetween(respuesta, "\"NOMBRE\": \"", "\",") + ",";
                    string cadena = nombreRespuesta;
                    TxtDiRe1.Text = Direccion;
                    TxtDiRe2.Text = "";
                    TxtDiRe3.Text = "";
                    LlenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    LlenadoDepartamentoNit();
                    CmbDepartamentoNIT.SelectedValue = " ";
                    LlenadoMunicipioNIT();
                    int contadorComas = cadena.Count(c => c == ',');
                    if (contadorComas >= 5)
                    {
                        apellido1 = GetBetween(nombreRespuesta, "", ",");
                        apellido2 = GetBetween(nombreRespuesta, apellido1 + ",", ",");
                        apellidoCasada = GetBetween(nombreRespuesta, apellido2 + ",", ",");
                        nombre1 = GetBetween(nombreRespuesta, apellido1 + "," + apellido2 + "," + apellidoCasada + ",", ",");
                        nombre2 = GetBetween(nombreRespuesta, nombre1 + ",", ",");
                        TxtNombreR.Text = textInfo.ToTitleCase(nombre1 + " " + nombre2);
                        TxtApellidoR.Text = apellido1 + " " + apellido2;
                        TxtCasadaR.Text = apellidoCasada;
                    }
                    else
                    {
                        nombreRespuesta = nombreRespuesta.TrimEnd(',');
                        largo = nombreRespuesta.Length;
                        string[] arrayDePalabras = DividirEnArray(nombreRespuesta);
                        int mitad = arrayDePalabras.Count() - (arrayDePalabras.Count() / 2);
                        int triparte1 = arrayDePalabras.Count() / 3;
                        int triparte2 = (arrayDePalabras.Count() - (arrayDePalabras.Count() / 3)) / 2;
                        int triparte3 = arrayDePalabras.Count() - (triparte1 + triparte2);
                        int contadorEmpresa = 0;

                        if (largo < 61)
                        {
                            for (int i = 0; i < mitad; i++)
                            {
                                if (TxtNombreR.Text.IsNullOrWhiteSpace())
                                {
                                    TxtNombreR.Text = arrayDePalabras[i];
                                    contadorEmpresa++;
                                }
                                else
                                {
                                    TxtNombreR.Text = TxtNombreR.Text + " " + arrayDePalabras[i];
                                    contadorEmpresa++;
                                }
                            }
                            for (int i = contadorEmpresa; i < arrayDePalabras.Count(); i++)
                            {
                                if (TxtApellidoR.Text.IsNullOrWhiteSpace())
                                {
                                    TxtApellidoR.Text = arrayDePalabras[i];
                                    contadorEmpresa++;
                                }
                                else
                                {
                                    TxtApellidoR.Text = TxtApellidoR.Text + " " + arrayDePalabras[i];
                                    contadorEmpresa++;
                                }
                            }
                        }
                        else if (largo > 60 && largo < 91)
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
                    ValidacionNit.Value = "0";
                    TxtDiRe1.Enabled = true;
                    TxtDiRe2.Enabled = true;
                    TxtDiRe3.Enabled = true;
                    txtNit.Enabled = true;
                    ValidarNIT.Enabled = true;
                }
                else
                {
                    TxtDiRe1.Enabled = true;
                    TxtDiRe2.Enabled = true;
                    TxtDiRe3.Enabled = true;
                    string nit = txtNit.Text;
                    txtNit.Text = nit;
                    TxtNombreR.Text = "";
                    TxtApellidoR.Text = "";
                    TxtCasadaR.Text = "";
                    TxtDiRe1.Text = "";
                    TxtDiRe2.Text = "";
                    TxtDiRe2.Text = "";
                    ValidarNIT.Enabled = true;
                    txtNit.Enabled = true;
                    LlenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    string script = "<script>NoExisteNit();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
            }
            TrueNit.Value = txtNit.Text;
        }
        protected void BtnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/ActualizacionGeneralEstudiantes.aspx");
        }
        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            ExisteBusqueda.Value = "0";
            if (CmbBusqueda.Text.Equals("Nombre"))
            {
                LoadData();
                ConsultaNombre(TxtBusqueda.Text);
                if (ExisteBusqueda.Value == "1")
                {
                    string script = "<script>Busqueda();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
                else
                {
                    string script = "<script>NoExiste();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }

            }

            if (CmbBusqueda.Text.Equals("Documento de Identificación"))
            {
                LoadData();
                ConsultarDocumento(TxtBusqueda.Text);
                if (ExisteBusqueda.Value == "1")
                {
                    string script = "<script>Busqueda();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
                else
                {
                    string script = "<script>NoExiste();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
            }

            if (CmbBusqueda.Text.Equals("Carnet"))
            {
                LoadData();
                ConsultarId(TxtBusqueda.Text);
                if (ExisteBusqueda.Value == "1")
                {
                    string script = "<script>Busqueda();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
                else
                {
                    string script = "<script>NoExiste();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
            }
        }
        protected void BtnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            BtnReload_Click(BtnReload, EventArgs.Empty);
        }
        protected void BtnAceptarBusqueda_Click(object sender, EventArgs e)
        {
            bool radioButtonSelected = false;
            foreach (GridViewRow row in GridViewBusqueda.Rows)
            {
                RadioButton rb = (RadioButton)row.FindControl("RBBusqueda");
                if (rb != null && rb.Checked)
                {
                    radioButtonSelected = true;
                    // Encontrar otros controles y obtener sus valores si es necesario
                    string id = row.Cells[1].Text; // Asumiendo que la columna ID es la segunda columna
                    string name = row.Cells[2].Text; // Asumiendo que la columna NAME es la tercera columna

                    txtEmplid.Value = id;
                    break; // Salir del bucle después de encontrar el elemento seleccionado
                }
            }

            if (!radioButtonSelected)
            {
                // Mostrar un mensaje indicando que no se seleccionó ningún elemento
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No se seleccionó ningún elemento.');", true);
                BtnBuscar_Click(BtnReload, EventArgs.Empty);
            }

            if (radioButtonSelected is true)
            {
                //string validarAcceso = ValidacionAccesoVista(txtEmplid.Value);
                /*if (validarAcceso != null)
                {*/

                string getInfo = null;
                BtnActualizar.Enabled = true;
                MostrarInformación(txtEmplid.Value);                
                if (txtNit.Text == "CF")
                {
                    txtNit.Enabled = false;
                    RadioButtonNombreSi.Checked = true;
                    ControlCF2.Value = "1";
                    ControlCF.Value = "CF";
                    ValidarNIT.Enabled = false;
                    if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value))
                    {
                        PaisNit.Text = CmbPais.SelectedValue;
                        DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                        MunicipioNit.Text = CmbMunicipio.SelectedValue;
                    }
                }
                else
                {
                    RadioButtonNombreNo.Checked = true;
                    ControlCF2.Value = "2";
                    TxtDiRe1.Enabled = true;
                    TxtDiRe2.Enabled = true;
                    TxtDiRe3.Enabled = true;
                    ValidarNIT.Enabled = true;
                    txtNit.Enabled = true;
                    if (txtNit.Text.IsNullOrWhiteSpace())
                    {
                        CmbPaisNIT.SelectedValue = " ";
                        //CmbDepartamentoNIT.SelectedValue = " ";
                        //CmbMunicipioNIT.SelectedValue = " ";
                    }
                }
                LlenadoDatosMedicos();
                LlenadoDatosAlergias();
                LlenadoDatosEnfermedades();
                LoadDataContactos();
                LlenadoContactosEmergencia();
                LoadDataDocumentos();
                LlenadoGridDocumentos();
                if (!String.IsNullOrEmpty(DOCUMENTO1_INICIAL.Value))
                    getInfo = ConsultaGet(DOCUMENTO1_INICIAL.Value);
                else
                    getInfo = ConsultaGet(DOCUMENTO2_INCIAL.Value);

                string PartyNumber = GetBetween(getInfo, "PartyNumber\" : \"", "\",");

                if (PartyNumber == "" && DOCUMENTO2_INCIAL.Value != "")
                    getInfo = ConsultaGet(DOCUMENTO2_INCIAL.Value);

                CmbTalla.SelectedValue = GetBetween(getInfo, "PersonDEO_TallaSudadero_c\" : \"", "\",");
                if (String.IsNullOrEmpty(txtCarne.Text))
                {
                    string script = "<script>NoExisteAlumno();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                    BtnBuscar.Enabled = true;
                    BtnLimpiarBusqueda.Enabled = false;
                    TxtBusqueda.Enabled = true;
                }
                else
                {
                    BtnBuscar.Enabled = false;
                    BtnLimpiarBusqueda.Enabled = true;
                    TxtBusqueda.Enabled = false;
                }
                /*}
                else
                {
                    string script = "<script>NoTienePermisos();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }*/
            }
        }
        protected void GridViewDocumentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlPais = (DropDownList)e.Row.FindControl("DDLPais");
                LlenarDDLPais(ddlPais);

                // Asegúrate de seleccionar el valor correcto después de llenar la lista
                string pais = DataBinder.Eval(e.Row.DataItem, "País").ToString();

                if (ddlPais.Items.FindByValue(pais) != null)
                {
                    ddlPais.SelectedValue = pais;
                }
                else
                {
                    // Agregar y seleccionar el valor si no está presente en la lista
                    ddlPais.Items.Add(new ListItem(pais, pais));
                    ddlPais.SelectedValue = pais;
                }

                TextBox txtNroDocumento = (TextBox)e.Row.FindControl("TxtNroDocumento");
                if (txtNroDocumento != null)
                {
                    // Verificar si es la primera fila
                    if (e.Row.RowIndex == 0)
                    {
                        txtNroDocumento.MaxLength = 13;
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "updatePrincipalRadioButton", "updatePrincipalRadioButton();", true);
            }
        }
        protected void GridViewContactos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList cmbParentesco = (DropDownList)e.Row.FindControl("CmbPatentesco");
                LlenarParentezco(cmbParentesco);

                // Asegúrate de seleccionar el valor correcto después de llenar la lista
                string parentesco = DataBinder.Eval(e.Row.DataItem, "Parentesco").ToString();

                if (cmbParentesco.Items.FindByValue(parentesco) != null)
                {
                    cmbParentesco.SelectedValue = parentesco;
                }
                else
                {
                    // Agregar y seleccionar el valor si no está presente en la lista
                    cmbParentesco.Items.Add(new ListItem(parentesco, parentesco));
                    cmbParentesco.SelectedValue = parentesco;
                }
            }
        }
        protected void GridViewDocumentos_DataBound(object sender, EventArgs e)
        {

        }
        //revisar para eliminar        
        protected void BtnDownload_Click(object sender, EventArgs e)
        {
            string archivoDescarga = CurrentDirectory + "/Manuales/ManualActivacionCamara.pdf";
            string nombreArchivo = "ManualActivacionCamara.pdf";
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nombreArchivo + "\"");
            Response.WriteFile(archivoDescarga);
            Response.End();
        }

        /*-------------------------------------------INICIAN FUNCIONES PARA METODO SOAP-------------------------------------------*/
        //Función para limpiar variables
        private static void LimpiarVariables()
        {
            //Cuerpo del servicio web (enviar información) 
            Variables.soapBody = "";
            Variables.strDocumentoRespuesta = "";
            //Direción del serivicio web
            Variables.wsUrl = "";
            //Usuario del servicio web
            Variables.wsUsuario = "";
            //Contraseña del servicio web
            Variables.wsPassword = "";
        }
        public class Variables
        {
            //Cuerpo del servicio web (enviar información) 
            public static string soapBody;
            public static string strDocumentoRespuesta;
            //Direción del serivicio web
            public static string wsUrl = "";
            //Usuario del servicio web
            public static string wsUsuario = "";
            //Contraseña del servicio web
            public static string wsPassword = "";
            //Acción del servicio web
            public static string wsAction = "";
        }
        //Función para obtener información de acceso al servicio de Campus
        private static void CredencialesEndPoint(string RutaConfiguracion)
        {
            int cont = 0;
            foreach (var line in File.ReadLines(RutaConfiguracion))
            {
                if (cont == 1)
                    Variables.wsUrl = line.ToString();
                if (cont == 3)
                    Variables.wsUsuario = line.ToString();
                if (cont == 5)
                    Variables.wsPassword = line.ToString();
                cont++;
            }
        }
        //Función para crear el elemento raíz para solicitud web 
        private static XmlDocument CreateSoapEnvelope(string xmlString)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(xmlString);
            return soapEnvelopeDocument;
        }
        //Función para crear el encabezado para la Solicitud web
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        //Función para crear unificar toda la estructura de la solicitud web
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        //Función para llamar un servicio web de Campus
        public string LlamarWebServiceCampus(string _url, string _action, string _xmlString)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(_xmlString);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            //Comienza la llamada asíncrona a la solicitud web.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            //Suspender este hilo hasta que se complete la llamada. Es posible que desee hacer algo útil aquí, como actualizar su interfaz de usuario.
            asyncResult.AsyncWaitHandle.WaitOne();

            //Obtener la respuesta de la solicitud web completada.
            string soapResult;
            try
            {
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                    return soapResult;
                }
            }
            catch (WebException ex)
            {
                using (var stream = new StreamReader(ex.Response.GetResponseStream()))
                {
                    soapResult = stream.ReadToEnd();
                }
                return soapResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string Respuesta(string StrCodigoRetorno, string StrMensajeRetorno)
        {
            //Inicia a crear la respuesta en formato XML.
            //Crea un nuevo docuemento para responder 
            XmlDocument xmlDocumentoRespuesta = new XmlDocument();

            //Declaración del XML
            XmlDeclaration xmlDeclaration = xmlDocumentoRespuesta.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
            XmlElement root = xmlDocumentoRespuesta.DocumentElement;
            xmlDocumentoRespuesta.InsertBefore(xmlDeclaration, root);

            //Mensaje
            XmlElement NodoMensaje = xmlDocumentoRespuesta.CreateElement(string.Empty, "mensaje", string.Empty);
            xmlDocumentoRespuesta.AppendChild(NodoMensaje);

            //Encabezado
            XmlElement NodoEncabezado = xmlDocumentoRespuesta.CreateElement(string.Empty, "encabezado", string.Empty);
            NodoMensaje.AppendChild(NodoEncabezado);

            /*Estado resultante de la transacción*/
            //Código retorno
            XmlElement NodoCodigoRetorno = xmlDocumentoRespuesta.CreateElement(string.Empty, "codigoRetorno", string.Empty);
            XmlText CodigoRetorno = xmlDocumentoRespuesta.CreateTextNode(StrCodigoRetorno);
            NodoCodigoRetorno.AppendChild(CodigoRetorno);
            NodoEncabezado.AppendChild(NodoCodigoRetorno);

            //Mensaje retorno
            XmlElement NodoMensajeRetorno = xmlDocumentoRespuesta.CreateElement(string.Empty, "mensajeRetorno", string.Empty);
            XmlText MensajeRetorno = xmlDocumentoRespuesta.CreateTextNode(StrMensajeRetorno);
            NodoMensajeRetorno.AppendChild(MensajeRetorno);
            NodoEncabezado.AppendChild(NodoMensajeRetorno);

            //Encabezado
            XmlElement NodoValor = xmlDocumentoRespuesta.CreateElement(string.Empty, "valor", string.Empty);
            NodoMensaje.AppendChild(NodoValor);

            //Se convierte el XML de respuesta en string
            using (var StringRespuestaConsultar = new StringWriter())
            using (var xmlAStringResputaConsultar = XmlWriter.Create(StringRespuestaConsultar))
            {
                xmlDocumentoRespuesta.WriteTo(xmlAStringResputaConsultar);
                xmlAStringResputaConsultar.Flush();
                return StringRespuestaConsultar.GetStringBuilder().ToString();
            }
        }

        [WebMethod]
        public string Consultar()
        {
            //Se limpian variables para guardar la nueva información
            LimpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            CredencialesEndPoint(archivoConfiguraciones);

            if (auxConsulta == 0)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UP_NAMES_PRI.Value, UP_NAMES_PRF.Value, UP_NAMES_NIT.Value, UP_PERS_DATA_EFFDT.Value, UP_ADDRESSES_NIT.Value, UP_ADDRESSES.Value, UP_PERSONAL_PHONE.Value, UP_EMAIL_ADDRESSES.Value, UP_BIRTHCOUNTRY.Value, UP_BIRTHPLACE.Value, UP_BIRTHDATE.Value, UP_BIRTHSTATE.Value, UP_IDENTIFICACION.Value, VersionUP.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UD_NAMES_PRI.Value, UD_NAMES_PRF.Value, UD_NAMES_NIT.Value, UD_PERS_DATA_EFFDT.Value, UD_ADDRESSES_NIT.Value, UD_ADDRESSES.Value, UD_PERSONAL_PHONE.Value, UD_EMAIL_ADDRESSES.Value, UD_BIRTHCOUNTRY.Value, UD_BIRTHPLACE.Value, UD_BIRTHDATE.Value, UD_BIRTHSTATE.Value, UD_IDENTIFICACION.Value, VersionUD.Value);
            }

            //Crea un documento de respuesta Campus
            System.Xml.XmlDocument xmlDocumentoRespuestaCampus = new System.Xml.XmlDocument();

            // Indica que no se mantengan los espacios y saltos de línea
            xmlDocumentoRespuestaCampus.PreserveWhitespace = false;

            try
            {
                // Carga el XML de respuesta de Campus
                xmlDocumentoRespuestaCampus.LoadXml(LlamarWebServiceCampus(Variables.wsUrl, Variables.wsAction, Variables.soapBody));
            }
            catch (WebException)
            {
                //Crea la respuesta cuando se genera una excepción web.
                Variables.strDocumentoRespuesta = Respuesta("05", "ERROR AL CONSULTAR EL REPORTE");
                return Variables.strDocumentoRespuesta;

            }
            XmlNodeList elemList = xmlDocumentoRespuestaCampus.GetElementsByTagName("notification");
            return elemList[0].InnerText.ToString();
        }


        /*-------------------PARA CONSUMO DE SERVICIOS CRM-------------------*/
        private static void CredencialesWS_CRM(string RutaConfiguracion)
        {
            //Función para obtener información de acceso al servicio de HCM
            int cont = 0;

            foreach (var line in File.ReadLines(RutaConfiguracion))
            {
                if (cont == 1)
                    Variables.wsUrl = line.ToString();
                if (cont == 2)
                    Variables.wsUsuario = line.ToString();
                if (cont == 3)
                    Variables.wsPassword = line.ToString();
                cont++;
            }
        }

        private void UpdatePatch(string info, string PartyNumber)
        {
            CredencialesWS_CRM(archivoWS);
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var respuesta = api.Patch_CRM(vchrUrlWS + "/crmRestApi/resources/11.13.18.05/contacts/" + PartyNumber, user, pass, info);
            respuestaPatch = respuesta.respuesta;
            respuestaMensajePatch = respuesta.mensaje;
        }

        private string ConsultaGet(string identificacion)
        {
            CredencialesWS_CRM(archivoWS);
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/crmRestApi/resources/11.13.18.05/contacts/?q=TaxpayerIdentificationNumber='" + identificacion + "'", user, pass);
            return respuesta;
        }

    }

}