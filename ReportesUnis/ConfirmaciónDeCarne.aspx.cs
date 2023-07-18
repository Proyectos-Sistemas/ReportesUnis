using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Ajax.Utilities;
using NPOI.SS.Formula.Functions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Resources;
using static System.Windows.Forms.AxHost;
using Windows.Devices.Sensors;
using Windows.UI.Xaml.Automation.Text;

namespace ReportesUnis
{
    public partial class ConfirmaciónDeCarne : System.Web.UI.Page
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
            }
        }

        protected void RadioButtonConfirmar_CheckedChanged(object sender, EventArgs e)
        {
            LimpiarCampos();
            divConfirmar.Visible = true;
            divGenerar.Visible = false;
            divCampos.Visible = true;
            divDPI.Visible = true;
            divFotografia.Visible = true;
            divBtnConfirmar.Visible = true;
            divBtnGenerar.Visible = false;
            Buscar("1");
            lblActualizacion.Text = null;
        }

        protected void RadioButtonGenerar_CheckedChanged(object sender, EventArgs e)
        {
            LimpiarCampos();
            divConfirmar.Visible = false;
            divGenerar.Visible = true;
            divCampos.Visible = true;
            divDPI.Visible = false;
            divFotografia.Visible = false;
            divBtnConfirmar.Visible = false;
            divBtnGenerar.Visible = true;
            txtCarne.Text = null;
            lblActualizacion.Text = null;
        }

        protected void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenado("CARNET = '" + CmbCarne.Text + "'");
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

        private void Buscar(string confirmacion)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET FROM DUAL UNION SELECT CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE TIPO_PERSONA = 2 AND CONFIRMACION = '" + confirmacion + "'";
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    CmbCarne.DataSource = ds;
                    CmbCarne.DataTextField = "CARNET";
                    CmbCarne.DataValueField = "CARNET";
                    CmbCarne.DataBind();
                    con.Close();
                }
            }
        }

        //Lectura de archivo txt para la conexion
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
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS, " +
                        "' ' NOMBRE_NIT,' ' APELLIDOS_NIT,' ' CASADA_NIT,' ' DIRECCION1_NIT,' ' DIRECCION2_NIT,' ' DIRECCION3_NIT, ' ' STATE_NIT , ' ' PAIS_NIT, ' ' PAIS_R, ' ' NO_PASAPORTE FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS, NOMBRE_NIT, APELLIDOS_NIT, CASADA_NIT, DIRECCION1_NIT, " +
                        "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, PAIS_R, NO_PASAPORTE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND TIPO_PERSONA = 2";
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
                        TxtCarrera.Text = reader["CARGO"].ToString();
                        TxtFacultad.Text = reader["FACULTAD"].ToString();
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
                    }
                    con.Close();
                }
            }
        }

        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            lblActualizacion.Text = null;
            if (!txtCarne.Text.IsNullOrWhiteSpace())
            {
                llenado("CARNET = '" + txtCarne.Text + "' AND CONFIRMACION = '0'");
                if (TxtPrimerNombre.Text.IsNullOrWhiteSpace())
                {
                    lblActualizacion.Text = "No se encontró información confirmada para el número de Carne " + txtCarne.Text;
                }
            }
            else
            {
                txtCarne.Text = null;
                lblActualizacion.Text = "Debe de ingresar un número de carnet para poder realizar la generación.";

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
            TxtCarrera.Text = null;
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
        }

        private void Rechazar(string Carnet)
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                lblActualizacion.Text = "";
                string constr = TxtURL.Text;
                int ID = 30000;
                using (OracleConnection con = new OracleConnection(constr))
                {
                    con.Open();
                    OracleTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + Carnet + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                            Buscar("1");
                            File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + Carnet + ".jpg");
                            for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                            {
                                File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                            }
                            lblActualizacion.Text = "Se ha rechazado la solicitud de carnet.";
                        }
                        catch (Exception)
                        {
                            lblActualizacion.Text = "No se pudo eliminar la información a causa de un error interno." + "  DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + CmbCarne.Text + "'";
                            transaction.Rollback();
                        }

                    }
                }
                LimpiarCampos();
            }
            else
            {
                lblActualizacion.Text = "Debe de ingresar un número de carnet para poder rechazar la información.";
            }
        }

        protected void BtnRechazar_Click(object sender, EventArgs e)
        {
            Rechazar(CmbCarne.Text);
        }

        protected void Confirmar(string Carnet)
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi();
                respuesta = QueryActualizaNombre(Carnet);

                if (respuesta == "0")
                {
                    //SE INGRESA LA INFORMACIÓN DEL NIT
                    respuesta = ActualizarNIT(CmbCarne.Text);
                    if (respuesta == "0")
                    {
                        respuesta = ConsumoOracle(txtInsertName.Text);

                        if (respuesta == "0")
                        {
                            respuesta = "";
                            QueryUpdateApex("0", fecha, fecha, fecha, "1", Carnet);
                            if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                            {
                                //SE INGRESA LA INFORMACIÓN EN EL BANCO
                                respuesta = ConsumoSQL(txtInsertBI.Text);
                                if (respuesta == "0")
                                    respuesta = ConsumoOracle(txtInsertApex.Text);
                                {
                                }
                            }
                        }
                    }

                    if (respuesta == "0")
                    {
                        lblActualizacion.Text = "Se confirmó correctamente la información";
                        Buscar("1");
                        File.Delete(CurrentDirectory + "/Usuarios/FotosConfirmacion/" + Carnet + ".jpg");
                        for (int i = 1; i <= Convert.ToInt16(txtCantidad.Text); i++)
                        {
                            File.Delete(CurrentDirectory + "/Usuarios/DPI/" + Carnet + "(" + i + ").jpg");
                        }
                        LimpiarCampos();
                    }
                    else
                    {
                        lblActualizacion.Text = "Ocurrió un problema al confirmar la información";
                        ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
                    }
                }
                else
                {
                    lblActualizacion.Text = "Ocurrió un problema al confirmar la información";
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + Carnet + "'");
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
                    //txtInsertBI.Text = "SELECT 'INSERT INTO[dbo].[Tarjeta_Identificacion_prueba] " +
                    cmd.CommandText = "SELECT 'INSERT INTO[dbo].[Tarjeta_Identificacion_prueba] " +
                                   "([Carnet] " +
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
                                    "||PAIS_NACIONALIDAD||''','''" + //PAIS_NACIONALIDAD
                                    "||PAIS_PASAPORTE||''','''" + //PAIS_PASAPORTE
                                    "||NO_PASAPORTE||''','''" + //NO_PASAPORTE
                                    "||PROFESION||''','''" + //PROFESION
                                    "||CASA||''','''" + //CASA
                                    "||APTO||''','''" + //APARTAMENTO
                                    "||CELULAR||''','''" + //CELULAR
                                    "||EMAIL||''','''" + //CELULAR
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
                                    "FROM ( SELECT * FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + CmbCarne.Text + "')";
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtInsertBI.Text = reader["INS"].ToString();
                    }
                }
            }
        }

        protected string QueryActualizaBi()
        {
            string consulta = null;
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            consulta = "UPDATE [dbo].[Tarjeta_Identificacion_prueba] SET " +
                "[Fecha_Solicitado] = '" + fecha + "' , " +
                "[Fecha_Entrega] = '" + fecha + "', " +
                "[Accion] = '2', " +
                "[Fecha_Hora] = '" + fecha + "', " +
                "[Fec_Emision] = '" + fecha + "', " +
                "[Validar_Envio] = '1'  " +
                "WHERE CARNET ='" + txtCarne.Text + "'";
            return consulta;
        }

        protected string QueryActualizaNombre(string emplid)
        {
            string constr = TxtURL.Text;
            string vchrApellidosCompletos = (TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text).TrimEnd();
            string vchrFNameNS1 = " ";
            string vchrLNameNS1 = " ";
            string vchrCNameNS1 = " ";
            string TxtNombre = (TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text).TrimEnd();
            string TxtApellidos = (TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text).TrimEnd();
            string TxtCasada = TxtApellidoCasada.Text;

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
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND  NAME_TYPE != 'REC' AND EMPLID = '" + emplid + "'";
                        OracleDataReader reader1 = cmd.ExecuteReader();
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorNombre = Convert.ToInt16(reader1["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'HOME' AND EMPLID = '" + emplid + "'";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            ContadorDirecion = Convert.ToInt16(reader1["CONTADOR"]);
                        }


                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + vchrApellidosCompletos + "') AS CADENA FROM DUAL";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            vchrLNameNS1 = reader1["CADENA"].ToString().TrimStart().TrimEnd(); ;
                        }

                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + TxtNombre + "') AS CADENA FROM DUAL";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            vchrFNameNS1 = reader1["CADENA"].ToString().TrimStart().TrimEnd(); ;
                        }

                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + TxtCasada + "') AS CADENA FROM DUAL";
                        reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            vchrCNameNS1 = reader1["CADENA"].ToString();
                        }

                        if (ContadorNombre == 0)
                        {
                            cmd.CommandText = "INSERT INTO SYSADM.PS_NAMES ( " +
                                                "EMPLID, " +
                                                "NAME_TYPE, " +
                                                "EFFDT, " +
                                                "EFF_STATUS, " +
                                                "COUNTRY_NM_FORMAT, " +
                                                "NAME, " +
                                                "NAME_INITIALS, " +
                                                "NAME_PREFIX, " +
                                                "NAME_SUFFIX, " +
                                                "NAME_ROYAL_PREFIX, " +
                                                "NAME_ROYAL_SUFFIX, " +
                                                "NAME_TITLE, " +
                                                "LAST_NAME_SRCH, " +
                                                "FIRST_NAME_SRCH, " +
                                                "LAST_NAME, " +
                                                "FIRST_NAME, " +
                                                "MIDDLE_NAME, " +
                                                "SECOND_LAST_NAME, " +
                                                "SECOND_LAST_SRCH, " +
                                                "NAME_AC, " +
                                                "PREF_FIRST_NAME, " +
                                                "PARTNER_LAST_NAME, " +
                                                "PARTNER_ROY_PREFIX, " +
                                                "LAST_NAME_PREF_NLD, " +
                                                "NAME_DISPLAY, " +
                                                "NAME_FORMAL, " +
                                                "NAME_DISPLAY_SRCH, " +
                                                "LASTUPDDTTM, " +
                                                "LASTUPDOPRID " +
                                                ") VALUES(" +
                                                    "'" + emplid + "'," + // EMPLID
                                                    "'PRF','" + // NAME_TYPE
                                                    DateTime.Now.ToString("dd/MM/yyyy") + "'," + // EFFDT
                                                    "'A'," + // EFF_STATUS
                                                    "'MEX'," + // COUNTRY_NM_FORMAT
                                                    "'" + vchrApellidosCompletos + "," + TxtNombre + "'," + // NAME
                                                    "' '," + // NAME_INITIALS
                                                    "' '," + // NAME_PREFIX
                                                    "' '," + // NAME_SUFFIX
                                                    "' '," + // NAME_ROYAL_PREFIX
                                                    "' '," + // NAME_ROYAL_SUFFIX
                                                    "' '," + // NAME_TITLE
                                                    "'" + vchrLNameNS1 + "'," + // LAST_NAME_SRCH
                                                    "'" + vchrFNameNS1 + "'," +// FIRST_NAME_SRCH
                                                    "'" + TxtApellidos + "'," + // LAST_NAME
                                                    "'" + TxtNombre + "'," + // FIRST_NAME
                                                    "' '," + // MIDDLE_NAME
                                                    "'" + TxtCasada + "'," + // SECOND_LAST_NAME
                                                    "'" + vchrCNameNS1 + " '," + // SECOND_LAST_SRCH
                                                    "' '," + // NAME_AC
                                                    "' '," + // PREF_FIRST_NAME
                                                    "' '," + // PARTNER_LAST_NAME
                                                    "' '," + // PARTNER_ROY_PREFIX
                                                    "'1'," + // LAST_NAME_PREF_NLD
                                                    "'" + TxtNombre + " " + vchrApellidosCompletos + "'," + // NAME_DISPLAY
                                                    "'" + TxtNombre + " " + vchrApellidosCompletos + "'," + // NAME_FORMAL
                                                    "'" + (vchrFNameNS1 + vchrLNameNS1 + vchrCNameNS1).TrimEnd() + "'," + // NAME_DISPLAY_SRCH
                                                    "SYSDATE," + // LASTUPDDTTM
                                                    "'" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";  // LASTUPDOPRID                                                                    
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO SYSADM.PS_NAMES ( " +
                                                "EMPLID, " +
                                                "NAME_TYPE, " +
                                                "EFFDT, " +
                                                "EFF_STATUS, " +
                                                "COUNTRY_NM_FORMAT, " +
                                                "NAME, " +
                                                "NAME_INITIALS, " +
                                                "NAME_PREFIX, " +
                                                "NAME_SUFFIX, " +
                                                "NAME_ROYAL_PREFIX, " +
                                                "NAME_ROYAL_SUFFIX, " +
                                                "NAME_TITLE, " +
                                                "LAST_NAME_SRCH, " +
                                                "FIRST_NAME_SRCH, " +
                                                "LAST_NAME, " +
                                                "FIRST_NAME, " +
                                                "MIDDLE_NAME, " +
                                                "SECOND_LAST_NAME, " +
                                                "SECOND_LAST_SRCH, " +
                                                "NAME_AC, " +
                                                "PREF_FIRST_NAME, " +
                                                "PARTNER_LAST_NAME, " +
                                                "PARTNER_ROY_PREFIX, " +
                                                "LAST_NAME_PREF_NLD, " +
                                                "NAME_DISPLAY, " +
                                                "NAME_FORMAL, " +
                                                "NAME_DISPLAY_SRCH, " +
                                                "LASTUPDDTTM, " +
                                                "LASTUPDOPRID " +
                                                ") VALUES(" +
                                                    "'" + emplid + "'," + // EMPLID
                                                    "'PRI','" + // NAME_TYPE
                                                    DateTime.Now.ToString("dd/MM/yyyy") + "'," + // EFFDT
                                                    "'A'," + // EFF_STATUS
                                                    "'MEX'," + // COUNTRY_NM_FORMAT
                                                    "'" + vchrApellidosCompletos + "," + TxtNombre + "'," + // NAME
                                                    "' '," + // NAME_INITIALS
                                                    "' '," + // NAME_PREFIX
                                                    "' '," + // NAME_SUFFIX
                                                    "' '," + // NAME_ROYAL_PREFIX
                                                    "' '," + // NAME_ROYAL_SUFFIX
                                                    "' '," + // NAME_TITLE
                                                    "'" + vchrLNameNS1 + "'," + // LAST_NAME_SRCH
                                                    "'" + vchrFNameNS1 + "'," +// FIRST_NAME_SRCH
                                                    "'" + TxtApellidos + "'," + // LAST_NAME
                                                    "'" + TxtNombre + "'," + // FIRST_NAME
                                                    "' '," + // MIDDLE_NAME
                                                    "'" + TxtCasada + "'," + // SECOND_LAST_NAME
                                                    "'" + vchrCNameNS1 + " '," + // SECOND_LAST_SRCH
                                                    "' '," + // NAME_AC
                                                    "' '," + // PREF_FIRST_NAME
                                                    "' '," + // PARTNER_LAST_NAME
                                                    "' '," + // PARTNER_ROY_PREFIX
                                                    "'1'," + // LAST_NAME_PREF_NLD
                                                    "'" + TxtNombre + " " + vchrApellidosCompletos + "'," + // NAME_DISPLAY
                                                    "'" + TxtNombre + " " + vchrApellidosCompletos + "'," + // NAME_FORMAL
                                                    "'" + (vchrFNameNS1 + vchrLNameNS1 + vchrCNameNS1).TrimEnd() + "'," + // NAME_DISPLAY_SRCH
                                                    "SYSDATE," + // LASTUPDDTTM
                                                    "'" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";  // LASTUPDOPRID                                                                    
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA NOMBRE
                            cmd.CommandText = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = '" + vchrApellidosCompletos + "," + TxtNombre + "'," +
                                            "PN.LAST_NAME_SRCH ='" + vchrLNameNS1 + "', " +
                                            "PN.FIRST_NAME_SRCH='" + vchrFNameNS1 + "', " +
                                            "LAST_NAME ='" + TxtApellidos + "', " +
                                            "FIRST_NAME='" + TxtNombre + "', " +
                                            "SECOND_LAST_NAME='" + TxtCasada + "', SECOND_LAST_SRCH=REPLACE(UPPER('" + TxtCasada + "'),' ','')||' ', " +
                                            "NAME_DISPLAY='" + TxtNombre + " " + vchrApellidosCompletos + "', " +
                                            "NAME_FORMAL='" + TxtNombre + " " + vchrApellidosCompletos + "', " +
                                            "NAME_DISPLAY_SRCH ='" + (vchrFNameNS1 + vchrLNameNS1 + vchrCNameNS1).TrimEnd() + "'," +
                                            "LASTUPDDTTM = SYSDATE, " +
                                            "LASTUPDOPRID = '" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "' " +
                                            "WHERE PN.EMPLID = '" + emplid + "' AND  NAME_TYPE IN ('PRI','PRF')";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_DATA " +
                                                                   "        SET NAME					='" + vchrApellidosCompletos + "," + TxtNombre + "'," +
                                                                   "            LAST_NAME_SRCH			='" + vchrLNameNS1 + "'," +
                                                                   "            FIRST_NAME_SRCH			='" + vchrFNameNS1 + "'," +
                                                                   "            LAST_NAME				='" + TxtApellidos + "'," +
                                                                   "            FIRST_NAME				='" + TxtNombre + "'," +
                                                                   "            SECOND_LAST_NAME		='" + TxtCasada + "'," +
                                                                   "            SECOND_LAST_SRCH		='" + vchrCNameNS1 + " '," +
                                                                   "            NAME_DISPLAY			='" + TxtNombre + " " + vchrApellidosCompletos + "'," +
                                                                   "            NAME_FORMAL				='" + TxtNombre + " " + vchrApellidosCompletos + "'," +
                                                                   "            LASTUPDDTTM				=SYSDATE" +
                                                                   "        WHERE EMPLID='" + emplid + "'";

                            cmd.ExecuteNonQuery();
                        }


                        transaction.Commit();
                        con.Close();
                        return "0";

                    }
                    catch (Exception x)
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
                " WHERE CARNET = '" + Carne + "'";
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

        protected void BtnConfirmar_Click(object sender, EventArgs e)
        {
            string carne = CmbCarne.Text;
            Confirmar(carne);
        }

        protected void BtnGenerar_Click(object sender, EventArgs e)
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                string consultaBi = QueryActualizaBi();
                txtExiste.Text = consultaBi;
                //SE INGRESA LA INFORMACIÓN EN EL BANCO
                respuesta = ConsumoSQL(consultaBi);
                if (respuesta == "0")
                {
                    respuesta = "";
                    QueryUpdateApex("0", fecha, fecha, fecha, "2", txtCarne.Text);
                    if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                    {
                        respuesta = ConsumoOracle(txtInsertApex.Text);
                    }
                }

                if (respuesta == "0")
                {
                    lblActualizacion.Text = "Se almacenó correctamente la información para la renovación del carné";
                    LimpiarCampos();
                }
                else
                {
                    lblActualizacion.Text = "Ocurrió un problema al almacenar la información";
                }
            }
            else
            {
                lblActualizacion.Text = "No se encontró información confirmada para el número de Carne " + txtCarne.Text;
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
                                        "WHERE PN.NATIONAL_ID ='" + TxtDpi.Text + "' " + //---1581737080101
                                                                                         //"WHERE PN.NATIONAL_ID ='3682754340101' " + // de la cerda  
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
                            "DIRECCION2_NIT, DIRECCION3_NIT, STATE_NIT, PAIS_NIT, NIT FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + emplid + "' AND TIPO_PERSONA = 2";
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

                        if (TxtDiRe2.IsNullOrWhiteSpace())
                            TxtDiRe2 = " ";
                        if (TxtDiRe3.IsNullOrWhiteSpace())
                            TxtDiRe3 = " ";

                        int ContadorNombreNit = 0;
                        int ContadorDirecionNit = 0;
                        int ContadorNit = 0;
                        int ContadorNit2 = 0;
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_NAMES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND  NAME_TYPE = 'REC' AND EMPLID = '" + emplid + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorNombreNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_ADDRESSES WHERE EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND ADDRESS_TYPE = 'REC' AND EMPLID = '" + emplid + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ContadorDirecionNit = Convert.ToInt16(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM SYSADM.PS_EXTERNAL_SYSTEM WHERE EXTERNAL_SYSTEM = 'NRE' AND EFFDT LIKE (TO_CHAR(SYSDATE,'dd/MM/yy')) AND EMPLID = '" + emplid + "'";
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
                        string vchrApellidosCompletos = (TxtApellidoR + " " + TxtCasadaR).TrimEnd();
                        string vchrFNameNS = " ";
                        string vchrLNameNS = " ";
                        string vchrCNameNS = " ";

                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + vchrApellidosCompletos + "') AS CADENA FROM DUAL";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            vchrLNameNS = reader["CADENA"].ToString().TrimStart().TrimEnd(); ;
                        }

                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + TxtNombreR + "') AS CADENA FROM DUAL";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            vchrFNameNS = reader["CADENA"].ToString().TrimStart().TrimEnd(); ;
                        }

                        cmd.CommandText = "SELECT UNIS_INTERFACES.FNT_GET_SEARCH_NAME('" + TxtCasadaR + "') AS CADENA FROM DUAL";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            vchrCNameNS = reader["CADENA"].ToString().TrimStart().TrimEnd();
                        }

                        if (!String.IsNullOrEmpty(existeNit))
                        {
                            //ACTUALIZA NOMBRE DEL NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME ='" + vchrApellidosCompletos + "," + TxtNombreR + "'," +
                                                "PN.LAST_NAME_SRCH = '" + vchrLNameNS + "', " +
                                                "PN.FIRST_NAME_SRCH='" + vchrFNameNS + "', " +
                                                "LAST_NAME ='" + TxtApellidoR + "', " +
                                                "FIRST_NAME='" + TxtNombreR + "', " +
                                                "SECOND_LAST_NAME='" + TxtCasadaR + "', " +
                                                "SECOND_LAST_SRCH='" + TxtCasadaR + "', " +
                                                "NAME_DISPLAY='" + TxtNombreR + " " + vchrApellidosCompletos + "', " +
                                                "NAME_FORMAL='" + TxtNombreR + " " + vchrApellidosCompletos + "', " +
                                                "NAME_DISPLAY_SRCH ='" + (vchrFNameNS + vchrLNameNS + vchrCNameNS).TrimEnd() + "'," +
                                                "LASTUPDDTTM = SYSDATE, " +
                                                "LASTUPDOPRID = '" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "' " +
                                                "WHERE PN.EMPLID = '" + emplid + "' AND NAME_TYPE IN 'REC'  AND EFFDT ='"+ DateTime.Now.ToString("dd/MM/yyyy")+"'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //INSERTA NOMBRE DEL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_NAMES ( " +
                                                            "EMPLID, " +
                                                            "NAME_TYPE, " +
                                                            "EFFDT, " +
                                                            "EFF_STATUS, " +
                                                            "COUNTRY_NM_FORMAT, " +
                                                            "NAME, " +
                                                            "NAME_INITIALS, " +
                                                            "NAME_PREFIX, " +
                                                            "NAME_SUFFIX, " +
                                                            "NAME_ROYAL_PREFIX, " +
                                                            "NAME_ROYAL_SUFFIX, " +
                                                            "NAME_TITLE, " +
                                                            "LAST_NAME_SRCH, " +
                                                            "FIRST_NAME_SRCH, " +
                                                            "LAST_NAME, " +
                                                            "FIRST_NAME, " +
                                                            "MIDDLE_NAME, " +
                                                            "SECOND_LAST_NAME, " +
                                                            "SECOND_LAST_SRCH, " +
                                                            "NAME_AC, " +
                                                            "PREF_FIRST_NAME, " +
                                                            "PARTNER_LAST_NAME, " +
                                                            "PARTNER_ROY_PREFIX, " +
                                                            "LAST_NAME_PREF_NLD, " +
                                                            "NAME_DISPLAY, " +
                                                            "NAME_FORMAL, " +
                                                            "NAME_DISPLAY_SRCH, " +
                                                            "LASTUPDDTTM, " +
                                                            "LASTUPDOPRID " +
                                                            ") VALUES(" +
                                                                "'" + emplid + "'," + // EMPLID
                                                                "'REC','" + // NAME_TYPE
                                                                DateTime.Now.ToString("dd/MM/yyyy") + "'," + // EFFDT
                                                                "'A'," + // EFF_STATUS
                                                                "'MEX'," + // COUNTRY_NM_FORMAT
                                                                "'" + vchrApellidosCompletos + "," + TxtNombreR + "'," + // NAME
                                                                "' '," + // NAME_INITIALS
                                                                "' '," + // NAME_PREFIX
                                                                "' '," + // NAME_SUFFIX
                                                                "' '," + // NAME_ROYAL_PREFIX
                                                                "' '," + // NAME_ROYAL_SUFFIX
                                                                "' '," + // NAME_TITLE
                                                                "'" + vchrLNameNS + "'," + // LAST_NAME_SRCH
                                                                "'" + vchrFNameNS + "'," +// FIRST_NAME_SRCH
                                                                "'" + TxtApellidoR + "'," + // LAST_NAME
                                                                "'" + TxtNombreR + "'," + // FIRST_NAME
                                                                "' '," + // MIDDLE_NAME
                                                                "'" + TxtCasadaR + "'," + // SECOND_LAST_NAME
                                                                "'" + vchrCNameNS + " '," + // SECOND_LAST_SRCH
                                                                "' '," + // NAME_AC
                                                                "' '," + // PREF_FIRST_NAME
                                                                "' '," + // PARTNER_LAST_NAME
                                                                "' '," + // PARTNER_ROY_PREFIX
                                                                "'1'," + // LAST_NAME_PREF_NLD
                                                                "'" + TxtNombreR + " " + vchrApellidosCompletos + "'," + // NAME_DISPLAY
                                                                "'" + TxtNombreR + " " + vchrApellidosCompletos + "'," + // NAME_FORMAL
                                                                "'" + (vchrFNameNS + vchrLNameNS + vchrCNameNS).TrimEnd() + "'," + // NAME_DISPLAY_SRCH
                                                                "SYSDATE," + // LASTUPDDTTM
                                                                "'" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";  // LASTUPDOPRID                                                                    
                            cmd.ExecuteNonQuery();
                        }

                        //ACTUALIZA NIT
                        if (ContadorNit == 0)
                        {
                            //INSERTA EL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSTEM (EMPLID, EXTERNAL_SYSTEM, EFFDT, EXTERNAL_SYSTEM_ID) " +
                            "VALUES ('" + emplid + "','NRE','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + NIT + "' AND EFFDT ='"+ DateTime.Now.ToString("dd/MM/yyyy")+"')";
                            cmd.ExecuteNonQuery();


                            if (ContadorNit2 == 0)
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_EXTERNAL_SYSKEY (EMPLID, EXTERNAL_SYSTEM) " +
                                "VALUES ('" + emplid + "','NRE')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            //ACTUALIZA NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_EXTERNAL_SYSTEM SET EXTERNAL_SYSTEM_ID = '" + NIT + "' " +
                                                " WHERE EXTERNAL_SYSTEM = 'NRE' AND EMPLID='" + emplid + "'";
                            cmd.ExecuteNonQuery();
                        }

                        if (ContadorDirecionNit == 0)
                        {
                            //INSERTA LA DIRECCION DEL NIT
                            cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES (EMPLID, ADDRESS_TYPE,COUNTY,CITY,NUM1, NUM2, HOUSE_TYPE, ADDR_FIELD1, ADDR_FIELD2, ADDR_FIELD3,POSTAL,GEO_CODE,IN_CITY_LIMIT," +
                                "ADDRESS1_AC,ADDRESS2_AC,ADDRESS3_AC,CITY_AC,REG_REGION,EFFDT,EFF_STATUS,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,STATE,LASTUPDDTTM,LASTUPDOPRID) " +
                               "VALUES('" + emplid + "', 'REC',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ', '" + DateTime.Now.ToString("dd/MM/yyyy")
                               + "', 'A', '" + PaisNit + "', '" + TxtDiRe1 + "', '" + TxtDiRe2 + "', '" + TxtDiRe3 + "', ' ','" + StateNit + "', '" +
                               DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "','" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "')";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES_SA (LOC_ADDR_DATA,LOC_ADDR, LOC_ADDR_LINE, LOC_ADDR_TYPE, EXT_ORG_ID, DESCR_EXT_ORG, DESCR_ORG_LOCATION, " +
                                "CONTACT_NAME, DATA_SOURCE, EMPLID, ADDRESS_TYPE,EFFDT,ORG_LOCATION,MAINT_ADDR_MANUAL,MAINT_OTHER_MANUAL,ORG_CONTACT,SEASONAL_ADDR) " +
                                "VALUES(' ',' ',' ',' ',' ',' ',' ',' ',' ','" + emplid + "', 'REC', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 0, 'N','N',0,'N')";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //ACTUALIZA DIRECCION DEL NIT
                            cmd.CommandText = "UPDATE SYSADM.PS_ADDRESSES A SET A.STATE =  '" + StateNit + "', " +
                                "A.ADDRESS1 = '" + TxtDiRe1 + "', " +
                                "A.ADDRESS2 = '" + TxtDiRe2 + "', " +
                                "A.ADDRESS3 = '" + TxtDiRe3 + "', " +
                                "A.COUNTRY = '" + PaisNit + "', LASTUPDOPRID ='" + Context.User.Identity.Name.Replace("@unis.edu.gt", "") + "',  LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") +
                                "' WHERE A.EMPLID = '" + emplid + "' AND ADDRESS_TYPE ='REC' AND EFFDT ='"+ DateTime.Now.ToString("dd/MM/yyyy")+"'";
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        con.Close();
                        return "0";
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        lblActualizacion.Text = "Ocurrió un problema al actualizar el NIT " + x;
                        return "1";
                    }
                }

            }
        }


    }
}