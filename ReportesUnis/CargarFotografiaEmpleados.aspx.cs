using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.IO;
using ReportesUnis.API;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using System.Globalization;
using Image = System.Drawing.Image;

namespace ReportesUnis
{
    public partial class CargarFotografiaEmpleados : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.dat");
        int aux = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }

            if (!IsPostBack)
            {

                lblMensaje.Text = "Al finalizar la carga se descargará un archivo con la información del estado de la carga de cada una de las imágenes.";
                string[] filePaths = Directory.GetFiles(Server.MapPath("~/Files/"));
                List<ListItem> files = new List<ListItem>();
                foreach (string filePath in filePaths)
                {
                    files.Add(new ListItem(Path.GetFileName(filePath), filePath));
                }
            }
        }        

        [WebMethod]
        public string Consultar(string dpi)
        {
            //Se limpian variables para guardar la nueva información
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguraciones, "Consultar");
            if (aux == 0)
            {
                //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                CuerpoConsultaPorDPI(Variables.wsUsuario, Variables.wsPassword, dpi);
            }

            //Crea un documento de respuesta HCM
            System.Xml.XmlDocument xmlDocumentoRespuestaHCM = new System.Xml.XmlDocument();

            // Indica que no se mantengan los espacios y saltos de línea
            xmlDocumentoRespuestaHCM.PreserveWhitespace = false;

            try
            {
                // Carga el XML de respuesta de HCM
                xmlDocumentoRespuestaHCM.LoadXml(LlamarWebServiceHCM(Variables.wsUrl, Variables.wsAction, Variables.soapBody));
            }
            catch (WebException)
            {
                //Crea la respuesta cuando se genera una excepción web.
                Variables.strDocumentoRespuesta = Respuesta("05", "ERROR AL CONSULTAR EL REPORTE");
                return Variables.strDocumentoRespuesta;

            }
            XmlNodeList elemList = xmlDocumentoRespuestaHCM.GetElementsByTagName("reportBytes");
            return elemList[0].InnerText.ToString();
        }

        //CONSUMO API
        ConsumoAPI api = new ConsumoAPI();
        int respuestaPatch = 0;
        int respuestaPost = 0;

        private string consultaGetworkers(string expand, int cantidad, string personId)
        {
            string consulta = consultaUser("nationalIdentifiers", personId);
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = getBetween(result[result.Count() - 1], "\"NationalIdentifierId\" : ", ",");
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/workers?q=PersonId=" + personID + "&effectiveDate=" + dtFechaBuscarPersona + "&expand=" + expand, user, pass);
            return respuesta;
        }

        private string consultaGetImagenes(string consultar, int cantidad, string personId)
        {
            string consulta = consultaUser("nationalIdentifiers", personId);
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = getBetween(result[result.Count() - 1], "\"NationalIdentifierId\" : ", ",");
            credencialesWS(archivoWS, "Consultar");
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
            respuestaPatch = respuestaPatch + respuesta;
        }

        private void createPhoto(string personId, string tables, string datos)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Post(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/emps/" + personId + "/child/" + tables, datos, user, pass);
            respuestaPost = respuestaPost + respuesta;
        }


        //FUNCIONES
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

        //Función para obtener información de acceso al servicio de HCM
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

        //Función para llamar un servicio web de HCM
        public string LlamarWebServiceHCM(string _url, string _action, string _xmlString)
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
        private static void credencialesWS(string RutaConfiguracion, string strMetodo)
        {
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

        //Función para decodificar respuesta de la API
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        protected void Upload(object sender, EventArgs e)
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
            GuardarBitacora(ArchivoBitacora, "                              Informe de ejecución de importación de fotografías HCM Fecha: " + FechaHoraInicioEjecución + "              ");
            GuardarBitacora(ArchivoBitacora, "");
            GuardarBitacora(ArchivoBitacora, "Nombre del archivo                    DPI                         Estado                 Descripción                                    ");
            GuardarBitacora(ArchivoBitacora, "------------------------------------  --------------------------  ---------------------  ------------------------------------------------------------");


            string constr = TxtURL.Text;
            string mensajeValidacion = "";

            //ACTUALIZACION-CREACION DE IMAGEN
            if (FileUpload1.HasFile)
            {
                string uploadFolder = Request.PhysicalApplicationPath + "CargaFotografíaCS\\";
                foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                {
                    ContadorArchivos++;
                    Error = false;
                    string ExtensionFotografia = Path.GetExtension(uploadedFile.FileName).ToLower();
                    string[] ExtensionesPermitidas = { ".jpeg", ".jpg" };

                    string NombreImagen = Path.GetFileNameWithoutExtension(uploadedFile.FileName);
                    string[] result = sustituirCaracteres(NombreImagen).Split('|');
                    if (ExtensionesPermitidas.Contains(ExtensionFotografia))
                    {
                        if (result.Length >4)
                        {
                            try
                            {
                                string expand = "legislativeInfo,phones,addresses,photos";
                                string consulta = consultaGetworkers(expand, NombreImagen.Length, result[5]);

                                //Se obtienen los datos de las tablas a las cuales se les agregará información
                                string personId = getBetween(consulta, "workers/", "/child/");
                                string comIm = personId + "/child/photo/";
                                string consultaImagenes = consultaGetImagenes(comIm, NombreImagen.Length, result[5]);
                                string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");
                                string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");

                                using (Stream fs = uploadedFile.InputStream)
                                {
                                    using (BinaryReader br = new BinaryReader(fs))
                                    {
                                        try
                                        {
                                            byte[] Imagen = br.ReadBytes((Int32)fs.Length);
                                            string b64 = Convert.ToBase64String(Imagen, 0, Imagen.Length);
                                            string consultaperfil = "\"PrimaryFlag\" : ";
                                            string perfil = getBetween(consulta, consultaperfil, ",\n");
                                            var Imgn = "{\"ImageName\" : \"" + NombreImagen + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + b64 + "\"}";
                                            if (perfil == "true")
                                            {
                                                updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                                                mensajeValidacion = "La fotografía se actualizó correctamente en HCM.";
                                            }
                                            else
                                            {
                                                createPhoto(personId, "photo", Imgn);
                                                mensajeValidacion = "La fotografía se creó correctamente en HCM.";
                                            }
                                            GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "  " + NombreImagen.PadRight(26) + "  Correcto               " + mensajeValidacion.PadRight(60));
                                            ContadorArchivosCorrectos++;
                                        }
                                        catch (Exception ex)
                                        {
                                            mensajeValidacion = "Error con la base de datos de HCM, no se registró la fotografía en HCM. " + ex.Message;
                                            GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                            if (Error == false)
                                            {
                                                ContadorArchivosConError++;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                mensajeValidacion = "Error con la base de datos de HCM, no se registró la fotografía en HCM. " + "Es necesario tener registrado un identificador nacional con el nombre de la fotografía";
                                GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                                if (Error == false)
                                {
                                    ContadorArchivosConError++;
                                }
                            }
                        }
                        else
                        {
                            mensajeValidacion = "No se registró la fotografía en HCM. " + "Es necesario tener registrado un identificador nacional con el nombre de la fotografía";
                            GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                            if (Error == false)
                            {
                                ContadorArchivosConError++;
                            }
                        }
                    }
                    else
                    {
                        mensajeValidacion = "La fotografía no tiene formato .JPEG o .JPG";
                        GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "                              Error                  " + mensajeValidacion.PadRight(60));
                        if (Error == false)
                        {
                            ContadorArchivosConError++;
                        }
                    }
                }
            }
            FileUpload1.Dispose();
            FileUpload1.Attributes.Clear();
            FileUpload1 = new FileUpload();
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

        protected void DownloadFile(object sender, EventArgs e)
        {
            //DecargaFoto("");
            //int id = int.Parse((sender as LinkButton).CommandArgument);
            //byte[] bytes;
            //string fileName, contentType;
            //string constr = TxtURL.Text;
            //using (OracleConnection con = new OracleConnection(constr))
            //{
            //    using (OracleCommand cmd = new OracleCommand())
            //    {
            //        cmd.CommandText = "SELECT P.*, CASE WHEN dbms_lob.substr(EMPLOYEE_PHOTO,3,1) = hextoraw('FFD8FF') THEN 'JPG' END Extension FROM SYSADM.PS_EMPL_PHOTO P WHERE EMPLID=:Id";
            //        //cmd.CommandText = "select Name, Data, ContentType from tblFiles where Id=:Id";
            //        //cmd.Parameters.AddWithValue(":Id", id);
            //        cmd.Parameters.Add(new OracleParameter("Id", id));
            //        cmd.Connection = con;
            //        con.Open();
            //        using (OracleDataReader sdr = cmd.ExecuteReader())
            //        {
            //            sdr.Read();

            //            bytes = (byte[])sdr["EMPLOYEE_PHOTO"];
            //            contentType = sdr["Extension"].ToString();
            //            fileName = sdr["EMPLID"].ToString() + "." + contentType.ToLower();
            //            Console.WriteLine(fileName);
            //        }
            //        con.Close();
            //    }
            //}

        }

        public static Image LoadBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);
            return image;
        }

        public string ImageToByteArray(string unicodestring)
        {

            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            //Encoding Utf8 = Encoding.UTF8;

            // // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(unicodestring);

            // // Perform the conversion from one encoding to the other.
            byte[] ascibytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(ascibytes, 0, ascibytes.Length)];
            ascii.GetChars(ascibytes, 0, ascibytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            // // Display the strings created before and after the conversion.
            //MessageBox.Show("Original string is"+unicodeString);
            return asciiString;
        }

        public static string removeUnicode(string input)
        {
            Regex replace00 = new Regex("\\u0000", RegexOptions.Compiled);
            input = replace00.Replace(input, "�");
            Regex replace01 = new Regex("\u0001", RegexOptions.Compiled);
            input = replace01.Replace(input, "");
            Regex replace02 = new Regex("\u0002", RegexOptions.Compiled);
            input = replace02.Replace(input, "");
            Regex replace03 = new Regex("\u0003", RegexOptions.Compiled);
            input = replace03.Replace(input, "");
            Regex replace04 = new Regex("\u0004", RegexOptions.Compiled);
            input = replace04.Replace(input, "");
            Regex replace05 = new Regex("\u0005", RegexOptions.Compiled);
            input = replace05.Replace(input, "");
            Regex replace06 = new Regex("\u0006", RegexOptions.Compiled);
            input = replace06.Replace(input, "");
            Regex replace07 = new Regex("\u0007", RegexOptions.Compiled);
            input = replace07.Replace(input, "");
            Regex replace08 = new Regex("\u0008", RegexOptions.Compiled);
            input = replace08.Replace(input, "");
            Regex replace09 = new Regex("\t", RegexOptions.Compiled);
            input = replace09.Replace(input, "");
            Regex replace0A = new Regex("\u000A", RegexOptions.Compiled);
            input = replace0A.Replace(input, "");
            Regex replace0B = new Regex("\u000B", RegexOptions.Compiled);
            input = replace0B.Replace(input, "");
            Regex replace0C = new Regex("\u000C", RegexOptions.Compiled);
            input = replace0C.Replace(input, "");
            Regex replace0D = new Regex("\u000D", RegexOptions.Compiled);
            input = replace0D.Replace(input, "");
            Regex replace0E = new Regex("\u000E", RegexOptions.Compiled);
            input = replace0E.Replace(input, "");
            Regex replace0F = new Regex("\u000F", RegexOptions.Compiled);
            input = replace0F.Replace(input, "");
            Regex replace10 = new Regex("\u0010", RegexOptions.Compiled);
            input = replace10.Replace(input, "");
            Regex replace11 = new Regex("\u0011", RegexOptions.Compiled);
            input = replace11.Replace(input, "");
            Regex replace12 = new Regex("\u0012", RegexOptions.Compiled);
            input = replace12.Replace(input, "");
            Regex replace13 = new Regex("\u0013", RegexOptions.Compiled);
            input = replace13.Replace(input, "");
            Regex replace14 = new Regex("\u0014", RegexOptions.Compiled);
            input = replace14.Replace(input, "");
            Regex replace15 = new Regex("\u0015", RegexOptions.Compiled);
            input = replace15.Replace(input, "");
            Regex replace16 = new Regex("\u0016", RegexOptions.Compiled);
            input = replace16.Replace(input, "");
            Regex replace17 = new Regex("\u0017", RegexOptions.Compiled);
            input = replace17.Replace(input, "");
            Regex replace18 = new Regex("\u0018", RegexOptions.Compiled);
            input = replace18.Replace(input, "");
            Regex replace19 = new Regex("\u0019", RegexOptions.Compiled);
            input = replace19.Replace(input, "");
            Regex replace1A = new Regex("\u001A", RegexOptions.Compiled);
            input = replace1A.Replace(input, "");
            Regex replace1B = new Regex("\u001B", RegexOptions.Compiled);
            input = replace1B.Replace(input, "");
            Regex replace1C = new Regex("\u001C", RegexOptions.Compiled);
            input = replace1C.Replace(input, "");
            Regex replace1D = new Regex("\u001D", RegexOptions.Compiled);
            input = replace1D.Replace(input, "");
            Regex replace1E = new Regex("\u001E", RegexOptions.Compiled);
            input = replace1E.Replace(input, "");
            Regex replace1F = new Regex("\u001F", RegexOptions.Compiled);
            input = replace1F.Replace(input, "");
            Regex replace20 = new Regex("\u0020", RegexOptions.Compiled);
            input = replace20.Replace(input, "");
            Regex replace21 = new Regex("\\u0021", RegexOptions.Compiled);
            input = replace21.Replace(input, "!");
            Regex replace22 = new Regex("\\u0022", RegexOptions.Compiled);
            input = replace22.Replace(input, "\"");
            Regex replace23 = new Regex("\\u0023", RegexOptions.Compiled);
            input = replace23.Replace(input, "#");
            Regex replace24 = new Regex("\\u0024", RegexOptions.Compiled);
            input = replace24.Replace(input, "$");
            Regex replace25 = new Regex("\\u0025", RegexOptions.Compiled);
            input = replace25.Replace(input, "%");
            Regex replace26 = new Regex("\\u0026", RegexOptions.Compiled);
            input = replace26.Replace(input, "&");
            Regex replace27 = new Regex("\\u0027", RegexOptions.Compiled);
            input = replace27.Replace(input, "");
            Regex replace28 = new Regex("\\u0028", RegexOptions.Compiled);
            input = replace28.Replace(input, "(");
            Regex replace29 = new Regex("\\u0029", RegexOptions.Compiled);
            input = replace29.Replace(input, ");");
            Regex replace2A = new Regex("\\u002A", RegexOptions.Compiled);
            input = replace2A.Replace(input, "*");
            Regex replace2B = new Regex("\\u002B", RegexOptions.Compiled);
            input = replace2B.Replace(input, "+");
            Regex replace2C = new Regex("\\u002C", RegexOptions.Compiled);
            input = replace2C.Replace(input, ",");
            Regex replace2D = new Regex("\\u002D", RegexOptions.Compiled);
            input = replace2D.Replace(input, "-");
            Regex replace2E = new Regex("\\u002E", RegexOptions.Compiled);
            input = replace2E.Replace(input, ".");
            Regex replace2F = new Regex("\\u002F", RegexOptions.Compiled);
            input = replace2F.Replace(input, "/");
            Regex replace30 = new Regex("\\u0030", RegexOptions.Compiled);
            input = replace30.Replace(input, "0");
            Regex replace31 = new Regex("\\u0031", RegexOptions.Compiled);
            input = replace31.Replace(input, "1");
            Regex replace32 = new Regex("\\u0032", RegexOptions.Compiled);
            input = replace32.Replace(input, "2");
            Regex replace33 = new Regex("\\u0033", RegexOptions.Compiled);
            input = replace33.Replace(input, "3");
            Regex replace34 = new Regex("\\u0034", RegexOptions.Compiled);
            input = replace34.Replace(input, "4");
            Regex replace35 = new Regex("\\u0035", RegexOptions.Compiled);
            input = replace35.Replace(input, "5");
            Regex replace36 = new Regex("\\u0036", RegexOptions.Compiled);
            input = replace36.Replace(input, "6");
            Regex replace37 = new Regex("\\u0037", RegexOptions.Compiled);
            input = replace37.Replace(input, "7");
            Regex replace38 = new Regex("\\u0038", RegexOptions.Compiled);
            input = replace38.Replace(input, "8");
            Regex replace39 = new Regex("\\u0039", RegexOptions.Compiled);
            input = replace39.Replace(input, "9");
            Regex replace3A = new Regex("\\u003A", RegexOptions.Compiled);
            input = replace3A.Replace(input, ":");
            Regex replace3B = new Regex("\\u003B", RegexOptions.Compiled);
            input = replace3B.Replace(input, ";");
            Regex replace3C = new Regex("\\u003C", RegexOptions.Compiled);
            input = replace3C.Replace(input, "<");
            Regex replace3D = new Regex("\\u003D", RegexOptions.Compiled);
            input = replace3D.Replace(input, "=");
            Regex replace3E = new Regex("\\u003E", RegexOptions.Compiled);
            input = replace3E.Replace(input, ">");
            Regex replace3F = new Regex("\\u003F", RegexOptions.Compiled);
            input = replace3F.Replace(input, "?");
            Regex replace40 = new Regex("\\u0040", RegexOptions.Compiled);
            input = replace40.Replace(input, "@");
            Regex replace41 = new Regex("\\u0041", RegexOptions.Compiled);
            input = replace41.Replace(input, "A");
            Regex replace42 = new Regex("\\u0042", RegexOptions.Compiled);
            input = replace42.Replace(input, "B");
            Regex replace43 = new Regex("\\u0043", RegexOptions.Compiled);
            input = replace43.Replace(input, "C");
            Regex replace44 = new Regex("\\u0044", RegexOptions.Compiled);
            input = replace44.Replace(input, "D");
            Regex replace45 = new Regex("\\u0045", RegexOptions.Compiled);
            input = replace45.Replace(input, "E");
            Regex replace46 = new Regex("\\u0046", RegexOptions.Compiled);
            input = replace46.Replace(input, "F");
            Regex replace47 = new Regex("\\u0047", RegexOptions.Compiled);
            input = replace47.Replace(input, "G");
            Regex replace48 = new Regex("\\u0048", RegexOptions.Compiled);
            input = replace48.Replace(input, "H");
            Regex replace49 = new Regex("\\u0049", RegexOptions.Compiled);
            input = replace49.Replace(input, "I");
            Regex replace4A = new Regex("\\u004A", RegexOptions.Compiled);
            input = replace4A.Replace(input, "J");
            Regex replace4B = new Regex("\\u004B", RegexOptions.Compiled);
            input = replace4B.Replace(input, "K");
            Regex replace4C = new Regex("\\u004C", RegexOptions.Compiled);
            input = replace4C.Replace(input, "L");
            Regex replace4D = new Regex("\\u004D", RegexOptions.Compiled);
            input = replace4D.Replace(input, "M");
            Regex replace4E = new Regex("\\u004E", RegexOptions.Compiled);
            input = replace4E.Replace(input, "N");
            Regex replace4F = new Regex("\\u004F", RegexOptions.Compiled);
            input = replace4F.Replace(input, "O");
            Regex replace50 = new Regex("\\u0050", RegexOptions.Compiled);
            input = replace50.Replace(input, "P");
            Regex replace51 = new Regex("\\u0051", RegexOptions.Compiled);
            input = replace51.Replace(input, "Q");
            Regex replace52 = new Regex("\\u0052", RegexOptions.Compiled);
            input = replace52.Replace(input, "R");
            Regex replace53 = new Regex("\\u0053", RegexOptions.Compiled);
            input = replace53.Replace(input, "S");
            Regex replace54 = new Regex("\\u0054", RegexOptions.Compiled);
            input = replace54.Replace(input, "T");
            Regex replace55 = new Regex("\\u0055", RegexOptions.Compiled);
            input = replace55.Replace(input, "U");
            Regex replace56 = new Regex("\\u0056", RegexOptions.Compiled);
            input = replace56.Replace(input, "V");
            Regex replace57 = new Regex("\\u0057", RegexOptions.Compiled);
            input = replace57.Replace(input, "W");
            Regex replace58 = new Regex("\\u0058", RegexOptions.Compiled);
            input = replace58.Replace(input, "X");
            Regex replace59 = new Regex("\\u0059", RegexOptions.Compiled);
            input = replace59.Replace(input, "Y");
            Regex replace5A = new Regex("\\u005A", RegexOptions.Compiled);
            input = replace5A.Replace(input, "Z");
            Regex replace5B = new Regex("\\u005B", RegexOptions.Compiled);
            input = replace5B.Replace(input, "[");
            Regex replace5C = new Regex("\\u005C", RegexOptions.Compiled);
            input = replace5C.Replace(input, "\\");
            Regex replace5D = new Regex("\\u005D", RegexOptions.Compiled);
            input = replace5D.Replace(input, "]");
            Regex replace5E = new Regex("\\u005E", RegexOptions.Compiled);
            input = replace5E.Replace(input, "^");
            Regex replace5F = new Regex("\\u005F", RegexOptions.Compiled);
            input = replace5F.Replace(input, "_");
            Regex replace60 = new Regex("\\u0060", RegexOptions.Compiled);
            input = replace60.Replace(input, "`");
            Regex replace61 = new Regex("\\u0061", RegexOptions.Compiled);
            input = replace61.Replace(input, "a");
            Regex replace62 = new Regex("\\u0062", RegexOptions.Compiled);
            input = replace62.Replace(input, "b");
            Regex replace63 = new Regex("\\u0063", RegexOptions.Compiled);
            input = replace63.Replace(input, "c");
            Regex replace64 = new Regex("\\u0064", RegexOptions.Compiled);
            input = replace64.Replace(input, "d");
            Regex replace65 = new Regex("\\u0065", RegexOptions.Compiled);
            input = replace65.Replace(input, "e");
            Regex replace66 = new Regex("\\u0066", RegexOptions.Compiled);
            input = replace66.Replace(input, "f");
            Regex replace67 = new Regex("\\u0067", RegexOptions.Compiled);
            input = replace67.Replace(input, "g");
            Regex replace68 = new Regex("\\u0068", RegexOptions.Compiled);
            input = replace68.Replace(input, "h");
            Regex replace69 = new Regex("\\u0069", RegexOptions.Compiled);
            input = replace69.Replace(input, "i");
            Regex replace6A = new Regex("\\u006A", RegexOptions.Compiled);
            input = replace6A.Replace(input, "j");
            Regex replace6B = new Regex("\\u006B", RegexOptions.Compiled);
            input = replace6B.Replace(input, "k");
            Regex replace6C = new Regex("\\u006C", RegexOptions.Compiled);
            input = replace6C.Replace(input, "l");
            Regex replace6D = new Regex("\\u006D", RegexOptions.Compiled);
            input = replace6D.Replace(input, "m");
            Regex replace6E = new Regex("\\u006E", RegexOptions.Compiled);
            input = replace6E.Replace(input, "n");
            Regex replace6F = new Regex("\\u006F", RegexOptions.Compiled);
            input = replace6F.Replace(input, "o");
            Regex replace70 = new Regex("\\u0070", RegexOptions.Compiled);
            input = replace70.Replace(input, "p");
            Regex replace71 = new Regex("\\u0071", RegexOptions.Compiled);
            input = replace71.Replace(input, "q");
            Regex replace72 = new Regex("\\u0072", RegexOptions.Compiled);
            input = replace72.Replace(input, "r");
            Regex replace73 = new Regex("\\u0073", RegexOptions.Compiled);
            input = replace73.Replace(input, "s");
            Regex replace74 = new Regex("\\u0074", RegexOptions.Compiled);
            input = replace74.Replace(input, "t");
            Regex replace75 = new Regex("\\u0075", RegexOptions.Compiled);
            input = replace75.Replace(input, "u");
            Regex replace76 = new Regex("\\u0076", RegexOptions.Compiled);
            input = replace76.Replace(input, "v");
            Regex replace77 = new Regex("\\u0077", RegexOptions.Compiled);
            input = replace77.Replace(input, "w");
            Regex replace78 = new Regex("\\u0078", RegexOptions.Compiled);
            input = replace78.Replace(input, "x");
            Regex replace79 = new Regex("\\u0079", RegexOptions.Compiled);
            input = replace79.Replace(input, "y");
            Regex replace7A = new Regex("\\u007A", RegexOptions.Compiled);
            input = replace7A.Replace(input, "z");
            Regex replace7B = new Regex("\\u007B", RegexOptions.Compiled);
            input = replace7B.Replace(input, "{");
            Regex replace7C = new Regex("\\u007C", RegexOptions.Compiled);
            input = replace7C.Replace(input, "|");
            Regex replace7D = new Regex("\\u007D", RegexOptions.Compiled);
            input = replace7D.Replace(input, "}");
            Regex replace7E = new Regex("\\u007E", RegexOptions.Compiled);
            input = replace7E.Replace(input, "~");
            Regex replace7F = new Regex("\\u007F", RegexOptions.Compiled);
            input = replace7F.Replace(input, "");
            return input;
        }
        public static string UnescapeUnicode(string str)
        {
            Regex Regex = new Regex(@"\\[uU]([0-9A-Fa-f]{4})");
            return Regex.Replace(str,
                match => ((char)int.Parse(match.Value.Substring(2),
                    NumberStyles.HexNumber)).ToString());
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

        //Crea el cuerpo que se utiliza para consultar los empleados por DPI
        private static void CuerpoConsultaPorDPI(string idPersona, string passwordServicio, string dpi)
        {
            Variables.soapBody = @"<?xml version=""1.0""?>
                                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:v2=""http://xmlns.oracle.com/oxp/service/v2"">
                                <soapenv:Header/>
                                <soapenv:Body>
                                  <v2:runReport>
                                     <v2:reportRequest>
                                        <v2:attributeFormat>csv</v2:attributeFormat>            
                                        <v2:flattenXML>false</v2:flattenXML>
                                        <v2:parameterNameValues>
                                        <v2:listOfParamNameValues>
                                           <!--1st Parameter of BIP Report-->    
                                            <v2:item>
                                                <v2:name>DPI</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + dpi + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Carga Masiva/ReporteDPICargaMasiva.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        public string sustituirCaracteres(string dpi)
        {
            string sustituto = DecodeStringFromBase64(Consultar(dpi)).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @"\n+", "");
            return sustituto;
        }

    }
}