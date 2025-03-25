using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class MantenimientoPantallas : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Grupos"] is null || (!((List<string>)Session["Grupos"]).Contains("ACCESO_CARNETIZACION") && !((List<string>)Session["Grupos"]).Contains("RLI_Admin")))
            {
                Response.Redirect(@"~/Default.aspx");
            }
            if (!IsPostBack)
            {
                LoadData();
                LeerInfoTxt();
                Buscar();
            }
        }

        private void LoadData()
        {
            GridViewReporte.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            GridViewReporte.RowStyle.HorizontalAlign = HorizontalAlign.Center;
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("FECHA_INICIO");
            dt.Columns.Add("FECHA_FIN");
            dt.Columns.Add("PANTALLA");

            dr["FECHA_INICIO"] = String.Empty;
            dr["FECHA_FIN"] = String.Empty;
            dr["PANTALLA"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewReporte.DataSource = dt;
            this.GridViewReporte.DataBind();
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

        private void Buscar()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT * FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE";
                    cmd.Connection = con;
                    con.Open();

                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GridViewReporte.DataSource = reader;
                        GridViewReporte.DataBind();
                    }
                }
            }
        }

        private void Insertar()
        {
            lblActualizacion.Text = "";
            string constr = TxtURL.Text;
            int contador = 0;
            divActualizar.Visible = false;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='" + DTInicio.Text + "' AND FECHA_FIN='" + DTFin.Text + "' AND PANTALLA ='" + CmbTipo.Text + "'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            contador = Convert.ToInt32(reader["CONTADOR"]);
                        }
                        if(!String.IsNullOrEmpty(DTInicio.Text) && (!String.IsNullOrEmpty(DTFin.Text))){

                            if (contador == 0)
                            {
                                if (Convert.ToDateTime(DTInicio.Text) < Convert.ToDateTime(DTFin.Text))
                                {
                                    try
                                    {
                                        cmd.Connection = con;
                                        cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_PANTALLA_CARNE (FECHA_INICIO, FECHA_FIN, PANTALLA) VALUES('" + DTInicio.Text + "','" + DTFin.Text + "','" + CmbTipo.Text + "')";
                                        cmd.ExecuteNonQuery();
                                        transaction.Commit();
                                        con.Close();
                                        Buscar();
                                        lblActualizacion.Text = "La información fue almacenada correctamente.";
                                    }
                                    catch (Exception)
                                    {
                                        lblActualizacion.Text = "No se pudo insertar la información a causa de un error interno.";
                                        transaction.Rollback();
                                    }
                                }
                                else
                                {
                                    lblActualizacion.Text = "La fecha de inicio debe de ser menor a la final.";

                                }
                            }
                            else
                            {
                                lblActualizacion.Text = "Las fechas ya han sido registradas para ese tipo";
                            }
                        }
                        else
                        {
                            lblActualizacion.Text = "Es necesario ingresar fechas para poder realizar la inserción.";
                        }
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        TXTINICIO.Text = TXTINICIO.Text + x;
                    }
                }
            }
        }

        public void Actualizar()
        {
            lblActualizacion.Text = "";
            string constr = TxtURL.Text;
            int ID = 30000;
            divActualizar.Visible = false;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT ID_REGISTRO FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='" + DTInicio.Text + "' AND FECHA_FIN='" + DTFin.Text + "' AND PANTALLA ='" + CmbTipo.Text + "'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ID = Convert.ToInt32(reader["ID_REGISTRO"]);
                        }

                        if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)) && ID != 30000 && ID != 0)
                        {
                            if (Convert.ToDateTime(DTNInicio.Text) < Convert.ToDateTime(DTNFin.Text))
                            {
                                try
                                {
                                    cmd.Connection = con;
                                    cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_PANTALLA_CARNE SET " +
                                        "FECHA_INICIO = '" + DTNInicio.Text + "'," +
                                        "FECHA_FIN = '" + DTNFin.Text + "'," +
                                        "PANTALLA = '" + CmbTipo.Text + "' " +
                                        "WHERE ID_REGISTRO = " + ID;
                                    cmd.ExecuteNonQuery();
                                    transaction.Commit();
                                    con.Close();
                                    Buscar();
                                    lblActualizacion.Text = "La información se actualizó correctamente.";
                                    DTInicio.Text = null; 
                                    DTFin.Text =null; 
                                    DTNFin.Text =null; 
                                    DTNInicio.Text =null;
                                }
                                catch (Exception)
                                {
                                    lblActualizacion.Text = "No se pudo actualizar la información a causa de un error interno.";
                                    transaction.Rollback();
                                }
                            }
                            else
                            {
                                lblActualizacion.Text = "La fecha de inicio debe de ser menor a la final.";
                                DTNFin.Text = null;
                                DTNInicio.Text = null;
                            }
                        }
                        else
                        {
                            lblActualizacion.Text = "Valide los datos ingresados para actualizar.";
                            DTNFin.Text = null;
                            DTNInicio.Text = null;
                        }
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        TXTINICIO.Text = TXTINICIO.Text + x;
                    }

                }
            }
        }
        public void Eliminar()
        {
            lblActualizacion.Text = "";
            string constr = TxtURL.Text;
            int ID = 30000;
            divActualizar.Visible = false;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT ID_REGISTRO FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='" + DTInicio.Text + "' AND FECHA_FIN='" + DTFin.Text + "' AND PANTALLA ='" + CmbTipo.Text + "'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ID = Convert.ToInt32(reader["ID_REGISTRO"]);
                        }

                        if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)) && ID != 30000 && ID != 0)
                        {
                            if (Convert.ToDateTime(DTInicio.Text) < Convert.ToDateTime(DTFin.Text))
                            {
                                try
                                {
                                    cmd.Connection = con;
                                    cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE ID_REGISTRO = " + ID;
                                    cmd.ExecuteNonQuery();
                                    transaction.Commit();
                                    con.Close();
                                    Buscar();
                                    lblActualizacion.Text = "La información se eliminó correctamente.";
                                    DTInicio.Text = null;
                                    DTFin.Text = null;
                                }
                                catch (Exception)
                                {
                                    lblActualizacion.Text = "No se pudo eliminar la información a causa de un error interno.";
                                    transaction.Rollback();
                                }
                            }
                            else
                            {
                                lblActualizacion.Text = "La fecha de inicio debe de ser menor a la final.";
                            }
                        }
                        else
                        {
                            lblActualizacion.Text = "Valide los datos ingresados para eliminar.";
                        }
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        TXTINICIO.Text = "DELETE FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE SET WHERE ID_REGISTRO = " + ID + "----" + x;
                    }

                }
            }
        }

        protected void BtnInsertar_Click(object sender, EventArgs e)
        {
            Insertar();
        }

        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)))
            {
                lblActualizacion.Text = "";
                if (divActualizar.Visible == false)
                {
                    divActualizar.Visible = true;
                }
                else
                {
                    Actualizar();
                }
            }
            else
            {
                lblActualizacion.Text = "Por favor ingrese el rango de fecha actual de la información que desea actualizar.";
            }
        }

        protected void BtnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar();
        }

        protected void DTInicio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime inicio = Convert.ToDateTime(DTInicio.Text).AddDays(4);
                DTFin.Text = Convert.ToDateTime(DTInicio.Text).AddDays(4).ToString("yyyy-MM-dd");
                lblActualizacion.Text = "";
            }
            catch (Exception)
            {
                lblActualizacion.Text = "Intente seleccionar la fecha desde el calendario2.";
            }
        }

        protected void DTNInicio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime inicio = Convert.ToDateTime(DTNInicio.Text).AddDays(4);
                DTNFin.Text = Convert.ToDateTime(DTNInicio.Text).AddDays(4).ToString("yyyy-MM-dd");
                lblActualizacion.Text = "";
            }
            catch (Exception)
            {
                lblActualizacion.Text = "Intente seleccionar la fecha desde el calendario.";
            }
        }
    }
}