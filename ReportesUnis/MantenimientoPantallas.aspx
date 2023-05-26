<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MantenimientoPantallas.aspx.cs" Inherits="ReportesUnis.MantenimientoPantallas" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">MANTENIMIENTO</h2>
    </div>
    <hr />
    <div class="container">
        <h4 style="text-align: center;">Configuración de Habilitación de Fechas</h4>

        <asp:Table id="tabla" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- FECHA INICIO LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Fecha Inicio:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FECHA INICIO  4--%>
                <asp:TableCell>
                    <asp:TextBox ID="DTInicio" runat="server"  TextMode="Date" placeholder="yyyy-mm-dd" AutoPostBack="true" OnTextChanged="DTInicio_TextChanged" ></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FECHA FIN LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Fecha Fin:</asp:Label>
                </asp:TableCell>

                <%-- ESPACIO --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FECHA FIN --%>
                <asp:TableCell>
                    <asp:TextBox ID="DTFin" runat="server"  TextMode="Date" placeholder="yyyy-mm-dd"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- TIPO LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Tipo:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- TIPO  12--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbTipo" runat="server" Width="220px">
                        <asp:ListItem>Semana</asp:ListItem>
                        <asp:ListItem>Carnetización Masiva</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>

                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>           
        
        <asp:Table id="tbactualizar" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 2.1--%>
                <asp:TableCell>
                    <asp:Button ID="BtnInsertar" runat="server" Text="Ingresar Fecha" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnInsertar_Click"/>
                </asp:TableCell>
                <%-- ESPACIO 2.2--%>
                <asp:TableCell>
                    <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar Fecha" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnActualizar_Click"/>
                </asp:TableCell>
                <%-- ESPACIO 2.3--%>
                <asp:TableCell>
                    <asp:Button ID="BtnEliminar" runat="server" Text="Eliminar Fecha" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnEliminar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 4--%>
                <asp:TableCell HorizontalAlign="Center">
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <hr />

    <div id="CamposAuxiliares" runat="server" visible="true">
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:Label ID="TxtURL" runat="server" Visible="false"></asp:Label>
        <%-- TXTINICIO SE UTILIZA PARA VISUALIZAR FECHA --%>
        <asp:Label ID="TXTINICIO" runat="server" Visible="true"></asp:Label>
    </div>

    <asp:Table id="TblGrid" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell  Width="50%">
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 2--%>
                <asp:TableCell>
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DE LOS TIEMPOS DE CARNE --%>
                                <div class="containerGV" id="GVContainer">
                                    <asp:GridView ID="GridViewReporte" runat="server"
                                    AutoGenerateColumns="false" CssClass="table table-condensed table-bordered ">
                                        <Columns>
                                            <asp:BoundField DataField=" ID_REGISTRO" HeaderText=" " Visible="false" />
                                            <asp:BoundField DataField="FECHA_INICIO" HeaderText="FECHA INICIO" />
                                            <asp:BoundField DataField="FECHA_FIN" HeaderText="FECHA FIN" />
                                            <asp:BoundField DataField="PANTALLA" HeaderText="TIPO" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 3--%>
                <asp:TableCell  Width="50%">
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    
    <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;",>
            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
            </asp:Label>
    </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>