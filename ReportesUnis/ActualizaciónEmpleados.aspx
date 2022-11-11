<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizaciónEmpleados.aspx.cs" Inherits="ReportesUnis.ActualizaciónEmpleados" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <br />
        <h2 style="text-align: center;">ACTUALIZACIÓN DE INFORMACIÓN DE EMPLEADOS</h2>
    </div>
    <hr />

    <div class="container">
        <asp:Label runat="server" Font-Bold="true">Fotografía:</asp:Label>
        <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="False" accept="image/jpeg" onchange="validateFileSize();" />
        <div id="dvMsg" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
            El tamaño máximo permitido es de 1 GB
        </div>
    </div>
    <hr />

    <%-- TEXTBOX USEREMPLID ALMACENA EL EMPLID DEL USUARIO QUE ESTA HACIENDO LA ACTUALIZACION --%>
    <asp:TextBox ID="UserEmplid" runat="server" Visible="false"></asp:TextBox>
    <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO--%>
    <asp:TextBox ID="State" runat="server" Visible="false"></asp:TextBox>
    
    <div>
        <br />
        <%-- TABLA EN LA QUE SE COLOCAN LOS OBJETOS --%>
        <asp:Table runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed ">

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO --%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- NOMBRE LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Nombres:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- NOMBRE  --%>
                <asp:TableCell>
                    <asp:Label ID="txtNombre" runat="server" Enabled="false" TextMode="MultiLine" Rows="2"></asp:Label>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- APELLIDO LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Apellidos:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- APELLIDO  --%>
                <asp:TableCell>
                    <asp:Label ID="txtApellido" runat="server" Enabled="false" TextMode="MultiLine" Rows="2"></asp:Label>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- DPI LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DPI/Pasaporte:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- DPI --%>
                <asp:TableCell>
                    <asp:Label ID="txtdPI" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- FECHA DE NACIMIENTO LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Fecha de Nacimiento:</asp:Label>
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- FECHA DE NACIMIENTO  4--%>
                <asp:TableCell>
                    <asp:Label ID="txtCumple" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 6--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 8--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- FACULTAD / DEPENDENCIA LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Facultad o Dependencia:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- FACULTAD / DEPENDENCIA  12--%>
                <asp:TableCell>
                    <asp:Label ID="txtFacultad" runat="server" Enabled="false" TextMode="MultiLine" Rows="3"></asp:Label>
                </asp:TableCell>
                <%-- ESPACIO 13--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- DIRECION LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Dirección*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- DIRECCION TEXTBOX 4--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" Rows="3" MaxLength="240" Width="350px"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- DIRECCION2  6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Dirección2:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- DIRECCION TEXTBOX2 8--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtDireccion2" runat="server" TextMode="MultiLine" Rows="3" MaxLength="240" Width="220px"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ZONA LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Zona:</asp:Label>                         
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ZONA TEXTBOX 12--%>
                <asp:TableCell>
                    <asp:DropDownList ID="txtZona" runat="server" AutoPostBack="true" EnableViewState="true" Width="220px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 13--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- PAIS2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">País*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- PAIS DRPDOWNLIST 4--%>
                <asp:TableCell>
                    <asp:DropDownList ID="cMBpAIS" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cMBpAIS_SelectedIndexChanged" EnableViewState="true" Width="350px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- DEPARTAMENTO 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- DEPARTAMENTO DRPDOWNLIST 8--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbDepartamento" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbDepartamento_SelectedIndexChanged" Width="220px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- MUNICIPIO LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- MUNICIPIO DROPDOWNLIST 12--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbMunicipio" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipio_SelectedIndexChanged" Width="220px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 13--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- ESTADO CIVIL LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Estado Civil:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ESTADO CIVIL DROPDOWNLIST 4--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbEstado" runat="server" Width="350px">
                        <asp:ListItem Selected="False" Value=""></asp:ListItem>
                        <asp:ListItem>Casado</asp:ListItem>
                        <asp:ListItem>Soltero</asp:ListItem>
                        <asp:ListItem>Sin Información</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 6--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 8--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- TELEFONO LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Teléfono*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="5%">
                        <br />
                </asp:TableCell>
                <%-- TELEFONO TEXTBOX 12--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="60" Width="220px"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO 13--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>

        </asp:Table>
        <asp:Table runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <%-- ESPACIO --%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" OnClick="BtnActualizar_Click" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="Center">
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <div style="margin-left: auto; margin-right: auto; text-align: center;">
            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
            </asp:Label>
        </div>
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>

        <%-- TXTUSER ALMACENA EL DPI DEL USUARIO QUE ESTA REALIZANDO CAMBIOS --%>
        <asp:TextBox ID="TextUser" runat="server" Visible="false"></asp:TextBox>
    </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
    <asp:CustomValidator ID="validarDireccion" runat="server" ControlToValidate="txtDireccion"
        ErrorMessage="La dirección debe de tener al menos 10 caracteres" ClientValidationFunction="VerificarCantidadTelefono" ForeColor="Red" Font-Size="Large" Font-Bold="true"></asp:CustomValidator>
    <script type="text/javascript">
        function VerificarCantidadDireccion(sender, args) {
            args.IsValid = (args.Value.length >= 9);
        }
    </script>

    <%-- VALIDACION CAMPOS NULOS --%>
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNull1" runat="server"
        ControlToValidate="txtDireccion"
        ErrorMessage="Ingresa una dirección."
        ForeColor="Red"
        Font-Size="Large" Font-Bold="true">
    </asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNull12" runat="server"
        ControlToValidate="txtTelefono"
        ErrorMessage=" Ingresa un número de teléfono."
        ForeColor="Red"
        Font-Size="Large" Font-Bold="true">
    </asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNull13" runat="server"
        ControlToValidate="cMBpAIS"
        ErrorMessage=" Seleccione un país."
        ForeColor="Red"
        Font-Size="Large" Font-Bold="true">
    </asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNull14" runat="server"
        ControlToValidate="CmbDepartamento"
        ErrorMessage=" Seleccione un departamento."
        ForeColor="Red"
        Font-Size="Large" Font-Bold="true">
    </asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNull15" runat="server"
        ControlToValidate="CmbMunicipio"
        ErrorMessage=" Seleccione un municipio."
        ForeColor="Red"
        Font-Size="Large" Font-Bold="true">
    </asp:RequiredFieldValidator>
    <%-- VALIDACION MINIMO DE CARACTERES --%>
    <asp:CustomValidator ID="validarTelefono" runat="server" ControlToValidate="txtTelefono"
        ErrorMessage="El número de teléfono debe de tener al menos 8 caracteres" ClientValidationFunction="VerificarCantidadTelefono" ForeColor="Red" Font-Size="Large" Font-Bold="true"></asp:CustomValidator>
    <script type="text/javascript">
        function VerificarCantidadTelefono(sender, args) {
            args.IsValid = (args.Value.length >= 7);
        }
    </script>
</asp:Content>
