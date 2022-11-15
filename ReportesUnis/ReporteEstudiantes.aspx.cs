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
using FileHelpers;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using Org.BouncyCastle.Utilities.Encoders;
using System.IdentityModel.Protocols.WSTrust;

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
            dt.Columns.Add("PLACE");
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
            dr["PLACE"] = String.Empty;
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
                if (!String.IsNullOrEmpty(TxtBuscador.Text))
                {

                    if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(lblBusqueda.Text))
                    {
                        if (!String.IsNullOrEmpty(TxtBuscador.Text) && !LbxBusqueda.Text.Equals("Facultad"))
                            TxtBuscador.Text = Mayuscula(TxtBuscador.Text);
                        if (!String.IsNullOrEmpty(TxtBuscador2.Text) && !LbxBusqueda2.Text.Equals("Facultad"))
                            TxtBuscador2.Text = Mayuscula(TxtBuscador2.Text);
                        string where = stringWhere();
                        string constr = TxtURL.Text;
                        using (OracleConnection con = new OracleConnection(constr))
                        {
                            using (OracleCommand cmd = new OracleCommand())
                            {
                                cmd.CommandText = "SELECT " +
                                                    "FLAG_DPI, " +
                                                    "FLAG_PAS, " +
                                                    "FLAG_CED, " +
                                                    "PROF, " +
                                                    "EMPLID, " +
                                                    "FIRST_NAME, " +
                                                    "SECOND_NAME, " +
                                                    "LAST_NAME, " +
                                                    "SECOND_LAST_NAME, " +
                                                    "CARNE, " +
                                                    "PHONE, " +
                                                    "DPI, " +
                                                    "CEDULA, " +
                                                    "PASAPORTE, " +
                                                    "CARRERA, " +
                                                    "FACULTAD, " +
                                                    "STATUS, " +
                                                    "BIRTHDATE, " +
                                                    "DIRECCION, " +
                                                    "MUNICIPIO, " +
                                                    "DEPARTAMENTO, " +
                                                    "SEX, " +
                                                    "PLACE " +
                                                    "FROM ( " +
                                                    "SELECT " +
                                                    "DISTINCT PD.EMPLID, " +
                                                    "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) CARNE, " +
                                                    "REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+') FIRST_NAME, " +
                                                    "SUBSTR(PD.FIRST_NAME,  LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))+2, LENGTH(PD.FIRST_NAME)-LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))) SECOND_NAME, " +
                                                    "PD.LAST_NAME, " +
                                                    "PD.SECOND_LAST_NAME, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN PN.NATIONAL_ID ELSE '' END DPI, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN '1' ELSE '0' END FLAG_DPI, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN '1' ELSE '0' END FLAG_CED, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE '' END PASAPORTE, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN '1' ELSE '0' END FLAG_PAS, " +
                                                    "PPD.PHONE, " +
                                                    "TO_CHAR(PD.BIRTHDATE, 'DD-MM-YYYY') BIRTHDATE, " +
                                                    "APD.DESCR CARRERA, " +
                                                    "AGT.DESCR FACULTAD, " +
                                                    "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE '' END SEX, " +
                                                    "CASE WHEN (PD.BIRTHPLACE = ' ' AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'Condición Migrante' WHEN (PD.BIRTHPLACE = ' ' AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE PD.BIRTHPLACE END PLACE," +
                                                    "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'Sin Información' END STATUS, " +
                                                    "(select A1.ADDRESS1 || ' ' || A1.ADDRESS2 || ' ' || A1.ADDRESS3 || ' ' || A1.ADDRESS4 from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) DIRECCION, " +
                                                    "REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO, " +
                                                    "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, " +
                                                    "'ESTUDIANTE' PROF " +
                                                    "FROM " +
                                                    "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                                    "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
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
                                                    where + ")" +
                                                    "  WHERE CARNE=DPI OR CARNE=PASAPORTE OR CARNE=CEDULA ORDER BY 6 ASC";
                                cmd.Connection = con;
                                con.Open();

                                OracleDataReader reader = cmd.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    GridViewReporte.DataSource = reader;
                                    GridViewReporte.DataBind();
                                    //ChBusqueda.Checked = false;
                                    //LbxBusqueda2.Visible = false;
                                    //TxtBuscador2.Visible = false;
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
            try
            {
                if (!String.IsNullOrEmpty(TxtBuscador.Text) || !String.IsNullOrEmpty(LbxBusqueda.Text))
                {
                    string where = stringWhere();
                    string constr = TxtURL.Text;
                    string txtFile = "IDUNIV|NOM_IMP|NOM1|NOM2|APE1|APE2|APE3|FE_NAC|SEXO|EST_CIV|NACIONAL|FLAG_CED|CEDULA|DEPCED|MUNCED|FLAG_DPI|DPI|FLAG_PAS|PASS|PAIS_PAS|NIT|PAIS_NIT|PROF|DIR|CASA|APTO|ZONA|COL|MUNRES|DEPRES|TEL|CEL|EMAIL|CARNET|CARR|FACUL|COD_EMP_U|PUESTO|DEP_EMP_U|COD_BARRAS|TIP_PER|ACCION|FOTO|TIPO_CTA|NO_CTA_BI|F_U|H_U|TIP_ACC|EMP_TRAB|FEC_IN_TR|ING_TR|EGR_TR|MONE_TR|PUESTO_TR|LUG_EMP|FE_IN_EMP|TEL_TR|DIR_TR|ZONA_TR|DEP_TR|MUNI_TR|PAIS_TR|ACT_EC|OTRA_NA|CONDMIG|O_CONDMIG";
                    txtFile += "\r\n";
                    var dt = new DataTable();
                    using (OracleConnection con = new OracleConnection(constr))
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.CommandText =
                            "SELECT '|' || '|' || FIRST_NAME || '|' || SECOND_NAME || '|' || LAST_NAME || '|' || '|' ||" +
                            " SECOND_LAST_NAME || '|' || BIRTHDATE || '|' || SEX || '|' || STATUS || '|' || PLACE || '|' ||" +
                            " FLAG_CED || '|' || CEDULA || '|' || '|' || '|' || FLAG_DPI || '|' || DPI || '|' || FLAG_PAS ||" +
                            " '|' || PASAPORTE || '|' || '|' || '|' || '|' || PROF || '|' || DIRECCION || '|' || '|' || '|' ||" +
                            " '|' || '|' || MUNICIPIO || '|' || DEPARTAMENTO || '|' || PHONE || '|' || '|' || '|' || CARNE || '|' ||" +
                            " CARRERA || '|' || FACULTAD || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' ||" +
                            " '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' || '|' ||" +
                            " '|' || '|' || '|' || '|' || '|' || '|' " +
                            "FROM ( " +
                            "SELECT " +
                            "DISTINCT PD.EMPLID, " +
                            "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) CARNE, " +
                            "REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+') FIRST_NAME, " +
                            "SUBSTR(PD.FIRST_NAME,  LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))+2, LENGTH(PD.FIRST_NAME)-LENGTH(REGEXP_SUBSTR(PD.FIRST_NAME, '[^ ]+'))) SECOND_NAME, " +
                            "PD.LAST_NAME, " +
                            "PD.SECOND_LAST_NAME, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN PN.NATIONAL_ID ELSE '' END DPI, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN '1' ELSE '0' END FLAG_DPI, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN '1' ELSE '0' END FLAG_CED, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE '' END PASAPORTE, " +
                            "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN '1' WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN '1' ELSE '0' END FLAG_PAS, " +
                            "PPD.PHONE, " +
                            "TO_CHAR(PD.BIRTHDATE, 'DD-MM-YYYY') BIRTHDATE, " +
                            "APD.DESCR CARRERA, " +
                            "AGT.DESCR FACULTAD, " +
                            "CASE WHEN PD.SEX = 'M' THEN '1' WHEN PD.SEX = 'F' THEN '2' ELSE '' END SEX, " +
                            "CASE WHEN (PD.BIRTHPLACE = ' ' AND (PN.NATIONAL_ID_TYPE = 'PAS' OR PN.NATIONAL_ID_TYPE = 'EXT') ) THEN 'Condición Migrante' WHEN (PD.BIRTHPLACE = ' ' AND (PN.NATIONAL_ID_TYPE = 'DPI' OR PN.NATIONAL_ID_TYPE = 'CED') )THEN 'Guatemala' ELSE PD.BIRTHPLACE END PLACE," +
                            "CASE WHEN PD.MAR_STATUS = 'M' THEN 'Casado' WHEN PD.MAR_STATUS = 'S' THEN 'Soltero' ELSE 'Sin Información' END STATUS, " +
                            "(select A1.ADDRESS1 || ' ' || A1.ADDRESS2 || ' ' || A1.ADDRESS3 || ' ' || A1.ADDRESS4 from SYSADM.PS_ADDRESSES A1 where PD.EMPLID = A1.EMPLID ORDER BY CASE WHEN A1.ADDRESS_TYPE = 'HOME' THEN 1 ELSE 2 END FETCH FIRST 1 ROWS ONLY) DIRECCION, " +
                            "REGEXP_SUBSTR(ST.DESCR, '[^-]+') MUNICIPIO, " +
                            "SUBSTR(ST.DESCR, (INSTR(ST.DESCR, '-') + 1)) DEPARTAMENTO, " +
                            "'ESTUDIANTE' PROF " +
                            "FROM " +
                            "SYSADM.PS_PERS_DATA_SA_VW PD " +
                            "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
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
                            where + ")" +
                            "  WHERE CARNE=DPI OR CARNE=PASAPORTE OR CARNE=CEDULA ORDER BY 1 ASC";
                            cmd.Connection = con;
                            con.Open();

                            OracleDataReader reader = cmd.ExecuteReader();
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            if (reader.HasRows)
                            {
                                adapter.Fill(dt);
                                int contador = dt.Rows.Count;
                                for (int i = 0; i < contador; i++)
                                {
                                    txtFile = txtFile + dt.Rows[i].ItemArray[0].ToString();
                                    txtFile += "\r\n";
                                }
                            }
                            else
                            {
                                lblBusqueda.Text = "No se encontró información con los valores ingresados";
                            }
                            con.Close();
                        }
                    }
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
            }
            catch
            {
                lblBusqueda.Text = "Ha ocurido un error";
            }
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
            LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));

            TxtBuscador.Visible = true;
            TxtBuscador2.Text = "";
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
                LbxBusqueda2.Items.Remove(LbxBusqueda2.Items.FindByValue(LbxBusqueda.Text));
            }
            else
            {
                LbxBusqueda2.Visible = false;
                TxtBuscador2.Visible = false;
                TxtBuscador2.Text = "";
            }
        }

        protected void LbxBusqueda2_SelectedIndexChanged(object sender, EventArgs e)
        {
            TxtBuscador2.Visible = true;
            TxtBuscador2.Text = "";
        }

        protected string DownloadAllFile(string where)
        {
            string nombre = "ImagenesEstudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".zip";
            string constr = TxtURL.Text;
            string ret = "0";
            int total = 0;
            DataSetLocalRpt dsDownload = new DataSetLocalRpt();

            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT * FROM ( " +
                                        "SELECT P.*, CASE WHEN dbms_lob.substr(EMPLOYEE_PHOTO,3,1) = hextoraw('FFD8FF') THEN 'JPG' END Extension, " +
                                        "ROW_NUMBER() OVER(PARTITION BY P.EMPLID ORDER BY P.EMPLID) AS CNT " +
                                        "FROM SYSADM.PS_PERS_DATA_SA_VW PD " +
                                        "JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                        "JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                        "JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                        "JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                        "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                        "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID AND SE.STDNT_ENRL_STATUS = 'E' AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                        "JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID AND CT.STRM = SE.STRM AND CT.ACAD_CAREER = SE.ACAD_CAREER AND SE.INSTITUTION = CT.INSTITUTION " +
                                        "JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG AND CT.ACAD_CAREER = APD.ACAD_CAREER AND CT.INSTITUTION = APD.INSTITUTION " +
                                        "JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP AND APD.INSTITUTION = AGT.INSTITUTION " +
                                        "JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM AND CT.INSTITUTION = TT.INSTITUTION " +
                                        where +
                                        "AND employee_photo IS NOT NULL " +
                                        ")WHERE CNT =1";
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
                    }
                }
            }

            if (total > 0)
            {
                string user = Environment.UserName;
                string unidad = unidadAlmacenamiento().Substring(0, 2);
                string path = unidad + ":\\Users\\" + user + "\\Downloads";
                if (!Directory.Exists(path))
                {
                    File.Create(path).Close();
                }
                string folder = path + "\\" + nombre;
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
                lblBusqueda.Text = "";
                lblDescarga.Visible = true;
                lblDescarga.Text = "Las fotografías fueron almacenadas en la ubicación: <a href=" + path + ">" + path + "</a>";
                ret = "1";
            }
            else
            {
                ret = "2";
            }
            return ret;
        }

        protected void ButtonFts_Click(object sender, EventArgs e)
        {
            try
            {
                string where = stringWhere();
                string respuesta = DownloadAllFile(where);
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

        public string unidadAlmacenamiento()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string name = "";
            foreach (DriveInfo drive in drives)
            {
                string label = drive.IsReady ?
                    String.Format(" - {0}", drive.VolumeLabel) : null;
                Console.WriteLine("{0} - {1}{2}", drive.Name, drive.DriveType, label);
                name = name + " " + drive.Name;
            }
            return name;
        }

        public string stringWhere()
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
            var where = "";
            if (!ChBusqueda.Checked)
            {
                TxtBuscador.Text = TxtBuscador.Text.TrimEnd(' ');
                TxtBuscador.Text = TxtBuscador.Text.TrimStart(' ');
                string busqueda = LbxBusqueda.Text;
                if (LbxBusqueda.Text.Equals("Nombre"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Apellido"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') ) AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("DPI/Carné"))
                {
                    where = "WHERE UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Facultad"))
                {
                    where = "WHERE UPPER(AGT.DESCR) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
            }
            else //CREACION DE WHERE PARA BUSQUEDA MULTIPLE CON LAS COMBINACIONES POSIBLES
            {

                TxtBuscador.Text = TxtBuscador.Text.TrimEnd(' ');
                TxtBuscador.Text = TxtBuscador.Text.TrimStart(' ');

                TxtBuscador2.Text = TxtBuscador2.Text.TrimEnd(' ');
                TxtBuscador2.Text = TxtBuscador2.Text.TrimStart(' ');
                if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Apellido"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR(TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "')) AND (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') )";

                }
                else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("DPI/Carné"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') ";
                }
                else if (LbxBusqueda.Text.Equals("Nombre") && LbxBusqueda2.Text.Equals("Facultad"))
                {                    
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(AGT.DESCR) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%')";
                }
                else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("DPI/Carné"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text.ToUpper() + "%')) AND UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + fin + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Apellido") && LbxBusqueda2.Text.Equals("Facultad"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text.ToUpper() + "%')) AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(AGT.DESCR) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%')";

                }
                else if (LbxBusqueda.Text.Equals("DPI/Carné") && LbxBusqueda2.Text.Equals("Facultad"))
                {
                    where = "WHERE UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(AGT.DESCR) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%')";
                }
                else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Apellido"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') OR PD.SECOND_LAST_NAME LIKE('%" + TxtBuscador.Text.ToUpper() + "%'))  ";

                }
                else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("DPI/Carné"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') ";
                }
                else if (LbxBusqueda2.Text.Equals("Nombre") && LbxBusqueda.Text.Equals("Facultad"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(UPPER(AGT.DESCR)) LIKE('%" + TxtBuscador.Text.ToUpper().ToUpper() + "%')";
                }
                else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("DPI/Carné"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') ) AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') ";
                }
                else if (LbxBusqueda2.Text.Equals("Apellido") && LbxBusqueda.Text.Equals("Facultad"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%') ) AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' )) AND UPPER(UPPER(AGT.DESCR)) LIKE('%" + TxtBuscador.Text.ToUpper() + "%')";
                }
                else if (LbxBusqueda2.Text.Equals("DPI/Carné") && LbxBusqueda.Text.Equals("Facultad"))
                {
                    where = "WHERE UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador2.Text.ToUpper() + "%')  AND UPPER(UPPER(AGT.DESCR)) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND  ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
            }
            return where;
        }
    }

}
