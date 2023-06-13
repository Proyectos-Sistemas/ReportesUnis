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

namespace ReportesUnis
{
    public partial class ConfirmaciónDeCarne : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
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
            if (txtCantidad.Text != "0")
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1.ImageUrl = "~/Usuarios/DPI/" + CmbCarne.Text + "(1).jpg";
                    }
                    if (i == 1)
                    {
                        ImgDPI2.ImageUrl = "~/Usuarios/DPI/" + CmbCarne.Text + "(2).jpg";
                    }
                }
            }
            else
            {
                HDocumentacion.Visible = true;
            }
            if (!CmbCarne.Text.IsNullOrWhiteSpace())
            {
                lblActualizacion.Text = null;
            }
            HFoto.Visible = true;
            ImgFoto1.ImageUrl = "~/Usuarios/Fotos/" + TxtDpi.Text + ".jpg";
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
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS FROM DUAL UNION " +
                        "SELECT NO_CUI||DEPTO_CUI||MUNI_CUI CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE " + where + " AND TIPO_PERSONA = 2";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpi.Text = reader["CUI"].ToString();
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
                        txtCantidad.Text = reader["TOTALFOTOS"].ToString();
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

                lblActualizacion.Text = "No se encontró información confirmada para el número de Carne " + txtCarne.Text;
            }
            else
            {
                txtCarne.Text = null;
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
        }

        private void Rechazar()
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
                            cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + CmbCarne.Text + "'";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            con.Close();
                            Buscar("1");
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
            Rechazar();
        }

        protected void Confirmar()
        {
            if (!TxtPrimerNombre.Text.IsNullOrWhiteSpace())
            {
                string respuesta = null;
                string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                QueryInsertBi();
                QueryActualizaNombre();
                //SE INGRESA LA INFORMACIÓN EN EL BANCO
                respuesta = ConsumoSQL(txtInsertBI.Text);
                if (respuesta == "0")
                {
                    respuesta = ConsumoOracle(txtInsertName.Text);

                    if (respuesta == "0")
                    {
                        respuesta = "";
                        QueryUpdateApex("0", fecha, fecha, fecha, "1", CmbCarne.Text);
                        if (!txtInsertApex.Text.IsNullOrWhiteSpace())
                        {
                            respuesta = ConsumoOracle(txtInsertApex.Text);
                        }
                    }
                }

                if (respuesta == "0")
                {
                    lblActualizacion.Text = "Se confirmó correctamente la información";
                    Buscar("1");
                    File.Delete(CurrentDirectory+"/Usuarios/Fotos/" + TxtDpi.Text + ".jpg");
                    for (int i = 0; i < Convert.ToInt16(txtCantidad.Text); i++)
                    {
                        File.Delete(CurrentDirectory+ "/Usuarios/DPI/" + CmbCarne.Text + "(" + i + ").jpg");
                    }
                    LimpiarCampos();
                }
                else
                {
                    lblActualizacion.Text = "Ocurrió un problema al confirmar la información";
                    ConsumoSQL("DELETE FROM [dbo].[Tarjeta_Identificacion_prueba] WHERE CARNET ='" + CmbCarne.Text + "'");
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

        protected void QueryActualizaNombre()
        {
            txtInsertName.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = '" + TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + " " + TxtApellidoCasada.Text + "," + TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text + "', PN.LAST_NAME_SRCH =REPLACE(UPPER('" + TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + "'),' ',''), PN.FIRST_NAME_SRCH=REPLACE(UPPER('" + TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text + "'),' ',''), LAST_NAME ='" + TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + "', FIRST_NAME='" + TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text + "', SECOND_LAST_NAME='" + TxtApellidoCasada.Text + "', SECOND_LAST_SRCH=(REPLACE(UPPER('" + TxtApellidoCasada.Text + "'),' ',''))||' ', NAME_DISPLAY='" + TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text + " " + TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + " " + TxtApellidoCasada.Text + "', NAME_FORMAL='" + TxtPrimerNombre.Text + " " + TxtSegundoNombre.Text + " " + TxtPrimerApellido.Text + " " + TxtSegundoApellido.Text + " " + TxtApellidoCasada.Text + "', NAME_DISPLAY_SRCH =UPPER(REPLACE('" + TxtPrimerNombre.Text + TxtSegundoNombre.Text + TxtPrimerApellido.Text + TxtSegundoApellido.Text + TxtApellidoCasada.Text + "',' ',''))  WHERE PN.EMPLID = '" + CmbCarne.Text + "'";
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
            Confirmar();
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
                lblActualizacion.Text = "Debe de ingresar un número de carnet para poder realizar la generación.";
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

    }
}