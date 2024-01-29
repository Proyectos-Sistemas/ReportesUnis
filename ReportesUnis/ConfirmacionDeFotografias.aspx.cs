using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
//using System.Drawing;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Ajax.Utilities;
using NPOI.SS.Formula.Functions;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Resources;
using NPOI.Util;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Net;
using System.Text;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace ReportesUnis
{
    public partial class ConfirmacionDeFotografias : System.Web.UI.Page
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
                llenadoGrid();
                ViewState["ActiveTabIndex"] = 0;
                ControlTabs.Value = "AC";
                // Establecer la pestaña activa y su estilo correspondiente
                SetActiveTab(0);

            }
            if (GridViewFotos.Rows.Count == 0)
            {
                lblActualizacion.Text = "No hay fotografías para confirmar en este apartado.";
            }

        }

        void llenadoGrid()
        {
            string[] archivos = Directory.GetFiles(rutaFisicaAC);
            List<object> imagenes = new List<object>();

            foreach (string archivo in archivos)
            {
                string nombreImagen = Path.GetFileName(archivo);
                imagenes.Add(new { NombreImagen = nombreImagen });
            }

            GridViewFotos.DataSource = imagenes;
            GridViewFotos.DataBind();
            if (GridViewFotos.Rows.Count == 0)
            {
                lblActualizacion.Text = "No hay fotografías para confirmar en este apartado.";
                TbEliminarD.Visible = false;
            }
            else
            {
                TbEliminarD.Visible = true;
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
            else
            {
                TbEliminarPC.Visible = true;
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
            else
            {
                TbEliminarRC.Visible = true;
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
            string rutaCompleta = CurrentDirectory + "PathConfirmacion.txt";

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

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotos.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImage");
                if (checkBox.Checked)
                {
                    // Obtener el nombre de la imagen seleccionada sin extension

                    int cargaFt = 0;

                    if (cargaFt == 0)
                    {
                        string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                        carne.Value = nombre;
                        string[] datos = DatosCorreo(carne.Value);
                        string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + nombre + "' OR CODIGO = '" + nombre + "'";
                        string respuesta = ConsumoOracle(cadena);
                        if (respuesta == "0")
                        {
                            File.Delete(CurrentDirectory + txtPathAC.Text + "/" + row.Cells[1].Text);
                            //File.Delete(txtPath2.Text + row.Cells[1].Text);
                            llenadoGrid();
                            log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");
                            lblActualizacion.Text = "Se rechazaron las fotos seleccionadas.";
                            EnvioCorreo("bodyRechazoFotoEstudiante.txt", "datosRechazoFotoEstudiante.txt", datos[1], datos[0]);
                        }
                        else
                        {
                            log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");
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

        protected void GridViewFotos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image)e.Row.FindControl("Image1");
                string nombreImagen = DataBinder.Eval(e.Row.DataItem, "NombreImagen").ToString();
                string rutaImagen = Path.Combine("~" + txtPathAC.Text, nombreImagen);
                image.ImageUrl = rutaImagen;
            }
        }

        protected void ButtonSubmitPC_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotosPC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImagePC");
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
                        string[] datos = DatosCorreo(carne.Value);
                        string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + nombre + "' OR CODIGO = '"+nombre+"'";
                        string respuesta = ConsumoOracle(cadena);
                        string cadena2 = "DELETE FROM UNIS_INTERFACES.TBL_CONTROL_CARNET WHERE EMPLID = '" + nombre + "'";
                        string respuesta2 = "0"; ConsumoOracle(cadena2);
                        if (respuesta == "0" && respuesta2 == "0")
                        {
                            File.Delete(CurrentDirectory + txtPathPC.Text + "/" + row.Cells[1].Text);
                            File.Delete(txtPath2.Text + row.Cells[1].Text);
                            llenadoGridPC();
                            log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE PC");
                            lblActualizacionPC.Text = "Se rechazaron las fotos seleccionadas.";
                            EnvioCorreo("bodyRechazoFotoEstudiante.txt", "datosRechazoFotoEstudiante.txt", datos[1], datos[0]);
                        }
                        else
                        {
                            log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE PC");
                            lblActualizacionPC.Text = "Ocurrió un error al eliminar los registros";
                        }
                    }
                    else
                    {
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
            string Ncarnet = "";
            string Merror = "Ocurrió un problema al confirmar la información de:";
            string MensajeFinal = "";
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
                    Ncarnet = carnet;
                    string carne= carnet;
                    QueryInsertBi(carnet, "RC");
                    //SE INGRESA LA INFORMACIÓN EN EL BANCO
                    ConsumoSQL("DELETE FROM [Carnets].[dbo].[Tarjeta_Identificacion_prueba]  WHERE CARNET = '" + carnet + "'");
                    respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                    //respuesta = "0";

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
                                log("ERROR - Actualización de Fotografía", carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE PC");
                            }
                        }
                    }
                    else
                    {
                        log("ERROR - Inserta BI del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE PC");
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacionPC.Text = "Se confirmó correctamente la información.";
                        File.Delete(CurrentDirectory + txtPathPC.Text + "/" + row.Cells[1].Text);
                        llenadoGridPC();
                        string[] datos = DatosCorreo(carnet);
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE PC");
                        EnvioCorreo("bodyConfirmacionFotoEstudiante.txt", "datosConfirmacionFotoEstudiante.txt", datos[1], datos[0]);
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
                lblActualizacionPC.Text = MensajeFinal;
                log("ERROR - " + MensajeFinal, Ncarnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");

            }

        }
        protected void ButtonSubmitRC_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotosRC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImageRC");
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
                        string[] datos = DatosCorreo(carne.Value);
                        string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + nombre + "' OR CODIGO = '" + nombre + "'";
                        string respuesta = ConsumoOracle(cadena);
                        if (respuesta == "0")
                        {
                            File.Delete(CurrentDirectory + txtPathRC.Text + "/" + row.Cells[1].Text);
                            File.Delete(txtPath2.Text + row.Cells[1].Text);
                            llenadoGridRC();
                            log("La fotografía de fue rechazada por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");
                            lblActualizacionRC.Text = "Se rechazaron las fotos seleccionadas.";
                            EnvioCorreo("bodyRechazoFotoEstudiante.txt", "datosRechazoFotoEstudiante.txt", datos[1], datos[0]);
                        }
                        else
                        {
                            log("ERROR - Error al eliminar el registro", nombre, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");
                            lblActualizacionRC.Text = "Ocurrió un error al eliminar los registros";
                        }
                    }
                    else
                    {
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
            string Ncarnet = "";
            string Merror = "Ocurrió un problema al confirmar la información de:";
            string MensajeFinal = "";
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
                    Ncarnet = carnet;
                    carne.Value = carnet;
                    QueryInsertBi(carnet, "PC");
                    //SE INGRESA LA INFORMACIÓN EN EL BANCO
                    ConsumoSQL("DELETE FROM [Carnets].[dbo].[Tarjeta_Identificacion_prueba]  WHERE CARNET = '" + carnet + "'");
                    respuesta = ConsumoSQL(txtInsertBI.Text.ToUpper());
                    //respuesta = "0";

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
                                log("ERROR - Actualización de Fotografía", carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");
                            }
                        }
                    }
                    else
                    {
                        log("ERROR - Inserta BI del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacionRC.Text = "Se confirmó correctamente la información.";
                        File.Delete(CurrentDirectory + txtPathRC.Text + "/" + row.Cells[1].Text);
                        llenadoGridRC();
                        string[] datos = DatosCorreo(carnet);
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");
                        EnvioCorreo("bodyConfirmacionFotoEstudiante.txt", "datosConfirmacionFotoEstudiante.txt", datos[1], datos[0]);
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
                lblActualizacionRC.Text = MensajeFinal;
                log("ERROR - " + MensajeFinal, Ncarnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE RC");

            }

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

        public string[] DatosCorreo(string carne)
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
                    cmd.CommandText = "SELECT EMAIL, NOMBRE1||' '||APELLIDO1 AS NOMBRE, NO_CUI||DEPTO_CUI||MUNI_CUI CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO ='" + carne + "'  OR CARNET = '" + carne + "'";
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
                    log("ERROR - Al enviar el correo para : " + EmailInstitucional, "", "CONFIRMACION FOTOGRAFIA ESTUDIANTE");
                }
            }

        }
                       
        protected void QueryUpdateApex(string Confirmación, string Solicitado, string Entrega, string FechaHora, string Accion, string Carne)
        {
            txtInsertApex.Text = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION = '" + Confirmación + "', FECHA_SOLICITADO='" + Solicitado + "', FECHA_ENTREGA='" + Entrega + "', " +
                "ACCION='" + Accion + "', FECHA_HORA='" + FechaHora + "'" +
                " WHERE CARNET = '" + Carne + "'";
        }

        protected void QueryInsertBi(string Carnet, string CONTROL)
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
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + Carnet + "' OR CODIGO = '" + Carnet + "' AND CONFIRMACION != 1 AND CONTROL_ACCION != '" + CONTROL + "')";
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtInsertBI.Text = reader["INS"].ToString();
                    }
                }
            }
        }

        protected void BtnConfirmar_Click(object sender, EventArgs e)
        {
            prueba.Text = "0";
            ValidacionCheck();
            string Ncarnet = "";
            string Merror = "Ocurrió un problema al confirmar la información de:";
            string MensajeFinal = "";
            if (Convert.ToInt16(prueba.Text) > 0 || prueba.Text.IsNullOrWhiteSpace())
            {
                lblActualizacion.Text = "Antes de confirmar recuerda eliminar las imágenes seleccionadas.";
            }
            else
            {
                foreach (GridViewRow row in GridViewFotos.Rows)
                {
                    string respuesta = null;
                    string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    string carnet = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    Ncarnet = carnet;
                    carne.Value = carnet;
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
                            log("ERROR - Actualización de Fotografía", carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");
                        }
                    }
                    /* }
                     else
                     {
                         log("ERROR - Inserta BI del carnet: " + carnet, carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");
                     }*/

                    if (respuesta == "0")
                    {
                        lblActualizacion.Text = "Se confirmó correctamente la información.";
                        File.Delete(CurrentDirectory + txtPathAC.Text + "/" + row.Cells[1].Text);
                        llenadoGrid();
                        string[] datos = DatosCorreo(carnet);
                        log("La fotografía de: " + DPI.Value + ", con el carne : " + carnet + " fue confirmada de forma correcta por el usuario " + Context.User.Identity.Name.Replace("@unis.edu.gt", ""), carnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");
                        EnvioCorreo("bodyConfirmacionFotoEstudiante.txt", "datosConfirmacionFotoEstudiante.txt", datos[1], datos[0]);
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
                log("ERROR - " + MensajeFinal, Ncarnet, "CONFIRMACION FOTOGRAFIA ESTUDIANTE AC");

            }
        }

        private void ValidacionCheck()
        {
            foreach (GridViewRow row in GridViewFotos.Rows)
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
            foreach (GridViewRow row in GridViewFotosPC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImagePC");
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
            foreach (GridViewRow row in GridViewFotosRC.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImageRC");
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
                llenadoGrid();
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
        protected void Tab1_Click(object sender, EventArgs e)
        {
            // Actualizar el índice de la pestaña activa en el ViewState
            ViewState["ActiveTabIndex"] = 0;
            ControlTabs.Value = "AC";
            lblActualizacion.Text = "";
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
