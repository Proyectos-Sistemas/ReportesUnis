﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace ReportesUnis
{
    public partial class ActualizacionEstudiantes : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            TextUser.Text = Context.User.Identity.Name.Replace("@unis.edu.gt", "");
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("DATOS_FOTOGRAFIAS") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LeerInfoTxt();
                llenadoPais();
                mostrarInformación();
                llenadoDepartamento();
                llenadoState();
                if (String.IsNullOrEmpty(txtCarne.Text))
                {
                    BtnActualizar.Visible = false;
                    lblActualizacion.Text = "No se encontró información";
                    CmbPais.SelectedValue = "Guatemala";
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
        private void mostrarInformación()
        {
            string constr = TxtURL.Text;
            var dia = "";
            var mes = "";
            var anio = "";
            var bday = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT PAIS, EMPLID,FIRST_NAME,LAST_NAME,CARNE,PHONE,DPI,CARRERA,FACULTAD,STATUS,BIRTHDATE,DIRECCION,DIRECCION2,DIRECCION3,MUNICIPIO,DEPARTAMENTO, CNT FROM (" +
                   "SELECT PD.EMPLID, PN.NATIONAL_ID CARNE,  PD.FIRST_NAME, " +
                   "PD.LAST_NAME|| ' ' || PD.SECOND_LAST_NAME LAST_NAME, PN.NATIONAL_ID DPI, PN.NATIONAL_ID_TYPE, PPD.PHONE , " +
                   "TO_CHAR(PD.BIRTHDATE,'YYYY-MM-DD') BIRTHDATE, " +
                   "APD.DESCR CARRERA, AGT.DESCR FACULTAD, " +
                   "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'Sin Información' END STATUS, " +
                   "A.ADDRESS1 DIRECCION, A.ADDRESS2 DIRECCION2, A.ADDRESS3 DIRECCION3, " +
                   "REGEXP_SUBSTR(ST.DESCR,'[^-]+') MUNICIPIO, SUBSTR(ST.DESCR,(INSTR(ST.DESCR,'-')+1)) DEPARTAMENTO, ST.STATE, " +
                   "TT.TERM_BEGIN_DT, ROW_NUMBER() OVER (PARTITION BY PD.EMPLID ORDER BY 18 DESC) CNT, C.DESCR PAIS " +
                   "FROM SYSADM.PS_PERS_DATA_SA_VW PD " +
                   "JOIN SYSADM.PS_PERS_NID PN ON  PD.EMPLID = PN.EMPLID " +
                   "JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                   "JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                   "JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                   "JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID " +
                   "JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON CT.ACAD_GROUP_ADVIS = AGT.ACAD_GROUP " +
                   "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID " +
                   "JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                   "JOIN SYSADM.PS_ACAD_PROG AP ON PD.EMPLID = AP.EMPLID " +
                   "JOIN SYSADM.PS_ACAD_PROG_TBL APD ON AP.ACAD_PROG = APD.ACAD_PROG " +
                   "JOIN SYSADM.PS_COUNTRY_TBL C ON A.COUNTRY = C.COUNTRY " +
                   //"WHERE PN.NATIONAL_ID ='" + TextUser.Text + "' " +
                   "WHERE PN.NATIONAL_ID ='2226708940101' " +
                   ") WHERE CNT = 1";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtCarne.Text = reader["CARNE"].ToString();
                        txtNombre.Text = reader["FIRST_NAME"].ToString();
                        txtApellido.Text = reader["LAST_NAME"].ToString();
                        txtDPI.Text = reader["DPI"].ToString();
                        CmbEstado.SelectedValue = reader["STATUS"].ToString();

                        bday = reader["BIRTHDATE"].ToString();
                        anio = bday.Substring(0, 4);
                        mes = bday.Substring(5, 2);
                        dia = bday.Substring(8, 2);
                        txtCumple.Text = dia + "-" + mes + "-" + anio;

                        txtDireccion.Text = reader["DIRECCION"].ToString();
                        txtDireccion2.Text = reader["DIRECCION2"].ToString();
                        txtDireccion3.Text = reader["DIRECCION3"].ToString();
                        CmbDepartamento.SelectedValue = reader["DEPARTAMENTO"].ToString();
                        llenadoMunicipio();
                        CmbMunicipio.SelectedValue = reader["MUNICIPIO"].ToString();
                        CmbPais.SelectedValue = reader["PAIS"].ToString();
                        txtTelefono.Text = reader["PHONE"].ToString();
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
                    catch (Exception x)
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
                        "WHERE COUNTRY ='GTM' AND REGEXP_SUBSTR(ST.DESCR,'[^-]+') IS NOT NULL AND DESCR LIKE ('%-" + CmbDepartamento.SelectedValue + "') " +
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
                    cmd.CommandText = "SELECT DESCR AS PAIS, COUNTRY FROM SYSADM.PS_COUNTRY_TBL " + where + " ORDER BY PAIS";
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
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT STATE FROM SYSADM.PS_STATE_TBL " +
                            "WHERE DESCR ='" + CmbMunicipio.SelectedValue + "-" + CmbDepartamento.SelectedValue + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            State.Text = reader["STATE"].ToString();
                        }
                        con.Close();
                    }else
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
                VALOR = "";
            }
            return VALOR;
        }
        private string[] direccio()
        {
            string[] direccion = new string[4];
            if (txtDireccion.Text.Length <= 54)
            {
                direccion[0] = txtDireccion.Text.Substring(0, txtDireccion.Text.Length);
                direccion[1] = " ";
                direccion[2] = " ";
                direccion[3] = " ";
            }
            else if (txtDireccion.Text.Length >= 55 && txtDireccion.Text.Length < 110)
            {
                direccion[0] = txtDireccion.Text.Substring(0, 54);
                direccion[1] = txtDireccion.Text.Substring(55, txtDireccion.Text.Length - 55);
                direccion[2] = " ";
                direccion[3] = " ";
            }
            else if (txtDireccion.Text.Length >= 110 && txtDireccion.Text.Length < 165)
            {
                direccion[0] = txtDireccion.Text.Substring(0, 54);
                direccion[1] = txtDireccion.Text.Substring(55, txtDireccion.Text.Length - 55);
                direccion[2] = txtDireccion.Text.Substring(110, txtDireccion.Text.Length - 110);
                direccion[3] = " ";
            }
            else if (txtDireccion.Text.Length >= 165)
            {
                direccion[0] = txtDireccion.Text.Substring(0, 54);
                direccion[1] = txtDireccion.Text.Substring(54, 54);
                direccion[2] = txtDireccion.Text.Substring(108, 54);
                direccion[3] = txtDireccion.Text.Substring(162, txtDireccion.Text.Length - 165);
            }
            return direccion;
        }
        private void actualizarInformacion()
        {
            if (!String.IsNullOrEmpty(txtDireccion.Text) || !String.IsNullOrEmpty(txtTelefono.Text))
            {
                try
                {
                    string constr = TxtURL.Text;
                    string codPais = "";
                    string ec = estadoCivil();
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

                            try
                            {
                                //Numero de Telefono
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE SYSADM.PS_PERSONAL_DATA PPD SET PPD.PHONE = '" + txtTelefono.Text + "', PPD.STATE =  '" + State.Text + "', " +
                                    "PPD.ADDRESS1 = '" + txtDireccion.Text + "', PPD.ADDRESS2 = '" + txtDireccion2.Text + "', PPD.ADDRESS3 = '" + txtDireccion3.Text + "', PPD.COUNTRY = '" + codPais + "' WHERE PPD.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                                //Direccion
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE SYSADM.PS_ADDRESSES A SET A.STATE =  '" + State.Text + "', " +
                                    "A.ADDRESS1 = '" + txtDireccion.Text + "', A.ADDRESS2 = '" + txtDireccion2.Text + "', A.ADDRESS3 = '" + txtDireccion3.Text + "', A.COUNTRY = '" + codPais + "' WHERE A.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                                //Estado Civil
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE SYSADM.PS_PERS_DATA_EFFDT PD SET PD.MAR_STATUS = '" + ec + "' WHERE PD.EMPLID = '" + UserEmplid.Text + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                            }
                        }
                    }

                    lblActualizacion.Text = "Su información fue actualizada correctamente";
                }
                catch (Exception x)
                {
                    lblActualizacion.Text = "Ocurrió un problema al actualizar su información";
                }
            }
            else
                lblActualizacion.Text = "No puede enviarse información vacía";
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

        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            actualizarInformacion();
            Upload();
        }

        protected void Upload()
        {
            try
            {
                HttpPostedFile ArchivoCarga = FileUpload1.PostedFile;

                int TamañoArchivoCarga = ArchivoCarga.ContentLength;

                if (TamañoArchivoCarga > 1048576)  // 1GB
                {
                    //Finalziar cuando se exceda el archivo tiene un tamaño mayor a 1GB
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
                        string NombreFoto = "2226708940101";//Context.User.Identity.Name.Replace("@unis.edu.gt", ""); 

                        if (ExtensionesPermitidas.Contains(ExtensionFotografia))
                        {
                            EmplidFoto = "";
                            EmplidExisteFoto = "";

                            //FileUpload1.SaveAs(uploadFolder + uploadedFile.FileName); //Guarda archivo en el servidor

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
                                                    lblActualizacion.Text = lblActualizacion.Text + " y la fotografía fue almacenada correctamente.";
                                                }
                                                else //Se registra la nueva fotografía
                                                {
                                                    cmd.CommandText = "INSERT INTO SYSADM.PS_EMPL_PHOTO VALUES ('" + EmplidFoto + "', (TO_NUMBER((TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD'), 'YYYY-MM-DD') - TO_DATE(TO_CHAR('1999-12-31'), 'YYYY-MM-DD'))* 86400) + TO_NUMBER(TO_CHAR(SYSTIMESTAMP,'hh24missff2'))), :Fotografia)";
                                                    //query = ":Emplid, :Fotografia)";
                                                    mensajeValidacion = "La fotografía se registró correctamente en Campus.";
                                                    lblActualizacion.Text = lblActualizacion.Text + " y la fotografía fue almacenada correctamente.";
                                                }

                                                cmd.Connection = con;
                                                //  cmd.Parameters.Add(new OracleParameter("Emplid", EmplidFoto));
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
                                lblActualizacion.Text = lblActualizacion.Text + " pero la fotografía no fue almacenada correctamente.";
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
                            lblActualizacion.Text = lblActualizacion.Text + " y la fotografía fue almacenada correctamente.";
                        }
                    }
                }

                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "");
                GuardarBitacora(ArchivoBitacora, "-----------------------------------------------------------------------------------------------");
                GuardarBitacora(ArchivoBitacora, "Total de archivos: " + ContadorArchivos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos cargados correctamente: " + ContadorArchivosCorrectos.ToString());
                GuardarBitacora(ArchivoBitacora, "Archivos con error: " + ContadorArchivosConError.ToString());
                Response.Redirect(Request.Url.AbsoluteUri);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
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
    }
}