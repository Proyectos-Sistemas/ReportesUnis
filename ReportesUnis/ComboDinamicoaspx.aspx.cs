using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class ComboDinamicoaspx : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LlenarAlergias();

            }
        }

        protected void LlenarAlergias()
        {
            string constr = "User ID =DESA_PTRES;Password=D3s@_PmT22;Data Source=129.213.95.39/DBCSDESA_PDB1.subnet1.vcnpruebas.oraclevcn.com";


            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' FIELDNAME FROM DUAL UNION SELECT FIELDNAME FROM SYSADM.PS_XL_CAT_ALERGIAS ORDER BY 1 ASC";
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListItem item = new ListItem();
                            item.Text = reader["FIELDNAME"].ToString();
                            item.Value = reader["FIELDNAME"].ToString();
                            CmbAlergias.Items.Add(item);
                        }
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            List<string> selectedValues = new List<string>();
           
            // Recorrer los items del DropDownList y agregar los seleccionados a la lista
            foreach (ListItem item in CmbAlergias.Items)
            {
                if (item.Selected)
                {
                    selectedValues.Add(item.Value);
                }
            }

            // Verificar si se han seleccionado opciones
            if (selectedValues.Count > 0)
            {
                // Procesar los valores seleccionados
                string seleccionados = string.Join(",", selectedValues);
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Valores seleccionados: {seleccionados}');", true);
            }
            else
            {
                // Mostrar mensaje si no se selecciona ninguna opción
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Por favor selecciona al menos una opción.');", true);
            }
        }
    }
}
