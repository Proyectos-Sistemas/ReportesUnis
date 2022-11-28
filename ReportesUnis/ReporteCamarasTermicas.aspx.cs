using System;
using System.IO;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;
using System.Data;
using System.Web;
using SpreadsheetLight;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;
using NPOI.Util;
using System.Threading;
using System.Windows;
using System.IO.Packaging;

namespace ReportesUnis
{
    public partial class ReporteCamarasTermicas : System.Web.UI.Page
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
                BindGrid();
            }
        }

        //GENERACION DE CONSULTA A BD Y ASIGNACION A GRIDVIEW SIN BUSQUEDA
        private void BindGrid()
        {

            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add("FIRST_NAME");
            dt.Columns.Add("LAST_NAME");
            dt.Columns.Add("ID");
            dt.Columns.Add("TYPE");
            dt.Columns.Add("PERSON_GROUP");
            dt.Columns.Add("GENDER");
            dt.Columns.Add("Start_Time_of_Effective_Period");
            dt.Columns.Add("End_Time_of_Effective_Period");
            dt.Columns.Add("CARD");
            dt.Columns.Add("EMAIL");
            dt.Columns.Add("PHONE");
            dt.Columns.Add("REMARK");
            dt.Columns.Add("DOCK_STATION_LOGIN_PASSWORD");
            dt.Columns.Add("SUPPORTISSUEDCUSTOMPROPERTIES");
            dt.Columns.Add("SKINSURFACE_TEMPERATURE");
            dt.Columns.Add("TEMPERATURE_STATUS");
            dt.Columns.Add("DEPARTAMENTO");
            dt.Columns.Add("EMPLID");

            dr["FIRST_NAME"] = String.Empty;
            dr["LAST_NAME"] = String.Empty;
            dr["ID"] = String.Empty;
            dr["TYPE"] = String.Empty;
            dr["PERSON_GROUP"] = String.Empty;
            dr["GENDER"] = String.Empty;
            dr["Start_Time_of_Effective_Period"] = String.Empty;
            dr["End_Time_of_Effective_Period"] = String.Empty;
            dr["CARD"] = String.Empty;
            dr["EMAIL"] = String.Empty;
            dr["PHONE"] = String.Empty;
            dr["REMARK"] = String.Empty;
            dr["DOCK_STATION_LOGIN_PASSWORD"] = String.Empty;
            dr["SUPPORTISSUEDCUSTOMPROPERTIES"] = String.Empty;
            dr["SKINSURFACE_TEMPERATURE"] = String.Empty;
            dr["TEMPERATURE_STATUS"] = String.Empty;
            dr["DEPARTAMENTO"] = String.Empty;
            dr["EMPLID"] = String.Empty;

            dt.Rows.Add(dr);

            this.GridViewReporteCT.DataSource = dt;
            this.GridViewReporteCT.DataBind();

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

        //LLAMADA DE LA FUNCION PARA LA GENERACION DE BUSQUEDA
        protected void Busqueda(object sender, EventArgs e)
        {
            lblDescarga.Visible = false;
            try
            {
                consultaBusqueda();
            }
            catch (Exception)
            {
                lblBusqueda.Text = "No se encontró la información solicitada";
                return;
            }
        }

        //FUNCION PARA LA GENERACION DE CONSULTA A BD Y ASIGNACION A GRIDVIEW SIN BUSQUEDA
        public void consultaBusqueda()
        {
            string where = stringWhere();
            string constr = TxtURL.Text;

            try
            {
                if (!String.IsNullOrWhiteSpace(where))
                {
                    if (LbxBusqueda.Text != "Género" && !TxtBuscador.Text.ToLower().Equals("mujer"))
                    {
                        {
                            using (OracleConnection con = new OracleConnection(constr))
                            {
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.CommandText = "SELECT " +
                                                    "EMPLID, " +
                                                    "FIRST_NAME, " +
                                                    "LAST_NAME, " +
                                                    "ID, " +
                                                    "'Basic Person' TYPE, " +
                                                    "PERSON_GROUP || Departamento PERSON_GROUP, " +
                                                    "GENDER, " +
                                                    "'' Start_Time_of_Effective_Period, " +
                                                    "'' End_Time_of_Effective_Period, " +
                                                    "'' CARD, " +
                                                    "PHONE, " +
                                                    "EMAIL, " +
                                                    "'' Remark, " +
                                                    "'' Dock_Station_Login_Password, " +
                                                    "'' SupportIssuedCustomProperties, " +
                                                    "'' SkinSurface_Temperature, " +
                                                    "'' Temperature_Status, " +
                                                    "DEPARTAMENTO " +
                                                    "FROM " +
                                                    "( " +
                                                    "SELECT " +
                                                    "DISTINCT PD.EMPLID, " +
                                                    "PD.FIRST_NAME, " +
                                                    "PD.LAST_NAME || CASE WHEN LTRIM(RTRIM(PD.SECOND_LAST_NAME)) IS NOT NULL THEN ' ' || LTRIM(RTRIM(PD.SECOND_LAST_NAME)) END LAST_NAME, " +
                                                    "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) ID, " +
                                                    "CASE WHEN PD.SEX = 'F' THEN 'Female' WHEN PD.SEX = 'M' THEN 'Male' ELSE 'Unknown' END Gender, " +
                                                    "PPD.PHONE, " +
                                                    "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = PD.EMPLID AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) Email, " +
                                                    "AGT.DESCR DEPARTAMENTO," +
                                                    "APD.INSTITUTION || '/Estudiantes/' Person_Group, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN PN.NATIONAL_ID ELSE '' END DPI, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                                                    "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE '' END PASAPORTE " +
                                                    "FROM " +
                                                    "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                                    "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                                    "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                                    "AND A.EFFDT =(SELECT MAX(EFFDT) FROM SYSADM.PS_ADDRESSES A2 WHERE A.EMPLID = A2.EMPLID AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE) " +
                                                    "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                                    "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                                    "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID " +
                                                    "AND SE.STDNT_ENRL_STATUS = 'E' " +
                                                    "AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                                    "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID " +
                                                    "AND CT.STRM = SE.STRM " +
                                                    "AND CT.ACAD_CAREER = SE.ACAD_CAREER " +
                                                    "AND SE.INSTITUTION = CT.INSTITUTION " +
                                                    "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                                    "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                                    "AND CT.INSTITUTION = APD.INSTITUTION " +
                                                    "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                                    "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                                    "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                                    "AND CT.INSTITUTION = TT.INSTITUTION " +
                                                    "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                                    where +
                                                    ") " +
                                                    "WHERE  " +
                                                    "(ID = DPI " +
                                                    "OR ID = PASAPORTE " +
                                                    "OR ID = CEDULA )" +
                                                    "ORDER BY " +
                                                    "1 ASC ";
                                    cmd.Connection = con;
                                    con.Open();
                                    OracleDataReader reader = cmd.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        GridViewReporteCT.DataSource = cmd.ExecuteReader();
                                        GridViewReporteCT.DataBind();
                                        lblBusqueda.Text = "";
                                    }
                                    else
                                    {
                                        lblBusqueda.Text = "No se encontró la información solicitada";
                                        if (LbxBusqueda.Text == "Género")
                                            lblBusqueda.Text = lblBusqueda.Text + ". Para realizar búesqueda por género intente ingresando Male o Female";
                                    }
                                    con.Close();
                                }
                            }
                            TxtBuscador.Enabled = false;
                            CldrCiclosInicio.Enabled = false;
                            CldrCiclosFin.Enabled = false;
                            BtnImg.Enabled = true;
                            BtnTxt.Enabled = true;
                            BtnNBusqueda.Enabled = true;
                            BtnBuscar2.Enabled = false;
                            LbxBusqueda.Enabled = false;
                        }
                    }
                    else
                    {
                        lblBusqueda.Text = "Para realizar búesqueda por género intente ingresando Male o Female";
                    }
                }
                else
                {
                    lblBusqueda.Text = "Ingrese un valor a buscar";
                }
            }
            catch
            {
                lblBusqueda.Text = "No se encontró la información solicitada";
            }
        }

        //GENERACION DEL DOCUMENTO DE EXCEL
        protected void ExportGridToExcel(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = "Reporte Estudiantes " + DateTime.Now.ToString("G") + ".xls";
            StringWriter strwritter = new StringWriter();
            HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            GridViewReporteCT.GridLines = GridLines.Both;
            GridViewReporteCT.HeaderStyle.Font.Bold = true;
            GridViewReporteCT.RenderControl(htmltextwrtter);
            Response.Write(strwritter.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //required to avoid the run time error "  
            //Control 'GridViewReporteCT' of type 'Grid View' must be placed inside a form tag with runat=server."  
        }

        //Llenado de informacion a las columnas correspondientes del excel
        protected void GenerarExcel(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TxtBuscador.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text))

            {
                SLDocument sl = new SLDocument();
                int celda = 1;
                //Letras de las columnas para la generacion de excel
                string[] LETRA = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q" };

                //Texto plano
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Estudiantes " + DateTime.Now.ToString("G"));
                sl.SetCellValue("A" + celda, "Rule");
                celda++;
                sl.SetCellValue("A" + celda, "The items with asterisk are required.At least one of family name and given name is required.");
                celda++;
                sl.SetCellValue("A" + celda, "Do NOT change the layout and column title in this template file. The importing may fail if changed.");
                celda++;
                sl.SetCellValue("A" + celda, "Supports adding persons to the existing person group whose name is separated by slash. For example, the name format of Group A under All Persons is All Persons/Group A.");
                celda++;
                sl.SetCellValue("A" + celda, "Start/End Time of Effective Period: The effective period of the person for access control and time & attendance. Format: yyyy/mm/dd HH:MM:SS.");
                celda++;
                sl.SetCellValue("A" + celda, "Domain Person and Domain Group Person don't support adding and editing person's basic information and additional information by importing.");
                celda++;
                sl.SetCellValue("A" + celda, "No more than five cards can be issued to one person. Each two card numbers should be separated by semicolon, e.g., 01;02;03;04;05.");
                celda++;
                sl.SetCellValue("A" + celda, "It supports editing the persons' additional information in a batch, the fields of which are already created in the system. Please enter the additional information according to the type. For single selection type, select one from the drop-down list.");
                celda++;
                sl.SetCellValue("A" + celda, "Supports custom attribute input formats separated by commas, for example: attribute name 1, attribute name 2");
                celda++;

                //Cabeceras
                if (celda == 10)
                {
                    for (int k = 0; k < GridViewReporteCT.Columns.Count; k++)
                    {
                        sl.SetCellValue("A" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("B" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("C" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("D" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("E" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("F" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("G" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("H" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("I" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("J" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("K" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("L" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("M" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("N" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("O" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("P" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        sl.SetCellValue("Q" + celda, removeUnicode(GridViewReporteCT.Columns[k].ToString()));
                        k++;
                        celda++;
                    }
                }

                //Llenado de las columnas con la informacion
                if (celda > 10)
                {
                    var dt = new DataTable();
                    string where = stringWhere();
                    string constr = TxtURL.Text;

                    try
                    {
                        using (OracleConnection con = new OracleConnection(constr))
                        {
                            using (OracleCommand cmd = new OracleCommand())
                            {
                                cmd.CommandText = "SELECT " +
                                                "EMPLID, " +
                                                "FIRST_NAME, " +
                                                "LAST_NAME, " +
                                                "ID, " +
                                                "'Basic Person' TYPE, " +
                                                "PERSON_GROUP || Departamento PERSON_GROUP, " +
                                                "GENDER, " +
                                                "'' Start_Time_of_Effective_Period, " +
                                                "'' End_Time_of_Effective_Period, " +
                                                "'' CARD, " +
                                                "EMAIL, " +
                                                "PHONE, " +
                                                "'' Remark, " +
                                                "'' Dock_Station_Login_Password, " +
                                                "'' SupportIssuedCustomProperties, " +
                                                "'' SkinSurface_Temperature, " +
                                                "'' Temperature_Status, " +
                                                "DEPARTAMENTO " +
                                                "FROM " +
                                                "( " +
                                                "SELECT " +
                                                "DISTINCT PD.EMPLID, " +
                                                "PD.FIRST_NAME, " +
                                                "PD.LAST_NAME || CASE WHEN LTRIM(RTRIM(PD.SECOND_LAST_NAME)) IS NOT NULL THEN ' ' || LTRIM(RTRIM(PD.SECOND_LAST_NAME)) END LAST_NAME, " +
                                                "(SELECT PN2.NATIONAL_ID FROM SYSADM.PS_PERS_NID PN2 WHERE PD.EMPLID = PN2.EMPLID ORDER BY CASE WHEN PN2.NATIONAL_ID_TYPE = 'DPI' THEN 1 WHEN PN2.NATIONAL_ID_TYPE = 'PAS' THEN 2 WHEN PN2.NATIONAL_ID_TYPE = 'CED' THEN 3 ELSE 4 END FETCH FIRST 1 ROWS ONLY) ID, " +
                                                "CASE WHEN PD.SEX = 'F' THEN 'Female' WHEN PD.SEX = 'M' THEN 'Male' ELSE 'Unknown' END Gender, " +
                                                "PPD.PHONE, " +
                                                "(SELECT EMAIL.EMAIL_ADDR FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL WHERE EMAIL.EMPLID = PD.EMPLID AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG = 'Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR FETCH FIRST 1 ROWS ONLY) Email, " +
                                                "AGT.DESCR DEPARTAMENTO," +
                                                "APD.INSTITUTION || '/Estudiantes/' Person_Group, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'DPI' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'CER' THEN PN.NATIONAL_ID ELSE '' END DPI, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'CED' THEN PN.NATIONAL_ID ELSE '' END CEDULA, " +
                                                "CASE WHEN PN.NATIONAL_ID_TYPE = 'PAS' THEN PN.NATIONAL_ID WHEN PN.NATIONAL_ID_TYPE = 'EXT' THEN PN.NATIONAL_ID ELSE '' END PASAPORTE " +
                                                "FROM " +
                                                "SYSADM.PS_PERS_DATA_SA_VW PD " +
                                                "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                                "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                                "AND A.EFFDT =(SELECT MAX(EFFDT) FROM SYSADM.PS_ADDRESSES A2 WHERE A.EMPLID = A2.EMPLID AND A.ADDRESS_TYPE = A2.ADDRESS_TYPE) " +
                                                "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                                "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                                "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID " +
                                                "AND SE.STDNT_ENRL_STATUS = 'E' " +
                                                "AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                                "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID " +
                                                "AND CT.STRM = SE.STRM " +
                                                "AND CT.ACAD_CAREER = SE.ACAD_CAREER " +
                                                "AND SE.INSTITUTION = CT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                                "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                                "AND CT.INSTITUTION = APD.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                                "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                                "AND CT.INSTITUTION = TT.INSTITUTION " +
                                                "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                                where +
                                                ") " +
                                                "WHERE  " +
                                                "(ID = DPI " +
                                                "OR ID = PASAPORTE " +
                                                "OR ID = CEDULA )" +
                                                "ORDER BY " +
                                                "1 ASC ";
                                cmd.Connection = con;
                                con.Open();
                                OracleDataReader reader = cmd.ExecuteReader();
                                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                                if (reader.HasRows)
                                {
                                    int auxCelda = 1;
                                    adapter.Fill(dt);
                                    int contador = dt.Rows.Count;
                                    for (int i = 0; i < contador; i++)
                                    {
                                        for (int j = 1; j < 18; j++)
                                        {
                                            for (int k = 0; k < 17; k++)
                                            {
                                                sl.SetCellValue(LETRA[k] + celda, dt.Rows[i].ItemArray[j].ToString());
                                                j++;
                                            }
                                            celda++;
                                            auxCelda = auxCelda + 1;
                                        }
                                        celda = 10 + auxCelda;
                                    }
                                }
                                con.Close();
                            }
                        }
                    }
                    catch
                    {
                        lblBusqueda.Text = "No se encontró la información solicitada";
                    }
                }

                //Nombre del archivo
                string nombre = "Reporte Camara Termica Estudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".xlsx";
                //Lugar de almacenamiento
                sl.SaveAs(CurrentDirectory + "ReportesCT/" + nombre);
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
                Response.TransmitFile(CurrentDirectory + "ReportesCT/" + nombre);
            }
            else
            {
                lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
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
        protected string DownloadAllFile(string where)
        {
            string ret = "0";
            if (!String.IsNullOrEmpty(TxtBuscador.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text))
            {
                string nombre = "ImagenesEstudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".zip";
                string constr = TxtURL.Text;
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
                                            "LEFT JOIN SYSADM.PS_EMPL_PHOTO P ON P.EMPLID = PD.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_PERS_NID PN ON PD.EMPLID = PN.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_ADDRESSES A ON PD.EMPLID = A.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_PERSONAL_DATA PPD ON PD.EMPLID = PPD.EMPLID " +
                                            "LEFT JOIN SYSADM.PS_STATE_TBL ST ON PPD.STATE = ST.STATE " +
                                            "JOIN SYSADM.PS_STDNT_ENRL SE ON PD.EMPLID = SE.EMPLID " +
                                            "AND SE.STDNT_ENRL_STATUS = 'E' " +
                                            "AND SE.ENRL_STATUS_REASON = 'ENRL' " +
                                            "LEFT JOIN SYSADM.PS_STDNT_CAR_TERM CT ON SE.EMPLID = CT.EMPLID " +
                                            "AND CT.STRM = SE.STRM " +
                                            "AND CT.ACAD_CAREER = SE.ACAD_CAREER " +
                                            "AND SE.INSTITUTION = CT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_PROG_TBL APD ON CT.acad_prog_primary = APD.ACAD_PROG " +
                                            "AND CT.ACAD_CAREER = APD.ACAD_CAREER " +
                                            "AND CT.INSTITUTION = APD.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_ACAD_GROUP_TBL AGT ON APD.ACAD_GROUP = AGT.ACAD_GROUP " +
                                            "AND APD.INSTITUTION = AGT.INSTITUTION " +
                                            "LEFT JOIN SYSADM.PS_TERM_TBL TT ON CT.STRM = TT.STRM " +
                                            "AND CT.INSTITUTION = TT.INSTITUTION " +
                                            where +
                                            "AND employee_photo IS NOT NULL )" +
                                            "WHERE CNT =1";
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
            }
            else
            {
                lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga del archivo";
            }
            return ret;
        }

        protected void BtnImg_Click(object sender, EventArgs e)
        {
            string where = stringWhere();
            try
            {
                string respuesta = DownloadAllFile(where);
                if (respuesta == "0")
                {
                    lblBusqueda.Text = "Realice una búsqueda para poder realizar una descarga de fotografías";
                }
                else if (respuesta == "2")
                    lblBusqueda.Text = "No se encontraron imágenes relacionadas a los estudiantes.";
            }
            catch (Exception)
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
            var where = "";
            if (!String.IsNullOrEmpty(TxtBuscador.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text) && !String.IsNullOrEmpty(CldrCiclosFin.Text))

            {
                string busqueda = LbxBusqueda.Text;
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

                if (busqueda.Equals("Nombre"))
                {
                    where = "WHERE UPPER(PD.FIRST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Apellido"))
                {
                    where = "WHERE (UPPER(PD.LAST_NAME) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') ) AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("ID"))
                {
                    where = "WHERE UPPER(PN.NATIONAL_ID) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Departamento"))
                {
                    where = "WHERE UPPER(AGT.DESCR) LIKE('%" + TxtBuscador.Text.ToUpper() + "%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
                else if (LbxBusqueda.Text.Equals("Género"))
                {
                    string buscar = TxtBuscador.Text;
                    string min = buscar.ToLower();
                    if (min.Equals("male"))
                        where = "WHERE PD.SEX LIKE('%M%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                    else if (min.Equals("female"))
                        where = "WHERE PD.SEX LIKE ('%F%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                    else if (buscar == "%")
                        where = "WHERE PD.SEX LIKE ('%%%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                    else
                        where = "WHERE PD.SEX LIKE ('%Mujer%') AND ((TT.TERM_BEGIN_DT BETWEEN '" + inicio + "' AND '" + fin + "' OR TT.TERM_END_DT BETWEEN '" + inicio + "' AND '" + fin + "') OR (TT.TERM_BEGIN_DT <= '" + inicio + "'  AND TT.TERM_END_DT >= '" + fin + "' ))";
                }
            }
            return where;
        }

        protected void BtnNBusqueda_Click(object sender, EventArgs e)
        {
            LeerInfoTxt();
            BindGrid();
            BtnBuscar2.Enabled = true;
            BtnTxt.Enabled = false;
            BtnImg.Enabled = false;
            BtnNBusqueda.Enabled = false;
            TxtBuscador.Enabled = true;
            CldrCiclosFin.Enabled = true;
            CldrCiclosInicio.Enabled = true;
            lblBusqueda.Text = "";
            lblDescarga.Text = "";
            LbxBusqueda.Enabled = true;
        }
    }
}