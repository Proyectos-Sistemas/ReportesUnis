using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Net;
using System.Web.Services;
using System.Xml;
using ReportesUnis.API;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using NPOI.Util;
using DocumentFormat.OpenXml.Bibliography;


namespace ReportesUnis
{
    public partial class ConfirmaciónCarneEmpleados : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /*VARIABLES PARA ACTUALIZACION*/
        string TxtNombreRAC = "";
        string TxtApellidoRAC = "";
        string TxtCasadaRAC = "";
        string NITAC = "";
        string TxtDiRe1AC = "";
        string TxtDiRe2AC = "";
        string TxtDiRe3AC = "";
        string StateNitAC = "";
        string PaisNitAC = "";
        string Direccion1AC = "";
        string Direccion2AC = "";
        string Direccion3AC = "";

        /*VARIABLES PARA PRIMER CARNET*/
        string TxtNombreRPC = "";
        string TxtApellidoRPC = "";
        string TxtCasadaRPC = "";
        string NITPC = "";
        string TxtDiRe1PC = "";
        string TxtDiRe2PC = "";
        string TxtDiRe3PC = "";
        string StateNitPC = "";
        string PaisNitPC = "";
        string Direccion1PC = "";
        string Direccion2PC = "";
        string Direccion3PC = "";

        /*VARIABLES PARA RENOVACION DE CARNET*/
        string TxtNombreRRC = "";
        string TxtApellidoRRC = "";
        string TxtCasadaRRC = "";
        string NITRC = "";
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
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();

        // CONSUMO DE API
        ConsumoAPI api = new ConsumoAPI();
        int respuestaPatch = 0;
        int respuestaPost = 0;

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
                LeerVersionesSOAPCampus();
                txtControlNR.Text = "0";
                txtControlAR.Text = "0";

                //PARA TAB ACTUALIZACION
                LimpiarCamposAC();
                divCamposAC.Visible = true;
                divDPIAC.Visible = true;
                divFotografiaAC.Visible = true;
                divBtnConfirmarAC.Visible = true;
                BuscarAC("1");
                lblActualizacionAC.Text = null;
               

                //PARA TAB ACTUALIZACION
                LimpiarCamposPC();
                divCamposPC.Visible = true;
                divDPIPC.Visible = true;
                divFotografiaPC.Visible = true;
                divBtnConfirmarPC.Visible = true;
                BuscarPC("1");
                lblActualizacionPC.Text = null;


                //PARA TAB ACTUALIZACION
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

        //FUNCIONES ACTUALIZACION
        private void LlenadoAC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' DEPTO,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL, ' ' TIPO_PERSONA, ' ' ROLES, ' ' EMPLID FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CUI, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, DEPTO, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL, " +
                        "CASE WHEN TIPO_PERSONA = '3' THEN 'Docente' WHEN TIPO_PERSONA = '1' THEN 'Administrativo' ELSE 'Estudiante' END TIPO_PERSONA, ROLES, EMPLID " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiAC.Text = reader["CUI"].ToString();
                        if (TxtDpiAC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiAC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombreAC.Text = reader["NOMBRE1"].ToString();
                        TxtSegundoNombreAC.Text = reader["NOMBRE2"].ToString();
                        TxtPrimerApellidoAC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoAC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaAC.Text = reader["DECASADA"].ToString();
                        TxtPuestoAC.Text = reader["CARGO"].ToString();
                        TxtFacultadAC.Text = reader["DEPTO"].ToString();
                        TxtFechaNacAC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoAC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionAC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoAC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioAC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelAC.Text = reader["CELULAR"].ToString();
                        txtCantidad.Text = reader["TOTALFOTOS"].ToString();
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
                        EmailInstitucional.Value = reader["EMAIL"].ToString();
                        TxtCorreoPersonalAC.Text = reader["EMAIL_PERSONAL"].ToString();
                        TxtRolAC.Text = reader["TIPO_PERSONA"].ToString();
                        ROLES.Value = reader["ROLES"].ToString();
                        UserEmplid.Value = reader["EMPLID"].ToString();
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
            TxtPuestoAC.Text = null;
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
            txtCantidad.Text = null;
            TxtPaisAC.Text = null;
            TxtCorreoInstitucionalAC.Text = null;
            TxtCorreoPersonalAC.Text = null;
            TxtRolAC.Text = null;
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
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarAC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/UltimasCargas/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEmpleados.txt", "datosRechazoEmpleados.txt", TxtPrimerNombreAC.Text, TxtPrimerApellidoAC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- AC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
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
        protected void ConfirmarAC(string Carnet)
        {
            if (!TxtPrimerNombreAC.Text.IsNullOrWhiteSpace())
            {
                LlenadoAC("CODIGO = '" + Carnet + "' AND CONTROL_ACCION = 'AC' ");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");

                if (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor"))
                    respuesta = QueryActualizaNombreAC(Carnet);

                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == null)
                    respuesta = "0";

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    if (ROLES.Value.Contains("Estudiante"))
                    {
                        respuesta = ActualizarNITAC(CmbCarneAC.Text);
                    }
                    if (respuesta == "0")
                    {
                        respuesta = ServiciosHCM_AC();
                        if (respuesta == "0")
                        {
                            respuesta = "";
                            QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet, "AC");
                            if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                            {
                                respuesta = ConsumoOracleAC(txtInsertApex.Text);
                                if (respuesta == "0")
                                {
                                    if (respuesta == "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
                                    {
                                        Upload(Carnet);
                                    }
                                    else if (respuesta != "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
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
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- AC", Carnet);
                        }
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- AC", Carnet);

                        }
                    }

                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionAC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEmpleados.txt", "datosConfirmacionEmpleados.txt", TxtPrimerNombreAC.Text + " " + TxtPrimerApellidoAC.Text, TxtCorreoInstitucionalAC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- AC", Carnet);
                        BuscarAC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/ACTUALIZACION-AC/" + Carnet + ".jpg");
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/ACTUALIZACION-AC/" + Carnet + ".jpg");
                        LimpiarCamposAC();
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus del carnet: " + Carnet + "- AC", Carnet);
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- AC", Carnet);
                        }
                        lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQLAC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                    }
                }
                else
                {
                    lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- AC", Carnet);
                    ConsumoSQLAC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionAC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        public string ServiciosHCM_AC()
        {
            string constr = TxtURL.Text;
            int contador;
            //Obtener se obtiene toda la información del empleado
            string expand = "names,photos";
            string consulta = consultaGetworkers(expand, "nationalIdentifiers");

            //Se obtienen los id's de las tablas a las cuales se les agregará información
            string personId = getBetween(consulta, "workers/", "/child/");
            string comIm = personId + "/child/photo/";
            string consultaImagenes = consultaGetImagenes(comIm);
            string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
            string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");
            string base64String = "";
            string PersonNameId = getBetween(consulta, "\"PersonNameId\" : ", ",\n");
            string effectivePerson = getBetween(consulta, PersonNameId + ",\n      \"EffectiveStartDate\" : \"", "\",\n");
            string hrefName = getBetween(consulta, "\n      \"LocalNameInformation30\" :", "\n        \"name\" : \"names\",");
            hrefName = getBetween(hrefName, "/child/names/", "\"");
            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + CmbCarneAC.SelectedValue + "'";
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                            if (contador > 0)
                            {
                                byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/ACTUALIZACION-AC/" + CmbCarneAC.SelectedValue + ".jpg");
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                        con.Close();
                    }
                }

                //ACTUALIZACION-CREACION DE FOTOGRAFIA
                string pid = getBetween(consulta, "\"PhotoId\" :", ",");
                string consultaperfil = pid + ",\n      \"PrimaryFlag\" : ";
                string perfil = getBetween(consulta, consultaperfil, ",\n");
                var Imgn = "{\"ImageName\" : \"" + TxtDpiAC.Text + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + base64String + "\"}";
                string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
                string name = "{\"LastName\" : \"" + TxtPrimerApellidoAC.Text + "\",\"FirstName\": \"" + TxtPrimerNombreAC.Text + "\",\"MiddleNames\": \"" + TxtSegundoNombreAC.Text + "\"," +
                    "\"PreviousLastName\": \"" + TxtApellidoCasadaAC.Text + "\",\"NameInformation1\": \"" + TxtSegundoApellidoAC.Text + "\",\"LegislationCode\": \"GT\"}";
                if (perfil == "true" && ImageId != "")
                {
                    updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                }
                else
                {
                    create(personId, "photo", Imgn, "emps/");
                }

                //ACTUALIZAR NOMBRE
                if (respuestaPatch == 0 && respuestaPost == 0)
                {
                    updatePatch(name, personId, "names", hrefName, "names", Hoy, "workers/");
                }

                if (respuestaPatch == 0)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception)
            {
                return "1";
            }
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
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + emplid + "' AND TIPO_PERSONA != 2";
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
                        string ApellidoAnterior = "";
                        string ApellidoCAnterior = "";

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

                        if (EffdtDireccionNitUltimo != "" && !String.IsNullOrEmpty(EffdtDireccionNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1AC + "' AND ADDRESS2 = '" + TxtDiRe2AC + "' AND ADDRESS3 = '" + TxtDiRe3AC + "' " +
                                              "AND COUNTRY='" + PaisNitAC + "' AND STATE ='" + StateNitAC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNitUltimo != "" && !String.IsNullOrEmpty(EffdtNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITAC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNombreNitUltimo != "" && !String.IsNullOrEmpty(EffdtNombreNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRAC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRAC + "' AND SECOND_LAST_NAME='" + TxtCasadaRAC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                            }

                            cmd.CommandText = "SELECT LAST_NAME , SECOND_LAST_NAME FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ApellidoAnterior = reader["LAST_NAME"].ToString();
                                ApellidoCAnterior = reader["SECOND_LAST_NAME"].ToString();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_AC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

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

                        TxtApellidoRAC = System.Text.RegularExpressions.Regex.Replace(TxtApellidoRAC, @"\s+", " "); ;
                        TxtNombreRAC = System.Text.RegularExpressions.Regex.Replace(TxtNombreRAC, @"\s+", " "); ;
                        TxtCasadaRAC = System.Text.RegularExpressions.Regex.Replace(TxtCasadaRAC, @"\s+", " ");

                        string FechaEfectiva = "";
                        if (EFFDT_NameR_AC.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR_AC.Value;

                        UP_NAMES_PRI_AC.Value = "";
                        UP_NAMES_PRF_AC.Value = "";
                        UD_NAMES_PRI_AC.Value = "";
                        UD_NAMES_PRF_AC.Value = "";

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            if (!TxtApellidoRAC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRAC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UP_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                                }
                            }
                            else
                            {
                                UP_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE
                            if (!TxtApellidoRAC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRAC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRAC + "," + TxtNombreRAC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRAC + "," + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtApellidoRAC + "," + TxtNombreRAC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRAC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtNombreRAC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRAC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtNombreRAC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }
                            }
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            if (!TxtApellidoRPC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRPC.IsNullOrWhiteSpace())
                                {
                                    UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRAC + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRAC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRAC + "," + TxtNombreRAC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRAC + "," + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtApellidoRAC + "," + TxtNombreRAC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_AC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRAC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }

                            if (!ApellidoAnterior.IsNullOrWhiteSpace())
                            {
                                //ACTUALIZA NIT
                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRAC + "', " +
                                    "PN.NAME_FORMAL ='" + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtNombreRAC + "' " +
                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                            }

                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                            {
                                //ACTUALIZA NIT
                                txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRAC + "', " +
                                    "PN.NAME_FORMAL ='" + TxtNombreRAC + "', PN.NAME_DISPLAY ='" + TxtNombreRAC + "' " +
                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                            }

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
                        if (contadorUD > 0)
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
                            lblActualizacionAC.Text = "Ocurrió un problema al actualizar el NIT ";
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionAC.Text = "Ocurrió un problema al actualizar el NIT ";
                        return "1";
                    }
                }
            }
        }
        protected string QueryActualizaNombreAC(string emplid)
        {
            //EN CAMPUS
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoAC.Text + " " + TxtSegundoApellidoAC.Text + " " + TxtApellidoCasadaAC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombreAC.Text + " " + TxtSegundoNombreAC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoAC.Text + " " + TxtSegundoApellidoAC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaAC.Text;
            string EFFDT_Name = "";

            if (Direccion1AC == "")
            {
                Direccion1AC = " ";
            }
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
        protected string ConsumoOracleAC(string ComandoConsulta)
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
        protected string ConsumoSQLAC(string Consulta)
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

        //FUNCIONES PRIMER CARNET
        private void LlenadoPC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' DEPTO,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL, ' ' TIPO_PERSONA, ' ' ROLES, ' ' EMPLID FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CUI, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, DEPTO, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL, " +
                        "CASE WHEN TIPO_PERSONA = '3' THEN 'Docente' WHEN TIPO_PERSONA = '1' THEN 'Administrativo' ELSE 'Estudiante' END TIPO_PERSONA, ROLES, EMPLID " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiPC.Text = reader["CUI"].ToString();
                        if (TxtDpiPC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiPC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombrePC.Text = reader["NOMBRE1"].ToString();
                        TxtSegundoNombrePC.Text = reader["NOMBRE2"].ToString();
                        TxtPrimerApellidoPC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoPC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaPC.Text = reader["DECASADA"].ToString();
                        TxtPuestoPC.Text = reader["CARGO"].ToString();
                        TxtFacultadPC.Text = reader["DEPTO"].ToString();
                        TxtFechaNacPC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoPC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionPC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoPC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioPC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelPC.Text = reader["CELULAR"].ToString();
                        txtCantidad.Text = reader["TOTALFOTOS"].ToString();
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
                        EmailInstitucional.Value = reader["EMAIL"].ToString();
                        TxtCorreoPersonalPC.Text = reader["EMAIL_PERSONAL"].ToString();
                        TxtRolPC.Text = reader["TIPO_PERSONA"].ToString();
                        ROLES.Value = reader["ROLES"].ToString();
                        UserEmplid.Value = reader["EMPLID"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void LimpiarCamposPC()
        {
            TxtDpiPC.Text = null;
            TxtPrimerNombrePC.Text = null;
            TxtSegundoNombrePC.Text = null;
            TxtPrimerApellidoPC.Text = null;
            TxtSegundoApellidoPC.Text = null;
            TxtApellidoCasadaPC.Text = null;
            TxtPuestoPC.Text = null;
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
            txtCantidad.Text = null;
            TxtPaisPC.Text = null;
            TxtCorreoInstitucionalPC.Text = null;
            TxtCorreoPersonalPC.Text = null;
            TxtRolPC.Text = null;
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
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_CONTROL_CARNET WHERE EMPLID = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarPC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEmpleados.txt", "datosRechazoEmpleados.txt", TxtPrimerNombrePC.Text, TxtPrimerApellidoPC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- PC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
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
                            log("No se pudo eliminar la información a causa de un error interno. " + x + "- PC", Carnet);
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
        protected void ConfirmarPC(string Carnet)
        {
            if (!TxtPrimerNombrePC.Text.IsNullOrWhiteSpace())
            {
                LlenadoPC("CODIGO = '" + Carnet + "' AND CONTROL_ACCION ='PC' ");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi(CmbCarnePC.SelectedValue);

                if (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor"))
                    respuesta = QueryActualizaNombrePC(Carnet);

                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == null)
                    respuesta = "0";

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    if (ROLES.Value.Contains("Estudiante"))
                    {
                        respuesta = ActualizarNITPC(CmbCarnePC.Text);
                    }
                    if (respuesta == "0")
                    {
                        respuesta = ServiciosHCM_PC();
                        if (respuesta == "0")
                        {
                            respuesta = "";
                            QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet, "PC");
                            if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                            {
                                //SE INGRESA LA INFORMACIÓN EN EL BANCO
                                respuesta = ConsumoSQLPC(txtInsertBI.Text.ToUpper());
                                if (respuesta == "0")
                                {
                                    respuesta = ConsumoOraclePC(txtInsertApex.Text);
                                    if (respuesta == "0")
                                    {

                                        if (controlRenovacion == 0)
                                        {
                                            //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                            respuesta = ConsumoOraclePC("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                        }
                                        else
                                        {
                                            if (controlRenovacionFecha < 2)
                                            {
                                                //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                                respuesta = ConsumoOraclePC("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion++) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                            }
                                            else
                                            {
                                                respuesta = "0";
                                            }

                                            if (respuesta == "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
                                            {
                                                Upload(Carnet);
                                            }
                                            else if (respuesta != "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
                                            {
                                                log("ERROR - Actualizacion de fotografia en campus del carnet PC " + Carnet, Carnet);
                                            }
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
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- PC", Carnet);
                        }
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- PC", Carnet);

                        }
                    }

                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionPC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEmpleados.txt", "datosConfirmacionEmpleados.txt", TxtPrimerNombrePC.Text, TxtPrimerApellidoPC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- PC", Carnet);
                        BuscarPC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                        LimpiarCamposPC();
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus del carnet: " + Carnet + "- PC", Carnet);
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- PC", Carnet);
                        }
                        lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQLPC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                    }
                }
                else
                {
                    lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- PC", Carnet);
                    ConsumoSQLPC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionPC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        public string ServiciosHCM_PC()
        {
            string constr = TxtURL.Text;
            int contador;
            //Obtener se obtiene toda la información del empleado
            string expand = "names,photos";
            string consulta = consultaGetworkers(expand, "nationalIdentifiers");

            //Se obtienen los id's de las tablas a las cuales se les agregará información
            string personId = getBetween(consulta, "workers/", "/child/");
            string comIm = personId + "/child/photo/";
            string consultaImagenes = consultaGetImagenes(comIm);
            string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
            string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");
            string base64String = "";
            string PersonNameId = getBetween(consulta, "\"PersonNameId\" : ", ",\n");
            string effectivePerson = getBetween(consulta, PersonNameId + ",\n      \"EffectiveStartDate\" : \"", "\",\n");
            string hrefName = getBetween(consulta, "\n      \"LocalNameInformation30\" :", "\n        \"name\" : \"names\",");
            hrefName = getBetween(hrefName, "/child/names/", "\"");
            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + CmbCarnePC.SelectedValue + "'";
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                            if (contador > 0)
                            {
                                byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/PRIMER_CARNET-PC/" + CmbCarnePC.SelectedValue + ".jpg");
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                        con.Close();
                    }
                }

                //ACTUALIZACION-CREACION DE FOTOGRAFIA
                string pid = getBetween(consulta, "\"PhotoId\" :", ",");
                string consultaperfil = pid + ",\n      \"PrimaryFlag\" : ";
                string perfil = getBetween(consulta, consultaperfil, ",\n");
                var Imgn = "{\"ImageName\" : \"" + TxtDpiPC.Text + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + base64String + "\"}";
                string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
                string name = "{\"LastName\" : \"" + TxtPrimerApellidoPC.Text + "\",\"FirstName\": \"" + TxtPrimerNombrePC.Text + "\",\"MiddleNames\": \"" + TxtSegundoNombrePC.Text + "\"," +
                    "\"PreviousLastName\": \"" + TxtApellidoCasadaPC.Text + "\",\"NameInformation1\": \"" + TxtSegundoApellidoPC.Text + "\",\"LegislationCode\": \"GT\"}";
                if (perfil == "true" && ImageId != "")
                {
                    updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                }
                else
                {
                    create(personId, "photo", Imgn, "emps/");
                }

                //ACTUALIZAR NOMBRE
                if (respuestaPatch == 0 && respuestaPost == 0)
                {
                    updatePatch(name, personId, "names", hrefName, "names", Hoy, "workers/");
                }

                if (respuestaPatch == 0)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception)
            {
                return "1";
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
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + emplid + "' AND TIPO_PERSONA != 2";
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
                        string ApellidoAnterior = "";
                        string ApellidoCAnterior = "";

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

                        if (EffdtDireccionNitUltimo != "" && !String.IsNullOrEmpty(EffdtDireccionNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1PC + "' AND ADDRESS2 = '" + TxtDiRe2PC + "' AND ADDRESS3 = '" + TxtDiRe3PC + "' " +
                                              "AND COUNTRY='" + PaisNitPC + "' AND STATE ='" + StateNitPC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNitUltimo != "" && !String.IsNullOrEmpty(EffdtNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITPC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNombreNitUltimo != "" && !String.IsNullOrEmpty(EffdtNombreNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRPC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRPC + "' AND SECOND_LAST_NAME='" + TxtCasadaRPC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                            }

                            cmd.CommandText = "SELECT LAST_NAME , SECOND_LAST_NAME FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ApellidoAnterior = reader["LAST_NAME"].ToString();
                                ApellidoCAnterior = reader["SECOND_LAST_NAME"].ToString();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_PC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

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

                        TxtApellidoRPC = System.Text.RegularExpressions.Regex.Replace(TxtApellidoRPC, @"\s+", " "); ;
                        TxtNombreRPC = System.Text.RegularExpressions.Regex.Replace(TxtNombreRPC, @"\s+", " "); ;
                        TxtCasadaRPC = System.Text.RegularExpressions.Regex.Replace(TxtCasadaRPC, @"\s+", " ");

                        string FechaEfectiva = "";
                        if (EFFDT_NameR_PC.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR_PC.Value;

                        UD_NAMES_PRI_PC.Value = "";
                        UD_NAMES_PRF_PC.Value = "";
                        UP_NAMES_PRI_PC.Value = "";
                        UP_NAMES_PRF_PC.Value = "";

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            if (!TxtApellidoRPC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRPC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UP_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                                }
                            }
                            else
                            {
                                UP_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                            }
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE
                            if (!TxtApellidoRPC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRPC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRPC + "," + TxtNombreRPC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRPC + "," + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtApellidoRPC + "," + TxtNombreRPC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRPC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtNombreRPC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRPC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtNombreRPC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }
                            }
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            if (!TxtApellidoRPC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRPC.IsNullOrWhiteSpace())
                                {
                                    UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRPC + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRPC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRPC + "," + TxtNombreRPC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRPC + "," + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtApellidoRPC + "," + TxtNombreRPC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_PC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRPC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRPC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtNombreRPC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRPC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRPC + "', PN.NAME_DISPLAY ='" + TxtNombreRPC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }
                            }
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
                        if (contadorUD > 0)
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
                            lblActualizacionPC.Text = "Ocurrió un problema al actualizar el NIT ";
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionPC.Text = "Ocurrió un problema al actualizar el NIT ";
                        return "1";
                    }
                }
            }
        }
        protected string QueryActualizaNombrePC(string emplid)
        {
            //EN CAMPUS
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoPC.Text + " " + TxtSegundoApellidoPC.Text + " " + TxtApellidoCasadaPC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombrePC.Text + " " + TxtSegundoNombrePC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoPC.Text + " " + TxtSegundoApellidoPC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaPC.Text;
            string EFFDT_Name = "";

            if (Direccion1PC == "")
            {
                Direccion1PC = " ";
            }
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
                            // ACTUALIZAR
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
        protected string ConsumoOraclePC(string ComandoConsulta)
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
                        lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información " + x;
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }
        protected string ConsumoSQLPC(string Consulta)
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
                        TxtEstadoPC.Text += x.ToString();
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }

        //FUNCIONES RENOVACION CARNET
        private void LlenadoRC(string where)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CUI,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' DEPTO,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE,  " +
                        "' ' ADDRESS1, ' ' ADDRESS2, ' ' ADDRESS3, ' ' EMAIL_PERSONAL, ' ' EMAIL, ' ' TIPO_PERSONA, ' ' ROLES, ' ' EMPLID FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CUI, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, DEPTO, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE,  ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMAIL, " +
                        "CASE WHEN TIPO_PERSONA = '3' THEN 'Docente' WHEN TIPO_PERSONA = '1' THEN 'Administrativo' ELSE 'Estudiante' END TIPO_PERSONA, ROLES, EMPLID " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpiRC.Text = reader["CUI"].ToString();
                        if (TxtDpiRC.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpiRC.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombreRC.Text = reader["NOMBRE1"].ToString();
                        TxtSegundoNombreRC.Text = reader["NOMBRE2"].ToString();
                        TxtPrimerApellidoRC.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellidoRC.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasadaRC.Text = reader["DECASADA"].ToString();
                        TxtPuestoRC.Text = reader["CARGO"].ToString();
                        TxtFacultadRC.Text = reader["DEPTO"].ToString();
                        TxtFechaNacRC.Text = reader["FECHANAC"].ToString();
                        TxtEstadoRC.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccionRC.Text = reader["DIRECCION"].ToString();
                        TxtDepartamentoRC.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipioRC.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTelRC.Text = reader["CELULAR"].ToString();
                        txtCantidad.Text = reader["TOTALFOTOS"].ToString();
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
                        EmailInstitucional.Value = reader["EMAIL"].ToString();
                        TxtCorreoPersonalRC.Text = reader["EMAIL_PERSONAL"].ToString();
                        TxtRolRC.Text = reader["TIPO_PERSONA"].ToString();
                        ROLES.Value = reader["ROLES"].ToString();
                        UserEmplid.Value = reader["EMPLID"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void LimpiarCamposRC()
        {
            TxtDpiRC.Text = null;
            TxtPrimerNombreRC.Text = null;
            TxtSegundoNombreRC.Text = null;
            TxtPrimerApellidoRC.Text = null;
            TxtSegundoApellidoRC.Text = null;
            TxtApellidoCasadaRC.Text = null;
            TxtPuestoRC.Text = null;
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
            txtCantidad.Text = null;
            TxtPaisRC.Text = null;
            TxtCorreoInstitucionalRC.Text = null;
            TxtCorreoPersonalRC.Text = null;
            TxtRolRC.Text = null;
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
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + Carnet + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                BuscarRC("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEmpleados.txt", "datosRechazoEmpleados.txt", TxtPrimerNombreRC.Text, TxtPrimerApellidoRC.Text);
                                log("La información fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- RC", Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                                lblActualizacionRC.Text = "Se ha rechazado la solicitud de carnet.";
                            }
                            else
                            {
                                lblActualizacionRC.Text = "Ocurrió un error al rechazar la solicitud";
                                log("Ocurrió un error al eliminar la fotografía RC", Carnet);

                            }
                        }
                        catch (Exception x)
                        {
                            lblActualizacionRC.Text = "No se pudo eliminar la información a causa de un error interno.";
                            log("No se pudo eliminar la información a causa de un error interno. " + x + "- RC", Carnet);
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
        protected void ConfirmarRC(string Carnet)
        {
            if (!TxtPrimerNombreRC.Text.IsNullOrWhiteSpace())
            {
                LlenadoRC("CODIGO = '" + Carnet + "' AND CONTROL_ACCION ='RC' ");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi(CmbCarneRC.SelectedValue);

                if (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor"))
                    respuesta = QueryActualizaNombreRC(Carnet);

                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == null)
                    respuesta = "0";

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    if (ROLES.Value.Contains("Estudiante"))
                    {
                        respuesta = ActualizarNITRC(CmbCarneRC.Text);
                    }
                    if (respuesta == "0")
                    {
                        respuesta = ServiciosHCM_RC();
                        if (respuesta == "0")
                        {
                            respuesta = "";
                            QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet, "RC");
                            if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                            {
                                //SE INGRESA LA INFORMACIÓN EN EL BANCO
                                respuesta = ConsumoSQLRC(txtInsertBI.Text.ToUpper());
                                if (respuesta == "0")
                                {
                                    respuesta = ConsumoOracleRC(txtInsertApex.Text);
                                    if (respuesta == "0")
                                    {

                                        if (controlRenovacion == 0)
                                        {
                                            //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                            respuesta = ConsumoOracleRC("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                        }
                                        else
                                        {
                                            if (controlRenovacionFecha < 2)
                                            {
                                                //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                                respuesta = ConsumoOracleRC("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                            }
                                            else
                                            {
                                                respuesta = "0";
                                            }

                                            if (respuesta == "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
                                            {
                                                Upload(Carnet);
                                            }
                                            else if (respuesta != "0" && (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor")))
                                            {
                                                log("ERROR - Actualizacion de fotografia en campus RC", Carnet);
                                            }
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
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- RC", Carnet);
                        }
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet + "- RC", Carnet);

                        }
                    }

                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacionRC.Text = "Se confirmó correctamente la información";
                        EnvioCorreo("bodyConfirmacionEmpleados.txt", "datosConfirmacionEmpleados.txt", TxtPrimerNombreRC.Text, TxtPrimerApellidoRC.Text);
                        log("La información fue confirmada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "- RC", Carnet); 
                        BuscarRC("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                        File.Delete(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
                        LimpiarCamposRC();
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus del carnet: " + Carnet + "- RC", Carnet);
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet + "- RC", Carnet);
                        }
                        lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQLRC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                    }
                }
                else
                {
                    lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet + "- RC", Carnet);
                    ConsumoSQLRC("DELETE FROM [dbo].[Tarjeta_Identificacion_admins] WHERE CODIGO ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacionRC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        public string ServiciosHCM_RC()
        {
            string constr = TxtURL.Text;
            int contador;
            //Obtener se obtiene toda la información del empleado
            string expand = "names,photos";
            string consulta = consultaGetworkers(expand, "nationalIdentifiers");

            //Se obtienen los id's de las tablas a las cuales se les agregará información
            string personId = getBetween(consulta, "workers/", "/child/");
            string comIm = personId + "/child/photo/";
            string consultaImagenes = consultaGetImagenes(comIm);
            string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
            string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");
            string base64String = "";
            string PersonNameId = getBetween(consulta, "\"PersonNameId\" : ", ",\n");
            string effectivePerson = getBetween(consulta, PersonNameId + ",\n      \"EffectiveStartDate\" : \"", "\",\n");
            string hrefName = getBetween(consulta, "\n      \"LocalNameInformation30\" :", "\n        \"name\" : \"names\",");
            hrefName = getBetween(hrefName, "/child/names/", "\"");
            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + CmbCarneRC.SelectedValue + "'";
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                            if (contador > 0)
                            {
                                byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/FotosConfirmacion/RENOVACION_CARNE-RC/" + CmbCarneRC.SelectedValue + ".jpg");
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                        con.Close();
                    }
                }

                //ACTUALIZACION-CREACION DE FOTOGRAFIA
                string pid = getBetween(consulta, "\"PhotoId\" :", ",");
                string consultaperfil = pid + ",\n      \"PrimaryFlag\" : ";
                string perfil = getBetween(consulta, consultaperfil, ",\n");
                var Imgn = "{\"ImageName\" : \"" + TxtDpiRC.Text + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + base64String + "\"}";
                string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
                string name = "{\"LastName\" : \"" + TxtPrimerApellidoRC.Text + "\",\"FirstName\": \"" + TxtPrimerNombreRC.Text + "\",\"MiddleNames\": \"" + TxtSegundoNombreRC.Text + "\"," +
                    "\"PreviousLastName\": \"" + TxtApellidoCasadaRC.Text + "\",\"NameInformation1\": \"" + TxtSegundoApellidoRC.Text + "\",\"LegislationCode\": \"GT\"}";
                if (perfil == "true" && ImageId != "")
                {
                    updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                }
                else
                {
                    create(personId, "photo", Imgn, "emps/");
                }

                //ACTUALIZAR NOMBRE
                if (respuestaPatch == 0 && respuestaPost == 0)
                {
                    updatePatch(name, personId, "names", hrefName, "names", Hoy, "workers/");
                }

                if (respuestaPatch == 0)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception)
            {
                return "1";
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
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + emplid + "' AND TIPO_PERSONA != 2";
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
                        string ApellidoAnterior = "";
                        string ApellidoCAnterior = "";

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

                        if (EffdtDireccionNitUltimo != "" && !String.IsNullOrEmpty(EffdtDireccionNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' " +
                                              "AND ADDRESS1 ='" + TxtDiRe1RC + "' AND ADDRESS2 = '" + TxtDiRe2RC + "' AND ADDRESS3 = '" + TxtDiRe3RC + "' " +
                                              "AND COUNTRY='" + PaisNitRC + "' AND STATE ='" + StateNitRC + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                              "ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNitUltimo != "" && !String.IsNullOrEmpty(EffdtNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' " +
                            " AND EXTERNAL_SYSTEM_ID = '" + NITRC + "' AND EMPLID = '" + emplid + "'" +
                            " AND EFFDT = '" + Convert.ToDateTime(EffdtNitUltimo).ToString("dd/MM/yyyy") + "'";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                            }
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

                        if (EffdtNombreNitUltimo != "" && !String.IsNullOrEmpty(EffdtNombreNitUltimo))
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoRRC + "' " +
                                               "AND FIRST_NAME='" + TxtNombreRRC + "' AND SECOND_LAST_NAME='" + TxtCasadaRRC + "' " +
                                               "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                            }

                            cmd.CommandText = "SELECT LAST_NAME , SECOND_LAST_NAME FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ApellidoAnterior = reader["LAST_NAME"].ToString();
                                ApellidoCAnterior = reader["SECOND_LAST_NAME"].ToString();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_A_NIT_RC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

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

                        TxtApellidoRRC = System.Text.RegularExpressions.Regex.Replace(TxtApellidoRRC, @"\s+", " "); ;
                        TxtNombreRRC = System.Text.RegularExpressions.Regex.Replace(TxtNombreRRC, @"\s+", " "); ;
                        TxtCasadaRRC = System.Text.RegularExpressions.Regex.Replace(TxtCasadaRRC, @"\s+", " ");

                        UD_NAMES_PRI_RC.Value = "";
                        UD_NAMES_PRF_RC.Value = "";
                        UP_NAMES_PRI_RC.Value = "";
                        UP_NAMES_PRF_RC.Value = "";

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            if (!TxtApellidoRRC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRRC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UP_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtApellidoRRC + "," + TxtNombreRRC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UP_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }
                            }
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE
                            if (!TxtApellidoRRC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRRC.IsNullOrWhiteSpace())
                                {
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
                                }
                                else
                                {
                                    UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtApellidoRRC + "," + TxtNombreRRC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                            }
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            if (!TxtApellidoRRC.IsNullOrWhiteSpace())
                            {
                                if (!TxtCasadaRRC.IsNullOrWhiteSpace())
                                {
                                    UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "        <COLL_NAMES>" +
                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                    "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                    "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                    "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                    "          <PROP_SECOND_LAST_NAME>" + TxtCasadaRRC + @"</PROP_SECOND_LAST_NAME>" +
                                                    "        </COLL_NAMES>" +
                                                    "      </COLL_NAME_TYPE_VW>";
                                }
                                else
                                {
                                    UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                     "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                     "        <COLL_NAMES>" +
                                                     "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                     "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                     "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                     "          <PROP_LAST_NAME>" + TxtApellidoRRC + @"</PROP_LAST_NAME>" +
                                                     "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                     "        </COLL_NAMES>" +
                                                     "      </COLL_NAME_TYPE_VW>";

                                    if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                    {
                                        //ACTUALIZA NIT
                                        txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', " +
                                            "PN.NAME_FORMAL ='" + TxtApellidoRRC + "," + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtApellidoRRC + "," + TxtNombreRRC + "' " +
                                            "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                        "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                    }
                                }
                            }
                            else
                            {
                                UD_NAMES_NIT_RC.Value = "<COLL_NAME_TYPE_VW> " +
                                                     "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                     "        <COLL_NAMES>" +
                                                     "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                     "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                     "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                     "          <PROP_FIRST_NAME>" + TxtNombreRRC + @"</PROP_FIRST_NAME>" +
                                                     "        </COLL_NAMES>" +
                                                     "      </COLL_NAME_TYPE_VW>";

                                if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }

                                if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                {
                                    //ACTUALIZA NIT
                                    txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreRRC + "', " +
                                        "PN.NAME_FORMAL ='" + TxtNombreRRC + "', PN.NAME_DISPLAY ='" + TxtNombreRRC + "' " +
                                        "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + emplid + "' " +
                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                }
                            }
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
                        if (contadorUD > 0)
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
                            lblActualizacionRC.Text = "Ocurrió un problema al actualizar el NIT ";
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacionRC.Text = "Ocurrió un problema al actualizar el NIT ";
                        return "1";
                    }
                }
            }
        }
        protected string QueryActualizaNombreRC(string emplid)
        {
            //EN CAMPUS
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellidoRC.Text + " " + TxtSegundoApellidoRC.Text + " " + TxtApellidoCasadaRC.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombreRC.Text + " " + TxtSegundoNombreRC.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellidoRC.Text + " " + TxtSegundoApellidoRC.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasadaRC.Text;
            string EFFDT_Name = "";

            if (Direccion1RC == "")
            {
                Direccion1RC = " ";
            }
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
                            // ACTUALIZAR
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
                            // ACTUALIZAR
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
        protected string ConsumoOracleRC(string ComandoConsulta)
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
                        lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información " + x;
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }
        protected string ConsumoSQLRC(string Consulta)
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
                        TxtEstadoRC.Text += x.ToString();
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }


        protected void QueryInsertBi( string CODIGO)
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
                    if (ROLES.Value.Contains("Estudiante"))
                    {
                        cmd.CommandText = "SELECT 'INSERT INTO[dbo].[Tarjeta_Identificacion_admins] " +
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
                                "VALUES ('''||CODIGO||''','''" + // APELLIDO DE CASADA
                                    "||CARGO||''','''" + //Carrera
                                    "||DIRECCION||''','''" + //DIRECCION
                                    "||ZONA||''','''" + //ZONA
                                    "||COLONIA||''','''" + //COLONIA
                                    "||CEDULA||''','''" + //DECULA
                                    "||DEPTO_CEDULA||''',''' " + //DEPARTAMENTO CEDULA
                                    "||MUNI_CEDULA||''',''' " + //MUNICIPIO CEDULA
                                    "||FACULTAD||''','''" + //CARGO
                                    "||FACULTAD||''',''' " + //DEPARTAMENTO 
                                    "||DEPTO||''','''" + //FACULTAD
                                    "||CARNET||''','''" + //CODIGO
                                    "||TIPO_PERSONA||''','''" + //TIPO_PERSONA
                                    "||NO_CTA_BI||''',''' " + //NO CTA BI
                                    "||FECHANAC||''',''' " + //FECHA NACIMIENTO
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
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + CODIGO + "')";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT 'INSERT INTO[dbo].[Tarjeta_Identificacion_admins] " +
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
                                    "VALUES ('''||CODIGO||''','''" + // APELLIDO DE CASADA
                                        "||FACULTAD||''','''" + //Carrera
                                        "||DIRECCION||''','''" + //DIRECCION
                                        "||ZONA||''','''" + //ZONA
                                        "||COLONIA||''','''" + //COLONIA
                                        "||CEDULA||''','''" + //DECULA
                                        "||DEPTO_CEDULA||''',''' " + //DEPARTAMENTO CEDULA
                                        "||MUNI_CEDULA||''',''' " + //MUNICIPIO CEDULA
                                        "||CARGO||''','''" + //CARGO
                                        "||DEPTO||''',''' " + //DEPARTAMENTO 
                                        "||FACULTAD||''','''" + //FACULTAD
                                        "||CODIGO||''','''" + //CODIGO
                                        "||TIPO_PERSONA||''','''" + //TIPO_PERSONA
                                        "||NO_CTA_BI||''',''' " + //NO CTA BI
                                        "||FECHANAC||''',''' " + //FECHA NACIMIENTO
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
                                        "||PAIS_NACIONALIDAD||''','''" + //PAIS_NACIONALIDAD
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
                                        "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + CODIGO + "')";
                    }
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtInsertBI.Text = reader["INS"].ToString();
                    }
                }
            }
        }
        private void BuscarAC(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CODIGO FROM DUAL UNION SELECT CODIGO FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'AC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarneAC.DataSource = ds;
                    CmbCarneAC.DataTextField = "CODIGO";
                    CmbCarneAC.DataValueField = "CODIGO";
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
                    cmd.CommandText = "SELECT ' ' CODIGO FROM DUAL UNION SELECT CODIGO FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'PC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarnePC.DataSource = ds;
                    CmbCarnePC.DataTextField = "CODIGO";
                    CmbCarnePC.DataValueField = "CODIGO";
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
                    cmd.CommandText = "SELECT ' ' CODIGO FROM DUAL UNION SELECT CODIGO FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = '" + confirmacion + "' AND CONTROL_ACCION = 'RC'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarneRC.DataSource = ds;
                    CmbCarneRC.DataTextField = "CODIGO";
                    CmbCarneRC.DataValueField = "CODIGO";
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
        protected void QueryUpdateApex(string Confirmación, string Solicitado, string Entrega, string FechaHora, string Accion, string Carne, string ControlAccion)
        {
            txtInsertApex.Text = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION = '" + Confirmación + "', FECHA_SOLICITADO='" + Solicitado + "', FECHA_ENTREGA='" + Entrega + "', " +
                "ACCION='" + Accion + "', FECHA_HORA='" + FechaHora + "'" +
                " WHERE CODIGO = '" + Carne + "' AND CONTROL_ACCION = '" + ControlAccion + "'";
        }
        void LeerInfoTxtPath()
        {
            //Lectura de archivo txt para la ruta de almacenamiento en el servidor
            string rutaCompleta = CurrentDirectory + "PathAlmacenamiento.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath.Text = line;
                file.Close();
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
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/ACTUALIZACION-AC/" + Carnet + ".jpg");
                            }
                            if (ControlTabs.Value == "PC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/PRIMER_CARNET-PC/" + Carnet + ".jpg");
                            }
                            if (ControlTabs.Value == "RC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/RENOVACION_CARNE-RC/" + Carnet + ".jpg");
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
            catch (Exception)
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
        private string consultaGetworkers(string expand, string expandUser)
        {
            credencialesWS(archivoWS, "Consultar");
            string consulta = consultaUser(expandUser, UserEmplid.Value);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = UserEmplid.Value;
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/workers?q=PersonId=" + personID + "&effectiveDate=" + dtFechaBuscarPersona + "&expand=" + expand, user, pass);
            return respuesta;
        }
        private string consultaGetImagenes(string consultar)
        {
            credencialesWS(archivoWS, "Consultar");
            string consulta = consultaUser("nationalIdentifiers", UserEmplid.Value);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = getBetween(result[result.Count() - 1], "\"NationalIdentifierId\" : ", ",");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/emps/" + consultar, user, pass);
            return respuesta;
        }
        private string consultaUser(string expand, string personId)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/workers?q=PersonId=" + personId + "&effectiveDate=" + dtFechaBuscarPersona + "&expand=" + expand, user, pass);
            return respuesta;
        }
        private void updatePatch(string info, string personId, string tables, string ID, string consulta, string effective, string esquema)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Patch(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/" + esquema + personId + "/child/" + tables + "/" + ID, user, pass, info, consulta, effective);
            respuestaPatch = respuesta + respuestaPatch;
        }
        private void create(string personId, string tables, string datos, string EXTEN)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Post(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/" + EXTEN + personId + "/child/" + tables, datos, user, pass);
            respuestaPost = respuestaPost + respuesta;
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            //Funcion para extraer contenido que se encuentre en una cadena
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
        private static void credencialesWS(string RutaConfiguracion, string strMetodo)
        {
            //Función para obtener información de acceso al servicio de Campus
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
        public int contadorID(int largo, string[] cadena)
        {
            int posicion = 0;
            for (int i = 0; i < largo; i++)
            {
                if (cadena[i].Contains("EffectiveStartDate"))
                {
                    posicion = i;
                }
            }
            return posicion;
        }
        public int contadorSlash(int largo, string cadena)
        {
            int contador = 0;
            string letra;
            for (int i = 0; i < largo; i++)
            {
                letra = cadena.Substring(i, 1);
                if (letra == "\"")
                {
                    contador++;
                }
            }
            return contador;
        }
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }
        public string LeerBodyEmail(string archivo)
        {
            string rutaCompleta = CurrentDirectory + "/Emails/" + archivo;
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
            string rutaCompleta = CurrentDirectory + "/Emails/" + archivo;
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
        public void EnvioCorreo(string body, string subject, string TxtPrimerNombre, string TxtPrimerApellido)
        {

            string htmlBody = LeerBodyEmail(body);
            string[] datos = LeerInfoEmail(subject);
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            var para = TxtPrimerNombre + " " + TxtPrimerApellido;

            email.From.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.To.Add(new MailboxAddress(para, EmailInstitucional.Value));

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
                catch (Exception)
                {
                    //lblActualizacion.Text = ex.ToString();
                    log("ERROR - Al enviar el correo para : " + EmailInstitucional.Value, "");
                }
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

        protected void Tab1_Click(object sender, EventArgs e)
        {
            // Evento cuando se hace clic en la Tab 1
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 0;
            ControlTabs.Value = "AC";
            lblActualizacionAC.Text = "";
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(0);
        }
        protected void Tab2_Click(object sender, EventArgs e)
        {
            // Evento cuando se hace clic en la Tab 2
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 1;
            ControlTabs.Value = "PC";
            //lblActualizacionPC.Text = "";
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(1);
        }
        protected void Tab3_Click(object sender, EventArgs e)
        {
            // Evento cuando se hace clic en la Tab 3
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 2;
            ControlTabs.Value = "RC";
            //lblActualizacionRC.Text = "";
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


        //EVENTOS ACTUALIZAR
        protected void CmbTipo_SelectedIndexChangedAC(object sender, EventArgs e)
        {
            LlenadoAC("CODIGO = '" + CmbCarneAC.Text + "' AND CONTROL_ACCION = 'AC' ");
            if (txtCantidad.Text != "0" && !txtCantidad.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
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

                if (txtCantidad.Text == "1")
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
            ImgFoto1AC.ImageUrl = "~/Usuarios/FotosColaboradores/FotosConfirmacion/ACTUALIZACION-AC/" + CmbCarneAC.Text + ".jpg";
        }
        protected void BtnRechazarAC_Click(object sender, EventArgs e)
        {
            if (CmbCarneAC.SelectedValue != " ")
            {
                RechazarAC(CmbCarneAC.Text);
            }
            else
            {
                lblActualizacionAC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void BtnConfirmarAC_Click(object sender, EventArgs e)
        {
            if (CmbCarneAC.SelectedValue != " ")
            {
                string carne = CmbCarneAC.Text;
                ConfirmarAC(carne);
            }
            else
            {
                lblActualizacionAC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }

        //EVENTOS PRIMER CARNET
        protected void CmbTipo_SelectedIndexChangedPC(object sender, EventArgs e)
        {
            LlenadoPC("CODIGO = '" + CmbCarnePC.Text + "' AND CONTROL_ACCION = 'PC' ");
            if (txtCantidad.Text != "0" && !txtCantidad.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
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

                if (txtCantidad.Text == "1")
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
            ImgFoto1PC.ImageUrl = "~/Usuarios/FotosColaboradores/FotosConfirmacion/PRIMER_CARNET-PC/" + CmbCarnePC.Text + ".jpg";
        }
        protected void BtnRechazarPC_Click(object sender, EventArgs e)
        {
            if (CmbCarnePC.SelectedValue != " ")
            {
                RechazarPC(CmbCarnePC.Text);
            }
            else
            {
                lblActualizacionPC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void BtnConfirmarPC_Click(object sender, EventArgs e)
        {
            if (CmbCarnePC.SelectedValue != " ")
            {
                string carne = CmbCarnePC.Text;
                ConfirmarPC(carne);
            }
            else
            {
                lblActualizacionPC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }

        //EVENTOS RENOVACION CARNET
        protected void CmbTipo_SelectedIndexChangedRC(object sender, EventArgs e)
        {
            LlenadoRC("CODIGO = '" + CmbCarneRC.Text + "' AND CONTROL_ACCION = 'RC' ");
            if (txtCantidad.Text != "0" && !txtCantidad.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
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

                if (txtCantidad.Text == "1")
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
            ImgFoto1RC.ImageUrl = "~/Usuarios/FotosColaboradores/FotosConfirmacion/RENOVACION_CARNE-RC/" + CmbCarneRC.Text + ".jpg";
        }
        protected void BtnRechazarRC_Click(object sender, EventArgs e)
        {
            if (CmbCarneRC.SelectedValue != " ")
            {
                RechazarRC(CmbCarneRC.Text);
            }
            else
            {
                lblActualizacionRC.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void BtnConfirmarRC_Click(object sender, EventArgs e)
        {
            if (CmbCarneRC.SelectedValue != " ")
            {
                string carne = CmbCarneRC.Text;
                ConfirmarRC(carne);
            }
            else
            {
                lblActualizacionRC.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
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
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarneAC.SelectedValue, UP_NAMES_PRI_AC.Value, UP_NAMES_PRF_AC.Value, UP_NAMES_NIT_AC.Value, UP_ADDRESSES_NIT_AC.Value, VersionUP.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarneAC.SelectedValue, UD_NAMES_PRI_AC.Value, UD_NAMES_PRF_AC.Value, UD_NAMES_NIT_AC.Value, UD_ADDRESSES_NIT_AC.Value, VersionUD.Value);
            }
            else if (auxConsulta == 2)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarnePC.SelectedValue, UP_NAMES_PRI_AC.Value, UP_NAMES_PRF_AC.Value, UP_NAMES_NIT_AC.Value, UP_ADDRESSES_NIT_PC.Value,VersionUP.Value);
            }
            else if (auxConsulta == 3)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarnePC.SelectedValue, UD_NAMES_PRI_PC.Value, UD_NAMES_PRF_PC.Value, UD_NAMES_NIT_PC.Value, UD_ADDRESSES_NIT_PC.Value,VersionUD.Value);
            }
            else if (auxConsulta == 4)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarneRC.SelectedValue, UP_NAMES_PRI_RC.Value, UP_NAMES_PRF_RC.Value, UP_NAMES_NIT_RC.Value, UP_ADDRESSES_NIT_RC.Value, VersionUP.Value);
            }
            else if (auxConsulta == 5)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarneRC.SelectedValue, UD_NAMES_PRI_RC.Value, UD_NAMES_PRF_RC.Value, UD_NAMES_NIT_RC.Value, UD_ADDRESSES_NIT_RC.Value, VersionUD.Value);
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
                return elemList[0].InnerText.ToString();
            }
            catch
            {
                return "0";
            }
        }
        private static void CuerpoConsultaUD(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRI, string COLL_NAMES_PRF, string COLL_NAMES_NIT, string COLL_ADDRESSES_NIT, string VersionUD)
        {
            //Crea el cuerpo que se utiliza para hacer PATCH
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
                                         " + COLL_NAMES_PRI + @"
                                         " + COLL_NAMES_PRF + @"
                                         " + COLL_NAMES_NIT + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                      </Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUP(string Usuario, string Pass, string EMPLID, string COLL_NAMES_PRI, string COLL_NAMES_PRF, string COLL_NAMES_NIT, string COLL_ADDRESSES_NIT, string VersionUP)
        {
            //Crea el cuerpo que se utiliza para hacer POST
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