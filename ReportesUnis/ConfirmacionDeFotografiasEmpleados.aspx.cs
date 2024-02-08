using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Linq;
using ReportesUnis.API;
using System.Text;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using NPOI.Util;

namespace ReportesUnis
{
    public partial class ConfirmacionDeFotografiasEmpleados : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        ConsumoAPI api = new ConsumoAPI();
        string rutaFisicaAC = "";
        string rutaFisicaPC = "";
        string rutaFisicaRC = "";
        int respuestaPatch = 0;
        int respuestaPost = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            LeerInfoTxtPath();
            LeerInfoTxtPathGrid();
            LeerInfoTxtSQL();
            LeerInfoTxt();
            rutaFisicaAC = Server.MapPath("~" + txtPathAC.Text);
            rutaFisicaPC = Server.MapPath("~" + txtPathPC.Text);
            rutaFisicaRC = Server.MapPath("~" + txtPathRC.Text);
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("ACCESO_CARNETIZACION") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                //llenadoGridAC();
                ViewState["ActiveTabIndex"] = 0;
                ControlTabs.Value = "AC";
                // Establecer la pestaña activa y su estilo correspondiente
                SetActiveTab(0);
            }
            if (GridViewFotosAC.Rows.Count == 0)
            {
                lblActualizacionAC.Text = "No hay fotografías para confirmar en este apartado.";
            }
        }

        //FUNCIONES
        void llenadoGridAC()
        {
            string[] archivos = Directory.GetFiles(rutaFisicaAC);
            List<object> imagenes = new List<object>();

            foreach (string archivo in archivos)
            {
                string nombreImagen = Path.GetFileName(archivo);
                imagenes.Add(new { NombreImagen = nombreImagen });
            }
            GridViewFotosAC.DataSource = imagenes;
            GridViewFotosAC.DataBind();
            if (GridViewFotosAC.Rows.Count == 0)
            {
                lblActualizacionAC.Text = "No hay fotografías para confirmar en este apartado.";
                TbEliminarAC.Visible = false;
            }
        }
        void llenadoGridPC()
        {
            string[] archivos = Directory.GetFiles(rutaFisicaPC);
            List<object> imagenes = new List<object>();

            foreach (string archivo in archivos)
            {
                string nombreImagen = Path.GetFileName(archivo);
                imagenes.Add(new { NombreImagen = nombreImagen });
            }
            GridViewFotosPC.DataSource = imagenes;
            GridViewFotosPC.DataBind();
            if (GridViewFotosPC.Rows.Count == 0)
            {
                lblActualizacionPC.Text = "No hay fotografías para confirmar en este apartado.";
                TbEliminarPC.Visible = false;
            }
        }
        void llenadoGridRC()
        {
            string[] archivos = Directory.GetFiles(rutaFisicaRC);
            List<object> imagenes = new List<object>();

            foreach (string archivo in archivos)
            {
                string nombreImagen = Path.GetFileName(archivo);
                imagenes.Add(new { NombreImagen = nombreImagen });
            }
            GridViewFotosRC.DataSource = imagenes;
            GridViewFotosRC.DataBind();
            if (GridViewFotosRC.Rows.Count == 0)
            {
                lblActualizacionRC.Text = "No hay fotografías para confirmar en este apartado.";
                TbEliminarRC.Visible = false;
            }
        }
        void LeerInfoTxtPath()
        {
            string rutaCompleta = CurrentDirectory + "PathAlmacenamiento.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath2.Text = line;
                file.Close();
            }
        }
        void LeerInfoTxtPathGrid()
        {
            string rutaCompleta = CurrentDirectory + "PathConfirmacionEmpleados.txt";

            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                txtPathAC.Text = linea1;
                txtPathPC.Text = linea2;
                txtPathRC.Text = linea3;
                file.Close();
            }
        }
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
                    catch (Exception)
                    {
                        transaction.Rollback();
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
        protected void QueryInsertBi(string Carnet)
        {
            tipoPersona(Carnet);
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
                    if (TipoPersona.Value.Contains("Estudiante"))
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
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + Carnet + "')";
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
                                        "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + Carnet + "')";
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
        protected void QueryUpdateApex(string Confirmación, string Solicitado, string Entrega, string FechaHora, string Accion, string Carne)
        {
            txtInsertApex.Text = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION = '" + Confirmación + "', FECHA_SOLICITADO='" + Solicitado + "', FECHA_ENTREGA='" + Entrega + "', " +
                "ACCION='" + Accion + "', FECHA_HORA='" + FechaHora + "'" +
                " WHERE CODIGO = '" + Carne + "'";
        }
        private void ValidacionCheck()
        {
            foreach (GridViewRow row in GridViewFotosAC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImage");
                if (checkBox.Checked)
                {
                    if (prueba.Text.IsNullOrWhiteSpace())
                    {
                        prueba.Text = "1";
                    }
                    else
                    {
                        prueba.Text = (Convert.ToInt16(prueba.Text) + 1).ToString();
                    }
                }
            }
        }

        private void ValidacionCheckPC()
        {
            foreach (GridViewRow row in GridViewFotosAC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImageP");
                if (checkBox.Checked)
                {
                    if (prueba.Text.IsNullOrWhiteSpace())
                    {
                        prueba.Text = "1";
                    }
                    else
                    {
                        prueba.Text = (Convert.ToInt16(prueba.Text) + 1).ToString();
                    }
                }
            }
        }

        private void ValidacionCheckRC()
        {
            foreach (GridViewRow row in GridViewFotosAC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImage");
                if (checkBox.Checked)
                {
                    if (prueba.Text.IsNullOrWhiteSpace())
                    {
                        prueba.Text = "1";
                    }
                    else
                    {
                        prueba.Text = (Convert.ToInt16(prueba.Text) + 1).ToString();
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
                    byte[] imageBytes = null;
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
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/ACTUALIZACION-AC/" + CODIGO.Value + ".jpg");

                            }
                            if (ControlTabs.Value == "PC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/PRIMER_CARNET-PC/" + CODIGO.Value + ".jpg");

                            }
                            if (ControlTabs.Value == "RC")
                            {
                                imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/RENOVACION_CARNE-RC/" + CODIGO.Value + ".jpg");
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
                string FechaHoraInicioEjecución = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                /* int ContadorArchivos = 0;
                 int ContadorArchivosCorrectos = 0;
                 int ContadorArchivosConError = 0;
                 bool Error = false;*/

                /*//Ruta del archivo que guarda la bitácora
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
                */
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
        public void tipoPersona(string carne)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ROLES, " +
                        "NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, CODIGO, EMPLID, EMAIL, NOMBRE1||' '||APELLIDO1 AS NOMBRE " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + carne + "'  OR CARNET = '" + carne + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TipoPersona.Value = reader["ROLES"].ToString();
                        DPI.Value = reader["CARNET"].ToString();
                        CODIGO.Value = reader["CODIGO"].ToString();
                        EMPLID.Value = reader["EMPLID"].ToString();
                        EMAIL.Value = reader["EMAIL"].ToString();
                        NOMBRE.Value = reader["NOMBRE"].ToString();
                    }
                    con.Close();
                }
            }
        }
        public string serviciosHCM()
        {
            string base64String = "";
            string constr = TxtURL.Text;
            int contador;
            //Obtener se obtiene toda la información del empleado
            string expand = "names,photos";
            string consulta = consultaGetworkers(expand, "nationalIdentifiers");

            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        byte[] imageBytes = null;
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + CODIGO.Value + "'";
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                            if (contador > 0)
                            {
                                if (ControlTabs.Value == "AC")
                                {
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/ACTUALIZACION-AC/" + CODIGO.Value + ".jpg");

                                }
                                if (ControlTabs.Value == "PC")
                                {
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/PRIMER_CARNET-PC/" + CODIGO.Value + ".jpg");

                                }
                                if (ControlTabs.Value == "RC")
                                {
                                    imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/FotosColaboradores/UltimasCargas/RENOVACION_CARNE-RC/" + CODIGO.Value + ".jpg");
                                }
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                        con.Close();
                    }
                }

                //ACTUALIZACION-CREACION DE FOTOGRAFIA
                string personId = getBetween(consulta, "workers/", "/child/");
                string comIm = personId + "/child/photo/";
                string consultaImagenes = consultaGetImagenes(comIm);
                string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
                string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");
                string pid = getBetween(consulta, "\"PhotoId\" :", ",");
                string consultaperfil = pid + ",\n      \"PrimaryFlag\" : ";
                string perfil = getBetween(consulta, consultaperfil, ",\n");
                var Imgn = "{\"ImageName\" : \"" + DPI.Value + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + base64String + "\"}";

                if (perfil == "true" && ImageId != "")
                {
                    updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                }
                else
                {
                    create(personId, "photo", Imgn, "emps/");
                }

                return "0";
            }
            catch (Exception)
            {
                return "1";
            }
        }
        private string consultaGetworkers(string expand, string expandUser)
        {
            credencialesWS(archivoWS, "Consultar");
            string consulta = consultaUser(expandUser, EMPLID.Value);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = EMPLID.Value;
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
            string consulta = consultaUser("nationalIdentifiers", CODIGO.Value);
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
            //Funcion para extraerlos Id's
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
        public void EnvioCorreo(string body, string subject, string Nombre, string EmailInstitucional)
        {
            string htmlBody = LeerBodyEmail(body);
            string[] datos = LeerInfoEmail(subject);
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            var para = Nombre;

            email.From.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.To.Add(new MailboxAddress(para, EmailInstitucional));

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
                catch
                {
                    log("ERROR - Al enviar el correo para : " + EmailInstitucional, "", "CONFIRMACION FOTOGRAFIA EMPLEADO");
                }
            }

        }
        public string[] DatosCorreo()
        {
            string[] datos;
            string constr = TxtURL.Text;
            string EmailInstitucional = "";
            string Nombre = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT EMAIL, NOMBRE1||' '||APELLIDO1 AS NOMBRE, NO_CUI||DEPTO_CUI||MUNI_CUI CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + carne.Value + "'  OR CARNET = '" + carne.Value + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        EmailInstitucional = reader["EMAIL"].ToString();
                        Nombre = reader["NOMBRE"].ToString();
                        DPI.Value = reader["CARNET"].ToString();
                    }
                    con.Close();
                }
            }
            datos = new string[] { EmailInstitucional, Nombre };
            return datos;
        }

        private void log(string ErrorLog, string carnet, string Pantalla)
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
                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + carnet + "','" + ErrorLog + "','" + Pantalla + "',SYSDATE)";
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                }
            }
        }

        //EVENTOS
        protected void ButtonSubmitAC_Click(object sender, EventArgs e)
        {
            CheckBox checkBox;
            foreach (GridViewRow row in GridViewFotosAC.Rows)
            {
                checkBox = (CheckBox)row.FindControl("CheckBoxImage");

                if (checkBox.Checked)
                {
                    string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    carne.Value = nombre;
                    tipoPersona(nombre);
                    string[] datos = DatosCorreo();
                    string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + nombre + "' OR CARNET = '" + nombre + "'";
                    string respuesta = ConsumoOracle(cadena);
                    if (respuesta == "0")
                    {
                        File.Delete(CurrentDirectory + txtPathAC.Text + row.Cells[1].Text);
                        //File.Delete(txtPath2.Text + row.Cells[1].Text);
                        llenadoGridAC();
                        log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS AC");
                        lblActualizacionAC.Text = "Se rechazaron las fotos seleccionadas.";
                        EnvioCorreo("bodyRechazoFotoEmpleados.txt", "datosRechazoFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS AC");
                        lblActualizacionAC.Text = "Ocurrió un error al eliminar los registros";
                    }
                }

            }
        }
        protected void GridViewFotosAC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image)e.Row.FindControl("Image1");
                string nombreImagen = DataBinder.Eval(e.Row.DataItem, "NombreImagen").ToString();
                string rutaImagen = Path.Combine("~" + txtPathAC.Text, nombreImagen);
                image.ImageUrl = rutaImagen;
            }
        }
        protected void BtnConfirmarAC_Click(object sender, EventArgs e)
        {
            prueba.Text = "0";
            ValidacionCheck();
            if (Convert.ToInt16(prueba.Text) > 0 || prueba.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionAC.Text = "Antes de confirmar recuerda eliminar las imágenes seleccionadas.";
            }
            else
            {
                foreach (GridViewRow row in GridViewFotosAC.Rows)
                {
                    string respuesta = null;
                    string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    string carnet = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    tipoPersona(carnet);
                    carne.Value = carnet;

                    QueryUpdateApex("0", fecha, fecha, fecha, "1", carnet);
                    if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                    {
                        respuesta = ConsumoOracle(txtInsertApex.Text);
                        if (respuesta == "0")
                        {
                            respuesta = serviciosHCM();
                            if ((respuesta == "0" && TipoPersona.Value.Contains("Estudiante")) || (respuesta == "0" && TipoPersona.Value.Contains("Profesor")))
                            {
                                Upload(carnet);
                            }
                            else if (respuesta != "0")
                            {
                                log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                            }
                        }
                        else
                        {
                            log("ERROR - Inserta APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                        }
                    }
                    else
                    {
                        log("ERROR - al armar consulta Update APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacionAC.Text = "Se confirmó correctamente la información";
                        File.Delete(CurrentDirectory + txtPathAC.Text + row.Cells[1].Text);
                        llenadoGridAC();
                        string[] datos = DatosCorreo();
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                        EnvioCorreo("bodyConfirmacionFotoEmpleados.txt", "datosConfirmacionFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        if (TipoPersona.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO AC");
                        }
                        lblActualizacionAC.Text = "Ocurrió un problema al confirmar la información";
                    }
                }
            }
        }
        protected void ButtonSubmitPC_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotosPC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImageP");
                Console.WriteLine($"Row Index: {row.RowIndex}, CheckBox ID: {checkBox.ID}, Checked: {checkBox.Checked}");

                if (checkBox.Checked)
                {
                    string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    carne.Value = nombre;
                    tipoPersona(nombre);
                    string[] datos = DatosCorreo();
                    string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + nombre + "' OR CODIGO = '" + nombre + "'";
                    string respuesta = ConsumoOracle(cadena);
                    string cadena2 = "DELETE FROM UNIS_INTERFACES.TBL_CONTROL_CARNET WHERE EMPLID = '" + nombre + "'";
                    string respuesta2 = "0"; ConsumoOracle(cadena2);
                    if (respuesta == "0" && respuesta2 == "0")
                    {
                        File.Delete(CurrentDirectory + txtPathPC.Text + row.Cells[1].Text);
                        File.Delete(txtPath2.Text + row.Cells[1].Text);
                        llenadoGridPC();
                        log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS PC");
                        lblActualizacionPC.Text = "Se rechazaron las fotos seleccionadas.";
                        EnvioCorreo("bodyRechazoFotoEmpleados.txt", "datosRechazoFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS PC");
                        lblActualizacionPC.Text = "Ocurrió un error al eliminar los registros";
                    }
                }
            }
        }
        protected void GridViewFotosPC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image)e.Row.FindControl("Image1");
                string nombreImagen = DataBinder.Eval(e.Row.DataItem, "NombreImagen").ToString();
                string rutaImagen = Path.Combine("~" + txtPathPC.Text, nombreImagen);
                image.ImageUrl = rutaImagen;
            }
        }
        protected void BtnConfirmarPC_Click(object sender, EventArgs e)
        {
            prueba.Text = "0";
            ValidacionCheckPC();
            if (Convert.ToInt16(prueba.Text) > 0 || prueba.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionPC.Text = "Antes de confirmar recuerda eliminar las imágenes seleccionadas.";
            }
            else
            {
                foreach (GridViewRow row in GridViewFotosPC.Rows)
                {
                    string respuesta = null;
                    string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    string carnet = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    tipoPersona(carnet);
                    QueryInsertBi(carnet);
                    ConsumoSQL("DELETE FROM [Carnets].[dbo].[Tarjeta_Identificacion_admins]  WHERE CARNET = '" + carnet + "'");
                    //SE INGRESA LA INFORMACIÓN EN EL BANCO
                    respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", carnet);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            respuesta = ConsumoOracle(txtInsertApex.Text);
                            if (respuesta == "0")
                            {
                                respuesta = serviciosHCM();
                                if ((respuesta == "0" && TipoPersona.Value.Contains("Estudiante")) || (respuesta == "0" && TipoPersona.Value.Contains("Profesor")))
                                {
                                    Upload(carnet);
                                }
                                else if (respuesta != "0")
                                {
                                    log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                                }
                            }
                            else
                            {
                                log("ERROR - Inserta APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                            }
                        }
                        else
                        {
                            log("ERROR - al armar consulta Update APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                        }
                    }
                    else
                    {
                        log("ERROR - Inserta BI del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacionPC.Text = "Se confirmó correctamente la información";
                        File.Delete(CurrentDirectory + txtPathPC.Text + row.Cells[1].Text);
                        string[] datos = DatosCorreo();
                        llenadoGridPC();
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                        EnvioCorreo("bodyConfirmacionFotoEmpleados.txt", "datosConfirmacionFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        if (TipoPersona.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO PC");
                        }
                        lblActualizacionPC.Text = "Ocurrió un problema al confirmar la información";
                    }
                }
            }
        }
        protected void ButtonSubmitRC_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotosRC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImageRC");
                Console.WriteLine($"Row Index: {row.RowIndex}, CheckBox ID: {checkBox.ID}, Checked: {checkBox.Checked}");

                if (checkBox.Checked)
                {
                    string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    carne.Value = nombre;
                    tipoPersona(nombre);
                    string[] datos = DatosCorreo();
                    string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + nombre + "'";
                    string respuesta = ConsumoOracle(cadena);
                    if (respuesta == "0")
                    {
                        File.Delete(CurrentDirectory + txtPathRC.Text + row.Cells[1].Text);
                        //File.Delete(txtPath2.Text + row.Cells[1].Text);
                        llenadoGridRC();
                        log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS RC");
                        lblActualizacionRC.Text = "Se rechazaron las fotos seleccionadas.";
                        EnvioCorreo("bodyRechazoFotoEmpleados.txt", "datosRechazoFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA EMPLEADOS RC");
                        lblActualizacionRC.Text = "Ocurrió un error al eliminar los registros";
                    }
                }
            }
        }
        protected void GridViewFotosRC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image)e.Row.FindControl("Image1");
                string nombreImagen = DataBinder.Eval(e.Row.DataItem, "NombreImagen").ToString();
                string rutaImagen = Path.Combine("~" + txtPathRC.Text, nombreImagen);
                image.ImageUrl = rutaImagen;
            }
        }
        protected void BtnConfirmarRC_Click(object sender, EventArgs e)
        {
            prueba.Text = "0";
            ValidacionCheckRC();
            if (Convert.ToInt16(prueba.Text) > 0 || prueba.Text.IsNullOrWhiteSpace())
            {
                lblActualizacionRC.Text = "Antes de confirmar recuerda eliminar las imágenes seleccionadas.";
            }
            else
            {
                foreach (GridViewRow row in GridViewFotosRC.Rows)
                {
                    string respuesta = null;
                    string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    string carnet = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    carne.Value = carnet;
                    tipoPersona(carnet);
                    ConsumoSQL("DELETE FROM [Carnets].[dbo].[Tarjeta_Identificacion_admins]  WHERE CARNET = '" + carnet + "'");
                    QueryInsertBi(carnet);
                    //SE INGRESA LA INFORMACIÓN EN EL BANCO
                    respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", carnet);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            respuesta = ConsumoOracle(txtInsertApex.Text);
                            if (respuesta == "0")
                            {
                                respuesta = serviciosHCM();
                                if ((respuesta == "0" && TipoPersona.Value.Contains("Estudiante")) || (respuesta == "0" && TipoPersona.Value.Contains("Profesor")))
                                {
                                    Upload(carnet);
                                }
                                else if (respuesta != "0")
                                {
                                    log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                                }
                            }
                            else
                            {
                                log("ERROR - Inserta APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                            }
                        }
                        else
                        {
                            log("ERROR - al armar consulta Update APEX del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                        }
                    }
                    else
                    {
                        log("ERROR - Inserta BI del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacionRC.Text = "Se confirmó correctamente la información";
                        File.Delete(CurrentDirectory + txtPathRC.Text + row.Cells[1].Text);
                        string[] datos = DatosCorreo();
                        llenadoGridRC();
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                        EnvioCorreo("bodyConfirmacionFotoEmpleados.txt", "datosConfirmacionFotoEmpleados.txt", datos[1], datos[0]);
                    }
                    else
                    {
                        if (TipoPersona.Value.Contains("Estudiante"))
                        {
                            log("ERROR - Actualizacion foto Campus: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                        }
                        else
                        {
                            log("ERROR - Actualizacion HCM del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA EMPLEADO RC");
                        }
                        lblActualizacionRC.Text = "Ocurrió un problema al confirmar la información";
                    }
                }
            }
        }

        // Función para establecer la pestaña activa y su estilo
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
                llenadoGridAC();
            }
            else if (tabIndex == 1)
            {
                Tab2.CssClass = "Clicked";
                MainView.ActiveViewIndex = 1;
                llenadoGridPC();
            }
            else if (tabIndex == 2)
            {
                Tab3.CssClass = "Clicked";
                MainView.ActiveViewIndex = 2;
                llenadoGridRC();
            }
        }

        // Evento cuando se hace clic en la Tab 1
        protected void Tab1_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 0;
            ControlTabs.Value = "AC";
            lblActualizacionAC.Text = "";
            // Establecer la pestaña activa y su estilo correspondiente
            SetActiveTab(0);
        }

        // Evento cuando se hace clic en la Tab 2
        protected void Tab2_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 1;
            ControlTabs.Value = "PC";
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

    }
}