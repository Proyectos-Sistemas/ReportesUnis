<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfirmaciónDeCarne.aspx.cs" Inherits="ReportesUnis.ConfirmaciónDeCarne" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">CARNETIZACIÓN ESTUDIANTES</h2>
    </div>
    <hr />
    <div id="CamposAuxiliares" runat="server" visible="false">
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTINICIO SE UTILIZA PARA VISUALIZAR FECHA --%>
        <asp:TextBox ID="TXTINICIO" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTPath ALMACENA EL PATH DONDE SE ALMACENARA LA IMAGEN --%>
        <asp:Label ID="txtPath" runat="server" Visible="false"></asp:Label>
        <%-- TXTURLSQL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:Label ID="TxtURLSql" runat="server" Visible="false"></asp:Label>
        <%-- TXTEXISTE ALMACENA vALORES PARA HACER VALIDACIONES --%>
        <asp:TextBox ID="txtExiste" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
        <asp:TextBox ID="txtInsertBI" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT DEL NOMBRE --%>
        <asp:TextBox ID="txtInsertName" runat="server" Visible="false"></asp:TextBox>
        <%-- txtInsertApexI ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
        <asp:TextBox ID="txtInsertApex" runat="server" Visible="false"></asp:TextBox>

        <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
        <input type="hidden" id="EFFDT_A_NIT_AC" runat="server" />
        <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
        <input type="hidden" id="EFFDT_NameR_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
        <input type="hidden" id="UP_NAMES_NIT_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
        <input type="hidden" id="UP_ADDRESSES_NIT_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRF--%>
        <input type="hidden" id="UP_NAMES_PRF_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRI--%>
        <input type="hidden" id="UP_NAMES_PRI_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
        <input type="hidden" id="UD_NAMES_NIT_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
        <input type="hidden" id="UD_ADDRESSES_NIT_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRF_AC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRI_AC" runat="server" />

        <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
        <input type="hidden" id="EFFDT_A_NIT_PC" runat="server" />
        <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
        <input type="hidden" id="EFFDT_NameR_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
        <input type="hidden" id="UP_NAMES_NIT_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
        <input type="hidden" id="UP_ADDRESSES_NIT_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRF--%>
        <input type="hidden" id="UP_NAMES_PRF_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRI--%>
        <input type="hidden" id="UP_NAMES_PRI_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
        <input type="hidden" id="UD_NAMES_NIT_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
        <input type="hidden" id="UD_ADDRESSES_NIT_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRF_PC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRI_PC" runat="server" />

        <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
        <input type="hidden" id="EFFDT_A_NIT_RC" runat="server" />
        <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
        <input type="hidden" id="EFFDT_NameR_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
        <input type="hidden" id="UP_NAMES_NIT_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
        <input type="hidden" id="UP_ADDRESSES_NIT_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRF--%>
        <input type="hidden" id="UP_NAMES_PRF_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRI--%>
        <input type="hidden" id="UP_NAMES_PRI_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
        <input type="hidden" id="UD_NAMES_NIT_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
        <input type="hidden" id="UD_ADDRESSES_NIT_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRF_RC" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRI_RC" runat="server" />
        <input type="hidden" id="VersionUP" runat="server" />
        <input type="hidden" id="VersionUD" runat="server" />
        <%-- TxtCantidad, almacena la cantidad de imagenes almacenadas --%>
        <asp:Label ID="txtCantidadAC" runat="server" Visible="false">0</asp:Label>
        <asp:Label ID="txtCantidadPC" runat="server" Visible="false">0</asp:Label>
        <asp:Label ID="txtCantidadRC" runat="server" Visible="false">0</asp:Label>

        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER CONTROL NOMBRES RECIBO --%>
        <asp:TextBox ID="txtUpdateAR" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="txtUpdateNR" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="txtControlNR" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="txtControlAR" runat="server" Visible="false"></asp:TextBox>

        <%-- TEXTBOX ALMACENA CONTROL DE TABS--%>
        <input type="hidden" id="ControlTabs" runat="server" />
    </div>

    <!-- Código ASP.NET para las pestañas -->
    <div class="tab">
        <asp:Button Text="Actualización" ID="Tab1" CssClass="tablinks" runat="server" OnClick="Tab1_Click" />
        <asp:Button Text="Primer Carne" ID="Tab2" CssClass="tablinks" runat="server" OnClick="Tab2_Click" />
        <asp:Button Text="Renovación de Carne" ID="Tab3" CssClass="tablinks" runat="server" OnClick="Tab3_Click" />
    </div>
    <br />
    <asp:MultiView ID="MainView" runat="server">
        <asp:View ID="View1" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <div class="container" id="divConfirmarAC" runat="server">
                            <div class="row">
                                <div class="col-md">
                                    <div class="container">
                                        <div class="container">
                                            <div class="row">
                                                <div class="col-md-4 mx-auto text-center">
                                                </div>
                                                <div class="col-md-4 mx-auto text-center">
                                                    <asp:Label runat="server" Font-Bold="true">CARNE:</asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="CmbCarneAC" runat="server" AutoPostBack="true" OnTextChanged="CmbTipo_SelectedIndexChangedAC" EnableViewState="true" Width="150">
                                                    </asp:DropDownList>
                                                    <br />
                                                </div>
                                                <div class="col-md-4 mx-auto text-center">
                                                    <br />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />

                        <div class="container" id="divCamposAC" runat="server">
                            <div class="row">
                                <div class="col-md">
                                    <div class="container">
                                        <div class="container">

                                            <div class="row">

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">DPI/PASAPORTE:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtDpiAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">PRIMER NOMBRE:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtPrimerNombreAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">SEGUNDO NOMBRE:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtSegundoNombreAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>


                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">APELLIDO 1:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtPrimerApellidoAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">APELLIDO 2:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtSegundoApellidoAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">APELLIDO DE CASADA:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtApellidoCasadaAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>



                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">CARRERA:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtCarreraAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">FACULTAD:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtFacultadAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">TELEFONO:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtTelAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>



                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">FECHA DE NACIMIENTO:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtFechaNacAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtEstadoAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtDireccionAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>



                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">PAIS:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtPaisAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">DEPARTAMENTO:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtDepartamentoAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">MUNICIPIO:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtMunicipioAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>



                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">CORREO PERSONAL:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtCorreoPersonalAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <br />
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">CORREO INSTITUCIONAL:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="TxtCorreoInstitucionalAC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="container" id="divDPIAC" runat="server" visible="false">
                            <h4 style="text-align: center;" runat="server" visible="true" id="HDocumentacion">Documentación Adjunta</h4>
                            <asp:Table ID="tabla5" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                <asp:TableRow>
                                    <%-- ESPACIO 1--%>
                                    <asp:TableCell Width="25%">
                                            <br />
                                    </asp:TableCell>

                                    <%-- ESPACIO 2--%>
                                    <asp:TableCell Width="25%">
                                        <asp:Image ID="ImgDPI1AC" runat="server" Width="350px" Visible="false" />
                                    </asp:TableCell>

                                    <%-- ESPACIO 3--%>
                                    <asp:TableCell Width="25%">
                                        <br />  
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <%-- ESPACIO 1--%>
                                    <asp:TableCell Width="25%">
                                            <br />
                                    </asp:TableCell>

                                    <%-- ESPACIO 2--%>
                                    <asp:TableCell Width="25%">
                                        <asp:Image ID="ImgDPI2AC" runat="server" Width="350px" Visible="false" />
                                    </asp:TableCell>

                                    <%-- ESPACIO 3--%>
                                    <asp:TableCell Width="25%">
                                            <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </div>
                        <div class="container" id="divFotografiaAC" runat="server" visible="false">
                            <h4 style="text-align: center;" runat="server" visible="true" id="HFoto">Fotografía Adjunta</h4>
                            <asp:Table ID="tabla6" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                <asp:TableRow>
                                    <%-- ESPACIO 1--%>
                                    <asp:TableCell Width="25%">
                                            <br />
                                    </asp:TableCell>

                                    <%-- ESPACIO 2--%>
                                    <asp:TableCell>
                                        <asp:Image ID="ImgFoto1AC" runat="server" Width="350px" />
                                    </asp:TableCell>

                                    <%-- ESPACIO 3--%>
                                    <asp:TableCell Width="25%">
                                            <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </div>

                        <div class="container" id="divBtnConfirmarAC" runat="server" visible="false">
                            <asp:Table ID="TbBtnsConfirmarAC" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                <asp:TableRow>
                                    <%-- ESPACIO 1--%>
                                    <asp:TableCell>
                                            <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <%-- ESPACIO 2.1--%>
                                    <asp:TableCell>
                                        <asp:Button ID="BtnConfirmarAC" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaAceptarAC();" OnClick="BtnConfirmarAC_Click" />
                                    </asp:TableCell>
                                    <%-- ESPACIO 2.2--%>
                                    <asp:TableCell>
                                        <asp:Button ID="BtnRechazarAC" runat="server" Text="Rechazar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaRechazoAC();" OnClick="BtnRechazarAC_Click" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <%-- ESPACIO 3--%>
                                    <asp:TableCell HorizontalAlign="Center">
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </div>

                        <div class="modal" id="myModalActualizacionAC" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
                            <div class="modal-dialog" role="document">
                                <div class="modal-content">
                                    <div class="row">
                                        <div class="col-md-4 mx-auto text-center">
                                        </div>
                                        <div class="col-md-4 mx-auto text-center">
                                            <div class="modal-spinnerCarne">
                                                <div class="spinner"></div>
                                            </div>
                                        </div>
                                        <div class="col-md-4 mx-auto text-center">
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 mx-auto text-center">
                                            <div class="modal-messageCarne">Por favor, espera mientras la información se está actualizando...</div>
                                            <div style="margin-bottom: 20px;"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;">
                            <asp:Label ID="lblActualizacionAC" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                            </asp:Label>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                            <tr>
                                <td>
                                    <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                                        <tr>
                                            <td>
                                                <div class="container" id="divConfirmarPC" runat="server">
                                                    <div class="row">
                                                        <div class="col-md">
                                                            <div class="container">
                                                                <div class="container">
                                                                    <div class="row">
                                                                        <div class="col-md-4 mx-auto text-center">
                                                                        </div>
                                                                        <div class="col-md-4 mx-auto text-center">
                                                                            <asp:Label runat="server" Font-Bold="true">CARNE:</asp:Label>
                                                                            <br />
                                                                            <asp:DropDownList ID="CmbCarnePC" runat="server" AutoPostBack="true" OnTextChanged="CmbTipo_SelectedIndexChangedPC" EnableViewState="true" Width="150">
                                                                            </asp:DropDownList>
                                                                            <br />
                                                                        </div>
                                                                        <div class="col-md-4 mx-auto text-center">
                                                                            <br />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />

                                                <div class="container" id="divCamposPC" runat="server">
                                                    <div class="row">
                                                        <div class="col-md">
                                                            <div class="container">
                                                                <div class="container">

                                                                    <div class="row">

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">DPI/PASAPORTE:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtDpiPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">PRIMER NOMBRE:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtPrimerNombrePC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">SEGUNDO NOMBRE:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtSegundoNombrePC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>


                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">APELLIDO 1:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtPrimerApellidoPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">APELLIDO 2:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtSegundoApellidoPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">APELLIDO DE CASADA:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtApellidoCasadaPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>



                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">CARRERA:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtCarreraPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">FACULTAD:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtFacultadPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">TELEFONO:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtTelPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>



                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">FECHA DE NACIMIENTO:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtFechaNacPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtEstadoPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtDireccionPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>



                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">PAIS:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtPaisPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">DEPARTAMENTO:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtDepartamentoPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">MUNICIPIO:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtMunicipioPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>



                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">CORREO PERSONAL:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtCorreoPersonalPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <br />
                                                                        </div>

                                                                        <div class="form-group col-md-4">
                                                                            <asp:Label runat="server" Font-Bold="true">CORREO INSTITUCIONAL:</asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="TxtCorreoInstitucionalPC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="container" id="divDPIPC" runat="server" visible="false">
                                                    <h4 style="text-align: center;" runat="server" visible="true" id="H1">Documentación Adjunta</h4>
                                                    <asp:Table ID="Table1" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 1--%>
                                                            <asp:TableCell Width="25%">
                                            <br />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 2--%>
                                                            <asp:TableCell Width="25%">
                                                                <asp:Image ID="ImgDPI1PC" runat="server" Width="350px" Visible="false" />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 3--%>
                                                            <asp:TableCell Width="25%">
                                        <br />  
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 1--%>
                                                            <asp:TableCell Width="25%">
                                            <br />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 2--%>
                                                            <asp:TableCell Width="25%">
                                                                <asp:Image ID="ImgDPI2PC" runat="server" Width="350px" Visible="false" />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 3--%>
                                                            <asp:TableCell Width="25%">
                                            <br />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </div>
                                                <div class="container" id="divFotografiaPC" runat="server" visible="false">
                                                    <h4 style="text-align: center;" runat="server" visible="true" id="H2">Fotografía Adjunta</h4>
                                                    <asp:Table ID="Table2" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 1--%>
                                                            <asp:TableCell Width="25%">
                                            <br />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 2--%>
                                                            <asp:TableCell>
                                                                <asp:Image ID="ImgFoto1PC" runat="server" Width="350px" />
                                                            </asp:TableCell>

                                                            <%-- ESPACIO 3--%>
                                                            <asp:TableCell Width="25%">
                                            <br />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </div>

                                                <div class="container" id="divBtnConfirmarPC" runat="server" visible="false">
                                                    <asp:Table ID="TbBtnsConfirmarPC" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 1--%>
                                                            <asp:TableCell>
                                            <br />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 2.1--%>
                                                            <asp:TableCell>
                                                                <asp:Button ID="BtnConfirmarPC" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaAceptarPC();" OnClick="BtnConfirmarPC_Click" />
                                                            </asp:TableCell>
                                                            <%-- ESPACIO 2.2--%>
                                                            <asp:TableCell>
                                                                <asp:Button ID="BtnRechazarPC" runat="server" Text="Rechazar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaRechazoPC();" OnClick="BtnRechazarPC_Click" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 3--%>
                                                            <asp:TableCell HorizontalAlign="Center">
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </div>

                                                <div class="modal" id="myModalActualizacionPC" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
                                                    <div class="modal-dialog" role="document">
                                                        <div class="modal-content">
                                                            <div class="row">
                                                                <div class="col-md-4 mx-auto text-center">
                                                                </div>
                                                                <div class="col-md-4 mx-auto text-center">
                                                                    <div class="modal-spinnerCarne">
                                                                        <div class="spinner"></div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-md-4 mx-auto text-center">
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-12 mx-auto text-center">
                                                                    <div class="modal-messageCarne">Por favor, espera mientras la información se está actualizando...</div>
                                                                    <div style="margin-bottom: 20px;"></div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;">
                                                    <asp:Label ID="lblActualizacionPC" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                                                    </asp:Label>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View3" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                            <tr>
                                <td>
                                    <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                                        <tr>
                                            <td>
                                                <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                                                    <tr>
                                                        <td>
                                                            <div class="container" id="divConfirmarRC" runat="server">
                                                                <div class="row">
                                                                    <div class="col-md">
                                                                        <div class="container">
                                                                            <div class="container">
                                                                                <div class="row">
                                                                                    <div class="col-md-4 mx-auto text-center">
                                                                                    </div>
                                                                                    <div class="col-md-4 mx-auto text-center">
                                                                                        <asp:Label runat="server" Font-Bold="true">CARNE:</asp:Label>
                                                                                        <br />
                                                                                        <asp:DropDownList ID="CmbCarneRC" runat="server" AutoPostBack="true" OnTextChanged="CmbTipo_SelectedIndexChangedRC" EnableViewState="true" Width="150">
                                                                                        </asp:DropDownList>
                                                                                        <br />
                                                                                    </div>
                                                                                    <div class="col-md-4 mx-auto text-center">
                                                                                        <br />
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <br />

                                                            <div class="container" id="divCamposRC" runat="server">
                                                                <div class="row">
                                                                    <div class="col-md">
                                                                        <div class="container">
                                                                            <div class="container">

                                                                                <div class="row">

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">DPI/PASAPORTE:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtDpiRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">PRIMER NOMBRE:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtPrimerNombreRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">SEGUNDO NOMBRE:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtSegundoNombreRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>


                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">APELLIDO 1:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtPrimerApellidoRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">APELLIDO 2:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtSegundoApellidoRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">APELLIDO DE CASADA:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtApellidoCasadaRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>



                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">CARRERA:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtCarreraRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">FACULTAD:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtFacultadRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">TELEFONO:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtTelRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>



                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">FECHA DE NACIMIENTO:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtFechaNacRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtEstadoRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtDireccionRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>



                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">PAIS:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtPaisRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">DEPARTAMENTO:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtDepartamentoRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">MUNICIPIO:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtMunicipioRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>



                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">CORREO PERSONAL:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtCorreoPersonalRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <br />
                                                                                    </div>

                                                                                    <div class="form-group col-md-4">
                                                                                        <asp:Label runat="server" Font-Bold="true">CORREO INSTITUCIONAL:</asp:Label>
                                                                                        <br />
                                                                                        <asp:TextBox ID="TxtCorreoInstitucionalRC" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                            <div class="container" id="divDPIRC" runat="server" visible="false">
                                                                <h4 style="text-align: center;" runat="server" visible="true" id="H3">Documentación Adjunta</h4>
                                                                <asp:Table ID="Table3" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 1--%>
                                                                        <asp:TableCell Width="25%">
                                            <br />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 2--%>
                                                                        <asp:TableCell Width="25%">
                                                                            <asp:Image ID="ImgDPI1RC" runat="server" Width="350px" Visible="false" />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 3--%>
                                                                        <asp:TableCell Width="25%">
                                        <br />  
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 1--%>
                                                                        <asp:TableCell Width="25%">
                                            <br />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 2--%>
                                                                        <asp:TableCell Width="25%">
                                                                            <asp:Image ID="ImgDPI2RC" runat="server" Width="350px" Visible="false" />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 3--%>
                                                                        <asp:TableCell Width="25%">
                                            <br />
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                </asp:Table>
                                                            </div>
                                                            <div class="container" id="divFotografiaRC" runat="server" visible="false">
                                                                <h4 style="text-align: center;" runat="server" visible="true" id="H4">Fotografía Adjunta</h4>
                                                                <asp:Table ID="Table4" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 1--%>
                                                                        <asp:TableCell Width="25%">
                                            <br />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 2--%>
                                                                        <asp:TableCell>
                                                                            <asp:Image ID="ImgFoto1RC" runat="server" Width="350px" />
                                                                        </asp:TableCell>

                                                                        <%-- ESPACIO 3--%>
                                                                        <asp:TableCell Width="25%">
                                            <br />
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                </asp:Table>
                                                            </div>

                                                            <div class="container" id="divBtnConfirmarRC" runat="server" visible="false">
                                                                <asp:Table ID="TbBtnsConfirmarRC" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 1--%>
                                                                        <asp:TableCell>
                                            <br />
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 2.1--%>
                                                                        <asp:TableCell>
                                                                            <asp:Button ID="BtnConfirmarRC" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaAceptarRC();" OnClick="BtnConfirmarRC_Click" />
                                                                        </asp:TableCell>
                                                                        <%-- ESPACIO 2.2--%>
                                                                        <asp:TableCell>
                                                                            <asp:Button ID="BtnRechazarRC" runat="server" Text="Rechazar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaRechazoRC();" OnClick="BtnRechazarRC_Click" />
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                    <asp:TableRow>
                                                                        <%-- ESPACIO 3--%>
                                                                        <asp:TableCell HorizontalAlign="Center">
                                                                        </asp:TableCell>
                                                                    </asp:TableRow>
                                                                </asp:Table>
                                                            </div>

                                                            <div class="modal" id="myModalActualizacionRC" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
                                                                <div class="modal-dialog" role="document">
                                                                    <div class="modal-content">
                                                                        <div class="row">
                                                                            <div class="col-md-4 mx-auto text-center">
                                                                            </div>
                                                                            <div class="col-md-4 mx-auto text-center">
                                                                                <div class="modal-spinnerCarne">
                                                                                    <div class="spinner"></div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="col-md-4 mx-auto text-center">
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-md-12 mx-auto text-center">
                                                                                <div class="modal-messageCarne">Por favor, espera mientras la información se está actualizando...</div>
                                                                                <div style="margin-bottom: 20px;"></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;">
                                                                <asp:Label ID="lblActualizacionRC" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                                                                </asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>


    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>


    <script>
        function mostrarAlertaRechazoAC() {
            var modal = document.getElementById("myModalActualizacionAC");
            if (confirm("¿Está seguro de que desea rechazar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnRechazarAC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaAceptarAC() {
            if (confirm("¿Está seguro de que desea confirmar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnConfirmarAC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function ocultarModalActualizacionAC() {
            var modal = document.getElementById("myModalActualizacionAC");
            modal.style.display = "none";
        }

        function mostrarAlertaRechazoPC() {
            var modal = document.getElementById("myModalActualizacionPC");
            if (confirm("¿Está seguro de que desea rechazar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnRechazarPC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaAceptarPC() {
            if (confirm("¿Está seguro de que desea confirmar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnConfirmarPC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function ocultarModalActualizacionPC() {
            var modal = document.getElementById("myModalActualizacionPC");
            modal.style.display = "none";
        }
        function mostrarAlertaRechazoRC() {
            var modal = document.getElementById("myModalActualizacionRC");
            if (confirm("¿Está seguro de que desea rechazar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnRechazarRC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaAceptarRC() {
            if (confirm("¿Está seguro de que desea confirmar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnConfirmarRC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function ocultarModalActualizacionRC() {
            var modal = document.getElementById("myModalActualizacionRC");
            modal.style.display = "none";
        }

    </script>

</asp:Content>
