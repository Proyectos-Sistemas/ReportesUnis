using System;
using System.Linq;
using System.Web.UI;//.WebControls;
using System.Web.Services;
using System.Xml;
using System.Net;
using System.IO;
using System.Globalization;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web;
using Ionic.Zip;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.EnterpriseServices;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using NPOI.HSSF.Record;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ReportesUnis
{
    public partial class ReporteEmpleados : System.Web.UI.Page
    {
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

            dt.Columns.Add("Nombre1");
            dt.Columns.Add("Nombre2");
            dt.Columns.Add("Apellido1");
            dt.Columns.Add("Apellido2");
            dt.Columns.Add("Apellido3");
            dt.Columns.Add("Cumpleaños");
            dt.Columns.Add("Sexo");
            dt.Columns.Add("Estado Civil");
            dt.Columns.Add("Nacionalidad");
            dt.Columns.Add("FLAG_CED");
            dt.Columns.Add("Cedula");
            dt.Columns.Add("FLAG_DPI");
            dt.Columns.Add("DPI");
            dt.Columns.Add("FLAG_PAS");
            dt.Columns.Add("Pasaporte");
            dt.Columns.Add("NIT");
            dt.Columns.Add("Direccion");
            dt.Columns.Add("Municipio");
            dt.Columns.Add("Departamento");
            dt.Columns.Add("Telefono");
            dt.Columns.Add("CARNE");
            dt.Columns.Add("Dependencia");
            dt.Columns.Add("NOM_IMP");

            dr["Nombre1"] = String.Empty;
            dr["Nombre2"] = String.Empty;
            dr["Apellido1"] = String.Empty;
            dr["Apellido2"] = String.Empty;
            dr["Apellido3"] = String.Empty;
            dr["Cumpleaños"] = String.Empty;
            dr["Sexo"] = String.Empty;
            dr["Estado Civil"] = String.Empty;
            dr["Nacionalidad"] = String.Empty;
            dr["FLAG_CED"] = String.Empty;
            dr["Cedula"] = String.Empty;
            dr["FLAG_DPI"] = String.Empty;
            dr["DPI"] = String.Empty;
            dr["FLAG_PAS"] = String.Empty;
            dr["Pasaporte"] = String.Empty;
            dr["NIT"] = String.Empty;
            dr["Direccion"] = String.Empty;
            dr["Municipio"] = String.Empty;
            dr["Departamento"] = String.Empty;
            dr["Telefono"] = String.Empty;
            dr["CARNE"] = String.Empty;
            dr["Dependencia"] = String.Empty;
            dr["NOM_IMP"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewReporte.DataSource = dt;
            this.GridViewReporte.DataBind();
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
        public string Consultar(string dpi)
        {
            string busqueda = "", FI = "", FF = "", busqueda2 = "";

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
            if (!String.IsNullOrEmpty(TxtBuscador2.Text))
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string inicial = TxtBuscador2.Text.Substring(0, 1).ToUpper();
                string letras = TxtBuscador2.Text.Substring(1, TxtBuscador2.Text.Length - 1).Trim(' ').ToLower();
                busqueda2 = textInfo.ToTitleCase(inicial + letras);
            }
            //Se limpian variables para guardar la nueva información
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguraciones, "Consultar");

            if (desc == 0)
            {
                if (!ChBusqueda.Checked)
                {
                    if (LbxBusqueda.Text.Equals("Nombre"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombre(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorApellido(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                }
                else
                {
                    if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreApellido(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorApellidoDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorApellidoDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("DPI") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorDPIDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreApellido(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreDPI(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorNombreDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorApellidoDPI(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorApellidoDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("DPI") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaPorDPIDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                }
            }
            else
            {
                if (!ChBusqueda.Checked)
                {
                    if (LbxBusqueda.Text.Equals("Nombre"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaDescargaXNombre(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaDescargaXApellido(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaDescargaXDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoConsultaDescargaXDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, FI, FF);
                    }

                }
                else
                {
                    if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreApellido(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXApellidoDPI(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXApellidoDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda.Text.Equals("DPI") && LbxBusqueda2.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXDPIDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda, busqueda2, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreApellido(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreDPI(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXNombreDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXApellidoDPI(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXApellidoDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
                    else if (LbxBusqueda2.Text.Equals("DPI") && LbxBusqueda.Text.Equals("Dependencia"))
                    {
                        //Crea el cuerpo que se utiliza para consultar el servicio de HCM
                        CuerpoDescargaXDPIDependencia(Variables.wsUsuario, Variables.wsPassword, busqueda2, busqueda, FI, FF);
                    }
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

            foreach (var line in System.IO.File.ReadLines(RutaConfiguracion))
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
                                                <v2:reportAbsolutePath>/Reportes IS/PT/ReporteEmpleadosBN.xdo</v2:reportAbsolutePath>
                                                <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                            </v2:reportRequest>
                                            <v2:userID>" + idPersona + @"</v2:userID>
                                            <v2:password>" + passwordServicio + @"</v2:password>
                                        </v2:runReport>
                                    </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por DPI
        private static void CuerpoConsultaPorDPI(string idPersona, string passwordServicio, string dpi, string fechaInicio, string fechaFin)
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
                                                <v2:reportAbsolutePath>/Reportes IS/PT/ReporteEmpleadosBD.xdo</v2:reportAbsolutePath>
                                                <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                            </v2:reportRequest>
                                            <v2:userID>" + idPersona + @"</v2:userID>
                                            <v2:password>" + passwordServicio + @"</v2:password>
                                        </v2:runReport>
                                    </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Apellido
        private static void CuerpoConsultaPorApellido(string idPersona, string passwordServicio, string lastname, string fechaInicio, string fechaFin)
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
                                                                <v2:item>" + lastname + @"</v2:item>
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
                                                <v2:reportAbsolutePath>/Reportes IS/PT/ReporteEmpleadosBA.xdo</v2:reportAbsolutePath>
                                                <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                            </v2:reportRequest>
                                            <v2:userID>" + idPersona + @"</v2:userID>
                                            <v2:password>" + passwordServicio + @"</v2:password>
                                        </v2:runReport>
                                    </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Dependencia
        private static void CuerpoConsultaPorDependencia(string idPersona, string passwordServicio, string dependencia, string fechaInicio, string fechaFin)
        {
            Variables.soapBody = Variables.soapBody = @"<?xml version=""1.0""?>
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
                                                            <v2:name>Dependencia</v2:name>
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
                                                <v2:reportAbsolutePath>/Reportes IS/PT/ReporteEmpleadosBDP.xdo</v2:reportAbsolutePath>
                                                <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                            </v2:reportRequest>
                                            <v2:userID>" + idPersona + @"</v2:userID>
                                            <v2:password>" + passwordServicio + @"</v2:password>
                                        </v2:runReport>
                                    </soapenv:Body>
                                </soapenv:Envelope>";
        }

        /*CUERPOS DE BUSQUEDAS MULTIPLES*/

        //Crea el cuerpo que se utiliza para consultar los empleados por Nombre y Apellido
        private static void CuerpoConsultaPorNombreApellido(string idPersona, string passwordServicio, string nombre, string apellido, string fechaInicio, string fechaFin)
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
                                                            <v2:item>" + nombre + @"</v2:item>
                                                        </v2:values>
                                                    </v2:item>
                                                    <!--2nd Parameter of BIP Report-->
                                                    <v2:item>
                                                        <v2:name>LastName</v2:name>
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
                                            <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBNA.xdo</v2:reportAbsolutePath>
                                            <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                        </v2:reportRequest>
                                        <v2:userID>" + idPersona + @"</v2:userID>
                                        <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Nombre y DPI
        private static void CuerpoConsultaPorNombreDPI(string idPersona, string passwordServicio, string nombre, string dpi, string fechaInicio, string fechaFin)
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
                                                        <v2:item>" + nombre + @"</v2:item>
                                                    </v2:values>
                                                </v2:item>
                                                <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                    <v2:name>DPI</v2:name>
                                                    <v2:values>
                                                        <v2:item>" + dpi + @"</v2:item>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBND.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Nombre y Dependencia
        private static void CuerpoConsultaPorNombreDependencia(string idPersona, string passwordServicio, string nombre, string dependencia, string fechaInicio, string fechaFin)
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
                                                        <v2:item>" + nombre + @"</v2:item>
                                                    </v2:values>
                                                </v2:item>
                                                <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                    <v2:name>Dependencia</v2:name>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBNDP.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Apellido y DPI
        private static void CuerpoConsultaPorApellidoDPI(string idPersona, string passwordServicio, string apellido, string dpi, string fechaInicio, string fechaFin)
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
                                                        <v2:item>" + apellido + @"</v2:item>
                                                    </v2:values>
                                                </v2:item>
                                                <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                    <v2:name>DPI</v2:name>
                                                    <v2:values>
                                                        <v2:item>" + dpi + @"</v2:item>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBAD.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por Apellido y Dependencia
        private static void CuerpoConsultaPorApellidoDependencia(string idPersona, string passwordServicio, string apellido, string dependencia, string fechaInicio, string fechaFin)
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
                                                        <v2:item>" + apellido + @"</v2:item>
                                                    </v2:values>
                                                </v2:item>
                                                <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                    <v2:name>Dependencia</v2:name>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBADP.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar los empleados por DPI y Dependencia
        private static void CuerpoConsultaPorDPIDependencia(string idPersona, string passwordServicio, string dpi, string dependencia, string fechaInicio, string fechaFin)
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
                                                <!--2nd Parameter of BIP Report-->
                                                <v2:item>
                                                    <v2:name>Dependencia</v2:name>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/BusquedaMultiple/ReporteEmpleadosBDDP.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
        }

        /*CUERPOS DE DESCARGA DE IMAGENES CON BUSQUEDAS SIMPLES*/
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

        /*CUERPOS DE DESCARGAS CON BUSQUEDAS MULTIPLES*/
        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de Nombre y Apellido
        private static void CuerpoDescargaXNombreApellido(string idPersona, string passwordServicio, string nombre, string apellido, string fechaInicio, string fechaFin)
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
                                            <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaApellidoNombre.xdo</v2:reportAbsolutePath>
                                            <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                        </v2:reportRequest>
                                        <v2:userID>" + idPersona + @"</v2:userID>
                                        <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de Nombre y DPI
        private static void CuerpoDescargaXNombreDPI(string idPersona, string passwordServicio, string nombre, string dpi, string fechaInicio, string fechaFin)
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
                                                    <v2:name>DPI</v2:name>
                                                    <v2:values>
                                                        <v2:item>" + dpi + @"</v2:item>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaNombreDPI.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de Nombre y Dependencia
        private static void CuerpoDescargaXNombreDependencia(string idPersona, string passwordServicio, string nombre, string dependencia, string fechaInicio, string fechaFin)
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
                                                    <v2:name>NAME</v2:name>
                                                    <v2:values>
                                                        <v2:item>" + nombre + @"</v2:item>
                                                    </v2:values>
                                                </v2:item>
                                                <!--2nd Parameter of BIP Report-->
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaNombreDependencia.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                </v2:runReport>
                                </soapenv:Body>
                                </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de Apellido y DPI
        private static void CuerpoDescargaXApellidoDPI(string idPersona, string passwordServicio, string apellido, string dpi, string fechaInicio, string fechaFin)
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
                                                    <v2:name>DPI</v2:name>
                                                    <v2:values>
                                                        <v2:item>" + dpi + @"</v2:item>
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaApellidoDPI.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de Apellido y Dependencia
        private static void CuerpoDescargaXApellidoDependencia(string idPersona, string passwordServicio, string apellido, string dependencia, string fechaInicio, string fechaFin)
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaApellidoDependencia.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
        }

        //Crea el cuerpo que se utiliza para consultar las imagenes a descargar por busqueda  de DPI y Dependencia
        private static void CuerpoDescargaXDPIDependencia(string idPersona, string passwordServicio, string dpi, string dependencia, string fechaInicio, string fechaFin)
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
                                                <!--2nd Parameter of BIP Report-->
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
                                        <v2:reportAbsolutePath>/Reportes IS/PT/DescargaImagenes/InformeDescargaDependenciaDPI.xdo</v2:reportAbsolutePath>
                                        <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                    </v2:reportRequest>
                                    <v2:userID>" + idPersona + @"</v2:userID>
                                    <v2:password>" + passwordServicio + @"</v2:password>
                                    </v2:runReport>
                                    </soapenv:Body>
                                    </soapenv:Envelope>";
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

        //Función para decodificar respuesta de la API
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        //Sustituye las comillas dobles y elimina los primeros caracteres que corresponden a los Headers
        public string sustituirCaracteres(string dpi)
        {
            string sustituto = "";

            if (desc == 0)
            {
                sustituto = DecodeStringFromBase64(Consultar(dpi)).Replace('"', '\n');
                sustituto = Regex.Replace(sustituto, @" \n+", "\n");
                sustituto = Regex.Replace(sustituto, @"\n+", "");
                sustituto = Regex.Replace(sustituto, @"--", " ");
                if (sustituto.Length > 110)
                {
                    if (String.IsNullOrEmpty(TxtBuscador.Text) && String.IsNullOrEmpty(CldrCiclosInicio.Text))
                    {
                        sustituto = sustituto.Remove(0, 136);
                    }
                    else
                    {
                        //Se valida que tipo de busqueda se realiza pues este dato lo devuelve el string sustituto y dependiendo
                        //de eso son los caracteres que se eliminan para que unicamente quede la informacion que se necesita.

                        if (LbxBusqueda.Text.Equals("Nombre") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                        {
                            int largo = 237;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("Apellido") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                        {
                            int largo = 241;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("DPI") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                        {
                            int largo = 236;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if (LbxBusqueda.Text.Equals("Dependencia") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                        {
                            int largo = 244;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido")))
                        {
                            int largo = 246;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI")))
                        {
                            int largo = 241;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Dependencia")))
                        {
                            int largo = 249;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if ((LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI")) || (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI")))
                        {
                            int largo = 245;
                            sustituto = sustituto.Remove(0, largo);

                        }
                        else if ((LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Dependencia")))
                        {
                            int largo = 253;
                            sustituto = sustituto.Remove(0, largo);
                        }
                        else if ((LbxBusqueda.Text.Equals("DPI") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("DPI") && LbxBusqueda.Text.Equals("Dependencia")))
                        {
                            int largo = 248;
                            sustituto = sustituto.Remove(0, largo);
                        }
                    }
                }
            }
            else
            {
                sustituto = DecodeStringFromBase64(Consultar(dpi)).Replace('\"', '\t');
                sustituto = Regex.Replace(sustituto, @"\n+", " ");
                sustituto = Regex.Replace(sustituto, @"\t", "");

                if (LbxBusqueda.Text.Equals("Nombre") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                {
                    int largo = 55;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (LbxBusqueda.Text.Equals("Apellido") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                {
                    int largo = 57;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (LbxBusqueda.Text.Equals("DPI") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                {
                    int largo = 52;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (LbxBusqueda.Text.Equals("Dependencia") && String.IsNullOrEmpty(LbxBusqueda2.Text))
                {
                    int largo = 60;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido")))
                {
                    int largo = 63;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI")))
                {
                    int largo = 58;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if ((LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Dependencia")))
                {
                    int largo = 66;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if ((LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI")) || (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI")))
                {
                    int largo = 60;
                    sustituto = sustituto.Remove(0, largo);

                }
                else if ((LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Dependencia")))
                {
                    int largo = 68;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if ((LbxBusqueda.Text.Equals("DPI") && LbxBusqueda2.Text.Equals("Dependencia")) || (LbxBusqueda2.Text.Equals("DPI") && LbxBusqueda.Text.Equals("Dependencia")))
                {
                    int largo = 63;
                    sustituto = sustituto.Remove(0, largo);
                }
            }
            return sustituto;
        }
        public void matrizDatos()
        {
            if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(LbxBusqueda2.Text) || !String.IsNullOrEmpty(CldrCiclosFin.Text) || !String.IsNullOrEmpty(CldrCiclosFin.Text))
            {
                {
                    GridViewReporte.DataSource = "";
                    if (!ChBusqueda.Checked)
                    {
                        LbxBusqueda2.Text = "";
                    }
                    string[] result = sustituirCaracteres("").Split('|');
                    decimal registros = 0;
                    decimal count = 0;
                    int datos = 0;
                    string[,] arrlist;

                    if (result.Count() > 20)
                    {
                        if (!ChBusqueda.Checked)
                        {
                            //Busqueda simple por Nombre, Apellido, DPI o dependencia
                            registros = result.Count() / 24;
                            count = Math.Round(registros, 0);
                            if (registros == 0)
                                count = 1;
                            arrlist = new string[Convert.ToInt32(count), 24];
                            if (result.Count() > 23)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    for (int k = 0; k < 24; k++)
                                    {
                                        arrlist[i, k] = result[datos];
                                        datos++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Busqueda multiple por asignacion
                            registros = result.Count() / 25;
                            count = Math.Round(registros, 0);
                            arrlist = new string[Convert.ToInt32(count), 25];
                            if (registros == 0)
                                count = 1;
                            for (int i = 0; i < count; i++)
                            {
                                for (int k = 0; k < 25; k++)
                                {
                                    arrlist[i, k] = result[datos];
                                    datos++;
                                }
                            }
                        }

                        try
                        {
                            var bday = "";
                            var dia = "";
                            var mes = "";
                            var anio = "";
                            int flag_pas = 0;
                            DataSetLocalRpt dsReporte = new DataSetLocalRpt();
                            try
                            {
                                //Valida si no se hace busqueda multiple
                                if (!ChBusqueda.Checked)
                                {
                                    //Generacion de matriz para llenado de grid desde una consulta
                                    for (int i = 0; i < count; i++)
                                    {
                                        DataRow newFila = dsReporte.Tables["RptEmpleados"].NewRow();
                                        newFila["Dependencia"] = (arrlist[i, 4] ?? "").ToString();
                                        newFila["Telefono"] = (arrlist[i, 5] ?? "").ToString();
                                        newFila["Estado Civil"] = (arrlist[i, 6] ?? "").ToString();
                                        if (!arrlist[i, 7].ToString().Equals(""))
                                        {
                                            bday = arrlist[i, 7].ToString().Substring(0, 10);
                                            anio = bday.Substring(0, 4);
                                            mes = bday.Substring(5, 2);
                                            dia = bday.Substring(8, 2);
                                            bday = dia + "-" + mes + "-" + anio;
                                        }
                                        else
                                        {
                                            bday = "Unknown";
                                        }

                                        newFila["Cumpleaños"] = bday;

                                        newFila["Direccion"] = (arrlist[i, 8] ?? "").ToString();
                                        newFila["Municipio"] = (arrlist[i, 9] ?? "").ToString();
                                        newFila["Departamento"] = (arrlist[i, 10] ?? "").ToString();
                                        newFila["Nombre1"] = (arrlist[i, 11] ?? "").ToString();
                                        newFila["Nombre2"] = (arrlist[i, 12] ?? "").ToString();
                                        newFila["Apellido1"] = (arrlist[i, 13] ?? "").ToString();
                                        newFila["Apellido2"] = (arrlist[i, 14] ?? "").ToString();
                                        newFila["Apellido3"] = (arrlist[i, 15] ?? "").ToString();
                                        newFila["NOM_IMP"] = (arrlist[i, 11] ?? "").ToString() + " " + (arrlist[i, 13] ?? "").ToString();
                                        newFila["Sexo"] = (arrlist[i, 16] ?? "").ToString();
                                        newFila["CARNE"] = (arrlist[i, 17] ?? "").ToString();
                                        if ((arrlist[i, 3] ?? "").ToString() == (arrlist[i, 17] ?? "").ToString())
                                        {
                                            newFila["Pasaporte"] = "";
                                            newFila["FLAG_PAS"] = "0";
                                            newFila["FLAG_DPI"] = "1";
                                            newFila["DPI"] = (arrlist[i, 3] ?? "").ToString();
                                        }
                                        else
                                        {
                                            newFila["Pasaporte"] = (arrlist[i, 18] ?? "").ToString();
                                            newFila["FLAG_PAS"] = "1";
                                            newFila["FLAG_DPI"] = "0";
                                            newFila["DPI"] = "";
                                            flag_pas = 1;
                                        }
                                        newFila["Cedula"] = (arrlist[i, 19] ?? "").ToString();
                                        newFila["NIT"] = (arrlist[i, 20] ?? "").ToString();

                                        if ((arrlist[i, 21] ?? "").ToString() == "-" && flag_pas == 1)
                                        {
                                            newFila["Nacionalidad"] = "Condición Migrante";
                                        }
                                        else
                                        {
                                            newFila["Nacionalidad"] = (arrlist[i, 21] ?? "").ToString();

                                        }

                                        newFila["FLAG_CED"] = "0";
                                        dsReporte.Tables["RptEmpleados"].Rows.Add(newFila);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < count; i++)
                                    {

                                        DataRow newFila = dsReporte.Tables["RptEmpleados"].NewRow();
                                        newFila["DPI"] = (arrlist[i, 4] ?? "").ToString();
                                        newFila["Dependencia"] = (arrlist[i, 5] ?? "").ToString();
                                        newFila["Telefono"] = (arrlist[i, 6] ?? "").ToString();
                                        newFila["Estado Civil"] = (arrlist[i, 7] ?? "").ToString();
                                        if (!arrlist[i, 8].ToString().Equals(""))
                                        {
                                            bday = arrlist[i, 8].ToString().Substring(0, 10);
                                            anio = bday.Substring(0, 4);
                                            mes = bday.Substring(5, 2);
                                            dia = bday.Substring(8, 2);
                                            bday = dia + "-" + mes + "-" + anio;
                                        }
                                        else
                                        {
                                            bday = "Unknown";
                                        }

                                        newFila["Cumpleaños"] = bday;

                                        newFila["Direccion"] = (arrlist[i, 9] ?? "").ToString();
                                        newFila["Municipio"] = (arrlist[i, 10] ?? "").ToString();
                                        newFila["Departamento"] = (arrlist[i, 11] ?? "").ToString();
                                        newFila["Nombre1"] = (arrlist[i, 12] ?? "").ToString();
                                        newFila["Nombre2"] = (arrlist[i, 13] ?? "").ToString();
                                        newFila["Apellido1"] = (arrlist[i, 14] ?? "").ToString();
                                        newFila["Apellido2"] = (arrlist[i, 15] ?? "").ToString();
                                        newFila["Apellido3"] = (arrlist[i, 16] ?? "").ToString();
                                        newFila["NOM_IMP"] = (arrlist[i, 12] ?? "").ToString() + " " + (arrlist[i, 14] ?? "").ToString();
                                        newFila["Sexo"] = (arrlist[i, 17] ?? "").ToString();
                                        newFila["CARNE"] = (arrlist[i, 18] ?? "").ToString();
                                        if ((arrlist[i, 3] ?? "").ToString() == (arrlist[i, 19] ?? "").ToString())
                                        {
                                            newFila["Pasaporte"] = "";
                                            newFila["FLAG_PAS"] = "0";
                                            newFila["FLAG_DPI"] = "1";
                                            newFila["DPI"] = "";
                                        }
                                        else
                                        {
                                            newFila["Pasaporte"] = (arrlist[i, 19] ?? "").ToString();
                                            newFila["FLAG_PAS"] = "1";
                                            newFila["FLAG_DPI"] = "0";
                                        }
                                        newFila["Cedula"] = (arrlist[i, 20] ?? "").ToString();
                                        newFila["NIT"] = (arrlist[i, 21] ?? "").ToString();
                                        newFila["Nacionalidad"] = (arrlist[i, 22] ?? "").ToString();
                                        newFila["FLAG_CED"] = "0";
                                        dsReporte.Tables["RptEmpleados"].Rows.Add(newFila);
                                    }
                                }
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine(x.ToString());
                            }

                            GridViewReporte.DataSource = dsReporte.Tables["RptEmpleados"];
                            GridViewReporte.DataBind();
                            GridViewReporte.UseAccessibleHeader = true;
                            GridViewReporte.HeaderRow.TableSection = System.Web.UI.WebControls.TableRowSection.TableHeader;
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
                    }
                }

            }
            else
            {
                lblBusqueda.Text = "Es necesario que seleccione e ingrese los valores para realizar una búsqueda.";
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string txtFile = "IDUNIV|NOM_IMP|NOM1|NOM2|APE1|APE2|APE3|FE_NAC|SEXO|EST_CIV|NACIONAL|FLAG_CED|CEDULA|DEPCED|MUNCED|FLAG_DPI|DPI|FLAG_PAS|PASS|PAIS_PAS|NIT|PAIS_NIT|PROF|DIR|CASA|APTO|ZONA|COL|MUNRES|DEPRES|TEL|CEL|EMAIL|CARNET|CARR|FACUL|COD_EMP_U|PUESTO|DEP_EMP_U|COD_BARRAS|TIP_PER|ACCION|FOTO|TIPO_CTA|NO_CTA_BI|F_U|H_U|TIP_ACC|EMP_TRAB|FEC_IN_TR|ING_TR|EGR_TR|MONE_TR|PUESTO_TR|LUG_EMP|FE_IN_EMP|TEL_TR|DIR_TR|ZONA_TR|DEP_TR|MUNI_TR|PAIS_TR|ACT_EC|OTRA_NA|CONDMIG|O_CONDMIG|";
            txtFile += "\r\n";

            //Llenado de las columnas con la informacion

            int ret = 0;

            if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(LbxBusqueda2.Text) || !String.IsNullOrEmpty(CldrCiclosFin.Text) || !String.IsNullOrEmpty(CldrCiclosFin.Text))
            {
                {
                    GridViewReporte.DataSource = "";
                    if (!ChBusqueda.Checked)
                    {
                        LbxBusqueda2.Text = "";
                    }
                    string[] result = sustituirCaracteres("").Split('|');
                    decimal registros = 0;
                    decimal count = 0;
                    int datos = 0;
                    string[,] arrlist;

                    if (result.Count() > 20)
                    {
                        if (!ChBusqueda.Checked)
                        {
                            //Busqueda simple por Nombre, Apellido, DPI o dependencia
                            registros = result.Count() / 24;
                            count = Math.Round(registros, 0);
                            if (registros == 0)
                                count = 1;
                            arrlist = new string[Convert.ToInt32(count), 24];
                            if (result.Count() > 23)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    for (int k = 0; k < 24; k++)
                                    {
                                        if (result[datos].Equals("-"))
                                        {
                                            result[datos] = "";
                                        }
                                        arrlist[i, k] = result[datos];
                                        datos++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Busqueda multiple por asignacion
                            registros = result.Count() / 25;
                            count = Math.Round(registros, 0);
                            arrlist = new string[Convert.ToInt32(count), 25];
                            if (registros == 0)
                                count = 1;
                            for (int i = 0; i < count; i++)
                            {
                                for (int k = 0; k < 25; k++)
                                {
                                    if (result[datos].Equals("-"))
                                    {
                                        result[datos] = "";
                                    }
                                    arrlist[i, k] = result[datos];
                                    datos++;
                                }
                            }
                        }

                        try
                        {
                            var bday = "";
                            var dia = "";
                            var mes = "";
                            var anio = "";
                            int flag_pas = 0;
                            var nacionalidad = "";
                            var Pasaporte = "";
                            var FLAG_PAS = "";
                            var FLAG_DPI = "";
                            var DPI = "";

                            DataSetLocalRpt dsReporte = new DataSetLocalRpt();
                            try
                            {
                                string texto = "";
                                int aux = 100;
                                //Valida si no se hace busqueda multiple
                                if (!ChBusqueda.Checked && aux == 100)
                                {
                                    //Generacion de matriz para llenado de grid desde una consulta
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (!arrlist[i, 7].ToString().Equals(""))
                                        {
                                            bday = arrlist[i, 7].ToString().Substring(0, 10);
                                            anio = bday.Substring(0, 4);
                                            mes = bday.Substring(5, 2);
                                            dia = bday.Substring(8, 2);
                                            bday = dia + "-" + mes + "-" + anio;
                                        }
                                        else
                                        {
                                            bday = "Unknown";
                                        }
                                        if ((arrlist[i, 21] ?? "").ToString() == "-" && flag_pas == 1)
                                        {
                                            nacionalidad = "Condición Migrante";
                                        }
                                        else
                                        {
                                            nacionalidad = (arrlist[i, 21] ?? "").ToString();

                                        }

                                        if ((arrlist[i, 3] ?? "").ToString() == (arrlist[i, 17] ?? "").ToString())
                                        {
                                            Pasaporte = "";
                                            FLAG_PAS = "0";
                                            FLAG_DPI = "1";
                                            DPI = (arrlist[i, 3] ?? "").ToString();
                                        }
                                        else
                                        {
                                            Pasaporte = (arrlist[i, 18] ?? "").ToString();
                                            FLAG_PAS = "1";
                                            FLAG_DPI = "0";
                                            DPI = "";
                                            flag_pas = 1;
                                        }

                                        texto = "|" + //IDUNIV
                                        (arrlist[i, 11] ?? "").ToString() + " " + (arrlist[i, 13] ?? "").ToString() + "|" + //NOM_IMP
                                        (arrlist[i, 11] ?? "").ToString() + "|" +//NOM1
                                        (arrlist[i, 12] ?? "").ToString() + "|" +//NOM2
                                        (arrlist[i, 13] ?? "").ToString() + "|" +//APE1
                                        (arrlist[i, 14] ?? "").ToString() + "|" +//APE2
                                        (arrlist[i, 15] ?? "").ToString() + "|" +//APE3
                                        bday + "|" +//FE_NAC
                                        (arrlist[i, 16] ?? "").ToString() + "|" +//SEXO
                                        (arrlist[i, 6] ?? "").ToString() + "|" +//EST_CIV
                                        nacionalidad + "|" +//NACIONALIDAD
                                        "0|" +//FLAG_CED
                                        (arrlist[i, 19] ?? "").ToString() + "|" +//CEDULA
                                        "|" +//DEPCED
                                        "|" +//MUNCED
                                        FLAG_DPI + "|" +//FLAG_DPI
                                        DPI + "|" +//DPI
                                        FLAG_PAS + "|" +//FLAG_PAS
                                        Pasaporte + "|" +//PASS
                                        "|" +//PAIS_PAS
                                        (arrlist[i, 20] ?? "").ToString() + "|" +//NIT
                                        "|" +//PASI_NIT
                                        "|" +//PROF
                                        (arrlist[i, 8] ?? "").ToString() + "|" +//DIRECCION
                                        "|" +//CASA
                                        "|" +//APTO
                                        "|" +//ZONA
                                        "|" +//COL
                                        (arrlist[i, 9] ?? "").ToString() + "|" +//MUNRES
                                        (arrlist[i, 10] ?? "").ToString() + "|" +//DEPRES
                                        "|" +//TEL
                                        (arrlist[i, 5] ?? "").ToString() + "|" +//CEL
                                        "|" +//EMAIL
                                        (arrlist[i, 17] ?? "").ToString() + "|" +//CARNE
                                        "|" +//CARR
                                        (arrlist[i, 4] ?? "").ToString() + "|" +//FACUL                                         
                                        "|" +//COD_EMP_U
                                        "|" +//PUESTO
                                        "|" +//DEP_EMP_U
                                        "|" +//COD_BARRAS
                                        "|" +//TIP_PER
                                        "|" +//ACCION
                                        "|" +//FOTO
                                        "|" +//TIPO_CTA
                                        "|" +//NO_CTA_BI
                                        "|" +//F_U
                                        "|" +//H_U
                                        "|" +//TIP_ACC
                                        "|" +//EMP_TRAB
                                        "|" +//FEC_IN_TR
                                        "|" +//ING_TR
                                        "|" +//EGR_TR
                                        "|" +//MONE_TR
                                        "|" +//PUESTO_TR
                                        "|" +//LUG_EMP
                                        "|" +//FE_IN_EMP
                                        "|" +//TEL_TR
                                        "|" +//DIR_TR
                                        "|" +//ZONA_TR
                                        "|" +//DEP_TR
                                        "|" +//MUNI_TR
                                        "|" +//PAIS_TR
                                        "|" +//ACT_EC
                                        "|" +//OTRA_NA
                                        "|" +//CONDMIG
                                        "|";//O_CONDMIG
                                        txtFile += texto;
                                        txtFile += "\r\n";
                                    }
                                    aux = 500;
                                }
                                else if (ChBusqueda.Checked && aux == 100)
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (!arrlist[i, 8].ToString().Equals(""))
                                        {
                                            bday = arrlist[i, 8].ToString().Substring(0, 10);
                                            anio = bday.Substring(0, 4);
                                            mes = bday.Substring(5, 2);
                                            dia = bday.Substring(8, 2);
                                            bday = dia + "-" + mes + "-" + anio;
                                        }
                                        else
                                        {
                                            bday = "Unknown";
                                        }
                                        if ((arrlist[i, 22] ?? "").ToString() == "-" && flag_pas == 1)
                                        {
                                            nacionalidad = "Condición Migrante";
                                        }
                                        else
                                        {
                                            nacionalidad = (arrlist[i, 22] ?? "").ToString();

                                        }

                                        if ((arrlist[i, 4] ?? "").ToString() == (arrlist[i, 18] ?? "").ToString())
                                        {
                                            Pasaporte = "";
                                            FLAG_PAS = "0";
                                            FLAG_DPI = "1";
                                            DPI = (arrlist[i, 4] ?? "").ToString();
                                        }
                                        else
                                        {
                                            Pasaporte = (arrlist[i, 19] ?? "").ToString();
                                            FLAG_PAS = "1";
                                            FLAG_DPI = "0";
                                            DPI = "";
                                            flag_pas = 1;
                                        }

                                        texto = "|" + //IDUNIV
                                        (arrlist[i, 12] ?? "").ToString() + " " + (arrlist[i, 14] ?? "").ToString() + "|" + //NOM_IMP
                                        (arrlist[i, 12] ?? "").ToString() + "|" +//NOM1
                                        (arrlist[i, 13] ?? "").ToString() + "|" +//NOM2
                                        (arrlist[i, 14] ?? "").ToString() + "|" +//APE1
                                        (arrlist[i, 15] ?? "").ToString() + "|" +//APE2
                                        (arrlist[i, 16] ?? "").ToString() + "|" +//APE3
                                        bday + "|" +//FE_NAC
                                        (arrlist[i, 17] ?? "").ToString() + "|" +//SEXO
                                        (arrlist[i, 7] ?? "").ToString() + "|" +//EST_CIV
                                        nacionalidad + "|" +//NACIONALIDAD
                                        "0|" +//FLAG_CED
                                        (arrlist[i, 20] ?? "").ToString() + "|" +//CEDULA
                                        "|" +//DEPCED
                                        "|" +//MUNCED
                                        FLAG_DPI + "|" +//FLAG_DPI
                                        DPI + "|" +//DPI
                                        FLAG_PAS + "|" +//FLAG_PAS
                                        Pasaporte + "|" +//PASS
                                        "|" +//PAIS_PAS
                                        (arrlist[i, 21] ?? "").ToString() + "|" +//NIT
                                        "|" +//PASI_NIT
                                        "|" +//PROF
                                        (arrlist[i, 9] ?? "").ToString() + "|" +//DIRECCION
                                        "|" +//CASA
                                        "|" +//APTO
                                        "|" +//ZONA
                                        "|" +//COL
                                        (arrlist[i, 10] ?? "").ToString() + "|" +//MUNRES
                                        (arrlist[i, 11] ?? "").ToString() + "|" +//DEPRES
                                        "|" +//TEL
                                        (arrlist[i, 6] ?? "").ToString() + "|" +//CEL
                                        "|" +//EMAIL
                                        (arrlist[i, 18] ?? "").ToString() + "|" +//CARNE
                                        "|" +//CARR
                                        (arrlist[i, 5] ?? "").ToString() + "|" +//FACUL                                         
                                        "|" +//COD_EMP_U
                                        "|" +//PUESTO
                                        "|" +//DEP_EMP_U
                                        "|" +//COD_BARRAS
                                        "|" +//TIP_PER
                                        "|" +//ACCION
                                        "|" +//FOTO
                                        "|" +//TIPO_CTA
                                        "|" +//NO_CTA_BI
                                        "|" +//F_U
                                        "|" +//H_U
                                        "|" +//TIP_ACC
                                        "|" +//EMP_TRAB
                                        "|" +//FEC_IN_TR
                                        "|" +//ING_TR
                                        "|" +//EGR_TR
                                        "|" +//MONE_TR
                                        "|" +//PUESTO_TR
                                        "|" +//LUG_EMP
                                        "|" +//FE_IN_EMP
                                        "|" +//TEL_TR
                                        "|" +//DIR_TR
                                        "|" +//ZONA_TR
                                        "|" +//DEP_TR
                                        "|" +//MUNI_TR
                                        "|" +//PAIS_TR
                                        "|" +//ACT_EC
                                        "|" +//OTRA_NA
                                        "|" +//CONDMIG
                                        "|";//O_CONDMIG
                                        txtFile += texto;
                                        txtFile += "\r\n";
                                        i = 11;
                                    }
                                    aux = 500;
                                }
                                else
                                {
                                    ret = 1;
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
                        lblBusqueda.Text = " ";
                    }
                    else
                    {
                        lblBusqueda.Text = "No se encontró información con los valores ingresados";
                    }
                }
                ret = 0;

            }
            else
            {
                lblBusqueda.Text = "Es necesario que seleccione e ingrese los valores para realizar una búsqueda.";
                ret = 1;
            }

            //SE GENERA EL ARCHIVO
            if (ret == 0)
            {
                Response.Clear();
                Response.Buffer = true;
                string FileName = "Reporte Empleados" + DateTime.Now + ".txt";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(txtFile);
                Response.Flush();
                Response.End();
            }
            else
            {
                lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
            }
        }

        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            lblDescarga.Visible = false;
            if (!String.IsNullOrEmpty(LbxBusqueda.Text) && !String.IsNullOrEmpty(TxtBuscador.Text))
                matrizDatos();
            else
                lblBusqueda.Text = "Es necesario que seleccione e ingrese los valores para realizar una búsqueda.";
        }

        protected void LbxBusqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            LbxBusqueda2.Items.Clear();
            LbxBusqueda2.Items.Insert(0, "");
            LbxBusqueda2.Items.Insert(1, "Nombre");
            LbxBusqueda2.Items.Insert(2, "Apellido");
            LbxBusqueda2.Items.Insert(3, "DPI");
            LbxBusqueda2.Items.Insert(4, "Dependencia");
            LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));
            TxtBuscador.Visible = true;
            TxtBuscador2.Text = "";
        }

        protected void ChBusqueda_CheckedChanged(object sender, EventArgs e)
        {
            if (ChBusqueda.Checked)
            {
                LbxBusqueda2.Visible = true;
                TxtBuscador2.Visible = true;
                TxtBuscador2.Text = "";
                LbxBusqueda2.Items.Clear();
                LbxBusqueda2.Items.Insert(0, "");
                LbxBusqueda2.Items.Insert(1, "Nombre");
                LbxBusqueda2.Items.Insert(2, "Apellido");
                LbxBusqueda2.Items.Insert(3, "DPI");
                LbxBusqueda2.Items.Insert(4, "Dependencia");
                LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));
            }
            else
            {
                LbxBusqueda2.Visible = false;
                TxtBuscador2.Visible = false;
                TxtBuscador2.Text = "";
            }
        }
        protected void LbxBusqueda2_SelectedIndexChanged(object sender, EventArgs e)
        {

            TxtBuscador2.Visible = true;
            TxtBuscador2.Text = "";
        }

        //Función que sustituye caracteres unicode a las letras correspondientes
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
                if (!ChBusqueda.Checked)
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
                                if (k == 5 && i <count-1)
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
                }
                else
                {
                    //Busqueda multiple por asignacion
                    registros = result.Count() / 6;
                    count = Math.Round(registros, 0);
                    if (registros == 0)
                        count = 1;
                    arrlist = new string[Convert.ToInt32(count), 7];
                    if (result.Count() > 7)
                    {
                        datos = 1;
                        for (int i = 0; i < count; i++)
                        {
                            for (int k = 1; k < 7; k++)
                            {
                                if (k == 6 && i < count - 1)
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
                            for (int k = 0; k < 7; k++)
                            {
                                arrlist[i, k] = result[datos];
                                datos++;
                            }
                        }
                    }
                }


                int total = 0;
                DataSetLocalRpt dsDownload = new DataSetLocalRpt();
                for (int i = 0; i < count; i++)
                {
                    desc = 1;
                    if (!ChBusqueda.Checked)
                    {
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
                    else
                    {
                        if (arrlist.Length > 6)
                        {
                            DataRow newFila = dsDownload.Tables["AllDownloadEmp"].NewRow();
                            newFila["bytes"] = arrlist[i, 6];
                            newFila["contentType"] = "jpg";
                            newFila["fileName"] = arrlist[i, 4] + ".jpg";
                            dsDownload.Tables["AllDownloadEmp"].Rows.Add(newFila);
                            total = total + 1;
                        }
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
                //desc = 0;                
            }else if(result.Count() > 0)
            {
                ret = "2";
            }
            return ret;
        }


        protected void ButtonFts_Click(object sender, EventArgs e)
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

        public void eliminarArchivo()
        {
            File.Delete(AppDomain.CurrentDomain.BaseDirectory + nombre);
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
    }
}