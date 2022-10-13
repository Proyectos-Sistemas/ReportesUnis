using System;
using System.IO;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;
using System.Text;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.Web;
using SpreadsheetLight;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;

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
            string constr = TxtURL.Text;
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "SELECT " +
                    "EMPLID, FIRST_NAME, LAST_NAME, ID, TYPE, PERSON_GROUP||Departamento PERSON_GROUP, GENDER, " +
                    /*"MIN (Start_Time_of_Effective_Period) Start_Time_of_Effective_Period, " +
                    "MAX(End_Time_of_Effective_Period) End_Time_of_Effective_Period, " +*/
                    "CARD, EMAIL, PHONE, REMARK, DOCK_STATION_LOGIN_PASSWORD, SUPPORTISSUEDCUSTOMPROPERTIES, " +
                    "SKINSURFACE_TEMPERATURE, TEMPERATURE_STATUS, DEPARTAMENTO " +
                    "FROM (SELECT DISTINCT PD.EMPLID, PD.FIRST_NAME, " +
                    "PD.LAST_NAME || CASE WHEN LTRIM(RTRIM(PD.SECOND_LAST_NAME)) IS NOT NULL THEN ' ' || LTRIM(RTRIM(SECOND_LAST_NAME)) END LAST_NAME, " +
                    "(SELECT NID.NATIONAL_ID FROM SYSADM.PS_PERS_NID NID " +
                    "WHERE NID.EMPLID=PD.EMPLID AND NID.NATIONAL_ID_TYPE IN ('DPI','PAS')   " +
                    "ORDER BY CASE WHEN NID.NATIONAL_ID_TYPE='DPI' THEN 1 ELSE 2 END " +
                    "FETCH FIRST 1 ROWS ONLY) ID, 'Basic Person' TYPE, " +
                    "PROG_T.INSTITUTION||'/Estudiantes/' Person_Group, " +
                    "CASE WHEN SEX = 'F' THEN 'Female' WHEN SEX = 'M' THEN 'Male' " +
                    "ELSE 'Unknown' END Gender, " +
                    "TERM.TERM_BEGIN_DT Start_Time_of_Effective_Period, " +
                    "TERM.TERM_END_DT End_Time_of_Effective_Period, " +
                    "'' Card, " +
                    "(SELECT EMAIL.EMAIL_ADDR " +
                    "FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL " +
                    "WHERE EMAIL.EMPLID=PD.EMPLID " +
                    "AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' " +
                    "ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG='Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR " +
                    "FETCH FIRST 1 ROWS ONLY) Email, " +
                    "(SELECT PH.PHONE " +
                    "FROM SYSADM.PS_PERSONAL_PHONE PH " +
                    "WHERE PH.EMPLID=PD.EMPLID " +
                    "AND PH.PREF_PHONE_FLAG='Y' " +
                    "ORDER BY CASE WHEN PH.PREF_PHONE_FLAG='Y' THEN 1 ELSE 2 END, PH.PHONE " +
                    "FETCH FIRST 1 ROWS ONLY) Phone, " +
                    "'' Remark, " +
                    "'' Dock_Station_Login_Password, " +
                    "'' SupportIssuedCustomProperties, " +
                    "'' SkinSurface_Temperature, " +
                    "'' Temperature_Status, " +
                    "(SELECT PROG_T1.ACAD_GROUP " +
                    "FROM SYSADM.PS_PERS_DATA_SA_VW PD1 " +
                    "JOIN SYSADM.PS_STDNT_ENRL ENRL1 ON PD1.EMPLID=ENRL1.EMPLID AND ENRL1.STDNT_ENRL_STATUS='E' AND ENRL1.ENRL_STATUS_REASON='ENRL' " +
                    "JOIN SYSADM.PS_STDNT_CAR_TERM STERM1 ON STERM1.EMPLID=ENRL1.EMPLID AND STERM1.ACAD_CAREER=ENRL1.ACAD_CAREER AND STERM1.INSTITUTION=ENRL1.INSTITUTION AND STERM1.STRM=ENRL1.STRM " +
                    "JOIN SYSADM.PS_TERM_TBL TERM1 ON STERM1.STRM=TERM1.STRM AND STERM1.ACAD_CAREER = TERM1.ACAD_CAREER AND STERM1.INSTITUTION = TERM1.INSTITUTION " +
                    "JOIN SYSADM.PS_ACAD_PROG PROG1 ON PD1.EMPLID = PROG1.EMPLID AND STERM1.ACAD_CAREER=PROG1.ACAD_CAREER AND STERM1.INSTITUTION=PROG1.INSTITUTION AND ENRL1.ACAD_PROG=PROG1.ACAD_PROG AND PROG_ACTION='MATR' " +
                    "JOIN SYSADM.PS_ACAD_PROG_TBL PROG_T1 ON ENRL1.ACAD_PROG = PROG_T1.ACAD_PROG AND  (PROG_T1.EFFDT = (SELECT MAX(PROG_T3.EFFDT) " +
                    "FROM   SYSADM.PS_ACAD_PROG_TBL PROG_T3 " +
                    "WHERE  PROG_T1.INSTITUTION = PROG_T3.INSTITUTION " +
                    "AND PROG_T1.ACAD_PROG = PROG_T3.ACAD_PROG " +
                    "AND PROG_T3.EFFDT <= SYSDATE)) " +
                    "WHERE (((TO_DATE('01/01/22') BETWEEN TERM1.TERM_BEGIN_DT AND TERM1.TERM_END_DT) OR (TO_DATE('07/06/22') BETWEEN TERM1.TERM_BEGIN_DT AND TERM1.TERM_END_DT)) OR " +
                    "((TERM1.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')) AND (TERM1.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')))) " +
                    "AND PD1.EMPLID=PD.EMPLID " +
                    "ORDER BY PROG1.EFFDT ASC, CASE WHEN PROG1.ACAD_CAREER='PROG1.ACAD_CAREER' THEN 1 ELSE 2 END, PROG_T1.ACAD_GROUP " +
                    "FETCH FIRST 1 ROWS ONLY) " +
                    "Departamento " +
                    "FROM SYSADM.PS_PERS_DATA_SA_VW PD  " +
                    "JOIN SYSADM.PS_STDNT_ENRL ENRL ON PD.EMPLID=ENRL.EMPLID AND ENRL.STDNT_ENRL_STATUS='E' AND ENRL.ENRL_STATUS_REASON='ENRL'  " +
                    "JOIN SYSADM.PS_STDNT_CAR_TERM STERM ON STERM.EMPLID=ENRL.EMPLID AND STERM.ACAD_CAREER=ENRL.ACAD_CAREER AND STERM.INSTITUTION=ENRL.INSTITUTION AND STERM.STRM=ENRL.STRM  " +
                    "JOIN SYSADM.PS_TERM_TBL TERM ON STERM.STRM=TERM.STRM AND STERM.ACAD_CAREER = TERM.ACAD_CAREER AND STERM.INSTITUTION = TERM.INSTITUTION  " +
                    "JOIN SYSADM.PS_ACAD_PROG_TBL PROG_T ON ENRL.ACAD_PROG = PROG_T.ACAD_PROG AND  (PROG_T.EFFDT = (SELECT MAX(PROG_T2.EFFDT)  " +
                    "FROM   SYSADM.PS_ACAD_PROG_TBL PROG_T2  " +
                    "WHERE  PROG_T.INSTITUTION = PROG_T2.INSTITUTION  " +
                    "AND PROG_T.ACAD_PROG = PROG_T2.ACAD_PROG  " +
                    "AND PROG_T2.EFFDT <= SYSDATE))  " +
                    "WHERE (((TO_DATE('01/01/22') BETWEEN TERM.TERM_BEGIN_DT AND TERM.TERM_END_DT) OR (TO_DATE('07/06/22') BETWEEN TERM.TERM_BEGIN_DT AND TERM.TERM_END_DT)) OR  " +
                    "((TERM.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')) AND (TERM.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22'))))  " +
                    ") tblDatosAlumnos  " +
                    "GROUP BY EMPLID, FIRST_NAME, LAST_NAME, ID, TYPE, PERSON_GROUP||Departamento, GENDER, CARD, EMAIL, PHONE, REMARK, DOCK_STATION_LOGIN_PASSWORD, SUPPORTISSUEDCUSTOMPROPERTIES, SKINSURFACE_TEMPERATURE, TEMPERATURE_STATUS, DEPARTAMENTO  " +
                    "ORDER BY EMPLID, ID ";
                    cmd.Connection = con;
                    con.Open();
                    GridViewReporteCT.DataSource = cmd.ExecuteReader();
                    GridViewReporteCT.DataBind();
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

        //LLAMADA DE LA FUNCION PARA LA GENERACION DE BUSQUEDA
        protected void Busqueda(object sender, EventArgs e)
        {
            try
            {
                consultaBusqueda();
            }
            catch (Exception c)
            {
                BindGrid();
                lblBusqueda.Text = "No se encontró la información solicitada";
                return;
            }
        }

        //FUNCION PARA LA GENERACION DE CONSULTA A BD Y ASIGNACION A GRIDVIEW SIN BUSQUEDA
        public void consultaBusqueda()
        {
            var where = "";
            string busqueda = LbxBusqueda.Text;
            if (busqueda.Equals("Nombre"))
            {
                where = where = "WHERE FIRST_NAME LIKE('%" + TxtBuscador.Text + "%') ";
            }
            else if (LbxBusqueda.Text.Equals("Apellido"))
            {
                where = "WHERE LAST_NAME LIKE('%" + TxtBuscador.Text + "%') ";

            }
            else if (LbxBusqueda.Text.Equals("ID"))
            {
                where = "WHERE ID LIKE('%" + TxtBuscador.Text + "%') ";

            }
            else if (LbxBusqueda.Text.Equals("Departamento"))
            {
                where = "WHERE DEPARTAMENTO LIKE('%" + TxtBuscador.Text + "%') ";

            }
            else if (LbxBusqueda.Text.Equals("Género"))
            {
                string buscar = TxtBuscador.Text;
                string min = buscar.ToLower();
                if (min.Equals("male"))
                    where = "WHERE GENDER LIKE('%M%') ";
                else if (min.Equals("famale"))
                    where = "WHERE GENDER LIKE ('%F%') ";
            }

            string constr = TxtURL.Text;
            try
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.CommandText = "SELECT " +
                        "EMPLID, FIRST_NAME, LAST_NAME, ID, TYPE, PERSON_GROUP||Departamento PERSON_GROUP, GENDER, " +
                        "'' Start_Time_of_Effective_Period, " +
                        "'' End_Time_of_Effective_Period, " +
                        "CARD, EMAIL, PHONE, REMARK, DOCK_STATION_LOGIN_PASSWORD, SUPPORTISSUEDCUSTOMPROPERTIES, " +
                        "SKINSURFACE_TEMPERATURE, TEMPERATURE_STATUS, DEPARTAMENTO " +
                        "FROM (SELECT DISTINCT PD.EMPLID, PD.FIRST_NAME, " +
                        "PD.LAST_NAME || CASE WHEN LTRIM(RTRIM(PD.SECOND_LAST_NAME)) IS NOT NULL THEN ' ' || LTRIM(RTRIM(SECOND_LAST_NAME)) END LAST_NAME, " +
                        "(SELECT NID.NATIONAL_ID FROM SYSADM.PS_PERS_NID NID " +
                        "WHERE NID.EMPLID=PD.EMPLID AND NID.NATIONAL_ID_TYPE IN ('DPI','PAS')   " +
                        "ORDER BY CASE WHEN NID.NATIONAL_ID_TYPE='DPI' THEN 1 ELSE 2 END " +
                        "FETCH FIRST 1 ROWS ONLY) ID, 'Basic Person' TYPE, " +
                        "PROG_T.INSTITUTION||'/Estudiantes/' Person_Group, " +
                        "CASE WHEN SEX = 'F' THEN 'Female' WHEN SEX = 'M' THEN 'Male' " +
                        "ELSE 'Unknown' END Gender, " +
                        "TERM.TERM_BEGIN_DT Start_Time_of_Effective_Period, " +
                        "TERM.TERM_END_DT End_Time_of_Effective_Period, " +
                        "'' Card, " +
                        "(SELECT EMAIL.EMAIL_ADDR " +
                        "FROM SYSADM.PS_EMAIL_ADDRESSES EMAIL " +
                        "WHERE EMAIL.EMPLID=PD.EMPLID " +
                        "AND UPPER(EMAIL.EMAIL_ADDR) LIKE '%UNIS.EDU.GT%' " +
                        "ORDER BY CASE WHEN EMAIL.PREF_EMAIL_FLAG='Y' THEN 1 ELSE 2 END, EMAIL.EMAIL_ADDR " +
                        "FETCH FIRST 1 ROWS ONLY) Email, " +
                        "(SELECT PH.PHONE " +
                        "FROM SYSADM.PS_PERSONAL_PHONE PH " +
                        "WHERE PH.EMPLID=PD.EMPLID " +
                        "AND PH.PREF_PHONE_FLAG='Y' " +
                        "ORDER BY CASE WHEN PH.PREF_PHONE_FLAG='Y' THEN 1 ELSE 2 END, PH.PHONE " +
                        "FETCH FIRST 1 ROWS ONLY) Phone, " +
                        "'' Remark, " +
                        "'' Dock_Station_Login_Password, " +
                        "'' SupportIssuedCustomProperties, " +
                        "'' SkinSurface_Temperature, " +
                        "'' Temperature_Status, " +
                        "(SELECT PROG_T1.ACAD_GROUP " +
                        "FROM SYSADM.PS_PERS_DATA_SA_VW PD1 " +
                        "JOIN SYSADM.PS_STDNT_ENRL ENRL1 ON PD1.EMPLID=ENRL1.EMPLID AND ENRL1.STDNT_ENRL_STATUS='E' AND ENRL1.ENRL_STATUS_REASON='ENRL' " +
                        "JOIN SYSADM.PS_STDNT_CAR_TERM STERM1 ON STERM1.EMPLID=ENRL1.EMPLID AND STERM1.ACAD_CAREER=ENRL1.ACAD_CAREER AND STERM1.INSTITUTION=ENRL1.INSTITUTION AND STERM1.STRM=ENRL1.STRM " +
                        "JOIN SYSADM.PS_TERM_TBL TERM1 ON STERM1.STRM=TERM1.STRM AND STERM1.ACAD_CAREER = TERM1.ACAD_CAREER AND STERM1.INSTITUTION = TERM1.INSTITUTION " +
                        "JOIN SYSADM.PS_ACAD_PROG PROG1 ON PD1.EMPLID = PROG1.EMPLID AND STERM1.ACAD_CAREER=PROG1.ACAD_CAREER AND STERM1.INSTITUTION=PROG1.INSTITUTION AND ENRL1.ACAD_PROG=PROG1.ACAD_PROG AND PROG_ACTION='MATR' " +
                        "JOIN SYSADM.PS_ACAD_PROG_TBL PROG_T1 ON ENRL1.ACAD_PROG = PROG_T1.ACAD_PROG AND  (PROG_T1.EFFDT = (SELECT MAX(PROG_T3.EFFDT) " +
                        "FROM   SYSADM.PS_ACAD_PROG_TBL PROG_T3 " +
                        "WHERE  PROG_T1.INSTITUTION = PROG_T3.INSTITUTION " +
                        "AND PROG_T1.ACAD_PROG = PROG_T3.ACAD_PROG " +
                        "AND PROG_T3.EFFDT <= SYSDATE)) " +
                        "WHERE (((TO_DATE('01/01/22') BETWEEN TERM1.TERM_BEGIN_DT AND TERM1.TERM_END_DT) OR (TO_DATE('07/06/22') BETWEEN TERM1.TERM_BEGIN_DT AND TERM1.TERM_END_DT)) OR " +
                        "((TERM1.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')) AND (TERM1.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')))) " +
                        "AND PD1.EMPLID=PD.EMPLID " +
                        "ORDER BY PROG1.EFFDT ASC, CASE WHEN PROG1.ACAD_CAREER='PROG1.ACAD_CAREER' THEN 1 ELSE 2 END, PROG_T1.ACAD_GROUP " +
                        "FETCH FIRST 1 ROWS ONLY) " +
                        "Departamento " +
                        "FROM SYSADM.PS_PERS_DATA_SA_VW PD  " +
                        "JOIN SYSADM.PS_STDNT_ENRL ENRL ON PD.EMPLID=ENRL.EMPLID AND ENRL.STDNT_ENRL_STATUS='E' AND ENRL.ENRL_STATUS_REASON='ENRL'  " +
                        "JOIN SYSADM.PS_STDNT_CAR_TERM STERM ON STERM.EMPLID=ENRL.EMPLID AND STERM.ACAD_CAREER=ENRL.ACAD_CAREER AND STERM.INSTITUTION=ENRL.INSTITUTION AND STERM.STRM=ENRL.STRM  " +
                        "JOIN SYSADM.PS_TERM_TBL TERM ON STERM.STRM=TERM.STRM AND STERM.ACAD_CAREER = TERM.ACAD_CAREER AND STERM.INSTITUTION = TERM.INSTITUTION  " +
                        "JOIN SYSADM.PS_ACAD_PROG_TBL PROG_T ON ENRL.ACAD_PROG = PROG_T.ACAD_PROG AND  (PROG_T.EFFDT = (SELECT MAX(PROG_T2.EFFDT)  " +
                        "FROM   SYSADM.PS_ACAD_PROG_TBL PROG_T2  " +
                        "WHERE  PROG_T.INSTITUTION = PROG_T2.INSTITUTION  " +
                        "AND PROG_T.ACAD_PROG = PROG_T2.ACAD_PROG  " +
                        "AND PROG_T2.EFFDT <= SYSDATE))  " +
                        "WHERE (((TO_DATE('01/01/22') BETWEEN TERM.TERM_BEGIN_DT AND TERM.TERM_END_DT) OR (TO_DATE('07/06/22') BETWEEN TERM.TERM_BEGIN_DT AND TERM.TERM_END_DT)) OR  " +
                        "((TERM.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22')) AND (TERM.TERM_BEGIN_DT BETWEEN TO_DATE('01/01/22') AND TO_DATE('07/06/22'))))  " +
                        ") tblDatosAlumnos  " +
                        where +
                        "GROUP BY EMPLID, FIRST_NAME, LAST_NAME, ID, TYPE, PERSON_GROUP||Departamento, GENDER, CARD, EMAIL, PHONE, REMARK, DOCK_STATION_LOGIN_PASSWORD, SUPPORTISSUEDCUSTOMPROPERTIES, SKINSURFACE_TEMPERATURE, TEMPERATURE_STATUS, DEPARTAMENTO  " +
                        "ORDER BY EMPLID, ID ";
                        cmd.Connection = con;
                        con.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            GridViewReporteCT.DataSource = cmd.ExecuteReader();
                            GridViewReporteCT.DataBind();
                            lblBusqueda.Text = "";
                            TxtBuscador.Text = "";
                        }
                        else
                        {
                            BindGrid();
                            lblBusqueda.Text = "No se encontró la información solicitada";
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

            if (String.IsNullOrEmpty(LbxBusqueda.Text))
            {
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
                    for (int k = 0; k < 17; k++)
                    {
                        for (int j = 0; j < GridViewReporteCT.Columns.Count; j++)
                        {
                            for (int i = 0; i < GridViewReporteCT.Rows.Count; i++)
                            {
                                string texto = removeUnicode(GridViewReporteCT.Rows[i].Cells[j].Text);
                                sl.SetCellValue(LETRA[k] + celda, texto);
                                celda++;
                            }
                            celda = celda - GridViewReporteCT.Rows.Count;
                            k++;
                        }
                    }
                }
            }
            else
            {
                //consultaBusqueda();
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
                    for (int k = 0; k < 17; k++)
                    {
                        for (int j = 0; j < GridViewReporteCT.Columns.Count-1; j++)
                        {
                            for (int i = 0; i < GridViewReporteCT.Rows.Count; i++)
                            {
                                string texto = removeUnicode(GridViewReporteCT.Rows[i].Cells[j].Text);
                                if (texto.Equals("&nbsp;"))
                                    texto = " ";
                                sl.SetCellValue(LETRA[k] + celda, texto);
                                celda++;
                            }
                            celda = celda - GridViewReporteCT.Rows.Count;
                            k++;
                        }
                    }
                }
            }
            //Nombre del archivo
            string nombre = "Reporte Camara Termica Estudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".xlsx";
            //Lugar de almacenamiento
            sl.SaveAs(CurrentDirectory + "ReportesCT/" + nombre);
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
            Response.TransmitFile(CurrentDirectory + "ReportesCT/" + nombre);
            //Apertura del archivo
            //Process.Start(CurrentDirectory + "ReportesCT/" + nombre);
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
            string nombre = "ImagenesEstudiantes" + DateTime.Now.ToString("dd MM yyyy hh_mm_ss t") + ".zip";
            string constr = TxtURL.Text;
            string ret = "0";
            where = where.TrimEnd(',');
            int total = 0;
            DataSetLocalRpt dsDownload = new DataSetLocalRpt();
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd1 = new OracleCommand())
                {
                    cmd1.CommandText = "SELECT Count(*) FROM SYSADM.PS_EMPL_PHOTO P WHERE EMPLID in (" + where + ") AND employee_photo IS NOT NULL ";
                    cmd1.Connection = con;
                    con.Open();
                    OracleDataReader reader = cmd1.ExecuteReader();
                    string getValue = cmd1.ExecuteScalar().ToString();
                    total = Convert.ToInt32(getValue);
                    con.Close();
                }
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

                        }
                        con.Close();


                        string folder = AppDomain.CurrentDomain.BaseDirectory + nombre;
                        File.Create(folder).Close();

                        using (FileStream zipToOpen = new FileStream(folder, FileMode.Open))
                        {

                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                for (int i = 0; i < total; i++)
                                {
                                    ZipArchiveEntry readmeEntry = archive.CreateEntry(dsDownload.Tables["AllDownload"].Rows[i]["fileName"].ToString());
                                }
                            }
                        }

                        Response.ContentType = "application/zip";
                        Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
                        Response.TransmitFile(AppDomain.CurrentDomain.BaseDirectory + nombre);
                        ret = "1";
                    }
                }
            }
            return ret;
        }

        protected void BtnImg_Click(object sender, EventArgs e)
        {
            try
            {
                ////AGREGA EL NOMBRE DE LAS COLUMNAS AL ARCHIVO.  
                string emplid = "";
                int total = 0;
                for (int k = 0; k < GridViewReporteCT .Rows.Count; k++)
                {
                    emplid += "'" + removeUnicode(GridViewReporteCT.Rows[k].Cells[17].Text) + "',";
                }

                string respuesta = DownloadAllFile(emplid);
                if (respuesta == "0")
                    lblBusqueda.Text = "No se encontrarón imagenes según la busqueda realizada";
            }
            catch (Exception x)
            {
                lblBusqueda.Text = "Ha ocurido un error";
            }
        }
    }
}