using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class ConfirmaciónDeCarne : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            LeerInfoTxt();
        }

        protected void RadioButtonConfirmar_CheckedChanged(object sender, EventArgs e)
        {
            //if (RadioButtonConfirmar.Checked)
            //{
                divConfirmar.Visible = true;
                divGenerar.Visible = false;
                Buscar();
            //}
        }

        protected void RadioButtonGenerar_CheckedChanged(object sender, EventArgs e)
        {
            //if (RadioButtonConfirmar.Checked)
            //{
                divConfirmar.Visible = false;
                divGenerar.Visible = true;
            //}
        }

        protected void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenado();
            if (!txtCantidad.Text.Equals("0"))
            {
                for (int i = 0; i < Convert.ToInt32(txtCantidad.Text); i++)
                {
                    HDocumentacion.Visible = true;
                    if (i == 0)
                    {
                        ImgDPI1.ImageUrl = "~/DPIUsuarios/" + CmbCarne.Text+"(1).jpg";
                    }
                    if (i == 1)
                    {
                        ImgDPI2.ImageUrl = "~/DPIUsuarios/" + CmbCarne.Text+"(2).jpg";
                    }
                }
            }
            else
            {
                HDocumentacion.Visible = true;
            }
        }

        private void Buscar()
        {
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET FROM DUAL UNION SELECT CARNET FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CONFIRMACION = 1";
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

        private void llenado()
        {
            
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ' ' CARNET,' ' NOMBRE1,' ' NOMBRE2,' ' APELLIDO1,' ' APELLIDO2,' ' DECASADA,' ' CARGO," +
                        "' ' FACULTAD,' ' CELULAR,' ' FECHANAC,' ' ESTADO_CIVIL,' ' DIRECCION,' ' DEPTO_RESIDENCIA,' ' MUNI_RESIDENCIA, ' ' TOTALFOTOS FROM DUAL UNION " +
                        "SELECT CARNET, NOMBRE1, NOMBRE2, APELLIDO1, APELLIDO2, DECASADA, CARGO, FACULTAD, CELULAR, FECHANAC, " +
                        "CASE WHEN ESTADO_CIVIL = 1 THEN 'SOLTERO' WHEN ESTADO_CIVIL ='2' THEN 'CASADO' ELSE '' END ESTADO_CIVIL, DIRECCION, " +
                        "DEPTO_RESIDENCIA, MUNI_RESIDENCIA, TOTALFOTOS FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET ='" + CmbCarne.Text + "'";
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TxtDpi.Text = reader["CARNET"].ToString();
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
                    }
                    con.Close();
                }
            }
        }               
    }
}