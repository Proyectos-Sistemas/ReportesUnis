using System;
using System.IO;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Ionic.Zip;
using System.Web;
using System.IO.Compression;
using Microsoft.Win32;
using System.Globalization;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System.Diagnostics;

namespace ReportesUnis
{
    public partial class ReporteEstudiantes : System.Web.UI.Page
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
                LoadData();
            }
        }

        private void LoadData()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("FIRST_NAME");
            dt.Columns.Add("SECOND_NAME");
            dt.Columns.Add("LAST_NAME");
            dt.Columns.Add("PHONE");
            dt.Columns.Add("CARNE");
            dt.Columns.Add("DPI");
            dt.Columns.Add("CARRERA");
            dt.Columns.Add("FACULTAD");
            dt.Columns.Add("STATUS");
            dt.Columns.Add("BIRTHDATE");
            dt.Columns.Add("DIRECCION");
            dt.Columns.Add("MUNICIPIO");
            dt.Columns.Add("DEPARTAMENTO");
            dt.Columns.Add("SEX");
            dt.Columns.Add("BIRTHPLACE");
            dt.Columns.Add("FLAG_CED");
            dt.Columns.Add("FLAG_PAS");
            dt.Columns.Add("FLAG_DPI");
            dt.Columns.Add("PASAPORTE");
            dt.Columns.Add("CEDULA");
            dt.Columns.Add("PROF");

            dr["FIRST_NAME"] = String.Empty;
            dr["SECOND_NAME"] = String.Empty;
            dr["LAST_NAME"] = String.Empty;
            dr["PHONE"] = String.Empty;
            dr["CARNE"] = String.Empty;
            dr["DPI"] = String.Empty;
            dr["CARRERA"] = String.Empty;
            dr["FACULTAD"] = String.Empty;
            dr["STATUS"] = String.Empty;
            dr["BIRTHDATE"] = String.Empty;
            dr["DIRECCION"] = String.Empty;
            dr["MUNICIPIO"] = String.Empty;
            dr["DEPARTAMENTO"] = String.Empty;
            dr["SEX"] = String.Empty;
            dr["BIRTHPLACE"] = String.Empty;
            dr["FLAG_CED"] = String.Empty;
            dr["FLAG_DPI"] = String.Empty;
            dr["FLAG_PAS"] = String.Empty;
            dr["PASAPORTE"] = String.Empty;
            dr["CEDULA"] = String.Empty;
            dr["PROF"] = String.Empty;


            dt.Columns.Add("EMPLID");

            dr["EMPLID"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewReporte.DataSource = dt;
            this.GridViewReporte.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowIndex > -1)
                for (int i = 0; i < row.Cells.Count; i += 1)
                    row.Cells[i].Visible = false;
        }

        public string Mayuscula(string busqueda)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string inicial = busqueda.Substring(0, 1).ToUpper();
            string letras = busqueda.Substring(1, busqueda.Length - 1).Trim(' ').ToLower();
            string resultado = textInfo.ToTitleCase(inicial + letras);
            return resultado;
        }

        //GENERACION DE CONSULTA A BD Y ASIGNACION A GRIDVIEW SEGUN LA BUSQUEDA DESEADA
        protected void Busqueda(object sender, EventArgs e)
        {
            lblDescarga.Visible = false;
            try
            {
                var fechaI = CldrCiclosInicio.Text;
                var anioI = fechaI.Substring(2, 2);
                var mesI = fechaI.Substring(5, 2);
                var diaI = fechaI.Substring(8, 2);
                var fechaF = CldrCiclosFin.Text;
                var anioF = fechaF.Substring(2, 2);
                var mesF = fechaF.Substring(5, 2);
                var diaF = fechaF.Substring(8, 2);
                var inicio = diaI + "/" + mesI + "/" + anioI;
                var fin = diaF + "/" + mesF + "/" + anioF;

                if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(lblBusqueda.Text))
                {
                    if (!String.IsNullOrEmpty(TxtBuscador.Text))
                        TxtBuscador.Text = Mayuscula(TxtBuscador.Text);
                    if (!String.IsNullOrEmpty(TxtBuscador2.Text))
                        TxtBuscador2.Text = Mayuscula(TxtBuscador2.Text);
                    var where = "";
                    if (!ChBusqueda.Checked)
                    {
                        string busqueda = LbxBusqueda.Text;
                        if (LbxBusqueda.Text.Equals("Nombre"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador.Text + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                        }
                        else if (LbxBusqueda.Text.Equals("Apellido"))
                        {
                            where = "WHERE (PD.LAST_NAME LIKE('%" + TxtBuscador.Text + "%') ) AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                        }
                        else if (LbxBusqueda.Text.Equals("DPI/Carné"))
                        {
                            where = "WHERE PN.NATIONAL_ID LIKE('%" + TxtBuscador.Text + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                        }
                        else if (LbxBusqueda.Text.Equals("Facultad"))
                        {
                            where = "WHERE AGT.DESCR LIKE('%" + TxtBuscador.Text + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                        }
                    }
                    else //CREACION DE WHERE PARA BUSQUEDA MULTIPLE CON LAS COMBINACIONES POSIBLES
                    {
                        if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador.Text + "%') AND((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR(TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "')) AND (PD.LAST_NAME LIKE('%" + TxtBuscador2.Text + "%') )";

                        }
                        else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI/Carné"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND PN.NATIONAL_ID LIKE('%" + TxtBuscador2.Text + "%') ";
                        }
                        else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Facultad"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND AGT.DESCR LIKE('%" + TxtBuscador2.Text + "%')";
                        }
                        else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI/Carné"))
                        {
                            where = "WHERE (PD.LAST_NAME LIKE('%" + TxtBuscador.Text + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text + "%')) AND PN.NATIONAL_ID LIKE('%" + TxtBuscador2.Text + "%') ";
                        }
                        else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Facultad"))
                        {
                            where = "WHERE (PD.LAST_NAME LIKE('%" + TxtBuscador.Text + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text + "%')) AND AGT.DESCR LIKE('%" + TxtBuscador2.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND AGT.DESCR LIKE('%" + TxtBuscador2.Text + "%')";

                        }
                        else if (LbxBusqueda.Text.Equals("DPI/Carné") && LbxBusqueda2.Text.Equals("Facultad"))
                        {
                            where = "WHERE PN.NATIONAL_ID LIKE('%" + TxtBuscador.Text + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND AGT.DESCR LIKE('%" + TxtBuscador2.Text + "%')";
                        }
                        else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador2.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND (PD.LAST_NAME LIKE('%" + TxtBuscador.Text + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text + "%'))  ";

                        }
                        else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI/Carné"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador2.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND PN.NATIONAL_ID LIKE('%" + TxtBuscador.Text + "%') ";
                        }
                        else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Facultad"))
                        {
                            where = "WHERE PD.FIRST_NAME LIKE('%" + TxtBuscador2.Text + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND AGT.DESCR LIKE('%" + TxtBuscador.Text + "%')";
                        }
                        else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI/Carné"))
                        {
                            where = "WHERE (PD.LAST_NAME LIKE('%" + TxtBuscador2.Text + "%') ) AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND PN.NATIONAL_ID LIKE('%" + TxtBuscador.Text + "%') ";
                        }
                        else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Facultad"))
                        {
                            where = "WHERE (PD.LAST_NAME LIKE('%" + TxtBuscador2.Text + "%') ) AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND AGT.DESCR LIKE('%" + TxtBuscador.Text + "%')";
                        }
                        else if (LbxBusqueda2.Text.Equals("DPI/Carné") && LbxBusqueda.Text.Equals("Facultad"))
                        {
                            where = "WHERE PN.NATIONAL_ID LIKE('%" + TxtBuscador2.Text + "%')  AND AGT.DESCR LIKE('%" + TxtBuscador.Text + "%')";
                        }
                    }
                    string constr = TxtURL.Text;
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.CommandText = "SELECT FLAG_DPI, FLAG_PAS, FLAG_CED, PROF, EMPLID,FIRST_NAME,SECOND_NAME,LAST_NAME, SECOND_LAST_NAME,CARNE,PHONE,DPI, " +
                                "CEDULA, PASAPORTE, CARRERA, FACULTAD,STATUS,BIRTHDATE,DIRECCION,MUNICIPIO,DEPARTAMENTO, SEX, BIRTHPLACE, CNT " +
                                "FROM ( SELECT PD.EMPLID, PN.NATIONAL_ID CARNE, REGEXP_SUBSTR(PD.FIRST_NAME,'[^ ]+') FIRST_NAME, " +
                                "SUBSTR(PD.FIRST_NAME,(INSTR(PD.FIRST_NAME,' ')+1)) SECOND_NAME, PD.LAST_NAME, PD.SECOND_LAST_NAME,  " +
                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN  PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN  PN.NATIONAL_ID  ELSE '' END DPI, " +
                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN  '1' WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN  '1'  ELSE '0' END FLAG_DPI, " +
                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN  PN.NATIONAL_ID  ELSE '' END CEDULA, CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN  '1'  ELSE '0' END FLAG_CED," +
                                " CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN  PN.NATIONAL_ID  WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN  PN.NATIONAL_ID  ELSE '' END PASAPORTE, " +
                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN  '1'  WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN  '1'  ELSE '0' END FLAG_PAS, " +
                                "PPD.PHONE , TO_CHAR(PD.BIRTHDATE,'DD-MM-YYYY') BIRTHDATE, APD.DESCR CARRERA, AGT.DESCR FACULTAD,  " +
                                "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE '' END SEX, PD.BIRTHPLACE , " +
                                "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'Sin Información' END STATUS, " +
                                "A.ADDRESS1 || ' ' || A.ADDRESS2 || ' ' || A.ADDRESS3 || ' ' || A.ADDRESS4 DIRECCION, REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO,  " +
                                "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, TT.TERM_BEGIN_DT, ROW_NUMBER() OVER(PARTITION BY PD.EMPLID ORDER BY 18 DESC) CNT, " +
                                "'ESTUDIANTE' PROF " +
                                "FROM SYSADM.PS_PERS_DATA_SA_VW PD " +
                                "LEFT JOIN SYSADM.PS_PERS_NID PN ON  PD.EMPLID = PN.EMPLID " +
                                "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON PD.EMPLID = CT.EMPLID " +
                                "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON CT.ACAD_GROUP_ADVIS = AGT.ACAD_GROUP " +
                                "LEFT JOIN SYSADM.PS_STDNT_ENRL SE ON CT.ACAD_CAREER = SE.ACAD_CAREER AND CT.STRM = SE.STRM AND CT.INSTITUTION=SE.INSTITUTION AND CT.EMPLID = SE.EMPLID LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                "LEFT JOIN SYSADM.PS_ACAD_PROG AP ON PD.EMPLID = AP.EMPLID " +
                                "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON AP.ACAD_PROG = APD.ACAD_PROG " +
                                "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = AP.EMPLID " +
                                where +
                                " AND SE.STDNT_ENRL_STATUS = 'E' AND SE.ENRL_STATUS_REASON='ENRL' AND AP.PROG_ACTION = 'MATR') WHERE CNT = 1";
                            cmd.Connection = con;
                            con.Open();

                            OracleDataReader reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                GridViewReporte.DataSource = reader;
                                GridViewReporte.DataBind();
                                GridVieweMPLID.DataSource = reader;
                                GridVieweMPLID.DataBind();
                                LbxBusqueda.Text = "";
                                TxtBuscador.Text = "";
                                TxtBuscador2.Text = "";
                                ChBusqueda.Checked = false;
                                LbxBusqueda2.Visible = false;
                                TxtBuscador2.Visible = false;
                                //CldrCiclosInicio2.Visible = false;
                                //CldrCiclosFin2.Visible = false;
                                //FFin2.Visible = false;
                                //FInicio2.Visible = false;
                                lblBusqueda.Text = " ";
                            }
                            else
                            {
                                lblBusqueda.Text = "No se encontró información con los valores ingresados";
                            }
                            con.Close();
                        }
                    }
                }
                else
                {
                    lblBusqueda.Text = "Ingrese un valor a buscar";
                }
            }
            catch (Exception)
            {

                lblBusqueda.Text = "Debe de agrar un rango de fechas para realizar una búsqueda";
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

        protected void BtnRead_Click(object sender, EventArgs e)
        {
            LeerInfoTxt();
        }

        //EXPORTACION DE INFORMACION CONTENIDA EN EL GRID A TXT
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string txtFile = string.Empty;

            for (int k = 0; k < GridViewReporte.Columns.Count - 1; k++)
            {
                string texto = removeUnicode(GridViewReporte.Columns[k].ToString());
                txtFile += texto + "|";
            }

            txtFile += "\r\n";

            //Llenado de las columnas con la informacion

            int ret = 0;
            for (int j = 0; j < GridViewReporte.Rows.Count; j++)
            {
                int aux = 0;
                for (int i = 0; i < GridViewReporte.Columns.Count - 1; i++)
                {
                    string texto = removeUnicode(GridViewReporte.Rows[j].Cells[i].Text);
                    texto = texto.TrimEnd();
                    txtFile += texto + "|";
                    if (texto != "" && ret == 0)
                    {
                        aux = 0;
                    }
                    else if (aux < GridViewReporte.Columns.Count - 2)
                    {
                        aux = aux + 1;

                    }
                    else
                    {
                        ret = 1;
                        j = GridViewReporte.Rows.Count + 2;
                        i = GridViewReporte.Columns.Count + 2;
                    }
                }
                txtFile += "\r\n";
            }

            

            //SE GENERA EL ARCHIVO
            if (ret == 0)
            {
                Response.Clear();
                Response.Buffer = true;
                string FileName = "Reporte Estudiantes" + DateTime.Now + ".txt";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(txtFile);
                Response.Flush();
                Response.End();
            }
            else
            {
                lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
            }

            //DownloadAllFile(emplid, total);
        }
        public static string removeUnicode(string input)
        {
            //Mayusculas con Tilde
            Regex replaceAt = new Regex("&#193;", RegexOptions.Compiled);
            input = replaceAt.Replace(input, "Á");
            Regex replaceEt = new Regex("&#201;", RegexOptions.Compiled);
            input = replaceEt.Replace(input, "É");
            Regex replaceIt = new Regex("&#205;", RegexOptions.Compiled);
            input = replaceIt.Replace(input, "Í");
            Regex replaceOt = new Regex("&#211;", RegexOptions.Compiled);
            input = replaceOt.Replace(input, "Ó");
            Regex replaceUt = new Regex("&#218;", RegexOptions.Compiled);
            input = replaceUt.Replace(input, "Ú");

            //Minusculas con tilde
            Regex replaceA = new Regex("&#225;", RegexOptions.Compiled);
            input = replaceA.Replace(input, "á");
            Regex replaceE = new Regex("&#233;", RegexOptions.Compiled);
            input = replaceE.Replace(input, "é");
            Regex replaceI = new Regex("&#237;", RegexOptions.Compiled);
            input = replaceI.Replace(input, "í");
            Regex replaceO = new Regex("&#243;", RegexOptions.Compiled);
            input = replaceO.Replace(input, "ó");
            Regex replaceU = new Regex("&#250;", RegexOptions.Compiled);
            input = replaceU.Replace(input, "ú");

            //Ñ y ñ
            Regex replaceN = new Regex("&#209;", RegexOptions.Compiled);
            input = replaceN.Replace(input, "Ñ");
            Regex replacen = new Regex("&#241;", RegexOptions.Compiled);
            input = replacen.Replace(input, "ñ");

            //Mayusculas con dieresis
            Regex replaceAd = new Regex("&#196;", RegexOptions.Compiled);
            input = replaceAd.Replace(input, "Ä");
            Regex replaceEd = new Regex("&#203;", RegexOptions.Compiled);
            input = replaceEd.Replace(input, "Ë");
            Regex replaceId = new Regex("&#207;", RegexOptions.Compiled);
            input = replaceId.Replace(input, "Ï");
            Regex replaceOd = new Regex("&#214;", RegexOptions.Compiled);
            input = replaceOd.Replace(input, "Ö");
            Regex replaceUd = new Regex("&#220;", RegexOptions.Compiled);
            input = replaceUt.Replace(input, "Ü");

            //Minusculas con tilde
            Regex replaceAmd = new Regex("&#228;", RegexOptions.Compiled);
            input = replaceAmd.Replace(input, "ä");
            Regex replaceEmd = new Regex("&#235;", RegexOptions.Compiled);
            input = replaceEmd.Replace(input, "ë");
            Regex replaceImd = new Regex("&#239;", RegexOptions.Compiled);
            input = replaceImd.Replace(input, "ï");
            Regex replaceOmd = new Regex("&#246;", RegexOptions.Compiled);
            input = replaceOmd.Replace(input, "ö");
            Regex replaceUmd = new Regex("&#252;", RegexOptions.Compiled);
            input = replaceUmd.Replace(input, "ü");

            Regex replaceEspace = new Regex("&nbsp;", RegexOptions.Compiled);
            input = replaceEspace.Replace(input, " ");

            return input;
        }
        protected void LbxBusqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            LbxBusqueda2.Items.Clear();
            LbxBusqueda2.Items.Insert(0, "Nombre");
            LbxBusqueda2.Items.Insert(1, "Apellido");
            LbxBusqueda2.Items.Insert(2, "DPI/Carné");
            LbxBusqueda2.Items.Insert(3, "Facultad");
            LbxBusqueda2.Items.Insert(4, "Ciclo");
            LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));
            if (LbxBusqueda.Text.Equals("Ciclo"))
            {
                TxtBuscador.Visible = false;
                //CldrCiclosInicio.Visible = true;
                //CldrCiclosFin.Visible = true;
                //FFin.Visible = true;
                //FInicio.Visible = true;
                TxtBuscador2.Visible = false;
                TxtBuscador2.Text = "";
                //CldrCiclosInicio2.Visible = false;
                //CldrCiclosFin2.Visible = false;
                //FFin2.Visible = false;
                //FInicio2.Visible = false;
            }
            else
            {
                TxtBuscador.Visible = true;
                TxtBuscador2.Text = "";
                //CldrCiclosInicio.Visible = false;
                //CldrCiclosFin.Visible = false;
                //FFin.Visible = false;
                //FInicio.Visible = false;
            }
        }

        protected void ChBusqueda_CheckedChanged(object sender, EventArgs e)
        {
            if (ChBusqueda.Checked)
            {
                LbxBusqueda2.Visible = true;
                TxtBuscador2.Visible = true;
                TxtBuscador2.Text = "";
                LbxBusqueda2.Items.Clear();
                LbxBusqueda2.Items.Insert(0, "Nombre");
                LbxBusqueda2.Items.Insert(1, "Apellido");
                LbxBusqueda2.Items.Insert(2, "DPI/Carné");
                LbxBusqueda2.Items.Insert(3, "Facultad");
                LbxBusqueda2.Items.Insert(4, "Ciclo");
                LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));
            }
            else
            {
                LbxBusqueda2.Visible = false;
                TxtBuscador2.Visible = false;
                TxtBuscador2.Text = "";
                //CldrCiclosInicio2.Visible = false;
                //CldrCiclosFin2.Visible = false;
                //FFin2.Visible = false;
                //FInicio2.Visible = false;
            }
        }

        protected void LbxBusqueda2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LbxBusqueda2.Text.Equals("Ciclo"))
            {
                TxtBuscador2.Visible = false;
                TxtBuscador2.Text = "";
                //CldrCiclosInicio2.Visible = true;
                //CldrCiclosFin2.Visible = true;
                //FFin2.Visible = true;
                //FInicio2.Visible = true;
                //TxtBuscador.Visible = true;
                //CldrCiclosInicio.Visible = false;
                //CldrCiclosFin.Visible = false;
                //FFin.Visible = false;
                //FInicio.Visible = false;
            }
            else
            {
                TxtBuscador2.Visible = true;
                TxtBuscador2.Text = "";
                //CldrCiclosInicio2.Visible = false;
                //CldrCiclosFin2.Visible = false;
                //FFin2.Visible = false;
                //FInicio2.Visible = false;
            }
        }

        protected string DownloadAllFile(string where)
        {
            string nombre = "ImagenesEstudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".zip";
            string constr = TxtURL.Text;
            string ret = "0";
            where = where.TrimEnd(',');
            int total = 0;
            DataSetLocalRpt dsDownload = new DataSetLocalRpt();
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT P.*, CASE WHEN dbms_lob.substr(EMPLOYEE_PHOTO,3,1) = hextoraw('FFD8FF') THEN 'JPG' END Extension FROM SYSADM.PS_EMPL_PHOTO P WHERE EMPLID in (" + where + ") AND employee_photo IS NOT NULL ";
                    cmd.Connection = con;
                    con.Open();
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            DataRow newFila = dsDownload.Tables["AllDownload"].NewRow();
                            newFila["bytes"] = (byte[])row["EMPLOYEE_PHOTO"];
                            newFila["contentType"] = row["Extension"].ToString();
                            newFila["fileName"] = row["EMPLID"].ToString() + "." + row["Extension"].ToString().ToLower();
                            dsDownload.Tables["AllDownload"].Rows.Add(newFila);
                            total = total + 1;
                        }
                        con.Close();

                        if (total > 0)
                        {
                            string user = Environment.UserName;
                            string path = "C:\\Users\\" + user + "\\Downloads";
                            if (!Directory.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            string folder = path + "\\" + nombre;
                            //string folder = AppDomain.CurrentDomain.BaseDirectory + nombre;
                            File.Create(folder).Close();

                            using (FileStream zipToOpen = new FileStream(folder, FileMode.Open))
                            {

                                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                                {
                                    for (int i = 0; i < total; i++)
                                    {
                                        byte[] base64 = (byte[])dsDownload.Tables["AllDownload"].Rows[i]["bytes"];
                                        ZipArchiveEntry readmeEntry = archive.CreateEntry(dsDownload.Tables["AllDownload"].Rows[i]["filename"].ToString(), CompressionLevel.Fastest);

                                        var zipStream = readmeEntry.Open();
                                        zipStream.Write(base64, 0, base64.Length);

                                    }
                                }
                            }

                            lblDescarga.Visible = true;
                            lblDescarga.Text = "Las fotografías fueron almacenadas en la carpeta de descargas.";
                            Process.Start(folder);
                            ret = "1";
                        }
                        else
                        {
                            ret = "2";
                        }

                    }
                    else
                    {
                        ret = "2";
                    }
                }
            }
            return ret;
        }

        protected void ButtonFts_Click(object sender, EventArgs e)
        {
            try
            {
                string emplid = ""; 
                for (int k = 0; k < GridViewReporte.Rows.Count; k++)
                {
                    emplid += "'" + removeUnicode(GridViewReporte.Rows[k].Cells[66].Text) + "',";
                }
                
                string respuesta = DownloadAllFile(emplid);
                if (respuesta == "0")
                {
                    lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga de fotografías";
                }
                else if (respuesta == "2")
                    lblBusqueda.Text = "No se encontraron imágenes relacionadas a los estudiantes.";
            }
            catch (Exception x)
            {
                lblBusqueda.Text = "Ha ocurido un error";
            }
        }
    }

}
