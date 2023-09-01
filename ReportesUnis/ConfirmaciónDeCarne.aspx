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
        <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
        <input type="hidden" id="EFFDT_A_NIT" runat="server" />
        <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
        <input type="hidden" id="EFFDT_NameR" runat="server" />        
        <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
        <input type="hidden" id="UP_NAMES_NIT" runat="server" />
        <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
        <input type="hidden" id="UP_ADDRESSES_NIT" runat="server" />
        <%-- TEXTBOX ALMACENA UP NOMBRE PRF--%>
        <input type="hidden" id="UP_NAMES_PRF" runat="server" />  
        <%-- TEXTBOX ALMACENA UP NOMBRE PRI--%>
        <input type="hidden" id="UP_NAMES_PRI" runat="server" />       
        <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
        <input type="hidden" id="UD_NAMES_NIT" runat="server" />
        <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
        <input type="hidden" id="UD_ADDRESSES_NIT" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRF" runat="server" />
        <%-- TEXTBOX ALMACENA UD NOMBRE--%>
        <input type="hidden" id="UD_NAMES_PRI" runat="server" />
    </div>
   
    
    <div class="container" id="divConfirmar" runat="server">
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
                                <asp:DropDownList ID="CmbCarne" runat="server" AutoPostBack="true" OnTextChanged="CmbTipo_SelectedIndexChanged" EnableViewState="true" Width="150">
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
    
     <div class="container" id="divCampos" runat="server">
        <div class="row">
            <div class="col-md">
                <div class="container">
                    <div class="container">

                    <div class="row">

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">DPI/PASAPORTE:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtDpi" runat="server" Enabled="false"  Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">PRIMER NOMBRE:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtPrimerNombre" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">SEGUNDO NOMBRE:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtSegundoNombre" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>


                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">APELLIDO 1:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtPrimerApellido" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">APELLIDO 2:</asp:Label>
                            <br />
                            <asp:TextBox ID="TxtSegundoApellido" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">APELLIDO DE CASADA:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtApellidoCasada" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>



                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">CARRERA:</asp:Label>         
                            <br />
                            <asp:TextBox ID="TxtCarrera" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">FACULTAD:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtFacultad" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">TELEFONO:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtTel" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>



                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">FECHA DE NACIMIENTO:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtFechaNac" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">ESTADO CIVIL:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtEstado" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">DIRECCION:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtDireccion" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>



                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">PAIS:</asp:Label>
                            <br />
                            <asp:TextBox ID="TxtPais" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">DEPARTAMENTO:</asp:Label>   
                            <br />
                            <asp:TextBox ID="TxtDepartamento" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">MUNICIPIO:</asp:Label> 
                            <br />
                            <asp:TextBox ID="TxtMunicipio" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>



                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">CORREO PERSONAL:</asp:Label>
                            <br />
                            <asp:TextBox ID="TxtCorreoPersonal" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-4">
                            <br />
                        </div>

                        <div class="form-group col-md-4">
                            <asp:Label runat="server" Font-Bold="true">CORREO INSTITUCIONAL:</asp:Label>
                            <br />
                            <asp:TextBox ID="TxtCorreoInstitucional" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

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

    <div id="myModalActualizacion" class="modal">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-spinner">
                    <div class="spinner"></div>
                </div>
                <div class="modal-message">Por favor, espera mientras la información se está actualizando...</div>

            </div>
        </div>
    </div>

    <div visible="false" style="margin-left: auto; margin-right: auto; text-align: center;",>
            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
            </asp:Label>
    </div>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>


    <script>
        function mostrarAlertaRechazo() {
            var modal = document.getElementById("myModalActualizacion");
            if (confirm("¿Está seguro de que desea rechazar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnRechazar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaAceptar() {
            if (confirm("¿Está seguro de que desea confirmar la información?")) {
                modal.style.display = "block";
                __doPostBack('<%= BtnConfirmar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function ocultarModalActualizacion() {
            var modal = document.getElementById("myModalActualizacion");
            modal.style.display = "none";
        }

    </script>

</asp:Content>