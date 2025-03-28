﻿using System;
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

namespace ReportesUnis
{
    public partial class ActualizacionEstudiantes : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string mensaje = "";
        int controlPantalla;
        int controlRenovacion = 0;
        int controlRenovacionFecha = 0;
        string emplid;
        int auxConsulta = 0;
        int contadorUP = 0;
        int contadorUD = 0;
        string CONFIRMACION = "1000";
        ConsumoAPI api = new ConsumoAPI();
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigCampus.dat");
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();

        protected void Page_Load(object sender, EventArgs e)
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            controlCamposVisibles(true);
            LeerInfoTxt();
            LeerPathApex();
            LeerCredencialesNIT();
            LeerVersionesSOAPCampus();
            controlPantalla = PantallaHabilitada("Carnetización Masiva");
            txtExiste.Text = controlPantalla.ToString();
            if (controlPantalla >= 1)
            {
                TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");

                if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_VistaAlumnos") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
                {
                    Response.Redirect(@"~/Default.aspx");
                }
                if (!IsPostBack)
                {
                    LeerInfoTxtSQL();
                    LeerInfoTxtPath();
                    llenadoPais();
                    llenadoDepartamento();
                    llenadoState();
                    txtControlBit.Text = "0";
                    txtControlNR.Text = "0";
                    txtControlAR.Text = "0";
                    emplid = mostrarInformación();
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
                            CmbDepartamentoNIT.SelectedValue = " ";
                            CmbMunicipioNIT.SelectedValue = " ";
                        }
                    }

                    if (urlPathControl2.Value == "1")
                    {
                        AlmacenarFotografia();
                    }
                    try
                    {
                        fotoAlmacenada();
                    }
                    catch
                    {
                    }

                    if (String.IsNullOrEmpty(txtCarne.Text))
                    {
                        BtnActualizar.Visible = false;
                        lblActualizacion.Text = "El usuario utilizado no se encuentra matriculado en un ciclo lectivo vigente";
                        CmbPais.SelectedValue = "Guatemala";
                        tabla.Visible = false;
                        CargaFotografia.Visible = false;
                        InfePersonal.Visible = false;
                        divActividad.Visible = false;
                    }
                }
            }

            else
            {
                lblActualizacion.Text = "¡IMPORTANTE! Esta página no está disponible, ¡Permanece atento a nuevas fechas para actualizar tus datos!";
                controlCamposVisibles(false);
            }
        }

        //FUNCIONES
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

            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                txtPath.Text = linea1;
                file.Close();
            }
        }
        public void controlCamposVisibles(bool Condicion)
        {
            CargaFotografia.Visible = Condicion;
            tabla.Visible = Condicion;
            tbactualizar.Visible = Condicion;
            InfePersonal.Visible = Condicion;
            divActividad.Visible = Condicion;
        }
        private string mostrarInformación()
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
                    "WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' ";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        emplid = reader["EMPLID"].ToString();
                    }

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, " +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, MUNICIPIO_NIT, DEPARTAMENTO_NIT, STATE_NIT, PAIS_NIT, STATE, EMAILUNIS,EMAILPERSONAL FROM ( " +
                                        "SELECT PD.EMPLID, PN.NATIONAL_ID CARNE,  PD.FIRST_NAME, " +
                                        "PD.LAST_NAME, PD.SECOND_LAST_NAME, PN.NATIONAL_ID DPI, PN.NATIONAL_ID_TYPE, PP.PHONE , " +
                                        "TO_CHAR(PD.BIRTHDATE,'YYYY-MM-DD') BIRTHDATE, " +
                                        "APD.DESCR CARRERA, AGT.DESCR FACULTAD, " +
                                        "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'No Consta' END STATUS, " +
                                        "(SELECT EXTERNAL_SYSTEM_ID FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NIT," +
                                        "(SELECT PNA.FIRST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NOMBRE_NIT, " +
                                        "(SELECT PNA.LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) APELLIDO_NIT, " +
                                        "(SELECT SECOND_LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY PNA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) CASADA_NIT, " +
                                        "(SELECT ADDRESS1 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION1_NIT, " +
                                        "(SELECT ADDRESS2 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION2_NIT, " +
                                        "(SELECT ADDRESS3 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION3_NIT, " +
                                        "(SELECT C.DESCR FROM SYSADM.PS_ADDRESSES PA JOIN SYSADM.PS_COUNTRY_TBL C ON PA.COUNTRY = C.COUNTRY AND PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) PAIS_NIT, " +
                                        "(SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) MUNICIPIO_NIT, " +
                                        "(SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DEPARTAMENTO_NIT, " +
                                        "(SELECT ST.STATE FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) STATE_NIT, " +
                                        "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) EMAILUNIS , " +
                                        "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND EMAIL.E_ADDR_TYPE IN ('HOM1') FETCH FIRST 1 ROWS ONLY) EMAILPERSONAL , " +
                                        "A.ADDRESS1 DIRECCION, A.ADDRESS2 DIRECCION2, A.ADDRESS3 DIRECCION3, " +
                                        "REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO, ST.STATE, " +
                                        "TT.TERM_BEGIN_DT, C.DESCR PAIS " +
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
                                        "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                        "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                        "AND CT.INSTITUTION = APD.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                        "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                        "JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                        "AND CT.INSTITUTION = TT.INSTITUTION " +
                                        "AND (SYSDATE BETWEEN TT.TERM_BEGIN_DT AND TT.TERM_END_DT)" +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_PHONE PP ON PD.EMPLID = PP.EMPLID " +
                                        "AND PP.PHONE_TYPE = 'HOME' " +
                                        "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON A.COUNTRY = C.COUNTRY " +
                                        "WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                                        "ORDER BY CT.FULLY_ENRL_DT DESC " +
                                        "FETCH FIRST 1 ROWS ONLY" +
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

                        if (txtApellido.Text.Length > 4)
                        {
                            if (txtApellido.Text.Length > 6)
                            {
                                if ((txtApellido.Text.Substring(0, 6)).ToUpper().Equals("DE LA ")){
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

                        bday = reader["BIRTHDATE"].ToString();
                        anio = bday.Substring(0, 4);
                        mes = bday.Substring(5, 2);
                        dia = bday.Substring(8, 2);
                        txtCumple.Text = dia + "-" + mes + "-" + anio;
                        txtDireccion.Text = reader["DIRECCION"].ToString().Length > 54 ? reader["DIRECCION"].ToString().Substring(0, 54) : reader["DIRECCION"].ToString();
                        TrueDir.Text = reader["DIRECCION"].ToString().Length > 54 ? reader["DIRECCION"].ToString().Substring(0, 54) : reader["DIRECCION"].ToString();
                        txtDireccion2.Text = reader["DIRECCION2"].ToString();
                        txtDireccion3.Text = reader["DIRECCION3"].ToString();
                        TxtDiRe1.Text = reader["DIRECCION1_NIT"].ToString().Length > 54 ? reader["DIRECCION1_NIT"].ToString().Substring(0, 54) : reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2.Text = reader["DIRECCION2_NIT"].ToString().Length > 54 ? reader["DIRECCION2_NIT"].ToString().Substring(0, 54) : reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3.Text = reader["DIRECCION3_NIT"].ToString().Length > 54 ? reader["DIRECCION3_NIT"].ToString().Substring(0, 54) : reader["DIRECCION3_NIT"].ToString();
                        if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                        {
                            CmbPais.SelectedValue = reader["PAIS"].ToString();
                            llenadoDepartamento();
                            CmbDepartamento.SelectedValue = reader["DEPARTAMENTO"].ToString();
                            llenadoMunicipio();
                            CmbMunicipio.SelectedValue = reader["MUNICIPIO"].ToString();
                        }
                        else
                        {
                            CmbPais.SelectedValue = "";
                        }

                        if (!String.IsNullOrEmpty(reader["PAIS_NIT"].ToString()))
                        {
                            llenadoPaisnit();
                            CmbPaisNIT.SelectedValue = reader["PAIS_NIT"].ToString();
                            PaisNit.Text = reader["PAIS_NIT"].ToString();
                            llenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO_NIT"].ToString();
                            DepartamentoNit.Text = reader["DEPARTAMENTO_NIT"].ToString();
                            llenadoMunicipioNIT();
                            MunicipioNit.Text = reader["MUNICIPIO_NIT"].ToString();
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
        private void fotoAlmacenada()
        {
            if (ControlAct.Value != "AC" && ControlAct.Value != "PC" && ControlAct.Value != "RC")
            {
                validarAccion();
            }
            string constr = TxtURL.Text;
            int contador = 0;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + txtCarne.Text + "' AND CONTROL = '1'";
                    OracleDataReader reader3 = cmd.ExecuteReader();
                    while (reader3.Read())
                    {
                        contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                        if (contador > 0)
                        {
                            ImgBase.Visible = true;
                            byte[] imageBytes = null;

                            try
                            {
                                if (ControlAct.Value == "AC")
                                {
                                    ImgBase.ImageUrl = (File.Exists(Server.MapPath($"~/Usuarios/UltimasCargas/ACTUALIZACION-AC/{txtCarne.Text}.jpg"))) ? $"~/Usuarios/UltimasCargas/ACTUALIZACION-AC/{txtCarne.Text}.jpg?c={DateTime.Now.Ticks}" : string.Empty;
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/ACTUALIZACION-AC/" + txtCarne.Text + ".jpg");
                                }
                                else if (ControlAct.Value == "PC")
                                {
                                    ImgBase.ImageUrl = (File.Exists(Server.MapPath($"~/Usuarios/UltimasCargas/PRIMER_CARNET-PC/{txtCarne.Text}.jpg"))) ? $"~/Usuarios/UltimasCargas/PRIMER_CARNET-PC/{txtCarne.Text}.jpg?c={DateTime.Now.Ticks}" : string.Empty;
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/PRIMER_CARNET-PC/" + txtCarne.Text + ".jpg");
                                }
                                else if (ControlAct.Value == "RC")
                                {
                                    ImgBase.ImageUrl = (File.Exists(Server.MapPath($"~/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/{txtCarne.Text}.jpg"))) ? $"~/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/{txtCarne.Text}.jpg?c={DateTime.Now.Ticks}" : string.Empty;
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/" + txtCarne.Text + ".jpg");
                                }
                                string base64String = Convert.ToBase64String(imageBytes);
                                urlPath2.Value = base64String;
                                urlPathControl2.Value = "0";
                            }
                            catch
                            {
                            }
                        }
                    }
                    con.Close();
                }
            }
        }
        private void EliminarAlmacenada()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE SET FOTOGRAFIA = 'Existe', CONTROL = '2'" +
                                                        "WHERE CARNET = '" + txtCarne.Text + "'";
                    cmd.ExecuteNonQuery();
                    File.Delete(CurrentDirectory + "/Usuarios/UltimasCargas/CONTROL/" + txtCarne.Text + ".jpg");
                }
            }
        }
        private void EliminarRegistrosFotos()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                    cmd.ExecuteNonQuery();
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
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
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
        public void llenadoDepartamentoNit()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
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
                            if (CmbPaisNIT.Text != "Guatemala")
                            {
                                cmd.CommandText = "SELECT ' ' AS MUNICIPIO, ' ' AS STATE FROM DUAL UNION SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
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
        protected void llenadoMunicipio()
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
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
        protected void llenadoPais()
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
        public void llenadoPaisnit()
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
            string confirmacion = ValidarRegistros();
            int contador = 0;

            validarAccion();

            if (txtAInicial.Value == txtApellido.Text && txtNInicial.Value == txtNombre.Text && txtCInicial.Value.TrimEnd() == txtCasada.Text)
            {
                txtAccion.Text = "1";
                txtTipoAccion.Text = "1.1";
                txtConfirmacion.Text = "02"; //VALIDACIÓN DE FOTOGRAFÍA
                if (confirmacion != txtConfirmacion.Text && confirmacion != "0")
                {
                    string constr = TxtURL.Text;
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        con.Open();
                        OracleTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Transaction = transaction;
                            try
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception x)
                            {
                                log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                                File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                            }
                        }
                    }
                }

                if (RadioButtonNombreNo.Checked)
                {
                    if (!CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                    {
                        IngresoDatos();
                    }
                    else
                    {
                        mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                        lblActualizacion.Text = mensaje;
                    }
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
                    IngresoDatos();
                }
                else if (RadioButtonNombreSi.Checked)
                {
                    IngresoDatos();
                }
            }
            else
            {
                if (FileUpload2.HasFile)
                {
                    int txtCantidad = 0;
                    string constr = TxtURL.Text;
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
                            cmd.CommandText = "SELECT TOTALFOTOS FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + UserEmplid.Text + "'";
                            OracleDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                txtCantidad = Convert.ToInt16(reader["TOTALFOTOS"]);
                            }
                        }
                    }
                    for (int i = 1; i <= txtCantidad; i++)
                    {
                        File.Delete(CurrentDirectory + "/Usuarios/DPI/" + txtCarne.Text + "(" + i + ").jpg");
                    }
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
                    if (RadioButtonNombreSi.Checked && (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value) || ControlCF.Value != "CF"))
                    {
                        TxtNombreR.Text = txtNombre.Text;
                        TxtApellidoR.Text = txtApellido.Text;
                        TxtCasadaR.Text = txtCasada.Text;
                        TxtDiRe1.Text = txtDireccion.Text;
                        TxtDiRe2.Text = txtDireccion2.Text;
                        TxtDiRe3.Text = txtDireccion3.Text;
                        txtNit.Text = "CF";
                        IngresoDatos();

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
                                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "'");
                                int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");
                                try
                                {
                                    if (controlRenovacion == 0)
                                    {
                                        //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                        if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + UserEmplid.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + UserEmplid.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                }
                            }
                        }
                    }
                    else 
                    {
                        IngresoDatos();

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
                                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "'");
                                int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");
                                if (controlRenovacion == 0)
                                {
                                    //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                    if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                    {
                                        cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                    "VALUES ('" + UserEmplid.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                    {
                                        cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                    "VALUES ('" + UserEmplid.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (ControlAct.Value == "PC")
                                    {
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                            " WHERE EMPLID='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();

                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                            " WHERE CARNET='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (ControlAct.Value == "RC")
                                    {
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                            " WHERE EMPLID='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                            " WHERE CARNET='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    if (ControlAct.Value == "PC")
                                    {
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                            " WHERE EMPLID='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();

                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                            " WHERE CARNET='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (ControlAct.Value == "RC")
                                    {
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                            " WHERE EMPLID='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                            " WHERE CARNET='" + UserEmplid.Text + "'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                string registroCarne = ValidarRegistros();
                                if (registroCarne == "0")
                                {
                                    cmd.CommandText = txtInsert.Text;
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                                con.Close();
                            }
                        }
                    }
                }
                else
                {
                    if (txtAInicial.Value != txtApellido.Text || txtNInicial.Value != txtNombre.Text || txtCInicial.Value.TrimEnd() != txtCasada.Text)
                    {
                        string script = "<script>Documentos();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        mensaje = "Es necesario adjuntar la imagen de su documento de actualización para continuar con la actualización.";
                        if ((InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim()) || String.IsNullOrEmpty(InicialNR1.Value))
                        {
                            TxtNombreR.Text = txtNombre.Text;
                            TxtApellidoR.Text = txtApellido.Text;
                            TxtCasadaR.Text = TxtCasadaR.Text;
                        }
                    }
                    fotoAlmacenada();
                }
            }
            return mensaje;
        }
        public void DescargaArchivo()
        {
            string rutaArchivo = CurrentDirectory + "/Manuales/";
            string nombreArchivo = "ManualActivacionCamara.pdf";
            // Configurar las cabeceras de la respuesta
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);

            // Descargar el archivo
            Response.TransmitFile(rutaArchivo + nombreArchivo);
            Response.Flush();
        }
        public void EnvioCorreo()
        {
            string htmlBody = LeerBodyEmail("bodyIngresoEstudiante-Operador.txt");
            string[] datos = LeerInfoEmail("datosIngresoEstudiante-Operador.txt");
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Actualización Alumnos", credenciales[3]));
            email.To.Add(new MailboxAddress(credenciales[0], credenciales[3]));

            email.Subject = datos[0];
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

                }
                catch (Exception ex)
                {
                    log("ERROR - Envio de correo para operador");
                    lblActualizacion.Text = ex.ToString();
                }
            }

        }
        public void EnvioCorreoEmpleado()
        {
            string htmlBody = LeerBodyEmail("bodyIngresoEstudiante.txt");
            string[] datos = LeerInfoEmail("datosIngresoEstudiante.txt");
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            var para = txtNombre.Text + " " + txtPrimerApellido.Text;


            email.From.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.To.Add(new MailboxAddress(para, EmailUnis.Text));

            email.Subject = datos[0];
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

                }
                catch (Exception ex)
                {
                    log("ERROR - Envio de correo para " + EmailUnis.Text);
                    lblActualizacion.Text = ex.ToString();
                }
            }
        }
        public string LeerBodyEmail(string archivo)
        {
            string rutaCompleta = CurrentDirectory + "/Emails/Estudiantes/" + archivo;
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
            string rutaCompleta = CurrentDirectory + "/Emails/Estudiantes/" + archivo;
            string[] datos;
            string subjet = "";
            string to = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                string linea4 = file.ReadLine();
                subjet = linea2;
                to = linea4;
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

        private string ValidarRegistros()
        {
            string constr = TxtURL.Text;
            string RegistroCarne = "0";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    //SE BUSCA EL ULTIMO REGISTRO DE CONFIRMACION
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RegistroCarne = reader["CONFIRMACION"].ToString();
                    }
                }
            }
            return RegistroCarne;
        }
        protected string IngresoDatos()
        {
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
                    txtContaador.Text = txtAInicial.Value.Length.ToString() + " " + posicion.ToString();
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
                            txtPrimerApellido.Text = txtApellido.Text.TrimEnd();
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

                        //Obtener codigo país nit
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
                        txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            RegistroCarne = reader["CONTADOR"].ToString();
                        }

                        txtExiste.Text = RegistroCarne.ToString() + " registros";
                        string nombreArchivo = txtCarne.Text + ".jpg";
                        string ruta = null;
                        int cargaFt = 0;
                        if (ControlAct.Value == "AC")
                        {
                            cargaFt = 0;

                        }
                        if (ControlAct.Value == "PC")
                        {
                            ruta = txtPath.Text + nombreArchivo;
                            cargaFt = 0;
                            mensaje = SaveCanvasImage(urlPath2.Value, txtPath.Text, txtCarne.Text + ".jpg");
                        }
                        if (ControlAct.Value == "RC")
                        {
                            ruta = txtPath.Text + nombreArchivo;
                            cargaFt = 0;
                            mensaje = SaveCanvasImage(urlPath2.Value, txtPath.Text, txtCarne.Text + ".jpg");
                        }
                        if (mensaje.Equals("Imagen guardada correctamente.") || (ControlAct.Value == "AC" && mensaje.Equals("")))
                        {
                            cargaFt = 0;
                        }
                        else
                        {
                            cargaFt = 1;
                        }

                        if (cargaFt == 0)
                        {
                            int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");
                            int controlRenovacionPC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'PC'");

                            if (txtConfirmacion.Text == "01")
                            {
                                if (ControlAct.Value == "AC" && (CONFIRMACION == "1000" || CONFIRMACION == "0"))
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosConfirmacion\\ACTUALIZACION-AC\\", txtCarne.Text + ".jpg");
                                }
                                if (ControlAct.Value == "PC" || (ControlAct.Value == "AC" && CONFIRMACION == "1"))
                                {
                                    if (controlRenovacionPC == 0)
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosConfirmacion\\PRIMER_CARNET-PC\\", txtCarne.Text + ".jpg");
                                    else
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosConfirmacion\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                }
                                if (ControlAct.Value == "RC")
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosConfirmacion\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                }
                            }
                            else
                            {
                                if (ControlAct.Value == "AC" && (CONFIRMACION == "1000" || CONFIRMACION == "0"))
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\", txtCarne.Text + ".jpg");
                                }
                                if (ControlAct.Value == "PC" || (ControlAct.Value == "AC" && CONFIRMACION == "1"))
                                {
                                    if (controlRenovacionPC <= 1 || controlRenovacionAC <= 1)
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\", txtCarne.Text + ".jpg");
                                    else
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                }
                                if (ControlAct.Value == "RC")
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                }
                            }

                            cmd.Transaction = transaction;
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
                            cmd.CommandText = "SELECT 'INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte , OTRA_NA , Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, " +
                                            " Fec_Emision, NO_CTA_BI, ID_AGENCIA, CONFIRMACION,TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_R, ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, CONTROL_ACCION) VALUES ('''" +
                                            "||'" + txtPrimerApellido.Text + "'''||'," + //APELLIDO1
                                            "''" + txtApellidoAPEX.Text + //APELLIDO2
                                            "'','||''''||SUBSTR(CARNE,0,13)||''''||','" + //CARNE
                                            "||''''||CEDULA||''''||','" + //CEDULA
                                            "||'''" + txtCasada.Text + "'''||','" +// APELLIDO DE CASADA
                                            "||''''||UPPER(DEPARTAMENTO)||''''||','" + //DEPARTAMENTO DE RESIDENCIA
                                            "||'''" + txtDireccion.Text + "'''||','" +// DIRECCION
                                            "||''''||EMAIL||''''||','" + // CORREO ELECTRONICO
                                            "||STATUS||','" + // ESTADO CIVIL
                                            "||'''" + txtFacultad.Text + "'''||','" + // FACULTAD
                                            "||''''||BIRTHDATE||''''||','" + //FECHA DE NACIMIENTO
                                            "||''''||FLAG_CED||''''||','" +
                                            "||''''||FLAG_DPI||''''||','" +
                                            "||''''||FLAG_PAS||''''||','" +
                                            "||''''||OTRA_NA||''''||','" +
                                            "||''''||UPPER(MUNICIPIO)||''''||'," + //MUNICIPIO DE RESIDENCIA
                                            "''" + txtNit.Text + "'''||','" +//NIT
                                            "||''''||DPI||''''||','" + // NO_CUI
                                            "||''''||PASAPORTE||''''||','" + // NUMERO DE PASAPORTE
                                            "||'''" + nombres[0].ToString() + "'''||','" + //NOMBRE1
                                            "||'''" + txtNombreAPEX.Text + "'''||','" +// NOMBRE 2
                                            "||''''||FIRST_NAME||' '||'" + txtPrimerApellido.Text + "'||''''||','" + //APELLIDO DE IMPRESION
                                            "||''''||PLACE||''''||','" + // PAIS NACIONALIDAD
                                            "||''''||PROF||''''||','" + // PROFESION
                                            "||''''||SEX||''''||'," + // SEXO
                                            "NULL," + //TELEFONO
                                            "NULL," + //ZONA
                                            "" + txtAccion.Text + ",'" + //ACCION
                                            "||'''" + txtTelefono.Text + "'''||','" +// CELULAR
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
                                            "||'''" + CmbPais.Text + "'''||','" +
                                            "||'''" + txtDireccion.Text + "'''||','" +
                                            "||'''" + txtDireccion2.Text + "'''||','" +
                                            "||'''" + txtDireccion3.Text + "'''||','" +
                                            "||'''" + TxtCorreoPersonal.Text + "'''||','" +
                                            "||'''" + ControlAct.Value + "'''||')'" +
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
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN 'HONDURAS' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN 'HONDURAS' ELSE '' END OTRA_NA, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN 'RESIDENTE PERM' ELSE NULL END CONDMIG, " +
                                            "PPD.PHONE, " +
                                            "TO_CHAR(PD.BIRTHDATE, 'DD-MM-YYYY') BIRTHDATE, " +
                                            "AGT.DESCR FACULTAD, " +
                                            "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE NULL END SEX, " +
                                            "CASE WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'Condición Migrante' WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE ' ' END PLACE," +
                                            "CASE WHEN PD.MAR_STATUS = 'M' THEN '2' WHEN PD.MAR_STATUS = 'S' THEN '1' ELSE '1' END STATUS, " +
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
                                            "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID  " +
                                            "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                            "WHERE PN.NATIONAL_ID ='" + TextUser.Text + "') " +
                                            "WHERE CODIGO_BARRAS=DPI||DEPARTAMENTO_CUI||MUNICIPIO_CUI OR CODIGO_BARRAS=PASAPORTE OR CODIGO_BARRAS=CEDULA " +
                                            "ORDER BY 1 ASC";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                txtInsert.Text = reader["INS"].ToString();
                                txtInsertBit.Text = txtInsert.Text.Replace("TBL_HISTORIAL_CARNE", "TBL_BI_HISTORIAL_CARNE");
                            }
                            cmd.Transaction = transaction;
                            cmd.Connection = con;


                            string consultaUP = "1";
                            string consultaUD = "1";
                            try
                            {
                                //Numero de Telefono
                                if (!String.IsNullOrEmpty(TruePhone.Text))
                                { //UPDATE
                                    UD_PERSONAL_PHONE.Value = "<COLL_PERSONAL_PHONE> \n" +
                                                                "                                            <EMPLID>" + txtCarne.Text + @"</EMPLID>" +
                                                                "\n" +
                                                                "                                            <KEYPROP_PHONE_TYPE>HOME</KEYPROP_PHONE_TYPE> \n" +
                                                                "                                            <PROP_PHONE>" + txtTelefono.Text + @"</PROP_PHONE>" +
                                                                "\n" +
                                                                "                                            <PROP_PREF_PHONE_FLAG>Y</PROP_PREF_PHONE_FLAG> \n" +
                                                             "                                         </COLL_PERSONAL_PHONE> \n";
                                    contadorUD = contadorUD + 1;
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
                                    contadorUP = contadorUP + 1;
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
                                    contadorUD = contadorUD + 1;
                                }
                                else
                                {//INSERT
                                    UP_EMAIL_ADDRESSES.Value = "<COLL_EMAIL_ADDRESSES>\n" +
                                                                    "                                            <KEYPROP_E_ADDR_TYPE>HOM1</KEYPROP_E_ADDR_TYPE> \n" +
                                                                    "                                            <PROP_EMAIL_ADDR>" + TxtCorreoPersonal.Text + @"</PROP_EMAIL_ADDR> " +
                                                                    "\n" +
                                                                    "                                            <PROP_PREF_EMAIL_FLAG>N</PROP_PREF_EMAIL_FLAG> \n" +
                                                                 "                                         </COLL_EMAIL_ADDRESSES> \n";
                                    contadorUP = contadorUP + 1;
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
                                    contadorUP = contadorUP + 1;
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
                                    contadorUD = contadorUD + 1;
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
                                    contadorUD = contadorUD + 1;
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
                                        contadorUP = contadorUP + 1;
                                    }
                                    else
                                    {
                                        UD_PERS_DATA_EFFDT.Value = "<COLL_PERS_DATA_EFFDT>" +
                                                            " <KEYPROP_EFFDT>" + EFFDT_EC.Value + @"</KEYPROP_EFFDT>" +
                                                            " <PROP_MAR_STATUS>" + ec + @"</PROP_MAR_STATUS>" +
                                                             "</COLL_PERS_DATA_EFFDT>";
                                        contadorUD = contadorUD + 1;
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
                                            contadorUP = contadorUP + 1;
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

                                            contadorUD = contadorUD + 1;
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
                                            contadorUD = contadorUD + 1;
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
                                            contadorUP = contadorUP + 1;
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
                                            contadorUD = contadorUD + 1;
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
                                            contadorUD = contadorUD + 1;
                                        }

                                        controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                        controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + UserEmplid.Text + "'");
                                        controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");
                                        if (controlRenovacion == 0)
                                        {
                                            //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                            if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                            {
                                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                            "VALUES ('" + UserEmplid.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                            {
                                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                            "VALUES ('" + UserEmplid.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "PC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();

                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "RC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            if (ControlAct.Value == "PC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();

                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "RC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        llenadoPaisnit();
                                    }
                                }

                                cmd.CommandText = "SELECT ID_REGISTRO AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    RegistroCarne = reader["CONTADOR"].ToString();
                                }

                                if ((txtAInicial.Value != txtApellido.Text || txtNInicial.Value != txtNombre.Text || txtCInicial.Value.TrimEnd() != txtCasada.Text))
                                {
                                    cmd.Connection = con;
                                    cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE ID_REGISTRO = '" + RegistroCarne + "'";
                                    cmd.ExecuteNonQuery();
                                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + txtCarne.Text + "'");
                                    FileUpload2.Visible = false;
                                    CargaDPI.Visible = false;
                                    mostrarInformación();
                                    mensaje = "Su información fue almacenada correctamente. </br> La información ingresada debe ser aprobada antes de ser confirmada. </br> Actualmente, solo se muestran los datos que han sido previamente confirmados.";
                                    string script = "<script>ConfirmacionActualizacionSensible();</script>";
                                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                                }

                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    RegistroCarne = reader["CONTADOR"].ToString();
                                }

                                if (RegistroCarne == "0")
                                {
                                    cmd.CommandText = txtInsert.Text;
                                    cmd.ExecuteNonQuery();
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
                                    if (urlPathControl2.Value == "1")
                                    {
                                        AlmacenarFotografia();
                                    }

                                    fotoAlmacenada();
                                    if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value.Trim() != TxtCasadaR.Text.Trim() || String.IsNullOrEmpty(InicialNR1.Value))
                                    {
                                        PaisNit.Text = CmbPais.SelectedValue;
                                        DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                                        MunicipioNit.Text = CmbMunicipio.SelectedValue;
                                    }
                                    mensaje = "Su información fue actualizada correctamente";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    EliminarAlmacenada();
                                    log("ERROR - Error en almacenamiento Campus: UD = " + consultaUD + "; UP = " + consultaUP + " SOAP: " + Variables.soapBody);
                                    File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                }
                            }
                            catch (Exception x)
                            {
                                transaction.Rollback();
                                EliminarAlmacenada();
                                log("ERROR - "+ x.Message +"-  Error en almacenamiento Campus: UD = " + consultaUD + "; UP = " + consultaUP + " SOAP: " + Variables.soapBody);
                                File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                            }
                        }
                        else
                        {
                            EliminarAlmacenada();
                            log("ERROR - No se almaceno la fotografía de manera correcta");
                            File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                        }
                    }
                }
            }
            catch (Exception X)
            {
                EliminarAlmacenada();
                log("ERROR - Error en la funcion IngresoDatos: " + X.Message);
                try
                {
                    File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                }
                catch (Exception)
                {
                    log("ERROR - No se encontró la ruta");
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
            }

            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
            return mensaje;
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
                if (urlPathControl2.Value == "1")
                    imageData = imageData.Substring(23, largo - 23);
                try
                {
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
            var body = "{\"Credenciales\" : \"" + CREDENCIALES_NIT.Value + "\",\"NIT\":\"" + nit + "\"}";
            string respuesta = api.PostNit(URL_NIT.Value, body);
            return respuesta;
        }
        public void AlmacenarFotografia()
        {
            EliminarRegistrosFotos();
            if (ControlAct.Value != "AC" && ControlAct.Value != "PC" && ControlAct.Value != "RC")
            {
                validarAccion();
            }
            if (!urlPath2.Value.IsNullOrWhiteSpace())
            {
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
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET = '" + txtCarne.Text + "' AND CONTROL ='1'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ExisteFoto = Convert.ToInt16(reader["CONTADOR"]);
                            try
                            {
                                cmd.Connection = con;
                                if (ExisteFoto > 0)
                                {
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE SET FOTOGRAFIA = 'Existe', CONTROL = '1'" +
                                                        "WHERE CARNET = '" + txtCarne.Text + "'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE (FOTOGRAFIA, CARNET, CONTROL) VALUES ('Existe', '" + txtCarne.Text + "', '1')";
                                    cmd.ExecuteNonQuery();
                                }

                                int controlRenovacionPC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'PC'");
                                int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");

                                if (ControlAct.Value == "AC" && (CONFIRMACION == "1000" || CONFIRMACION == "0"))
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/ACTUALIZACION-AC/", txtCarne.Text + ".jpg");
                                }
                                else
                                if (ControlAct.Value == "PC" || (ControlAct.Value == "AC" && CONFIRMACION == "1"))
                                {
                                    if (controlRenovacionPC <= 1 || controlRenovacionAC <= 1)
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/PRIMER_CARNET-PC/", txtCarne.Text + ".jpg");
                                    else
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/", txtCarne.Text + ".jpg");
                                }
                                else
                                if (ControlAct.Value == "RC")
                                {
                                    SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/", txtCarne.Text + ".jpg");
                                }

                                SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/CONTROL/", txtCarne.Text + ".jpg");
                                transaction.Commit();
                                urlPathControl2.Value = "";
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                                fotoAlmacenada();
                            }
                        }
                    }
                }
            }
        }

        //EVENTOS       
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            llenadoState();
            fotoAlmacenada();

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
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            llenadoStateNIT();
            fotoAlmacenada();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            llenadoMunicipio();
            llenadoState();
            fotoAlmacenada();
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
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            llenadoMunicipioNIT();
            llenadoStateNIT();
            fotoAlmacenada();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
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
        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            string constr = TxtURL.Text;
            string control = null;
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
                    cmd.CommandText = "SELECT CONTROL_ACCION, CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + UserEmplid.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        control = reader["CONTROL_ACCION"].ToString();
                        CONFIRMACION = reader["CONFIRMACION"].ToString();
                    }
                    validarAccion();

                    if (control != ControlAct.Value && CONFIRMACION != "0")
                    {
                        cmd.Transaction = transaction;
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + UserEmplid.Text + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                        }
                        catch (Exception x)
                        {
                            log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                            File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                        }
                    }
                }

                if ((control == "AC" && CONFIRMACION != "0") || (control == null && CONFIRMACION == "1000"))
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                }
                else if (control == "PC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                }
                else if (control == "RC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                }
                else if (ControlAct.Value == "PC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                }
                else if (ControlAct.Value == "RC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                }
            }

            string informacion = actualizarInformacion();
            if (informacion != "0")
            {
                if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(CmbPais.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                {
                    if (RadioButtonNombreNo.Checked)
                    {
                        if (CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() || CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() || CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                        {
                            if (urlPathControl2.Value == "1")
                            {
                                AlmacenarFotografia();
                            }
                            fotoAlmacenada();
                            mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                            lblActualizacion.Text = mensaje;
                            // Al finalizar la actualización, ocultar el modal
                            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                        }
                        else
                        {
                            // Llama a la función JavaScript para mostrar el modal
                            EliminarAlmacenada();
                            EnvioCorreo();
                            EnvioCorreoEmpleado();
                            log("La información fue actualizada de forma correcta");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        EliminarAlmacenada();
                        EnvioCorreo();
                        EnvioCorreoEmpleado();
                        log("La información fue actualizada de forma correcta");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                    }
                }
                else
                {
                    EliminarAlmacenada();
                    log("ERROR - Es necesario seleccionar un teléfono, estado civil, país, departamento y municipio ");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }
            }
        }
        protected void CmbPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
            llenadoDepartamento();
            llenadoMunicipio();
            llenadoState();
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
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            if (CmbPaisNIT.SelectedValue != " ")
            {
                llenadoDepartamentoNit();
                llenadoMunicipioNIT();
                llenadoStateNIT();
            }
            fotoAlmacenada();
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
            TxtNombreR.Text = "";
            TxtApellidoR.Text = "";
            TxtCasadaR.Text = "";
            TxtDiRe1.Text = "";
            TxtDiRe2.Text = "";
            TxtDiRe3.Text = "";
            validarAccion();
            string respuesta;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            respuesta = consultaNit(txtNit.Text);
            string constr = TxtURL.Text;
            if (ControlAct.Value == "AC")
                RadioButtonActualiza.Checked = true;
            else
                RadioButtonCarne.Checked = true;

            if (respuesta.Equals("BadRequest") || respuesta.Equals("1"))
            {
                TxtNombreR.Text = null;
                TxtApellidoR.Text = null;
                TxtCasadaR.Text = null;
                llenadoPaisnit();
                CmbPaisNIT.SelectedValue = " ";
                llenadoDepartamentoNit();
                CmbDepartamentoNIT.SelectedValue = " ";
                llenadoMunicipioNIT();
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
                    TxtDiRe2.Text = "";
                    TxtDiRe3.Text = "";
                    llenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    llenadoDepartamentoNit();
                    CmbDepartamentoNIT.SelectedValue = " ";
                    llenadoMunicipioNIT();
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
                    if (urlPathControl2.Value == "1")
                    {
                        AlmacenarFotografia();
                    }
                    fotoAlmacenada();
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
                    llenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    string script = "<script>NoExisteNit();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
            }
            TrueNit.Value = txtNit.Text;
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
        }
        static string[] DividirEnArray(string cadena)
        {
            // Dividir la cadena en un array de strings usando los espacios como delimitadores
            string[] arrayDePalabras = cadena.Split(' ');
            return arrayDePalabras;
        }
        protected void BtnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/ActualizacionEstudiantes.aspx");
        }
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
        protected string ConsumoSQL(string Consulta)
        {
            string constr = TxtURLSql.Text;
            string retorno = "";
            using (SqlConnection conexion = new SqlConnection(TxtURLSql.Text))
            {
                conexion.Open();
                var trans = conexion.BeginTransaction();
                using (SqlCommand sqlCom = new SqlCommand(Consulta))
                {
                    sqlCom.Transaction = trans;
                    try
                    {
                        sqlCom.Connection = conexion;
                        sqlCom.ExecuteNonQuery();
                        trans.Commit();
                        conexion.Close();
                        retorno = "0";
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }
        protected void BtnAceptarCarga_Click(object sender, EventArgs e)
        {
            string constr = TxtURL.Text;
            string control = null;
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
                    cmd.CommandText = "SELECT CONTROL_ACCION, CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + UserEmplid.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        control = reader["CONTROL_ACCION"].ToString();
                        CONFIRMACION = reader["CONFIRMACION"].ToString();
                    }
                    validarAccion();

                    if (control != ControlAct.Value && CONFIRMACION != "0")
                    {
                        cmd.Transaction = transaction;
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + UserEmplid.Text + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                        }
                        catch (Exception x)
                        {
                            log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                            File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                        }
                    }
                }

                if ((control == "AC" && CONFIRMACION != "0") || (control == null && CONFIRMACION == "1000"))
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                }
                else if (control == "PC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                }
                else if (control == "RC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                }
                else if (ControlAct.Value == "PC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + UserEmplid.Text + ".jpg");
                }
                else if (ControlAct.Value == "RC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + UserEmplid.Text + ".jpg");
                }
            }
            string informacion = actualizarInformacion();
            if (informacion != "0" && informacion != "")
            {
                if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(CmbPais.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                {
                    if (RadioButtonNombreNo.Checked)
                    {
                        if (CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() || CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() || CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                        {
                            if (urlPathControl2.Value == "1")
                            {
                                AlmacenarFotografia();
                            }

                            fotoAlmacenada();
                            mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                            lblActualizacion.Text = mensaje;
                            // Al finalizar la actualización, ocultar el modal
                            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                        }
                        else
                        {
                            // Llama a la función JavaScript para mostrar el modal
                            EliminarAlmacenada();
                            EnvioCorreo();
                            EnvioCorreoEmpleado();

                            using (OracleConnection con = new OracleConnection(constr))
                            {
                                con.Open();
                                OracleTransaction transaction;
                                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Connection = con;
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION ='1'" + " WHERE CARNET='" + UserEmplid.Text + "'";
                                    cmd.ExecuteNonQuery();
                                    transaction.Commit();
                                }
                            }
                            log("La información sensible fue actualizada de forma correcta");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalSensible", "ConfirmacionActualizacionSensible();", true);
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        EliminarAlmacenada();
                        EnvioCorreo();
                        EnvioCorreoEmpleado();

                        using (OracleConnection con = new OracleConnection(constr))
                        {
                            con.Open();
                            OracleTransaction transaction;
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            using (OracleCommand cmd = new OracleCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION ='1'" + " WHERE CARNET='" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                        }
                        log("La información sensible fue actualizada de forma correcta");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalSensible", "ConfirmacionActualizacionSensible();", true);
                    }
                }
                else
                {
                    EliminarAlmacenada();
                    EnvioCorreo();
                    EnvioCorreoEmpleado();
                    log("La información fue actualizada de forma correcta");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                }
            }
            else
            {
                EliminarAlmacenada();
                log("ERROR - Error en la funcion actualizarInformacion en AceptarCarga " + informacion);
                File.Delete(txtPath.Text + UserEmplid.Text + ".jpg");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
            }
        }
        protected int ControlRenovacion(string cadena)
        {
            txtExiste4.Text = "SELECT CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_CONTROL_CARNET " + cadena;
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
                        cmd.CommandText = txtExiste4.Text;
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

        protected int ControlRenovacionIntermedia(string cadena)
        {
            txtExiste4.Text = "SELECT CONFIRMACION " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE " + cadena;
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
                        cmd.CommandText = txtExiste4.Text;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONFIRMACION"].ToString();
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
        protected int ControlAC(string cadena)
        {
            txtExiste4.Text = "SELECT COUNT(*) CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_CONTROL_CARNET " + cadena;
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
                        cmd.CommandText = txtExiste4.Text;
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

        private void log(string ErrorLog)
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
                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + txtCarne.Text + "','" + ErrorLog.Replace('\'', ' ') + "','ACTUALIZACION EMPLEADOS',SYSDATE)";
                    //cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + txtCarne.Text + "','" + ErrorLog + "','ACTUALIZACION ESTUDIANTES',SYSDATE)";
                    cmd.ExecuteNonQuery();
                    if (txtControlBit.Text == "0" && !txtInsertBit.Text.IsNullOrWhiteSpace())
                    {
                        cmd.CommandText = txtInsertBit.Text;
                        cmd.ExecuteNonQuery();
                        txtControlBit.Text = "1";
                    }
                    if (txtControlAR.Text == "0" && !txtUpdateAR.Text.IsNullOrWhiteSpace())
                    {
                        cmd.CommandText = txtUpdateAR.Text;
                        cmd.ExecuteNonQuery();
                        txtControlAR.Text = "1";
                    }
                    if (txtControlNR.Text == "0" && !txtUpdateNR.Text.IsNullOrWhiteSpace())
                    {
                        cmd.CommandText = txtUpdateNR.Text;
                        cmd.ExecuteNonQuery();
                        txtControlNR.Text = "1";
                    }

                    transaction.Commit();

                }
            }
        }

        public void validarAccion()
        {
            int contadorRegistro = 0;
            int contadorConfirmacion = 0;
            contadorRegistro = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "'");
            contadorConfirmacion = ControlRenovacionIntermedia("WHERE CARNET  ='" + txtCarne.Text + "'");
            int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'AC'");
            int controlRenovacionPC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'PC'");
            int controlRenovacionRC = ControlAC("WHERE EMPLID  ='" + UserEmplid.Text + "' AND ACCION = 'RC'");

            if (RadioButtonActualiza.Checked)
            {
                if (contadorRegistro == 0)
                {
                    // INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                    if (controlRenovacionAC == 0 || controlRenovacionAC == 1)
                    {
                        ControlAct.Value = "AC";
                    }
                    else if (controlRenovacionPC <= 1 && controlRenovacionRC == 0)
                    {
                        ControlAct.Value = "PC";
                    }
                }
                else if (controlRenovacionRC >= 1)
                {
                    if (contadorConfirmacion == 0)
                    {
                        ControlAct.Value = "AC";
                    }
                    else
                    {
                        ControlAct.Value = "RC";
                    }
                }
                else if ((controlRenovacionPC < 1 && contadorConfirmacion == 0) || (controlRenovacionAC <= 1 && contadorConfirmacion != 0 && controlRenovacionRC == 0))
                {
                    ControlAct.Value = "PC";
                }
                else
                {
                    ControlAct.Value = "AC";
                }

            }
            else if (RadioButtonCarne.Checked)
            {
                if (controlRenovacionRC >= 1 || (controlRenovacionPC >= 1 && contadorConfirmacion == 0))
                {
                    ControlAct.Value = "RC";
                }
                else
                {
                    ControlAct.Value = "PC";
                }
            }
        }

        /*-------------------------------------------INICIAN FUNCIONES PARA METODO SOAP-------------------------------------------*/
        //Función para limpiar variables
        private static void limpiarVariables()
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
        private static void credencialesEndPoint(string RutaConfiguracion, string strMetodo)
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
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguraciones, "Consultar");

            if (auxConsulta == 0)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UP_NAMES_NIT.Value, UP_PERS_DATA_EFFDT.Value, UP_ADDRESSES_NIT.Value, UP_ADDRESSES.Value, UP_PERSONAL_PHONE.Value, UP_EMAIL_ADDRESSES.Value, VersionUP.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UD_NAMES_NIT.Value, UD_PERS_DATA_EFFDT.Value, UD_ADDRESSES_NIT.Value, UD_ADDRESSES.Value, UD_PERSONAL_PHONE.Value, UD_EMAIL_ADDRESSES.Value, VersionUD.Value);
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
            //return elemList[0].InnerText.ToString();
            return elemList[0].InnerText.ToString();
        }

        private static void CuerpoConsultaUD(string Usuario, string Pass, string EMPLID, string COLL_NAMES, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE,
            string COLL_EMAIL_ADDRESSES, string VersionUD)
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
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                      </Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUP(string Usuario, string Pass, string EMPLID, string COLL_NAMES, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE,
            string COLL_EMAIL_ADDRESSES, string VersionUP)
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
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                      </Update__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }


    }
}