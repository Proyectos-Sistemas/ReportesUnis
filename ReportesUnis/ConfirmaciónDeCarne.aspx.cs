using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Net;
using System.Web.Services;
using System.Xml;
using System.Text;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using System.Web.Security;


namespace ReportesUnis
{
    public partial class ConfirmaciónDeCarne : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        /*VARIABLES PARA ACTUALIZACION EN CAMPUS*/
        string NITAC = "";
        string TxtNombreRAC = "";
        string TxtApellidoRAC = "";
        string TxtCasadaRAC = "";
        string TxtDiRe1AC = "";
        string TxtDiRe2AC = "";
        string TxtDiRe3AC = "";
        string StateNitAC = "";
        string PaisNitAC = "";
        string Direccion1AC = "";
        string Direccion2AC = "";
        string Direccion3AC = "";
        /*VARIABLES PARA PRIMER CARNET*/
        string NITPC = "";
        string TxtNombreRPC = "";
        string TxtApellidoRPC = "";
        string TxtCasadaRPC = "";
        string TxtDiRe1PC = "";
        string TxtDiRe2PC = "";
        string TxtDiRe3PC = "";
        string StateNitPC = "";
        string PaisNitPC = "";
        string Direccion1PC = "";
        string Direccion2PC = "";
        string Direccion3PC = "";
        /*VARIABLES PARA PRIMER CARNET*/
        string NITRC = "";
        string TxtNombreRRC = "";
        string TxtApellidoRRC = "";
        string TxtCasadaRRC = "";
        string TxtDiRe1RC = "";
        string TxtDiRe2RC = "";
        string TxtDiRe3RC = "";
        string StateNitRC = "";
        string PaisNitRC = "";
        string Direccion1RC = "";
        string Direccion2RC = "";
        string Direccion3RC = "";


        int controlRenovacion = 0;
        int controlRenovacionFecha = 0;
        int auxConsulta = 0;
        int contadorUP = 0;
        int contadorUD = 0;
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigCampus.dat");
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("ACCESO_CARNETIZACION") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LeerInfoTxt();
                LeerInfoTxtSQL();
                LeerInfoTxtPath();
                //PARA TAB ACTUALIZACION
                LimpiarCamposAC();
                divCamposAC.Visible = true;
                divDPIAC.Visible = true;
                divFotografiaAC.Visible = true;
                divBtnConfirmarAC.Visible = true;
                BuscarAC("1");
                lblActualizacionAC.Text = null;

                //PARA TAB PRIMER CARNE
                LimpiarCamposPC();
                divCamposPC.Visible = true;
                divDPIPC.Visible = true;
                divFotografiaPC.Visible = true;
                divBtnConfirmarPC.Visible = true;
                BuscarPC("1");
                lblActualizacionPC.Text = null;

                //PARA TAB RENOVACION CARNE
                LimpiarCamposRC();
                divCamposRC.Visible = true;
                divDPIRC.Visible = true;
                divFotografiaRC.Visible = true;
                divBtnConfirmarRC.Visible = true;
                BuscarRC("1");
                lblActualizacionRC.Text = null;

                // Establecer el índice de la pestaña activa por defecto en la primera carga
                ViewState["ActiveTabIndex"] = 0;
                ControlTabs.Value = "AC";
                // Establecer la pestaña activa y su estilo correspondiente
                SetActiveTab(0);
            }
        }

        //FUNCIONES
        private void BuscarAC(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET FROM DUAL UNION SELECT CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE TIPO_PERSONA = 2 AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'AC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarneAC.DataSource = ds;
                    CmbCarneAC.DataTextField = "CARNET";
                    CmbCarneAC.DataValueField = "CARNET";
                    CmbCarneAC.DataBind();
                    con.Close();
                }
            }
        }
        private void BuscarPC(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET FROM DUAL UNION SELECT CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE TIPO_PERSONA = 2 AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'PC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarnePC.DataSource = ds;
                    CmbCarnePC.DataTextField = "CARNET";
                    CmbCarnePC.DataValueField = "CARNET";
                    CmbCarnePC.DataBind();
                    con.Close();
                }
            }
        }
        private void BuscarRC(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET FROM DUAL UNION SELECT CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE TIPO_PERSONA = 2 AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'RC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarneRC.DataSource = ds;
                    CmbCarneRC.DataTextField = "CARNET";
                    CmbCarneRC.DataValueField = "CARNET";
                    CmbCarneRC.DataBind();
                    con.Close();
                }
            }
        }
        void LeerInfoTxt()
        {
            //Lectura de archivo txt para la conexion ORACLE
            string rutaCompleta = CurrentDirectory + "conexion.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                TxtURL.Text = line;
                file.Close();
            }
        }
        void LeerInfoTxtSQL()
        {
            //Lectura de archivo txt para la conexion SQL
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
            //Lectura de archivo txt para el almacenamiento en el servidor
            string rutaCompleta = CurrentDirectory + "PathAlmacenamiento.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath.Text = line;
                file.Close();
            }
        }
        private void llenadoAC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND TIPO_PERSONA = 2 AND CONFIRMACION = 1 AND CONTROL_ACCION = 'AC'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiAC.Text = reader["CUI"].ToString();
                        if (TxtDpiAC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiAC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombreAC.Text = reader["NOMBRE1"].ToString().TrimEnd();
                        TxtSegundoNombreAC.Text = reader["NOMBRE2"].ToString().TrimEnd();
                        TxtPrimerApellidoAC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoAC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaAC.Text = reader["DECASADA"].ToString();
                        TxtCarreraAC.Text = reader["CARGO"].ToString();
                        TxtFacultadAC.Text = reader["FACULTAD"].ToString();
                        TxtFechaNacAC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoAC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionAC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoAC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioAC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelAC.Text = reader["CELULAR"].ToString();
                        txtCantidadAC.Text = reader["TOTALFOTOS"].ToString();
                        TxtNombreRAC = reader["NOMBRE_NIT"].ToString();
                        TxtApellidoRAC = reader["APELLIDOS_NIT"].ToString();
                        TxtCasadaRAC = reader["CASADA_NIT"].ToString();
                        TxtDiRe1AC = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2AC = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3AC = reader["DIRECCION3_NIT"].ToString();
                        StateNitAC = reader["STATE_NIT"].ToString();
                        PaisNitAC = reader["PAIS_NIT"].ToString();
                        TxtPaisAC.Text = reader["PAIS_R"].ToString();
                        Direccion1AC = reader["ADDRESS1"].ToString();
                        Direccion2AC = reader["ADDRESS2"].ToString();
                        Direccion3AC = reader["ADDRESS3"].ToString();
                        TxtCorreoInstitucionalAC.Text = reader["EMAIL"].ToString();
                        TxtCorreoPersonalAC.Text = reader["EMAIL_PERSONAL"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void llenadoPC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND TIPO_PERSONA = 2 AND CONFIRMACION = 1 AND CONTROL_ACCION = 'PC'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiPC.Text = reader["CUI"].ToString();
                        if (TxtDpiPC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiPC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombrePC.Text = reader["NOMBRE1"].ToString().TrimEnd();
                        TxtSegundoNombrePC.Text = reader["NOMBRE2"].ToString().TrimEnd();
                        TxtPrimerApellidoPC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoPC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaPC.Text = reader["DECASADA"].ToString();
                        TxtCarreraPC.Text = reader["CARGO"].ToString();
                        TxtFacultadPC.Text = reader["FACULTAD"].ToString();
                        TxtFechaNacPC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoPC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionPC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoPC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioPC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelPC.Text = reader["CELULAR"].ToString();
                        txtCantidadPC.Text = reader["TOTALFOTOS"].ToString();
                        TxtNombreRPC = reader["NOMBRE_NIT"].ToString();
                        TxtApellidoRPC = reader["APELLIDOS_NIT"].ToString();
                        TxtCasadaRPC = reader["CASADA_NIT"].ToString();
                        TxtDiRe1PC = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2PC = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3PC = reader["DIRECCION3_NIT"].ToString();
                        StateNitPC = reader["STATE_NIT"].ToString();
                        PaisNitPC = reader["PAIS_NIT"].ToString();
                        TxtPaisPC.Text = reader["PAIS_R"].ToString();
                        Direccion1PC = reader["ADDRESS1"].ToString();
                        Direccion2PC = reader["ADDRESS2"].ToString();
                        Direccion3PC = reader["ADDRESS3"].ToString();
                        TxtCorreoInstitucionalPC.Text = reader["EMAIL"].ToString();
                        TxtCorreoPersonalPC.Text = reader["EMAIL_PERSONAL"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void llenadoRC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND TIPO_PERSONA = 2 AND CONFIRMACION = 1 AND CONTROL_ACCION = 'RC'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiRC.Text = reader["CUI"].ToString();
                        if (TxtDpiRC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiRC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombreRC.Text = reader["NOMBRE1"].ToString().TrimEnd();
                        TxtSegundoNombreRC.Text = reader["NOMBRE2"].ToString().TrimEnd();
                        TxtPrimerApellidoRC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoRC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaRC.Text = reader["DECASADA"].ToString();
                        TxtCarreraRC.Text = reader["CARGO"].ToString();
                        TxtFacultadRC.Text = reader["FACULTAD"].ToString();
                        TxtFechaNacRC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoRC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionRC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoRC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioRC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelRC.Text = reader["CELULAR"].ToString();
                        txtCantidadRC.Text = reader["TOTALFOTOS"].ToString();
                        TxtNombreRRC = reader["NOMBRE_NIT"].ToString();
                        TxtApellidoRRC = reader["APELLIDOS_NIT"].ToString();
                        TxtCasadaRRC = reader["CASADA_NIT"].ToString();
                        TxtDiRe1RC = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2RC = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3RC = reader["DIRECCION3_NIT"].ToString();
                        StateNitRC = reader["STATE_NIT"].ToString();
                        PaisNitRC = reader["PAIS_NIT"].ToString();
                        TxtPaisRC.Text = reader["PAIS_R"].ToString();
                        Direccion1RC = reader["ADDRESS1"].ToString();
                        Direccion2RC = reader["ADDRESS2"].ToString();
                        Direccion3RC = reader["ADDRESS3"].ToString();
                        TxtCorreoInstitucionalRC.Text = reader["EMAIL"].ToString();
                        TxtCorreoPersonalRC.Text = reader["EMAIL_PERSONAL"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void LimpiarCamposAC()
        {
            TxtDpiAC.Text = null;
            TxtPrimerNombreAC.Text = null;
            TxtSegundoNombreAC.Text = null;
            TxtPrimerApellidoAC.Text = null;
            TxtSegundoApellidoAC.Text = null;
            TxtApellidoCasadaAC.Text = null;
            TxtCarreraAC.Text = null;
            TxtFacultadAC.Text = null;
            TxtFechaNacAC.Text = null;
            TxtEstadoAC.Text = null;
            TxtDireccionAC.Text = null;
            TxtDepartamentoAC.Text = null;
            TxtMunicipioAC.Text = null;
            TxtTelAC.Text = null;
            ImgDPI2AC.ImageUrl = null;
            ImgDPI1AC.ImageUrl = null;
            ImgFoto1AC.ImageUrl = null;
            txtCantidadAC.Text = null;
            TxtPaisAC.Text = null;
            TxtCorreoInstitucionalAC.Text = null;
            TxtCorreoPersonalAC.Text = null;
        }
        private void LimpiarCamposPC()
        {
            TxtDpiPC.Text = null;
            TxtPrimerNombrePC.Text = null;
            TxtSegundoNombrePC.Text = null;
            TxtPrimerApellidoPC.Text = null;
            TxtSegundoApellidoPC.Text = null;
            TxtApellidoCasadaPC.Text = null;
            TxtCarreraPC.Text = null;
            TxtFacultadPC.Text = null;
            TxtFechaNacPC.Text = null;
            TxtEstadoPC.Text = null;
            TxtDireccionPC.Text = null;
            TxtDepartamentoPC.Text = null;
            TxtMunicipioPC.Text = null;
            TxtTelPC.Text = null;
            ImgDPI2PC.ImageUrl = null;
            ImgDPI1PC.ImageUrl = null;
            ImgFoto1PC.ImageUrl = null;
            txtCantidadPC.Text = null;
            TxtPaisPC.Text = null;
            TxtCorreoInstitucionalPC.Text = null;
            TxtCorreoPersonalPC.Text = null;
        }
        private void LimpiarCamposRC()
        {
            TxtDpiRC.Text = null;
            TxtPrimerNombreRC.Text = null;
            TxtSegundoNombreRC.Text = null;
            TxtPrimerApellidoRC.Text = null;
            TxtSegundoApellidoRC.Text = null;
            TxtApellidoCasadaRC.Text = null;
            TxtCarreraRC.Text = null;
            TxtFacultadRC.Text = null;
            TxtFechaNacRC.Text = null;
            TxtEstadoRC.Text = null;
            TxtDireccionRC.Text = null;
            TxtDepartamentoRC.Text = null;
            TxtMunicipioRC.Text = null;
            TxtTelRC.Text = null;
            ImgDPI2RC.ImageUrl = null;
            ImgDPI1RC.ImageUrl = null;
            ImgFoto1RC.ImageUrl = null;
            txtCantidadRC.Text = null;
            TxtPaisRC.Text = null;
            TxtCorreoInstitucionalRC.Text = null;
            TxtCorreoPersonalRC.Text = null;
        }
        private void RechazarAC(string Carnet)
        {
            if (!TxtPrimerNombreAC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionAC.Text = "";
                string constr = TxtURL.Text;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            int cargaFt = 0;
                            try
                            {
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                cargaFt = 0;
                            }
                            catch (Exception)
                            {
                                cargaFt = 1;
                            }
                            if (cargaFt == 0)
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarAC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/ACTUALIZACION-AC/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidadAC.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEstudiante.txt", "datosRechazoEstudiante.txt", TxtPrimerNombreAC.Text + " " + TxtPrimerApellidoAC.Text, TxtCorreoInstitucionalAC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- AC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionAC();", true);
                                lblActualizacionAC.Text = "Se ha rechazado la solicitud de carnet.";
                            }
                            else
                            {
                                lblActualizacionAC.Text = "Ocurrió un error al rechazar la solicitud";
                                log("Ocurrió un error al eliminar la fotografía AC", Carnet);
                            }
                        }
                        catch (Exception x)
                        {
                            lblActualizacionAC.Text = "No se pudo eliminar la información a causa de un error interno.";
                            log("No se pudo eliminar la información a causa de un error interno. " + x + "- AC", Carnet);
                            transaction.Rollback();
                        }
                    }
                }
                LimpiarCamposAC();
            }
            else
            {
                lblActualizacionAC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        private void RechazarPC(string Carnet)
        {
            if (!TxtPrimerNombrePC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionPC.Text = "";
                string constr = TxtURL.Text;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            int cargaFt = 0;
                            try
                            {
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                cargaFt = 0;
                            }
                            catch (Exception)
                            {
                                cargaFt = 1;
                            }
                            if (cargaFt == 0)
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarPC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidadPC.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEstudiante.txt", "datosRechazoEstudiante.txt", TxtPrimerNombrePC.Text + " " + TxtPrimerApellidoPC.Text, TxtCorreoInstitucionalPC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- PC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionPC();", true);
                                lblActualizacionPC.Text = "Se ha rechazado la solicitud de carnet.";
                            }
                            else
                            {
                                lblActualizacionPC.Text = "Ocurrió un error al rechazar la solicitud";
                                log("Ocurrió un error al eliminar la fotografía PC", Carnet);
                            }
                        }
                        catch (Exception x)
                        {
                            lblActualizacionPC.Text = "No se pudo eliminar la información a causa de un error interno.";
                            log("No se pudo eliminar la información a causa de un error interno. PC" + x, Carnet);
                            transaction.Rollback();
                        }
                    }
                }
                LimpiarCamposPC();
            }
            else
            {
                lblActualizacionPC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        private void RechazarRC(string Carnet)
        {
            if (!TxtPrimerNombreRC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionRC.Text = "";
                string constr = TxtURL.Text;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            int cargaFt = 0;
                            try
                            {
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                cargaFt = 0;
                            }
                            catch (Exception)
                            {
                                cargaFt = 1;
                            }
                            if (cargaFt == 0)
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarRC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidadRC.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEstudiante.txt", "datosRechazoEstudiante.txt", TxtPrimerNombreRC.Text + " " + TxtPrimerApellidoRC.Text, TxtCorreoInstitucionalRC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- RC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionRC();", true);
                                lblActualizacionRC.Text = "Se ha rechazado la solicitud de carnet.";
                            }
                            else
                            {
                                lblActualizacionRC.Text = "Ocurrió un error al rechazar la solicitud";
                                log("Ocurrió un error al eliminar la fotografía RC" + Carnet, Carnet);
                            }
                        }
                        catch (Exception x)
                        {
                            lblActualizacionRC.Text = "No se pudo eliminar la información a causa de un error interno.";
                            log("No se pudo eliminar la información a causa de un error interno. RC" + x, Carnet);
                            transaction.Rollback();
                        }
                    }
                }
                LimpiarCamposRC();
            }
            else
            {
                lblActualizacionRC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void ConfirmarAC(string Carnet)
        {
            if (!TxtPrimerNombreAC.Text.IsNullOrWhiteSpace())
            {
                llenadoAC("CARNET = '" + Carnet + "'");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                //QueryInsertBi();
                respuesta = QueryActualizaNombreAC(Carnet);
                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    respuesta = ActualizarNITAC(CmbCarneAC.Text);
                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            respuesta = ConsumoOracle(txtInsertApex.Text);
                            if (respuesta == "0")
                            {

                                if (controlRenovacion == 0)
                                {
                                    //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                    respuesta = ConsumoOracle("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                }
                                else
                                {
                                    if (controlRenovacionFecha < 1)
                                    {
                                        //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                        respuesta = ConsumoOracle("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                    }
                                    else
                                    {
                                        respuesta = "0";
                                    }
                                }

                                if (respuesta == "0")
                                {
                                    Upload(Carnet);
                                }
                                else if (respuesta != "0")
                                {
                                    log("ERROR - Actualizacion de fotografia en campus AC", Carnet);
                                }
                            }
                            else
                            {
                                log("ERROR - Inserta APEX del carnet: " + Carnet + "- AC", Carnet);
                            }
                        }
                        else
                        {
                            log("ERROR - al armar consulta Update APEX del carnet: " + Carnet + "- AC", Carnet);
                        }
                    }
                    else
                    {
                        log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- AC", Carnet);
                    }
                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionAC();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionAC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEstudiante.txt", "datosConfirmacionEstudiante.txt", TxtPrimerNombreAC.Text + " " + TxtPrimerApellidoAC.Text, TxtCorreoInstitucionalAC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- AC", Carnet);
                        BuscarAC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidadAC.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/ACTUALIZACION-AC/" + Carnet + ".jpg");
                        LimpiarCamposAC();
                    }
                    else
                    {
                        lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                        log("Ocurrió un error al eliminar la fotografía de: " + TxtDpiAC.Text + ", con el carne : " + Carnet + "- AC", Carnet);
                    }
                }
                else
                {
                    lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- AC", Carnet);
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionAC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        protected void ConfirmarPC(string Carnet)
        {
            if (!TxtPrimerNombrePC.Text.IsNullOrWhiteSpace())
            {
                llenadoPC("CARNET = '" + Carnet + "'");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi(CmbCarnePC.SelectedValue);
                respuesta = QueryActualizaNombrePC(Carnet);
                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    respuesta = ActualizarNITPC(CmbCarnePC.Text);
                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            //SE INGRESA LA INFORMACIÓN EN EL BANCO
                            respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                            if (respuesta == "0")
                            {
                                respuesta = ConsumoOracle(txtInsertApex.Text);
                                if (respuesta == "0")
                                {

                                    if (controlRenovacion == 0)
                                    {
                                        //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                        respuesta = ConsumoOracle("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                    }
                                    else
                                    {
                                        if (controlRenovacionFecha < 1)
                                        {
                                            //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                            respuesta = ConsumoOracle("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                        }
                                        else
                                        {
                                            respuesta = "0";
                                        }
                                    }

                                    if (respuesta == "0")
                                    {
                                        respuesta = Upload(Carnet);
                                    }
                                    else if (respuesta != "0")
                                    {
                                        log("ERROR - Actualizacion de fotografia en campus del carnet: " + Carnet + "- PC", Carnet);
                                    }
                                }
                                else
                                {
                                    log("ERROR - Inserta APEX del carnet: " + Carnet + "- PC", Carnet);
                                }
                            }
                            else
                            {
                                log("ERROR - Inserta BI del carnet: " + Carnet + "- PC", Carnet);
                            }
                        }
                        else
                        {
                            log("ERROR - al armar consulta Update APEX del carnet: " + Carnet + "- PC", Carnet);
                        }
                    }
                    else
                    {
                        log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- PC", Carnet);
                    }
                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionPC();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionPC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEstudiante.txt", "datosConfirmacionEstudiante.txt", TxtPrimerNombrePC.Text + " " + TxtPrimerApellidoPC.Text, TxtCorreoInstitucionalPC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- PC", Carnet);
                        BuscarPC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidadPC.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                        LimpiarCamposPC();
                    }
                    else
                    {
                        lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                        log("Ocurrió un error al eliminar la fotografía de: " + TxtDpiPC.Text + ", con el carne : " + Carnet + "- PC", Carnet);
                    }
                }
                else
                {
                    lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- PC", Carnet);
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionPC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        protected void ConfirmarRC(string Carnet)
        {
            if (!TxtPrimerNombreRC.Text.IsNullOrWhiteSpace())
            {
                llenadoRC("CARNET = '" + Carnet + "'");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi(CmbCarneRC.SelectedValue);
                respuesta = QueryActualizaNombreRC(Carnet);
                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    respuesta = ActualizarNITRC(CmbCarneRC.Text);
                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            //SE INGRESA LA INFORMACIÓN EN EL BANCO
                            respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                            if (respuesta == "0")
                            {
                                respuesta = ConsumoOracle(txtInsertApex.Text);
                                if (respuesta == "0")
                                {

                                    if (controlRenovacion == 0)
                                    {
                                        //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                        respuesta = ConsumoOracle("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                    }
                                    else
                                    {
                                        if (controlRenovacionFecha < 1)
                                        {
                                            //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                            respuesta = ConsumoOracle("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                        }
                                        else
                                        {
                                            respuesta = "0";
                                        }
                                    }

                                    if (respuesta == "0")
                                    {
                                        Upload(Carnet);
                                    }
                                    else if (respuesta != "0")
                                    {
                                        log("ERROR - Actualizacion de fotografia en campus del carnet: " + Carnet + "- RC", Carnet);
                                    }
                                }
                                else
                                {
                                    log("ERROR - Inserta APEX del carnet: " + Carnet + "- RC", Carnet);
                                }
                            }
                            else
                            {
                                log("ERROR - Inserta BI del carnet: " + Carnet + "- RC", Carnet);
                            }
                        }
                        else
                        {
                            log("ERROR - al armar consulta Update APEX del carnet: " + Carnet + "- RC", Carnet);
                        }
                    }
                    else
                    {
                        log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- RC", Carnet);
                    }
                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacionRC();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionRC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEstudiante.txt", "datosConfirmacionEstudiante.txt", TxtPrimerNombreRC.Text + " " + TxtPrimerApellidoRC.Text, TxtCorreoInstitucionalRC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- RC", Carnet);
                        BuscarRC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidadRC.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                        LimpiarCamposRC();
                    }
                    else
                    {
                        lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                        log("Ocurrió un error al eliminar la fotografía de: " + TxtDpiRC.Text + ", con el carne : " + Carnet + "- RC", Carnet);
                    }
                }
                else
                {
                    lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- RC", Carnet);
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionRC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        protected void QueryInsertBi(string carne)
        {
            string constr = TxtURL.Text;
            txtInsertBI.Text = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT 'INSERT INTO[dbo].[Tarjeta_Identificacion_prueba] " +
                                   "([Carnet] " +
                                   ",[Carrera] " +
                                   ",[Direccion] " +
                                   ",[Zona] " +
                                   ",[Colonia] " +
                                   ",[Cedula] " +
                                   ",[Depto_Cedula] " +
                                   ",[Muni_Cedula] " +
                                   ",[Cargo] " +
                                   ",[Depto] " +
                                   ",[Facultad] " +
                                   ",[Codigo] " +
                                   ",[Tipo_Persona] " +
                                   ",[No_Cta_Bi] " +
                                   ",[FechaNac] " +
                                   ",[Fecha_Solicitado] " +
                                   ",[Fecha_Entrega] " +
                                   ",[Accion] " +
                                   ",[Telefono] " +
                                   ",[Nit] " +
                                   ",[Nombre1] " +
                                   ",[Apellido1] " +
                                   ",[Apellido2] " +
                                   ",[Decasada] " +
                                   ",[Nombre2] " +
                                   ",[Nombreimp] " +
                                   ",[Sexo] " +
                                   ",[Estado_Civil] " +
                                   ",[Path_file] " +
                                   ",[Fecha_Hora] " +
                                   ",[Tipo_Accion] " +
                                   ",[IDUNIV] " +
                                   ",[Codigo_Barras] " +
                                   ",[Fec_Emision] " +
                                   ",[Nombre] " +
                                   ",[Promocion] " +
                                   ",[No_Recibo] " +
                                   ",[Tipo_Sangre] " +
                                   ",[Status] " +
                                   ",[Tipo_Documento] " +
                                   ",[ID_AGENCIA] " +
                                   ",[Muni_Residencia] " +
                                   ",[Depto_Residencia] " +
                                   ",[norden] " +
                                   ",[Observaciones] " +
                                   ",[Pais_nacionalidad] " +
                                   ",[Pais_pasaporte] " +
                                   ",[No_Pasaporte] " +
                                   ",[Profesion] " +
                                   ",[Casa] " +
                                   ",[Apto] " +
                                   ",[Celular] " +
                                   ",[Email] " +
                                   ",[No_Cui] " +
                                   ",[Depto_Cui] " +
                                   ",[Muni_Cui] " +
                                   ",[Pais_Nit] " +
                                   ",[Flag_cedula] " +
                                   ",[Flag_dpi] " +
                                   ",[Flag_pasaporte] " +
                                   ",[Otra_Na] " +
                                   ",[Condmig] " +
                                   ",[O_Condmig] " +
                                   ",[Validar_Envio]) " +
                                "VALUES ('''||CARNET||''','''" + // APELLIDO DE CASADA
                                    "||CARGO||''','''" + //Carrera
                                    "||DIRECCION||''','''" + //DIRECCION
                                    "||ZONA||''','''" + //ZONA
                                    "||COLONIA||''','''" + //COLONIA
                                    "||CEDULA||''','''" + //DECULA
                                    "||DEPTO_CEDULA||''',''' " + //DEPARTAMENTO CEDULA
                                    "||MUNI_CEDULA||''',''' " + //MUNICIPIO CEDULA
                                    "||''||''','''" + //CARGO
                                    "||DEPTO||''',''' " + //DEPARTAMENTO 
                                    "||FACULTAD||''','''" + //FACULTAD
                                    "||CODIGO||''','''" + //CODIGO
                                    "||TIPO_PERSONA||''','''" + //TIPO_PERSONA
                                    "||NO_CTA_BI||''',''' " + //NO CTA BI
                                    "||TO_CHAR(TO_DATE(FECHANAC),'YYYY-MM-DD')||''',''' " + //FECHA NACIMIENTO
                                    "||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''','''" + //FECHA_SOLICITADO
                                    "||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''','''" + //FECHA_ENTREGA
                                    "||ACCION||''','''" + //ACCION
                                    "||TELEFONO||''','''" + //TELEFONO
                                    "||NIT||''','''  " + //NIT
                                    "||NOMBRE1||''',''' " + //NOMBRE1
                                    "||APELLIDO1||''','''  " + //APELLIDO1
                                    "||APELLIDO2||''','''  " + //APELLIDO2
                                    "||DECASADA||''','''   " + //DE CASADA
                                    "||NOMBRE2||''',''' " + //NOMBRE2
                                    "||NOMBREIMP||''','''  " + //NOMBREIMP
                                    "||SEXO||''',''' " + //SEXO
                                    "||ESTADO_CIVIL||''',''' " + //ESTADO_CIVIL
                                    "||PATH_FILE||''',''' " + //PATH
                                    "||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''',''' " + //FECHA_HORA
                                    "||TIPO_ACCION||''',''' " + //TIPO_ACCION
                                    "||IDUNIV||''','''  " + //IDUNIV
                                    "||CODIGO_BARRAS||''',''' " + //CODIGO DE BARRAS
                                    "||FEC_EMISION||''','''" + //FECHA_EMISION
                                    "||NOMBRE||''','''" + //NOMBRE
                                    "||PROMOCION||''','''" + //PROMOCION
                                    "||NO_RECIBO||''','''" + //NO_RECIBIDO
                                    "||TIPO_SANGRE||''','''" + //TIPO_SANGRE
                                    "||STATUS||''','''" + //STATUS
                                    "||TIPO_DOCUMENTO||''','''" + //TIPO_DOCUMENTO
                                    "||ID_AGENCIA||''','''" + //ID_AGENCIA
                                    "||MUNI_RESIDENCIA||''','''" + //MUNI_RESIDENCIA
                                    "||DEPTO_RESIDENCIA||''','''" + //DEPTO_RESIDENCIA
                                    "||NORDEN||''','''" + //NO_ORDER
                                    "||OBSERVACIONES||''','''" + //OBSERVACIONES
                                    "||'GUATEMALA'||''','''" + //PAIS_NACIONALIDAD
                                    "||PAIS_PASAPORTE||''','''" + //PAIS_PASAPORTE
                                    "||NO_PASAPORTE||''','''" + //NO_PASAPORTE
                                    "||PROFESION||''','''" + //PROFESION
                                    "||CASA||''','''" + //CASA
                                    "||APTO||''','''" + //APARTAMENTO
                                    "||CELULAR||''','''" + //CELULAR
                                    "||EMAIL||''','''" + //EMAIL
                                    "||NO_CUI||''','''" + //CELULAR
                                    "||DEPTO_CUI||''','''" + //DEPARTAMENTO_CUI
                                    "||MUNI_CUI||''','''" + //MUNI_CUI
                                    "||PAIS_NIT||''','''" + //PAIS_NIT
                                    "||FLAG_CEDULA||''',''' " +
                                    "||FLAG_DPI||''',''' " +
                                    "||FLAG_PASAPORTE||''',''' " +
                                    "||OTRA_NA||''',''' " + //OTRA_NA 
                                    "||CONDMIG||''',''' " + //CONDICION MIGRANTE
                                    "||O_CONDMIG||''','''  " + //OTRA CONDICION MIGRANTE
                                    "||VALIDAR_ENVIO||''')'" +//OTRA CONDICION MIGRANTE 
                                    " AS INS " +
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + carne + "')";
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtInsertBI.Text = reader["INS"].ToString();
                    }
                }
            }
        }
        protected string QueryActualizaNombreAC(string emplid)
        {
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoAC.Text + " " + TxtSegundoApellidoAC.Text + " " + TxtApellidoCasadaAC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombreAC.Text + " " + TxtSegundoNombreAC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoAC.Text + " " + TxtSegundoApellidoAC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaAC.Text;
            string EFFDT_Name = "";

            if (Direccion2AC == "")
            {
                Direccion2AC = " ";
            }
            if (Direccion3AC == "")
            {
                Direccion3AC = " ";
            }
            if (TxtCasada.IsNullOrWhiteSpace())
            {
                TxtCasada = " ";
            }

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        int ContadorNombre = 0;
                        int ContadorDirecion = 0;
                        int ContadorEffdtNombre = 0;
                        string EffdtNombreUltimo = "";
                        cmd.Connection = con;

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        OracleDataReader reader1 = cmd.ExecuteReader();
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            EffdtNombreUltimo = (Convert.ToDateTime(reader1["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE" +
                            " NAME = '" + vchrApellidosCompletos + "," + TxtNombre + "' " +
                            "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreUltimo).ToString("dd/MM/yyyy") + "' " +
                            "AND NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE !='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
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

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'HOME' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorDirecion = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'PRI' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorEffdtNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        if (EffdtNombreUltimo != Hoy && ContadorNombre == 0 && ContadorEffdtNombre == 0)
                        {
                            // INSERT
                            UP_NAMES_PRF_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UP_NAMES_PRI_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreUltimo == Hoy && ContadorNombre > 0 && ContadorEffdtNombre > 0)
                        {
                            // ACTUALIZAR
                            UD_NAMES_PRF_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {
                            // ACTUALIZAR
                            UD_NAMES_PRF_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        auxConsulta = 0;
                        string consultaUP = "1";
                        string consultaUD = "1";
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
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información ";
                            return "1";
                        }

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información ";
                        return "1";
                    }
                }
            }
        }
        protected string QueryActualizaNombrePC(string emplid)
        {
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoPC.Text + " " + TxtSegundoApellidoPC.Text + " " + TxtApellidoCasadaPC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombrePC.Text + " " + TxtSegundoNombrePC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoPC.Text + " " + TxtSegundoApellidoPC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaPC.Text;
            string EFFDT_Name = "";

            if (Direccion2PC == "")
            {
                Direccion2PC = " ";
            }
            if (Direccion3PC == "")
            {
                Direccion3PC = " ";
            }
            if (TxtCasada.IsNullOrWhiteSpace())
            {
                TxtCasada = " ";
            }

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        int ContadorNombre = 0;
                        int ContadorDirecion = 0;
                        int ContadorEffdtNombre = 0;
                        string EffdtNombreUltimo = "";
                        cmd.Connection = con;

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        OracleDataReader reader1 = cmd.ExecuteReader();
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            EffdtNombreUltimo = (Convert.ToDateTime(reader1["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE" +
                            " NAME = '" + vchrApellidosCompletos + "," + TxtNombre + "' " +
                            "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreUltimo).ToString("dd/MM/yyyy") + "' " +
                            "AND NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE !='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
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

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'HOME' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorDirecion = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'PRI' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorEffdtNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }
                        UD_NAMES_NIT_PC.Value = "";
                        if (EffdtNombreUltimo != Hoy && ContadorNombre == 0 && ContadorEffdtNombre == 0)
                        {
                            // INSERT
                            UP_NAMES_PRF_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UP_NAMES_PRI_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreUltimo == Hoy && ContadorNombre > 0 && ContadorEffdtNombre > 0)
                        {
                            // ACTUALIZAR
                            UD_NAMES_PRF_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {
                            // PCTUALIZAR
                            UD_NAMES_PRF_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        auxConsulta = 2;
                        string consultaUP = "1";
                        string consultaUD = "1";
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }
                        auxConsulta = 3;
                        if (contadorUD > 0)
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información ";
                            return "1";
                        }

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información ";
                        return "1";
                    }
                }
            }
        }
        protected string QueryActualizaNombreRC(string emplid)
        {
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoRC.Text + " " + TxtSegundoApellidoRC.Text + " " + TxtApellidoCasadaRC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombreRC.Text + " " + TxtSegundoNombreRC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoRC.Text + " " + TxtSegundoApellidoRC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaRC.Text;
            string EFFDT_Name = "";

            if (Direccion2RC == "")
            {
                Direccion2RC = " ";
            }
            if (Direccion3RC == "")
            {
                Direccion3RC = " ";
            }
            if (TxtCasada.IsNullOrWhiteSpace())
            {
                TxtCasada = " ";
            }

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        int ContadorNombre = 0;
                        int ContadorDirecion = 0;
                        int ContadorEffdtNombre = 0;
                        string EffdtNombreUltimo = "";
                        cmd.Connection = con;

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        OracleDataReader reader1 = cmd.ExecuteReader();
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            EffdtNombreUltimo = (Convert.ToDateTime(reader1["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE" +
                            " NAME = '" + vchrApellidosCompletos + "," + TxtNombre + "' " +
                            "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreUltimo).ToString("dd/MM/yyyy") + "' " +
                            "AND NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE !='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
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

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'HOME' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorDirecion = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'PRI' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorEffdtNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        if (EffdtNombreUltimo != Hoy && ContadorNombre == 0 && ContadorEffdtNombre == 0)
                        {
                            // INSERT
                            UP_NAMES_PRF_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UP_NAMES_PRI_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreUltimo == Hoy && ContadorNombre > 0 && ContadorEffdtNombre > 0)
                        {
                            // RCTUALIZAR
                            UD_NAMES_PRF_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {
                            // RCTUALIZAR
                            UD_NAMES_PRF_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRF</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";

                            UD_NAMES_PRI_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>PRI</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidos + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombre + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasada + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        auxConsulta = 4;
                        string consultaUP = "1";
                        string consultaUD = "1";
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }
                        auxConsulta = 5;
                        if (contadorUD > 0)
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información ";
                            return "1";
                        }

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información ";
                        return "1";
                    }
                }
            }
        }
        protected void QueryUpdateApex(string Confirmación, string Solicitado, string Entrega, string FechaHora, string Accion, string Carne)
        {
            txtInsertApex.Text = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION = '" + Confirmación + "', FECHA_SOLICITADO='" + Solicitado + "', FECHA_ENTREGA='" + Entrega + "', " +
                "ACCION='" + Accion + "', FECHA_HORA='" + FechaHora + "'" +
                " WHERE CARNET = '" + Carne + "'";
        }
        protected string ConsumoOracle(string ComandoConsulta)
        {
            string constr = TxtURL.Text;
            string retorno = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (!ComandoConsulta.IsNullOrWhiteSpace())
                        {
                            cmd.Transaction = transaction;
                            cmd.Connection = con;
                            cmd.CommandText = ComandoConsulta;
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        con.Close();
                        retorno = "0";
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información " + x;
                        retorno = "1";
                    }
                }
            }
            return retorno;
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
                    catch (Exception x)
                    {
                        TxtEstadoAC.Text += x.ToString();
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }
        private string ActualizarNITAC(string emplid)
        {
            string constr = TxtURL.Text;
            string existeNit = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, " +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, CNT FROM ( " +
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
                                        "WHERE PN.NATIONAL_ID ='" + TxtDpiAC.Text + "' " +
                                       ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        existeNit = reader["NIT"].ToString();
                    }

                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText =
                            "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                            "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                            "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + emplid + "' AND TIPO_PERSONA = 2";
                        OracleDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            TxtNombreRAC = reader2["NOMBRE_NIT"].ToString();
                            TxtApellidoRAC = reader2["APELLIDOS_NIT"].ToString();
                            TxtCasadaRAC = reader2["CASADA_NIT"].ToString();
                            TxtDiRe1AC = reader2["DIRECCION1_NIT"].ToString();
                            TxtDiRe2AC = reader2["DIRECCION2_NIT"].ToString();
                            TxtDiRe3AC = reader2["DIRECCION3_NIT"].ToString();
                            StateNitAC = reader2["STATE_NIT"].ToString();
                            PaisNitAC = reader2["PAIS_NIT"].ToString();
                            NITAC = reader2["NIT"].ToString();
                        }

                        int ContadorNombreNit = 0;
                        int ContadorEffdtNombreNit = 0;
                        int ContadorEffdtDirecionNit = 0;
                        string EffdtDireccionNitUltimo = "";
                        string EffdtNombreNitUltimo = "";
                        string EffdtNitUltimo = "";
                        int ContadorDirecionNit = 0;
                        int ContadorNit = 0;
                        int ContadorNit2 = 0;
                        string EFFDT_SYSTEM = "";
                        string EFFDT_AddressNit = "";

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND  EMPLID = '" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE = 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNombreNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtDireccionNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1AC + "' AND ADDRESS2 = '" + TxtDiRe2AC + "' AND ADDRESS3 = '" + TxtDiRe3AC + "' " +
                                              "AND COUNTRY='" + PaisNitAC + "' AND STATE ='" + StateNitAC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_AddressNit = reader["EFFDT"].ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'" +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITAC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSKEY WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit2 = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_SYSTEM = reader["CONTADOR"].ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRAC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRAC + "' AND SECOND_LAST_NAME='" + TxtCasadaRAC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_AC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();// + reader["EFFDT"].ToString().Substring(9, 2).TrimEnd();

                            if (EFFDT_A_NIT_AC.Value.Length == 9)
                            {
                                EFFDT_A_NIT_AC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A_NIT_AC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_NameR_AC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_NameR_AC.Value.Length == 9)
                            {
                                EFFDT_NameR_AC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_NameR_AC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        string FechaEfectiva = "";
                        if (EFFDT_NameR_AC.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR_AC.Value;

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            UP_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRAC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE

                            UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRAC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;

                        }
                        else
                        {//UPDATE

                            UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRAC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }

                        //ACTUALIZA NIT
                        if (EffdtNitUltimo != Hoy && ContadorNit == 0)
                        {
                            //INSERTA EL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) VALUES ('" + emplid + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + NITAC + "')";
                            cmd.ExecuteNonQuery();

                            if (ContadorNit2 == 0)
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                "VALUES ('" + emplid + "','NRE')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (EffdtNitUltimo == Hoy && ContadorNit > 0)
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITAC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITAC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + EFFDT_SYSTEM.Substring(0, 10).TrimEnd() + "'";
                            cmd.ExecuteNonQuery();
                        }

                        if (EffdtDireccionNitUltimo != Hoy && ContadorDirecionNit == 0 && ContadorEffdtDirecionNit == 0)
                        {//INSERTA
                            UP_ADDRESSES_NIT_AC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitAC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1AC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2AC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3AC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitAC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtDireccionNitUltimo == Hoy && ContadorDirecionNit > 0 && ContadorEffdtDirecionNit > 0)
                        {//ACTUALIZA
                            UD_ADDRESSES_NIT_AC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitAC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1AC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2AC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3AC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitAC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            UD_ADDRESSES_NIT_AC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                "                                            <COLL_ADDRESSES> \n" +
                                                  "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                  "                                                <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT> " +
                                                  "\n" +
                                                  "                                                <PROP_COUNTRY>" + PaisNitAC + @"</PROP_COUNTRY> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS1>" + TxtDiRe1AC + @"</PROP_ADDRESS1> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS2>" + TxtDiRe2AC + @"</PROP_ADDRESS2> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS3>" + TxtDiRe3AC + @"</PROP_ADDRESS3> " +
                                                  "\n" +
                                                  "                                                <PROP_STATE>" + StateNitAC + @"</PROP_STATE>  " +
                                                  "\n" +
                                                "                                            </COLL_ADDRESSES> \n" +
                                             "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }

                        auxConsulta = 0;
                        string consultaUP = "1";
                        string consultaUD = "1";
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }
                        auxConsulta = 1;
                        if (contadorUD > 0 && consultaUP == "1")
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            transaction.Commit();
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionAC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionAC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                        return "1";
                    }
                }
            }
        }
        private string ActualizarNITPC(string emplid)
        {
            string constr = TxtURL.Text;
            string existeNit = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, " +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, CNT FROM ( " +
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
                                        "WHERE PN.NATIONAL_ID ='" + TxtDpiPC.Text + "' " +
                                       ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        existeNit = reader["NIT"].ToString();
                    }

                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText =
                            "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                            "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                            "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + emplid + "' AND TIPO_PERSONA = 2";
                        OracleDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            TxtNombreRPC = reader2["NOMBRE_NIT"].ToString();
                            TxtApellidoRPC = reader2["APELLIDOS_NIT"].ToString();
                            TxtCasadaRPC = reader2["CASADA_NIT"].ToString();
                            TxtDiRe1PC = reader2["DIRECCION1_NIT"].ToString();
                            TxtDiRe2PC = reader2["DIRECCION2_NIT"].ToString();
                            TxtDiRe3PC = reader2["DIRECCION3_NIT"].ToString();
                            StateNitPC = reader2["STATE_NIT"].ToString();
                            PaisNitPC = reader2["PAIS_NIT"].ToString();
                            NITPC = reader2["NIT"].ToString();
                        }

                        int ContadorNombreNit = 0;
                        int ContadorEffdtNombreNit = 0;
                        int ContadorEffdtDirecionNit = 0;
                        string EffdtDireccionNitUltimo = "";
                        string EffdtNombreNitUltimo = "";
                        string EffdtNitUltimo = "";
                        int ContadorDirecionNit = 0;
                        int ContadorNit = 0;
                        int ContadorNit2 = 0;
                        string EFFDT_SYSTEM = "";
                        string EFFDT_AddressNit = "";

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND  EMPLID = '" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE = 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNombreNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtDireccionNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1PC + "' AND ADDRESS2 = '" + TxtDiRe2PC + "' AND ADDRESS3 = '" + TxtDiRe3PC + "' " +
                                              "AND COUNTRY='" + PaisNitPC + "' AND STATE ='" + StateNitPC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_AddressNit = reader["EFFDT"].ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'" +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITPC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSKEY WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit2 = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_SYSTEM = reader["CONTADOR"].ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRPC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRPC + "' AND SECOND_LAST_NAME='" + TxtCasadaRPC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_PC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();// + reader["EFFDT"].ToString().Substring(9, 2).TrimEnd();

                            if (EFFDT_A_NIT_PC.Value.Length == 9)
                            {
                                EFFDT_A_NIT_PC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A_NIT_PC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_NameR_PC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_NameR_PC.Value.Length == 9)
                            {
                                EFFDT_NameR_PC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_NameR_PC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        string FechaEfectiva = "";
                        if (EFFDT_NameR_PC.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR_PC.Value;

                        UD_NAMES_PRI_PC.Value = "";
                        UD_NAMES_PRF_PC.Value = "";

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            UP_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRPC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE

                            UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRPC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;

                        }
                        else
                        {//UPDATE

                            UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRPC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }

                        //ACTUALIZA NIT
                        if (EffdtNitUltimo != Hoy && ContadorNit == 0)
                        {
                            //INSERTA EL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) VALUES ('" + emplid + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + NITPC + "')";
                            cmd.ExecuteNonQuery();

                            if (ContadorNit2 == 0)
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                "VALUES ('" + emplid + "','NRE')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (EffdtNitUltimo == Hoy && ContadorNit > 0)
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITPC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITPC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + EFFDT_SYSTEM.Substring(0, 10).TrimEnd() + "'";
                            cmd.ExecuteNonQuery();
                        }

                        if (EffdtDireccionNitUltimo != Hoy && ContadorDirecionNit == 0 && ContadorEffdtDirecionNit == 0)
                        {//INSERTA
                            UP_ADDRESSES_NIT_PC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitPC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1PC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2PC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3PC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitPC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtDireccionNitUltimo == Hoy && ContadorDirecionNit > 0 && ContadorEffdtDirecionNit > 0)
                        {//ACTUALIZA
                            UD_ADDRESSES_NIT_PC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitPC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1PC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2PC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3PC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitPC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            UD_ADDRESSES_NIT_PC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                "                                            <COLL_ADDRESSES> \n" +
                                                  "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                  "                                                <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT> " +
                                                  "\n" +
                                                  "                                                <PROP_COUNTRY>" + PaisNitPC + @"</PROP_COUNTRY> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS1>" + TxtDiRe1PC + @"</PROP_ADDRESS1> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS2>" + TxtDiRe2PC + @"</PROP_ADDRESS2> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS3>" + TxtDiRe3PC + @"</PROP_ADDRESS3> " +
                                                  "\n" +
                                                  "                                                <PROP_STATE>" + StateNitPC + @"</PROP_STATE>  " +
                                                  "\n" +
                                                "                                            </COLL_ADDRESSES> \n" +
                                             "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }

                        auxConsulta = 2;
                        string consultaUP = "1";
                        string consultaUD = "1";
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }
                        auxConsulta = 3;
                        if (contadorUD > 0 && consultaUP == "1")
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            transaction.Commit();
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionPC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionPC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                        return "1";
                    }
                }
            }
        }
        private string ActualizarNITRC(string emplid)
        {
            string constr = TxtURL.Text;
            string existeNit = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO, " +
                                        "DEPARTAMENTO, SECOND_LAST_NAME, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, CNT FROM ( " +
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
                                        "WHERE PN.NATIONAL_ID ='" + TxtDpiRC.Text + "' " +
                                       ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        existeNit = reader["NIT"].ToString();
                    }

                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText =
                            "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                            "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                            "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + emplid + "' AND TIPO_PERSONA = 2";
                        OracleDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            TxtNombreRRC = reader2["NOMBRE_NIT"].ToString();
                            TxtApellidoRRC = reader2["APELLIDOS_NIT"].ToString();
                            TxtCasadaRRC = reader2["CASADA_NIT"].ToString();
                            TxtDiRe1RC = reader2["DIRECCION1_NIT"].ToString();
                            TxtDiRe2RC = reader2["DIRECCION2_NIT"].ToString();
                            TxtDiRe3RC = reader2["DIRECCION3_NIT"].ToString();
                            StateNitRC = reader2["STATE_NIT"].ToString();
                            PaisNitRC = reader2["PAIS_NIT"].ToString();
                            NITRC = reader2["NIT"].ToString();
                        }

                        int ContadorNombreNit = 0;
                        int ContadorEffdtNombreNit = 0;
                        int ContadorEffdtDirecionNit = 0;
                        string EffdtDireccionNitUltimo = "";
                        string EffdtNombreNitUltimo = "";
                        string EffdtNitUltimo = "";
                        int ContadorDirecionNit = 0;
                        int ContadorNit = 0;
                        int ContadorNit2 = 0;
                        string EFFDT_SYSTEM = "";
                        string EFFDT_AddressNit = "";

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND  EMPLID = '" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE = 'REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNombreNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtDireccionNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1RC + "' AND ADDRESS2 = '" + TxtDiRe2RC + "' AND ADDRESS3 = '" + TxtDiRe3RC + "' " +
                                              "AND COUNTRY='" + PaisNitRC + "' AND STATE ='" + StateNitRC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_AddressNit = reader["EFFDT"].ToString();
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'" +
                                                " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EffdtNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITRC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSKEY WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNit2 = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_SYSTEM = reader["CONTADOR"].ToString();
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "'" +
                                                "AND EFFDT ='" + HoyEffdt + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorEffdtNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRRC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRRC + "' AND SECOND_LAST_NAME='" + TxtCasadaRRC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_RC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();// + reader["EFFDT"].ToString().Substring(9, 2).TrimEnd();

                            if (EFFDT_A_NIT_RC.Value.Length == 9)
                            {
                                EFFDT_A_NIT_RC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A_NIT_RC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_NameR_RC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_NameR_RC.Value.Length == 9)
                            {
                                EFFDT_NameR_RC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_NameR_RC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        string FechaEfectiva = "";
                        if (EFFDT_NameR_RC.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR_RC.Value;

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            UP_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRRC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE

                            UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRRC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;

                        }
                        else
                        {//UPDATE

                            UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRRC + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }

                        //ACTUALIZA NIT
                        if (EffdtNitUltimo != Hoy && ContadorNit == 0)
                        {
                            //INSERTA EL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) VALUES ('" + emplid + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + NITRC + "')";
                            cmd.ExecuteNonQuery();

                            if (ContadorNit2 == 0)
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                "VALUES ('" + emplid + "','NRE')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (EffdtNitUltimo == Hoy && ContadorNit > 0)
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITRC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NITRC + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + EFFDT_SYSTEM.Substring(0, 10).TrimEnd() + "'";
                            cmd.ExecuteNonQuery();
                        }

                        if (EffdtDireccionNitUltimo != Hoy && ContadorDirecionNit == 0 && ContadorEffdtDirecionNit == 0)
                        {//INSERTA
                            UP_ADDRESSES_NIT_RC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitRC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1RC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2RC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3RC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitRC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtDireccionNitUltimo == Hoy && ContadorDirecionNit > 0 && ContadorEffdtDirecionNit > 0)
                        {//ACTUALIZA
                            UD_ADDRESSES_NIT_RC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                            "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                            "                                            <COLL_ADDRESSES> \n" +
                                              "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                              "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                              "\n" +
                                              "                                                <PROP_COUNTRY>" + PaisNitRC + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1RC + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2RC + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3RC + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNitRC + @"</PROP_STATE>  " +
                                              "\n" +
                                            "                                            </COLL_ADDRESSES> \n" +
                                         "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            UD_ADDRESSES_NIT_RC.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                "                                            <COLL_ADDRESSES> \n" +
                                                  "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                  "                                                <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT> " +
                                                  "\n" +
                                                  "                                                <PROP_COUNTRY>" + PaisNitRC + @"</PROP_COUNTRY> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS1>" + TxtDiRe1RC + @"</PROP_ADDRESS1> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS2>" + TxtDiRe2RC + @"</PROP_ADDRESS2> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS3>" + TxtDiRe3RC + @"</PROP_ADDRESS3> " +
                                                  "\n" +
                                                  "                                                <PROP_STATE>" + StateNitRC + @"</PROP_STATE>  " +
                                                  "\n" +
                                                "                                            </COLL_ADDRESSES> \n" +
                                             "                                        </COLL_ADDRESS_TYPE_VW> \n";
                            contadorUD = contadorUD + 1;
                        }

                        auxConsulta = 4;
                        string consultaUP = "1";
                        string consultaUD = "1";
                        if (contadorUP > 0)
                        {
                            consultaUP = Consultar();
                        }
                        auxConsulta = 5;
                        if (contadorUD > 0 && consultaUP == "1")
                        {
                            consultaUD = Consultar();
                        }

                        if (consultaUD == "1" && consultaUP == "1")
                        {
                            transaction.Commit();
                            con.Close();
                            return "0";
                        }
                        else
                        {
                            transaction.Rollback();
                            lblActualizacionRC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionRC.Text = "Ocurrió un problema al actualizar el NIT " + Variables.soapBody;
                        return "1";
                    }
                }
            }
        }
        protected string Upload(string Carnet)
        {
            string ImagenData = "";
            string constr = TxtURL.Text;
            int contador;
            byte[] imageBytes = null;

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + Carnet + "'";
                    OracleDataReader reader3 = cmd.ExecuteReader();
                    while (reader3.Read())
                    {
                        contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                        if (contador > 0)
                        {
                            if (ControlTabs.Value == "AC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/ACTUALIZACION-AC/" + Carnet + ".jpg");
                            }
                            if (ControlTabs.Value == "PC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                            }
                            if (ControlTabs.Value == "RC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                            }

                            string base64String = Convert.ToBase64String(imageBytes);
                            ImagenData = base64String;
                        }
                    }
                    con.Close();
                }
            }

            string mensaje = "";
            try
            {
                string EmplidFoto = Carnet;
                string EmplidExisteFoto = "";
                string mensajeValidacion = "";

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
                    }
                }
                byte[] bytes = Convert.FromBase64String(ImagenData);
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
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
                            mensaje = "<br/>La fotografía fue almacenada correctamente.";
                        }

                        cmd.Connection = con;
                        cmd.Parameters.Add(new OracleParameter("Fotografia", bytes));
                        cmd.ExecuteNonQuery();

                    }
                }
                mensaje = "0";
            }
            catch (Exception X)
            {
                mensaje = ". Ocurrió un error al cargar la imagen";
                mensaje = "1";
            }
            return mensaje;
        }
        public void GuardarBitacora(string ArchivoBitacora, string DescripcionBitacora)
        {
            //Función para guardar bitacora en el archivo .txt
            //Guarda nueva línea para el registro de bitácora en el serividor
            File.AppendAllText(ArchivoBitacora, DescripcionBitacora + Environment.NewLine);
        }
        public void CrearArchivoBitacora(string archivoBitacora, string FechaHoraEjecución)
        {
            //Crea un archivo .txt para guardar bitácora
            StreamWriter sw = File.CreateText(archivoBitacora);
        }
        protected int ControlRenovacion(string cadena)
        {
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
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_CONTROL_CARNET " + cadena;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONTADOR"].ToString();
                        }
                        con.Close();
                    }
                    catch (Exception)
                    {
                        control = "3";
                    }
                }
            }
            return Convert.ToInt32(control);
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
        public void EnvioCorreo(string body, string subject, string para, string emailInstitu)
        {

            string htmlBody = LeerBodyEmail(body);
            string[] datos = LeerInfoEmail(subject);
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            //var para = TxtPrimerNombre.Text + " " + TxtPrimerApellido.Text;

            email.From.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.To.Add(new MailboxAddress(para, emailInstitu));
            //email.To.Add(new MailboxAddress(para, TxtCorreoInstitucional.Text));

            email.Subject = datos[0];
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    //smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(credenciales[1], credenciales[2]);

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                catch (Exception ex)
                {
                    lblActualizacionAC.Text = ex.ToString();
                }
            }

        }

        //EVENTOS
        protected void BtnRechazarAC_Click(object sender, EventArgs e)
        {
            RechazarAC(CmbCarneAC.Text);
        }
        protected void BtnRechazarPC_Click(object sender, EventArgs e)
        {
            RechazarPC(CmbCarnePC.Text);
        }
        protected void BtnRechazarRC_Click(object sender, EventArgs e)
        {
            RechazarRC(CmbCarneRC.Text);
        }
        protected void BtnConfirmarAC_Click(object sender, EventArgs e)
        {
            string carne = CmbCarneAC.Text;
            ConfirmarAC(carne);
        }
        protected void BtnConfirmarPC_Click(object sender, EventArgs e)
        {
            string carne = CmbCarnePC.Text;
            ConfirmarPC(carne);
        }
        protected void BtnConfirmarRC_Click(object sender, EventArgs e)
        {
            string carne = CmbCarneRC.Text;
            ConfirmarRC(carne);
        }

        protected void CmbTipo_SelectedIndexChangedRC(object sender, EventArgs e)
        {
            llenadoRC("CARNET = '" + CmbCarneRC.Text + "'");
            if (txtCantidadRC.Text != "0" && !txtCantidadRC.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidadRC.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1RC.Visible = true;
                        ImgDPI1RC.ImageUrl = "~/Usuarios/DPI/" + CmbCarneRC.Text + "(" + (i + 1) + ").jpg";
                    }
                    if (i == 1)
                    {
                        ImgDPI2RC.Visible = true;
                        ImgDPI2RC.ImageUrl = "~/Usuarios/DPI/" + CmbCarneRC.Text + "(" + (i + 1) + ").jpg";
                    }
                }
                if (txtCantidadRC.Text == "1")
                {
                    ImgDPI2RC.Visible = false;
                }
            }
            else
            {
                ImgDPI1RC.Visible = false;
                ImgDPI2RC.Visible = false;
                ImgFoto1RC.Visible = false;
            }
            if (!CmbCarneRC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionRC.Text = null;
            }
            HFoto.Visible = true;
            ImgFoto1RC.ImageUrl = "~/Usuarios/FotosConfirmacion/RENOVACION_CARNE-RC/" + CmbCarneRC.Text + ".jpg";
        }
        protected void CmbTipo_SelectedIndexChangedPC(object sender, EventArgs e)
        {
            llenadoPC("CARNET = '" + CmbCarnePC.Text + "'");
            if (txtCantidadPC.Text != "0" && !txtCantidadPC.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidadPC.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1PC.Visible = true;
                        ImgDPI1PC.ImageUrl = "~/Usuarios/DPI/" + CmbCarnePC.Text + "(" + (i + 1) + ").jpg";
                    }
                    if (i == 1)
                    {
                        ImgDPI2PC.Visible = true;
                        ImgDPI2PC.ImageUrl = "~/Usuarios/DPI/" + CmbCarnePC.Text + "(" + (i + 1) + ").jpg";
                    }
                }
                if (txtCantidadPC.Text == "1")
                {
                    ImgDPI2PC.Visible = false;
                }
            }
            else
            {
                ImgDPI1PC.Visible = false;
                ImgDPI2PC.Visible = false;
                ImgFoto1PC.Visible = false;
            }
            if (!CmbCarnePC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionPC.Text = null;
            }
            HFoto.Visible = true;
            ImgFoto1PC.ImageUrl = "~/Usuarios/FotosConfirmacion/PRIMER_CARNET-PC/" + CmbCarnePC.Text + ".jpg";
        }
        protected void CmbTipo_SelectedIndexChangedAC(object sender, EventArgs e)
        {
            llenadoAC("CARNET = '" + CmbCarneAC.Text + "'");
            if (txtCantidadAC.Text != "0" && !txtCantidadAC.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidadAC.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1AC.Visible = true;
                        ImgDPI1AC.ImageUrl = "~/Usuarios/DPI/" + CmbCarneAC.Text + "(" + (i + 1) + ").jpg";
                    }
                    if (i == 1)
                    {
                        ImgDPI2AC.Visible = true;
                        ImgDPI2AC.ImageUrl = "~/Usuarios/DPI/" + CmbCarneAC.Text + "(" + (i + 1) + ").jpg";
                    }
                }
                if (txtCantidadAC.Text == "1")
                {
                    ImgDPI2AC.Visible = false;
                }
            }
            else
            {
                ImgDPI1AC.Visible = false;
                ImgDPI2AC.Visible = false;
                ImgFoto1AC.Visible = false;
            }
            if (!CmbCarneAC.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionAC.Text = null;
            }
            HFoto.Visible = true;
            ImgFoto1AC.ImageUrl = "~/Usuarios/FotosConfirmacion/ACTUALIZACION-AC/" + CmbCarneAC.Text + ".jpg";
        }

        // Evento cuando se hace clic en la Tab 1
        protected void Tab1_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 0;
            ControlTabs.Value = "AC";
            lblActualizacionAC.Text = "";
            BuscarAC("1");
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(0);
        }

        // Evento cuando se hace clic en la Tab 2
        protected void Tab2_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 1;
            ControlTabs.Value = "PC";
            BuscarPC("1");
            lblActualizacionPC.Text = "";
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(1);
        }

        // Evento cuando se hace clic en la Tab 3
        protected void Tab3_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 2;
            ControlTabs.Value = "RC";
            lblActualizacionRC.Text = "";
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(2);
        }
        private void SetActiveTab(int tabIndex)
        {
            // Restablecer todos los estilos de las pestañas a "Initial"
            Tab1.CssClass = "Initial";
            Tab2.CssClass = "Initial";
            Tab3.CssClass = "Initial";

            // Establecer la pestaña activa según el índice
            if (tabIndex == 0)
            {
                Tab1.CssClass = "Clicked";
                MainView.ActiveViewIndex = 0;
            }
            else if (tabIndex == 1)
            {
                Tab2.CssClass = "Clicked";
                MainView.ActiveViewIndex = 1;
            }
            else if (tabIndex == 2)
            {
                Tab3.CssClass = "Clicked";
                MainView.ActiveViewIndex = 2;
            }
        }

        private void log(string ErrorLog, string carnet)
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
                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + carnet + "','" + ErrorLog + "','CONFIRMACIÓN DATOS SENSIBLES ESTUDIANTES',SYSDATE)";
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                }
            }
        }

        /*-------------------------------------------INICIAN FUNCIONES PARA METODO SOAP-------------------------------------------*/
        private static void limpiarVariables()
        {
            //Función para limpiar variables
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
        private static void credencialesEndPoint(string RutaConfiguracion, string strMetodo)
        {
            //Función para obtener información de acceso al servicio de Campus
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
        private static XmlDocument CreateSoapEnvelope(string xmlString)
        {
            //Función para crear el elemento raíz para solicitud web 
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(xmlString);
            return soapEnvelopeDocument;
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            //Función para crear el encabezado para la Solicitud web
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            //Función para crear unificar toda la estructura de la solicitud web
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        public string LlamarWebServiceCampus(string _url, string _action, string _xmlString)
        {
            //Función para llamar un servicio web de Campus
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
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarneAC.SelectedValue, UP_NAMES_PRI_AC.Value, UP_NAMES_PRF_AC.Value, UP_NAMES_NIT_AC.Value, UP_ADDRESSES_NIT_AC.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarneAC.SelectedValue, UD_NAMES_PRI_AC.Value, UD_NAMES_PRF_AC.Value, UD_NAMES_NIT_AC.Value, UD_ADDRESSES_NIT_AC.Value);
            }else if (auxConsulta == 2)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarnePC.SelectedValue, UP_NAMES_PRI_PC.Value, UP_NAMES_PRF_PC.Value, UP_NAMES_NIT_PC.Value, UP_ADDRESSES_NIT_PC.Value);
            }else if (auxConsulta == 3)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarnePC.SelectedValue, UD_NAMES_PRI_PC.Value, UD_NAMES_PRF_PC.Value, UD_NAMES_NIT_PC.Value, UD_ADDRESSES_NIT_PC.Value);
            }else if (auxConsulta == 4)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarneRC.SelectedValue, UP_NAMES_PRI_RC.Value, UP_NAMES_PRF_RC.Value, UP_NAMES_NIT_RC.Value, UP_ADDRESSES_NIT_RC.Value);
            }
            else if (auxConsulta == 5)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarneRC.SelectedValue, UD_NAMES_PRI_RC.Value, UD_NAMES_PRF_RC.Value, UD_NAMES_NIT_RC.Value, UD_ADDRESSES_NIT_RC.Value);
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

            try
            {
                XmlNodeList elemList = xmlDocumentoRespuestaCampus.GetElementsByTagName("notification");
                //return elemList[0].InnerText.ToString();
                return elemList[0].InnerText.ToString();
            }
            catch
            {
                return "0";
            }
        }
        private static void CuerpoConsultaUD(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRI, string COLL_NAMES_PRF, string COLL_NAMES_NIT, string COLL_ADDRESSES_NIT)
        {
            //Crea el cuerpo que se utiliza para hacer PATCH
            Variables.soapBody = @"<?xml version=""1.0""?>
                                 <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:m64=""http://xmlns.oracle.com/Enterprise/Tools/schemas/M644328134.V1"">
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
                                         " + COLL_NAMES_PRI + @"
                                         " + COLL_NAMES_PRF + @"
                                         " + COLL_NAMES_NIT + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                      </Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUP(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRI, string COLL_NAMES_PRF, string COLL_NAMES_NIT, string COLL_ADDRESSES_NIT)
        {
            //Crea el cuerpo que se utiliza para hacer POST
            Variables.soapBody = @"<?xml version=""1.0""?>
                                 <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:m64=""http://xmlns.oracle.com/Enterprise/Tools/schemas/M780623797.V1"">
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
                                         " + COLL_NAMES_PRI + @"
                                         " + COLL_NAMES_PRF + @"
                                         " + COLL_NAMES_NIT + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                      </Update__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
    }
}