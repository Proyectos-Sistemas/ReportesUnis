using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Globalization;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Web.UI.WebControls;
using SpreadsheetLight;
using System.Web.UI;
using NPOI.Util;
using System.IO.Compression;
using System.Diagnostics;
using DocumentFormat.OpenXml.Bibliography;
using System.Web.UI.WebControls.WebParts;
using Newtonsoft.Json;

namespace ReportesUnis
{
    public partial class ReporteCamarasTermicasEmpleados : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        int desc = 0;
        string nombre = "ImagenesEmpleados" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".zip";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("FIRST_NAME");
            dt.Columns.Add("LAST_NAME");
            dt.Columns.Add("ID");
            dt.Columns.Add("TYPE");
            dt.Columns.Add("PERSON_GROUP");
            dt.Columns.Add("GENDER");
            dt.Columns.Add("Start_Time_of_Effective_Period");
            dt.Columns.Add("End_Time_of_Effective_Period");
            dt.Columns.Add("CARD");
            dt.Columns.Add("EMAIL");
            dt.Columns.Add("PHONE");
            dt.Columns.Add("REMARK");
            dt.Columns.Add("DOCK_STATION_LOGIN_PASSWORD");
            dt.Columns.Add("SUPPORTISSUEDCUSTOMPROPERTIES");
            dt.Columns.Add("SKINSURFACE_TEMPERATURE");
            dt.Columns.Add("TEMPERATURE_STATUS");
            dt.Columns.Add("DEPARTAMENTO");
            dt.Columns.Add("EMPLID");


            dr["FIRST_NAME"] = String.Empty;
            dr["LAST_NAME"] = String.Empty;
            dr["ID"] = String.Empty;
            dr["TYPE"] = String.Empty;
            dr["PERSON_GROUP"] = String.Empty;
            dr["GENDER"] = String.Empty;
            dr["Start_Time_of_Effective_Period"] = String.Empty;
            dr["End_Time_of_Effective_Period"] = String.Empty;
            dr["CARD"] = String.Empty;
            dr["EMAIL"] = String.Empty;
            dr["PHONE"] = String.Empty;
            dr["REMARK"] = String.Empty;
            dr["DOCK_STATION_LOGIN_PASSWORD"] = String.Empty;
            dr["SUPPORTISSUEDCUSTOMPROPERTIES"] = String.Empty;
            dr["SKINSURFACE_TEMPERATURE"] = String.Empty;
            dr["TEMPERATURE_STATUS"] = String.Empty;
            dr["DEPARTAMENTO"] = String.Empty;
            dr["EMPLID"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewReporteCT.DataSource = dt;
            this.GridViewReporteCT.DataBind();
        }

        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.dat");

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

        [WebMethod]
        public string ConsultaWS(string dpi)
        {
            string busqueda = "", FI = "", FF = "", departamentos = "";

            if (!String.IsNullOrEmpty(CldrCiclosInicio.Text))
            {
                FI = Convert.ToDateTime(CldrCiclosInicio.Text).ToString("dd-MM-yyyy");
                FF = Convert.ToDateTime(CldrCiclosFin.Text).ToString("dd-MM-yyyy");
            }

            if (!String.IsNullOrEmpty(TxtBuscador.Text))
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string inicial = TxtBuscador.Text.Substring(0, 1).ToUpper();
                string letras = TxtBuscador.Text.Substring(1, TxtBuscador.Text.Length - 1).Trim(' ').ToLower();
                busqueda = textInfo.ToTitleCase(inicial + letras);
            }
            //Se limpian variables para guardar la nueva información
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguraciones, "Consultar");

            if (desc == 0)
            {
                if (LbxBusqueda.Text.Equals("Nombre"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaPorNombre(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Apellido"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaPorApellido(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("ID"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaPorID(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Departamento"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    departamentos = getBetween(departamentosAPEX(busqueda.ToUpper()), "\"items\": [", "],");
                    departamentos = departamentos.Replace("\r\n    {\r\n      \"departamento\": \"", "<v2:item>");
                    departamentos = departamentos.Replace("\"", "");
                    departamentos = departamentos.Replace("\r\n      ", " ");
                    departamentos = departamentos.Replace("\r\n    },", "</v2:item> \r\n");
                    departamentos = departamentos.Replace("\r\n    }\r\n  ", "</v2:item> \r\n").TrimStart(' ');                    

                    CuerpoConsultaPorDepartamento(Variables.wsUsuario, Variables.wsPassword, departamentos, FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Género"))
                {
                    busqueda = busqueda.Substring(0, 1);
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaPorGenero(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
            }
            else
            {
                if (LbxBusqueda.Text.Equals("Nombre"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaDescargaXNombre(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Apellido"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaDescargaXApellido(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("ID"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaDescargaXDPI(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Departamento"))
                {
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaDescargaXDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
                else if (LbxBusqueda.Text.Equals("Género"))
                {
                    busqueda = busqueda.Substring(0, 1);
                    //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                    CuerpoConsultaDescargaXGenero(Variables.wsUsuario, Variables.wsPassword, busqueda.ToUpper(), FI, FF);
                }
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

        //Crea el cuerpo que se utiliza para consultar los empleados por nombre
        private static void CuerpoConsultaPorNombre(string idPersona, string passwordServicio, string name, string fechaInicio, string fechaFin)
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
                                                <v2:name>Name</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + name + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                            <!--2nd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                            <!--3rd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/ReportesCT/CTBusquedaNombre.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por apellido
        private static void CuerpoConsultaPorApellido(string idPersona, string passwordServicio, string last, string fechaInicio, string fechaFin)
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
                                                <v2:name>LastName</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + last + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                            <!--2nd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                            <!--3rd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/ReportesCT/CTBusquedaApellido.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por departamento
        private static void CuerpoConsultaPorDepartamento(string idPersona, string passwordServicio, string dpto, string fechaInicio, string fechaFin)
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
                                                <v2:name>Departamento</v2:name>
                                                        <v2:values>"+ 
                                                        dpto
                                                            //<v2:item>" + dpto + @"</v2:item>
                                                        +@"</v2:values>
                                                </v2:item>
                                            <!--2nd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                            <!--3rd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/ReportesCT/CTBusquedaDepartamento.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por ID
        private static void CuerpoConsultaPorID(string idPersona, string passwordServicio, string ID, string fechaInicio, string fechaFin)
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
                                                <v2:name>ID</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + ID + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                            <!--2nd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                            <!--3rd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/ReportesCT/CTBusquedaID.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Genero
        private static void CuerpoConsultaPorGenero(string idPersona, string passwordServicio, string gen, string fechaInicio, string fechaFin)
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
                                                <v2:name>Sexo</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + gen + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                            <!--2nd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                            <!--3rd Parameter of BIP Report-->
                                            <v2:item>
                                                <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                            </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/ReportesCT/CTBusquedaGenero.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar
        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda Nombre
        private static void CuerpoConsultaDescargaXNombre(string idPersona, string passwordServicio, string nombre, string fechaInicio, string fechaFin)
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
                                                <v2:name>NOMBRE</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + nombre + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                                <!--3rd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>                                           
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaNombre.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda Apellido
        private static void CuerpoConsultaDescargaXApellido(string idPersona, string passwordServicio, string apellido, string fechaInicio, string fechaFin)
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
                                                <v2:name>APELLIDO</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + apellido + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                                <!--3rd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>                                           
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaApellido.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda DPI
        private static void CuerpoConsultaDescargaXDPI(string idPersona, string passwordServicio, string DPI, string fechaInicio, string fechaFin)
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
                                                            <v2:item>" + DPI + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                                <!--3rd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>                                           
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaDPI.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda Dependencia
        private static void CuerpoConsultaDescargaXDependencia(string idPersona, string passwordServicio, string dependencia, string fechaInicio, string fechaFin)
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
                                                <v2:name>DEPENDENCIA</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + dependencia + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                                <!--3rd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>                                           
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaDependencia.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda Dependencia
        private static void CuerpoConsultaDescargaXGenero(string idPersona, string passwordServicio, string sexo, string fechaInicio, string fechaFin)
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
                                                <v2:name>SEX</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + sexo + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaInicio</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaInicio + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                                <!--3rd Parameter of BIP Report-->
                                                <v2:item>
                                                        <v2:name>FechaFin</v2:name>
                                                        <v2:values>
                                                        <v2:item>" + fechaFin + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>                                           
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaGenero.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }



        //Función para decodificar respuesta de la API
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        //Sustituye las comillas dobles y elimina los primeros caracteres que corresponden a los Headers
        public string sustituirCaracteres(string dpi)
        {
            string sustituto = "";
            sustituto = DecodeStringFromBase64(ConsultaWS("")).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @" \n+", "\n");
            sustituto = Regex.Replace(sustituto, @"\n+", "");

            if (desc == 0)
            {
                if (sustituto.Length > 110)
                {
                    if (String.IsNullOrEmpty(TxtBuscador.Text))
                    {
                        sustituto = sustituto.Remove(0, 136);
                    }
                    else
                    {
                        //Se valida que tipo de busqueda se realiza pues este dato lo devuelve el string sustituto y dependiendo
                        //de eso son los caracteres que se eliminan para que unicamente quede la informacion que se necesita.

                        if (LbxBusqueda.Text.Equals("Nombre"))
                        {
                            int largo = 107;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("Apellido"))
                        {
                            int largo = 107;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("ID"))
                        {
                            int largo = 107;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("Departamento"))
                        {
                            int largo = 115;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("Género"))
                        {
                            int largo = 107;
                            sustituto = sustituto.Remove(0, largo);
                        }
                    }
                }
            }
            else
            {
                int largo = TxtBuscador.Text.Length;
                largo = largo + 53;
                if (sustituto.Length > largo)
                    sustituto = sustituto.Remove(0, largo);
            }
            return sustituto;
        }

        public void matrizDatos()
        {
            if (LbxBusqueda.Text != "Género" && !TxtBuscador.Text.ToLower().Equals("mujer"))
            {
                if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(lblBusqueda.Text))
                {
                    GridViewReporteCT.DataSource = "";

                    string[] result = sustituirCaracteres("").Split('|');
                    decimal registros = 0;
                    decimal count = 0;
                    int datos = 0;
                    string[,] arrlist;
                    string jerarquiaPerson;
                    if (result.Count() > 12)
                    {
                        registros = result.Count() / 13;
                        count = Math.Round(registros, 0);
                        arrlist = new string[Convert.ToInt32(count), 13];

                        for (int i = 0; i < count; i++)
                        {
                            for (int k = 0; k < 13; k++)
                            {
                                arrlist[i, k] = result[datos];
                                datos++;
                            }
                        }

                        try
                        {
                            DataSetLocalRpt dsReporte = new DataSetLocalRpt();
                            try
                            {

                                //Generacion de matriz para llenado de grid desde una consulta
                                for (int i = 0; i < count; i++)
                                {
                                    DataRow newFila = dsReporte.Tables["RptCTEmpleados"].NewRow();
                                    newFila["FIRST_NAME"] = (arrlist[i, 5] ?? "").ToString();
                                    newFila["LAST_NAME"] = (arrlist[i, 6] ?? "").ToString();
                                    newFila["ID"] = (arrlist[i, 7] ?? "").ToString();
                                    jerarquiaPerson = jerarquia((arrlist[i, 10] ?? "").ToString());
                                    jerarquiaPerson = getBetween(jerarquiaPerson, "\"jerarquia\": \"", "\"");
                                    newFila["PERSON_GROUP"] = "UNIS/" + (arrlist[i, 8] ?? "").ToString() + "/" + jerarquiaPerson;//(arrlist[i, 10] ?? "").ToString();
                                    newFila["Start_Time_of_Effective_Period"] = "";
                                    newFila["End_Time_of_Effective_Period"] = "";
                                    newFila["PHONE"] = (arrlist[i, 9] ?? "").ToString();
                                    newFila["DEPARTAMENTO"] = (arrlist[i, 10] ?? "").ToString();
                                    newFila["GENDER"] = (arrlist[i, 11] ?? "").ToString();


                                    if (arrlist[i, 12].ToString() != "-")
                                    {
                                        int busqueda = 29;
                                        string email = arrlist[i, 12].ToString();
                                        email = StringExtensions.RemoveEnd(email, busqueda);
                                        newFila["EMAIL"] = email;
                                    }
                                    newFila["TYPE"] = "";
                                    newFila["CARD"] = "";
                                    newFila["REMARK"] = "";
                                    newFila["DOCK_STATION_LOGIN_PASSWORD"] = "";
                                    newFila["SUPPORTISSUEDCUSTOMPROPERTIES"] = "";
                                    newFila["SKINSURFACE_TEMPERATURE"] = "";
                                    newFila["TEMPERATURE_STATUS"] = "";
                                    newFila["EMPLID"] = "";
                                    dsReporte.Tables["RptCTEmpleados"].Rows.Add(newFila);
                                }

                            }
                            catch (Exception x)
                            {
                                Console.WriteLine(x.ToString());
                            }

                            GridViewReporteCT.DataSource = dsReporte.Tables["RptCTEmpleados"];
                            GridViewReporteCT.DataBind();
                            GridViewReporteCT.UseAccessibleHeader = true;
                            GridViewReporteCT.HeaderRow.TableSection = TableRowSection.TableHeader;
                            lblBusqueda.Text = "";
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.ToString());
                        }
                        lblBusqueda.Text = " ";
                    }
                    else
                    {
                        lblBusqueda.Text = "No se encontró información con los valores ingresados";
                        if (LbxBusqueda.Text == "Género")
                            lblBusqueda.Text = lblBusqueda.Text + ". Para realizar búesqueda por género intente ingresando Male o Female";
                    }
                }
                else
                {
                    lblBusqueda.Text = "Ingrese un valor a buscar";
                }
            }
            else
            {
                lblBusqueda.Text = "Para realizar búesqueda por género intente ingresando Male o Female";
            }
        }

        protected void BtnBuscar2_Click(object sender, EventArgs e)
        {
            matrizDatos();
        }
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

        public override void VerifyRenderingInServerForm(Control control)
        {
            //required to avoid the run time error "  
            //Control 'GridViewReporteCT' of type 'Grid View' must be placed inside a form tag with runat=server."  
        }

        //Llenado de informacion a las columnas correspondientes del excel
        protected void GenerarExcel(object sender, EventArgs e)
        {
            SLDocument sl = new SLDocument();
            int celda = 1;
            //Letras de las columnas para la generacion de excel
            string[] LETRA = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q" };
            //Texto plano
            sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Estudiantes " + DateTime.Now.ToString("G"));
            sl.SetCellValue("A" + celda, "Rule");
            celda++;
            sl.SetCellValue("A" + celda, "The items with asterisk are required.At least one of family name and given name is required.");
            celda++;
            sl.SetCellValue("A" + celda, "Do NOT change the layout and column title in this template file. The importing may fail if changed.");
            celda++;
            sl.SetCellValue("A" + celda, "Supports adding persons to the existing person group whose name is separated by slash. For example, the name format of Group A under All Persons is All Persons/Group A.");
            celda++;
            sl.SetCellValue("A" + celda, "Start/End Time of Effective Period: The effective period of the person for access control and time & attendance. Format: yyyy/mm/dd HH:MM:SS.");
            celda++;
            sl.SetCellValue("A" + celda, "Domain Person and Domain Group Person don't support adding and editing person's basic information and additional information by importing.");
            celda++;
            sl.SetCellValue("A" + celda, "No more than five cards can be issued to one person. Each two card numbers should be separated by semicolon, e.g., 01;02;03;04;05.");
            celda++;
            sl.SetCellValue("A" + celda, "It supports editing the persons' additional information in a batch, the fields of which are already created in the system. Please enter the additional information according to the type. For single selection type, select one from the drop-down list.");
            celda++;
            sl.SetCellValue("A" + celda, "Supports custom attribute input formats separated by commas, for example: attribute name 1, attribute name 2");
            celda++;

            if (!String.IsNullOrEmpty(LbxBusqueda.Text))
            {
                //Cabeceras
                if (celda == 10)
                {
                    for (int k = 0; k < GridViewReporteCT.Columns.Count; k++)
                    {
                        sl.SetCellValue("A" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("B" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("C" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("D" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("E" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("F" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("G" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("H" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("I" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("J" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("K" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("L" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("M" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("N" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("O" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("P" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("Q" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        celda++;
                    }
                }

                //Llenado de las columnas con la informacion

                if (celda > 10)
                {
                    string[] result = sustituirCaracteres("").Split('|');
                    decimal registros = 0;
                    decimal count = 0;
                    int datos = 0;
                    string[,] arrlist;
                    string jerarquiaPerson;
                    if (result.Count() > 16)
                    {
                        registros = result.Count() / 13;
                        count = Math.Round(registros, 0);
                        arrlist = new string[Convert.ToInt32(count), 13];

                        for (int i = 0; i < count; i++)
                        {
                            for (int k = 0; k < 13; k++)
                            {
                                arrlist[i, k] = result[datos];
                                datos++;
                            }
                        }
                        int contador = GridViewReporteCT.Rows.Count;
                        for (int i = 0; i < contador; i++)
                        {

                            sl.SetCellValue("A" + celda, (arrlist[i, 5] ?? "").ToString()); //First Name
                            sl.SetCellValue("B" + celda, (arrlist[i, 6] ?? "").ToString()); //Last Name
                            sl.SetCellValue("C" + celda, (arrlist[i, 7] ?? "").ToString()); //ID
                            sl.SetCellValue("D" + celda, "Basic Person"); //TYPE
                            jerarquiaPerson = jerarquia((arrlist[i, 10] ?? "").ToString());
                            jerarquiaPerson = getBetween(jerarquiaPerson, "\"jerarquia\": \"", "\"");
                            sl.SetCellValue("E" + celda, "UNIS/" + (arrlist[i, 8] ?? "").ToString() + "/" + jerarquiaPerson);//+ (arrlist[i, 10] ?? "").ToString()); //Person Group
                            sl.SetCellValue("F" + celda, (arrlist[i, 11] ?? "").ToString()); //GENDER
                            sl.SetCellValue("G" + celda, ""); //Start Time of Effective Period
                            sl.SetCellValue("H" + celda, ""); //End Time of Effective Period
                            sl.SetCellValue("I" + celda, ""); //CARD
                            sl.SetCellValue("J" + celda, arrlist[i, 12].ToString()); //EMAIL
                            sl.SetCellValue("K" + celda, (arrlist[i, 9] ?? "").ToString()); //PHONE
                            sl.SetCellValue("L" + celda, ""); //REMARK
                            sl.SetCellValue("M" + celda, ""); //Dock Station Login Password
                            sl.SetCellValue("N" + celda, ""); //Support Issued Custom Properties
                            sl.SetCellValue("O" + celda, ""); //Skin-surface Temperature
                            sl.SetCellValue("P" + celda, ""); //Temperature Status
                            sl.SetCellValue("Q" + celda, (arrlist[i, 10] ?? "").ToString()); //DEPARTAMENTO
                            celda++;
                        }
                        if (result.Count() > 3)
                        {
                            //Nombre del archivo
                            string nombre = "Reporte Camara Termica Empleados " + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".xlsx";
                            //Lugar de almacenamiento
                            sl.SaveAs(CurrentDirectory + "ReportesCT/" + nombre);
                            Response.ContentType = "application/ms-excel";
                            Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
                            Response.TransmitFile(CurrentDirectory + "ReportesCT/" + nombre);
                        }
                        else
                        {
                            lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
                        }
                    }
                    else
                    {
                        registros = result.Count() / 13;
                        count = Math.Round(registros, 0);
                        arrlist = new string[Convert.ToInt32(count), 13];

                        for (int i = 0; i < count; i++)
                        {
                            for (int k = 0; k < 13; k++)
                            {
                                arrlist[i, k] = result[datos];
                                datos++;
                            }
                        }
                        int contador = GridViewReporteCT.Rows.Count;
                        for (int i = 0; i < contador; i++)
                        {

                            sl.SetCellValue("A" + celda, (arrlist[i, 5] ?? "").ToString()); //FIRS NAME
                            sl.SetCellValue("B" + celda, (arrlist[i, 6] ?? "").ToString()); //LAST NAME
                            sl.SetCellValue("C" + celda, (arrlist[i, 7] ?? "").ToString()); //ID
                            sl.SetCellValue("D" + celda, "Basic Person"); //TYPE
                            jerarquiaPerson = jerarquia((arrlist[i, 10] ?? "").ToString());
                            jerarquiaPerson = getBetween(jerarquiaPerson, "\"jerarquia\": \"", "\"");
                            sl.SetCellValue("E" + celda, "UNIS/" + (arrlist[i, 8] ?? "").ToString() + "/" + jerarquiaPerson);//(arrlist[i, 10] ?? "").ToString()); //PERSON GROUP
                            sl.SetCellValue("F" + celda, (arrlist[i, 11] ?? "").ToString()); //GENDER
                            sl.SetCellValue("G" + celda, ""); //Start Time of Effective Period
                            sl.SetCellValue("H" + celda, ""); //End Time of Effective Period
                            sl.SetCellValue("I" + celda, ""); //CARD
                            sl.SetCellValue("J" + celda, arrlist[i, 12].ToString()); //EMAIL
                            sl.SetCellValue("K" + celda, (arrlist[i, 9] ?? "").ToString()); //PHONE
                            sl.SetCellValue("L" + celda, ""); //REMARK
                            sl.SetCellValue("M" + celda, ""); //Dock Station Login Password
                            sl.SetCellValue("N" + celda, ""); //Support Issued Custom Properties
                            sl.SetCellValue("O" + celda, ""); //Skin-surface Temperature
                            sl.SetCellValue("P" + celda, ""); //Temperature Status
                            sl.SetCellValue("Q" + celda, (arrlist[i, 10] ?? "").ToString()); //DEPARTAMENTO
                            celda++;
                        }
                        if (result.Count() > 3)
                        {
                            //Nombre del archivo
                            string nombre = "Reporte Camara Termica Empleados " + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".xlsx";
                            //Lugar de almacenamiento
                            sl.SaveAs(CurrentDirectory + "ReportesCT/" + nombre);
                            Response.ContentType = "application/ms-excel";
                            Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
                            Response.TransmitFile(CurrentDirectory + "ReportesCT/" + nombre);
                        }
                        else
                        {
                            lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
                        }
                    }
                }
            }
        }

        public static string removeUnicode(string input)
        {
            //Mayusculas con Tilde
            Regex replaceAt = new Regex("&#193;", RegexOptions.Compiled);
            input = replaceAt.Replace(input, "Á");
            Regex replaceEt = new Regex("&#201;", RegexOptions.Compiled);
            input = replaceEt.Replace(input, "É");
            Regex replaceIt = new Regex("&#205;", RegexOptions.Compiled);
            input = replaceIt.Replace(input, "Í");
            Regex replaceOt = new Regex("&#211;", RegexOptions.Compiled);
            input = replaceOt.Replace(input, "Ó");
            Regex replaceUt = new Regex("&#218;", RegexOptions.Compiled);
            input = replaceUt.Replace(input, "Ú");

            //Minusculas con tilde
            Regex replaceA = new Regex("&#225;", RegexOptions.Compiled);
            input = replaceA.Replace(input, "á");
            Regex replaceE = new Regex("&#233;", RegexOptions.Compiled);
            input = replaceE.Replace(input, "é");
            Regex replaceI = new Regex("&#237;", RegexOptions.Compiled);
            input = replaceI.Replace(input, "í");
            Regex replaceO = new Regex("&#243;", RegexOptions.Compiled);
            input = replaceO.Replace(input, "ó");
            Regex replaceU = new Regex("&#250;", RegexOptions.Compiled);
            input = replaceU.Replace(input, "ú");

            //Ñ y ñ
            Regex replaceN = new Regex("&#209;", RegexOptions.Compiled);
            input = replaceN.Replace(input, "Ñ");
            Regex replacen = new Regex("&#241;", RegexOptions.Compiled);
            input = replacen.Replace(input, "ñ");

            //Mayusculas con dieresis
            Regex replaceAd = new Regex("&#196;", RegexOptions.Compiled);
            input = replaceAd.Replace(input, "Ä");
            Regex replaceEd = new Regex("&#203;", RegexOptions.Compiled);
            input = replaceEd.Replace(input, "Ë");
            Regex replaceId = new Regex("&#207;", RegexOptions.Compiled);
            input = replaceId.Replace(input, "Ï");
            Regex replaceOd = new Regex("&#214;", RegexOptions.Compiled);
            input = replaceOd.Replace(input, "Ö");
            Regex replaceUd = new Regex("&#220;", RegexOptions.Compiled);
            input = replaceUt.Replace(input, "Ü");

            //Minusculas con tilde
            Regex replaceAmd = new Regex("&#228;", RegexOptions.Compiled);
            input = replaceAmd.Replace(input, "ä");
            Regex replaceEmd = new Regex("&#235;", RegexOptions.Compiled);
            input = replaceEmd.Replace(input, "ë");
            Regex replaceImd = new Regex("&#239;", RegexOptions.Compiled);
            input = replaceImd.Replace(input, "ï");
            Regex replaceOmd = new Regex("&#246;", RegexOptions.Compiled);
            input = replaceOmd.Replace(input, "ö");
            Regex replaceUmd = new Regex("&#252;", RegexOptions.Compiled);
            input = replaceUmd.Replace(input, "ü");

            Regex replaceEspace = new Regex("&nbsp;", RegexOptions.Compiled);
            input = replaceEspace.Replace(input, " ");

            return input;
        }

        protected string DownloadAllFile()
        {
            lblDescarga.Text = "";
            desc = 1;
            string[] result = sustituirCaracteres("").Split('|');
            decimal registros = 0;
            decimal count = 0;
            int datos = 0;
            string[,] arrlist;
            string ret = "0";

            if (result.Count() > 5)
            {
                //Busqueda simple por Nombre, Apellido, DPI o dependencia
                registros = result.Count() / 5;
                count = Math.Round(registros, 0);
                if (registros == 0)
                    count = 1;
                arrlist = new string[Convert.ToInt32(count), 6];
                if (result.Count() > 6)
                {
                    datos = 1;
                    for (int i = 0; i < count; i++)
                    {
                        for (int k = 1; k < 6; k++)
                        {
                            if (k == 5 && i < count - 1)
                            {
                                string resultado = result[datos];
                                int removerUltimos = 10;
                                resultado = resultado.Remove(resultado.Length - removerUltimos);
                                arrlist[i, k] = result[datos].Remove(result[datos].Length - 10);
                                arrlist[i, 0] = "";
                            }
                            else
                            {
                                arrlist[i, k] = result[datos];
                            }
                            datos++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            arrlist[i, k] = result[datos];
                            datos++;
                        }
                    }
                }


                int total = 0;
                DataSetLocalRpt dsDownload = new DataSetLocalRpt();
                for (int i = 0; i < count; i++)
                {
                    desc = 1;
                    if (arrlist.Length > 5)
                    {
                        DataRow newFila = dsDownload.Tables["AllDownloadEmp"].NewRow();
                        newFila["bytes"] = arrlist[i, 5];
                        newFila["contentType"] = "jpg";
                        newFila["fileName"] = arrlist[i, 3] + ".jpg";
                        dsDownload.Tables["AllDownloadEmp"].Rows.Add(newFila);
                        total = total + 1;
                    }
                }

                if (total > 0)
                {
                    string user = Environment.UserName;
                    string unidad = unidadAlmacenamiento().Substring(0, 2);
                    string path = unidad + ":\\Users\\" + user + "\\Downloads";
                    if (!Directory.Exists(path))
                    {
                        File.Create(path).Close();
                    }
                    string folder = path + "\\" + nombre;
                    File.Create(folder).Close();

                    using (FileStream zipToOpen = new FileStream(folder, FileMode.Open))
                    {

                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            for (int i = 0; i < total; i++)
                            {

                                byte[] base64 = Convert.FromBase64String(dsDownload.Tables["AllDownloadEmp"].Rows[i]["bytes"].ToString());
                                ZipArchiveEntry readmeEntry = archive.CreateEntry(dsDownload.Tables["AllDownloadEmp"].Rows[i]["filename"].ToString(), CompressionLevel.Fastest);

                                var zipStream = readmeEntry.Open();
                                zipStream.Write(base64, 0, base64.Length);
                            }
                        }
                        /*------------FUNCIONA, PERO SUSTITUYE EL  ZIP POR LA FOTO
                        using (FileStream foto = new FileStream(folder + dsDownload.Tables["AllDownload"].Rows[0]["filename"].ToString(), FileMode.Open))
                        {
                            using (GZipStream gz = new GZipStream(foto, CompressionMode.Compress, false))
                            {
                                gz.Write(base64, 0, base64.Length);
                            }
                        }
                        ---------------*/
                    }
                    lblBusqueda.Text = "";
                    lblDescarga.Visible = true;
                    lblDescarga.Text = "Las fotografías fueron almacenadas en la ubicación: <a href=" + path + ">" + path + "</a>";
                    ret = "1";
                }
                else
                {
                    ret = "2";
                }
            }
            return ret;
        }

        protected void BtnImg_Click(object sender, EventArgs e)
        {
            try
            {
                string respuesta = DownloadAllFile();
                if (respuesta == "0")
                {
                    lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga de fotografías";
                }
                else if (respuesta == "2")
                    lblBusqueda.Text = "No se encontraron imágenes relacionadas a los empleados.";
            }
            catch (Exception)
            {
                lblBusqueda.Text = "Ha ocurido un error";
            }
        }

        public string unidadAlmacenamiento()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string name = "";
            foreach (DriveInfo drive in drives)
            {
                string label = drive.IsReady ?
                    String.Format(" - {0}", drive.VolumeLabel) : null;
                Console.WriteLine("{0} - {1}{2}", drive.Name, drive.DriveType, label);
                name = name + " " + drive.Name;
            }
            return name;
        }

        public string jerarquia(string departamento)
        {
            WebClient _clientW = new WebClient();
            _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            _clientW.Headers.Add("departamento", departamento);
            string json = _clientW.DownloadString("https://apexdes.unis.edu.gt:8443/ords/unis_interfaces/Centralizador/JerarquiaDepartamentos");
            dynamic respuesta = JsonConvert.DeserializeObject(json).ToString();

            return respuesta;
        }
        public string departamentosAPEX(string jerarquia)
        {
            WebClient _clientW = new WebClient();
            _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            _clientW.Headers.Add("jerarquia", jerarquia);
            string json = _clientW.DownloadString("https://apexdes.unis.edu.gt:8443/ords/unis_interfaces/Centralizador/BusquedaXDepartamento");
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


        public static int contarPalabras(string cadena)
        {
            string[] palabras = cadena.Split(' ');
            int contador = 0;
            foreach (string palabra in palabras)
            {
                if (palabra.Equals("\"departamento\":"))
                {
                    contador = contador + 1;
                }
            }
            return contador;
        }
    }
}