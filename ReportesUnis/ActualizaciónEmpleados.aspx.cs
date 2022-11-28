using System;
using System.Web.Services;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ReportesUnis.API;
using System.Web.Services.Description;
using Microsoft.Win32;
using NPOI.SS.Formula.Functions;

namespace ReportesUnis
{
    public partial class ActualizaciónEmpleados : System.Web.UI.Page
    {
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.dat");
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        public static class StringExtensions
        {
            public static String RemoveEnd(String str, int len)
            {
                if (str.Length < len)
                {
                    return string.Empty;
                }

                return str.Substring(0, str.Length - len);
            }
        }
        int aux = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                matrizDatos();
                aux = 2;
                listadoMunicipios();
                aux = 3;
                listadoZonas();
                aux = 4;
                PaisInicial.Text = Pais.Text;
                if (String.IsNullOrEmpty(txtdPI.Text))
                {
                    BtnActualizar.Visible = false;
                    lblActualizacion.Text = "El usuario utilizado no se encuentra registrado como empleados";
                    tabla.Visible = false;
                    FileUpload1.Visible = false;
                    lblfoto.Visible = false;
                }
            }
            else
                aux = 2;

        }

        [WebMethod]
        public string Consultar()
        {
            //Se limpian variables para guardar la nueva información
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguraciones, "Consultar");

            if (aux == 0)
            {
                //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                CuerpoConsultaPorDPI(Variables.wsUsuario, Variables.wsPassword, TextUser.Text);
            }
            else if (aux == 1)
            {
                CuerpoConsultaDepartamento(Variables.wsUsuario, Variables.wsPassword, cMBpAIS.SelectedValue);
            }
            else if (aux == 2)
            {
                CuerpoConsultaPorMunicipio(Variables.wsUsuario, Variables.wsPassword, CmbDepartamento.SelectedValue);
            }
            else if (aux == 3)
            {
                CuerpoConsultaPorZona(Variables.wsUsuario, Variables.wsPassword, CmbMunicipio.SelectedValue);
            }
            else if (aux == 4)
            {
                CuerpoConsultaPorPais(Variables.wsUsuario, Variables.wsPassword);
            }
            else if (aux == 5)
            {
                CuerpoConsultaCodigoPais(Variables.wsUsuario, Variables.wsPassword, Pais.Text);
            }

            //Crea un documento de respuesta Campus
            System.Xml.XmlDocument xmlDocumentoRespuestaCampus = new System.Xml.XmlDocument();

            // Indica que no se mantengan los espacios y saltos de línea
            xmlDocumentoRespuestaCampus.PreserveWhitespace = false;

            try
            {
                // Carga el XML de respuesta de Campus
                xmlDocumentoRespuestaCampus.LoadXml(LlamarWebServiceHCM(Variables.wsUrl, Variables.wsAction, Variables.soapBody));
            }
            catch (WebException)
            {
                //Crea la respuesta cuando se genera una excepción web.
                Variables.strDocumentoRespuesta = Respuesta("05", "ERROR AL CONSULTAR EL REPORTE");
                return Variables.strDocumentoRespuesta;

            }
            XmlNodeList elemList = xmlDocumentoRespuestaCampus.GetElementsByTagName("reportBytes");
            return elemList[0].InnerText.ToString();
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
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Actualización/InformeActualizarEmpleados.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        //Crea el cuerpo que se utiliza para consultar el codigo del pais
        private static void CuerpoConsultaCodigoPais(string idPersona, string passwordServicio, string pais)
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
                                                <v2:name>COUNTRY</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + pais + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Catalogos/InformacionCodigoPais.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los municipios
        private static void CuerpoConsultaPorMunicipio(string idPersona, string passwordServicio, string departamento)
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
                                                <v2:name>COUNTRY</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + departamento + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Catalogos/RInformacionMunicipios.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        //Crea el cuerpo que se utiliza para consultar las zonas
        private static void CuerpoConsultaPorPais(string idPersona, string passwordServicio)
        {
            Variables.soapBody = @"<?xml version=""1.0""?>
                                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:v2=""http://xmlns.oracle.com/oxp/service/v2"">
                                <soapenv:Header/>
                                <soapenv:Body>                  
                                    <v2:runReport>
                                        <v2:reportRequest>
                                            <v2:attributeFormat>csv</v2:attributeFormat>                                            
                                            <v2:flattenXML>false</v2:flattenXML>                                        
                                            <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Catalogos/RInformacionPaises.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                        </v2:reportRequest>
                                        <v2:userID>" + idPersona + @"</v2:userID>
                                        <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }
        //Crea el cuerpo que se utiliza para consultar las zonas
        private static void CuerpoConsultaPorZona(string idPersona, string passwordServicio, string municipio)
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
                                                <v2:name>COUNTRY</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + municipio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Catalogos/InformacionZonasGT.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los departamentos
        private static void CuerpoConsultaDepartamento(string idPersona, string passwordServicio, string pais)
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
                                                <v2:name>PAIS</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + pais + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Catalogos/RInformacionDepartamentos.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
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

        //Función para obtener información de acceso al servicio de Campus
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
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        //Sustituye las comillas dobles y elimina los primeros caracteres que corresponden a los Headers
        public string sustituirCaracteres()
        {
            string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @"\n+", "");

            try
            {
                if (aux == 0)
                {
                    int largo = 0;
                    string nombre = TextUser.Text.TrimEnd(' ');
                    largo = nombre.Length + 156;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (aux == 1)
                {
                    try
                    {
                        int pais = cMBpAIS.SelectedValue.Length + 44;
                        sustituto = sustituto.Remove(0, pais);
                    }
                    catch (Exception)
                    {

                        sustituto = "";
                    }
                }
                else if (aux == 2)
                {
                    try
                    {
                        int mun = CmbDepartamento.SelectedValue.Length + 28;
                        sustituto = sustituto.Remove(0, mun);
                        sustituto = sustituto.TrimEnd('|');
                    }
                    catch (Exception)
                    {

                        sustituto = ("");
                    }
                }
                else if (aux == 4)
                {
                    int largo = 0;
                    string nombre = TextUser.Text.TrimEnd(' ');
                    largo = nombre.Length + 141;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (aux == 3)
                {
                    int mun = CmbMunicipio.Text.Length + 24;
                    if (sustituto.Length > mun)
                        sustituto = sustituto.Remove(0, mun);
                }
                else if (aux == 5)
                {
                    int pais = cMBpAIS.Text.Length + 25;
                    sustituto = sustituto.Remove(0, pais);
                }
            }
            catch (Exception)
            {

                sustituto = sustituto;
            }

            return sustituto;
        }

        public void matrizDatos()
        {
            string[] result = sustituirCaracteres().Split('|');
            decimal registros = 0;
            decimal count = 0;
            int datos = 0;
            string[,] arrlist;
            int valor = 15;

            aux = 4;
            listaPaises();

            registros = result.Count() / valor;
            count = Math.Round(registros, 0);
            arrlist = new string[Convert.ToInt32(count), valor];

            for (int i = 0; i < count; i++)
            {
                for (int k = 0; k < valor; k++)
                {
                    arrlist[i, k] = result[datos];
                    datos++;
                }
            }

            try
            {
                var estado = "";
                var bday = "";
                var dia = "";
                var mes = "";
                var anio = "";
                DataSetLocalRpt dsReporte = new DataSetLocalRpt();
                try
                {
                    if (valor == 15)
                    {
                        //Generacion de matriz para llenado de grid desde una consulta
                        for (int i = 0; i < count; i++)
                        {
                            //DataRow newFila = dsReporte.Tables["RptEmpleados"].NewRow();
                            txtNombre.Text = (arrlist[i, 1] ?? "").ToString();
                            txtApellido.Text = (arrlist[i, 2] ?? "").ToString();
                            txtdPI.Text = (arrlist[i, 3] ?? "").ToString();
                            txtFacultad.Text = (arrlist[i, 4] ?? "").ToString();
                            txtTelefono.Text = (arrlist[i, 5] ?? "").ToString();
                            if (!arrlist[i, 6].ToString().Equals(""))
                            {
                                if (arrlist[i, 6].ToString().Equals("1"))
                                {
                                    estado = "Soltero";
                                }
                                else if (arrlist[i, 6].ToString().Equals("2"))
                                {
                                    estado = "Casado";
                                }
                            }
                            else
                            {
                                estado = "Sin Información";
                            }

                            CmbEstado.SelectedValue = estado.ToString();

                            if (!arrlist[i, 7].ToString().Equals(""))
                            {
                                bday = arrlist[i, 7].ToString().Substring(0, 10);
                                anio = bday.Substring(0, 4);
                                mes = bday.Substring(5, 2);
                                dia = bday.Substring(8, 2);
                                bday = dia + "-" + mes + "-" + anio;
                                ;
                            }
                            else
                            {
                                bday = "Unknown";
                            }

                            txtCumple.Text = bday;


                            txtDireccion.Text = arrlist[i, 8].ToString();
                            txtDireccion2.Text = arrlist[i, 12].ToString();
                            cMBpAIS.SelectedValue = (arrlist[i, 11] ?? "").ToString();
                            aux = 1;
                            listaDepartamentos();
                            aux = 0;
                            CmbMunicipio.SelectedValue = (arrlist[i, 9] ?? "").ToString();
                            CmbDepartamento.SelectedValue = (arrlist[i, 10] ?? "").ToString();
                            UserEmplid.Text = (arrlist[i, 14] ?? "").ToString();
                            txtZona.Text = (arrlist[i, 13] ?? "").ToString();

                            //dsReporte.Tables["RptEmpleados"].Rows.Add(newFila);
                        }
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.ToString());
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }
        public void listaDepartamentos()
        {
            string[] result;
            int count = 0;
            int pais = cMBpAIS.SelectedValue.ToString().Length;
            string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\r');
            sustituto = Regex.Replace(sustituto, @"\n+", "");
            sustituto = Regex.Replace(sustituto, @"\r", "");
            if (cMBpAIS.Text.Equals("Guatemala"))
            {
                result = new string[23];
            }
            else
            {
                result = new string[3];
            }

            result = sustituirCaracteres().Split('|');
            count = result.Length / 2;
            string[] resultado = new string[count];
            string[,] arrlist;
            int datos = 0;
            arrlist = new string[Convert.ToInt32(count), 2];

            try
            {

                for (int i = 0; i < count; i++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        arrlist[i, k] = result[datos];
                        if (k == 0)
                        {
                            resultado[i] = arrlist[i, k];
                        }
                        datos++;
                    }
                }
                if (cMBpAIS.Text.Equals("Guatemala"))
                {
                    Pais.Text = StringExtensions.RemoveEnd(arrlist[0, 1], pais);
                    CmbDepartamento.DataSource = resultado;
                }
                else
                {
                    Pais.Text = arrlist[0, 1];
                    resultado[0] = arrlist[0, 0];
                    CmbDepartamento.DataSource = resultado;
                }



                CmbDepartamento.DataTextField = "";
                CmbDepartamento.DataValueField = "";
                CmbDepartamento.DataBind();
            }
            catch (Exception)
            {
                CmbDepartamento.DataSource = "";
                CmbDepartamento.DataTextField = "";
                CmbDepartamento.DataValueField = "";
                CmbDepartamento.DataBind();
            }
        }

        public void listaPaises()
        {
            aux = 4;
            string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @"\n+", "|");

            int largo = 20;
            sustituto = sustituto.Remove(0, largo);
            sustituto = sustituto + "-";
            sustituto = sustituto.TrimEnd('|');

            string[] result = new string[23];
            result = sustituto.Split('|');
            cMBpAIS.DataSource = result;
            cMBpAIS.DataTextField = "";
            cMBpAIS.DataValueField = "";
            cMBpAIS.DataBind();
        }

        public void listadoMunicipios()
        {
            int count = 0;
            int depto = CmbDepartamento.SelectedValue.ToString().Length;
            string[] result = sustituirCaracteres().Split('|');
            count = result.Length;
            string[] resultado = new string[count];
            //string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\r');
            //sustituto = Regex.Replace(sustituto, @"\n+", "");
            //sustituto = Regex.Replace(sustituto, @"\r", "");

            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (count == 1 || i == count - 1)
                    {
                        resultado[i] = result[i];
                    }
                    else
                    {
                        string palabra = result[i];
                        resultado[i] = StringExtensions.RemoveEnd(palabra, depto);
                    }
                }

                CmbMunicipio.DataSource = resultado;
                CmbMunicipio.DataTextField = "";
                CmbMunicipio.DataValueField = "";
                CmbMunicipio.DataBind();
            }
            catch (Exception)
            {
                CmbMunicipio.DataSource = "-";
                CmbMunicipio.DataTextField = "-";
                CmbMunicipio.DataValueField = "-";
            }
        }

        public void listadoZonas()
        {
            int count = 0;
            int mun = CmbMunicipio.SelectedValue.ToString().Length;
            string[] result = sustituirCaracteres().Split('|');
            count = result.Count();
            string[,] arrlist;
            string[] resultado = new string[count];
            arrlist = new string[Convert.ToInt32(count), 2];

            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (i == count - 1)
                    {
                        resultado[i] = "-";
                    }
                    else if (i != count - 1)
                    {
                        string palabra = result[i];
                        resultado[i] = StringExtensions.RemoveEnd(palabra, mun);
                    }
                    else
                    {
                        string palabra = result[i];
                        resultado[i] = palabra;
                    }
                }

                txtZona.DataSource = resultado;
                txtZona.DataTextField = "";
                txtZona.DataValueField = "";
                txtZona.DataBind();
            }
            catch (Exception)
            {
                txtZona.DataSource = "";
                txtZona.DataTextField = "";
                txtZona.DataValueField = "";
                txtZona.DataBind();
            }
        }

        public string CodigoPais()
        {
            string cadena = DecodeStringFromBase64(Consultar()).Replace('"', '\n');
            cadena = Regex.Replace(cadena, @"\n+", "");
            string[] result = cadena.Split('|');
            try
            {
                return result[2].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            aux = 2;
            listadoMunicipios();
        }

        /// CONSUMO DE API
        ConsumoAPI api = new ConsumoAPI();
        int respuestaPatch = 0;
        int respuestaPost = 0;

        private string consultaGetworkers(string expand)
        {
            string consulta = consultaUser("nationalIdentifiers", UserEmplid.Text);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = UserEmplid.Text;
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            var dtFechaBuscarPersona = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string respuesta = api.Get(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/workers?q=PersonId=" + personID + "&effectiveDate=" + dtFechaBuscarPersona + "&expand=" + expand, user, pass);
            return respuesta;
        }

        private string consultaGetImagenes(string consultar)
        {
            string consulta = consultaUser("nationalIdentifiers", UserEmplid.Text);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
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

        private void create(string personId, string tables, string datos, string EXTEN)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Post(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/" + EXTEN + personId + "/child/" + tables, datos, user, pass);
            respuestaPost = respuestaPost + respuesta;
        }



        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (!cMBpAIS.Text.Equals("-") && !CmbMunicipio.Text.Equals("-") && !CmbDepartamento.Text.Equals("-"))
            {
                string FechaHoraInicioEjecución = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                int ContadorArchivos = 0;
                int ContadorArchivosCorrectos = 0;
                int ContadorArchivosConError = 0;

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
                //Obtener se obtiene toda la información del empleado
                string expand = "legislativeInfo,phones,addresses,photos";
                string consulta = consultaGetworkers(expand);
                aux = 5;
                string country = CodigoPais();

                //Se obtienen los id's de las tablas a las cuales se les agregará información
                string personId = getBetween(consulta, "workers/", "/child/");
                string PhoneId = getBetween(consulta, "\"PhoneId\" : ", ",\n");
                string AddressId = getBetween(consulta, "child/addresses/", "\",\n");
                string PersonLegislativeId = getBetween(consulta, "child/legislativeInfo/", "\",\n");
                string pli = getBetween(consulta, "\"PersonLegislativeId\" : ", ",");
                string effective = getBetween(consulta, "\"PersonLegislativeId\" : " + pli + ",\n      \"EffectiveStartDate\" : \"", "\",\n");

                string comIm = personId + "/child/photo/";
                string consultaImagenes = consultaGetImagenes(comIm);
                string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
                string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");

                //ACTUALIZACION-CREACION DE IMAGEN
                if (FileUpload1.HasFile)
                {
                    foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                    {
                        //Nombre de la fotografía cargada (Sin extensión)
                        string NombreImagen = Path.GetFileNameWithoutExtension(uploadedFile.FileName);

                        using (Stream fs = uploadedFile.InputStream)
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                byte[] Imagen = br.ReadBytes((Int32)fs.Length);
                                string b64 = Convert.ToBase64String(Imagen, 0, Imagen.Length);
                                string consultaperfil = "\"PrimaryFlag\" : ";
                                string perfil = getBetween(consulta, consultaperfil, ",\n");
                                var Imgn = "{\"ImageName\" : \"" + NombreImagen + "\",\"PrimaryFlag\" : \"Y\", \"Image\":\"" + b64 + "\"}";
                                if (perfil == "true" && !String.IsNullOrEmpty(ImageId))
                                {
                                    updatePatch(Imgn, personId, "photo", ImageId, "photo", "", "emps/");
                                    mensajeValidacion = "y la fotografía se actualizó correctamente en HCM.";
                                }
                                else
                                {
                                    create(personId, "photo", Imgn, "emps/");
                                    mensajeValidacion = "y la fotografía se creó correctamente en HCM.";
                                }
                                GuardarBitacora(ArchivoBitacora, NombreImagen.PadRight(36) + "  " + NombreImagen.PadRight(26) + "  Correcto               " + mensajeValidacion.PadRight(60));
                                ContadorArchivosCorrectos++;
                            }
                        }
                    }
                }
                else
                {
                    mensajeValidacion = " , no se encontró ninguna fotografía para almacenar.";
                }

                effective = effective.Replace("\"", "");
                string departamento = CmbDepartamento.Text;
                if (departamento.Equals("-"))
                    departamento = "";
                //Se crea el body que se enviará a cada tabla
                var estadoC = "{\"MaritalStatus\": " + estadoCivil(CmbEstado.Text) + "}";
                var phoneNumber = "{\"PhoneNumber\": \"" + txtTelefono.Text + "\"}";
                var Address = "{\"AddressLine1\": \"" + txtDireccion.Text + "\", \"AddressLine2\": \"" + txtDireccion2.Text + "\",\"AddressType\" :\"HOME\",\"Region1\": \"" + departamento + "\",\"TownOrCity\": \"" + CmbMunicipio.Text + "\",\"AddlAddressAttribute3\": \"" + txtZona.Text + "\",\"Country\": \"" + country + "\"}";
                //Actualiza por medio del metodo PATCH            
                updatePatch(phoneNumber, personId, "phones", PhoneId, "phones", "", "workers/");
                updatePatch(estadoC, personId, "legislativeInfo", PersonLegislativeId, "legislativeInfo", effective, "workers/");
                if (PaisInicial.Text == cMBpAIS.Text)
                {
                    updatePatch(Address, personId, "addresses", AddressId, "addresses", effective, "workers/");
                }
                else
                    create(personId, "addresses", Address, "workers/");
                int au = respuestaPost;
                if (respuestaPatch != 0 || respuestaPost != 0)
                    lblActualizacion.Text = "Ocurrió un problema al actualizar su información";
                else
                {
                    lblActualizacion.Text = "Su información fue actualizada correctamente " + mensajeValidacion;
                    if (mensajeValidacion == " pero no se encontró ninguna fotografía para almacenar.")
                        GuardarBitacora(ArchivoBitacora, "---".PadRight(36) + "  " + Context.User.Identity.Name.Replace("@unis.edu.gt", "").PadRight(26) + "  No se ingresó ninguna imagen               ".PadRight(60));

                }
                // lblActualizacion.Text = "Su información fue actualizada correctamente " + mensajeValidacion;

                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "-----------------------------------------------------------------------------------------------");
                GuardarBitacora(ArchivoBitacora, "Total de archivos: " + ContadorArchivos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos cargados correctamente: " + ContadorArchivosCorrectos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos con error: " + ContadorArchivosConError.ToString());
            }
            else
            {
                lblActualizacion.Text = "Es necesario seleccionar un país, departamento y muncipio";
            }

        }

        //Funcion para extraerlos Id's
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

        public string estadoCivil(string estadoCivilTexto)
        {
            var estado = "";
            if (estadoCivilTexto.Equals("Soltero"))
            {
                estado = "1";
            }
            else if (estadoCivilTexto.Equals("Casado"))
            {
                estado = "2";
            }
            else
            {
                estado = null;
            }

            return estado;
        }
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            aux = 3;
            listadoZonas();
        }

        protected void cMBpAIS_SelectedIndexChanged(object sender, EventArgs e)
        {
            aux = 1;
            listaDepartamentos();
            aux = 2;
            listadoMunicipios();
            aux = 3;
            listadoZonas();
        }

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

    }
}