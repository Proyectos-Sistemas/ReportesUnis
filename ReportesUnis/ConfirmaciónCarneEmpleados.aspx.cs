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


namespace ReportesUnis
{
    public partial class ConfirmaciónCarneEmpleados : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string TxtNombreR = "";
        string TxtApellidoR = "";
        string TxtCasadaR = "";
        string NIT = "";
        string TxtDiRe1 = "";
        string TxtDiRe2 = "";
        string TxtDiRe3 = "";
        string StateNit = "";
        string PaisNit = "";
        string Direccion1 = "";
        string Direccion2 = "";
        string Direccion3 = "";
        int controlRenovacion = 0;
        int controlRenovacionFecha = 0;
        int auxConsulta = 0;
        int contadorUP = 0;
        int contadorUD = 0;
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigCampus.dat");
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();
        /// CONSUMO DE API
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
                LimpiarCampos();
                divCampos.Visible = true;
                divDPI.Visible = true;
                divFotografia.Visible = true;
                divBtnConfirmar.Visible = true;
                Buscar("1");
                lblActualizacion.Text = null;
            }
        }

        //FUNCIONES
        private void Buscar(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CODIGO FROM DUAL UNION SELECT CODIGO FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE (TIPO_PERSONA != 2 OR ROLES IS NOT NULL) AND CONFIRMACION = '" + confirmacion + "'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarne.DataSource = ds;
                    CmbCarne.DataTextField = "CODIGO";
                    CmbCarne.DataValueField = "CODIGO";
                    CmbCarne.DataBind();
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
        private void llenado(string where)
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
                        TxtDpi.Text = reader["CUI"].ToString();
                        if (TxtDpi.Text.IsNullOrWhiteSpace())
                        {
                            TxtDpi.Text = reader["NO_PASAPORTE"].ToString();
                        }
                        TxtPrimerNombre.Text = reader["NOMBRE1"].ToString();
                        TxtSegundoNombre.Text = reader["NOMBRE2"].ToString();
                        TxtPrimerApellido.Text = reader["APELLIDO1"].ToString();
                        TxtSegundoApellido.Text = reader["APELLIDO2"].ToString();
                        TxtApellidoCasada.Text = reader["DECASADA"].ToString();
                        TxtPuesto.Text = reader["CARGO"].ToString();
                        TxtFacultad.Text = reader["DEPTO"].ToString();
                        TxtFechaNac.Text = reader["FECHANAC"].ToString();
                        TxtEstado.Text = reader["ESTADO_CIVIL"].ToString();
                        TxtDireccion.Text = reader["DIRECCION"].ToString();
                        TxtDepartamento.Text = reader["DEPTO_RESIDENCIA"].ToString();
                        TxtMunicipio.Text = reader["MUNI_RESIDENCIA"].ToString();
                        TxtTel.Text = reader["CELULAR"].ToString();
                        txtCantidad.Text = reader["TOTALFOTOS"].ToString();
                        TxtNombreR = reader["NOMBRE_NIT"].ToString();
                        TxtApellidoR = reader["APELLIDOS_NIT"].ToString();
                        TxtCasadaR = reader["CASADA_NIT"].ToString();
                        TxtDiRe1 = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2 = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3 = reader["DIRECCION3_NIT"].ToString();
                        StateNit = reader["STATE_NIT"].ToString();
                        PaisNit = reader["PAIS_NIT"].ToString();
                        TxtPais.Text = reader["PAIS_R"].ToString();
                        Direccion1 = reader["ADDRESS1"].ToString();
                        Direccion2 = reader["ADDRESS2"].ToString();
                        Direccion3 = reader["ADDRESS3"].ToString();
                        TxtCorreoInstitucional.Text = reader["EMAIL"].ToString();
                        EmailInstitucional.Value = reader["EMAIL"].ToString();
                        TxtCorreoPersonal.Text = reader["EMAIL_PERSONAL"].ToString();
                        TxtRol.Text = reader["TIPO_PERSONA"].ToString();
                        ROLES.Value = reader["ROLES"].ToString();
                        UserEmplid.Value = reader["EMPLID"].ToString();
                    }
                    con.Close();
                }
            }
        }
        private void LimpiarCampos()
        {
            TxtDpi.Text = null;
            TxtPrimerNombre.Text = null;
            TxtSegundoNombre.Text = null;
            TxtPrimerApellido.Text = null;
            TxtSegundoApellido.Text = null;
            TxtApellidoCasada.Text = null;
            TxtPuesto.Text = null;
            TxtFacultad.Text = null;
            TxtFechaNac.Text = null;
            TxtEstado.Text = null;
            TxtDireccion.Text = null;
            TxtDepartamento.Text = null;
            TxtMunicipio.Text = null;
            TxtTel.Text = null;
            ImgDPI2.ImageUrl = null;
            ImgDPI1.ImageUrl = null;
            ImgFoto1.ImageUrl = null;
            txtCantidad.Text = null;
            TxtPais.Text = null;
            TxtCorreoInstitucional.Text = null;
            TxtCorreoPersonal.Text = null;
            TxtRol.Text = null;
        }
        private void Rechazar(string Carnet)
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                lblActualizacion.Text = "";
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
                                Buscar("1");
                                File.Delete(txtPath.Text + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + Carnet + ".jpg");
                                File.Delete(CurrentDirectory + "/Usuarios/UltimasCargas/" + Carnet + ".jpg");
                                for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                                {
                                    File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                                }
                                EnvioCorreo("bodyRechazoEmpleados.txt", "datosRechazoEmpleados.txt");
                                log("La información de: " + TxtDpi.Text + ", con el carne : " + Carnet + " fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), Carnet);
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                                lblActualizacion.Text = "Se ha rechazado la solicitud de carnet.";
                            }
                            else
                            {
                                lblActualizacion.Text = "Ocurrió un error al rechazar la solicitud";
                                log("Ocurrió un error al eliminar la fotografía de: " + TxtDpi.Text + ", con el carne : " + Carnet, Carnet);

                            }
                        }
                        catch (Exception x)
                        {
                            lblActualizacion.Text = "No se pudo eliminar la información a causa de un error interno.";
                            log("No se pudo eliminar la información de:" + TxtDpi.Text + ", con el carne : " + Carnet + " a causa de un error interno. " + x, Carnet);
                            transaction.Rollback();
                        }
                    }
                }
                LimpiarCampos();
            }
            else
            {
                lblActualizacion.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void Confirmar(string Carnet)
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                llenado("CODIGO = '" + Carnet + "'");
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi();

                if (ROLES.Value.Contains("Estudiante") || ROLES.Value.Contains("Profesor"))
                    respuesta = QueryActualizaNombre(Carnet);

                controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + Carnet + "'");

                if (respuesta == null)
                    respuesta = "0";

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    if (ROLES.Value.Contains("Estudiante"))
                    {
                        respuesta = ActualizarNIT(CmbCarne.Text);
                    }
                    if (respuesta == "0")
                    {
                        respuesta = serviciosHCM();
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
                                        if (controlRenovacion < 2 || (controlRenovacion == 2 && controlRenovacionFecha == 1))
                                        {
                                            if (controlRenovacion == 0)
                                            {
                                                //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                                respuesta = ConsumoOracle("INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO) VALUES ('" + Carnet + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "')");
                                            }
                                            else
                                            {
                                                if (controlRenovacionFecha < 2)
                                                {
                                                    //ACTUALIZA INFORMACIÓN DE LA RENOVACION
                                                    respuesta = ConsumoOracle("UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "' WHERE EMPLID='" + Carnet + "'");
                                                }
                                                else
                                                {
                                                    respuesta = "0";
                                                }

                                                if (respuesta == "0" && ROLES.Value.Contains("Estudiante"))
                                                {
                                                    Upload(Carnet);
                                                }
                                                else if (respuesta != "0")
                                                {
                                                    log("ERROR - Actualizacion de fotografia en campus del carnet: " + Carnet, Carnet);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            log("ERROR - El carnet " + Carnet + " llegó al límite de renovacines", Carnet);
                                        }
                                    }
                                    else
                                    {
                                        log("ERROR - Inserta APEX del carnet: " + Carnet, Carnet);
                                    }
                                }
                                else
                                {
                                    log("ERROR - Inserta BI del carnet: " + Carnet, Carnet);
                                }
                            }
                            else
                            {
                                log("ERROR - al armar consulta Update APEX del carnet: " + Carnet, Carnet);
                            }
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet, Carnet);
                        }
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - al actualizar en el NIT en Campus del carnet: " + Carnet, Carnet);

                        }
                    }

                    // Al finalizar la actualización, ocultar el modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);

                    if (respuesta == "0")
                    {
                        lblActualizacion.Text = "Se confirmó correctamente la información";
                        log("La información de: " + TxtDpi.Text + ", con el carne : " + Carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), Carnet);
                        EnvioCorreo("bodyConfirmacionEmpleados.txt", "datosConfirmacionEmpleados.txt");
                        Buscar("1");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + Carnet + ".jpg");
                        File.Delete(CurrentDirectory + "/Usuarios/UltimasCargas/" + Carnet + ".jpg");
                        LimpiarCampos();
                    }
                    else
                    {
                        if (ROLES.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus del carnet: " + Carnet, Carnet);
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + Carnet, Carnet);
                        }
                        lblActualizacion.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CODIGO ='" + Carnet + "'");
                    }
                }
                else
                {
                    lblActualizacion.Text = "Ocurrió un problema al confirmar la información";
                    log("ERROR - Actualizacion nombre en Campus del carnet: " + Carnet, Carnet);
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CODIGO ='" + Carnet + "'");
                }
            }
            else
            {
                lblActualizacion.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
            }
        }
        protected void QueryInsertBi()
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
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + CmbCarne.SelectedValue + "')";
                    }
                    else
                    {
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
                                        "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + CmbCarne.SelectedValue + "')";
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
        protected string QueryActualizaNombre(string emplid)
        {
            //EN CAMPUS
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + " " + TxtApellidoCasada.Text).TrimEnd();
            string TxtNombre = (TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasada.Text;
            string EFFDT_Name = "";

            if (Direccion2 == "")
            {
                Direccion2 = " ";
            }
            if (Direccion3 == "")
            {
                Direccion3 = " ";
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
                            UP_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
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

                            UP_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
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
                            UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
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

                            UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
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
                            UD_NAMES_PRF.Value = "<COLL_NAME_TYPE_VW> " +
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

                            UD_NAMES_PRI.Value = "<COLL_NAME_TYPE_VW> " +
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
                            lblActualizacion.Text = "Ocurrió un problema al confirmar la información ";
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacion.Text = "Ocurrió un problema al confirmar la información ";
                        return "1";
                    }
                }
            }
        }
        protected void QueryUpdateApex(string Confirmación, string Solicitado, string Entrega, string FechaHora, string Accion, string Carne)
        {
            txtInsertApex.Text = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION = '" + Confirmación + "', FECHA_SOLICITADO='" + Solicitado + "', FECHA_ENTREGA='" + Entrega + "', " +
                "ACCION='" + Accion + "', FECHA_HORA='" + FechaHora + "'" +
                " WHERE CODIGO = '" + Carne + "'";
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
                        lblActualizacion.Text = "Ocurrió un problema al confirmar la información " + x;
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
                        TxtEstado.Text += x.ToString();
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
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
        private string ActualizarNIT(string emplid)
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
                                        "WHERE PN.NATIONAL_ID ='" + TxtDpi.Text + "' " +
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
                            TxtNombreR = reader2["NOMBRE_NIT"].ToString();
                            TxtApellidoR = reader2["APELLIDOS_NIT"].ToString();
                            TxtCasadaR = reader2["CASADA_NIT"].ToString();
                            TxtDiRe1 = reader2["DIRECCION1_NIT"].ToString();
                            TxtDiRe2 = reader2["DIRECCION2_NIT"].ToString();
                            TxtDiRe3 = reader2["DIRECCION3_NIT"].ToString();
                            StateNit = reader2["STATE_NIT"].ToString();
                            PaisNit = reader2["PAIS_NIT"].ToString();
                            NIT = reader2["NIT"].ToString();
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
                                              "AND ADDRESS1 ='" + TxtDiRe1 + "' AND ADDRESS2 = '" + TxtDiRe2 + "' AND ADDRESS3 = '" + TxtDiRe3 + "' " +
                                              "AND COUNTRY='" + PaisNit + "' AND STATE ='" + StateNit + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
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
                            " AND EXTERNAL_SYSTEM_ID = '" + NIT + "' AND EMPLID = '" + emplid + "'" +
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

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoR + "' " +
                                               "AND FIRST_NAME='" + TxtNombreR + "' AND SECOND_LAST_NAME='" + TxtCasadaR + "' " +
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
                            EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_A_NIT.Value.Length == 9)
                            {
                                EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_A_NIT.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + emplid + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();

                            if (EFFDT_NameR.Value.Length == 9)
                            {
                                EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                            }
                            else
                            {
                                EFFDT_NameR.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                            }
                        }

                        string FechaEfectiva = "";
                        if (EFFDT_NameR.Value.IsNullOrWhiteSpace())
                            FechaEfectiva = "1900-01-01";
                        else
                            FechaEfectiva = EFFDT_NameR.Value;

                        if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit >= 0)
                        {//INSERT
                            UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoR + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreR + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUP = contadorUP + 1;
                        }
                        else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit >= 0 && ContadorEffdtNombreNit > 0)
                        {//UPDATE
                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoR + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreR + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }
                        else
                        {//UPDATE
                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "        <COLL_NAMES>" +
                                                "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                "          <KEYPROP_EFFDT>" + EffdtDireccionNitUltimo + @"</KEYPROP_EFFDT>" +
                                                "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                "          <PROP_LAST_NAME>" + TxtApellidoR + @"</PROP_LAST_NAME>" +
                                                "          <PROP_FIRST_NAME>" + TxtNombreR + @"</PROP_FIRST_NAME>" +
                                                "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR + @"</PROP_SECOND_LAST_NAME>" +
                                                "        </COLL_NAMES>" +
                                                "      </COLL_NAME_TYPE_VW>";
                            contadorUD = contadorUD + 1;
                        }

                        //ACTUALIZA NIT
                        if (EffdtNitUltimo != Hoy && ContadorNit == 0)
                        {
                            //INSERTA EL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) VALUES ('" + emplid + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + NIT + "')";
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
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NIT + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + HoyEffdt + "'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NIT + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "' AND EFFDT ='" + EFFDT_SYSTEM.Substring(0, 10).TrimEnd() + "'";
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
                                              "                                                <PROP_COUNTRY>" + PaisNit + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1 + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2 + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3 + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNit + @"</PROP_STATE>  " +
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
                                              "                                                <PROP_COUNTRY>" + PaisNit + @"</PROP_COUNTRY> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS1>" + TxtDiRe1 + @"</PROP_ADDRESS1> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS2>" + TxtDiRe2 + @"</PROP_ADDRESS2> " +
                                              "\n" +
                                              "                                                <PROP_ADDRESS3>" + TxtDiRe3 + @"</PROP_ADDRESS3> " +
                                              "\n" +
                                              "                                                <PROP_STATE>" + StateNit + @"</PROP_STATE>  " +
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
                                                  "                                                <PROP_COUNTRY>" + PaisNit + @"</PROP_COUNTRY> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS1>" + TxtDiRe1 + @"</PROP_ADDRESS1> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS2>" + TxtDiRe2 + @"</PROP_ADDRESS2> " +
                                                  "\n" +
                                                  "                                                <PROP_ADDRESS3>" + TxtDiRe3 + @"</PROP_ADDRESS3> " +
                                                  "\n" +
                                                  "                                                <PROP_STATE>" + StateNit + @"</PROP_STATE>  " +
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
                            lblActualizacion.Text = "Ocurrió un problema al actualizar el NIT ";
                            return "1";
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        lblActualizacion.Text = "Ocurrió un problema al actualizar el NIT ";
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
                            byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/" + Carnet + ".jpg");
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
        public string serviciosHCM()
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
                        cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + CmbCarne.SelectedValue + "'";
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                            if (contador > 0)
                            {
                                byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + CmbCarne.SelectedValue + ".jpg");
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
                var Imgn = "{\"ImageName\" : \"" + TxtDpi.Text + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + base64String + "\"}";
                string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
                string name = "{\"LastName\" : \"" + TxtPrimerApellido.Text + "\",\"FirstName\": \"" + TxtPrimerNombre.Text + "\",\"MiddleNames\": \"" + TxtSegundoNombre.Text + "\"," +
                    "\"PreviousLastName\": \"" + TxtApellidoCasada.Text + "\",\"NameInformation1\": \"" + TxtSegundoApellido.Text + "\",\"LegislationCode\": \"GT\"}";
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
        public void EnvioCorreo(string body, string subject)
        {

            string htmlBody = LeerBodyEmail(body);
            string[] datos = LeerInfoEmail(subject);
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            var para = TxtPrimerNombre.Text + " " + TxtPrimerApellido.Text;

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
                catch (Exception ex)
                {
                    //lblActualizacion.Text = ex.ToString();
                    log("ERROR - Al enviar el correo para : " + EmailInstitucional.Value, "");
                }
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
                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + carnet + "','" + ErrorLog + "','CONFIRMACIÓN DATOS SENSIBLES EMPLEADOS',SYSDATE)";
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                }
            }
        }

        //EVENTOS
        protected void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenado("CODIGO = '" + CmbCarne.Text + "'");
            if (txtCantidad.Text != "0" && !txtCantidad.Text.IsNullOrWhiteSpace())
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1.Visible = true;
                        ImgDPI1.ImageUrl = "~/Usuarios/DPI/" + CmbCarne.Text + "(" + (i + 1) + ").jpg";
                    }

                    if (i == 1)
                    {
                        ImgDPI2.Visible = true;
                        ImgDPI2.ImageUrl = "~/Usuarios/DPI/" + CmbCarne.Text + "(" + (i + 1) + ").jpg";
                    }
                }

                if (txtCantidad.Text == "1")
                {
                    ImgDPI2.Visible = false;
                }
            }
            else
            {
                ImgDPI1.Visible = false;
                ImgDPI2.Visible = false;
                ImgFoto1.Visible = false;
            }

            if (!CmbCarne.Text.IsNullOrWhiteSpace())
            {
                lblActualizacion.Text = null;
            }

            HFoto.Visible = true;
            ImgFoto1.ImageUrl = "~/Usuarios/FotosConfirmacion/" + CmbCarne.Text + ".jpg";
        }
        protected void BtnRechazar_Click(object sender, EventArgs e)
        {
            if (CmbCarne.SelectedValue != " ")
            {
                Rechazar(CmbCarne.Text);
            }
            else
            {
                lblActualizacion.Text = "Debe de seleccionar un número de carnet para poder rechazar la información.";
            }
        }
        protected void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (CmbCarne.SelectedValue != " ")
            {
                string carne = CmbCarne.Text;
                Confirmar(carne);
            }
            else
            {
                lblActualizacion.Text = "Debe de seleccionar un número de carnet para poder confirmar la información.";
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
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, CmbCarne.SelectedValue, UP_NAMES_PRI.Value, UP_NAMES_PRF.Value, UP_NAMES_NIT.Value, UP_ADDRESSES_NIT.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, CmbCarne.SelectedValue, UD_NAMES_PRI.Value, UD_NAMES_PRF.Value, UD_NAMES_NIT.Value, UD_ADDRESSES_NIT.Value);
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