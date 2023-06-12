using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
//using System.Drawing;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Ajax.Utilities;
using NPOI.SS.Formula.Functions;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Resources;

namespace ReportesUnis
{
    public partial class ConfirmacionDeFotografias : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string rutaFisica = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            LeerInfoTxtPath();
            LeerInfoTxtSQL();
            LeerInfoTxt();
            rutaFisica = Server.MapPath("~" + txtPath.Text);

            if (!IsPostBack)
            {
                llenadoGrid();

            }

        }

        void llenadoGrid()
        {
            string[] archivos = Directory.GetFiles(rutaFisica);
            List<object> imagenes = new List<object>();

            foreach (string archivo in archivos)
            {
                string nombreImagen = Path.GetFileName(archivo);
                imagenes.Add(new { NombreImagen = nombreImagen });
            }

            GridViewFotos.DataSource = imagenes;
            GridViewFotos.DataBind();
        }

        void LeerInfoTxtPath()
        {
            string rutaCompleta = CurrentDirectory + "PathConfirmacion.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                txtPath.Text = line;
                file.Close();
            }
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
                        trans.Rollback();
                        conexion.Close();
                        retorno = "1";
                    }
                }
            }
            return retorno;
        }


        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridViewFotos.Rows)
            {
                CheckBox checkBox = (CheckBox)row.FindControl("CheckBoxImage");
                if (checkBox.Checked)
                {
                    //prueba.Text = row.Cells[2].Text;
                    // Obtener el nombre de la imagen seleccionada sin extension
                    string nombre = row.Cells[1].Text.Substring(0, row.Cells[1].Text.Length - 4);
                    string cadena = "DELETE FROM UNIS_INTERFACES.TBL_HISTORIAL_CARNE WHERE CARNET = '" + nombre + "'";
                    string respuesta = ConsumoOracle(cadena);
                    if (respuesta == "0")
                    {
                        File.Delete(CurrentDirectory + txtPath.Text + row.Cells[1].Text);
                        llenadoGrid();

                    }
                    else
                    {
                        lblActualizacion.Text = "Ocurrió un error al eliminar los registros";
                    }
                }
            }
        }

        protected void GridViewFotos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image)e.Row.FindControl("Image1");
                string nombreImagen = DataBinder.Eval(e.Row.DataItem, "NombreImagen").ToString();
                string rutaImagen = Path.Combine("~" + txtPath.Text, nombreImagen);
                image.ImageUrl = rutaImagen;
            }
        }
    }
}