using Microsoft.Win32;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class AccesosFacultad_ActulizacionGeneral : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private List<int> _filasSeleccionadas = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LeerInfoTxt();
                LoadData();
                llenadoGrid("");
            }
        }

        //FUNCIONES
        private void LoadData()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("NOMBRE");
            dt.Columns.Add("DPI");
            dt.Columns.Add("COD_FACULTAD");
            dt.Columns.Add("FECHA_REGISTRO");

            dr["NOMBRE"] = String.Empty;
            dr["DPI"] = String.Empty;
            dr["COD_FACULTAD"] = String.Empty;
            dr["FECHA_REGISTRO"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewInformación.DataSource = dt;
            this.GridViewInformación.DataBind();
        }
        private void LoadDataBusqueda()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("NAME");
            dt.Columns.Add("DPI");
            dt.Columns.Add("COD_FACULTAD");

            dr["NAME"] = String.Empty;
            dr["DPI"] = String.Empty;
            dr["COD_FACULTAD"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewBusqueda.DataSource = dt;
            this.GridViewBusqueda.DataBind();
        }
        private string llenadoGrid(string where)
        {
            string existe = "0";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT DPI, NOMBRE, COD_FACULTAD, FECHA_REGISTRO FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                        where +
                        " ORDER BY FECHA_REGISTRO DESC";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewInformación.DataSource = cmd.ExecuteReader();
                        GridViewInformación.DataBind();
                    }
                    reader.Close();

                    if (!String.IsNullOrEmpty(where))
                    {
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                            where +
                            " ORDER BY FECHA_REGISTRO DESC";
                        cmd.Connection = con;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            existe = reader["CONTADOR"].ToString();
                        }
                        reader.Close();
                    }
                }
            }

            return existe;
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
        public void llenadoFacultad(DropDownList ddl)
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand("SELECT ACAD_GROUP||' - '||DESCR DESCRIPCION, ACAD_GROUP CODIGO FROM SYSADM.PS_ACAD_GROUP_TBL ORDER BY 1", con))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Seleccione una facultad a asignar", "")); // Elemento predeterminado

                        while (reader.Read())
                        {
                            string descripcion = reader["DESCRIPCION"].ToString();
                            string codigo = reader["CODIGO"].ToString();
                            ddl.Items.Add(new ListItem(descripcion, codigo));
                        }
                    }
                }
            }
        }
        protected void GridViewBusqueda_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlFacultades = (DropDownList)e.Row.FindControl("CmbFacultades");
                if (ddlFacultades != null)
                {
                    llenadoFacultad(ddlFacultades);
                }
            }
        }
        public string consultaNombre(string NombreBusqueda)
        {
            NombreBusqueda = NombreBusqueda.Replace(" ", "%");
            string control = "0";
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = " SELECT PN.NATIONAL_ID DPI, PV.NAME,PN.NATIONAL_ID_TYPE  " +
                            "FROM SYSADM.PS_PERS_NID PN " +
                            "INNER JOIN SYSADM.PS_PERSONAL_VW PV ON PN.EMPLID = PV.EMPLID " +
                            "WHERE PN.NATIONAL_ID_TYPE IN ('DPI','PAS') " +
                            "AND NAME LIKE '%" + NombreBusqueda + "%'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                        control = "0";
                    }
                    else
                    {
                        control = "1";
                    }
                }
            }
            return control;
        }
        public string consultarDocumento(string Documento)
        {
            string constr = TxtURL.Text;
            string control = "0";
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = " SELECT PN.NATIONAL_ID DPI, PV.NAME,PN.NATIONAL_ID_TYPE  " +
                            "FROM SYSADM.PS_PERS_NID PN " +
                            "INNER JOIN SYSADM.PS_PERSONAL_VW PV ON PN.EMPLID = PV.EMPLID " +
                            "WHERE PN.NATIONAL_ID_TYPE IN ('DPI','PAS') " +
                            "AND PN.NATIONAL_ID =  '" + Documento + "' " +
                            "ORDER BY PN.NATIONAL_ID_TYPE FETCH FIRST 1 ROWS ONLY";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                        control = "0";
                    }
                    else
                    {
                        control = "1";
                    }
                }
            }
            return control;
        }
        public string agregarRegistro(string facultad, string dpi, string nombre)
        {
            string constr = TxtURL.Text;
            int contador = 0;
            int registro = 0;
            string control = null;

            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR " +
                            "FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                            "WHERE COD_FACULTAD = '" + facultad + "' " +
                            "AND DPI = '" + dpi + "' ";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            contador = Convert.ToInt32(reader["CONTADOR"]);
                        }

                        cmd.CommandText = "SELECT ID_REGISTRO " +
                            "FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                            "ORDER BY 1 DESC " +
                            "FETCH FIRST 1 ROWS ONLY";
                        cmd.Connection = con;

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            registro = Convert.ToInt32(reader["ID_REGISTRO"]);
                        }

                        registro = registro + 1;

                        if (contador == 0)
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                                "(ID_REGISTRO,DPI, COD_FACULTAD, FECHA_REGISTRO, NOMBRE) " +
                                "VALUES(" + registro + ",'" + dpi + "','" + facultad + "', SYSDATE ,'" + nombre + "')";
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            control = "0";
                        }
                    }
                    catch (Exception X)
                    {
                        transaction.Rollback();
                        control = "1";
                    }
                }
            }
            return control;
        }
        public string eliminarRegistro()
        {
            string constr = TxtURL.Text;
            string resultado = null;

            foreach (GridViewRow row in GridViewInformación.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxRegistro");

                if (checkBox != null && checkBox.Checked)
                {
                    string dpi = row.Cells[2].Text;
                    string facultad = row.Cells[3].Text;

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
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_PERMISOS_ACT_CARNET " +
                                "WHERE DPI = '" + dpi + "' " +
                                "AND COD_FACULTAD = '" + facultad + "'";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                resultado = "0";
                            }
                            catch (Exception X)
                            {
                                transaction.Rollback();
                                resultado = "1";
                            }
                        }
                    }
                }
            }
            return resultado;
        }

        //EVENTOS
        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            string resultado = null;
            if (!String.IsNullOrEmpty(TxtBusqueda.Text))
            {
                if (CmbBusqueda.Text.Equals("Nombre"))
                {
                    resultado = llenadoGrid("WHERE UPPER(NOMBRE) LIKE '%" + TxtBusqueda.Text.Replace(" ", "%").ToUpper() + "%'");
                    if (resultado == "0")
                    {
                        string script = "<script>NoExiste();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        BtnLimpiarBusqueda.Enabled = false;
                        BtnBuscar.Enabled = true;
                    }
                }

                if (CmbBusqueda.Text.Equals("Documento de Identificación"))
                {
                    resultado = llenadoGrid("WHERE DPI LIKE '%" + TxtBusqueda.Text.Trim() + "%'");
                    if (resultado == "0")
                    {
                        string script = "<script>NoExiste();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        BtnLimpiarBusqueda.Enabled = false;
                        BtnBuscar.Enabled = true;
                    }
                }

                if (CmbBusqueda.Text.Equals("Facultad"))
                {
                    resultado = llenadoGrid("WHERE COD_FACULTAD LIKE '%" + TxtBusqueda.Text.Trim().ToUpper() + "%'");
                    if (resultado == "0")
                    {
                        string script = "<script>NoExiste();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                        BtnLimpiarBusqueda.Enabled = false;
                        BtnBuscar.Enabled = true;
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Por favor, ingrese un dato para realizar la busqueda');", true);
            }

            BtnBuscar.Enabled = false;
            TxtBusqueda.Enabled = false;
            BtnLimpiarBusqueda.Enabled = true;
        }
        protected void BtnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            BtnLimpiarBusqueda.Enabled = false;
            BtnBuscar.Enabled = true;
            TxtBusqueda.Text = "";
        }
        protected void BtnNuevo_Click(object sender, EventArgs e)
        {
            string existe = string.Empty;
            if (!String.IsNullOrEmpty(TxtBusqueda.Text))
            {
                LoadDataBusqueda();
                if (CmbBusqueda.Text.Equals("Facultad"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No es posible ingesar información a partir de una facultad, unicamente por nombre o documento de identifiación.');", true);
                }
                if (CmbBusqueda.Text.Equals("Nombre"))
                {
                    existe = consultaNombre(TxtBusqueda.Text);
                    if (existe == "0")
                    {
                        string script = "<script>Busqueda();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                    }
                    else
                    {
                        string script = "<script>NoExiste();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                    }
                }
                if (CmbBusqueda.Text.Equals("Documento de Identificación"))
                {
                    existe = consultarDocumento(TxtBusqueda.Text);
                    if (existe == "0")
                    {
                        string script = "<script>Busqueda();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                    }
                    else
                    {
                        string script = "<script>NoExiste();</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Es necesario ingresar datos para generar un nuevo registro.');", true);
            }
        }
        protected void BtnEliminar_Click(object sender, EventArgs e)
        {
            string resultado = null;
            resultado = eliminarRegistro();
            if (resultado == "0")
            {
                string script = "<script>Eliminado();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
            }
            else
            {
                string script = "<script>mostrarModalError();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
            }
        }
        protected void BtnAgregar_Click(object sender, EventArgs e)
        {
            bool radioButtonSelected = false;
            string dpi = null;
            string nombre = null;
            string facultad = null;
            string resultado = null;
            foreach (GridViewRow row in GridViewBusqueda.Rows)
            {
                RadioButton rb = (RadioButton)row.FindControl("RBBusqueda");

                if (rb != null && rb.Checked)
                {
                    radioButtonSelected = true;
                    dpi = row.Cells[1].Text;
                    nombre = HttpUtility.HtmlDecode(row.Cells[2].Text);
                    DropDownList ddlFacultades = (DropDownList)row.FindControl("CmbFacultades");
                    if (ddlFacultades != null)
                    {
                        facultad = ddlFacultades.SelectedValue;
                    }
                    break;
                }
            }

            resultado = agregarRegistro(facultad, dpi, nombre);

            if (resultado == "0")
            {
                string script = "<script>Agregado();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
            }
            else
            {
                string script = "<script>mostrarModalError();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);
            }

        }

        protected void GridViewInformación_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox checkBox = (CheckBox)e.Row.FindControl("CheckBoxRegistro");
                if (checkBox != null)
                {
                    checkBox.AutoPostBack = false; // Desactivar el postback automático
                    checkBox.Attributes.Add("onclick", "javascript:UpdateCheckBoxState(this);"); // Agregar un evento onclick que llama a una función JavaScript
                }
            }
        }

        public static class CheckBoxStateManager
        {
            private static Dictionary<string, bool> checkBoxStates = new Dictionary<string, bool>();

            public static void UpdateCheckBoxState(string checkBoxStateKey, bool checkBoxState)
            {
                checkBoxStates[checkBoxStateKey] = checkBoxState;
            }

            public static bool GetCheckBoxState(string checkBoxStateKey)
            {
                return checkBoxStates.ContainsKey(checkBoxStateKey) ? checkBoxStates[checkBoxStateKey] : false;
            }
        }

        [WebMethod]
        public static void UpdateCheckBoxState(string checkBoxStateKey, bool checkBoxState)
        {
            CheckBoxStateManager.UpdateCheckBoxState(checkBoxStateKey, checkBoxState);
        }
    }
}