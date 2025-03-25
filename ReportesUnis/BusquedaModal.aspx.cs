using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class BusquedaModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void consultaNombre(string NombreBusqueda)
        {
            NombreBusqueda = NombreBusqueda.Replace(" ", "%");
            string constr = "User ID =DESA_PTRES;Password=D3s@_PmT22;Data Source=10.11.0.36/DBCSDESA_PDB1.subnet1.vcnpruebas.oraclevcn.com";
            string NOMBRE = "";
            string EMPLID = "";
            using (OracleConnection con = new OracleConnection(constr))
            {
               con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT EMPLID, NAME FROM SYSADM.PS_PERSONAL_VW " +
                    "WHERE NAME LIKE '%"+NombreBusqueda+"%'";
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    /*while (reader.Read())
                    {
                        EMPLID = reader["EMPLID"].ToString();
                        NOMBRE = reader["NAME"].ToString();
                    }*/
                    if (reader.HasRows)
                    {
                        GridViewBusqueda.DataSource = cmd.ExecuteReader();
                        GridViewBusqueda.DataBind();
                    }
                }
            }
        }

        private void LoadData()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("EMPLID");
            dt.Columns.Add("NAME");

            dr["EMPLID"] = String.Empty;
            dr["NAME"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewBusqueda.DataSource = dt;
            this.GridViewBusqueda.DataBind();
        }

        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (CmbBusqueda.Text.Equals("Nombre"))
            {
                LoadData();
                consultaNombre(TxtBusqueda.Text);
                string script = "<script>Busqueda();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "FuncionJavaScript", script);

            }
        }
    }
}