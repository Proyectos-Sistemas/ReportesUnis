using DocumentFormat.OpenXml.Office.Word;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ReportesUnis
{
    public partial class MantenimientoPantallas : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            LeerInfoTxt();
            Buscar();
        }

        private void LoadData()
        {
            GridViewReporte.HeaderStyle.HorizontalAlign= HorizontalAlign.Center;
            GridViewReporte.RowStyle.HorizontalAlign= HorizontalAlign.Center;
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
            string constr = TxtURL.Text;
            int contador = 0;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                OracleTransaction transaction;
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT COUNT(*) AS CONTADOR FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='"+DTInicio.Text+"' AND FECHA_FIN='"+DTFin.Text+"' AND PANTALLA ='"+CmbTipo.Text+"'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            contador = Convert.ToInt32(reader["CONTADOR"]);
                        }

                        if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)) && contador == 0)
                        {
                            if (Convert.ToDateTime(DTInicio.Text) < Convert.ToDateTime(DTFin.Text))
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "INSERT INTO UNIS_INTERFACES.TBL_PANTALLA_CARNE (FECHA_INICIO, FECHA_FIN, PANTALLA) VALUES('" + DTInicio.Text + "','" + DTFin.Text + "','" + CmbTipo.Text + "')";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                Buscar();
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
                        cmd.CommandText = "SELECT ID_REGISTRO FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='" + DTInicio.Text + "' AND FECHA_FIN='" + DTFin.Text + "' AND PANTALLA ='" + CmbTipo.Text + "'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ID = Convert.ToInt32(reader["ID_REGISTRO"]);
                        }

                        if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)) && ID != 30000)
                        {
                            if (Convert.ToDateTime(DTInicio.Text) < Convert.ToDateTime(DTFin.Text))
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "UPDATE UNIS_INTERFACES.TBL_PANTALLA_CARNE SET " +
                                    "FECHA_INICIO = '" + DTInicio.Text +"'," +
                                    "FECHA_FIN = '" + DTFin.Text + "'," +
                                    "PANTALLA = '" + CmbTipo.Text + "' " +
                                    "WHERE ID_REGISTRO = "+ID;
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                Buscar();
                            }
                            else
                            {
                                lblActualizacion.Text = "La fecha de inicio debe de ser menor a la final.";

                            }
                        }
                        else
                        {
                            lblActualizacion.Text = "Valide los datos ingresados para actualizar.";
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
                        cmd.CommandText = "SELECT ID_REGISTRO FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE FECHA_INICIO='" + DTInicio.Text + "' AND FECHA_FIN='" + DTFin.Text + "' AND PANTALLA ='" + CmbTipo.Text + "'";
                        cmd.Connection = con;

                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ID = Convert.ToInt32(reader["ID_REGISTRO"]);
                        }

                        if ((!String.IsNullOrEmpty(DTInicio.Text)) && (!String.IsNullOrEmpty(DTFin.Text)) && ID != 30000)
                        {
                            if (Convert.ToDateTime(DTInicio.Text) < Convert.ToDateTime(DTFin.Text))
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "DELETE FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE WHERE ID_REGISTRO = "+ID;
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                                Buscar();
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
                        TXTINICIO.Text = "DELETE FROM UNIS_INTERFACES.TBL_PANTALLA_CARNE SET WHERE ID_REGISTRO = " + ID+"----" + x;
                    }

                }
            }
        }

        protected void BtnInsertar_Click(object sender, EventArgs e)
        {
            Insertar();
            //TXTINICIO.Text = "INSERT INTO UNIS_INTERFACES.TBL_PANTALLA_CARNE (FECHA_INICIO, FECHA_FIN, PANTALLA) VALUES('" + DTInicio.Text + "','" + DTFin.Text + "','" + CmbTipo.Text + "')";
        }
        
        protected void BtnActualizar_Click(object sender, EventArgs e)
        {
            Actualizar();
            //TXTINICIO.Text = "INSERT INTO UNIS_INTERFACES.TBL_PANTALLA_CARNE (FECHA_INICIO, FECHA_FIN, PANTALLA) VALUES('" + DTInicio.Text + "','" + DTFin.Text + "','" + CmbTipo.Text + "')";
        }

        protected void BtnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar();
        }

        protected void DTInicio_TextChanged(object sender, EventArgs e)
        {
            DateTime inicio = Convert.ToDateTime(DTInicio.Text).AddDays(4);
            DTFin.Text = Convert.ToDateTime(DTInicio.Text).AddDays(4).ToString("yyyy-MM-dd");
        }
    }
}