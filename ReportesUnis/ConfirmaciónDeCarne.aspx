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
        <%-- TxtCantidad, almacena la cantidad de imagenes almacenadas --%>
        <asp:Label ID="txtCantidad" runat="server" Visible="false">0</asp:Label>  
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
                        <asp:TextBox ID="txtCarne" runat="server" MaxLength="13" Width="150px" ></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell > 
                    <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnBuscar_Click" />
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell > 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell Width="10"> 
                </asp:TableCell>
        </asp:TableRow>                       
        </asp:Table>
        <br />
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
    </div>

    <div class="container" id="divCampos" runat="server" visible="false">     
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
                        <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 8 --%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtEstado" runat="server" Enabled="false" Width="200px"></asp:TextBox>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell >
                </asp:TableCell>

                <%-- ESPACIO  12--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtDireccion" runat="server" Enabled="false" Width="200px"></asp:TextBox>
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
                        <asp:Label runat="server" Font-Bold="true">PAIS:</asp:Label> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell >
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                        <asp:TextBox ID="TxtPais" runat="server" Enabled="false" Width="200px"></asp:TextBox>
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
        </asp:Table>
        <br />
    </div>

    <div class="container" id="divDPI" runat="server" visible="false">  
        <h4 style="text-align: center;" runat="server" visible="true" id="HDocumentacion">Documentación Adjunta</h4>
        <asp:Table id="tabla5" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>
                
                <%-- ESPACIO 2--%>
                <asp:TableCell Width="25%">
                        <asp:Image ID="ImgDPI1" runat="server" Width="350px" Visible="false"/> 
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
                        <asp:Image ID="ImgDPI2" runat="server" Width="350px" Visible="false" /> 
                </asp:TableCell>
                
                 <%-- ESPACIO 3--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div class="container" id="divFotografia" runat="server" visible="false">  
        <h4 style="text-align: center;" runat="server" visible="true" id="HFoto">Fotografía Adjunta</h4>
        <asp:Table id="tabla6" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
                <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>

                <%-- ESPACIO 2--%>
                <asp:TableCell>
                        <asp:Image ID="ImgFoto1" runat="server" Width="350px" /> 
                </asp:TableCell>
                
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="25%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <div class="container" id="divBtnConfirmar" runat="server" visible="false">
        <asp:Table id="TbBtnsConfirmar" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 2.1--%>
                <asp:TableCell>
                    <asp:Button ID="BtnConfirmar" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaAceptar();" OnClick="BtnConfirmar_Click" />
                </asp:TableCell>
                <%-- ESPACIO 2.2--%>
                <asp:TableCell>
                    <asp:Button ID="BtnRechazar" runat="server" Text="Rechazar" CssClass="btn-danger-unis" Enabled="true"  OnClientClick="return mostrarAlertaRechazo();" OnClick="BtnRechazar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 3--%>
                <asp:TableCell HorizontalAlign="Center">
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <div class="container" id="divBtnGenerar" runat="server" visible="false">
        <asp:Table id="TbBtnsGenerar" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 2.1--%>
                <asp:TableCell>
                    <asp:Button ID="BtnGenerar" runat="server" Text="Generar Renovación" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaGenerar();" OnClick="BtnGenerar_Click" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>                
                <%-- ESPACIO 3--%>
                <asp:TableCell HorizontalAlign="Center">
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;",>
            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
            </asp:Label>
    </div>

    <script>
        function mostrarAlertaRechazo() {
            if (confirm("¿Está seguro de que desea rechazar la información?")) {
                __doPostBack('<%= BtnRechazar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaAceptar() {
            if (confirm("¿Está seguro de que desea confirmar la información?")) {
                __doPostBack('<%= BtnConfirmar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }
        function mostrarAlertaGenerar() {
            if (confirm("¿Está seguro de que desea generar nuevamente la información?")) {
                __doPostBack('<%= BtnGenerar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

    </script>

</asp:Content>