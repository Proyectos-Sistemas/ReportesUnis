using MailKit.Security;
using Microsoft.Ajax.Utilities;
using MimeKit;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class Tabs : System.Web.UI.Page
    {
        
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string rutaFisicaAC = "";
            string rutaFisicaPC = "";
            string rutaFisicaRC = "";
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
                    llenadoGridAC();
            }
                if (GridViewFotosAC.Rows.Count == 0)
                {
                    lblActualizacion.Text = "No hay información para confirmar.";
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
                string rutaCompleta = CurrentDirectory + "PathConfirmacion.txt";

                using (StreamReader file = new StreamReader(rutaCompleta))
                {
                    string linea1 = file.ReadLine();
                    string linea2 = file.ReadLine();
                    string linea3 = file.ReadLine();
                    txtPathAC.Text = linea1;
                    txtPathPC.Text = linea2;
                    txtPathRC.Text = linea2;
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
                                        "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + Carnet + "' AND CONFIRMACION != 1 AND CONTROL_ACCION = 'PC')";
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
                    " WHERE CARNET = '" + Carne + "'";
            }
            private void ValidacionCheckAC()
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
                    string FechaHoraInicioEjecución = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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
                catch (Exception )
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
           

            //EVENTOS
            protected void ButtonSubmitAC_Click(object sender, EventArgs e)
            {
                foreach (GridViewRow row in GridViewFotosAC.Rows)
                {
                    CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImage");
                    if (checkBox.Checked)
                    {
                        int cargaFt = 0;
                        try
                        {
                            File.Delete(txtPath2.Text + row.Cells[1].Text);
                            cargaFt = 0;
                        }
                        catch (Exception)
                        {
                            cargaFt = 1;
                        }
                        if (cargaFt == 0)
                        {
                            string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                            carne.Value = nombre;
                            string[] datos = DatosCorreo();
                            string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + nombre + "'";
                            string respuesta = ConsumoOracle(cadena);
                            if (respuesta == "0")
                            {
                                File.Delete(CurrentDirectory + txtPathAC.Text + row.Cells[1].Text);
                                File.Delete(txtPath2.Text + row.Cells[1].Text);
                                llenadoGridAC();
                                log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre);
                                lblActualizacion.Text = "Se rechazaron las fotos seleccionadas.";
                                //EnvioCorreo("bodyRechazoFotoEstudiante.txt", "datosRechazoFotoEstudiante.txt", datos[1], datos[0]);
                            }
                            else
                            {
                                log("ERROR - Error al eliminar el registro", nombre);
                                lblActualizacion.Text = "Ocurrió un error al eliminar los registros";
                            }
                        }
                        else
                        {
                            lblActualizacion.Text = "Ocurrió un error al eliminar los registros";
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
                ValidacionCheckAC();
                string Ncarnet = "";
                string Merror = "Ocurrió un problema al confirmar la información de:";
                string MensajeFinal = "";
                if (Convert.ToInt16(prueba.Text) > 0 || prueba.Text.IsNullOrWhiteSpace())
                {
                    lblActualizacion.Text = "Antes de confirmar recuerda eliminar las imágenes seleccionadas.";
                }
                else
                {
                    foreach (GridViewRow row in GridViewFotosAC.Rows)
                    {
                        string respuesta = null;
                        string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                        string carnet = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                        Ncarnet = carnet;
                        carne.Value = carnet;
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
                                    Upload(carnet);
                                }
                                else
                                {
                                    log("ERROR - Actualización de Fotografía", carnet);
                                }
                            }
                        }
                        else
                        {
                            log("ERROR - Inserta BI del carnet: " + carnet, carnet);
                        }

                        if (respuesta == "0")
                        {
                            lblActualizacion.Text = "Se confirmó correctamente la información.";
                            File.Delete(CurrentDirectory + txtPathAC.Text + row.Cells[1].Text);
                            llenadoGridAC();
                            string[] datos = DatosCorreo();
                            log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet);
                            //EnvioCorreo("bodyConfirmacionFotoEstudiante.txt", "datosConfirmacionFotoEstudiante.txt", datos[1], datos[0]);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(MensajeFinal))
                                MensajeFinal = carnet;
                            else
                                MensajeFinal = MensajeFinal + ", " + carnet;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(MensajeFinal))
                {
                    MensajeFinal = Merror + " " + MensajeFinal;
                    lblActualizacion.Text = MensajeFinal;
                    log("ERROR - " + MensajeFinal, Ncarnet);

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
                        cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + carnet + "','" + ErrorLog + "','CONFIRMACIÓN FOTOS EMPLEADOS',SYSDATE)";
                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                    }
                }
            }

        

    }
}