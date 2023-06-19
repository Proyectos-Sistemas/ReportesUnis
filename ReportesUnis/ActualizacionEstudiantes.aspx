<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizacionEstudiantes.aspx.cs" Inherits="ReportesUnis.ActualizacionEstudiantes" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">ACTUALIZACIÓN DE INFORMACIÓN</h2>
    </div>
    <hr />

    <div class="container2" id="CargaFotografia" runat="server" visible="true">

        <div>
            <h5 style="text-align: center;">Toma de Fografía</h5>
            <br />
            <video id="videoElement" width="400" height="300" autoplay></video>
            <canvas id="canvas" width="400" height="300"></canvas>
        </div>
        <br />
        <div>
            <button id="captureBtn" name="captureBtn" class="btn-danger-unis">Capturar imagen</button>
            <textarea id="urlPath" name="urlPath" style="display: none"></textarea>
        </div>
    </div>
    <br />
    <hr />
    <%-- <div class="container2" id="CargaDPI" runat="server" visible="false">--%>
    <div class="container2" id="CargaDPI" runat="server" style="display: none">
        <div>
            <h5 style="text-align: center;">Carga de Documento de identificación</h5>
        </div>
        <asp:Label ID="Label3" runat="server" Font-Bold="false">Para realizar un cambio en su nombre es necesario adjuntar fotografia de su DPI(ambos lados)/Pasaporte</asp:Label>
        <br />
        <br />
        <asp:FileUpload ID="FileUpload2" runat="server" AllowMultiple="true" accept="image/jpeg" onchange="validateFileSize();" />
        <div id="dvMsge" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
            El tamaño máximo permitido es de 1 GB
        </div>
        <br />
        <hr />
    </div>
    <div id="CamposAuxiliares" runat="server" visible="true">
        <%-- TEXTBOX USEREMPLID ALMACENA EL EMPLID DEL USUARIO QUE ESTA HACIENDO LA ACTUALIZACION --%>
        <asp:Label ID="UserEmplid" runat="server" Visible="false"></asp:Label>
        <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO--%>
        <asp:Label ID="State" runat="server" Visible="false"></asp:Label>
        <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE TELEFONO O NO--%>
        <asp:Label ID="TruePhone" runat="server" Visible="false"></asp:Label>
        <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE NIT O NO--%>
        <asp:Label ID="TrueNit" runat="server" Visible="false"></asp:Label>
        <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE DIRECCION O NO--%>
        <asp:Label ID="TrueDir" runat="server" Visible="false"></asp:Label>
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:Label ID="TxtURL" runat="server" Visible="false"></asp:Label>
        <%-- TXTURLSQL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:Label ID="TxtURLSql" runat="server" Visible="false"></asp:Label>
        <%-- TXTUSER ALMACENA EL DPI DEL USUARIO QUE ESTA REALIZANDO CAMBIOS --%>
        <asp:Label ID="TextUser" runat="server" Visible="false"></asp:Label>
        <%-- TXTCONTADOR para contar las excepciones encontradas de apellidos --%>
        <asp:TextBox ID="txtContaador" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTPRIMERAPELLIDO  se almacena el primer apellido del estudiante --%>
        <asp:Label ID="txtPrimerApellido" runat="server" Visible="false"></asp:Label>
        <%-- TXTACCION se almacena el primer apellido del estudiante --%>
        <asp:Label ID="txtAccion" runat="server" Visible="false"></asp:Label>
        <%-- TXTTipoACCION se almacena el primer apellido del estudiante --%>
        <asp:Label ID="txtTipoAccion" runat="server" Visible="false"></asp:Label>
        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT ESPEJO --%>
        <asp:TextBox ID="txtInsert" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
        <asp:TextBox ID="txtInsertBI" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTEXISTE ALMACENA vALORES PARA HACER VALIDACIONES --%>
        <asp:Label ID="txtExiste" runat="server" Visible="false"></asp:Label>
        <%-- TXTEXISTE2 ALMACENA vALORES PARA HACER VALIDACIONES --%>
        <asp:Label ID="txtExiste2" runat="server" Visible="false"></asp:Label>
        <%-- TXTEXISTE3 ALMACENA vALORES PARA HACER VALIDACIONES --%>
        <asp:TextBox ID="txtExiste3" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTMUNICIPIODPI ALMACENA EL MUNICIPIO DEL DPI --%>
        <asp:Label ID="txtMunicipioDPI" runat="server" Visible="false"></asp:Label>
        <%-- TXTDEPARTAMENTODPI ALMACENA EL QUERY PARA HACER INSERT ESPEJO --%>
        <asp:Label ID="txtDepartamentoDPI" runat="server" Visible="false"></asp:Label>
        <%-- TXTPath ALMACENA EL PATH DONDE SE ALMACENARA LA IMAGEN --%>
        <asp:Label ID="txtPath" runat="server" Visible="false"></asp:Label>
        <%-- TXTPath URL APEX SERVICIO --%>
        <asp:Label ID="txtApex" runat="server" Visible="false"></asp:Label>
        <%-- NOMBRE INICIAL--%>
        <asp:Label ID="txtNInicial" runat="server" Visible="true" ForeColor="White"></asp:Label>
        <%-- APELLIDO INICIAL --%>
        <asp:Label ID="txtAInicial" runat="server" Visible="true" ForeColor="White"></asp:Label>
        <%-- APELLIDO PARA APEX --%>
        <asp:TextBox ID="txtApellidoAPEX" runat="server" Visible="false"></asp:TextBox>
        <%-- NOMBRE PARA APEX --%>
        <asp:TextBox ID="txtNombreAPEX" runat="server" Visible="false"></asp:TextBox>
        <%-- APELLIDO CASADA INICIAL --%>
        <asp:TextBox ID="txtCInicial" runat="server" Visible="false"></asp:TextBox>
        <%-- CONFIRMACION OPERADOR --%>
        <asp:Label ID="txtConfirmacion" runat="server" Visible="false"></asp:Label>
        <%-- ¡tiene pasaporte? --%>
        <asp:Label ID="txtPaisPasaporte" runat="server" Visible="false"></asp:Label>
        <%-- ¡tiene pasaporte? --%>
        <asp:Label ID="txtCantidadImagenesDpi" runat="server" Visible="false">0</asp:Label>
    </div>
    <div id="InfePersonal" runat="server">
        <h5 id="HPersonal" style="text-align: center;">Información Personal</h5>
        <br />
        <%-- TABLA EN LA QUE SE COLOCAN LOS OBJETOS --%>
        <asp:Table ID="tabla" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- CARNE LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Carné:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- CARNE  4--%>
                <asp:TableCell>
                    <asp:Label ID="txtCarne" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FECHA DE NACIMIENTO LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Fecha de Nacimiento:</asp:Label>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- FECHA DE NACIMIENTO  --%>
                <asp:TableCell>
                    <asp:Label ID="txtCumple" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- DPI LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">DPI/Pasaporte:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- DPI  12--%>
                <asp:TableCell>
                    <asp:Label ID="txtDPI" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>

                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- NOMBRE LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Nombres*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
                <%-- NOMBRE  4--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtNombre" runat="server" Enabled="true" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNombre" ErrorMessage="Ingrese su nombre." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
                <%-- APELLIDO LABEL 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Apellidos*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 7 --%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- APELLIDO   8--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtApellido" runat="server" Enabled="true" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtApellido" ErrorMessage="Ingrese su apellido." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
                <%-- APELLIDO CASADA LABEL --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Apellido de Casada:</asp:Label>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- APELLIDO CASADA  --%>
                <asp:TableCell>
                    <asp:TextBox ID="txtCasada" runat="server" Enabled="true" TextMode="MultiLine" Rows="2"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>

                <%-- CARRERA LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Carrera:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- CARRERA 4--%>
                <asp:TableCell>
                    <asp:Label ID="txtCarrera" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 6--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 7--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 8--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FACULTAD LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Facultad:</asp:Label> 
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FACULTAD  12--%>
                <asp:TableCell>
                    <asp:Label ID="txtFacultad" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>

                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
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
                        <asp:Label runat="server" Font-Bold="true">Dirección 1*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
                <%-- DIRECCION TEXTBOX 4--%>
                <asp:TableCell>
                    <asp:CustomValidator ID="validarDireccion" runat="server" ControlToValidate="txtDireccion" ErrorMessage="La dirección debe de tener al menos 10 caracteres" ClientValidationFunction="VerificarCantidadDireccion" ForeColor="Red" Font-Size="Small"></asp:CustomValidator>
                    <br />
                    <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" Rows="3" MaxLength="55" Width="365px"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDireccion" ErrorMessage="Ingrese su dirección." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- DIRECION2 LABEL 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Dirección 2:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- DIRECCION2 TEXTBOX 8--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtDireccion2" runat="server" TextMode="MultiLine" Rows="3" MaxLength="55" Width="220px"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
                <%-- DIRECION3 LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Dirección 3:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- DIRECCION3 TEXTBOX 12--%>
                <asp:TableCell>
                    <asp:TextBox ID="txtDireccion3" runat="server" TextMode="MultiLine" Rows="3" MaxLength="55" Width="220px"></asp:TextBox>
                </asp:TableCell>
                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- PAIS LABEL 2 --%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">País*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- PAIS DROPDOWNLIST 4--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbPais" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPais_SelectedIndexChanged" EnableViewState="true" Width="365px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- DEPARTAMENTO LABEL 6--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- DEPARTAMENTO DROPDOWNLIST 8--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbDepartamento" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbDepartamento_SelectedIndexChanged" Width="220px">
                    </asp:DropDownList>
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- MUNICIPIO LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- MUNICIPIO DROPDOWN 12--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbMunicipio" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipio_SelectedIndexChanged" Width="220px">
                    </asp:DropDownList>
                </asp:TableCell>

                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">
                <%-- ESPACIO 1--%>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
                <%-- TELEFONO LABEL 2--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Teléfono*:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 4--%>
                <asp:TableCell>
                    <asp:CustomValidator ID="validarTelefono" runat="server" ControlToValidate="txtTelefono" ErrorMessage="El número de teléfono debe de tener al menos 8 caracteres" Font-Size="Small" ClientValidationFunction="VerificarCantidadTelefono" ForeColor="Red"></asp:CustomValidator>
                    <br />
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="24" Width="365px"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtTelefono" ErrorMessage="Ingrese un número de teléfono." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>
                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 6--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 7--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESPACIO 8--%>
                <asp:TableCell Width="2%">
                    <br />
                </asp:TableCell>
                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESTADO CIVIL LABEL 10--%>
                <asp:TableCell>
                        <asp:Label runat="server" Font-Bold="true">Estado Civil:</asp:Label> 
                </asp:TableCell>
                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>
                <%-- ESTADO CIVIL DROPDOWN 12--%>
                <asp:TableCell>
                    <asp:DropDownList ID="CmbEstado" runat="server" Width="220px">
                        <asp:ListItem Selected="False" Value=""></asp:ListItem>
                        <asp:ListItem>Casado</asp:ListItem>
                        <asp:ListItem>Soltero</asp:ListItem>
                        <asp:ListItem>No Consta</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                        <br />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <div class="container2">
            <hr />
            <h5 style="text-align: center;">Información para Recibos de Pago</h5>
            <asp:Label runat="server">Utilizar el mismo nombre del registro:  </asp:Label>
            <br />
            <asp:RadioButton ID="RadioButtonNombreSi" runat="server" GroupName="confirmar" Text="SI" Checked="true" />
            <asp:RadioButton ID="RadioButtonNombreNo" runat="server" GroupName="confirmar" Text="NO" />
            <br />
        </div>
        <br />

        <asp:Table ID="TableNit" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center" CssClass="table-condensed table-border">
            <asp:TableRow HorizontalAlign="Center">
                <%-- 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- 2--%>
                <asp:TableCell>
                    <asp:Label runat="server" Font-Bold="true">Nombres*:</asp:Label> 
                    <br />
                    <br />
                </asp:TableCell>

                <%-- 3 --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- 4 --%>
                <asp:TableCell>
                    <asp:TextBox ID="TxtNombreR" runat="server" Enabled="false"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TxtNombreR" ErrorMessage="Ingrese su nombre." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>

                <%-- 5 --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- 6 --%>
                <asp:TableCell>
                    <asp:Label runat="server" Font-Bold="true">Apellidos*:</asp:Label> 
                    <br />
                    <br />
                </asp:TableCell>
                <%-- 7 --%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- 8 --%>
                <asp:TableCell>
                    <asp:TextBox ID="TxtApellidoR" runat="server" Enabled="false"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TxtApellidoR" ErrorMessage="Ingrese su apellido." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>

                <%-- 9 --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- 10 --%>
                <asp:TableCell>
                    <asp:Label runat="server" Font-Bold="true">Apellido de Casada:</asp:Label>
                    <br />
                    <br />
                </asp:TableCell>

                <%-- 11 --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- 12 --%>
                <asp:TableCell>
                    <asp:TextBox ID="TxtCasadaR" runat="server" Enabled="false"></asp:TextBox>
                    <br />
                    <br />
                </asp:TableCell>

                <%-- 13 --%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow HorizontalAlign="Center">

                <%-- ESPACIO 1--%>
                <asp:TableCell Width="2%">
                        <br />
                </asp:TableCell>

                <%-- CARNE LABEL 2--%>
                <asp:TableCell>
                        <br>
                </asp:TableCell>

                <%-- ESPACIO 3--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- CARNE  4--%>
                <asp:TableCell>
                    <br>
                </asp:TableCell>

                <%-- ESPACIO 5--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- FECHA DE NACIMIENTO LABEL --%>
                <asp:TableCell>
                    <asp:Label runat="server" Font-Bold="true">NIT*:</asp:Label> 
                    <br />
                    <br />
                </asp:TableCell>
                <%-- ESPACIO --%>
                <asp:TableCell Width="2%">
                       <%--<br />--%>
                </asp:TableCell>
                <%-- FECHA DE NACIMIENTO  --%>
                <asp:TableCell>
                    <asp:TextBox ID="txtNit" runat="server" Enabled="true"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtNit" ErrorMessage="Ingrese su NIT." ForeColor="Red"> </asp:RequiredFieldValidator>
                </asp:TableCell>

                <%-- ESPACIO 9--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- DPI LABEL 10--%>
                <asp:TableCell>
                    <br>
                </asp:TableCell>

                <%-- ESPACIO 11--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

                <%-- DPI  12--%>
                <asp:TableCell>
                    <br>
                </asp:TableCell>

                <%-- ESPACIO 13--%>
                <asp:TableCell Width="2%">
                        <%--<br />--%>
                </asp:TableCell>

            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Table ID="tbactualizar" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlerta();" OnClick="BtnActualizar_Click" />
                    <%--<asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnActualizar_Click" OnClientClick="return mostrarAlerta();" />--%>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div style="margin-left: auto; margin-right: auto; text-align: center;">
        <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
        </asp:Label>
        <br />
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        // Acceder a la cámara y mostrar el video en el elemento de video
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                var videoElement = document.getElementById('videoElement');
                videoElement.srcObject = stream;
            })
            .catch(function (error) {
                console.error('Error al acceder a la cámara: ', error);
            });

        // Capturar imagen cuando se haga clic en el botón
        var videoElement = document.getElementById('videoElement');
        var canvas = document.getElementById('canvas');
        var context = canvas.getContext('2d');
        var captureBtn = document.getElementById('captureBtn');
        const textarea = document.getElementById("urlPath");
        const textoFoto = document.getElementById("texto");
        captureBtn.addEventListener('click', function () {
            context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
            event.preventDefault();
            //Convertir la imagen del lienzo en base64
            var imageData = canvas.toDataURL('image/jpeg');
            textarea.value = imageData;
        });

        function CambiarEstadoBoton(habilitado) {
            var boton = document.getElementById('captureBtn');
            var videoElement = document.getElementById('videoElement');
            boton.disabled = !habilitado;
            videoElement.disabled = !habilitado;
        }
        function mostrarAlerta() {
            var mensaje = "";
            var apellido = document.getElementById('<%= txtApellido.ClientID %>').value;
            var nombre = document.getElementById('<%= txtNombre.ClientID %>').value;
            var nombreR = document.getElementById('<%= TxtNombreR.ClientID %>').value;
            var apellidoR = document.getElementById('<%= TxtApellidoR.ClientID %>').value;
            var NuevonombreR = document.getElementById('<%= TxtNombreR.ClientID %>');
            var NuevoapellidoR = document.getElementById('<%= TxtApellidoR.ClientID %>');
            var nit = document.getElementById('<%= txtNit.ClientID %>').value;
            var direccion1 = document.getElementById('<%= txtDireccion.ClientID %>').value;
            var telefono = document.getElementById('<%= txtTelefono.ClientID %>').value;
            var foto = document.getElementById('urlPath').value;
            var radioButtonS = document.getElementById('<%= RadioButtonNombreSi.ClientID %>');
            var radioButtonN = document.getElementById('<%= RadioButtonNombreNo.ClientID %>');
            /* var divInvisible = document.getElementById("CargaDPI");*/

            if (radioButtonS.checked) {
                NuevonombreR.value = nombre;
                NuevoapellidoR.value = apellido;
            }
            if (radioButtonN.checked) {
                NuevonombreR.value = "";
                NuevoapellidoR.value = "";
            }

            if (apellido.trim() === "") {
                mensaje = "-Los Apellidos son requerido";
            }

            if (nombre.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-Los Nombres son requeridos";
                } else {
                    mensaje = mensaje + "\n-Los Nombres son requeridos";
                }
            }

            if (nombreR.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-Los Nombres para el recibo son requeridos";
                } else {
                    mensaje = mensaje + "\n-Los Nombres para el recibo son requeridos";
                }
            }
            if (apellidoR.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-Los Apellidos para el recibo son requeridos";
                } else {
                    mensaje = mensaje + "\n-Los Apellidos para el recibo es requerido";
                }
            }

            if (nit.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-El NIT para el recibo son requeridos";
                } else {
                    mensaje = mensaje + "\n-El NIT para el recibo es requerido";
                }
            }

            if (direccion1.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-La Dirección 1 es requerido";
                } else {
                    mensaje = mensaje + "\n-La Dirección 1 es requerido";
                }
            }

            if (direccion1.length > 0 && direccion1.length < 11) {
                if (mensaje.trim() == "") {
                    mensaje = "-La Dirección 1 debe de tener como minimo 10 caracteres";
                } else {
                    mensaje = mensaje + "\n-La Dirección 1 debe de tener como minimo 10 caracteres";
                }
            }

            if (telefono.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-El Teléfono es requerido";
                } else {
                    mensaje = mensaje + "\n-El Teléfono es requerido";
                }
            }

            if (telefono.length > 0 && telefono.length <= 7) {
                if (mensaje.trim() == "") {
                    mensaje = "-El Teléfono debe de tener 8 carácteres";
                } else {
                    mensaje = mensaje + "\n-El Teléfono debe de tener 8 carácteres";
                }
            }

            if (foto.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-La fotografía es requerida";
                } else {
                    mensaje = mensaje + "\n-La fotografía es requerida";
                }
            }

            if (mensaje.trim() !== "") {
                mensaje = mensaje.replace("/\n/g", "<br>");
                alert(mensaje);
                return false;
            } else if (confirm("¿Está seguro de que su información es correcta?")) {
                // Aquí puedes agregar el código para realizar la acción deseada después de la confirmación.
                //alert("Acción confirmada");
                __doPostBack('<%= BtnActualizar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function DivDpi() {

            var DivDPI = document.getElementById('CargaDPI');
            if (DivDPI.style.display === "none") {
                return 1;
            } else {
                return 2;
            }
        }


        $(document).ready(function () {
            $('#<%= RadioButtonNombreSi.ClientID %>').on('change', function () {
                if ($(this).is(':checked')) {
                    $('#<%= TxtNombreR.ClientID %>').val($('#<%= txtNombre.ClientID %>').val());
                    $('#<%= TxtApellidoR.ClientID %>').val($('#<%= txtApellido.ClientID %>').val());
                    $('#<%= TxtCasadaR.ClientID %>').val($('#<%= txtCasada.ClientID %>').val());
                    $('#<%= TxtNombreR.ClientID %>').prop('disabled', true);
                    $('#<%= TxtApellidoR.ClientID %>').prop('disabled', true);
                    $('#<%= TxtCasadaR.ClientID %>').prop('disabled', true);
                }
            });
        });

        $(document).ready(function () {
            $('#<%= RadioButtonNombreNo.ClientID %>').on('change', function () {
                if ($(this).is(':checked')) {
                    $('#<%= TxtNombreR.ClientID %>').val("");
                    $('#<%= TxtApellidoR.ClientID %>').val("");
                    $('#<%= TxtCasadaR.ClientID %>').val("");
                    $('#<%= TxtNombreR.ClientID %>').prop('disabled', false);
                    $('#<%= TxtApellidoR.ClientID %>').prop('disabled', false);
                    $('#<%= TxtCasadaR.ClientID %>').prop('disabled', false);
                }
            });
        });        

        $(document).ready(function () {
            $('#<%= txtNombre.ClientID %> , #<%= txtApellido.ClientID %>').on('input', function () {
                var txtNombre = $('#<%= txtNombre.ClientID %>').val().trim();
                var txtNInicial = $('#<%= txtNInicial.ClientID %>').text().trim();
                var txtApellido = $('#<%= txtApellido.ClientID %>').val().trim();
                var txtAInicial = $('#<%= txtAInicial.ClientID %>').text().trim();
                
                if (txtNombre !== txtNInicial || txtApellido !== txtAInicial) {
                    $('#<%= CargaDPI.ClientID %>').css('display', 'block');
                 } else {
                     $('#<%= CargaDPI.ClientID %>').css('display', 'none');
                 }
            });
        });

        function VerificarCantidadTelefono(sender, args) {
            args.IsValid = (args.Value.length >= 7);
        }

        function VerificarCantidadDireccion(sender, args) {
            args.IsValid = (args.Value.length >= 9 && args.Value.length >= 1);
        }

        var textoAnterior = document.getElementById('<%= txtNombre.ClientID %>').value;

        function detectarCambioTexto() {
            var textBox = document.getElementById('<%= txtNombre.ClientID %>');
            var nuevoTexto = textBox.value;

            if (nuevoTexto != textoAnterior) {
                // El texto ha cambiado, realiza las acciones necesarias
                alert("Texto cambiado: " + nuevoTexto);

                // Actualiza el valor anterior con el nuevo valor
                textoAnterior = nuevoTexto;
            }
        }
    </script>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>

