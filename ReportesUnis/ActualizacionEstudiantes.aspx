<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizacionEstudiantes.aspx.cs" Inherits="ReportesUnis.ActualizacionEstudiantes" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div class="container">
        <div class="row">
            <div class="form-group col">
                <h2 style="text-align: center;">ACTUALIZACIÓN DE INFORMACIÓN</h2>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnCameraAvailable" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="cameraPermissionsGranted" runat="server" ClientIDMode="Static" />
    <div id="CargaFotografia" runat="server" style="display: none">
        <hr />
        <div class="container">
            <div class="row">
                <div class="form-group  col-md">
                    <h5 style="text-align: center;">Toma de Fografía</h5>
                </div>
            </div>
        </div>
        <br />


        <div class="container">
            <div class="row">
                <div>
                </div>

                <div class="form-group  col-md-5" style="align-content: center; justify-content: center; display: flex;">
                    <video id="videoElement" width="375" height="275" autoplay playsinline="true"></video>
                </div>


                <div class="form-group  col-md-2">
                </div>

                <div class="form-group  col-md-5" style="align-content: center; justify-content: center; display: flex;">
                    <asp:Image ID="ImgBase" runat="server" Visible="true" Style="max-width: 375px; max-height: 275px;" />
                </div>

                <div>
                </div>
            </div>
        </div>

        <textarea id="urlPath" name="urlPath" style="display: none"></textarea>
        <textarea id="urlPathControl" name="urlPathControl" style="display: none"></textarea>
        <canvas id="canvas" style="max-width: 375px; max-height: 275px; display: none"></canvas>
        <div class="container">
            <div class="row">
                <div class="col-md-4">
                </div>

                <div class="col-md-4 mx-auto text-center d-flex align-items-center justify-content-center">
                    <button id="captureBtn" name="captureBtn" class="btn-danger-unis">Capturar imagen</button>
                </div>

                <div class="col-md-4">
                </div>
            </div>
        </div>
        <br />
        <hr />

        <div class="container" id="CargaDPI" runat="server" style="display: none">
            <div>
                <h5 style="text-align: center; color: darkred;"><strong>Carga de Documento de identificación</strong></h5>
            </div>
            <asp:Label ID="Label3" runat="server" Font-Bold="false" ForeColor="Blue">Para realizar un cambio en su nombre es necesario adjuntar según sea el caso:</asp:Label>
            <br />
            <asp:Label ID="Label4" runat="server" Font-Bold="false" Font-Size="Small" ForeColor="Blue">a.) Fotografía de su DPI de ambos lados, es decir 2 fotografías</asp:Label>
            <br />
            <asp:Label ID="Label5" runat="server" Font-Bold="false" Font-Size="Small" ForeColor="Blue">b.) Fotografía de su Pasaporte</asp:Label>
            <br />
            <br />

            <br />
            <hr />
        </div>
        <div id="CamposAuxiliares" runat="server" visible="false">
            <%-- TEXTBOX USEREMPLID ALMACENA EL EMPLID DEL USUARIO QUE ESTA HACIENDO LA ACTUALIZACION --%>
            <asp:Label ID="UserEmplid" runat="server" Visible="false"></asp:Label>
            <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO--%>
            <asp:TextBox ID="State" runat="server" Visible="true"></asp:TextBox>
            <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO DEL NIT--%>
            <asp:TextBox ID="StateNIT" runat="server" Visible="true"></asp:TextBox>
            <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE TELEFONO O NO--%>
            <asp:Label ID="TruePhone" runat="server" Visible="false"></asp:Label>
            <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE EMAIL O NO--%>
            <asp:Label ID="TrueEmail" runat="server" Visible="false"></asp:Label>
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
            <%-- TXTEXISTE2 ALMACENA vALORES PARA HACER VALIDACIONES --%>
            <asp:Label ID="txtExiste4" runat="server" Visible="false"></asp:Label>
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
            <%-- APELLIDO PARA APEX --%>
            <asp:TextBox ID="txtApellidoAPEX" runat="server" Visible="false"></asp:TextBox>
            <%-- NOMBRE PARA APEX --%>
            <asp:TextBox ID="txtNombreAPEX" runat="server" Visible="false"></asp:TextBox>
            <%-- CONFIRMACION OPERADOR --%>
            <asp:Label ID="txtConfirmacion" runat="server" Visible="false"></asp:Label>
            <%-- ¡tiene pasaporte? --%>
            <asp:Label ID="txtPaisPasaporte" runat="server" Visible="false"></asp:Label>
            <%-- ¡tiene pasaporte? --%>
            <asp:Label ID="txtCantidadImagenesDpi" runat="server" Visible="false">0</asp:Label>
        </div>
        <div id="InfePersonal" runat="server" style="display: none">

            <div class="container">
                <div class="row">
                    <div class="col-md-4 mx-auto text-center">
                    </div>
                    <div class="col-md-4 mx-auto text-center">
                        <h5 style="text-align: center;">Información Personal</h5>
                    </div>
                    <div class="col-md-4 mx-auto text-center">
                    </div>
                </div>
            </div>

            <%-- NOMBRE INICIAL--%>
            <input type="hidden" id="txtNInicial" runat="server" />
            <%-- APELLIDO INICIAL --%>
            <input type="hidden" id="txtAInicial" runat="server" />
            <%-- APELLIDO CASADA INICIAL --%>
            <input type="hidden" id="txtCInicial" runat="server" />
            <%-- CONTROL DE VALIDACION DE NIT--%>
            <input type="hidden" id="ValidacionNit" runat="server" />
            <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE NIT O NO--%>
            <input type="hidden" id="TrueNit" runat="server" />
            <%-- TEXTBOX ALMACENA EL ESTADO CIVIL INICIAL--%>
            <input type="hidden" id="TrueEstadoCivil" runat="server" />
            <%-- TEXTBOX ALMACENA EL EFFDT DE ESTADO CIVIL--%>
            <input type="hidden" id="EFFDT_EC" runat="server" />
            <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION --%>
            <input type="hidden" id="EFFDT_A" runat="server" />
            <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
            <input type="hidden" id="EFFDT_A_NIT" runat="server" />
            <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
            <input type="hidden" id="EFFDT_NameR" runat="server" />
            <%-- TEXTBOX ALMACENA EL EFFDT DEL TELEFONO--%>
            <input type="hidden" id="EFFDT_P" runat="server" />
            <%-- TEXTBOX ALMACENA EL FT STUDENT DEL ESTADO CIVIL--%>
            <input type="hidden" id="FT_STUDENT" runat="server" />
            <%-- TEXTBOX ALMACENA EL NIVEL EDUCATIVO DEL ESTADO CIVIL--%>
            <input type="hidden" id="HIGH_LVL" runat="server" />
            <%-- TEXTBOX ALMACENA EL SEXO DEL ESTADO CIVIL--%>
            <input type="hidden" id="SEX_EC" runat="server" />

            <%-- TEXTBOX ALMACENA UP ESTADO CIVIL--%>
            <input type="hidden" id="UP_PERS_DATA_EFFDT" runat="server" />
            <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
            <input type="hidden" id="UP_NAMES_NIT" runat="server" />
            <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
            <input type="hidden" id="UP_ADDRESSES_NIT" runat="server" />
            <%-- TEXTBOX ALMACENA UP DIRECCION--%>
            <input type="hidden" id="UP_ADDRESSES" runat="server" />
            <%-- TEXTBOX ALMACENA UP TELEFONO--%>
            <input type="hidden" id="UP_PERSONAL_PHONE" runat="server" />
            <%-- TEXTBOX ALMACENA UP CORREO PERSONAL--%>
            <input type="hidden" id="UP_EMAIL_ADDRESSES" runat="server" />

            <%-- TEXTBOX ALMACENA UD ESTADO CIVIL--%>
            <input type="hidden" id="UD_PERS_DATA_EFFDT" runat="server" />
            <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
            <input type="hidden" id="UD_NAMES_NIT" runat="server" />
            <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
            <input type="hidden" id="UD_ADDRESSES_NIT" runat="server" />
            <%-- TEXTBOX ALMACENA UD DIRECCION--%>
            <input type="hidden" id="UD_ADDRESSES" runat="server" />
            <%-- TEXTBOX ALMACENA UD TELEFONO--%>
            <input type="hidden" id="UD_PERSONAL_PHONE" runat="server" />
            <%-- TEXTBOX ALMACENA UD CORREO PERSONAL--%>
            <input type="hidden" id="UD_EMAIL_ADDRESSES" runat="server" />
            <%-- TEXTBOX ALMACENA LA VARIABLE DE SESION--%>
            <input type="text" id="ISESSION" style="display: none" value="0" runat="server" />
            <input type="hidden" id="banderaSESSION" runat="server" />

            <%-- CREDENCIALES NIT--%>
            <input type="hidden" id="CREDENCIALES_NIT" runat="server" />
            <input type="hidden" id="URL_NIT" runat="server" />


            <%-- TABLA EN LA QUE SE COLOCAN LOS OBJETOS --%>
            <div class="container" id="tabla" runat="server" style="display: none">
                <div class="row">
                    <div class="col-md">
                        <div class="container">
                            <div class="row">

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Carné:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtCarne" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Fecha de nacimiento:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtCumple" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">DPI/Pasaporte:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtDPI" runat="server" Enabled="false"></asp:Label>
                                </div>


                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Correo Institucional:</asp:Label>
                                    <br />
                                    <asp:Label ID="EmailUnis" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Facultad:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtFacultad" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Carrera:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtCarrera" runat="server" Enabled="false"></asp:Label>
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Nombres*:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtNombre" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNombre" ErrorMessage="Ingrese su nombre." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Apellidos*:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtApellido" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtApellido" ErrorMessage="Ingrese su apellido." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Apellido de casada:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtCasada" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <br />
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Dirección 1*:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDireccion" ErrorMessage="Ingrese su dirección." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Dirección 2:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtDireccion2" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Dirección 3:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtDireccion3" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">País*:</asp:Label><br />
                                    <asp:DropDownList ID="CmbPais" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPais_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                    </asp:DropDownList>
                                    <br />
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                    <asp:DropDownList ID="CmbDepartamento" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbDepartamento_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                    </asp:DropDownList>
                                    <br />
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                    <asp:DropDownList ID="CmbMunicipio" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipio_SelectedIndexChanged" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                    </asp:DropDownList>
                                    <br />
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Teléfono*:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="24" CssClass="form-control" Width="275px" onblur="validarTelefono(this.value)"></asp:TextBox>
                                    <span id="errorTelefono" style="color: red; font-size: small"></span>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Correo personal*:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="TxtCorreoPersonal" runat="server" MaxLength="70" CssClass="form-control" Width="275px" onblur="validarCorreo(this.value)"></asp:TextBox>
                                    <span id="errorCorreo" style="color: red; font-size: small"></span>
                                    <br />
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Estado civil:</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="CmbEstado" runat="server" Width="275px" CssClass="form-control">
                                        <asp:ListItem Selected="False" Value=""></asp:ListItem>
                                        <asp:ListItem>Casado</asp:ListItem>
                                        <asp:ListItem>Soltero</asp:ListItem>
                                        <asp:ListItem>No Consta</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                </div>
                            </div>
                        </div>

                        <hr />
                        <div class="container">
                            <div class="row">
                                <div class="col-md-4 mx-auto text-center">
                                </div>
                                <div class="col-md-4 mx-auto text-center">
                                    <h5 style="text-align: center;">Información para recibos de pago</h5>
                                </div>
                                <div class="col-md-4 mx-auto text-center">
                                </div>
                            </div>
                        </div>
                        <div class="container">
                            <div class="row">
                                <div class="col-md-4 mx-auto text-center">
                                </div>
                                <div class="col-md-4 mx-auto text-center">
                                    <asp:Label runat="server">Desea utilizar CF:  </asp:Label>
                                    <br />
                                    <asp:RadioButton ID="RadioButtonNombreSi" runat="server" GroupName="confirmar" Text="SI" OnCheckedChanged="RadioButtonNombreSi_CheckedChanged" />
                                    <asp:RadioButton ID="RadioButtonNombreNo" runat="server" GroupName="confirmar" Text="NO" OnCheckedChanged="RadioButtonNombreNo_CheckedChanged" />
                                </div>
                                <div class="col-md-4 mx-auto text-center">
                                </div>
                            </div>
                        </div>
                        <div id="TableNit">
                            <div class="container">
                                <div class="row">

                                    <div class="col-md-4 mx-auto text-center">
                                    </div>

                                    <div class="col-md-4 mx-auto text-center">
                                        <asp:Label runat="server" Font-Bold="true">NIT*:</asp:Label>
                                    </div>

                                    <div class="col-md-4 mx-auto text-center">
                                    </div>


                                    <div class="col-md-4 mx-auto text-center">
                                    </div>

                                    <div class="col-md-4 mx-auto text-center d-flex align-items-center justify-content-center">
                                        <asp:TextBox ID="txtNit" runat="server" Width="275px" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-md-4 mx-auto text-center">
                                    </div>


                                    <div class="col-md-4 mx-auto text-center">
                                    </div>

                                    <div class="col-md-4 mx-auto text-center">
                                        <asp:Label runat="server" Font-Size="Small" Text="El NIT no debe de contener guión (-)"></asp:Label>
                                    </div>

                                    <div class="col-md-4 mx-auto text-center">
                                    </div>
                                </div>
                            </div>

                            <div class="container">
                                <div class="row">
                                    <div class="col-md-4 mx-auto text-center">
                                    </div>
                                    <div class="col-md-4 mx-auto text-center">
                                        <asp:Button ID="ValidarNIT" runat="server" Text="Validar Nit" CssClass="btn-danger-unis" Enabled="true" OnClick="txtNit_TextChanged" CausesValidation="false" />
                                    </div>
                                    <div class="col-md-4 mx-auto text-center">
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="container">
                                <div class="row">
                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Nombre 1*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="TxtNombreR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Nombre 2:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="TxtApellidoR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Nombre 3:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="TxtCasadaR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Dirección 1*:</asp:Label>
                                        <asp:TextBox ID="TxtDiRe1" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Dirección 2:</asp:Label>
                                        <asp:TextBox ID="TxtDiRe2" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Dirección 3:</asp:Label>
                                        <asp:TextBox ID="TxtDiRe3" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                        <br />
                                    </div>

                                    <div class="container" id="Combos" runat="server">
                                        <div class="row">
                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">País*:</asp:Label>
                                                <br />
                                                <asp:DropDownList ID="CmbPaisNIT" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPaisNIT_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                <br />
                                            </div>

                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                                <br />
                                                <asp:DropDownList ID="CmbDepartamentoNIT" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbDepartamentoNIT_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                <br />
                                            </div>

                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                                <br />
                                                <asp:DropDownList ID="CmbMunicipioNIT" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipioNIT_SelectedIndexChanged" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                <br />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="container" id="sustituirCombos" runat="server">
                                        <div class="row">
                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">País*:</asp:Label>
                                                <br />
                                                <asp:TextBox ID="PaisNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                <br />
                                            </div>

                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                                <br />
                                                <asp:TextBox ID="DepartamentoNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                <br />
                                            </div>

                                            <div class="form-group col-md-4">
                                                <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                                <br />
                                                <asp:TextBox ID="MunicipioNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                <br />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <br />


        <div class="container" id="tbactualizar" runat="server" style="display: none">
            <div class="row">
                <div class="col-md-4 mx-auto text-center">
                </div>
                <div class="col-md-4 mx-auto text-center">
                    <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlerta();" OnClick="BtnActualizar_Click" />
                </div>
                <div class="col-md-4 mx-auto text-center">
                </div>
            </div>
        </div>
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
    <div id="myModalCorrecto" class="modal">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-message">Su información fue actualizada correctamente.</div>

            </div>
        </div>
    </div>
    <div id="myModalEspera" class="modal">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-spinner">
                    <div class="spinner"></div>
                </div>
                <div class="modal-message">Por favor, espera un momento</div>

            </div>
        </div>
    </div>




    <div class="container" runat="server">
        <div class="row">
            <div class="col-md-2">
            </div>
            <div class="col-md-8 d-flex justify-content-center align-items-center">
                <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"></asp:Label>
            </div>
            <div class="col-md-2">
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-4 mx-auto text-center">
            </div>
            <div class="col-md-4 mx-auto text-center d-flex justify-content-center align-items-center">
                <asp:Button ID="BtnDownload" runat="server" Text="Descargar Manual" CssClass="btn-danger-unis" OnClick="BtnDownload_Click" Style="display: none" />
            </div>
            <div class="col-md-4 mx-auto text-center">
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-4 mx-auto text-center">
            </div>
            <div class="col-md-4 mx-auto text-center d-flex justify-content-center align-items-center">
                <asp:Button ID="BtnReload" runat="server" Text="Recargar Página" CssClass="btn-danger-unis" OnClick="BtnReload_Click" Style="display: none" />
            </div>
            <div class="col-md-4 mx-auto text-center">
            </div>
        </div>
    </div>

    <div id="myModal" class="modal">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 style="text-align: center; color: darkred; text-align: center"><strong>Carga de Documento de Identificación</strong></h4>
                    <span class="close">&times;</span>
                </div>
                <div class="modal-body">
                    <contenttemplate>
                        <div class="container emp-profile">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="profile-head">
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="Black">Para realizar un cambio en su nombre es necesario adjuntar según sea el caso:</asp:Label>
                                                <br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Label ID="Label2" runat="server" Font-Bold="false" Font-Size="Small" ForeColor="Black">&nbsp;&nbsp;&nbsp;&nbsp;a.) Fotografia de su DPI de ambos lados, es decir 2 fotografías</asp:Label>
                                                <br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Label ID="Label6" runat="server" Font-Bold="false" Font-Size="Small" ForeColor="Black">&nbsp;&nbsp;&nbsp;&nbsp;b.) Fotografia de su Pasaporte</asp:Label>
                                                <br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:FileUpload ID="FileUpload2" runat="server" AllowMultiple="true" accept="image/jpeg" onchange="validarCargaArchivos();" />
                                                <div id="dvMsge1" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
                                                    El tamaño máximo permitido es de 1 GB
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="form-group col-md-5">
                                            </div>
                                            <div class="form-group col-md-2">
                                                <asp:Button ID="BtnAceptarCarga" runat="server" Text="Aceptar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnAceptarCarga_Click" />
                                            </div>
                                            <div class="form-group col-md-5">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </contenttemplate>
                </div>
            </div>
        </div>
    </div>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.datatables.net/v/dt/dt-1.13.6/datatables.min.js"></script>

    <script>
        var userAgent = navigator.userAgent;

        if (userAgent.indexOf("Safari") != -1 && userAgent.indexOf("Chrome") == 0) {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (stream) {
                    var videoElement = document.getElementById('videoElement');
                    videoElement.srcObject = stream;
                })
                .catch(function (error) {
                    error;
                });
        } else if (userAgent.indexOf("Chrome") != -1) {
            // Acceder a la cámara y mostrar el video en el elemento de video
            navigator.getMedia = (navigator.getUserMedia ||
                navigator.webkitGetUserMedia ||
                navigator.mozGetUserMedia ||
                navigator.msGetUserMedia);

            navigator.getMedia({ video: true }, function (stream) {
                var videoElement = document.getElementById('videoElement');
                videoElement.srcObject = stream;
                videoElement.onplay;
                $("#<%= CargaFotografia.ClientID %>").css("display", "block");
                $('#<%= tabla.ClientID %>').css("display", "block");
                $('#<%= tbactualizar.ClientID %>').css("display", "block");
                $('#<%= InfePersonal.ClientID %>').css("display", "block");
                $('#<%= BtnReload.ClientID %>').css("display", "none");
                $('#<%= BtnDownload.ClientID %>').css("display", "none");
                var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                lblActualizacion.html("");
                $('#<%= BtnReload.ClientID %>').click;
            }, function (error) {
                let mensajeError = error.message;
                if (mensajeError == "Permission denied") {
                    $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                    $('#<%= tabla.ClientID %>').css("display", "none");
                    $('#<%= tbactualizar.ClientID %>').css("display", "none");
                    $('#<%= InfePersonal.ClientID %>').css("display", "none");
                    var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                    mensaje = "La cámara no tiene permisos disponibles o su dispositivo no cuenta con una cámara. <br />  <br>Para asignar los permisos correspondientes, descargue el manual y siga las instrucciones, al finalizar, haga clic en el botón de Recargar Página. <br>";
                    lblActualizacion.css("color", "black");
                    lblActualizacion.html(mensaje);
                    $('#<%= BtnReload.ClientID %>').css("display", "block");
                    $('#<%= BtnDownload.ClientID %>').css("display", "block");
                } else if (mensajeError == "Could not start video source") {
                    $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                    $('#<%= tabla.ClientID %>').css("display", "none");
                    $('#<%= tbactualizar.ClientID %>').css("display", "none");
                    $('#<%= InfePersonal.ClientID %>').css("display", "none");
                    var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                    mensaje = "La cámara está siendo utilizada por otras aplicaciones. <br />  <br>Para poder continuar cierre dichas aplicaciones y luego haga clic en el botón de Recargar Página. <br>";
                    lblActualizacion.css("color", "black");
                    lblActualizacion.html(mensaje);
                    $('#<%= BtnReload.ClientID %>').css("display", "block");
                    $('#<%= BtnDownload.ClientID %>').css("display", "none");
                }
            });
        } else if (userAgent.indexOf("Firefox") != -1) {
            // Acceder a la cámara y mostrar el video en el elemento de video
            navigator.getMedia = (navigator.getUserMedia ||
                navigator.webkitGetUserMedia ||
                navigator.mozGetUserMedia ||
                navigator.msGetUserMedia);

            navigator.getMedia({ video: true }, function (stream) {
                var videoElement = document.getElementById('videoElement');
                videoElement.srcObject = stream;
                videoElement.onplay;
                $("#<%= CargaFotografia.ClientID %>").css("display", "block");
                $('#<%= tabla.ClientID %>').css("display", "block");
                $('#<%= tbactualizar.ClientID %>').css("display", "block");
                $('#<%= InfePersonal.ClientID %>').css("display", "block");
                $('#<%= BtnReload.ClientID %>').css("display", "none");
                $('#<%= BtnDownload.ClientID %>').css("display", "none");
                var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                lblActualizacion.html("");
                $('#<%= BtnReload.ClientID %>').click;
            }, function (error) {
                let mensajeError = error.message;
                if (mensajeError.indexOf("Permission denied" != -1)) {
                    $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                    $('#<%= tabla.ClientID %>').css("display", "none");
                    $('#<%= tbactualizar.ClientID %>').css("display", "none");
                    $('#<%= InfePersonal.ClientID %>').css("display", "none");
                    var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                    mensaje = "La cámara no tiene permisos disponibles o su dispositivo no cuenta con una cámara. <br />  <br>Para asignar los permisos correspondientes, descargue el manual y siga las instrucciones, al finalizar, haga clic en el botón de Recargar Página. <br>";
                    lblActualizacion.css("color", "black");
                    lblActualizacion.html(mensaje);
                    $('#<%= BtnReload.ClientID %>').css("display", "block");
                    $('#<%= BtnDownload.ClientID %>').css("display", "block");
                }

                if (mensajeError.indexOf("Could not start video source" != -1)) {
                    $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                    $('#<%= tabla.ClientID %>').css("display", "none");
                    $('#<%= tbactualizar.ClientID %>').css("display", "none");
                    $('#<%= InfePersonal.ClientID %>').css("display", "none");
                    var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                    mensaje = "La cámara está siendo utilizada por otras aplicaciones. <br />  <br>Para poder continuar cierre dichas aplicaciones y luego haga clic en el botón de Recargar Página. <br>";
                    lblActualizacion.css("color", "black");
                    lblActualizacion.html(mensaje);
                    $('#<%= BtnReload.ClientID %>').css("display", "block");
                    $('#<%= BtnDownload.ClientID %>').css("display", "none");
                }
            });
        } else {
            var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
            mensaje = "El navegador no es compatible";
            lblActualizacion.css("color", "black");
            lblActualizacion.html(mensaje);
        }

        $(document).ready(function () {
            var videoElement = $('#videoElement')[0];
            var canvas = $('#canvas')[0];
            var context = canvas.getContext('2d');
            var captureBtn = $('#captureBtn');
            var textarea = $("#urlPath");
            var imgBase = $("#<%= ImgBase.ClientID %>");
            var urlPathControl = $("#urlPathControl");
            captureBtn.on('click', function (event) {
                event.preventDefault();
                context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
                var imageData = canvas.toDataURL('image/jpeg');
                textarea.val(imageData);
                urlPathControl.val('1');
                imgBase.attr('src', imageData);
            });
        });

        if (userAgent.indexOf("Chrome") != -1) {
            async function verificarPermisosCamara() {
                try {
                    const status = await navigator.permissions.query({ name: 'camera' });

                    status.onchange = function () {
                        location.reload();
                    };
                } catch (error) {

                }
            }
            // Ejecutar la función de verificación en tiempo real
            setInterval(verificarPermisosCamara, 10);
        } else {
            window.addEventListener('load', function () {
                ValidarEstadoCamara1();
            });

            function ValidarEstadoCamara1() {
                const date = new Date();
                var mensaje = "";
                var sesion = $('#<%= ISESSION.ClientID %>').val().trim();
                var bandera = $('#<%= banderaSESSION.ClientID %>').val().trim();
                navigator.mediaDevices.getUserMedia({ video: true })
                    .then(function () {
                        if ((sesion == "0" || sesion == "1") && bandera == 0) {
                            var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                            $("#<%= CargaFotografia.ClientID %>").css("display", "block");
                            $('#<%= tabla.ClientID %>').css("display", "block");
                            $('#<%= tbactualizar.ClientID %>').css("display", "block");
                            $('#<%= InfePersonal.ClientID %>').css("display", "block");
                            $('#<%= BtnReload.ClientID %>').css("display", "none");
                            $('#<%= BtnDownload.ClientID %>').css("display", "none");
                            $('#<%= BtnReload.ClientID %>').click;
                            guardarEnSessionStorage("1");
                            bandera.text = "1";
                            lblActualizacion.html("");
                        }
                    })
                    .catch(function (error) {
                        if ((sesion == "0" || sesion == "2") && bandera == 0) {
                            let mensajeError = error.message;
                            if (mensajeError.indexOf("Permission denied" != -1)) {
                                $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                                $('#<%= tabla.ClientID %>').css("display", "none");
                                $('#<%= tbactualizar.ClientID %>').css("display", "none");
                                $('#<%= InfePersonal.ClientID %>').css("display", "none");
                                var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                                mensaje = "La cámara no tiene permisos disponibles o su dispositivo no cuenta con una cámara. <br />  <br>Para asignar los permisos correspondientes, descargue el manual y siga las instrucciones, al finalizar, haga clic en el botón de Recargar Página. <br>";
                                lblActualizacion.css("color", "black");
                                lblActualizacion.html(mensaje);
                                $('#<%= BtnReload.ClientID %>').css("display", "block");
                                $('#<%= BtnDownload.ClientID %>').css("display", "block");
                            }

                            if (mensajeError.indexOf("Could not start video source" != -1)) {
                                $('#<%= CargaFotografia.ClientID %>').css("display", "none");
                                $('#<%= tabla.ClientID %>').css("display", "none");
                                $('#<%= tbactualizar.ClientID %>').css("display", "none");
                                $('#<%= InfePersonal.ClientID %>').css("display", "none");
                                var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
                                mensaje = "La cámara está siendo utilizada por otras aplicaciones. <br />  <br>Para poder continuar cierre dichas aplicaciones y luego haga clic en el botón de Recargar Página. <br>";
                                lblActualizacion.css("color", "black");
                                lblActualizacion.html(mensaje);
                                $('#<%= BtnReload.ClientID %>').css("display", "block");
                                $('#<%= BtnDownload.ClientID %>').css("display", "none");
                            }
                        }
                    });

                setTimeout(function () {
                    ValidarEstadoCamara1()
                }, 1);

            };

        }

        // Función para guardar en sessionStorage
        function guardarEnSessionStorage(valor) {
            var inputElement = $('#<%= ISESSION.ClientID %>').val().trim();
            // Verificar si sessionStorage está disponible en el navegador
            if (typeof sessionStorage !== 'undefined') {
                // Guardar el valor en sessionStorage
                sessionStorage.setItem("miVariable", valor);
                inputElement.text = valor;
            }
        }

        $(document).ready(function () {
            var videoElement = $('#videoElement')[0];
            var canvas = $('#canvas')[0];
            var context = canvas.getContext('2d');
            var captureBtn = $('#captureBtn');
            var textarea = $("#urlPath");
            var imgBase = $("#<%= ImgBase.ClientID %>");
            var urlPathControl = $("#urlPathControl");
            captureBtn.on('click', function (event) {
                event.preventDefault();
                context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
                var imageData = canvas.toDataURL('image/jpeg');
                textarea.val(imageData);
                urlPathControl.val('1');
                imgBase.attr('src', imageData);
            });
        });

        function validarCorreo(correo) {
            //var regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
            var errorCorreoElement = document.getElementById("errorCorreo");
            var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (correo.trim() === "") {
                errorCorreoElement.textContent = "Ingrese su correo personal.";
            } else if (!regex.test(correo)) {
                errorCorreoElement.textContent = "El formato del correo electrónico no es válido.";
            } else {
                errorCorreoElement.textContent = " ";
            }
        }

        function validarTelefono(Telefono) {
            //var regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
            var errorTelefonoElement = document.getElementById("errorTelefono");

            if (Telefono.trim() === "") {
                errorTelefonoElement.textContent = "Ingrese su teléfono.";
            } else if (Telefono.length > 0 && Telefono.length <= 7) {
                errorTelefonoElement.textContent = "El número de télefono debe de tener 8 caracteres";
            }
        }

        function mostrarAlerta() {
            var mensaje = "";
            var apellido = document.getElementById('<%= txtApellido.ClientID %>').value;
            var nombre = document.getElementById('<%= txtNombre.ClientID %>').value;
            var nombreR = document.getElementById('<%= TxtNombreR.ClientID %>').value;
            var nit = document.getElementById('<%= txtNit.ClientID %>').value;
            var direccion1 = document.getElementById('<%= txtDireccion.ClientID %>').value;
            var direccionR1 = document.getElementById('<%= TxtDiRe1.ClientID %>').value;
            var telefono = document.getElementById('<%= txtTelefono.ClientID %>').value;
            var pais = document.getElementById('<%= CmbPais.ClientID %>').value;
            var depto = document.getElementById('<%= CmbDepartamento.ClientID %>').value;
            var muni = document.getElementById('<%= CmbMunicipio.ClientID %>').value;
            var Correo = document.getElementById('<%= TxtCorreoPersonal.ClientID %>').value;
            var foto = document.getElementById('urlPath').value;
            var foto2 = document.getElementById('urlPathControl').value;
            var ValidacionNit = $('#<%= ValidacionNit.ClientID %>').val().trim();
            var TrueNit = $('#<%= TrueNit.ClientID %>').val().trim();
            var txtNombre = $('#<%= txtNombre.ClientID %>').val().trim();
            var txtNInicial = $('#<%= txtNInicial.ClientID %>').val().trim();
            var txtApellido = $('#<%= txtApellido.ClientID %>').val().trim();
            var txtAInicial = $('#<%= txtAInicial.ClientID %>').val().trim();
            var txtCasada = $('#<%= txtCasada.ClientID %>').val().trim();
            var txtCInicial = $('#<%= txtCInicial.ClientID %>').val().trim();
            var modal = document.getElementById("myModalActualizacion");

            if (txtNombre !== txtNInicial || txtApellido !== txtAInicial || txtCasada !== txtCInicial) {
                $('#myModal').css('display', 'block');
                return false;
            } else if (TrueNit !== nit) {
                // Realiza las acciones necesarias si el valor es diferente de cero
                if (nit !== "CF") {
                    alert("El NIT ha cambiado, es necesario validar.");
                    return false;
                }
            } else {

                if (apellido.trim() === "") {
                    mensaje = "-Los Apellidos son requerido.";
                }

                if (nombre.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-Los Nombres son requeridos.";
                    } else {
                        mensaje = mensaje + "\n-Los Nombres son requeridos.";
                    }
                }

                if (direccion1.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-La Dirección 1 es requerida.";
                    } else {
                        mensaje = mensaje + "\n-La Dirección 1 es requerida.";
                    }
                }

                if (pais.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El país es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El país es requerido.";
                    }
                }
                if (depto.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El departamento es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El departamento es requerido.";
                    }
                }
                if (muni.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El municipio es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El municipio es requerido.";
                    }
                }

                if (Correo.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El Correo Personal es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El Correo Personal es requerido.";
                    }
                }

                if (telefono.length > 0 && telefono.length <= 7) {
                    if (mensaje.trim() == "") {
                        mensaje = "-El Teléfono debe de tener 8 carácteres";
                    } else {
                        mensaje = mensaje + "\n-El Teléfono debe de tener 8 carácteres";
                    }
                }

                if (foto.trim() === "" && foto2 === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-La fotografía es requerida";
                    } else {
                        mensaje = mensaje + "\n-La fotografía es requerida";
                    }
                }

                if (nit.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El NIT para el recibo es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El NIT para el recibo es requerido.";
                    }
                }

                if (direccionR1.trim() === "" && nombreR.trim() !== "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-La Dirección 1 para el recibo es requerida.";
                    } else {
                        mensaje = mensaje + "\n-La Dirección 1 para el recibo es requerida.";
                    }
                }

                if (mensaje.trim() !== "") {
                    mensaje = mensaje.replace("/\n/g", "<br>");
                    alert(mensaje);
                    return false;
                } else if (confirm("¿Está seguro de que su información es correcta?")) {
                    $('#myModalActualizacion').css('display', 'block');
                    __doPostBack('<%= BtnActualizar.ClientID %>', '');
                    return true; // Permite continuar con la acción del botón
                } else {
                    return false; // Cancela la acción del botón
                }
            }
        }

        function Documentos() {
            alert("Es necesario adjuntar la imagen de su documento de actualización para continuar con la actualización.");
            $('#myModal').css('display', 'block');
            return false;
        }

        function NoExisteNit() {
            alert("El NIT no existe. Intente de nuevo");
        }

        function ConfirmacionActualizacionSensible() {
            mensaje = "Su información fue almacenada correctamente. \nLa información ingresada debe ser aprobada antes de ser confirmada.\nActualmente, solo se muestran los datos que han sido previamente confirmados.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionEstudiantes.aspx";
        }

        function ConfirmacionActualizacion() {
            mensaje = "Su información fue actualizada correctamente.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionEstudiantes.aspx";
        }

        $(document).ready(function () {
            // Function to add the code
            function RBSi() {
                $('#<%= RadioButtonNombreSi.ClientID %>').on('change', function () {
                    if ($(this).is(':checked')) {
                        $('#<%= TxtNombreR.ClientID %>').val($('#<%= txtNombre.ClientID %>').val());
                        $('#<%= TxtApellidoR.ClientID %>').val($('#<%= txtApellido.ClientID %>').val());
                        $('#<%= TxtCasadaR.ClientID %>').val($('#<%= txtCasada.ClientID %>').val());
                        $('#<%= TxtDiRe1.ClientID %>').val($('#<%= txtDireccion.ClientID %>').val());
                        $('#<%= TxtDiRe2.ClientID %>').val($('#<%= txtDireccion2.ClientID %>').val());
                        $('#<%= TxtDiRe3.ClientID %>').val($('#<%= txtDireccion3.ClientID %>').val());
                        $('#<%= CmbPaisNIT.ClientID %>').val($('#<%= CmbPais.ClientID %>').val());
                        $('#<%= CmbMunicipioNIT.ClientID %>').val($('#<%= CmbMunicipio.ClientID %>').val());
                        $('#<%= CmbDepartamentoNIT.ClientID %>').val($('#<%= CmbDepartamento.ClientID %>').val());
                        $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPais.ClientID %>').val());
                        $('#<%= MunicipioNit.ClientID %>').val($('#<%= CmbMunicipio.ClientID %>').val());
                        $('#<%= DepartamentoNit.ClientID %>').val($('#<%= CmbDepartamento.ClientID %>').val());
                        $('#<%= StateNIT.ClientID %>').val($('#<%= State.ClientID %>').val());
                        $('#<%= txtNit.ClientID %>').val('CF');
                        $('#<%= txtNit.ClientID %>').prop('disabled', true);
                        $('#<%= ValidarNIT.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe1.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe2.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe3.ClientID %>').prop('disabled', true);
                        $('#<%= PaisNit.ClientID %>').prop('disabled', true);
                        $('#<%= MunicipioNit.ClientID %>').prop('disabled', true);
                        $('#<%= DepartamentoNit.ClientID %>').prop('disabled', true);
                        $('#<%= lblActualizacion.ClientID %>').text('');

                        // Hacer visible la fila Combos
                        $('#<%= Combos.ClientID %>').hide();
                        // Hacer visible la fila sustitucion de Combos
                        $('#<%= sustituirCombos.ClientID %>').hide();
                    }
                });
            }

            // Call the function
            RBSi();
        });

        $(document).ready(function () {
            // Capturar el cambio de estado del RadioButton
            $('#<%= RadioButtonNombreSi.ClientID %>').change(function () {
                if ($(this).is(':checked')) {
                    // El RadioButton ha sido marcado, ocultar la fila
                    $('#<%= Combos.ClientID %>').hide();
                    $('#<%= sustituirCombos.ClientID %>').show();
                } else {
                    // El RadioButton ha sido desmarcado, mostrar la fila
                    $('#<%= Combos.ClientID %>').show();
                    $('#<%= sustituirCombos.ClientID %>').hide();
                }
            });

            // Verificar el estado inicial del RadioButton al cargar la página
            if ($('#<%= RadioButtonNombreSi.ClientID %>').is(':checked')) {
                // El RadioButton está marcado, ocultar la fila
                $('#<%= Combos.ClientID %>').hide();
                $('#<%= sustituirCombos.ClientID %>').show();
            } else {
                // El RadioButton no está marcado, mostrar la fila
                $('#<%= Combos.ClientID %>').show();
                $('#<%= sustituirCombos.ClientID %>').hide();
            }
        });

        $(document).ready(function () {
            // Function to add the code
            function RBNo() {
                $('#<%= RadioButtonNombreNo.ClientID %>').on('change', function () {
                    if ($(this).is(':checked')) {
                        $('#<%= TxtNombreR.ClientID %>').val("");
                        $('#<%= TxtApellidoR.ClientID %>').val("");
                        $('#<%= TxtCasadaR.ClientID %>').val("");
                        $('#<%= txtNit.ClientID %>').val("");
                        $('#<%= TxtDiRe1.ClientID %>').val("");
                        $('#<%= TxtDiRe2.ClientID %>').val("");
                        $('#<%= TxtDiRe3.ClientID %>').val("");
                        $('#<%= CmbPaisNIT.ClientID %>').val("");
                        $('#<%= CmbDepartamentoNIT.ClientID %>').val("");
                        $('#<%= CmbMunicipioNIT.ClientID %>').val("");
                        $('#<%= StateNIT.ClientID %>').val("");
                        $('#<%= txtNit.ClientID %>').prop('disabled', false);
                        $('#<%= ValidarNIT.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe1.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe2.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe3.ClientID %>').prop('disabled', false);
                        $('#<%= CmbPaisNIT.ClientID %>').prop('disabled', false);
                        $('#<%= CmbDepartamentoNIT.ClientID %>').prop('disabled', false);
                        $('#<%= CmbMunicipioNIT.ClientID %>').prop('disabled', false);
                        $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPaisNIT.ClientID %>').val());
                        // Hacer visible la fila Combos
                        $('#<%= Combos.ClientID %>').show();
                        $('#<%= sustituirCombos.ClientID %>').hide();
                        llenadoPaisnit();
                        llenadoDepartamentoNit();
                        llenadoMunicipioNIT();
                    }
                });
            }

            // Call the function
            RBNo();

        });

        $('.close').click(function () {
            $('#myModal').css('display', 'none');
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO NOMBRE EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtNombre.ClientID %>').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtNombreR.ClientID %>').val($('#<%= txtNombre.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO APELLIDO EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtApellido.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtApellidoR.ClientID %>').val($('#<%= txtApellido.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO APELLIDO DE CASADA EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtCasada.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtCasadaR.ClientID %>').val($('#<%= txtCasada.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 1 EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtDireccion.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtDiRe1.ClientID %>').val($('#<%= txtDireccion.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 2 EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtDireccion2.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtDiRe2.ClientID %>').val($('#<%= txtDireccion2.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 3 EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtDireccion3.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= TxtDiRe3.ClientID %>').val($('#<%= txtDireccion3.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO PAIS EN EL RECIBO
        $(document).ready(function () {
            $('#<%= CmbPaisNIT.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked')) {
                    $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPaisNIT.ClientID %>').val());
                }
            });
        });

        function VerificarCantidadTelefono(sender, args) {
            args.IsValid = (args.Value.length >= 7);
        }

        function checkCameraAccess() {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (stream) {
                    document.getElementById('<%= hdnCameraAvailable.ClientID %>').value = 'true';
                    stream.getTracks().forEach(function (track) {
                        track.stop();
                    });
                })
                .catch(function (error) {
                });
        }

        function validarCargaArchivos() {
            var fileUpload = document.getElementById('<%= FileUpload2.ClientID %>');
            var files = fileUpload.files;

            if (files.length > 2) {
                alert("Solo se permiten cargar 2 archivos.");
                // Eliminar los archivos adicionales
                while (files.length > 2) {
                    fileUpload.remove(files.length - 1);
                    fileUpload.value = "";
                }
            }
        }

        //FUNCION PARA EVITAR QUE SEA INGRESADO EL -
        $(document).ready(function () {
            $('#<%= txtNit.ClientID %>').on('keypress', function (event) {
                var keyCode = event.which || event.keyCode;
                var character = String.fromCharCode(keyCode);
                if (character === '-') {
                    event.preventDefault();
                    alert('No se permite el ingreso del guín (-)');
                }
            });
        });

        //Valdar Numeros de telefono
        $(document).ready(function () {
            $('#<%= txtTelefono.ClientID %>').on('input', function () {
                var inputValue = $(this).val();
                var numericValue = inputValue.replace(/[^0-9]/g, '');
                $(this).val(numericValue);
            });

            $('#<%= txtTelefono.ClientID %>').on('keypress', function (event) {
                var keyCode = event.which || event.keyCode;
                var character = String.fromCharCode(keyCode);

                if (!/^[0-9]+$/.test(character)) {
                    event.preventDefault();
                    alert('Solo se permiten ingresar números.');
                }
            });
        });

        //Detectar cambio de nit
        $(document).ready(function () {
            $('#<%= txtNit.ClientID %>').on('input', function () {
                var txtNit = $('#<%= txtNit.ClientID %>').val().trim();
                var TrueNit = $('#<%= TrueNit.ClientID %>').val().trim();
                var labelValidacion = $('#<%= ValidacionNit.ClientID %>').val().trim();
                if (txtNit !== TrueNit || txtNit !== 'CF') {
                    labelValidacion.text("1");
                } else {
                    labelValidacion.text("0");
                    TrueNit.text(txtNit);
                }
            });
        });

        document.addEventListener("DOMContentLoaded", function () {
            // Obtenemos el elemento de video y la imagen en JavaScript
            const videoElement = document.getElementById("videoElement");
            const imgBase = document.getElementById("<%= ImgBase.ClientID %>");
            const canvas = document.getElementById("canvas");
            const ctx = canvas.getContext("2d");

            // Cuando el video esté listo, obtenemos las dimensiones y las aplicamos a la imagen y el canvas
            videoElement.onloadedmetadata = function () {
                const videoWidth = videoElement.videoWidth;
                const videoHeight = videoElement.videoHeight;

                const maxWidth = 375;
                const maxHeight = 275;

                // Calculamos las dimensiones para la imagen considerando la relación de aspecto
                let newWidth, newHeight;
                if (videoWidth / videoHeight > maxWidth / maxHeight) {
                    newWidth = maxWidth;
                    newHeight = videoHeight * (maxWidth / videoWidth);
                } else {
                    newHeight = maxHeight;
                    newWidth = videoWidth * (maxHeight / videoHeight);
                }

                // Aplicamos las dimensiones a la imagen
                imgBase.style.width = newWidth + "px";
                imgBase.style.height = newHeight + "px";

                // Aplicamos las dimensiones al canvas
                canvas.style.width = newWidth + "px";
                canvas.style.height = newHeight + "px";
                canvas.width = newWidth;
                canvas.height = newHeight;
            };
        });

        function mostrarModalEspera() {
            var modal = document.getElementById("myModalEspera");
            modal.style.display = "block";
        }

        function ocultarModalEspera() {
            var modal = document.getElementById("myModalEspera");
            modal.style.display = "none";
        }

        function ocultarModalActualizacion() {
            var modal = document.getElementById("myModalActualizacion");
            modal.style.display = "none";
        }

        function mostrarModalCorrecto() {
            var modal = document.getElementById("myModalCorrecto");
            modal.style.display = "block";
            var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
            lblActualizacion.html("");

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "ActualizacionEstudiantes.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        //evitar enter
        function evitarEnter(e) {
            if (e.keyCode == 13) {
                e.preventDefault(); // Evitar que se realice una nueva línea
                return false;
            }
            return true;
        }

    </script>


    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>

