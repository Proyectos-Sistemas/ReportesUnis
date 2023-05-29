<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfirmaciónDeCarne.aspx.cs" Inherits="ReportesUnis.ConfirmaciónDeCarne" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">CARNETIZACIÓN</h2>
    </div>
        <hr />
    <div id="CamposAuxiliares" runat="server" visible="false">
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:TextBox ID="TxtURL" runat="server" Visible="true"></asp:TextBox>
        <%-- TXTINICIO SE UTILIZA PARA VISUALIZAR FECHA --%>
        <asp:TextBox ID="TXTINICIO" runat="server" Visible="true"></asp:TextBox>        
        <%-- TXTPath ALMACENA EL PATH DONDE SE ALMACENARA LA IMAGEN --%>
        <asp:Label ID="txtPath" runat="server" Visible="false"></asp:Label>       
        <%-- TxtCantidad, almacena la cantidad de imagenes almacenadas --%>
        <asp:Label ID="txtCantidad" runat="server" Visible="false"></asp:Label>    
    </div>
    <div>
        <asp:Table id="tabla" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="25%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">CONFIRMAR CARNETS  </asp:Label> 
                        <asp:RadioButton id="RadioButtonConfirmar" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButtonConfirmar_CheckedChanged" GroupName="confirmar"/>
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>                        
                        <asp:Label runat="server" Font-Bold="true">GENERAR CARNETS  </asp:Label> 
                        <asp:RadioButton id="RadioButtonGenerar" runat="server"  AutoPostBack="True" OnCheckedChanged="RadioButtonGenerar_CheckedChanged" GroupName="confirmar"/>                        
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell Width="25%">
                       <%-- <br />--%>
                </asp:TableCell>
        </asp:TableRow>
        </asp:Table>     
    </div>
    <hr />
    <div class="container" id="divGenerar" runat="server" visible="false">
        <asp:Table id="tabla2" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="10%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">CARNE:</asp:Label> 
                </asp:TableCell>

                <%-- COMBOBOX 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="txtCarne" runat="server" MaxLength="13" Width="150px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell > 
                    <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" CssClass="btn-danger-unis" Enabled="true"  />
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell > 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell Width="10"> 
                </asp:TableCell>
        </asp:TableRow>                       
        </asp:Table>   
    </div>

    <div class="container" id="divConfirmar" runat="server" visible="false">
        <asp:Table id="tabla3" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">CARNE:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- COMBOBOX 4--%>
                <asp:TableCell>
                        <asp:DropDownList ID="CmbCarne" runat="server" AutoPostBack="true" OnTextChanged="CmbTipo_SelectedIndexChanged" EnableViewState="true" Width="150">
                        </asp:DropDownList>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%"> 
                </asp:TableCell>
        </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Table id="tabla4" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DPI/PASAPORTE:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtDpi" runat="server" Enabled="false"  Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">PRIMER NOMBRE:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtPrimerNombre" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">SEGUNDO NOMBRE:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtSegundoNombre" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">APELLIDO 1:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtPrimerApellido" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">APELLIDO 2:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtSegundoApellido" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">APELLIDO DE CASADA:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtApellidoCasada" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">CARRERA:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtCarrera" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">FACULTAD:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtFacultad" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">TELEFONO:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtTel" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">FECHA DE NACIMIENTO:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtFechaNac" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtEstado" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtDireccion" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DEPARTAMENTO:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtDepartamento" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">MUNICIPIO:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtMunicipio" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 13 .--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>           
        </asp:Table><br />

        <h4 style="text-align: center;" runat="server" visible="false" id="HDocumentacion">Documentación Adjunta</h4>
        <asp:Table id="tabla5" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Image ID="ImgDPI1" runat="server" Width="350px" /> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>
                
                 <%-- ESPACIO 4--%>
                <asp:TableCell Width="25%">
                        <asp:Image ID="ImgDPI2" runat="server" Width="350px" /> 
                </asp:TableCell>
                    
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

</asp:Content>