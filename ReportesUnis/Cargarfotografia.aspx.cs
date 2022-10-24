using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.IO.Compression;
using NPOI.Util;

namespace ReportesUnis
{
    public partial class Cargarfotografia : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            else
            {
                LeerInfoTxt();
                BindGrid();
            }

            if (!IsPostBack)
            {
                string[] filePaths = Directory.GetFiles(Server.MapPath("~/Files/"));
                List<ListItem> files = new List<ListItem>();
                foreach (string filePath in filePaths)
                {
                    files.Add(new ListItem(Path.GetFileName(filePath), filePath));
                }
            }
        }

        private void BindGrid()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT * FROM SYSADM.PS_EMPL_PHOTO";
                    cmd.Connection = con;
                    con.Open();
                    GridView1.DataSource = cmd.ExecuteReader();
                    GridView1.DataBind();
                    con.Close();
                }
            }
        }

        protected void Upload(object sender, EventArgs e)
        {
            try
            {
                HttpPostedFile ArchivoCarga = FileUpload1.PostedFile;

                int TamañoArchivoCarga = ArchivoCarga.ContentLength;

                if (TamañoArchivoCarga > 1048576)  // 1GB
                {
                    //Finalizar cuando se exceda el archivo tiene un tamaño mayor a 1GB
                    return;
                }

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
                string EmplidFoto = "";
                string EmplidExisteFoto = "";
                string mensajeValidacion = "";
                int contadorDuplicadosXUsuario = 0;
                int contadorDuplicadosXNID = 0;

                if (FileUpload1.HasFile)
                {
                    string uploadFolder = Request.PhysicalApplicationPath + "CargaFotografíaCS\\";

                    foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                    {
                        ContadorArchivos++;
                        Error = false;
                        string ExtensionFotografia = Path.GetExtension(uploadedFile.FileName).ToLower();
                        string[] ExtensionesPermitidas = { ".jpeg", ".jpg" };

                        //Nombre de la fotografía cargada (Sin extensión)
                        string NombreFoto = Path.GetFileNameWithoutExtension(uploadedFile.FileName);

                        if (ExtensionesPermitidas.Contains(ExtensionFotografia))
                        {
                            EmplidFoto = "";
                            EmplidExisteFoto = "";

                            //Se obtiene el EMPLID del usuario, busando el nombre de la fotografía en la tabla de usuario
                            using (OracleConnection conEmplid = new OracleConnection(constr))
                            {

                                try
                                {
                                    OracleCommand cmdEmplid = new OracleCommand();
                                    cmdEmplid.CommandText = "SELECT DISTINCT EMPLID FROM SYSADM.PS_OPRDEFN2 WHERE OPRID = '" + NombreFoto + "'";
                                    cmdEmplid.Connection = conEmplid;
                                    conEmplid.Open();
                                    OracleDataReader reader = cmdEmplid.ExecuteReader();
                                    contadorDuplicadosXUsuario = 0; //Contador para saber si el nombre de la fotografía lo tiene registrado más de un usuario
                                    while (reader.Read())
                                    {
                                        EmplidFoto = reader["EMPLID"].ToString();
                                        contadorDuplicadosXUsuario++;
                                    }

                                    conEmplid.Close();

                                    if (contadorDuplicadosXUsuario > 1)
                                    {
                                        mensajeValidacion = "La fotografía no se puede registrar para más de un usuario en Campus.";
                                        GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                        ContadorArchivosConError++;
                                        if (Error == false)
                                        {
                                            ContadorArchivosConError++;
                                            Error = true;
                                        }
                                    }
                                    else
                                    {
                                        mensajeValidacion = "";
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

                            //Se obtiene el EMPLID del usuario, busando el nombre de la fotografía en la tabla de identificadores nacionales. 
                            //Casos en dónde la persona no tiene registrado un nombre de usuario
                            if (EmplidFoto == "")
                            {
                                using (OracleConnection conEmplid = new OracleConnection(constr))
                                {
                                    try
                                    {
                                        OracleCommand cmdEmplid = new OracleCommand();
                                        cmdEmplid.CommandText = "SELECT DISTINCT EMPLID FROM SYSADM.PS_PERS_NID WHERE NATIONAL_ID = '" + NombreFoto + "'";
                                        cmdEmplid.Connection = conEmplid;
                                        conEmplid.Open();
                                        OracleDataReader reader = cmdEmplid.ExecuteReader();
                                        contadorDuplicadosXNID = 0; //Contador para saber si el nombre de la fotografía lo tiene registrado más de una persona

                                        while (reader.Read())
                                        {
                                            EmplidFoto = reader["EMPLID"].ToString();
                                            contadorDuplicadosXNID++;
                                        }

                                        conEmplid.Close();

                                        if (contadorDuplicadosXNID > 1)
                                        {
                                            mensajeValidacion = "La fotografía no se puede registrar para más de una persona en Campus.";
                                            GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                            ContadorArchivosConError++;
                                            if (Error == false)
                                            {
                                                ContadorArchivosConError++;
                                                Error = true;
                                            }
                                        }
                                        else
                                        {
                                            mensajeValidacion = "";
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

                            //No existe error en validación y existe un EMPLID, guarda imagen
                            if (mensajeValidacion == "" && EmplidFoto != "")
                            {
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

                                //Se guarda la fotografía en Campus
                                using (Stream fs = uploadedFile.InputStream)
                                {
                                    using (BinaryReader br = new BinaryReader(fs))
                                    {
                                        byte[] bytes = br.ReadBytes((Int32)fs.Length);

                                        using (OracleConnection con = new OracleConnection(constr))
                                        {
                                            string query = "";

                                            using (OracleCommand cmd = new OracleCommand(query))
                                            {

                                                if (EmplidExisteFoto != "") //Se actualiza la fotografía
                                                {
                                                    cmd.CommandText = "UPDATE SYSADM.PS_EMPL_PHOTO SET PSIMAGEVER=(TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), EMPLOYEE_PHOTO=:Fotografia WHERE EMPLID = '" + EmplidFoto + "'";
                                                    mensajeValidacion = "La fotografía se actualizó correctamente en Campus.";
                                                }
                                                else //Se registra la nueva fotografía
                                                {
                                                    cmd.CommandText = "INSERT INTO SYSADM.PS_EMPL_PHOTO VALUES ('" + EmplidFoto + "', (TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), :Fotografia)";
                                                    mensajeValidacion = "La fotografía se registró correctamente en Campus.";
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
                                    }
                                }
                            }
                            else
                            {
                                mensajeValidacion = "La fotografía no se registró en Campus, es necesario tener registrado un ID de usuario o un identificador nacional con el nombre de la fotografía.";
                                GuardarBitacora(ArchivoBitacora, NombreFoto.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                if (Error == false)
                                {
                                    ContadorArchivosConError++;
                                }
                            }
                        }
                        else
                        {
                            mensajeValidacion = "La fotografía no tiene formato .JPEG o .JPG";
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

                Response.ContentType = "application/text";
                Response.AddHeader("content-disposition", "attachment; filename=Reporte de Carga.txt");
                Response.TransmitFile(ArchivoBitacora);
                Response.Flush();
                Response.End();
            }
            catch (Exception x)
            {
                Console.WriteLine("Error");
            }
        }

        protected void DownloadFile(object sender, EventArgs e)
        {
            int id = int.Parse((sender as LinkButton).CommandArgument);
            byte[] bytes;
            string fileName, contentType;
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT P.*, CASE WHEN dbms_lob.substr(EMPLOYEE_PHOTO,3,1) = hextoraw('FFD8FF') THEN 'JPG' END Extension FROM SYSADM.PS_EMPL_PHOTO P WHERE EMPLID=:Id";
                    cmd.Parameters.Add(new OracleParameter("Id", id));
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();

                        bytes = (byte[])sdr["EMPLOYEE_PHOTO"];
                        contentType = sdr["Extension"].ToString();
                        fileName = sdr["EMPLID"].ToString() + "." + contentType.ToLower();
                        Console.WriteLine(fileName);
                    }
                    con.Close();
                }
            }
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }


        public void desFotos(DataSet dsDownload, int i)
        {
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = dsDownload.Tables["AllDownload"].Rows[i]["contentType"].ToString();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + dsDownload.Tables["AllDownload"].Rows[i]["fileName"]);
            Response.BinaryWrite((byte[])dsDownload.Tables["AllDownload"].Rows[i]["bytes"]);
            Response.Flush();
            Response.End();
        }

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("index.html");
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
    }
}