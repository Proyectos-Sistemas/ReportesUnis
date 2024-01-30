using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using ReportesUnis.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Security.Authentication;
using Windows.Media.Protection.PlayReady;
using DocumentFormat.OpenXml.Bibliography;

namespace ReportesUnis
{
    public partial class ActualizaciónEmpleados : System.Web.UI.Page
    {
        public static string archivoConfiguraciones = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.dat");
        public static string archivoConfiguracionesCampus = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigCampus.dat");
        public static string archivoWS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWS.dat");
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        int controlPantalla;
        string mensaje = "";
        int controlRenovacion = 0;
        int controlRenovacionFecha = 0;
        int auxConsulta = 0;
        int contadorUP = 0;
        int contadorUD = 0;
        string CONFIRMACION = "1000";
        ConsumoAPI api = new ConsumoAPI();
        string Hoy = DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10).TrimEnd();
        string HoyEffdt = DateTime.Now.ToString("dd-MM-yyyy").Substring(0, 10).TrimEnd();
        int aux = 0;
        string mensajeError = "Ocurrió un problema al actualizar su: ";

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
        protected void Page_Load(object sender, EventArgs e)
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            LeerInfoTxt();
            LeerInfoTxtPath();
            LeerCredencialesNIT();
            LeerInfoTxtSQL();
            LeerVersionesSOAPCampus();
            controlPantalla = PantallaHabilitada("Carnetización Masiva");

            if (controlPantalla >= 1)
            {
                TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");
                if (!IsPostBack)
                {
                    matrizDatos();
                    txtControlBit.Text = "0";
                    txtControlNR.Text = "0";
                    txtControlAR.Text = "0";
                    if (txtAInicial1.Value == "\r\n" || txtAInicial1.Value == "-" || txtAInicial1.Value == " ")
                    {
                        txtAInicial1.Value = null;
                    }
                    if (txtNInicial1.Value == "\r\n" || txtNInicial1.Value == "-" || txtNInicial1.Value == " ")
                    {
                        txtNInicial1.Value = null;
                    }
                    if (txtAInicial2.Value == "\r\n" || txtAInicial2.Value == "-" || txtAInicial2.Value == " ")
                    {
                        txtAInicial2.Value = null;
                    }
                    if (txtNInicial2.Value == "\r\n" || txtNInicial2.Value == "-" || txtNInicial2.Value == " ")
                    {
                        txtNInicial2.Value = null;
                    }
                    if (txtCInicial.Value == "\r\n" || txtCInicial.Value == "-" || txtCInicial.Value == " ")
                    {
                        txtCInicial.Value = null;
                    }
                    if (txtApellido1.Text == " " || txtApellido1.Text == "-" || txtApellido1.Text == " ")
                    {
                        txtApellido1.Text = null;
                    }
                    if (txtApellido2.Text == " " || txtApellido2.Text == "-" || txtApellido2.Text == " ")
                    {
                        txtApellido2.Text = null;
                    }
                    if (txtNombre1.Text == " " || txtNombre1.Text == "-" || txtNombre1.Text == " ")
                    {
                        txtNombre1.Text = null;
                    }
                    if (txtNombre2.Text == " " || txtNombre2.Text == "-" || txtNombre2.Text == " ")
                    {
                        txtNombre2.Text = null;
                    }
                    if (txtApellidoCasada.Text == " " || txtApellidoCasada.Text == "-" || txtApellidoCasada.Text == " ")
                    {
                        txtApellidoCasada.Text = null;
                    }

                    listadoRoles();
                    listadoDependencia();
                    aux = 2;
                    listadoMunicipios();
                    aux = 3;
                    listadoZonas();
                    aux = 4;
                    PaisInicial.Text = Pais.Text;
                    llenadoState();
                    Estudiante.Value = EsEstudiante().ToString();

                    if (Convert.ToInt16(Estudiante.Value) > 0)
                    {
                        recibos.Style["display"] = "block";
                        mostrarInformaciónEstudiante();
                        CmbRoles.Items.Add(new ListItem("Estudiante", "A"));
                        ControlRoles.Value = ControlRoles.Value + " Estudiante";
                    }
                    else if (containsProf.Value == "1")
                    {
                        mostrarInformaciónProfesores();
                    }

                    if (TxtNombreR.Text == "\r\n" || TxtNombreR.Text == "\n")
                    {
                        TxtNombreR.Text = null;
                    }
                    if (TxtApellidoR.Text == "\r\n" || TxtApellidoR.Text == "\n")
                    {
                        TxtApellidoR.Text = null;
                    }
                    if (TxtCasadaR.Text == "\r\n" || TxtCasadaR.Text == "\n")
                    {
                        TxtCasadaR.Text = null;
                    }
                    if (InicialNR1.Value == "\r\n" || InicialNR1.Value == "\n")
                    {
                        InicialNR1.Value = null;
                    }
                    if (InicialNR2.Value == "\r\n" || InicialNR2.Value == "\n")
                    {
                        InicialNR2.Value = null;
                    }
                    if (InicialNR3.Value == "\r\n" || InicialNR3.Value == "\n")
                    {
                        InicialNR3.Value = null;
                    }

                    if (txtNit.Text == "CF")
                    {
                        txtNit.Enabled = false;
                        RadioButtonNombreSi.Checked = true;
                        ControlCF.Value = "CF";
                        ControlCF2.Value = "1";
                        ValidarNIT.Enabled = false;
                        if (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value != TxtCasadaR.Text || String.IsNullOrEmpty(InicialNR1.Value) && ChangeNIT.Value == "1")
                        {
                            PaisNit.Text = cMBpAIS.SelectedValue;
                            DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                            MunicipioNit.Text = CmbMunicipio.SelectedValue;
                            TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                            TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                            TxtCasadaR.Text = txtApellidoCasada.Text;
                            TxtDiRe1.Text = txtDireccion.Text;
                            TxtDiRe2.Text = txtDireccion2.Text;
                            TxtDiRe3.Text = txtZona.Text;
                            txtNit.Text = "CF";
                            ControlRBS.Value = "1";
                        }
                        else
                        {
                            ControlRBS.Value = "0";
                        }
                    }
                    else
                    {
                        RadioButtonNombreNo.Checked = true;
                        ControlCF2.Value = "2";
                        TxtDiRe1.Enabled = true;
                        TxtDiRe2.Enabled = true;
                        TxtDiRe3.Enabled = true;
                        ValidarNIT.Enabled = true;
                        txtNit.Enabled = true;
                    }

                    if (String.IsNullOrEmpty(txtdPI.Text))
                    {
                        BtnActualizar.Visible = false;
                        lblActualizacion.Text = "El usuario utilizado no se encuentra registrado como empleados";
                        tabla.Visible = false;
                    }
                }
                else
                    aux = 2;
            }
            else
            {
                lblActualizacion.Text = "¡IMPORTANTE! Esta página no está disponible, ¡Permanece atento a nuevas fechas para actualizar tus datos!";
                controlCamposVisibles(false);
            }
        }

        //FUNCIONES
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
        void LeerInfoTxtPath()
        {
            string rutaCompleta = CurrentDirectory + "PathAlmacenamiento.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath.Text = line;
                file.Close();
            }
        }
        void LeerCredencialesNIT()
        {
            string rutaCompleta = CurrentDirectory + "CredencialesNIT.txt";

            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                CREDENCIALES_NIT.Value = linea1;
                URL_NIT.Value = linea2;
                file.Close();
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
        public void controlCamposVisibles(bool Condicion)
        {
            CargaFotografia.Visible = Condicion;
            tabla.Visible = Condicion;
            tbactualizar.Visible = Condicion;
            tbactualizar.Visible = Condicion;
            InfePersonal.Visible = Condicion;
            divActividad.Visible = Condicion;
        }
        public string sustituirCaracteres()
        {
            //Sustituye las comillas dobles y elimina los primeros caracteres que corresponden a los Headers
            string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @"\n+", "");

            try
            {
                if (aux == 0)
                {
                    int largo = 0;
                    string nombre = TextUser.Text.TrimEnd(' ');
                    largo = nombre.Length + 285;
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
                        int mun = CmbDepartamento.SelectedValue.Length + 34;
                        int pais = cMBpAIS.SelectedValue.Length + mun;
                        sustituto = sustituto.Remove(0, pais);
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
                else if (aux == 6) //ROLES
                {
                    int largo = 26;
                    string nombre = TextUser.Text.TrimEnd(' ');
                    largo = nombre.Length + largo;
                    sustituto = sustituto.Remove(0, largo);
                }
                else if (aux == 7) //PUESTO Y DEPENDENCIA
                {
                    int largo = 32;
                    string nombre = TextUser.Text.TrimEnd(' ');
                    largo = nombre.Length + largo;
                    sustituto = sustituto.Remove(0, largo);
                }
            }
            catch (Exception)
            {
            }
            Txtsustituto.Text = sustituto;
            return sustituto;
        }
        public void listadoRoles()
        {
            int count = 0;
            aux = 6;
            string[] result = sustituirCaracteres().Split('|');
            count = result.Length;
            int largo = count / 2;
            if (count == 1)
                largo = 1;
            string[] resultadoVista = new string[largo];
            string[] resultadoValores = new string[largo];
            string usuario = TextUser.Text.TrimEnd(' ');
            ControlRoles.Value = result.ToString();
            try
            {
                int j = 0;
                for (int i = 0; i < count;)
                {
                    if (i == 0)
                    {
                        resultadoVista[j] = result[i];
                        ControlRoles.Value = result[i];
                    }
                    else
                    {
                        resultadoVista[j] = result[i];
                        ControlRoles.Value = ControlRoles.Value + " " + result[i];
                    }
                    i = i + 2;
                    j++;
                }

                j = 0;
                for (int i = 1; i < count;)
                {
                    if (i == 1)
                    {
                        if (result[i].Substring(0, 1) == "O" || result[i].Substring(0, 1) == "S" || result[i].Substring(0, 1) == "N")
                        {
                            resultadoValores[j] = (result[i].Substring(0, 1));
                        }
                        else
                        {
                            resultadoValores[j] = result[i].Substring(0, 1);
                        }
                    }
                    else
                    {
                        resultadoValores[j] = result[i];
                    }
                    i = i + 2;
                    j++;
                }

                if (resultadoVista[0].ToString().Equals(""))
                {
                    resultadoVista[0] = "-";
                    resultadoValores[0] = " ";
                }
                Dictionary<string, string> rolesDictionary = new Dictionary<string, string>();
                for (int i = 0; i < largo; i++)
                {
                    rolesDictionary.Add(resultadoVista[i], resultadoValores[i]);
                }

                // Enlazar el diccionario al DropDownList
                CmbRoles.DataSource = rolesDictionary;
                CmbRoles.DataTextField = "Key";       // La propiedad "Key" del diccionario se utilizará para mostrar la información
                CmbRoles.DataValueField = "Value";    // La propiedad "Value" del diccionario se utilizará como valor interno
                CmbRoles.DataBind();
            }
            catch (Exception)
            {
                CmbRoles.DataSource = "-";
                CmbRoles.DataTextField = "-";
                CmbRoles.DataValueField = "-";
            }

            if (CmbRoles.Items.Cast<ListItem>().Any(item => item.Text == "Profesor"))
            {
                containsProf.Value = "1";
            }
            else
            {
                containsProf.Value = "2";
            }
        }
        public void listadoDependencia()
        {
            aux = 7;
            string[] result = sustituirCaracteres().Split('|');
            try
            {
                txtPuesto.Text = result[0];
                txtFacultad.Text = result[1];
            }
            catch (Exception)
            {
                txtPuesto.Text = "";
                txtFacultad.Text = "";
            }
        }
        public void matrizDatos()
        {
            aux = 0;
            string[] result = sustituirCaracteres().Split('|');
            decimal registros = 0;
            decimal count = 0;
            int datos = 0;
            string[,] arrlist;
            int valor = 28;

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
                    if (valor == 28)
                    {
                        //Generacion de matriz para llenado de grid desde una consulta
                        for (int i = 0; i < count; i++)
                        {
                            txtNombre1.Text = (arrlist[i, 1] ?? "").ToString();
                            txtApellido1.Text = (arrlist[i, 2] ?? "").ToString();
                            txtNInicial1.Value = (arrlist[i, 1] ?? "").ToString();
                            txtAInicial1.Value = (arrlist[i, 2] ?? "").ToString();
                            txtdPI.Text = (arrlist[i, 3] ?? "").ToString();
                            txtTelefono.Text = (arrlist[i, 4] ?? "").ToString().TrimEnd().Replace('-', ' ');
                            TelefonoInicial.Value = (arrlist[i, 4] ?? "").ToString().TrimEnd().Replace('-', ' ');

                            estado = arrlist[i, 5].ToString().Replace('-', ' ').TrimEnd();
                            if (!estado.Equals(""))
                            {
                                EstadoCivil.Value = (arrlist[i, 5] ?? "1").ToString();
                                if (arrlist[i, 5].ToString().Equals("1"))
                                {
                                    estado = "Soltero";
                                    TrueEstadoCivil.Value = "S";
                                    EstadoCivilInicialNumero.Value = "1";
                                }
                                else if (arrlist[i, 5].ToString().Equals("2"))
                                {
                                    estado = "Casado";
                                    TrueEstadoCivil.Value = "M";
                                    EstadoCivilInicialNumero.Value = "2";
                                }
                            }
                            else
                            {
                                estado = "Sin Información";
                                TrueEstadoCivil.Value = "U";
                                EstadoCivilInicialNumero.Value = "";
                            }

                            CmbEstado.SelectedValue = estado.ToString();

                            if (!arrlist[i, 6].ToString().Replace('-', ' ').TrimEnd().Equals(""))
                            {
                                bday = arrlist[i, 6].ToString().Substring(0, 10);
                                anio = bday.Substring(0, 4);
                                mes = bday.Substring(5, 2);
                                dia = bday.Substring(8, 2);
                                bday = dia + "-" + mes + "-" + anio;
                                FechaNac.Value = anio + "-" + mes + "-" + dia;
                            }
                            else
                            {
                                bday = "Unknown";
                            }

                            txtCumple.Text = bday;

                            txtDireccion.Text = (arrlist[i, 7] ?? "").ToString().TrimEnd().Replace('%', ' ').TrimEnd();
                            Direccion1.Text = txtDireccion.Text;
                            aux = 4;
                            listaPaises();
                            cMBpAIS.SelectedValue = (arrlist[i, 10] ?? "").ToString().TrimEnd().Replace('-', ' ');
                            Pais.Text = cMBpAIS.SelectedValue;
                            aux = 1;
                            listaDepartamentos();
                            CmbDepartamento.SelectedValue = (arrlist[i, 9] ?? "").ToString().TrimEnd().Replace('-', ' ');
                            Departmento.Text = CmbDepartamento.SelectedValue;
                            aux = 2;
                            listadoMunicipios();
                            CmbMunicipio.SelectedValue = (arrlist[i, 8] ?? "").ToString().TrimEnd().Replace('-', ' ');
                            Municipio.Text = CmbMunicipio.SelectedValue;
                            txtDireccion2.Text = arrlist[i, 11].ToString().TrimEnd().Replace('%', ' ').TrimEnd();
                            Direccion2.Text = txtDireccion2.Text;
                            aux = 3;
                            listadoZonas();
                            txtZona.Text = (arrlist[i, 12] ?? "").ToString();
                            Zona.Text = txtZona.Text;
                            UserEmplid.Text = (arrlist[i, 13] ?? "").ToString();
                            txtNombre2.Text = (arrlist[i, 14] ?? "").ToString();
                            txtApellido2.Text = (arrlist[i, 15] ?? "").ToString().TrimEnd().Replace('-', ' ').TrimEnd();
                            txtNInicial2.Value = (arrlist[i, 14] ?? "").ToString();
                            txtAInicial2.Value = (arrlist[i, 15] ?? "").ToString();
                            txtApellidoCasada.Text = (arrlist[i, 16] ?? "").ToString().Replace('-', ' ');
                            txtCInicial.Value = (arrlist[i, 16] ?? "").ToString().Replace('-', ' ');
                            txtCarne.Text = (arrlist[i, 17] ?? "").ToString();

                            FlagDpi.Value = (arrlist[i, 18] ?? "0").ToString();
                            FlagPasaporte.Value = (arrlist[i, 19] ?? "0").ToString();
                            FlagCedula.Value = "0";
                            ConMig.Value = (arrlist[i, 20] ?? "").ToString().Replace('-', ' ').TrimEnd();
                            TipoDoc.Value = (arrlist[i, 21] ?? "").ToString();
                            NIT.Value = (arrlist[i, 22] ?? "").ToString();
                            CmbPaisNIT.SelectedValue = (arrlist[i, 23] ?? "").ToString();
                            PaisNit.Text = (arrlist[i, 23] ?? "").ToString();
                            PaisPass.Value = (arrlist[i, 24] ?? "").ToString().Replace('-', ' ').TrimEnd();
                            TxtCorreoInstitucional.Text = (arrlist[i, 25] ?? "").ToString();
                            Sexo.Value = (arrlist[i, 26] ?? "").ToString().TrimEnd().Replace('-', ' ').TrimEnd();
                            TxtCorreoPersonal.Text = (arrlist[i, 27] ?? "").ToString().TrimEnd().Replace('-', ' ').TrimEnd();
                            CorreoInicial.Value = (arrlist[i, 27] ?? "").ToString().TrimEnd().Replace('-', ' ').TrimEnd();
                            if (FlagDpi.Value == "1")
                            {
                                DeptoCui.Value = txtdPI.Text.Substring(9, 2);
                                MuniCui.Value = txtdPI.Text.Substring(11, 2);
                                NoCui.Value = txtdPI.Text.Substring(0, 9);
                                DPI.Value = txtdPI.Text;
                            }
                            if (FlagPasaporte.Value == "1")
                            {
                                Pasaporte.Value = txtdPI.Text;
                                DPI.Value = null;
                            }
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
        public void listaPaises()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            aux = 4;
            string sustituto = DecodeStringFromBase64(Consultar()).Replace('"', '\n');
            sustituto = Regex.Replace(sustituto, @"\n+", "|");
            int largo = 21;
            sustituto = sustituto.Remove(0, largo);
            sustituto = sustituto + "-";
            sustituto = sustituto.TrimEnd('|');
            string[] result = new string[23];
            result = sustituto.Split('|');
            cMBpAIS.DataSource = result;
            cMBpAIS.DataTextField = "";
            cMBpAIS.DataValueField = "";
            cMBpAIS.DataBind();
            lblActualizacion.Text = "";
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
        }
        public void listaDepartamentos()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
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
                    CmbDepartamento.DataSource = resultado;
                }
                else
                {
                    resultado[0] = arrlist[0, 0];
                    CmbDepartamento.DataSource = resultado;
                }
                Pais.Text = cMBpAIS.Text;
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
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
        }
        public void listadoMunicipios()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            int count = 0;
            int depto = cMBpAIS.SelectedValue.ToString().Length;
            string[] result = sustituirCaracteres().Split('|');
            count = result.Length;
            if (result[0].Contains("UNICO"))
            {
                count = 1;
            }
            string[] resultado = new string[count / 2 + 1];
            try
            {
                int j = 0;
                for (int i = 0; i < count;)
                {
                    if (count == 1 || i == count - 1)
                    {
                        resultado[j] = result[i];
                    }
                    else
                    {
                        string palabra = result[i];
                        resultado[j] = StringExtensions.RemoveEnd(palabra, depto);
                    }
                    i = i + 2;
                    j++;
                }

                if (resultado[0].ToString().Equals(""))
                    resultado[0] = "-";
                CmbMunicipio.DataSource = resultado;
                CmbMunicipio.DataTextField = "";
                CmbMunicipio.DataValueField = "";
                CmbMunicipio.DataBind();
                lblActualizacion.Text = "";
            }
            catch (Exception)
            {
                CmbMunicipio.DataSource = "-";
                CmbMunicipio.DataTextField = "-";
                CmbMunicipio.DataValueField = "-";
            }
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
        }
        public void listaDepartamentosNit()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPaisNIT.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDepartamentoNIT.DataSource = ds;
                        CmbDepartamentoNIT.DataTextField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataValueField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamentoNIT.DataTextField = "";
                        CmbDepartamentoNIT.DataValueField = "";
                    }
                }
            }
            ISESSION.Value = "0";
            banderaSESSION.Value = "1";
        }
        public void listaPaisesNit()
        {
            banderaSESSION.Value = "1";
            string where = "";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' PAIS, ' ' COUNTRY FROM DUAL UNION SELECT * FROM (SELECT DESCR AS PAIS, COUNTRY FROM SYSADM.PS_COUNTRY_TBL " + where + ")PAIS ORDER BY 1 ASC";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbPaisNIT.DataSource = ds;
                    CmbPaisNIT.DataTextField = "PAIS";
                    CmbPaisNIT.DataValueField = "PAIS";
                    CmbPaisNIT.DataBind();
                    con.Close();
                }
            }
            banderaSESSION.Value = "1";
        }
        public void listadoMunicipiosNit()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(CmbDepartamentoNIT.SelectedValue.ToString()))
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                            "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamentoNIT.SelectedValue + "') " +
                            "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                        else
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT ' ' MUNICIPIO FROM DUAL";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                    }
                    catch (Exception)
                    {
                        CmbMunicipioNIT.DataSource = "-";
                        CmbMunicipioNIT.DataTextField = "-";
                        CmbMunicipioNIT.DataValueField = "-";
                    }
                }
            }
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
        }
        public void listadoZonas()
        {
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
            int count = 0;
            int mun = CmbMunicipio.SelectedValue.ToString().Length;
            string[] result = sustituirCaracteres().Split('|');
            count = result.Count();
            string[,] arrlist;
            string[] resultado;
            arrlist = new string[Convert.ToInt32(count), 2];

            try
            {
                if (result.Count() > 1)
                {
                    resultado = new string[count + 1];
                    for (int i = 0; i < count + 1; i++)
                    {
                        if (i == count - 1)
                        {
                            string palabra = result[i];
                            resultado[i] = palabra;
                        }
                        else if (i != count - 1 && i < count - 1)
                        {
                            string palabra = result[i];
                            resultado[i] = StringExtensions.RemoveEnd(palabra, mun);
                        }
                        else
                        {
                            resultado[i] = "-";
                        }
                    }

                    txtZona.DataSource = resultado;
                    txtZona.DataTextField = "";
                    txtZona.DataValueField = "";
                    txtZona.DataBind();
                    lblActualizacion.Text = "";
                }
                else
                {
                    resultado = new string[count];
                    resultado[0] = "-";
                    txtZona.DataSource = resultado;
                    txtZona.DataTextField = "";
                    txtZona.DataValueField = "";
                    txtZona.DataBind();
                }
            }
            catch (Exception)
            {
                resultado = new string[count];
                resultado[0] = "-";
                txtZona.DataSource = resultado;
                txtZona.DataTextField = "";
                txtZona.DataValueField = "";
            }
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
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
        private string consultaNit(string nit)
        {
            var body = "{\"Credenciales\" : \"" + CREDENCIALES_NIT.Value + "\",\"NIT\":\"" + nit + "\"}";
            string respuesta = api.PostNit(URL_NIT.Value, body);
            return respuesta;
        }
        public int EsEstudiante()
        {
            string constr = TxtURL.Text;
            int CONTADOR = 0;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT  COUNT (*) AS CONTADOR " +
                                        "FROM " +
                                        "( " +
                                        "	SELECT  A.* " +
                                        "	       ,PN.NATIONAL_ID_TYPE " +
                                        "	       ,PN.NATIONAL_ID " +
                                        "	       ,ROW_NUMBER() OVER (PARTITION BY A.EMPLID ORDER BY  CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN 1 ELSE 2 END) FILA " +
                                        "	FROM " +
                                        "	( " +
                                        "		SELECT  DISTINCT PD.FIRST_NAME " +
                                        "		       ,PD.LAST_NAME " +
                                        "		       ,PD.EMPLID " +
                                        "		FROM SYSADM.PS_PERS_DATA_SA_VW PD " +
                                        "		LEFT JOIN SYSADM.PS_PERS_NID PN " +
                                        "		ON PD.EMPLID = PN.EMPLID " +
                                        "		LEFT JOIN SYSADM.PS_ADDRESSES A " +
                                        "		ON PD.EMPLID = A.EMPLID AND ADDRESS_TYPE = 'HOME'AND A.EFFDT = ( " +
                                        "		SELECT  MAX(EFFDT) " +
                                        "		FROM SYSADM.PS_ADDRESSES A2 " +
                                        "		WHERE A.EMPLID = A2.EMPLID " +
                                        "		AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE ) " +
                                        "		LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD " +
                                        "		ON PD.EMPLID = PPD.EMPLID " +
                                        "		LEFT JOIN SYSADM.PS_STATE_TBL ST " +
                                        "		ON PPD.STATE = ST.STATE " +
                                        "		LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT " +
                                        "		ON PD.EMPLID = CT.EMPLID " +
                                        "		LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD " +
                                        "		ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                        "		LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT " +
                                        "		ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                        "		JOIN SYSADM.PS_TERM_TBL TT " +
                                        "		ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION AND (SYSDATE BETWEEN TT.TERM_BEGIN_DT AND TT.TERM_END_DT)LEFT " +
                                        "		JOIN SYSADM.PS_PERSONAL_PHONE PP " +
                                        "		ON PD.EMPLID = PP.EMPLID AND PP.PHONE_TYPE = 'HOME' " +
                                        "		LEFT JOIN SYSADM.PS_COUNTRY_TBL C " +
                                        "		ON A.COUNTRY = C.COUNTRY " +
                                        "		WHERE PN.NATIONAL_ID = '" + TextUser.Text + "'   " +
                                        "    ) A " +
                                        "    LEFT JOIN SYSADM.PS_PERS_NID PN ON A.EMPLID = PN.EMPLID AND NATIONAL_ID_TYPE IN ('DPI','PAS')  " +
                                        ") B  " +
                                        "WHERE FILA='1' AND NATIONAL_ID <> ' '  " +
                                        "AND NATIONAL_ID ='" + TextUser.Text + "'  " +
                                        "ORDER BY 1";

                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CONTADOR = Convert.ToInt16(reader["CONTADOR"]);
                    }
                }
            }
            return CONTADOR;
        }
        private string mostrarInformaciónEstudiante()
        {
            string constr = TxtURL.Text;
            string emplid = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    emplid = txtCarne.Text;

                    cmd.Connection = con;
                    cmd.CommandText = "SELECT APELLIDO_NIT, NOMBRE_NIT, CASADA_NIT, NIT, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, CNT, MUNICIPIO_NIT, DEPARTAMENTO_NIT, STATE_NIT, PAIS_NIT, PHONE, STATE, EMAILPERSONAL, CARRERA, FACULTAD FROM ( " +
                                        "SELECT PD.EMPLID, PP.PHONE, ST.STATE,(SELECT EXTERNAL_SYSTEM_ID FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NIT," +
                                        "(SELECT PNA.FIRST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) NOMBRE_NIT, " +
                                        "(SELECT PNA.LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY EFFDT DESC FETCH FIRST 1 ROWS ONLY) APELLIDO_NIT, " +
                                        "(SELECT SECOND_LAST_NAME FROM SYSADM.PS_NAMES PNA WHERE PNA.NAME_TYPE = 'REC' AND PNA.EMPLID='" + emplid + "' ORDER BY PNA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) CASADA_NIT, " +
                                        "(SELECT ADDRESS1 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION1_NIT, " +
                                        "(SELECT ADDRESS2 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION2_NIT, " +
                                        "(SELECT ADDRESS3 FROM SYSADM.PS_ADDRESSES PA WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY) DIRECCION3_NIT, " +
                                        "NVL((SELECT C.DESCR FROM SYSADM.PS_ADDRESSES PA JOIN SYSADM.PS_COUNTRY_TBL C ON PA.COUNTRY = C.COUNTRY AND PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY),' ') PAIS_NIT, " +
                                        "NVL((SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY), ' ' ) MUNICIPIO_NIT, " +
                                        "NVL((SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY), ' ' ) DEPARTAMENTO_NIT, " +
                                        "NVL((SELECT ST.STATE FROM SYSADM.PS_STATE_TBL ST JOIN SYSADM.PS_ADDRESSES PA ON ST.STATE = PA.STATE WHERE PA.ADDRESS_TYPE = 'REC' AND PA.EMPLID='" + emplid + "' ORDER BY PA.EFFDT DESC FETCH FIRST 1 ROWS ONLY),' ') STATE_NIT, " +
                                        "NVL((SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND EMAIL.E_ADDR_TYPE IN ('HOM1') FETCH FIRST 1 ROWS ONLY), ' ') EMAILPERSONAL , " +
                                        "TT.TERM_BEGIN_DT, ROW_NUMBER() OVER (PARTITION BY PD.EMPLID ORDER BY 18 DESC) CNT, C.DESCR PAIS, " +
                                        "APD.DESCR CARRERA, AGT.DESCR FACULTAD " +
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
                                        "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                        "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                        "AND CT.INSTITUTION = APD.INSTITUTION " +
                                        "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                        "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                        "JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                        "AND CT.INSTITUTION = TT.INSTITUTION " +
                                        "AND (SYSDATE BETWEEN TT.TERM_BEGIN_DT AND TT.TERM_END_DT)" +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_PHONE PP ON PD.EMPLID = PP.EMPLID " +
                                        "AND PP.PHONE_TYPE = 'HOME' " +
                                        "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON A.COUNTRY = C.COUNTRY " +
                                        "WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                                        "ORDER BY CT.FULLY_ENRL_DT DESC" +
                                        ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtApellidoR.Text = reader["APELLIDO_NIT"].ToString();
                        InicialNR2.Value = reader["APELLIDO_NIT"].ToString();
                        TxtNombreR.Text = reader["NOMBRE_NIT"].ToString();
                        InicialNR1.Value = reader["NOMBRE_NIT"].ToString();
                        TxtCasadaR.Text = reader["CASADA_NIT"].ToString();
                        InicialNR3.Value = reader["CASADA_NIT"].ToString();
                        StateNIT.Text = reader["STATE_NIT"].ToString();
                        txtNit.Text = reader["NIT"].ToString();
                        TrueNit.Value = txtNit.Text;
                        ControlCF.Value = reader["NIT"].ToString();
                        ChangeNIT.Value = "0";
                        TxtDiRe1.Text = reader["DIRECCION1_NIT"].ToString();
                        TxtDiRe2.Text = reader["DIRECCION2_NIT"].ToString();
                        TxtDiRe3.Text = reader["DIRECCION3_NIT"].ToString();
                        State.Text = reader["STATE"].ToString().Replace("---", " ");
                        TruePhone.Text = reader["PHONE"].ToString();
                        TrueEmail.Text = reader["EMAILPERSONAL"].ToString();
                        Carrera.Value = reader["CARRERA"].ToString();
                        Facultad.Value = reader["FACULTAD"].ToString();
                        if (!String.IsNullOrEmpty(reader["PAIS_NIT"].ToString()))
                        {
                            CmbPaisNIT.SelectedValue = reader["PAIS_NIT"].ToString();
                            PaisNit.Text = reader["PAIS_NIT"].ToString();
                            llenadoPaisnit();
                            llenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO_NIT"].ToString();
                            DepartamentoNit.Text = reader["DEPARTAMENTO_NIT"].ToString();
                            llenadoMunicipioNIT();
                            CmbMunicipioNIT.SelectedValue = reader["MUNICIPIO_NIT"].ToString();
                            MunicipioNit.Text = reader["MUNICIPIO_NIT"].ToString();
                        }
                        else if (RadioButtonNombreSi.Checked)
                        {
                            llenadoPaisnit();
                            if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                                CmbPaisNIT.SelectedValue = reader["PAIS"].ToString();
                            else
                                CmbPaisNIT.SelectedValue = "";
                            llenadoDepartamentoNit();
                            CmbDepartamentoNIT.SelectedValue = reader["DEPARTAMENTO"].ToString();
                            llenadoMunicipioNIT();
                            CmbMunicipioNIT.SelectedValue = reader["MUNICIPIO"].ToString();
                        }
                        else
                        {
                            llenadoPaisnit();
                        }

                        if (TxtCorreoPersonal.Text.IsNullOrWhiteSpace())
                            TxtCorreoPersonal.Text = TrueEmail.Text;
                    }

                    cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE ='REC' AND EMPLID = '" + UserEmplid.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
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
                    con.Close();
                    fotoAlmacenada();
                }
            }
            return emplid;
        }
        private string mostrarInformaciónProfesores()
        {
            string constr = TxtURL.Text;
            string emplid = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    emplid = txtCarne.Text;
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * " +
                                        "FROM " +
                                        "( " +
                                        "SELECT A.*,  " +
                                        "PN.NATIONAL_ID_TYPE, PN.NATIONAL_ID, " +
                                        "ROW_NUMBER() OVER (PARTITION BY A.EMPLID ORDER BY CASE WHEN PN.NATIONAL_ID_TYPE='DPI' THEN 1 ELSE 2 END) FILA " +
                                        "FROM " +
                                        "( " +
                                        "SELECT DISTINCT PD.FIRST_NAME, PD.LAST_NAME, PD.EMPLID , PP.PHONE, ST.STATE," +
                                        "NVL((SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = '" + emplid + "' AND EMAIL.E_ADDR_TYPE IN ('HOM1') FETCH FIRST 1 ROWS ONLY), ' ') EMAILPERSONAL " +
                                        "FROM SYSADM.PS_PERSONAL_DATA PD  " +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_PHONE PP ON PD.EMPLID = PP.EMPLID AND PP.PHONE_TYPE = 'HOME' " +
                                        "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                        ") A " +
                                        "LEFT JOIN SYSADM.PS_PERS_NID PN ON A.EMPLID = PN.EMPLID AND NATIONAL_ID_TYPE IN ('DPI','PAS') " +
                                        ") B " +
                                        "WHERE FILA='1' AND NATIONAL_ID <> ' ' " +
                                        "AND NATIONAL_ID ='" + TextUser.Text + "' " +
                                        "ORDER BY 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        State.Text = reader["STATE"].ToString().Replace("---", " ");
                        TruePhone.Text = reader["PHONE"].ToString();
                        TrueEmail.Text = reader["EMAILPERSONAL"].ToString();
                    }
                    con.Close();

                    if (TxtCorreoPersonal.Text.IsNullOrWhiteSpace())
                        TxtCorreoPersonal.Text = TrueEmail.Text;
                    fotoAlmacenada();
                }
            }
            return emplid;
        }
        protected void llenadoState()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    string descrip = "";
                    descrip = CmbMunicipio.SelectedValue + "|" + CmbDepartamento.SelectedValue;
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT NVL(VCHRVALORCAMPUS,NULL) as STATE " +
                        "FROM UNIS_INTERFACES.TBLEQUIVALENCIASHCMCAMPUS " +
                        "WHERE VCHRLOOKUPTYPE='MUNICIPIO' AND  " +
                        "UPPER(VCHRVALORHCM)=UPPER('" + descrip.TrimEnd('-') + "')";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        State.Text = reader["STATE"].ToString();
                    }
                    con.Close();
                }
            }

            changeCombobox();
        }
        public void llenadoPaisnit()
        {
            banderaSESSION.Value = "1";
            string where = "";
            string constr = TxtURL.Text;
            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT ' ' PAIS, ' ' COUNTRY FROM DUAL UNION SELECT * FROM (SELECT DESCR AS PAIS, COUNTRY FROM SYSADM.PS_COUNTRY_TBL " + where + ")PAIS ORDER BY 1 ASC";
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbPaisNIT.DataSource = ds;
                        CmbPaisNIT.DataTextField = "PAIS";
                        CmbPaisNIT.DataValueField = "PAIS";
                        CmbPaisNIT.DataBind();
                        con.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            banderaSESSION.Value = "1";
        }
        public void llenadoMunicipioNIT()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(CmbDepartamentoNIT.SelectedValue.ToString()))
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                            "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamentoNIT.SelectedValue + "') " +
                            "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                        else
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT ' ' MUNICIPIO FROM DUAL";
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            CmbMunicipioNIT.DataSource = ds;
                            CmbMunicipioNIT.DataTextField = "MUNICIPIO";
                            CmbMunicipioNIT.DataValueField = "MUNICIPIO";
                            CmbMunicipioNIT.DataBind();
                            con.Close();
                        }
                    }
                    catch (Exception)
                    {
                        CmbMunicipioNIT.DataSource = "-";
                        CmbMunicipioNIT.DataTextField = "-";
                        CmbMunicipioNIT.DataValueField = "-";
                    }
                }
            }
            banderaSESSION.Value = "0";
            ISESSION.Value = "0";
        }
        public void llenadoDepartamentoNit()
        {
            banderaSESSION.Value = "1";
            ISESSION.Value = "0";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPaisNIT.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDepartamentoNIT.DataSource = ds;
                        CmbDepartamentoNIT.DataTextField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataValueField = "DEPARTAMENTO";
                        CmbDepartamentoNIT.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamentoNIT.DataTextField = "";
                        CmbDepartamentoNIT.DataValueField = "";
                    }
                }
            }
            ISESSION.Value = "0";
            banderaSESSION.Value = "1";
        }
        protected void llenadoStateNIT()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (!String.IsNullOrEmpty(CmbMunicipioNIT.SelectedValue))
                    {
                        string descrip = "";
                        if (cMBpAIS.SelectedValue == "Guatemala")
                        {
                            descrip = CmbMunicipioNIT.SelectedValue + "-" + CmbDepartamentoNIT.SelectedValue;
                        }
                        else
                        {
                            descrip = CmbDepartamentoNIT.SelectedValue;
                        }
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + descrip.TrimEnd('-') + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNIT.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                    else
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbDepartamentoNIT.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            StateNIT.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                }
            }
        }
        protected int ControlRenovacion(string cadena)
        {
            string txtExiste4 = "SELECT CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_CONTROL_CARNET " + cadena;
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
                        cmd.CommandText = txtExiste4;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONTADOR"].ToString();
                        }

                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return Convert.ToInt32(control);
        }
        protected int ControlAC(string cadena)
        {
            txtExiste4.Text = "SELECT COUNT(*) CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_CONTROL_CARNET " + cadena;
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
                        cmd.CommandText = txtExiste4.Text;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONTADOR"].ToString();
                        }
                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return Convert.ToInt32(control);
        }
        protected string homologaPais(string Combo)
        {
            string txtExiste4 = "SELECT NVL(VCHRVALORCAMPUS,NULL) AS CODIGO " +
                "FROM UNIS_INTERFACES.TBLEQUIVALENCIASHCMCAMPUS " +
                "WHERE VCHRLOOKUPTYPE='COUNTRY' AND  " +
                "UPPER(VCHRDESCRIPCION)=UPPER('" + Combo + "')";
            string constr = TxtURL.Text;
            string control = "0";
            string codigo = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = txtExiste4;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            codigo = reader["CODIGO"].ToString();
                        }
                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return codigo;
        }
        public void EnvioCorreo()
        {
            string htmlBody = LeerBodyEmail("bodyIngresoEmpleados-Operador.txt");
            string[] datos = LeerInfoEmail("datosIngresoEmpleados-Operador.txt");
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Actualización Empleados", credenciales[3]));
            email.To.Add(new MailboxAddress(credenciales[0], credenciales[3]));

            email.Subject = datos[0];
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(credenciales[1], credenciales[2]);

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                catch (Exception ex)
                {
                    log("ERROR - Envio de correo para el operador. ");
                    lblActualizacion.Text = ex.ToString();
                }
            }
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
        public void EnvioCorreoEmpleado()
        {
            string htmlBody = LeerBodyEmail("bodyIngresoEmpleados.txt");
            string[] datos = LeerInfoEmail("datosIngresoEmpleados.txt");
            string[] credenciales = LeerCredencialesMail();
            var email = new MimeMessage();
            var para = txtNombre1.Text + " " + txtPrimerApellido.Text;

            email.From.Add(new MailboxAddress(credenciales[0], credenciales[3]));
            email.To.Add(new MailboxAddress(para, TxtCorreoInstitucional.Text));

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
                    log("ERROR - Envio de correo para " + TxtCorreoInstitucional.Text);
                    lblActualizacion.Text = ex.ToString();
                }
            }
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
        protected string IngresoDatos()
        {
            if (!urlPath2.Value.IsNullOrWhiteSpace())
            {
                if (String.IsNullOrEmpty(txtNit.Text))
                {
                    txtNit.Text = "CF";
                }

                try
                {
                    txtNombreAPEX.Text = null;
                    string constr = TxtURL.Text;
                    string codPaisNIT = "";
                    string RegistroCarne = "0";

                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        con.Open();
                        OracleTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Transaction = transaction;
                            //Obtener codigo país nit
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT COUNTRY FROM SYSADM.PS_COUNTRY_TBL WHERE DESCR = '" + CmbPaisNIT.SelectedValue + "'";
                            OracleDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                codPaisNIT = reader["COUNTRY"].ToString();
                            }

                            //SE VALIDA QUE NO EXISTA INFORMACIÓN REGISTRADA
                            cmd.Transaction = transaction;
                            cmd.Connection = con;
                            txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO =SUBSTR('" + txtCarne.Text + "',0,13)";
                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO =SUBSTR('" + txtCarne.Text + "',0,13)";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                RegistroCarne = reader["CONTADOR"].ToString();
                            }

                            txtExiste.Text = RegistroCarne.ToString() + " registros";
                            string nombreArchivo = txtCarne.Text + ".jpg";
                            string ruta = txtPath.Text + nombreArchivo;
                            int cargaFt = 0;

                            if (ControlAct.Value == "AC")
                            {
                                cargaFt = 0;

                            }
                            if (ControlAct.Value == "PC")
                            {
                                ruta = txtPath.Text + nombreArchivo;
                                cargaFt = 0;
                                mensaje = SaveCanvasImage(urlPath2.Value, txtPath.Text, txtCarne.Text + ".jpg");
                            }
                            if (ControlAct.Value == "RC")
                            {
                                ruta = txtPath.Text + nombreArchivo;
                                cargaFt = 0;
                                mensaje = SaveCanvasImage(urlPath2.Value, txtPath.Text, txtCarne.Text + ".jpg");
                            }
                            if (mensaje.Equals("Imagen guardada correctamente.") || (ControlAct.Value == "AC" && mensaje.Equals("")))
                            {
                                cargaFt = 0;
                            }
                            else
                            {
                                cargaFt = 1;
                            }

                            if (cargaFt == 0)
                            {
                                int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'AC'");
                                int controlRenovacionPC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'PC'");

                                if (txtConfirmacion.Text == "01")
                                {
                                    if (ControlAct.Value == "AC" && (CONFIRMACION == "1000" || CONFIRMACION == "0"))
                                    {
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\FotosConfirmacion\\ACTUALIZACION-AC\\", txtCarne.Text + ".jpg");
                                    }
                                    if (ControlAct.Value == "PC" || (ControlAct.Value == "AC" && CONFIRMACION == "1"))
                                    {
                                        if (controlRenovacionPC == 0)
                                            SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\FotosConfirmacion\\PRIMER_CARNET-PC\\", txtCarne.Text + ".jpg");
                                        else
                                            SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosConfirmacion\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                    }
                                    if (ControlAct.Value == "RC")
                                    {
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\FotosConfirmacion\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                    }
                                }
                                else
                                {
                                    if (ControlAct.Value == "AC" && (CONFIRMACION == "1000" || CONFIRMACION == "0"))
                                    {
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\ACTUALIZACION-AC\\", txtCarne.Text + ".jpg");
                                    }
                                    if (ControlAct.Value == "PC" || (ControlAct.Value == "AC" && CONFIRMACION == "1"))
                                    {
                                        if (controlRenovacionPC <= 1 || controlRenovacionAC <= 1)
                                            SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\PRIMER_CARNET-PC\\", txtCarne.Text + ".jpg");
                                        else
                                            SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                    }
                                    if (ControlAct.Value == "RC")
                                    {
                                        SaveCanvasImage(urlPath2.Value, CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\RENOVACION_CARNE-RC\\", txtCarne.Text + ".jpg");
                                    }
                                }

                                cmd.Transaction = transaction;
                                txtExiste3.Text = txtPrimerApellido.Text + " insert";
                                if (String.IsNullOrEmpty(StateNIT.Text))
                                    StateNIT.Text = State.Text;

                                if (RadioButtonNombreSi.Checked && ControlRBS.Value == "1" && TrueNit.Value != txtNit.Text)
                                {
                                    TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                                    TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                                    TxtCasadaR.Text = txtApellidoCasada.Text;
                                    TxtDiRe1.Text = txtDireccion.Text;
                                    TxtDiRe2.Text = txtDireccion2.Text;
                                    TxtDiRe3.Text = txtZona.Text;
                                    txtNit.Text = "CF";
                                }

                                var direccion = txtDireccion.Text;
                                if (txtDireccion.Text.Length > 29)
                                    direccion = txtDireccion.Text.Substring(0, 29);
                                var estado = "";
                                if (CmbEstado.SelectedValue.Equals("Soltero"))
                                {
                                    estado = "1";
                                }
                                else if (CmbEstado.SelectedValue.Equals("Casado"))
                                {
                                    estado = "2";
                                }
                                else
                                {
                                    estado = "";
                                }

                                var tipoPersona = "";
                                if (CmbRoles.SelectedValue.Equals("E") || CmbRoles.SelectedValue.Equals("O") || CmbRoles.SelectedValue.Equals("N") || CmbRoles.SelectedValue.Equals("S"))
                                {
                                    tipoPersona = "1";
                                }
                                else if (CmbRoles.SelectedValue.Equals("C"))
                                {
                                    tipoPersona = "3";
                                }
                                else if (CmbRoles.SelectedValue.Equals("A"))
                                {
                                    tipoPersona = "2";
                                }

                                txtInsert.Text = "INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, " +
                                                "Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte, Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, " +
                                                "Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, " +
                                                "Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, Fec_Emision, NO_CTA_BI, ID_AGENCIA, " +
                                                "CONFIRMACION,TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_R, ADDRESS1, ADDRESS2, ADDRESS3, EMAIL_PERSONAL, EMPLID, CONTROL_ACCION, ROLES) VALUES (" +
                                                "'" + txtApellido1.Text + "'," + //APELLIDO1
                                                "'" + txtApellido2.Text + "'," + //APELLIDO2
                                                "NULL," + //CARNE
                                                "NULL," + //CEDULA
                                                "'" + txtApellidoCasada.Text + "'," +// APELLIDO DE CASADA
                                                "'" + CmbDepartamento.SelectedValue + "'," +// DEPARTAMENTO RESIDENCIA
                                                "'" + direccion + "'," +// DIRECCION
                                                "'" + TxtCorreoInstitucional.Text + "'," + // CORREO ELECTRONICO
                                                "" + estado + "," + // ESTADO CIVIL
                                                "NULL," + // FACULTAD
                                                "'" + FechaNac.Value + "'," + //FECHA DE NACIMIENTO
                                                "'" + FlagCedula.Value + "'," +
                                                "'" + FlagDpi.Value + "'," +
                                                "'" + FlagPasaporte.Value + "'," +
                                                "'" + CmbDepartamento.SelectedValue + "'," +//MUNICIPIO DE RESIDENCIA
                                                "'" + txtNit.Text + "'," +//NIT
                                                "'" + NoCui.Value + "'," +// NO_CUI
                                                "'" + Pasaporte.Value + "'," +// NUMERO DE PASAPORTE
                                                "'" + txtNombre1.Text + "'," + //NOMBRE1
                                                "'" + txtNombre2.Text + "'," +// NOMBRE 2
                                                "'" + txtNombre1.Text + ' ' + txtApellido1.Text + "'," + //NOMBRE DE IMPRESION
                                                "NULL," + // PAIS NACIONALIDAD
                                                "'OTROS'," + // PROFESION
                                                "'" + Sexo.Value + "'," + // SEXO
                                                "NULL," + //TELEFONO
                                                "NULL," + //ZONA
                                                "" + txtAccion.Text + "," + //ACCION
                                                "'" + txtTelefono.Text + "'," +// CELULAR
                                                "'" + txtdPI.Text + "'," + //CODIGO DE BARRAS
                                                "' " + ConMig.Value + "'," + //CONDICION MIGRANTE
                                                "2022," + //ID  UNIVERSIDAD
                                                "'" + PaisPass.Value + "'," + //PAIS PASAPORTE
                                                "'" + txtTipoAccion.Text + "'," +  //TIPO_ACCION
                                                "" + tipoPersona + "," + //TIPO PERSONA
                                                "'" + codPaisNIT + "'," + // PAIS NIT
                                                "'" + DeptoCui.Value + "'," + // DEPARTAMENTO CUI
                                                "'" + MuniCui.Value + "'," + //MUNICIPIO CUI
                                                "1," + //VALIDAR ENVIO
                                                "'" + ruta + "'," + //PATH
                                                "'" + txtCarne.Text + "'," + //CODIGO
                                                "'" + txtFacultad.Text + "'," + // DEPARTAMENTO
                                                "TO_CHAR(SYSDATE,'YYYY-MM-DD')," +//FECHA_HORA
                                                "TO_CHAR(SYSDATE,'YYYY-MM-DD')," +//FECHA_ENTREGA
                                                "TO_CHAR(SYSDATE,'YYYY-MM-DD')," +//FECHA_SOLICITADO
                                                "'" + TipoDoc.Value + "'," + //TIPO DOCUMENTO
                                                "'" + txtPuesto.Text + "'," + //CARGO
                                                "TO_CHAR(SYSDATE,'YYYY-MM-DD')," +//FECHA_EMISION
                                                " 0," + //NO CTA BI
                                                " 2002," +//ID AGENCIA
                                                txtConfirmacion.Text + "," +
                                                txtCantidadImagenesDpi.Text + "," +// confirmación operador
                                                "'" + TxtNombreR.Text + "'," + //NOMBRE
                                                "'" + TxtApellidoR.Text + "'," +
                                                "'" + TxtCasadaR.Text + "'," +
                                                "'" + TxtDiRe1.Text + "'," +
                                                "'" + TxtDiRe2.Text + "'," +
                                                "'" + TxtDiRe3.Text + "'," +
                                                "'" + StateNIT.Text + "'," +
                                                "'" + cMBpAIS.Text + "'," +
                                                "'" + txtDireccion.Text + "'," +
                                                "'" + txtDireccion2.Text + "'," +
                                                "NULL," +
                                                "'" + TxtCorreoPersonal.Text + "'," +
                                                "'" + UserEmplid.Text + "'," +
                                                "'" + ControlAct.Value+ "'," +
                                                "'" + ControlRoles.Value + "')"; //ROLES

                                txtInsertBit.Text = txtInsert.Text.Replace("TBL_HISTORIAL_CARNE", "TBL_BI_HISTORIAL_CARNE");
                                hPais.Value = homologaPais(cMBpAIS.SelectedValue);
                                codPaisNIT = homologaPais(CmbPaisNIT.SelectedValue);
                                if (String.IsNullOrEmpty(codPaisNIT))
                                    codPaisNIT = hPais.Value;

                                string consultaUP = "1";
                                string consultaUD = "1";
                                try
                                {
                                    if (containsProf.Value == "1" || Convert.ToInt16(Estudiante.Value) > 0)
                                    {
                                        //Numero de Telefono
                                        if (!String.IsNullOrEmpty(TruePhone.Text))
                                        { //UPDATE
                                            UD_PERSONAL_PHONE.Value = "<COLL_PERSONAL_PHONE> \n" +
                                                                        "                                            <EMPLID>" + txtCarne.Text + @"</EMPLID>" +
                                                                        "\n" +
                                                                        "                                            <KEYPROP_PHONE_TYPE>HOME</KEYPROP_PHONE_TYPE> \n" +
                                                                        "                                            <PROP_PHONE>" + txtTelefono.Text + @"</PROP_PHONE>" +
                                                                        "\n" +
                                                                        "                                            <PROP_PREF_PHONE_FLAG>Y</PROP_PREF_PHONE_FLAG> \n" +
                                                                     "                                         </COLL_PERSONAL_PHONE> \n";
                                            contadorUD = contadorUD + 1;
                                        }
                                        else
                                        {//INSERT
                                            UP_PERSONAL_PHONE.Value = "<COLL_PERSONAL_PHONE> \n" +
                                                                        "                                            <EMPLID>" + txtCarne.Text + @"</EMPLID>" +
                                                                        "\n" +
                                                                        "                                            <KEYPROP_PHONE_TYPE>HOME</KEYPROP_PHONE_TYPE> \n" +
                                                                        "                                            <PROP_PHONE>" + txtTelefono.Text + @"</PROP_PHONE>" +
                                                                        "\n" +
                                                                        "                                            <PROP_PREF_PHONE_FLAG>Y</PROP_PREF_PHONE_FLAG> \n" +
                                                                     "                                         </COLL_PERSONAL_PHONE> \n";
                                            contadorUP = contadorUP + 1;
                                        }

                                        //EMAIL PERSONAL
                                        if (!String.IsNullOrEmpty(TrueEmail.Text.TrimEnd()))
                                        {//UPDATE

                                            UD_EMAIL_ADDRESSES.Value = "<COLL_EMAIL_ADDRESSES>\n" +
                                                                            "                                            <KEYPROP_E_ADDR_TYPE>HOM1</KEYPROP_E_ADDR_TYPE> \n" +
                                                                            "                                            <PROP_EMAIL_ADDR>" + TxtCorreoPersonal.Text + @"</PROP_EMAIL_ADDR> " +
                                                                            "\n" +
                                                                            "                                            <PROP_PREF_EMAIL_FLAG>N</PROP_PREF_EMAIL_FLAG> \n" +
                                                                         "                                         </COLL_EMAIL_ADDRESSES> \n";
                                            contadorUD = contadorUD + 1;
                                        }
                                        else
                                        {//INSERT
                                            UP_EMAIL_ADDRESSES.Value = "<COLL_EMAIL_ADDRESSES>\n" +
                                                                            "                                            <KEYPROP_E_ADDR_TYPE>HOM1</KEYPROP_E_ADDR_TYPE> \n" +
                                                                            "                                            <PROP_EMAIL_ADDR>" + TxtCorreoPersonal.Text + @"</PROP_EMAIL_ADDR> " +
                                                                            "\n" +
                                                                            "                                            <PROP_PREF_EMAIL_FLAG>N</PROP_PREF_EMAIL_FLAG> \n" +
                                                                         "                                         </COLL_EMAIL_ADDRESSES> \n";
                                            contadorUP = contadorUP + 1;
                                        }

                                        //Direccion
                                        int ContadorDirecciones = 0;
                                        int ContadorEffdtDirecciones = 0;
                                        string EffdtDireccionUltimo = "";
                                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + txtCarne.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                                        reader = cmd.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            ContadorEffdtDirecciones = Convert.ToInt16(reader["CONTADOR"]);
                                        }

                                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + txtCarne.Text + "' " +
                                             " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                        reader = cmd.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            EffdtDireccionUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                                        }

                                        if (!String.IsNullOrEmpty(EffdtDireccionUltimo))
                                        {

                                            cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='HOME' AND EMPLID = '" + txtCarne.Text + "' " +
                                                "AND ADDRESS1 ='" + txtDireccion.Text + "' AND ADDRESS2 = '" + txtDireccion2.Text + "' AND ADDRESS3 = '" + txtZona.Text + "'" +
                                                "AND COUNTRY='" + hPais.Value + "' AND STATE ='" + State.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionUltimo).ToString("dd/MM/yyyy") + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                            reader = cmd.ExecuteReader();
                                            while (reader.Read())
                                            {
                                                ContadorDirecciones = Convert.ToInt16(reader["CONTADOR"]);
                                            }
                                        }
                                        else
                                        {
                                            ContadorDirecciones = 0;
                                        }

                                        cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_PERS_DATA_EFFDT WHERE EMPLID = '" + txtCarne.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                        reader = cmd.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(0, 10).TrimEnd();
                                            if (!String.IsNullOrEmpty(EFFDT_EC.Value))
                                            {
                                                if (EFFDT_EC.Value.Length == 9)
                                                {
                                                    EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(5, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(2, 2).TrimEnd() + "-0" + reader["EFFDT"].ToString().Substring(0, 1).TrimEnd();
                                                }
                                                else
                                                {
                                                    EFFDT_EC.Value = reader["EFFDT"].ToString().Substring(6, 4).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(3, 2).TrimEnd() + "-" + reader["EFFDT"].ToString().Substring(0, 2).TrimEnd();
                                                }
                                            }
                                        }

                                        llenadoState();
                                        if (txtNit.Text == "CF" && (InicialNR1.Value != TxtNombreR.Text || InicialNR2.Value != TxtApellidoR.Text || InicialNR3.Value != TxtCasadaR.Text || String.IsNullOrEmpty(InicialNR1.Value)))
                                        {
                                            StateNIT.Text = State.Text;
                                        }

                                        if (EffdtDireccionUltimo != Hoy && ContadorDirecciones == 0 && ContadorEffdtDirecciones == 0)
                                        {//INSERT
                                            UP_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                    "                                            <COLL_ADDRESSES> \n" +
                                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                                      "\n" +
                                                                      "                                                <PROP_COUNTRY>" + hPais.Value + @"</PROP_COUNTRY> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS3>" + txtZona.Text + @"</PROP_ADDRESS3> " +
                                                                      "\n" +
                                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                                      "\n" +
                                                                    "                                            </COLL_ADDRESSES> \n" +
                                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                            contadorUP = contadorUP + 1;
                                        }
                                        else if (EffdtDireccionUltimo == Hoy && ContadorDirecciones > 0 && ContadorEffdtDirecciones > 0)
                                        {
                                            //UPDATE
                                            UD_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                    "                                            <COLL_ADDRESSES> \n" +
                                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                                      "\n" +
                                                                      "                                                <PROP_COUNTRY>" + hPais.Value + @"</PROP_COUNTRY> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS3>" + txtZona.Text + @"</PROP_ADDRESS3> " +
                                                                      "\n" +
                                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                                      "\n" +
                                                                    "                                            </COLL_ADDRESSES> \n" +
                                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                            contadorUD = contadorUD + 1;
                                        }
                                        else
                                        {
                                            //UPDATE
                                            UD_ADDRESSES.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                                    "                                            <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                    "                                            <COLL_ADDRESSES> \n" +
                                                                      "                                                <KEYPROP_ADDRESS_TYPE>HOME</KEYPROP_ADDRESS_TYPE> \n" +
                                                                      "                                                <KEYPROP_EFFDT>" + EffdtDireccionUltimo + @"</KEYPROP_EFFDT> " +
                                                                      "\n" +
                                                                      "                                                <PROP_COUNTRY>" + hPais.Value + @"</PROP_COUNTRY> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS1>" + txtDireccion.Text + @"</PROP_ADDRESS1> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS2>" + txtDireccion2.Text + @"</PROP_ADDRESS2> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS3>" + txtZona.Text + @"</PROP_ADDRESS3> " +
                                                                      "\n" +
                                                                      "                                                <PROP_STATE>" + State.Text + @"</PROP_STATE>  " +
                                                                      "\n" +
                                                                    "                                            </COLL_ADDRESSES> \n" +
                                                                 "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                            contadorUD = contadorUD + 1;
                                        }

                                        //Estado Civil
                                        string ec = estadoCivil(CmbEstado.SelectedValue);
                                        if (TrueEstadoCivil.Value != ec)
                                        {
                                            if (EFFDT_EC.Value != Hoy)
                                            {
                                                UP_PERS_DATA_EFFDT.Value = "<COLL_PERS_DATA_EFFDT>\n" +
                                                            "                                            <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                            "\n" +
                                                            "                                             <PROP_MAR_STATUS>" + ec + @"</PROP_MAR_STATUS>" +
                                                            "\n" +
                                                             "                                            <PROP_HIGHEST_EDUC_LVL>A</PROP_HIGHEST_EDUC_LVL>" +
                                                            "\n" +
                                                             "                                            <PROP_FT_STUDENT>N</PROP_FT_STUDENT>" +
                                                            "\n" +
                                                            "                                            </COLL_PERS_DATA_EFFDT>";
                                                contadorUP = contadorUP + 1;
                                            }
                                            else
                                            {
                                                UD_PERS_DATA_EFFDT.Value = "<COLL_PERS_DATA_EFFDT>" +
                                                                    " <KEYPROP_EFFDT>" + EFFDT_EC.Value + @"</KEYPROP_EFFDT>" +
                                                                    " <PROP_MAR_STATUS>" + ec + @"</PROP_MAR_STATUS>" +
                                                                     "</COLL_PERS_DATA_EFFDT>";
                                                contadorUD = contadorUD + 1;
                                            }
                                        }
                                    }

                                    if (Convert.ToInt16(Estudiante.Value) > 0)
                                    {
                                        if (!String.IsNullOrEmpty(TxtNombreR.Text))
                                        {
                                            if (txtAInicial1.Value == txtApellido1.Text && txtNInicial1.Value == txtNombre1.Text && txtCInicial.Value == txtApellidoCasada.Text
                                                && txtAInicial2.Value == txtApellido2.Text && txtNInicial2.Value == txtNombre2.Text)
                                            {
                                                int ContadorNombreNit = 0;
                                                int ContadorEffdtNombreNit = 0;
                                                int ContadorEffdtNit = 0;
                                                int ContadorEffdtDirecionNit = 0;
                                                string EffdtDireccionNitUltimo = "";
                                                string EffdtNombreNitUltimo = "";
                                                string EffdtNitUltimo = "";
                                                int ContadorDirecionNit = 0;
                                                int ContadorNit = 0;
                                                int ContadorNit2 = 0;
                                                string EFFDT_SYSTEM = "";

                                                string ApellidoAnterior = "";
                                                string ApellidoCAnterior = "";


                                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND  EMPLID = '" + txtCarne.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    ContadorEffdtDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                                                }

                                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + txtCarne.Text + "' " +
                                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    EffdtDireccionNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                                                }

                                                if (!String.IsNullOrEmpty(EffdtDireccionNitUltimo))
                                                {
                                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE ADDRESS_TYPE ='REC' AND EMPLID = '" + txtCarne.Text + "' " +
                                                        "AND ADDRESS1 ='" + TxtDiRe1.Text + "' AND ADDRESS2 = '" + TxtDiRe2.Text + "' AND ADDRESS3 = '" + TxtDiRe3.Text + "' " +
                                                        "AND COUNTRY='" + codPaisNIT + "' AND STATE ='" + StateNIT.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtDireccionNitUltimo).ToString("dd/MM/yyyy") + "'" +
                                                        " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                                    reader = cmd.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                                                    }
                                                }
                                                else
                                                {
                                                    ContadorDirecionNit = 0;
                                                }

                                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_NAMES WHERE NAME_TYPE = 'REC' AND EMPLID = '" + txtCarne.Text + "' " +
                                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    EffdtNombreNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("yyyy-MM-dd")).ToString();
                                                }

                                                cmd.CommandText = "SELECT EFFDT AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' AND EMPLID = '" + txtCarne.Text + "' ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    EFFDT_SYSTEM = reader["CONTADOR"].ToString();
                                                }

                                                cmd.CommandText = "SELECT EFFDT FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + txtCarne.Text + "'" +
                                                    " ORDER BY 1 DESC FETCH FIRST 1 ROWS ONLY";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    EffdtNitUltimo = (Convert.ToDateTime(reader["EFFDT"]).ToString("dd-MM-yyyy")).ToString();
                                                }

                                                if (!String.IsNullOrEmpty(EffdtNitUltimo))
                                                {
                                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND  EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' AND EMPLID = '" + txtCarne.Text + "' AND EFFDT='" + EffdtNitUltimo + "'";
                                                    reader = cmd.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        ContadorNit = Convert.ToInt16(reader["CONTADOR"]);
                                                    }
                                                }
                                                else
                                                {
                                                    ContadorNit = 0;
                                                    EffdtNitUltimo = Hoy;
                                                }

                                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSKEY WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + txtCarne.Text + "'";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    ContadorNit2 = Convert.ToInt16(reader["CONTADOR"]);
                                                }

                                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                    "AND EFFDT ='" + HoyEffdt + "'";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    ContadorEffdtNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                                                }
                                                cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID = '" + txtCarne.Text + "' " +
                                                    "AND EFFDT ='" + HoyEffdt + "'";
                                                reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    ContadorEffdtNit = Convert.ToInt16(reader["CONTADOR"]);
                                                }

                                                if (!String.IsNullOrEmpty(EffdtNombreNitUltimo))
                                                {
                                                    cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES PN WHERE LAST_NAME ='" + TxtApellidoR.Text + "' " +
                                                        "AND FIRST_NAME='" + TxtNombreR.Text + "' AND SECOND_LAST_NAME='" + TxtCasadaR.Text + "' " +
                                                        "AND NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                    reader = cmd.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                                                    }

                                                    cmd.CommandText = "SELECT LAST_NAME , SECOND_LAST_NAME FROM SYSADM.PS_NAMES PN WHERE NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";

                                                    reader = cmd.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        ApellidoAnterior = reader["LAST_NAME"].ToString();
                                                        ApellidoCAnterior = reader["SECOND_LAST_NAME"].ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    ContadorNombreNit = 0;
                                                }

                                                string FechaEfectiva = "";
                                                if (EFFDT_NameR.Value.IsNullOrWhiteSpace())
                                                    FechaEfectiva = "1900-01-01";
                                                else
                                                    FechaEfectiva = EFFDT_NameR.Value;


                                                TxtApellidoR.Text.Replace(Environment.NewLine, string.Empty);
                                                TxtNombreR.Text.Replace(Environment.NewLine, string.Empty);
                                                TxtCasadaR.Text.Replace(Environment.NewLine, string.Empty);
                                                TxtApellidoR.Text = System.Text.RegularExpressions.Regex.Replace(TxtApellidoR.Text, @"\s+", " "); ;
                                                TxtNombreR.Text = System.Text.RegularExpressions.Regex.Replace(TxtNombreR.Text, @"\s+", " "); ;
                                                TxtCasadaR.Text = System.Text.RegularExpressions.Regex.Replace(TxtCasadaR.Text, @"\s+", " ");

                                                TxtApellidoR.Text = TxtApellidoR.Text.TrimEnd();
                                                TxtNombreR.Text = TxtNombreR.Text.TrimEnd();
                                                TxtCasadaR.Text = TxtCasadaR.Text.TrimEnd();

                                                if (EffdtNombreNitUltimo != Hoy && ContadorNombreNit == 0 && ContadorEffdtNombreNit == 0)
                                                {//INSERT
                                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                                    {
                                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                                        {
                                                            UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "        <COLL_NAMES>" +
                                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                            "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                                            "        </COLL_NAMES>" +
                                                                            "      </COLL_NAME_TYPE_VW>";
                                                        }
                                                        else
                                                        {
                                                            UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "        <COLL_NAMES>" +
                                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                            "        </COLL_NAMES>" +
                                                                            "      </COLL_NAME_TYPE_VW>";

                                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                            {
                                                                //ACTUALIZA NIT
                                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                                    " WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        UP_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                       "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                       "        <COLL_NAMES>" +
                                                                       "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                       "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                       "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                       "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                       "        </COLL_NAMES>" +
                                                                       "      </COLL_NAME_TYPE_VW>";
                                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }

                                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }
                                                    }
                                                    contadorUP = contadorUP + 1;
                                                }
                                                else if (EffdtNombreNitUltimo == Hoy && ContadorNombreNit > 0 && ContadorEffdtNombreNit > 0)
                                                {//UPDATE

                                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                                    {
                                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                                        {
                                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "        <COLL_NAMES>" +
                                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                            "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                                            "        </COLL_NAMES>" +
                                                                            "      </COLL_NAME_TYPE_VW>";
                                                        }
                                                        else
                                                        {
                                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "        <COLL_NAMES>" +
                                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                            "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                            "        </COLL_NAMES>" +
                                                                            "      </COLL_NAME_TYPE_VW>";

                                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                            {
                                                                //ACTUALIZA NIT
                                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                            "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "        <COLL_NAMES>" +
                                                                            "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                            "          <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT>" +
                                                                            "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                            "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                            "        </COLL_NAMES>" +
                                                                            "      </COLL_NAME_TYPE_VW>";
                                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }

                                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!TxtApellidoR.Text.IsNullOrWhiteSpace())
                                                    {
                                                        if (!TxtCasadaR.Text.IsNullOrWhiteSpace())
                                                        {

                                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                    "        <COLL_NAMES>" +
                                                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                    "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                                    "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                                    "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                                    "          <PROP_SECOND_LAST_NAME>" + TxtCasadaR.Text + @"</PROP_SECOND_LAST_NAME>" +
                                                                                    "        </COLL_NAMES>" +
                                                                                    "      </COLL_NAME_TYPE_VW>";
                                                        }
                                                        else
                                                        {

                                                            UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                                    "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                    "        <COLL_NAMES>" +
                                                                                    "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                    "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                                    "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                                    "          <PROP_LAST_NAME>" + TxtApellidoR.Text + @"</PROP_LAST_NAME>" +
                                                                                    "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                                    "          </COLL_NAMES>" +
                                                                                    "      </COLL_NAME_TYPE_VW>";
                                                            if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                            {
                                                                //ACTUALIZA NIT
                                                                txtUpdateAR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtApellidoR.Text + "," + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        UD_NAMES_NIT.Value = "<COLL_NAME_TYPE_VW> " +
                                                                                   "        <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                   "        <COLL_NAMES>" +
                                                                                   "          <KEYPROP_NAME_TYPE>REC</KEYPROP_NAME_TYPE>" +
                                                                                   "          <KEYPROP_EFFDT>" + EffdtNombreNitUltimo + @"</KEYPROP_EFFDT>" +
                                                                                   "          <PROP_COUNTRY_NM_FORMAT>MEX</PROP_COUNTRY_NM_FORMAT>" +
                                                                                   "          <PROP_FIRST_NAME>" + TxtNombreR.Text + @"</PROP_FIRST_NAME>" +
                                                                                   "          </COLL_NAMES>" +
                                                                                   "      </COLL_NAME_TYPE_VW>";
                                                        if (!ApellidoAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                    " WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }

                                                        if (!ApellidoCAnterior.IsNullOrWhiteSpace())
                                                        {
                                                            //ACTUALIZA NIT
                                                            txtUpdateNR.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.SECOND_LAST_NAME = ' ', PN.NAME ='" + TxtNombreR.Text + "', " +
                                                                    "PN.NAME_FORMAL ='" + TxtNombreR.Text + "', PN.NAME_DISPLAY ='" + TxtNombreR.Text + "' " +
                                                                    "WHERE PN.NAME_TYPE = 'REC' AND PN.EMPLID = '" + txtCarne.Text + "' " +
                                                                    "AND PN.EFFDT ='" + Convert.ToDateTime(EffdtNombreNitUltimo).ToString("dd/MM/yyyy") + "'";
                                                        }
                                                    }
                                                    contadorUD = contadorUD + 1;
                                                }

                                                if (EffdtNitUltimo == Hoy && ContadorNit == 0)
                                                {
                                                    //INSERTA EL NIT
                                                    cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) " +
                                                    "VALUES ('" + txtCarne.Text + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + txtNit.Text + "')";
                                                    cmd.ExecuteNonQuery();


                                                    if (ContadorNit2 == 0)
                                                    {
                                                        cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                                        "VALUES ('" + txtCarne.Text + "','NRE')";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }
                                                else if (EffdtNitUltimo != Hoy && ContadorNit > 0)
                                                {
                                                    //ACTUALIZA NIT
                                                    cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' " +
                                                                        " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + txtCarne.Text + "' AND EFFDT ='" + HoyEffdt + "'";
                                                    cmd.ExecuteNonQuery();

                                                }
                                                else
                                                {
                                                    //ACTUALIZA NIT
                                                    cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + txtNit.Text + "' " +
                                                                        " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + txtCarne.Text + "' AND EFFDT ='" + EffdtNitUltimo + "'";
                                                    cmd.ExecuteNonQuery();
                                                }

                                                if (String.IsNullOrEmpty(codPaisNIT))
                                                    codPaisNIT = "GTM";

                                                llenadoStateNIT();

                                                if (EffdtDireccionNitUltimo != Hoy && ContadorDirecionNit == 0 && ContadorEffdtDirecionNit == 0)
                                                {//INSERTA
                                                    UP_ADDRESSES_NIT.Value = "<COLL_ADDRESS_TYPE_VW>\n" +
                                                                    "                                            <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                                    "                                            <COLL_ADDRESSES> \n" +
                                                                      "                                                <KEYPROP_ADDRESS_TYPE>REC</KEYPROP_ADDRESS_TYPE> \n" +
                                                                      "                                                <KEYPROP_EFFDT>" + Hoy + @"</KEYPROP_EFFDT> " +
                                                                      "\n" +
                                                                      "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                                      "\n" +
                                                                      "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
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
                                                                      "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                                      "\n" +
                                                                      "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                                      "\n" +
                                                                      "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
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
                                                                          "                                                <PROP_COUNTRY>" + codPaisNIT + @"</PROP_COUNTRY> " +
                                                                          "\n" +
                                                                          "                                                <PROP_ADDRESS1>" + TxtDiRe1.Text + @"</PROP_ADDRESS1> " +
                                                                          "\n" +
                                                                          "                                                <PROP_ADDRESS2>" + TxtDiRe2.Text + @"</PROP_ADDRESS2> " +
                                                                          "\n" +
                                                                          "                                                <PROP_ADDRESS3>" + TxtDiRe3.Text + @"</PROP_ADDRESS3> " +
                                                                          "\n" +
                                                                          "                                                <PROP_STATE>" + StateNIT.Text + @"</PROP_STATE>  " +
                                                                          "\n" +
                                                                        "                                            </COLL_ADDRESSES> \n" +
                                                                     "                                        </COLL_ADDRESS_TYPE_VW> \n";
                                                    contadorUD = contadorUD + 1;
                                                }
                                            }
                                            else
                                            {
                                                llenadoPaisnit();
                                            }
                                        }
                                    }

                                    cmd.CommandText = "SELECT ID_REGISTRO AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                                    reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        RegistroCarne = reader["CONTADOR"].ToString();
                                    }

                                    controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                    controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "'");
                                    controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'AC'");
                                    if (controlRenovacion == 0)
                                    {
                                        //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                        if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + txtCarne.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + txtCarne.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + txtCarne.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }

                                    if ((txtAInicial1.Value != txtApellido1.Text || txtAInicial2.Value != txtApellido2.Text || txtNInicial1.Value != txtNombre1.Text || txtNInicial2.Value != txtNombre2.Text || txtCInicial.Value != txtApellidoCasada.Text))
                                    {
                                        cmd.Connection = con;
                                        cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + txtCarne.Text + "'";
                                        cmd.ExecuteNonQuery();
                                        string DeleteBanco = "DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CODIGO ='" + txtCarne.Text + "' OR CARNET '" + txtCarne.Text + "'";
                                        ConsumoSQL(DeleteBanco);

                                        //cmd.CommandText = txtInsert.Text;
                                        //cmd.ExecuteNonQuery();
                                        FileUpload2.Visible = false;
                                        CargaDPI.Visible = false;
                                        RegistroCarne = "1";
                                        matrizDatos();
                                        mensaje = "Su información fue almacenada correctamente. </br> La información ingresada debe ser aprobada antes de ser confirmada. </br> Actualmente, solo se muestran los datos que han sido previamente confirmados.";
                                        string script = "<script>ConfirmacionActualizacionSensible();</script>";
                                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                                    }

                                    if (RegistroCarne == "0")
                                    {
                                        cmd.CommandText = txtInsert.Text;
                                        cmd.ExecuteNonQuery();
                                    }

                                    auxConsulta = 0;
                                    if (contadorUP > 0)
                                    {
                                        consultaUP = ConsultarCampus();
                                    }
                                    auxConsulta = 1;
                                    if (contadorUD > 0)
                                    {
                                        consultaUD = ConsultarCampus();
                                    }
                                    limpiarVariables();
                                    int au = 0;
                                    if (consultaUD == "1" && consultaUP == "1")
                                    {

                                        if (!cMBpAIS.Text.Equals("-") && !CmbMunicipio.Text.Equals("-") && !CmbDepartamento.Text.Equals("-") && !String.IsNullOrEmpty(CmbEstado.Text))
                                        {
                                            //Obtener se obtiene toda la información del empleado
                                            string expand = "legislativeInfo,phones,addresses,photos,emails";
                                            string consulta = consultaGetworkers(expand, "nationalIdentifiers");
                                            aux = 5;
                                            string country = CodigoPais();

                                            if (urlPathControl2.Value == "1")
                                            {
                                                AlmacenarFotografia();
                                            }

                                            //Se obtienen los id's de las tablas a las cuales se les agregará información
                                            string personId = getBetween(consulta, "workers/", "/child/");
                                            string PhoneId = getBetween(consulta, "\"PhoneId\" : ", "\"" + TelefonoInicial.Value + "\",");
                                            string hoy = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
                                            string AYER = Convert.ToString(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                            if (PhoneId.Contains("PhoneId"))
                                            {
                                                PhoneId = getBetween(PhoneId, "\"PhoneId\" : ", "\"HF\",");
                                                PhoneId = getBetween(PhoneId, "", ",\n");
                                            }
                                            else
                                            {
                                                PhoneId = getBetween(PhoneId, "", ",\n");
                                            }
                                            string PersonLegislativeId = getBetween(consulta, "child/legislativeInfo/", "\",\n");
                                            string EmailId = getBetween(consulta, "\"EmailAddressId\" : ", "\"" + CorreoInicial.Value + "\",");
                                            if (EmailId.Contains("EmailAddressId") || EmailId.Contains("\"W1\""))
                                            {
                                                EmailId = getBetween(EmailId, "\"EmailAddressId\" : ", "\"H1\",");
                                                EmailId = getBetween(EmailId, "", ",\n");
                                            }
                                            else
                                            {
                                                EmailId = getBetween(EmailId, "", ",\n");
                                            }
                                            string pli = getBetween(consulta, "\"PersonLegislativeId\" : ", ",");
                                            string effectiveEC = getBetween(consulta, "\"PersonLegislativeId\" : " + pli + ",\n      \"EffectiveStartDate\" : \"", "\",\n");
                                            effectiveEC = effectiveEC.Replace("\"", "");
                                            string dff = getBetween(consulta, "\"AddressId\"", "\"PrimaryFlag\" : true");
                                            int largo = contadorSlash(dff.Length, dff);
                                            dff = dff.Replace('"', '\n');
                                            dff = Regex.Replace(dff, @" \n+", "\n");
                                            dff = Regex.Replace(dff, @"\n+", "");
                                            dff = Regex.Replace(dff, @",     ", "|");
                                            string[] result = dff.Split('|');
                                            int newDFF = contadorID(result.Length, result);
                                            string effectiveAdd = result[newDFF].Substring(20, result[newDFF].Length - 20);
                                            string comIm = personId + "/child/photo/";
                                            string consultaImagenes = consultaGetImagenes(comIm);
                                            string ImageId = getBetween(consultaImagenes, "\"ImageId\" : ", ",\n");
                                            string PhotoId = getBetween(consulta, "\"PhotoId\" : ", ",\n");

                                            string departamento = CmbDepartamento.Text;
                                            if (departamento.Equals("-"))
                                                departamento = "";
                                            //Se crea el body que se enviará a cada tabla
                                            var numeroEC = estadoCivil(CmbEstado.Text);
                                            if (numeroEC == "M")
                                            {
                                                numeroEC = "2";
                                            }
                                            else
                                            {
                                                numeroEC = "1";
                                            }

                                            var estadoC = "{\"MaritalStatus\": " + numeroEC + "}";
                                            var phoneNumber = "{\"PhoneNumber\": \"" + txtTelefono.Text + "\"}";
                                            var email = "{\"EmailAddress\": \"" + TxtCorreoPersonal.Text + "\"}";
                                            respuestaPatch = 0;
                                            respuestaPost = 0;

                                            string body = "";

                                            //TELEFONO
                                            if (String.IsNullOrEmpty(PhoneId))
                                            {
                                                //Actualiza por medio del metodo POST
                                                body = "{\"PhoneType\": \"HF\",\"PhoneNumber\": \"" + txtTelefono.Text + "\"}";
                                                createPost(personId, "phones", body, "workers/");
                                            }
                                            else
                                            {
                                                //Actualiza por medio del metodo PATCH
                                                updatePatch(phoneNumber, personId, "phones", PhoneId, "phones", "", "workers/");
                                            }

                                            //CORREO PERSONAL
                                            if (String.IsNullOrEmpty(EmailId))
                                            {
                                                //Actualiza por medio del metodo POST
                                                body = "{\"EmailType\": \"H1\",\"EmailAddress\": \"" + TxtCorreoPersonal.Text + "\"}";
                                                createPost(personId, "emails", body, "workers/");
                                            }
                                            else
                                            {
                                                //Actualiza por medio del metodo PATCH
                                                updatePatch(email, personId, "emails", EmailId, "emails", "", "workers/");
                                            }

                                            if (respuestaPatch != 0)
                                            {
                                                mensajeError = mensajeError + "Número de teléfono ";
                                                au = au + 1;
                                            }
                                            respuestaPatch = 0;
                                            respuestaPost = 0;

                                            //ESTADO CIVIL
                                            if (String.IsNullOrEmpty(PersonLegislativeId))
                                            {
                                                //Actualiza por medio del metodo POST
                                                body = "{\"LegislationCode\": \"GT\",\"MaritalStatus\": \"" + numeroEC + "\"}";
                                                createPost(personId, "legislativeInfo", body, "workers/");
                                            }
                                            else if (effectiveEC == hoy || numeroEC == EstadoCivilInicialNumero.Value)
                                            {
                                                //Actualiza por medio del metodo PATCH
                                                updatePatch(estadoC, personId, "legislativeInfo", PersonLegislativeId, "legislativeInfo", effectiveEC, "workers/");
                                            }
                                            else
                                            {
                                                //SE INGRESA UN NUEVO REGISTRO DEJANDO HISTORIAL DEL ESTADO CIVIL ANTERIOR ANTERIOR
                                                updatePatchEnd(estadoC, personId, "legislativeInfo", PersonLegislativeId, "legislativeInfo", hoy, "workers/", AYER);
                                            }

                                            if (respuestaPatch != 0 && mensajeError != "Ocurrió un problema al actualizar su: ")
                                            {
                                                mensajeError = mensajeError + "Estado civil ";
                                                au = au + 1;
                                            }
                                            else if (respuestaPatch != 0)
                                            {
                                                mensajeError = mensajeError + ", estado civil ";
                                                au = au + 1;
                                            }

                                            respuestaPatch = 0;
                                            respuestaPost = 0;

                                            //DIRECCION
                                            string primary = getBetween(consulta, "HOME\"", "\n        \"name\" ");
                                            string typeAdd = "HOME";
                                            string URLDelete = getBetween(consulta, "\"AddressId\"", "\"name\"");
                                            URLDelete = getBetween(URLDelete, "\"href\" : \"", "\",");

                                            if (String.IsNullOrEmpty(primary))
                                            {
                                                primary = getBetween(consulta, "HM\"", "\n        \"name\" ");
                                                typeAdd = "HM";
                                            }
                                            var Address = "{\"AddressLine1\": \"" + txtDireccion.Text + "\", \"AddressLine2\": \"" + txtDireccion2.Text + "\",\"AddressType\" :\"" + typeAdd + "\",\"Region1\": \"" + departamento + "\",\"TownOrCity\": \"" + CmbMunicipio.Text + "\",\"PrimaryFlag\": true,\"AddlAddressAttribute3\": \"" + txtZona.Text + "\",\"Country\": \"" + country + "\"}";

                                            string AddressId = getBetween(primary, "child/addresses", "\",");
                                            if ((PaisInicial.Text == Pais.Text && Departmento.Text == CmbDepartamento.SelectedValue && Municipio.Text == CmbMunicipio.SelectedValue
                                                && Direccion1.Text == txtDireccion.Text && Direccion2.Text == txtDireccion2.Text) || effectiveAdd == hoy)
                                            {
                                                Address = "{\"AddressLine1\": \"" + txtDireccion.Text + "\", \"AddressLine2\": \"" + txtDireccion2.Text + "\",\"Region1\": \"" + departamento + "\",\"TownOrCity\": \"" + CmbMunicipio.Text + "\",\"AddlAddressAttribute3\": \"" + txtZona.Text + "\"}";
                                                updatePatch(Address, personId, "addresses", AddressId, "addresses", effectiveAdd, "workers/");
                                                if (respuestaPatch != 0 && mensajeError.Contains("Ocurrió un problema al actualizar su: "))
                                                {
                                                    mensajeError = mensajeError + "Dirección ";
                                                    au = au + 1;
                                                }
                                                else if (respuestaPatch != 0)
                                                {
                                                    mensajeError = mensajeError + "y dirección ";
                                                    au = au + 1;
                                                }
                                            }
                                            else
                                            {
                                                //SE INGRESA UN NUEVO REGISTRO DEJANDO HISTORIAL DE LA DIRECCION ANTERIOR
                                                updatePatchEnd(Address, personId, "addresses", AddressId, "addresses", hoy, "workers/", AYER);
                                            }
                                        }
                                        else
                                        {
                                            lblActualizacion.Text = "Es necesario seleccionar: ";
                                            if (cMBpAIS.Text.Equals("-"))
                                                lblActualizacion.Text = lblActualizacion.Text + "Un país";
                                            if (CmbMunicipio.Text.Equals("-") && lblActualizacion.Text == "Es necesario seleccionar: ")
                                                lblActualizacion.Text = lblActualizacion.Text + "Un departamento";
                                            else if (CmbMunicipio.Text.Equals("-"))
                                                lblActualizacion.Text = lblActualizacion.Text + ", un departamento";
                                            if (CmbDepartamento.Text.Equals("-") && lblActualizacion.Text == "Es necesario seleccionar: ")
                                                lblActualizacion.Text = lblActualizacion.Text + "Un municipio";
                                            else if (CmbDepartamento.Text.Equals("-"))
                                                lblActualizacion.Text = lblActualizacion.Text + " y un municipio";
                                            if (String.IsNullOrEmpty(CmbEstado.Text) && lblActualizacion.Text == "Es necesario seleccionar: ")
                                                lblActualizacion.Text = lblActualizacion.Text + "Un estado civil";
                                            else if (String.IsNullOrEmpty(CmbEstado.Text))
                                                lblActualizacion.Text = lblActualizacion.Text + " y un estado civil";
                                        }

                                        if (au == 0)
                                        {
                                            transaction.Commit();
                                            con.Close();
                                            if (Request.Form["urlPathControl"] == "1")
                                            {
                                                AlmacenarFotografia();
                                            }
                                            fotoAlmacenada();
                                            if (ControlRBS.Value == "1" && TrueNit.Value != txtNit.Text)
                                            {
                                                PaisNit.Text = cMBpAIS.SelectedValue;
                                                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                                                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                                                TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                                                TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                                                TxtCasadaR.Text = txtApellidoCasada.Text;
                                                TxtDiRe1.Text = txtDireccion.Text;
                                                TxtDiRe2.Text = txtDireccion2.Text;
                                                TxtDiRe3.Text = txtZona.Text;
                                                txtNit.Text = "CF";
                                            }
                                            mensaje = "0";
                                            //log("La información fue actualizada de forma correcta");
                                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                            con.Close();
                                            EliminarAlmacenada();
                                            mensaje = "Error";
                                            log("ERROR - " + mensajeError + " en HCM");
                                            File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                        }
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        EliminarAlmacenada();
                                        mensaje = "Error";
                                        log("ERROR - Error en almacenamiento Campus: UD = " + consultaUD + "; UP = " + consultaUP + " SOAP: " + Variables.soapBody);
                                        File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                    }
                                }
                                catch (Exception X)
                                {
                                    transaction.Rollback();
                                    EliminarAlmacenada();
                                    mensaje = "Error";
                                    log("ERROR - Error en el ingreso de datos Empleado " + X.Message);
                                    log("ERROR - Error en almacenamiento Campus: UD = " + consultaUD + "; UP = " + consultaUP + " SOAP: " + Variables.soapBody);
                                    File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                                }
                            }
                            else
                            {
                                EliminarAlmacenada();
                                mensaje = "Error";
                                log("ERROR - No se almaceno la fotografía de manera correcta");
                                File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                            }
                        }
                    }
                }
                catch (Exception X)
                {
                    EliminarAlmacenada();
                    mensaje = "Error";
                    log("ERROR - Error en la funcion IngresoDatos: " + X.Message);
                    File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }

                if (urlPathControl2.Value == "1")
                {
                    AlmacenarFotografia();
                }
                fotoAlmacenada();
            }
            else
            {
                lblActualizacion.Text = "Es necesario tomar una fotografía.";
                mensaje = "Es necesario tomar una fotografía.";
            }
            return mensaje;
        }
        private string actualizarInformacion()
        {
            if (txtAInicial1.Value == "\r\n")
            {
                txtAInicial1.Value = null;
            }
            if (txtAInicial2.Value == "\r\n")
            {
                txtAInicial2.Value = null;
            }
            if (txtNInicial1.Value == "\r\n")
            {
                txtNInicial1.Value = null;
            }
            if (txtNInicial2.Value == "\r\n")
            {
                txtNInicial2.Value = null;
            }
            if (txtCInicial.Value == "\r\n")
            {
                txtCInicial.Value = null;
            }

            if (String.IsNullOrEmpty(txtNit.Text))
            {
                txtNit.Text = "CF";
            }
            string confirmacion = ValidarRegistros();
            int contador = 0;

            if (txtAInicial1.Value == txtApellido1.Text && txtAInicial2.Value == txtApellido2.Text && txtNInicial1.Value == txtNombre1.Text && txtNInicial2.Value == txtNombre2.Text && txtCInicial.Value == txtApellidoCasada.Text)
            {
                txtAccion.Text = "1";
                txtTipoAccion.Text = "1.1";
                txtConfirmacion.Text = "02"; //VALIDACIÓN DE FOTOGRAFÍA

                if (confirmacion != txtConfirmacion.Text && confirmacion != "0")
                {
                    string constr = TxtURL.Text;
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        con.Open();
                        OracleTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Transaction = transaction;
                            try
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + txtCarne.Text + "'  OR CARNET = '" + txtCarne.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception x)
                            {
                                log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                                File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                            }
                        }
                    }
                }

                if (Convert.ToInt16(Estudiante.Value) > 0)
                {

                    if (RadioButtonNombreNo.Checked)
                    {
                        if (!CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() && !CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                        {
                            mensaje = IngresoDatos();
                        }
                        else
                        {
                            mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                            lblActualizacion.Text = mensaje;
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        if (RadioButtonNombreSi.Checked && (InicialNR1.Value != txtNombre1.Text + " " + txtNombre2.Text || InicialNR2.Value != txtApellido1.Text + " " + txtApellido2.Text || InicialNR3.Value != TxtCasadaR.Text || String.IsNullOrEmpty(InicialNR1.Value) || ControlCF.Value != "CF"))
                        {
                            TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                            TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                            TxtCasadaR.Text = TxtCasadaR.Text;
                            TxtDiRe1.Text = txtDireccion.Text;
                            TxtDiRe2.Text = txtDireccion2.Text;
                            TxtDiRe3.Text = txtZona.Text;
                            txtNit.Text = "CF";
                        }
                        mensaje = IngresoDatos();
                    }
                    else if (RadioButtonNombreSi.Checked)
                    {
                        IngresoDatos();
                    }
                }
                else
                {
                    mensaje = IngresoDatos();
                }
            }
            else
            {
                if (FileUpload2.HasFile)
                {
                    int txtCantidad = 0;
                    string constr = TxtURL.Text;
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        con.Open();
                        OracleTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Transaction = transaction;
                            //Obtener codigo país
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT TOTALFOTOS FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + txtCarne.Text + "' OR CARNET = '"+ txtCarne.Text+ "'";
                            OracleDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                txtCantidad = Convert.ToInt16(reader["TOTALFOTOS"]);
                            }
                        }
                    }
                    for (int i = 1; i <= txtCantidad; i++)
                    {
                        File.Delete(CurrentDirectory + "/Usuarios/DPI/" + txtCarne.Text + "(" + i + ").jpg");
                    }
                    foreach (HttpPostedFile uploadedFile in FileUpload2.PostedFiles)
                    {
                        contador++;
                        string nombreArchivo = txtCarne.Text + "(" + contador + ").jpg";
                        string ruta = CurrentDirectory + "/Usuarios/DPI/" + nombreArchivo;
                        uploadedFile.SaveAs(ruta);
                    }
                    txtAccion.Text = "1";
                    txtTipoAccion.Text = "1.1";
                    txtConfirmacion.Text = "01"; //Requiere confirmación de operador 
                    txtCantidadImagenesDpi.Text = contador.ToString();

                    if (confirmacion != txtConfirmacion.Text && confirmacion != "0")
                    {
                        using (OracleConnection con = new OracleConnection(constr))
                        {
                            con.Open();
                            OracleTransaction transaction;
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            using (OracleCommand cmd = new OracleCommand())
                            {

                                cmd.Transaction = transaction;
                                try
                                {
                                    cmd.Connection = con;
                                    cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + txtCarne.Text + "'";
                                    cmd.ExecuteNonQuery();
                                    transaction.Commit();
                                    con.Close();
                                }
                                catch (Exception x)
                                {
                                    log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                                    File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                                }
                            }
                        }
                    }
                    //mensaje = IngresoDatos();
                    if (Convert.ToInt16(Estudiante.Value) > 0)
                    {
                        if (RadioButtonNombreSi.Checked && (InicialNR1.Value != txtNombre1.Text + " " + txtNombre2.Text || InicialNR2.Value != txtApellido1.Text + " " + txtApellido2.Text || InicialNR3.Value != TxtCasadaR.Text || String.IsNullOrEmpty(InicialNR1.Value) || ControlCF.Value != "CF"))
                        {
                            TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                            TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                            TxtCasadaR.Text = TxtCasadaR.Text;
                            TxtDiRe1.Text = txtDireccion.Text;
                            TxtDiRe2.Text = txtDireccion2.Text;
                            TxtDiRe3.Text = txtZona.Text;
                            txtNit.Text = "CF";
                            IngresoDatos();

                            using (OracleConnection con = new OracleConnection(constr))
                            {
                                con.Open();
                                OracleTransaction transaction;
                                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.Transaction = transaction;
                                    //Obtener codigo país
                                    cmd.Connection = con;
                                    controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                    controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "'");
                                    int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'AC'");
                                    try
                                    {
                                        if (controlRenovacion == 0)
                                        {
                                            //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                            if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                            {
                                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                            "VALUES ('" + UserEmplid.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                            {
                                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                            "VALUES ('" + UserEmplid.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "PC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();

                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "RC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            if (ControlAct.Value == "PC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();

                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                            else if (ControlAct.Value == "RC")
                                            {
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                    " WHERE EMPLID='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                    " WHERE CARNET='" + UserEmplid.Text + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        transaction.Commit();
                                    }
                                    catch (Exception)
                                    {
                                        transaction.Rollback();
                                    }
                                }
                            }
                        }
                        else
                        {
                            IngresoDatos();

                            using (OracleConnection con = new OracleConnection(constr))
                            {
                                con.Open();
                                OracleTransaction transaction;
                                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.Transaction = transaction;
                                    //Obtener codigo país
                                    cmd.Connection = con;
                                    controlRenovacionFecha = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "' AND FECH_ULTIMO_REGISTRO = '" + DateTime.Now.ToString("dd/MM/yyyy") + "'");
                                    controlRenovacion = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "'");
                                    int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'AC'");
                                    if (controlRenovacion == 0)
                                    {
                                        //INSERTA INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                                        if (ControlAct.Value == "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + UserEmplid.Text + "','0','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'AC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value != "AC" && controlRenovacionAC == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_CONTROL_CARNET (EMPLID, CONTADOR, FECH_ULTIMO_REGISTRO, ACCION) " +
                                        "VALUES ('" + UserEmplid.Text + "','1','" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'PC')";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        if (ControlAct.Value == "PC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '1', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='PC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();

                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='PC'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (ControlAct.Value == "RC")
                                        {
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_CONTROL_CARNET SET CONTADOR = '" + (controlRenovacion + 1) + "', FECH_ULTIMO_REGISTRO ='" + DateTime.Now.ToString("dd/MM/yyyy") + "', ACCION ='RC'" +
                                                                " WHERE EMPLID='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONTROL_ACCION ='RC', CONFIRMACION = '2'" +
                                                                " WHERE CARNET='" + UserEmplid.Text + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    string registroCarne = ValidarRegistros();
                                    if (registroCarne == "0")
                                    {
                                        cmd.CommandText = txtInsert.Text;
                                        cmd.ExecuteNonQuery();
                                    }
                                    transaction.Commit();
                                    con.Close();
                                }
                            }
                        }

                    }
                    else
                    {
                        mensaje = IngresoDatos();
                    }
                }
                else
                {
                    if (ControlRBS.Value == "1")
                    {
                        string script = "<script>Documentos();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        mensaje = "Es necesario adjuntar la imagen de su documento de identificación para continuar con la actualización.";

                        if (TrueNit.Value != txtNit.Text)
                        {
                            PaisNit.Text = cMBpAIS.SelectedValue;
                            DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                            MunicipioNit.Text = CmbMunicipio.SelectedValue;
                            TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                            TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                            TxtCasadaR.Text = txtApellidoCasada.Text;
                            TxtDiRe1.Text = txtDireccion.Text;
                            TxtDiRe2.Text = txtDireccion2.Text;
                            TxtDiRe3.Text = txtZona.Text;
                            txtNit.Text = "CF";
                        }
                    }
                    fotoAlmacenada();
                }
            }
            return mensaje;

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
        public string estadoCivil(string estadoCivilTexto)
        {
            var estado = "";
            if (estadoCivilTexto.Equals("Soltero"))
            {
                estado = "S";
            }
            else if (estadoCivilTexto.Equals("Casado"))
            {
                estado = "M";
            }
            else
            {
                estado = "S";
            }

            return estado;
        }
        public void GuardarBitacora(string ArchivoBitacora, string DescripcionBitacora)
        {
            //Guarda nueva línea para el registro de bitácora en el serividor
            File.AppendAllText(ArchivoBitacora, DescripcionBitacora + Environment.NewLine);
        }
        public void CrearArchivoBitacora(string archivoBitacora, string FechaHoraEjecución)
        {
            //Crea un archivo .txt para guardar bitácora
            StreamWriter sw = File.CreateText(archivoBitacora);
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
        protected int PantallaHabilitada(string PANTALLA)
        {
            txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE " +
                        "WHERE TO_CHAR(SYSDATE,'YYYY-MM-DD') " +
                        "BETWEEN FECHA_INICIO AND FECHA_FIN " +
                        "AND PANTALLA ='" + PANTALLA + "'";
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
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR " +
                        "FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE " +
                        "WHERE TO_CHAR(SYSDATE,'YYYY-MM-DD') " +
                        "BETWEEN FECHA_INICIO AND FECHA_FIN " +
                        "AND PANTALLA ='" + PANTALLA + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONTADOR"].ToString();
                        }

                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return Convert.ToInt32(control);
        }
        private string ValidarRegistros()
        {
            string constr = TxtURL.Text;

            string RegistroCarne = "0";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    //SE BUSCA EL ULTIMO REGISTRO DE CONFIRMACION
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CODIGO = '" + txtCarne.Text + "' OR CARNET = '" + txtCarne.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RegistroCarne = reader["CONFIRMACION"].ToString();
                    }
                }
            }
            return RegistroCarne;
        }
        private void AlmacenarFotografia()
        {
            EliminarRegistrosFotos();
            if (!urlPath2.Value.IsNullOrWhiteSpace())
            {
                int ExisteFoto;
                string constr = TxtURL.Text;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        //Obtener fotografia
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET = '" + txtCarne.Text + "' AND CONTROL ='1'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ExisteFoto = Convert.ToInt16(reader["CONTADOR"]);
                            try
                            {
                                cmd.Connection = con;
                                if (ExisteFoto > 0)
                                {
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE SET FOTOGRAFIA = 'Existe', CONTROL = '1'" +
                                                        "WHERE CARNET = '" + txtCarne.Text + "'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE (FOTOGRAFIA, CARNET, CONTROL) VALUES ('Existe', '" + txtCarne.Text + "', 1)";
                                    cmd.ExecuteNonQuery();
                                }
                                SaveCanvasImage(urlPath2.Value, CurrentDirectory + "/Usuarios/UltimasCargas/", txtCarne.Text + ".jpg");
                                transaction.Commit();
                            }
                            catch (Exception X)
                            {
                                transaction.Rollback();
                                fotoAlmacenada();
                            }
                        }
                    }
                }
            }
        }

        private void log(string ErrorLog)
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
                    cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_LOG_CARNE (CARNET, MESSAGE, PANTALLA, FECHA_REGISTRO) VALUES ('" + txtCarne.Text + "','" + ErrorLog + "','ACTUALIZACION EMPLEADOS',SYSDATE)";
                    cmd.ExecuteNonQuery();
                    if (txtControlBit.Text == "0" && !txtInsertBit.Text.IsNullOrWhiteSpace())
                    {
                        cmd.CommandText = txtInsertBit.Text;
                        cmd.ExecuteNonQuery();
                        txtControlBit.Text = "1";
                    }
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
        private void fotoAlmacenada()
        {
            string constr = TxtURL.Text;
            int contador;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNT(*) CONTADOR FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + txtCarne.Text + "' AND CONTROL = '1'";
                    OracleDataReader reader3 = cmd.ExecuteReader();
                    while (reader3.Read())
                    {
                        contador = Convert.ToInt32(reader3["CONTADOR"].ToString());
                        if (contador > 0)
                        {
                            ImgBase.Visible = true;
                            ImgBase.ImageUrl = (File.Exists(Server.MapPath($"~/Usuarios/UltimasCargas/{txtCarne.Text}.jpg"))) ? $"~/Usuarios/UltimasCargas/{txtCarne.Text}.jpg?c={DateTime.Now.Ticks}" : string.Empty;
                            byte[] imageBytes = File.ReadAllBytes(CurrentDirectory + "/Usuarios/UltimasCargas/" + txtCarne.Text + ".jpg");
                            string base64String = Convert.ToBase64String(imageBytes);
                            urlPath2.Value = base64String;
                            urlPathControl2.Value = "0";
                        }
                    }
                    con.Close();
                }
            }
        }
        private void EliminarAlmacenada()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE SET FOTOGRAFIA = 'Existe', CONTROL = '2'" +
                                                        "WHERE CARNET = '" + txtCarne.Text + "'";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void EliminarRegistrosFotos()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_FOTOGRAFIAS_CARNE WHERE CARNET ='" + txtCarne.Text + "'";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public string SaveCanvasImage(string imageData, string folderPath, string fileName)
        {
            if (!String.IsNullOrEmpty(imageData))
            {
                int largo = 0;
                largo = imageData.Length;
                if (urlPathControl2.Value == "1")
                    imageData = imageData.Substring(23, largo - 23);
                try
                {
                    // Ruta completa del archivo
                    string filePath = Path.Combine(folderPath, fileName);

                    // Guardar la imagen en el servidor
                    byte[] imageBytes = Convert.FromBase64String(Convert.ToString(imageData));
                    File.WriteAllBytes(filePath, imageBytes);
                    return "Imagen guardada correctamente.";
                }
                catch (Exception ex)
                {
                    return "Error al guardar la imagen: " + ex.Message;
                }
            }
            return "";
        }
        public void validarAccion()
        {
            int contadorRegistro = 0;
            int contadorConfirmacion = 0;
            contadorRegistro = ControlRenovacion("WHERE EMPLID  ='" + txtCarne.Text + "'");
            contadorConfirmacion = ControlRenovacionIntermedia("WHERE CARNET  ='" + txtCarne.Text + "'");
            int controlRenovacionAC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'AC'");
            int controlRenovacionPC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'PC'");
            int controlRenovacionRC = ControlAC("WHERE EMPLID  ='" + txtCarne.Text + "' AND ACCION = 'RC'");

            if (RadioButtonActualiza.Checked || ControlClicAct.Value == "A")
            {
                if (contadorRegistro == 0)
                {
                    // INFORMACIÓN PARA EL CONTROL DE LA RENOVACIÓN
                    if (controlRenovacionPC <= 1 && controlRenovacionRC == 0)
                    {
                        ControlAct.Value = "AC";
                    }
                    else if (controlRenovacionAC == 0 || controlRenovacionAC == 1)
                    {
                        ControlAct.Value = "PC";
                    }
                }
                else if (controlRenovacionRC >= 1)
                {
                    if (contadorConfirmacion == 0)
                    {
                        ControlAct.Value = "AC";
                    }
                    else
                    {
                        ControlAct.Value = "RC";
                    }
                }
                else if ((controlRenovacionPC <= 1 && contadorConfirmacion == 0) || (controlRenovacionAC <= 1 && contadorConfirmacion != 0 && controlRenovacionRC == 0))/*|| (controlRenovacionPC < 1 && contadorConfirmacion != 0)*/
                {
                    ControlAct.Value = "PC";
                }
                else
                {
                    ControlAct.Value = "AC";
                }

            }
            else if (RadioButtonCarne.Checked || ControlClicAct.Value == "C")
            {
                if (controlRenovacionRC >= 1 || (controlRenovacionPC >= 1 && contadorConfirmacion == 0))
                {
                    ControlAct.Value = "RC";
                }
                else
                {
                    ControlAct.Value = "PC";
                }
            }
        }
        protected int ControlRenovacionIntermedia(string cadena)
        {
            txtExiste4.Text = "SELECT CONFIRMACION " +
                        "FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE " + cadena;
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
                        cmd.CommandText = txtExiste4.Text;
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            control = reader["CONFIRMACION"].ToString();
                        }
                        con.Close();
                    }
                    catch (Exception x)
                    {
                        control = x.ToString();
                    }
                }
            }
            return Convert.ToInt32(control);
        }

        //EVENTOS    
        protected void cMBpAIS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();

            aux = 1;
            listaDepartamentos();
            aux = 2;
            listadoMunicipios();
            aux = 3;
            listadoZonas();
            llenadoState();

            if ((ControlRBS.Value == "1" && TrueNit.Value != txtNit.Text && RadioButtonNombreSi.Checked))// || ControlCF.Value != "CF")
            {
                PaisNit.Text = cMBpAIS.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                TxtCasadaR.Text = txtApellidoCasada.Text;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtZona.Text;
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
            }
            
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();

            aux = 2;
            listadoMunicipios();
            aux = 3;
            listadoZonas();
            llenadoState();
            if ((ControlRBS.Value == "1" && TrueNit.Value != txtNit.Text && RadioButtonNombreSi.Checked))// || ControlCF.Value != "CF")
            {
                PaisNit.Text = cMBpAIS.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                TxtCasadaR.Text = txtApellidoCasada.Text;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtZona.Text;
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();

            aux = 3;
            listadoZonas();
            llenadoState();
            if ((ControlRBS.Value == "1" && TrueNit.Value != txtNit.Text && RadioButtonNombreSi.Checked))// || ControlCF.Value != "CF")
            {
                PaisNit.Text = cMBpAIS.SelectedValue;
                DepartamentoNit.Text = CmbDepartamento.SelectedValue;
                MunicipioNit.Text = CmbMunicipio.SelectedValue;
                TxtNombreR.Text = txtNombre1.Text + " " + txtNombre2.Text;
                TxtApellidoR.Text = txtApellido1.Text + " " + txtApellido2.Text;
                TxtCasadaR.Text = txtApellidoCasada.Text;
                TxtDiRe1.Text = txtDireccion.Text;
                TxtDiRe2.Text = txtDireccion2.Text;
                TxtDiRe3.Text = txtZona.Text;
                txtNit.Text = "CF";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
                ValidarNIT.Enabled = false;
                txtNit.Enabled = false;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbPaisNit_SelectedIndexChanged(object sender, EventArgs e)
        {
            aux = 1;
            listaDepartamentosNit();
            if (!String.IsNullOrWhiteSpace(CmbDepartamentoNIT.Text) || CmbDepartamentoNIT.Text != "")
            {
                aux = 2;
                listadoMunicipiosNit();
            }
            else
            {
                string[] resultado = new string[1];
                resultado[0] = "-";
                CmbMunicipioNIT.DataSource = resultado;
                CmbMunicipioNIT.DataTextField = "";
                CmbMunicipioNIT.DataValueField = "";
                CmbMunicipioNIT.DataBind();
                CmbDepartamentoNIT.DataSource = resultado;
                CmbDepartamentoNIT.DataTextField = "";
                CmbDepartamentoNIT.DataValueField = "";
                CmbDepartamentoNIT.DataBind();
            }

            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }

            fotoAlmacenada();

            PaisNit.Text = cMBpAIS.SelectedValue;
            DepartamentoNit.Text = CmbDepartamento.SelectedValue;
            MunicipioNit.Text = CmbMunicipio.SelectedValue;
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbDepartamentoNit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();

            aux = 2;
            listadoMunicipiosNit();
            llenadoStateNIT();
            DepartamentoNit.Text = CmbDepartamento.SelectedValue;
            MunicipioNit.Text = CmbMunicipio.SelectedValue;
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbMunicipioNit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }

            aux = 3;
            listadoZonas();
            llenadoStateNIT();
            fotoAlmacenada();
            MunicipioNit.Text = CmbMunicipio.SelectedValue;
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            string constr = TxtURL.Text;
            string control = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {

                    cmd.Transaction = transaction;
                    //Obtener codigo país
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT CONTROL_ACCION, CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "' OR CODIGO = '" + txtCarne.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        control = reader["CONTROL_ACCION"].ToString();
                        CONFIRMACION = reader["CONFIRMACION"].ToString();
                    }
                    validarAccion();

                    if (control != ControlAct.Value && CONFIRMACION != "0")
                    {
                        cmd.Transaction = transaction;
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "' OR CODIGO = '" + txtCarne.Text + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                        }
                        catch (Exception x)
                        {
                            log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                            File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                        }
                    }
                }

                if ((control == "AC" && CONFIRMACION != "0") || (control == null && CONFIRMACION == "1000"))
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                }
                else if (control == "PC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                }
                else if (control == "RC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                }
                else if (ControlAct.Value == "PC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                }
                else if (ControlAct.Value == "RC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                }
            }

            string informacion = actualizarInformacion();
            if (informacion != "" && informacion != "Error")
            {
                if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(cMBpAIS.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                {
                    if (RadioButtonNombreNo.Checked)
                    {
                        if ((CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() || CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() || CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace()) && Convert.ToInt16(Estudiante.Value) > 0)
                        {
                            if (urlPathControl2.Value == "1")
                            {
                                AlmacenarFotografia();
                            }

                            fotoAlmacenada();
                            mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                            lblActualizacion.Text = mensaje;
                            // Al finalizar la actualización, ocultar el modal
                            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                        }
                        else
                        {
                            // Llama a la función JavaScript para mostrar el modal
                            EliminarAlmacenada();
                            EnvioCorreo();
                            EnvioCorreoEmpleado();
                            log("La información de fue actualizada de forma correcta");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                        }
                    }

                    if (RadioButtonNombreSi.Checked)
                    {
                        EliminarAlmacenada();
                        EnvioCorreo();
                        EnvioCorreoEmpleado();
                        log("La información de fue actualizada de forma correcta");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                    }
                }
                else
                {
                    EliminarAlmacenada();
                    log("ERROR - Es necesario seleccionar un País, departamento y municipio para el recibo");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
                }
            }

            /*try
            {
                File.Delete(CurrentDirectory + "\\Usuarios\\FotosColaboradores\\FotosConfirmacion\\" + txtCarne.Text + ".jpg");
            }
            catch
            {

            }*/
        }
        protected void CmbRoles_TextChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            if (CmbRoles.SelectedValue != "A")
            {
                listadoDependencia();
                LblPuesto.Text = "Puesto:";
                lblDependencia.Text = "Facultad o Dependencia:";
            }
            else
            {
                txtFacultad.Text = Facultad.Value;
                txtPuesto.Text = Carrera.Value;
                LblPuesto.Text = "Carrera:";
                lblDependencia.Text = "Facultad:";
            }
            fotoAlmacenada();
        }
        public void changeCombobox()
        {
            validarAccion();
            if (ControlAct.Value == "AC")
                RadioButtonActualiza.Checked = true;
            else if (ControlAct.Value == "PC" || ControlAct.Value == "RC")
                RadioButtonCarne.Checked = true;
        }
        protected void txtNit_TextChanged(object sender, EventArgs e)
        {
            string respuesta;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            respuesta = consultaNit(txtNit.Text);
            string constr = TxtURL.Text;
            if (ControlAct.Value == "AC")
                RadioButtonActualiza.Checked = true;
            else
                RadioButtonCarne.Checked = true;

            if (respuesta.Equals("BadRequest") || respuesta.Equals("1"))
            {
                TxtNombreR.Text = null;
                TxtApellidoR.Text = null;
                TxtCasadaR.Text = null;
                llenadoPaisnit();
                CmbPaisNIT.SelectedValue = " ";
                llenadoDepartamentoNit();
                CmbDepartamentoNIT.SelectedValue = " ";
                llenadoMunicipioNIT();
                CmbMunicipioNIT.Text = " ";
                CmbMunicipioNIT.Enabled = false;
                CmbDepartamentoNIT.Enabled = false;
                CmbPaisNIT.Enabled = false;
                txtNit.Enabled = true;
                ValidarNIT.Enabled = true;
                int ExisteNitValidacion = 0;

                //ALMACENAMIENTO DE INFORMACIÓN DE NIT PARA VALIDACION POSTERIOR
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Transaction = transaction;
                        //Obtener fotografia
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST WHERE EMPLID = '" + UserEmplid.Text + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ExisteNitValidacion = Convert.ToInt16(reader["CONTADOR"]);
                        }
                        cmd.Transaction = transaction;
                        try
                        {
                            if (ExisteNitValidacion == 0)
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST (NIT, EMPLID) VALUES ('" + txtNit.Text + "', '" + UserEmplid.Text + "')";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            else
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_NIT_PENDIENTE_ST SET NIT = '" + txtNit.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                lblActualizacion.Text = "El NIT sera validado más adelante";
                TxtDiRe1.Text = " ";
                TxtDiRe1.Enabled = false;
                TxtDiRe2.Enabled = false;
                TxtDiRe3.Enabled = false;
            }
            else if (respuesta != "1" && respuesta != "BadRequest")
            {
                string NIT = getBetween(respuesta, "\"NIT\": \"", "\",");
                string Direccion = getBetween(respuesta, "\"Direccion\": \"", "\",");
                string apellido1;
                string apellido2;
                string apellidoCasada;
                string nombre1;
                string nombre2;
                int largo;
                if (NIT != "CF")
                {
                    string nombreRespuesta = getBetween(respuesta, "\"NOMBRE\": \"", "\",") + ",";
                    string cadena = nombreRespuesta;
                    TxtDiRe1.Text = Direccion;
                    TxtDiRe2.Text = "";
                    TxtDiRe3.Text = "";
                    TxtDiRe1.Enabled = true;
                    TxtDiRe2.Enabled = true;
                    TxtDiRe3.Enabled = true;
                    txtNit.Enabled = true;
                    llenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    llenadoDepartamentoNit();
                    CmbDepartamentoNIT.SelectedValue = " ";
                    llenadoMunicipioNIT();
                    int contadorComas = cadena.Count(c => c == ',');
                    if (contadorComas >= 5)
                    {
                        apellido1 = getBetween(nombreRespuesta, "", ",");
                        apellido2 = getBetween(nombreRespuesta, apellido1 + ",", ",");
                        apellidoCasada = getBetween(nombreRespuesta, apellido2 + ",", ",");
                        nombre1 = getBetween(nombreRespuesta, apellido1 + "," + apellido2 + "," + apellidoCasada + ",", ",");
                        nombre2 = getBetween(nombreRespuesta, nombre1 + ",", ",");
                        TxtNombreR.Text = textInfo.ToTitleCase(nombre1 + " " + nombre2);
                        TxtApellidoR.Text = apellido1 + " " + apellido2;
                        TxtCasadaR.Text = apellidoCasada;
                    }
                    else
                    {
                        nombreRespuesta = nombreRespuesta.TrimEnd(',');
                        largo = nombreRespuesta.Length;

                        if (largo < 31)
                        {
                            TxtNombreR.Text = nombreRespuesta;
                        }
                        else if (largo > 30 && largo < 61)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, largo - 30);
                        }
                        else if (largo > 30 && largo < 91)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, 30);
                            TxtCasadaR.Text = nombreRespuesta.Substring(60, largo - 60);
                        }
                        else if (largo > 90)
                        {
                            TxtNombreR.Text = nombreRespuesta.Substring(0, 30);
                            TxtApellidoR.Text = nombreRespuesta.Substring(30, 30);
                            TxtCasadaR.Text = nombreRespuesta.Substring(60, 30);
                        }
                    }

                    lblActualizacion.Text = "";
                    if (urlPathControl2.Value == "1")
                    {
                        AlmacenarFotografia();
                    }


                    fotoAlmacenada();
                    ValidacionNit.Value = "0";
                    ValidarNIT.Enabled = true;
                }
                else
                {
                    TxtDiRe1.Enabled = true;
                    TxtDiRe2.Enabled = true;
                    TxtDiRe3.Enabled = true;
                    string nit = txtNit.Text;
                    txtNit.Text = nit;
                    TxtNombreR.Text = "";
                    TxtApellidoR.Text = "";
                    TxtCasadaR.Text = "";
                    TxtDiRe1.Text = "";
                    TxtDiRe2.Text = "";
                    TxtDiRe2.Text = "";
                    ValidarNIT.Enabled = true;
                    txtNit.Enabled = true;
                    llenadoPaisnit();
                    CmbPaisNIT.SelectedValue = " ";
                    llenadoDepartamentoNit();
                    CmbDepartamentoNIT.SelectedValue = " ";
                    llenadoMunicipioNIT();
                    string script = "<script>NoExisteNit();</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                }
            }
            TrueNit.Value = txtNit.Text;
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
        }
        protected void CmbPaisNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbDepartamentoNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void CmbMunicipioNIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (urlPathControl2.Value == "1")
            {
                AlmacenarFotografia();
            }
            fotoAlmacenada();
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalEspera();", true);
            changeCombobox();
        }
        protected void BtnAceptarCarga_Click(object sender, EventArgs e)
        {
            string constr = TxtURL.Text;
            string control = null;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {

                    cmd.Transaction = transaction;
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT CONTROL_ACCION, CONFIRMACION FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        control = reader["CONTROL_ACCION"].ToString();
                        CONFIRMACION = reader["CONFIRMACION"].ToString();
                    }
                    validarAccion();

                    if (control != ControlAct.Value && CONFIRMACION != "0")
                    {
                        cmd.Transaction = transaction;
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + txtCarne.Text + "' OR CODIGO = '"+ txtCarne.Text+ "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                        }
                        catch (Exception x)
                        {
                            log("ERROR - Error en la funcion actualizarInformacion: " + x.Message);
                            File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                        }
                    }
                }

                if ((control == "AC" && CONFIRMACION != "0") || (control == null && CONFIRMACION == "1000"))
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                }
                else if (control == "PC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                }
                else if (control == "RC" && ControlAct.Value != "AC" && CONFIRMACION != "0")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                }
                else if (ControlAct.Value == "PC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\RENOVACION_CARNE-RC\\" + txtCarne.Text + ".jpg");
                }
                else if (ControlAct.Value == "RC")
                {
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\PRIMER_CARNET-PC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\Fotos\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                    File.Delete(CurrentDirectory + "\\Usuarios\\UltimasCargas\\ACTUALIZACION-AC\\" + txtCarne.Text + ".jpg");
                }
            }

            string informacion = actualizarInformacion();
            if (informacion != "" && informacion != "Error")
            {
                if (Convert.ToInt16(Estudiante.Value) > 0)
                {
                    if (!String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(cMBpAIS.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                    {
                        if (RadioButtonNombreNo.Checked)
                        {
                            if (CmbPaisNIT.SelectedValue.IsNullOrWhiteSpace() || CmbDepartamentoNIT.SelectedValue.IsNullOrWhiteSpace() || CmbMunicipioNIT.SelectedValue.IsNullOrWhiteSpace())
                            {
                                if (urlPathControl2.Value == "1")
                                {
                                    AlmacenarFotografia();
                                }
                                fotoAlmacenada();
                                mensaje = "Es necesario seleccionar un País, departamento y municipio para el recibo.";
                                lblActualizacion.Text = mensaje;
                                // Al finalizar la actualización, ocultar el modal
                                ScriptManager.RegisterStartupScript(this, GetType(), "OcultarModal", "ocultarModalActualizacion();", true);
                            }
                            else
                            {
                                EliminarAlmacenada();
                                EnvioCorreo();
                                EnvioCorreoEmpleado();
                                using (OracleConnection con = new OracleConnection(constr))
                                {
                                    con.Open();
                                    OracleTransaction transaction;
                                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                    using (OracleCommand cmd = new OracleCommand())
                                    {
                                        cmd.Transaction = transaction;
                                        cmd.Connection = con;
                                        cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION ='1'" + " WHERE CARNET='" + txtCarne.Text + "' OR CODIGO = '" + txtCarne.Text + "'";
                                        cmd.ExecuteNonQuery();
                                        transaction.Commit();
                                    }
                                }
                                log("La información de fue actualizada de forma correcta");
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalSensible", "ConfirmacionActualizacionSensible();", true);
                            }
                        }

                        if (RadioButtonNombreSi.Checked)
                        {
                            EliminarAlmacenada();
                            EnvioCorreo();
                            EnvioCorreoEmpleado();

                            using (OracleConnection con = new OracleConnection(constr))
                            {
                                con.Open();
                                OracleTransaction transaction;
                                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Connection = con;
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_HISTORIAL_CARNE SET CONFIRMACION ='1'" + " WHERE CARNET='" + txtCarne.Text + "' OR CODIGO = '" + txtCarne.Text + "'";
                                    cmd.ExecuteNonQuery();
                                    transaction.Commit();
                                }
                            }

                            log("La información de fue actualizada de forma correcta");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalSensible", "ConfirmacionActualizacionSensible();", true);

                        }
                    }
                }
                else
                {
                    EliminarAlmacenada();
                    EnvioCorreo();
                    EnvioCorreoEmpleado();
                    log("La información de fue actualizada de forma correcta");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", "mostrarModalCorrecto();", true);
                }
            }
            else
            {
                EliminarAlmacenada();
                log("ERROR - Error en la funcion actualizarInformacion en AceptarCarga " + informacion);
                File.Delete(txtPath.Text + txtCarne.Text + ".jpg");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", "mostrarModalError();", true);
            }                        
        }
        protected void BtnDownload_Click(object sender, EventArgs e)
        {
            string archivoDescarga = CurrentDirectory + "/Manuales/ManualActivacionCamara.pdf";
            string nombreArchivo = "ManualActivacionCamara.pdf";
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nombreArchivo + "\"");
            Response.WriteFile(archivoDescarga);
            Response.End();
        }
        protected void BtnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/ActualizaciónEmpleados.aspx");
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
                CuerpoConsultaPorMunicipio(Variables.wsUsuario, Variables.wsPassword, CmbDepartamento.SelectedValue, cMBpAIS.SelectedValue);
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
            else if (aux == 6)
            {
                CuerpoConsultaRol(Variables.wsUsuario, Variables.wsPassword, TextUser.Text);
            }
            else if (aux == 7)
            {
                CuerpoConsultaPuestoDep(Variables.wsUsuario, Variables.wsPassword, TextUser.Text, CmbRoles.SelectedValue);
            }

            //Crea un documento de respuesta Campus
            System.Xml.XmlDocument xmlDocumentoRespuestaCampus = new System.Xml.XmlDocument();

            // Indica que no se mantengan los espacios y saltos de línea
            xmlDocumentoRespuestaCampus.PreserveWhitespace = false;

            try
            {
                // Carga el XML de respuesta de Campus
                
                xmlDocumentoRespuestaCampus.LoadXml(LlamarWebService(Variables.wsUrl, Variables.wsAction, Variables.soapBody));
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
        public string ConsultarCampus()
        {
            //Se limpian variables para guardar la nueva información
            limpiarVariables();

            //Obtiene información del servicio (URL y credenciales)
            credencialesEndPoint(archivoConfiguracionesCampus, "Consultar");

            if (auxConsulta == 0)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UP.V1";
                CuerpoConsultaUP(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UP_NAMES_NIT.Value, UP_PERS_DATA_EFFDT.Value, UP_ADDRESSES_NIT.Value, UP_ADDRESSES.Value, UP_PERSONAL_PHONE.Value, UP_EMAIL_ADDRESSES.Value, VersionUP.Value);
            }
            else if (auxConsulta == 1)
            {
                Variables.wsAction = "CI_CI_PERSONAL_DATA_UD.V1";
                CuerpoConsultaUD(Variables.wsUsuario, Variables.wsPassword, txtCarne.Text, UD_NAMES_NIT.Value, UD_PERS_DATA_EFFDT.Value, UD_ADDRESSES_NIT.Value, UD_ADDRESSES.Value, UD_PERSONAL_PHONE.Value, UD_EMAIL_ADDRESSES.Value, VersionUD.Value);
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
            XmlNodeList elemList = xmlDocumentoRespuestaCampus.GetElementsByTagName("notification");
            //return elemList[0].InnerText.ToString();
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
        private static void CuerpoConsultaPorDPI(string idPersona, string passwordServicio, string dpi)
        {
            //Crea el cuerpo que se utiliza para consultar los empleados por DPI
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
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Actualización/InformeActualizarEmpleadosV2.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaCodigoPais(string idPersona, string passwordServicio, string pais)
        {
            //Crea el cuerpo que se utiliza para consultar el codigo del pais
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
        private static void CuerpoConsultaPorMunicipio(string idPersona, string passwordServicio, string departamento, string pais)
        {
            //Crea el cuerpo que se utiliza para consultar los municipios
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
                                            <v2:item>
                                                <v2:name>PAIS</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + pais + @"</v2:item>
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
        private static void CuerpoConsultaPorPais(string idPersona, string passwordServicio)
        {
            //Crea el cuerpo que se utiliza para consultar las zonas
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
        private static void CuerpoConsultaPorZona(string idPersona, string passwordServicio, string municipio)
        {
            //Crea el cuerpo que se utiliza para consultar las zonas
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
        private static void CuerpoConsultaDepartamento(string idPersona, string passwordServicio, string pais)
        {
            //Crea el cuerpo que se utiliza para consultar los departamentos
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
        private static void CuerpoConsultaRol(string idPersona, string passwordServicio, string dpi)
        {
            //Crea el cuerpo que se utiliza para consultar los roles del empleado por DPI
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
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Actualización/RolUsuario.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaPuestoDep(string idPersona, string passwordServicio, string dpi, string codigo)
        {
            //Crea el cuerpo que se utiliza para consultar el puesto y dependencia del empleado
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
                                            <v2:item>
                                                <v2:name>CODIGO</v2:name>
                                                        <v2:values>
                                                            <v2:item>" + codigo + @"</v2:item>
                                                        </v2:values>
                                                </v2:item>
                                           </v2:listOfParamNameValues>
                                        </v2:parameterNameValues>           
                                        <v2:reportAbsolutePath>/Custom/UNIS/ Web Services/Actualización/DependenciaPuesto.xdo</v2:reportAbsolutePath>
                                       <v2:sizeOfDataChunkDownload>-1</v2:sizeOfDataChunkDownload>
                                     </v2:reportRequest>
                                     <v2:userID>" + idPersona + @"</v2:userID>
                                     <v2:password>" + passwordServicio + @"</v2:password>
                                  </v2:runReport>
                               </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUD(string Usuario, string Pass, string EMPLID, string COLL_NAMES, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE,
            string COLL_EMAIL_ADDRESSES, string VersionUD)
        {
            //Crea el cuerpo que se utiliza para hacer PATCH en CAMPUS
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
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                      </Updatedata__CompIntfc__CI_PERSONAL_DATA>
                                   </soapenv:Body>
                                </soapenv:Envelope>";
        }
        private static void CuerpoConsultaUP(string Usuario, string Pass, string EMPLID, string COLL_NAMES, string COLL_PERS_DATA_EFFDT, string COLL_ADDRESSES_NIT, string COLL_ADDRESSES, string COLL_PERSONAL_PHONE,
            string COLL_EMAIL_ADDRESSES, string VersionUP)
        {
            //Crea el cuerpo que se utiliza para hacer POST en CAMPUS
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
                                         " + COLL_PERS_DATA_EFFDT + @"
                                         " + COLL_NAMES + @"
                                         " + COLL_PERSONAL_PHONE + @"
                                         " + COLL_EMAIL_ADDRESSES + @"
                                         " + COLL_ADDRESSES + @"
                                         " + COLL_ADDRESSES_NIT + @"
                                      </Update__CompIntfc__CI_PERSONAL_DATA>
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
        private static void credencialesWS(string RutaConfiguracion, string strMetodo)
        {
            //Función para obtener información de acceso al servicio de HCM
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
        public string LlamarWebService(string _url, string _action, string _xmlString)
        {
            //Función para llamar un  servicio web 
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
        public string LlamarWebServiceCampus(string _url, string _action, string _xmlString)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(_xmlString);
            HttpWebRequest webRequest = CreateWebRequestCampus(_url, _action);
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
            webRequest.Headers.Clear();
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private static HttpWebRequest CreateWebRequestCampus(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Clear();
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
        public string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        //CONSUMO DE API
        int respuestaPatch = 0;
        int respuestaPost = 0;
        private string consultaGetworkers(string expand, string expandUser)
        {
            credencialesWS(archivoWS, "Consultar");
            string consulta = consultaUser(expandUser, UserEmplid.Text);
            int cantidad = consulta.IndexOf(Context.User.Identity.Name.Replace("@unis.edu.gt", ""));
            if (cantidad >= 0)
                consulta = consulta.Substring(0, cantidad);
            string consulta2 = consulta.Replace("\n    \"", "|");
            string[] result = consulta2.Split('|');
            string personID = UserEmplid.Text;
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
            string consulta = consultaUser("nationalIdentifiers", UserEmplid.Text);
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
        private void updatePatchEnd(string info, string personId, string tables, string ID, string consulta, string effective, string esquema, string end)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.PatchEnd(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/" + esquema + personId + "/child/" + tables + "/" + ID, user, pass, info, consulta, effective, end);
            respuestaPatch = respuesta + respuestaPatch;
        }
        private void delete(string url, string consulta, string effective)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Delete(url, user, pass, consulta, effective);
            respuestaPatch = respuesta + respuestaPatch;
        }
        private void createPost(string personId, string tables, string datos, string EXTEN)
        {
            credencialesWS(archivoWS, "Consultar");
            var vchrUrlWS = Variables.wsUrl;
            var user = Variables.wsUsuario;
            var pass = Variables.wsPassword;
            int respuesta = api.Post(vchrUrlWS + "/hcmRestApi/resources/11.13.18.05/" + EXTEN + personId + "/child/" + tables, datos, user, pass);
            respuestaPost = respuestaPost + respuesta;
        }

        protected void txtNit_TextChanged1(object sender, EventArgs e)
        {
            ChangeNIT.Value = "1";
        }
    }
}