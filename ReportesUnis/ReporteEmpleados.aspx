<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteEmpleados.aspx.cs" Inherits="ReportesUnis.ReporteEmpleados" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">REPORTE DE EMPLEADOS</h2>
    </div>
    <div class="container2">
        <hr />
        <div>
            <a>Busqueda por: </a>
            <%-- DROPDOWLIST CON OPCIONES DE BUSQUEDA --%>
            <asp:DropDownList ID="LbxBusqueda" Width=120px SelectionMode="Single" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="LbxBusqueda_SelectedIndexChanged">
                <%--<asp:ListItem Selected="True" Value=""></asp:ListItem>--%>
                <asp:ListItem>Nombre</asp:ListItem>
                <asp:ListItem>Apellido</asp:ListItem>
                <asp:ListItem>DPI</asp:ListItem>
                <asp:ListItem>Dependencia</asp:ListItem>
            </asp:DropDownList>

            <%-- TXTURL DONDE SE INGRESA EL TEXTO A BUSCAR --%>
            <asp:TextBox ID="TxtBuscador" runat="server" Width=245px></asp:TextBox>

            <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
            <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>

        </div>
        <br />
        <div style="text-align: center">

            <%-- CALENDARIO PARA BUSQUEDA --%>
            <asp:Label ID="FInicio" runat="server">Fecha inicio:</asp:Label>
            <asp:TextBox ID="CldrCiclosInicio" runat="server" TextMode="Date"> </asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="FFin" runat="server">Fecha fin:</asp:Label>
            <asp:TextBox ID="CldrCiclosFin" runat="server" TextMode="Date">
            </asp:TextBox>

        </div>
        <br />
        <div>
            <asp:CheckBox runat="server" ID="ChBusqueda" Checked="false" AutoPostBack="true" OnCheckedChanged="ChBusqueda_CheckedChanged" />
            <a>Busqueda multiple </a>
            <asp:DropDownList ID="LbxBusqueda2" Width=120px runat="server" SelectionMode="Single" AutoPostBack="true" EnableViewState="true" Visible="false" OnSelectedIndexChanged="LbxBusqueda2_SelectedIndexChanged">
            </asp:DropDownList>

            <%-- TXTBUSCADOR DONDE SE INGRESA EL TEXTO A BUSCAR --%>
            <asp:TextBox ID="TxtBuscador2" runat="server" Visible="false" Width=195px></asp:TextBox>

        </div>
        <br />
        <asp:Table runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" OnClick="BtnBuscar_Click" CssClass="btn-danger-unis" Width="130px"></asp:Button>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="Button1" runat="server" Text="Exportar TXT" OnClick="btnExport_Click" CssClass="btn-danger-unis" Enabled="false" Width="130px"/>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="ButtonFts" runat="server" Text="Exportar Fotos" OnClick="ButtonFts_Click" CssClass="btn-danger-unis" Enabled="false" Width="130px"/>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="BtnNBusqueda" runat="server" Text="Limpiar" OnClick="BtnNBusqueda_Click" CssClass="btn-danger-unis" Enabled="false" Width="145px"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <div style="text-align: center">
            <br />
            <asp:Label ID="lblBusqueda" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"></asp:Label>
            <asp:Label ID="lblDescarga" runat="server" Font-Bold="true" ForeColor="Blue" Text="" Font-Size="Large" Visible="false"></asp:Label>
        </div>
        <br />

    </div>

    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                <div class="containerGV" id="GVContainer">
                    <asp:GridView ID="GridViewReporte" runat="server"
                        AutoGenerateColumns="false" CssClass="table table-condensed table-bordered ">
                        <Columns>
                            <asp:BoundField DataField="" HeaderText="IDUNIV" />
                            <asp:BoundField DataField="NOM_IMP" HeaderText="NOM_IMP" />
                            <asp:BoundField DataField="Nombre1" HeaderText="NOM1" />
                            <asp:BoundField DataField="Nombre2" HeaderText="NOM2" />
                            <asp:BoundField DataField="Apellido1" HeaderText="APE1" />
                            <asp:BoundField DataField="Apellido2" HeaderText="APE2" />
                            <asp:BoundField DataField="Apellido3" HeaderText="APE3" />
                            <asp:BoundField DataField="Cumpleaños" HeaderText="FE_NAC" />
                            <asp:BoundField DataField="Sexo" HeaderText="SEXO" />
                            <asp:BoundField DataField="Estado Civil" HeaderText="EST_CIV" />
                            <asp:BoundField DataField="Nacionalidad" HeaderText="NACIONAL" />
                            <asp:BoundField DataField="FLAG_CED" HeaderText="FLAG_CED" />
                            <asp:BoundField DataField="Cedula" HeaderText="CEDULA" />
                            <asp:BoundField DataField="" HeaderText="DEPCED" />
                            <asp:BoundField DataField="" HeaderText="MUNCED" />
                            <asp:BoundField DataField="FLAG_DPI" HeaderText="FLAG_DPI" />
                            <asp:BoundField DataField="DPI" HeaderText="DPI" />
                            <asp:BoundField DataField="FLAG_PAS" HeaderText="FLAG_PAS" />
                            <asp:BoundField DataField="Pasaporte" HeaderText="PASS" />
                            <asp:BoundField DataField="" HeaderText="PAIS_PAS" />
                            <asp:BoundField DataField="NIT" HeaderText="NIT" />
                            <asp:BoundField DataField="" HeaderText="PAIS_NIT" />
                            <asp:BoundField DataField="" HeaderText="PROF" />
                            <asp:BoundField DataField="Direccion" HeaderText="DIR" />
                            <asp:BoundField DataField="" HeaderText="CASA" />
                            <asp:BoundField DataField="" HeaderText="APTO" />
                            <asp:BoundField DataField="ZONA" HeaderText="ZONA" />
                            <asp:BoundField DataField="" HeaderText="COL" />
                            <asp:BoundField DataField="Municipio" HeaderText="MUNRES" />
                            <asp:BoundField DataField="Departamento" HeaderText="DEPRES" />
                            <asp:BoundField DataField="Telefono" HeaderText="TEL" />
                            <asp:BoundField DataField="" HeaderText="CEL" />
                            <asp:BoundField DataField="EMAIL" HeaderText="EMAIL" />
                            <asp:BoundField DataField="CARNE" HeaderText="CARNET" />
                            <asp:BoundField DataField="" HeaderText="CARR" />
                            <asp:BoundField DataField="Dependencia" HeaderText="FACUL" />
                            <asp:BoundField DataField="" HeaderText="COD_EMP_U" />
                            <asp:BoundField DataField="" HeaderText="PUESTO" />
                            <asp:BoundField DataField="" HeaderText="DEP_EMP_U" />
                            <asp:BoundField DataField="" HeaderText="COD_BARRAS" />
                            <asp:BoundField DataField="" HeaderText="TIP_PER" />
                            <asp:BoundField DataField="" HeaderText="ACCION" />
                            <asp:BoundField DataField="" HeaderText="FOTO" />
                            <asp:BoundField DataField="" HeaderText="TIPO_CTA" />
                            <asp:BoundField DataField="" HeaderText="NO_CTA_BI" />
                            <asp:BoundField DataField="" HeaderText="F_U" />
                            <asp:BoundField DataField="" HeaderText="H_U" />
                            <asp:BoundField DataField="" HeaderText="TIP_ACC" />
                            <asp:BoundField DataField="" HeaderText="EMP_TRAB" />
                            <asp:BoundField DataField="" HeaderText="FEC_IN_TR" />
                            <asp:BoundField DataField="" HeaderText="ING_TR" />
                            <asp:BoundField DataField="" HeaderText="EGR_TR" />
                            <asp:BoundField DataField="" HeaderText="MONE_TR" />
                            <asp:BoundField DataField="" HeaderText="PUESTO_TR" />
                            <asp:BoundField DataField="" HeaderText="LUG_EMP" />
                            <asp:BoundField DataField="" HeaderText="FE_IN_EMP" />
                            <asp:BoundField DataField="" HeaderText="TEL_TR" />
                            <asp:BoundField DataField="" HeaderText="DIR_TR" />
                            <asp:BoundField DataField="" HeaderText="ZONA_TR" />
                            <asp:BoundField DataField="" HeaderText="DEP_TR" />
                            <asp:BoundField DataField="" HeaderText="MUNI_TR" />
                            <asp:BoundField DataField="" HeaderText="PAIS_TR" />
                            <asp:BoundField DataField="" HeaderText="ACT_EC" />
                            <asp:BoundField DataField="" HeaderText="OTRA_NA" />
                            <asp:BoundField DataField="" HeaderText="CONDMIG" />
                            <asp:BoundField DataField="" HeaderText="O_CONDMIG" />
                        </Columns>
                    </asp:GridView>

                </div>
                <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                <div class="containerGV" id="GVContainer">
                    <asp:GridView ID="GridView1" runat="server"
                        AutoGenerateColumns="false" CssClass="table table-condensed table-bordered ">
                        <Columns>
                            <asp:BoundField DataField="" HeaderText="IDUNIV" />
                            <asp:BoundField DataField="NOM_IMP" HeaderText="NOM_IMP" />
                            <asp:BoundField DataField="Nombre1" HeaderText="NOM1" />
                            <asp:BoundField DataField="Nombre2" HeaderText="NOM2" />
                            <asp:BoundField DataField="Apellido1" HeaderText="APE1" />
                            <asp:BoundField DataField="Apellido2" HeaderText="APE2" />
                            <asp:BoundField DataField="Apellido3" HeaderText="APE3" />
                            <asp:BoundField DataField="Cumpleaños" HeaderText="FE_NAC" />
                            <asp:BoundField DataField="Sexo" HeaderText="SEXO" />
                            <asp:BoundField DataField="Estado Civil" HeaderText="EST_CIV" />
                            <asp:BoundField DataField="Nacionalidad" HeaderText="NACIONAL" />
                            <asp:BoundField DataField="FLAG_CED" HeaderText="FLAG_CED" />
                            <asp:BoundField DataField="Cedula" HeaderText="CEDULA" />
                            <asp:BoundField DataField="" HeaderText="DEPCED" />
                            <asp:BoundField DataField="" HeaderText="MUNCED" />
                            <asp:BoundField DataField="FLAG_DPI" HeaderText="FLAG_DPI" />
                            <asp:BoundField DataField="DPI" HeaderText="DPI" />
                            <asp:BoundField DataField="FLAG_PAS" HeaderText="FLAG_PAS" />
                            <asp:BoundField DataField="Pasaporte" HeaderText="PASS" />
                            <asp:BoundField DataField="" HeaderText="PAIS_PAS" />
                            <asp:BoundField DataField="NIT" HeaderText="NIT" />
                            <asp:BoundField DataField="" HeaderText="PAIS_NIT" />
                            <asp:BoundField DataField="" HeaderText="PROF" />
                            <asp:BoundField DataField="Direccion" HeaderText="DIR" />
                            <asp:BoundField DataField="" HeaderText="CASA" />
                            <asp:BoundField DataField="" HeaderText="APTO" />
                            <asp:BoundField DataField="" HeaderText="ZONA" />
                            <asp:BoundField DataField="" HeaderText="COL" />
                            <asp:BoundField DataField="Municipio" HeaderText="MUNRES" />
                            <asp:BoundField DataField="Departamento" HeaderText="DEPRES" />
                            <asp:BoundField DataField="" HeaderText="TEL" />
                            <asp:BoundField DataField="Telefono" HeaderText="CEL" />
                            <asp:BoundField DataField="" HeaderText="EMAIL" />
                            <asp:BoundField DataField="CARNE" HeaderText="CARNET" />
                            <asp:BoundField DataField="" HeaderText="CARR" />
                            <asp:BoundField DataField="Dependencia" HeaderText="FACUL" />
                            <asp:BoundField DataField="" HeaderText="COD_EMP_U" />
                            <asp:BoundField DataField="" HeaderText="PUESTO" />
                            <asp:BoundField DataField="" HeaderText="DEP_EMP_U" />
                            <asp:BoundField DataField="" HeaderText="COD_BARRAS" />
                            <asp:BoundField DataField="" HeaderText="TIP_PER" />
                            <asp:BoundField DataField="" HeaderText="ACCION" />
                            <asp:BoundField DataField="" HeaderText="FOTO" />
                            <asp:BoundField DataField="" HeaderText="TIPO_CTA" />
                            <asp:BoundField DataField="" HeaderText="NO_CTA_BI" />
                            <asp:BoundField DataField="" HeaderText="F_U" />
                            <asp:BoundField DataField="" HeaderText="H_U" />
                            <asp:BoundField DataField="" HeaderText="TIP_ACC" />
                            <asp:BoundField DataField="" HeaderText="EMP_TRAB" />
                            <asp:BoundField DataField="" HeaderText="FEC_IN_TR" />
                            <asp:BoundField DataField="" HeaderText="ING_TR" />
                            <asp:BoundField DataField="" HeaderText="EGR_TR" />
                            <asp:BoundField DataField="" HeaderText="MONE_TR" />
                            <asp:BoundField DataField="" HeaderText="PUESTO_TR" />
                            <asp:BoundField DataField="" HeaderText="LUG_EMP" />
                            <asp:BoundField DataField="" HeaderText="FE_IN_EMP" />
                            <asp:BoundField DataField="" HeaderText="TEL_TR" />
                            <asp:BoundField DataField="" HeaderText="DIR_TR" />
                            <asp:BoundField DataField="" HeaderText="ZONA_TR" />
                            <asp:BoundField DataField="" HeaderText="DEP_TR" />
                            <asp:BoundField DataField="" HeaderText="MUNI_TR" />
                            <asp:BoundField DataField="" HeaderText="PAIS_TR" />
                            <asp:BoundField DataField="" HeaderText="ACT_EC" />
                            <asp:BoundField DataField="" HeaderText="OTRA_NA" />
                            <asp:BoundField DataField="" HeaderText="CONDMIG" />
                            <asp:BoundField DataField="" HeaderText="O_CONDMIG" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>

