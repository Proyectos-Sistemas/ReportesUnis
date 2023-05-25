using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Interop;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Ajax.Utilities;
//using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Oracle.ManagedDataAccess.Client;

namespace ReportesUnis
{
    public partial class ActualizacionEstudiantes : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string mensaje = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");
            TextUser.Text = "2676467470101";
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_VistaAlumnos") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LeerInfoTxt();
                LeerInfoTxtSQL();
                LeerInfoTxtPath();
                llenadoPais();
                mostrarInformación();
                llenadoDepartamento();
                llenadoState();

                
                if (String.IsNullOrEmpty(txtCarne.Text))
                {
                    BtnActualizar.Visible = false;
                    lblActualizacion.Text = "El usuario utilizado no se encuentra registrado como estudiante";
                    CmbPais.SelectedValue = "Guatemala";
                    tabla.Visible = false;
                    FileUpload1.Visible = false;
                    lblfoto.Visible = false;
                }
            }
        }

        //Metodos
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
        private void mostrarInformación()
        {
            string constr = TxtURL.Text;
            var dia = "";
            var mes = "";
            var anio = "";
            var bday = "";
            var apellidoEx = "0";
            int posicion = 0;
            int posicion2 = 0;
            int largoApellido = 0;
            int excepcionApellido = 0;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO," +
                        "DEPARTAMENTO, SECOND_LAST_NAME, CNT FROM (" +
                    "SELECT PD.EMPLID, PN.NATIONAL_ID CARNE,  PD.FIRST_NAME, " +
                    "PD.LAST_NAME, PD.SECOND_LAST_NAME, PN.NATIONAL_ID DPI, PN.NATIONAL_ID_TYPE, PP.PHONE , " +
                    "TO_CHAR(PD.BIRTHDATE,'YYYY-MM-DD') BIRTHDATE, " +
                    "APD.DESCR CARRERA, AGT.DESCR FACULTAD, " +
                    "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'No Consta' END STATUS, " +
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
                    //"WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " + //---1581737080101
                    //"WHERE PN.NATIONAL_ID ='3682754340101' " + // de la cerda
                    //"WHERE PN.NATIONAL_ID ='2676467470101' " + // DE LEON
                    "WHERE PN.NATIONAL_ID ='4681531' " + // DE LEON
                    //"WHERE PN.NATIONAL_ID ='2993196360101' " + // De Tezanos Rustrián  
                   ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtCarne.Text = reader["CARNE"].ToString();
                        txtNombre.Text = reader["FIRST_NAME"].ToString();
                        txtNInicial.Text = reader["FIRST_NAME"].ToString();
                        txtApellido.Text = reader["LAST_NAME"].ToString();
                        txtCasada.Text = reader["SECOND_LAST_NAME"].ToString();
                        txtCInicial.Text = reader["SECOND_LAST_NAME"].ToString();
                        txtAInicial.Text = reader["LAST_NAME"].ToString();
                        largoApellido = txtAInicial.Text.Length;// + " " + posicion.ToString();

                        if ((txtApellido.Text.Substring(0, 5)).ToUpper().Equals("DE LA"))
                        {
                            posicion = txtApellido.Text.Substring(6, largoApellido - 6).IndexOf(" ");
                            txtContaador.Text = txtAInicial.Text.Length.ToString() + " " + posicion.ToString();
                            txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 6);
                        }
                        else
                        {
                            posicion = reader["LAST_NAME"].ToString().IndexOf(" ");
                            apellidoEx = divisionApellidos(reader["LAST_NAME"].ToString().Substring(0, posicion));
                            txtContaador.Text = apellidoEx.ToString();
                            excepcionApellido = apellidoEx.ToString().IndexOf("    }");
                            txtContaador.Text = apellidoEx.ToString().Substring(excepcionApellido - 3, 1);
                            if (apellidoEx.ToString().Substring(excepcionApellido - 3, 1).Equals("1"))
                            {
                                posicion2 = txtApellido.Text.Substring(posicion + 1, largoApellido - (posicion+1)).IndexOf(" ");
                                txtContaador.Text = posicion2.ToString();
                                txtPrimerApellido.Text = txtApellido.Text.Substring(0, posicion + 1 + posicion2);
                            }
                        }

                        txtDPI.Text = reader["DPI"].ToString();
                        CmbEstado.SelectedValue = reader["STATUS"].ToString();

                        bday = reader["BIRTHDATE"].ToString();
                        anio = bday.Substring(0, 4);
                        mes = bday.Substring(5, 2);
                        dia = bday.Substring(8, 2);
                        txtCumple.Text = dia + "-" + mes + "-" + anio;

                        txtDireccion.Text = reader["DIRECCION"].ToString();
                        TrueDir.Text = reader["DIRECCION"].ToString();
                        txtDireccion2.Text = reader["DIRECCION2"].ToString();
                        txtDireccion3.Text = reader["DIRECCION3"].ToString();
                        if (!String.IsNullOrEmpty(reader["PAIS"].ToString()))
                            CmbPais.SelectedValue = reader["PAIS"].ToString();
                        else
                            CmbPais.SelectedValue = "";
                        llenadoDepartamento();
                        CmbDepartamento.SelectedValue = reader["DEPARTAMENTO"].ToString();
                        llenadoMunicipio();
                        CmbMunicipio.SelectedValue = reader["MUNICIPIO"].ToString();
                        txtTelefono.Text = reader["PHONE"].ToString();
                        TruePhone.Text = reader["PHONE"].ToString();
                        txtCarrera.Text = reader["CARRERA"].ToString();
                        txtFacultad.Text = reader["FACULTAD"].ToString();
                        UserEmplid.Text = reader["EMPLID"].ToString();
                    }
                    con.Close();
                }
            }
        }
        protected void llenadoDepartamento()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO FROM SYSADM.PS_STATE_TBL ST  " +
                    "JOIN SYSADM.PS_COUNTRY_TBL CT ON ST.COUNTRY = CT.COUNTRY " +
                    "WHERE CT.DESCR ='" + CmbPais.Text + "' AND SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) IS NOT NULL  " +
                    "GROUP BY SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) ORDER BY DEPARTAMENTO";

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbDepartamento.DataSource = ds;
                        CmbDepartamento.DataTextField = "DEPARTAMENTO";
                        CmbDepartamento.DataValueField = "DEPARTAMENTO";
                        CmbDepartamento.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbDepartamento.DataTextField = "";
                        CmbDepartamento.DataValueField = "";
                    }
                }
            }
        }
        protected void llenadoMunicipio()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, ST.STATE STATE FROM SYSADM.PS_STATE_TBL ST " +
                        "WHERE REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%" + CmbDepartamento.SelectedValue + "') " +
                        "GROUP BY REGEXP_SUBSTR(ST.DESCR,'[^-]+'), ST.STATE ORDER BY MUNICIPIO";
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        CmbMunicipio.DataSource = ds;
                        CmbMunicipio.DataTextField = "MUNICIPIO";
                        CmbMunicipio.DataValueField = "MUNICIPIO";
                        CmbMunicipio.DataBind();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        CmbMunicipio.DataSource = "-";
                        CmbMunicipio.DataTextField = "-";
                        CmbMunicipio.DataValueField = "-";
                    }
                }
            }
        }
        protected void llenadoPais()
        {
            string where = "";
            if (!String.IsNullOrEmpty(CmbPais.Text))
                where = "WHERE COUNTRY='" + CmbPais.Text + "'";
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
                    CmbPais.DataSource = ds;
                    CmbPais.DataTextField = "PAIS";
                    CmbPais.DataValueField = "PAIS";
                    CmbPais.DataBind();
                    con.Close();
                }
            }
        }
        protected void llenadoState()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (!String.IsNullOrEmpty(CmbMunicipio.SelectedValue))
                    {
                        string descrip = "";
                        if (CmbPais.SelectedValue == "Guatemala")
                        {
                            descrip = CmbMunicipio.SelectedValue + "-" + CmbDepartamento.SelectedValue;
                        }
                        else
                        {
                            descrip = CmbDepartamento.SelectedValue;
                        }
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + descrip.TrimEnd('-') + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            State.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                    else
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbDepartamento.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            State.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }
                }
            }
        }
        protected string estadoCivil()
        {
            var VALOR = CmbEstado.SelectedValue.Substring(0, 1).ToString();
            if (VALOR.Equals("S"))
            {
                VALOR = "S";
            }
            else if (VALOR.Equals("C"))
            {
                VALOR = "M";
            }
            else
            {
                VALOR = "U";
            }
            return VALOR;
        }
        private string actualizarInformacion()
        {
            
            if (txtAInicial.Text == txtApellido.Text && txtNInicial.Text == txtNombre.Text && txtCInicial.Text == txtCasada.Text)
            {
                txtAccion.Text = "1";
                txtTipoAccion.Text = "1.1";
                txtConfirmacion.Text = "00"; //No requiere confirmación de operador 
                if (FileUpload1.HasFile && !String.IsNullOrEmpty(txtDireccion.Text) && !String.IsNullOrEmpty(txtTelefono.Text) && !String.IsNullOrEmpty(CmbPais.Text) && !String.IsNullOrEmpty(CmbMunicipio.Text) && !String.IsNullOrEmpty(CmbDepartamento.Text) && !String.IsNullOrEmpty(CmbEstado.Text))
                {
                    IngresoDatos();
                }
                else
                {
                    if (FileUpload1.HasFile)
                    {
                        mensaje = "No puede enviarse información vacía y es necesario seleccionar el estado civil, un país, un departamento y un muncipio";
                    }
                    else
                    {
                        mensaje = "No puede enviarse información vacía y es necesario cargar una fotografía, seleccionar el estado civil, un país, un departamento y un muncipio";
                    }
                }
            }
            else
            {
                int contador = 1;
                if (FileUpload2.HasFile)
                {
                    if (FileUpload1.HasFiles && FileUpload2.HasFiles) { 
                        txtAccion.Text = "1";
                        txtTipoAccion.Text = "1.1";
                        txtConfirmacion.Text = "01"; //Requiere confirmación de operador 
                        IngresoDatos();

                        foreach (HttpPostedFile uploadedFile in FileUpload2.PostedFiles)
                        {
                            string nombreArchivo = txtCarne.Text + "(" + contador + ").jpg";
                            string ruta = CurrentDirectory + "/DPIUsuarios/" + nombreArchivo;
                            uploadedFile.SaveAs(ruta);
                            contador++;
                        }                        
                    }
                    else
                    {
                        mensaje = "Es necesario cargar la fotografía y los documentos de identificación. Por favor cargar ambas de nuevo.";
                    }
                }
                else
                {
                    CargaDPI.Visible = true;                
                }
            }
                return mensaje;
        }

        //Eventos       
        protected void CmbMunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenadoState();
        }
        protected void CmbDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenadoMunicipio();
            llenadoState();
        }

        protected string IngresoDatos()
        {
            try
            {
                string constr = TxtURL.Text;
                string codPais = "";
                string ec = estadoCivil();
                string RegistroCarne = "0";
                string controlOracle = "0";
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
                        cmd.CommandText = "SELECT COUNTRY FROM SYSADM.PS_COUNTRY_TBL WHERE DESCR = '" + CmbPais.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            codPais = reader["COUNTRY"].ToString();
                        }

                        //SE VALIDA QUE NO EXISTA INFORMACIÓN REGISTRADA
                        cmd.Transaction = transaction;
                        cmd.Connection = con;
                        txtExiste2.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARGO = '" + txtCarrera.Text + "' AND FACULTAD ='" + txtFacultad.Text + "' AND CARNET =SUBSTR('" + txtCarne.Text + "',0,13) AND (REPLACE(NOMBRE1||NOMBRE2,' ','') = REPLACE('"+txtNombre.Text+ "',' ','') AND REPLACE(APELLIDO1||APELLIDO2,' ','') = REPLACE('"+txtApellido.Text+ "', ' ', '')  AND UPPER(REPLACE(DECASADA,' ','')) = UPPER(REPLACE('" + txtCasada.Text+"', ' ', '')))";
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARGO = '" + txtCarrera.Text + "' AND FACULTAD ='" + txtFacultad.Text + "' AND CARNET =SUBSTR('" + txtCarne.Text + "',0,13) AND (REPLACE(NOMBRE1||NOMBRE2,' ','') = REPLACE('"+txtNombre.Text+ "',' ','') AND REPLACE(APELLIDO1||APELLIDO2,' ','') = REPLACE('"+txtApellido.Text+ "', ' ', '')  AND UPPER(REPLACE(DECASADA,' ','')) = UPPER(REPLACE('" + txtCasada.Text+"', ' ', '')))";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            RegistroCarne = reader["CONTADOR"].ToString();
                        }
                        //txtExiste.Text = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARGO = '" + txtCarrera.Text + "' AND FACULTAD ='" + txtFacultad.Text + "' AND CARNET =SUBSTR('" + txtCarne.Text + "',0,9)";
                        txtExiste.Text = RegistroCarne.ToString()+ " registros";

                        if (RegistroCarne == "0")
                        {
                            string nombreArchivo = txtCarne.Text + ".jpg";
                            string ruta = txtPath.Text + nombreArchivo;
                            FileUpload1.SaveAs(ruta);
                            //txtInsert.Text = txtPath.Text + nombreArchivo;

                            cmd.Transaction = transaction;
                            //Obtener codigo país
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT 'INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte, Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, " +
                            //txtInsert.Text = "SELECT 'INSERT INTO UNIS_INTERFACES.TBL_HISTORIAL_CARNE (Apellido1,Apellido2, Carnet, Cedula, Decasada, Depto_Residencia, Direccion, Email, Estado_Civil, Facultad, FechaNac, Flag_cedula, Flag_dpi, Flag_pasaporte, Muni_Residencia, Nit, No_Cui, No_Pasaporte, Nombre1, Nombre2, Nombreimp, Pais_nacionalidad, Profesion, Sexo, Telefono, Zona, Accion, Celular, Codigo_Barras, Condmig, IDUNIV, Pais_pasaporte, Tipo_Accion, Tipo_Persona, Pais_Nit, Depto_Cui, Muni_Cui, Validar_Envio, Path_file, Codigo, Depto, Fecha_Hora, Fecha_Entrega, Fecha_Solicitado, Tipo_Documento, Cargo, " +
                                            " Fec_Emision, NO_CTA_BI, ID_AGENCIA, CONFIRMACION) VALUES ('''" +
                                            "||'" + txtPrimerApellido.Text + "'''||','" + //APELLIDO1
                                            "||''''||SUBSTR(LAST_NAME, length('" + txtPrimerApellido.Text + "')+2, length(last_name)-length('" + txtPrimerApellido.Text + "')-1) ||' '||'" + txtCasada.Text.TrimEnd(' ')+"'||''''||','" + //APELLIDO2
                                            "||''''||SUBSTR(CARNE,0,13)||''''||','" + //CARNE
                                            "||''''||CEDULA||''''||','" + //CEDULA
                                            "||''''||SECOND_LAST_NAME||''''||','" +// APELLIDO DE CASADA
                                            "||''''||UPPER(DEPARTAMENTO)||''''||','" + //DEPARTAMENTO DE RESIDENCIA
                                            "||''''||SUBSTR(DIRECCION,0,30)||''''||','" + // DIRECCION
                                            "||''''||EMAIL||''''||','" + // CORREO ELECTRONICO
                                            "||STATUS||','" + // ESTADO CIVIL
                                            "||'''" + txtFacultad.Text + "'''||','" + // FACULTAD
                                            "||''''||BIRTHDATE||''''||','" + //FECHA DE NACIMIENTO
                                            "||''''||FLAG_CED||''''||','" +
                                            "||''''||FLAG_DPI||''''||','" +
                                            "||''''||FLAG_PAS||''''||','" +
                                            "||''''||UPPER(MUNICIPIO)||''''||'," + //MUNICIPIO DE RESIDENCIA
                                            "NULL,'" + //NIT
                                            "||''''||DPI||''''||','" + // NO_CUI
                                            "||''''||PASAPORTE||''''||','" + // NUMERO DE PASAPORTE
                                            "||''''||FIRST_NAME||''''||','" + //NOMBRE1
                                            "||''''||SECOND_NAME||''''||','" +// NOMBRE 2
                                            "||''''||FIRST_NAME||' '||'" + txtPrimerApellido.Text + "'||''''||','" + //APELLIDO DE IMPRESION
                                            "||''''||PLACE||''''||','" + // PAIS NACIONALIDAD
                                            "||''''||PROF||''''||','" + // PROFESION
                                            "||SEX||'," + // SEXO
                                            "NULL," + //TELEFONO
                                            "NULL," + //ZONA
                                            "" + txtAccion.Text + ",'" + //ACCION
                                            "||''''||PHONE||''''||','" + //CELULAR
                                            "||CARNE||','" + //CODIGO DE BARRAS
                                            "||''''||CONDMIG||''''||','" + //CONDICION MIGRANTE
                                            "||'2022,'" + //ID  UNIVERSIDAD
                                            "||''''||PAIS_PASAPORTE||''''||','" + //PAIS PASAPORTE
                                            "'" + txtTipoAccion.Text +  //TIPO_ACCION
                                            "'','||2||'" + //TIPO PERSONA
                                            ",NULL,'" + // PAIS NIT
                                            "||''''||DEPARTAMENTO_CUI||''''||','" + // DEPARTAMENTO CUI
                                            "||''''||MUNICIPIO_CUI||''''||'," + //MUNICIPIO CUI
                                            "1," + //VALIDAR ENVIO
                                            "'||'''" + ruta + "'''||'," + //PATH
                                            "NULL," + //CODIGO
                                            "NULL,'" + // DEPARTAMENTO
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_HORA
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_ENTREGA
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||','" +//FECHA_SOLICITADO
                                            "||TIPO_DOCUMENTO||','" + //TIPO DOCUMENTO
                                            "||'''" + txtCarrera.Text + "'''||','" + //CARGO
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD')||''''||'" +//FECHA_EMISION
                                            ", 0," + //NO CTA BI
                                            " 2002," +//ID AGENCIA
                                            ""+txtConfirmacion.Text+")'" + // confirmación operador
                                            " AS INS " +
                                            "FROM ( SELECT " +
                                            "DISTINCT PD.EMPLID, " +
                                            "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) CARNE, " +
                                            "REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+') FIRST_NAME, " +
                                            "SUBSTR(PD.FIRST_NAME,  LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))+2, LENGTH(PD.FIRST_NAME)-LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))) SECOND_NAME, " +
                                            "PD.LAST_NAME, PD.BIRTHCOUNTRY," +
                                            "PD.SECOND_LAST_NAME, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,0,9)" +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,0,9) ELSE '' END DPI, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,12,2) " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,12,2) ELSE '' END MUNICIPIO_CUI," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN  SUBSTR(PN.NATIONAL_ID,10,2) " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,10,2) ELSE '' END DEPARTAMENTO_CUI," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "    WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_DPI, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN C.DESCR WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN C.DESCR ELSE NULL END PAIS_PASAPORTE, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '2' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '2'" +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '3' ELSE ' ' END TIPO_DOCUMENTO," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_CED, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE NULL END PASAPORTE, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_PAS, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '2' ELSE NULL END CONDMIG, " +
                                            "PPD.PHONE, " +
                                            "TO_CHAR(PD.BIRTHDATE, 'DD-MM-YYYY') BIRTHDATE, " +
                                            //"APD.DESCR CARRERA, " +
                                            "AGT.DESCR FACULTAD, " +
                                            "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE '' END SEX, " +
                                            "CASE WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'Condición Migrante' WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE C.DESCR END PLACE," +
                                            "CASE WHEN PD.MAR_STATUS = 'M' THEN '2' WHEN PD.MAR_STATUS = 'S' THEN '1' ELSE '' END STATUS, " +
                                            "(select REPLACE(A1.ADDRESS1,'|' , ' ') || ' ' ||  REPLACE(A1.ADDRESS2,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) DIRECCION, " +
                                            " (select REPLACE(A1.ADDRESS3,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) ZONA, " +
                                            "REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO, " +
                                            "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, " +
                                            "'ESTUDIANTE' PROF, " +
                                            "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = PD.EMPLID AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) EMAIL " +
                                            "FROM " +
                                            "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                            "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON C.COUNTRY = PD.BIRTHCOUNTRY " +
                                            "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                            "AND A.EFFDT =(SELECT MAX(EFFDT) FROM SYSADM.PS_ADDRESSES A2 WHERE A.EMPLID = A2.EMPLID AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE) " +
                                            "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                            "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID AND SE.STDNT_ENRL_STATUS = 'E' AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                            "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID AND CT.STRM = SE.STRM AND CT.ACAD_CAREER = SE.ACAD_CAREER AND SE.INSTITUTION = CT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                            //"--WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                                            //"WHERE PN.NATIONAL_ID ='2327809510101')" +
                                            "WHERE PN.NATIONAL_ID ='4681531')" +
                                            "WHERE CARNE=DPI||DEPARTAMENTO_CUI||MUNICIPIO_CUI OR CARNE=PASAPORTE OR CARNE=CEDULA " +
                                            "ORDER BY 1 ASC";
                            //--4681531 PASAPORTE
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                txtInsert.Text = reader["INS"].ToString();
                            }
                            cmd.Transaction = transaction;
                            //Obtener codigo país
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
                                           ",[Tipo_cta] " +
                                           ",[Emp_trab] " +
                                           ",[Fec_In_Tr] " +
                                           ",[Puesto_Tr] " +
                                           ",[Lug_Tr] " +
                                           ",[Fe_In_Tr] " +
                                           ",[Ing_Tr] " +
                                           ",[Egr_Tr] " +
                                           ",[Mone_Tr] " +
                                           ",[Tel_Tr] " +
                                           ",[Dir_Tr] " +
                                           ",[Zona_Tr] " +
                                           ",[Dep_Tr] " +
                                           ",[Muni_Tr] " +
                                           ",[Pais_Tr] " +
                                           ",[Act_Ec] " +
                                           ",[Otra_Na] " +
                                           ",[Condmig] " +
                                           ",[O_Condmig] " +
                                           ",[Validar_Envio]) " +
                                     "VALUES ('''||SUBSTR(CARNE,0,13)||''''||','" + //CARNE
                                            "||''''||SUBSTR(DIRECCION,0,30)||''''||','" + // DIRECCION
                                            "||'NULL," + //ZONA
                                            " NULL,'" + //COLONIA
                                            "||''''||CEDULA||''''||','" + //CEDULA
                                            "||'NULL, " + //DEPARTAMENTO CEDULA
                                            "NULL,'" + //MUNICIPIO CEDULA
                                            "||'''" + txtCarrera.Text + "'''||','" + //CARGO
                                            "||'NULL,'" + //DEPARTAMENTO
                                            "||'''" + txtFacultad.Text + "'''||','" + // FACULTAD
                                            "||'NULL," + //CODIGO
                                            "2," + //TIPO PERSONA
                                            "0," + //NO CTA BI
                                            "'''||BIRTHDATE||''''||','" + //FECHA DE NACIMIENTO           
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''''||','" +//FECHA_ENTREGA
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''''||','" +//FECHA_SOLICITADO
                                            "||'" + txtAccion.Text + "," + //ACCION
                                            "NULL," + //TELEFONO
                                            "NULL,'" + //NIT
                                            "||''''||FIRST_NAME||''''||','" + //NOMBRE1
                                            "||'''" + txtPrimerApellido.Text + "'''||','" + //APELLIDO1
                                            "||''''||SUBSTR(LAST_NAME, length('" + txtPrimerApellido.Text + "')+2, length(last_name)-length('" + txtPrimerApellido.Text + "')-1)||' "+txtCasada.Text.TrimEnd(' ')+"'||''''||','" + //APELLIDO2
                                            "||''''||SECOND_LAST_NAME||''''||','" +// APELLIDO DE CASADA
                                            "||''''||SECOND_NAME||''''||','" +// NOMBRE 2
                                            "||''''||FIRST_NAME||' '||'" + txtPrimerApellido.Text + "'||''''||','" + //APELLIDO DE IMPRESION
                                            "||SEX||','" + // SEXO
                                            "||STATUS||','" + // ESTADO CIVIL
                                            "||'''" + ruta + "'''||','" + //PATH
                                                                          //"NULL,'" + // PATH
                                            "||''''||TO_CHAR(SYSDATE,'YYYY-MM-DD HH:MM:SS')||''''||'," +//FECHA_HORA
                                            "" + txtTipoAccion.Text + "," +//TIPO_ACCION
                                            "2022,'" + //ID  UNIVERSIDAD
                                            "||CARNE||'," + //CODIGO DE BARRAS
                                            "NULL," +//FECHA_EMISION
                                            "NULL," + //Nombre
                                            "NULL," + //Promocion
                                            "NULL," + //No_Recibo
                                            "NULL," + //Tipo_Sangre
                                            "NULL,'" + //Status
                                            "||TIPO_DOCUMENTO||'," + //TIPO DOCUMENTO
                                            "2002,'" +//ID AGENCIA
                                            "||''''||UPPER(MUNICIPIO)||''''||','" + //MUNICIPIO RESIDENCIA
                                            "||''''||UPPER(DEPARTAMENTO)||''''||'," + // DEPARTAMENTO RECIDENCIA
                                            "NULL," + //norden
                                            "NULL,'" + //Observaciones
                                            "||''''||PLACE||''''||','" + // PAIS NACIONALIDAD
                                            "||''''||PAIS_PASAPORTE||''''||','" + //PAIS PASAPORTE
                                            "||''''||PASAPORTE||''''||','" + // NUMERO DE PASAPORTE
                                            "||''''||PROF||''''||'," + // PROFESION
                                            "NULL," + //Casa
                                            "NULL,'" + //Apto
                                            "||''''||PHONE||''''||','" + //CELULAR
                                            "||''''||EMAIL||''''||','" + // CORREO ELECTRONICO
                                            "||''''||DPI||''''||','" + // NO_CUI
                                            "||''''||DEPARTAMENTO_CUI||''''||','" + // DEPARTAMENTO CUI
                                            "||''''||MUNICIPIO_CUI||''''||'," + //MUNICIPIO CUI
                                            "NULL,'" + //Pais_Nit
                                            "||''''||FLAG_CED||''''||','" +
                                            "||''''||FLAG_DPI||''''||','" +
                                            "||''''||FLAG_PAS||''''||'," +
                                            " 0," + //NO CTA BI
                                            "NULL," + //Emp_trab
                                            "NULL," + //Fec_In_Tr
                                            "NULL," + //Puesto_Tr
                                            "NULL," + //Lug_Tr
                                            "NULL," + //Fe_In_Tr
                                            "NULL," + //Ing_Tr
                                            "NULL," + //
                                            "NULL," + //
                                            "NULL," + //
                                            "NULL," + //Dir_Tr
                                            "NULL," + //Zona_Tr
                                            "NULL," + //Dep_Tr
                                            "NULL," + //Muni_Tr
                                            "NULL," + //Pais_Tr
                                            "NULL," + //Act_Ec
                                            "NULL,'" + //Otra_Na
                                            "||''''||CONDMIG||''''||'," + //CONDICION MIGRANTE
                                            "NULL," + //OTRA CONDICION MIGRANTE" 
                                            "1)'" + //Validar_Envio" 
                                            " AS INS " +
                                            "FROM ( SELECT " +
                                            "DISTINCT PD.EMPLID, " +
                                            "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) CARNE, " +
                                            "REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+') FIRST_NAME, " +
                                            "SUBSTR(PD.FIRST_NAME,  LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))+2, LENGTH(PD.FIRST_NAME)-LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))) SECOND_NAME, " +
                                            "PD.LAST_NAME, PD.BIRTHCOUNTRY," +
                                            "PD.SECOND_LAST_NAME, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,0,9)" +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,0,9) ELSE NULL END DPI, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN SUBSTR(PN.NATIONAL_ID,12,2) " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,12,2) ELSE NULL END MUNICIPIO_CUI," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN  SUBSTR(PN.NATIONAL_ID,10,2) " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN SUBSTR(PN.NATIONAL_ID,10,2) ELSE NULL END DEPARTAMENTO_CUI," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN C.DESCR WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN C.DESCR ELSE NULL END PAIS_PASAPORTE, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "    WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_DPI, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CER' AND PN.NATIONAL_ID != ' ' THEN '1' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '2' " +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '2'" +
                                            "     WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '3' ELSE NULL END TIPO_DOCUMENTO," +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE NULL END CEDULA, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_CED, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE NULL END PASAPORTE, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '1' ELSE '0' END FLAG_PAS, " +
                                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' AND PN.NATIONAL_ID != ' ' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' AND PN.NATIONAL_ID != ' ' THEN '2' ELSE NULL END CONDMIG, " +
                                            "PPD.PHONE, " +
                                            "TO_CHAR(PD.BIRTHDATE, 'YYYY-MM-DD HH:MM:SS') BIRTHDATE, " +
                                            //"APD.DESCR CARRERA, " +
                                            "AGT.DESCR FACULTAD, " +
                                            "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE NULL END SEX, " +
                                            "CASE WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'GUATEMALA' WHEN (C.DESCR = ' ' OR C.DESCR IS NULL AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE C.DESCR END PLACE," +
                                            "CASE WHEN PD.MAR_STATUS = 'M' THEN '2' WHEN PD.MAR_STATUS = 'S' THEN '1' ELSE NULL END STATUS, " +
                                            "(select REPLACE(A1.ADDRESS1,'|' , ' ') || ' ' ||  REPLACE(A1.ADDRESS2,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) DIRECCION, " +
                                            " (select REPLACE(A1.ADDRESS3,'|' , ' ') from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) ZONA, " +
                                            "REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO, " +
                                            "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, " +
                                            "'ESTUDIANTE' PROF, " +
                                            "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = PD.EMPLID AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) EMAIL " +
                                            "FROM " +
                                            "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                            "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_COUNTRY_TBL C ON C.COUNTRY = PD.BIRTHCOUNTRY " +
                                            "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                            "AND A.EFFDT =(SELECT MAX(EFFDT) FROM SYSADM.PS_ADDRESSES A2 WHERE A.EMPLID = A2.EMPLID AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE) " +
                                            "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                            "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID AND SE.STDNT_ENRL_STATUS = 'E' AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                            "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID AND CT.STRM = SE.STRM AND CT.ACAD_CAREER = SE.ACAD_CAREER AND SE.INSTITUTION = CT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                            //"--WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                                            "WHERE PN.NATIONAL_ID ='4681531')" +
                                            "WHERE CARNE=DPI||DEPARTAMENTO_CUI||MUNICIPIO_CUI OR CARNE=PASAPORTE OR CARNE=CEDULA " +
                                            "ORDER BY 1 ASC";
                            //--4681531 PASAPORTE
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                txtInsertBI.Text = reader["INS"].ToString();
                            }
                        };
                        
                        try
                        {
                            if (String.IsNullOrEmpty(State.Text))
                                State.Text = " ";
                            if (String.IsNullOrEmpty(txtDireccion2.Text))
                                txtDireccion2.Text = " ";
                            if (String.IsNullOrEmpty(txtDireccion3.Text))
                                txtDireccion3.Text = " ";
                            //Telefono y direccion
                            cmd.Connection = con;
                            cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_DATA PPD SET PPD.PHONE = '" + txtTelefono.Text + "', PPD.STATE =  '" + State.Text + "', " +
                                "PPD.ADDRESS1 = '" + txtDireccion.Text + "', PPD.ADDRESS2 = '" + txtDireccion2.Text + "', PPD.ADDRESS3 = '" + txtDireccion3.Text + "', PPD.COUNTRY = '" + codPais + "',LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "' WHERE PPD.EMPLID = '" + UserEmplid.Text + "'";
                            cmd.ExecuteNonQuery();
                            //Numero de Telefono
                            if (!String.IsNullOrEmpty(TruePhone.Text))
                            {
                                //TruePhone.Text = "UPDATE SYSADM.PS_PERSONAL_PHONE PP SET PP.PHONE = '" + txtTelefono.Text + "' WHERE PP.EMPLID = '" + UserEmplid.Text + "' AND PP.PHONE_TYPE='HOME'";
                                cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_PHONE PP SET PP.PHONE = '" + txtTelefono.Text + "' WHERE PP.EMPLID = '" + UserEmplid.Text + "' AND PP.PHONE_TYPE='HOME'";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_PERSONAL_PHONE (EMPLID, PHONE_TYPE,COUNTRY_CODE,EXTENSION,PHONE,PREF_PHONE_FLAG) VALUES ('" + UserEmplid.Text + "', 'HOME',' ',' ',  '" + txtTelefono.Text + "', 'Y')";
                                cmd.ExecuteNonQuery();
                            }
                            //Direccion
                            if (!String.IsNullOrEmpty(TrueDir.Text))
                            {
                                cmd.CommandText = "UPDATE SYSADM.PS_ADDRESSES A SET A.STATE =  '" + State.Text + "', A.ADDRESS1 = '" + txtDireccion.Text + "', A.ADDRESS2 = '" + txtDireccion2.Text + "', A.ADDRESS3 = '" + txtDireccion3.Text + "', A.COUNTRY = '" + codPais + "', LASTUPDOPRID ='" + TextUser.Text + "',  LASTUPDDTTM ='" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "' WHERE A.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES (EMPLID, ADDRESS_TYPE,COUNTY,CITY,NUM1, NUM2, HOUSE_TYPE, ADDR_FIELD1, ADDR_FIELD2, ADDR_FIELD3,POSTAL,GEO_CODE,IN_CITY_LIMIT,ADDRESS1_AC,ADDRESS2_AC,ADDRESS3_AC,CITY_AC,REG_REGION,EFFDT,EFF_STATUS,COUNTRY,ADDRESS1,ADDRESS2,ADDRESS3,ADDRESS4,STATE,LASTUPDDTTM,LASTUPDOPRID) " +
                                    "VALUES('" + UserEmplid.Text + "', 'HOME',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'A', '" + codPais + "', '" + txtDireccion.Text + "', '" + txtDireccion2.Text + "', '" + txtDireccion3.Text + "', ' ','" + State.Text + "', '" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "','" + TextUser.Text + "')";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = cmd.CommandText = "INSERT INTO SYSADM.PS_ADDRESSES_SA (LOC_ADDR_DATA,LOC_ADDR, LOC_ADDR_LINE, LOC_ADDR_TYPE, EXT_ORG_ID, DESCR_EXT_ORG, DESCR_ORG_LOCATION, CONTACT_NAME, DATA_SOURCE, EMPLID, ADDRESS_TYPE,EFFDT,ORG_LOCATION,MAINT_ADDR_MANUAL,MAINT_OTHER_MANUAL,ORG_CONTACT,SEASONAL_ADDR) " +
                                    "VALUES(' ',' ',' ',' ',' ',' ',' ',' ',' ','" + UserEmplid.Text + "', 'HOME', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', 0, 'N','N',0,'N')";
                                cmd.ExecuteNonQuery();
                            }
                            //Estado Civil
                            cmd.CommandText = "UPDATE SYSADM.PS_PERS_DATA_EFFDT PD SET PD.MAR_STATUS = '" + ec + "' WHERE PD.EMPLID = '" + UserEmplid.Text + "'";
                            cmd.ExecuteNonQuery();

                            if (txtAInicial.Text == txtApellido.Text && txtNInicial.Text == txtNombre.Text && txtCInicial.Text == txtCasada.Text)
                            {
                                txtExiste2.Text = "NO SE MODIFICA PS_NAMES";
                            }else
                            {                                
                                //ACTUALIZAR NOMBRES
                                //txtExiste2.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = '" + txtApellido.Text + " " + txtCasada.Text + "," + txtNombre.Text + "', PN.LAST_NAME_SRCH =REPLACE(UPPER('" + txtApellido.Text + "'),' ',''), PN.FIRST_NAME_SRCH=REPLACE(UPPER('" + txtNombre.Text + "'),' ',''), LAST_NAME ='" + txtApellido.Text + "', FIRST_NAME='" + txtNombre.Text + "', SECOND_LAST_NAME='" + txtCasada.Text + "', SECOND_LAST_SRCH=REPLACE(UPPER('" + txtCasada.Text + "'),' ',''), NAME_DISPLAY='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_FORMAL='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_DISPLAY_SRCH =UPPER(REPLACE('" + txtNombre.Text + txtApellido.Text + txtCasada.Text + "',' ',''))  WHERE PN.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.CommandText = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = '" + txtApellido.Text + " " + txtCasada.Text + "," + txtNombre.Text + "', PN.LAST_NAME_SRCH =REPLACE(UPPER('" + txtApellido.Text + "'),' ',''), PN.FIRST_NAME_SRCH=REPLACE(UPPER('" + txtNombre.Text + "'),' ',''), LAST_NAME ='" + txtApellido.Text + "', FIRST_NAME='" + txtNombre.Text + "', SECOND_LAST_NAME='" + txtCasada.Text + "', SECOND_LAST_SRCH=REPLACE(UPPER('" + txtCasada.Text + "'),' ',''), NAME_DISPLAY='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_FORMAL='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_DISPLAY_SRCH =UPPER(REPLACE('" + txtNombre.Text + txtApellido.Text + txtCasada.Text + "',' ',''))  WHERE PN.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                            }

                            if (!txtInsert.Text.IsNullOrWhiteSpace())
                            {
                                cmd.CommandText = txtInsert.Text;
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            con.Close();
                            mensaje = "Su información fue actualizada correctamente";
                        }
                        catch (Exception x)
                        {
                            transaction.Rollback();
                            mensaje = "Ocurrió un problema al actualizar su información " + x;
                            controlOracle = "1";
                        }
                        
                    }
                }
                if (RegistroCarne == "0" && controlOracle == "0")
                {
                    using (SqlConnection conexion = new SqlConnection(TxtURLSql.Text))
                    {
                        conexion.Open();
                        txtExiste.Text = "//";
                        var trans = conexion.BeginTransaction();

                        txtExiste.Text = "/";
                        using (SqlCommand sqlCommand = new SqlCommand(txtInsertBI.Text))
                        {
                            sqlCommand.Transaction = trans;
                            txtExiste.Text = "-";
                            try
                            {
                                txtExiste.Text = "--";
                                sqlCommand.Connection = conexion;
                                sqlCommand.ExecuteNonQuery();
                                trans.Commit();
                                conexion.Close();
                            }
                            catch (Exception x)
                            {
                                txtExiste.Text = "---";
                                mensaje = x.ToString();
                                trans.Rollback();
                                conexion.Close();
                            }
                        }
                    }
                   // txtExiste.Text = "UPDATE SYSADM.PS_NAMES PN SET PN.NAME = '" + txtApellido.Text + "," + txtCasada.Text + " " + txtNombre.Text + "', PN.LAST_NAME_SRCH =REPLACE(UPPER('" + txtApellido.Text + "'),' ',''), PN.FIRST_NAME_SRCH=REPLACE(UPPER('" + txtNombre.Text + "'),' ',''), LAST_NAME ='" + txtApellido.Text + "', FIRST_NAME='" + txtNombre.Text + "', SECOND_LAST_NAME='" + txtCasada.Text + "', SECOND_LAST_SRCH=REPLACE(UPPER('" + txtCasada.Text + "'),' ',''), NAME_DISPLAY='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_FORMAL='" + txtNombre.Text + " " + txtApellido.Text + " " + txtCasada.Text + "', NAME_DISPLAY_SRCH =REPLACE('" + txtNombre.Text + txtApellido.Text + txtCasada.Text + "',' ',''),  WHERE PN.EMPLID = '" + UserEmplid.Text + "'";

                }
            }
            catch (Exception X)
            {
                mensaje = "Ocurrió un problema al actualizar su información" + X;
            }
            return mensaje;
        }

        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            string informacion = actualizarInformacion();

            if ((informacion != "No puede enviarse información vacía y es necesario seleccionar el estado civil, un país, un departamento y un muncipio" || informacion != "No puede enviarse información vacía y es necesario cargar una fotografía, seleccionar el estado civil, un país, un departamento y un muncipio") && txtNInicial.Text == txtNombre.Text && txtAInicial.Text == txtApellido.Text)
            {
                if (FileUpload1.HasFile)
                {
                    informacion = informacion + Upload();
                }
            }
            else if (txtNInicial.Text != txtNombre.Text || txtAInicial.Text != txtApellido.Text || txtCInicial.Text != txtCasada.Text)
            {
                if (FileUpload1.HasFile && FileUpload2.HasFiles)
                {
                    informacion = informacion + Upload();
                }
            }

            lblActualizacion.Text = informacion;
        }

        protected string Upload()
        {
            string mensaje = "";
            try
            {
                HttpPostedFile ArchivoCarga = FileUpload1.PostedFile;

                int TamañoArchivoCarga = ArchivoCarga.ContentLength;

                if (TamañoArchivoCarga > 1048576)  // 1GB
                {
                    //Finalziar cuando se exceda el archivo tiene un tamaño mayor a 1GB
                    return "El Archivo es muy grande";
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
                        string NombreFoto = "3682754340101";//Context.User.Identity.Name.Replace("@unis.edu.gt", ""); 
                        //string NombreFoto = Context.User.Identity.Name.Replace("@unis.edu.gt", "");

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
                                                    mensaje = " y la fotografía fue almacenada correctamente.";
                                                }
                                                else //Se registra la nueva fotografía
                                                {
                                                    cmd.CommandText = "INSERT INTO SYSADM.PS_EMPL_PHOTO VALUES ('" + EmplidFoto + "', (TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), :Fotografia)";
                                                    mensajeValidacion = "La fotografía se registró correctamente en Campus.";
                                                    mensaje = " y la fotografía fue almacenada correctamente.";
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
                                mensaje = " y la fotografía no fue almacenada porque no tiene se encuentra registrado el ID de usuario";
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
                            mensaje = " y la fotografía no fue almacenada porque no tiene el formato correcto.";
                        }
                    }
                }
                else
                {
                    mensaje = ", no se encontró ninguna fotografía para almacenar.";
                }

                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "-----------------------------------------------------------------------------------------------");
                GuardarBitacora(ArchivoBitacora, "Total de archivos: " + ContadorArchivos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos cargados correctamente: " + ContadorArchivosCorrectos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos con error: " + ContadorArchivosConError.ToString());

            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                mensaje = ". Ocurrió un error al cargar la imagen";
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

        protected void CmbPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenadoDepartamento();
            llenadoMunicipio();
            llenadoState();
        }
        public string divisionApellidos(string apellido)
        {
            WebClient _clientW = new WebClient();
            _clientW.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            _clientW.Headers.Add("apellido", apellido);
            string json = _clientW.DownloadString("https://apexdes.unis.edu.gt:8443/ords/unis_interfaces/Centralizador/ExcepcionesApellidos");
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
    }
}